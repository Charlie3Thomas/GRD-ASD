using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CT.Utilities
{    
    using Windows;
    using Elements;
    using Data.Save;
    using ScriptableObjects;
    using Data;
    using System.Security.Permissions;
    using NUnit.Framework;

    public static class CTIOUtility
    {
        public static CTGraphView graph_view;

        private static string graph_filename;
        private static string container_folder_path;

        private static List<CTGroup> groups;
        private static List<CTNode> nodes;

        private static Dictionary<string, CTDialogueGroupSO> created_dialogue_groups;
        private static Dictionary<string, CTDialogueSO> created_dialogues;

        public static void Initialise(CTGraphView _ct_graph_view, string _graph_name)
        {
            graph_view = _ct_graph_view;
            graph_filename = _graph_name;
            container_folder_path = $"Assets/DialogueSystem/Dialogues/{_graph_name}";

            groups = new List<CTGroup>();
            nodes = new List<CTNode>();

            created_dialogue_groups = new Dictionary<string, CTDialogueGroupSO>();
            created_dialogues = new Dictionary<string, CTDialogueSO>();
        }

        #region Save Methods
        // Save method
        public static void Save()
        {
            // Write to folders
            CreateStaticFolders();
            GetElementsFromGraphView();
            CTGraphSaveDataSO graph_data = CreateAsset<CTGraphSaveDataSO>("Assets/Editor/DialogueSystem/Graphs", $"{graph_filename}Graph");
            graph_data.Initialise(graph_filename);
            CTDialogueContainerSO dialogue_container = CreateAsset<CTDialogueContainerSO>(container_folder_path, graph_filename);
            dialogue_container.Initialise(graph_filename);
            SaveGroups(graph_data, dialogue_container);
            SaveNodes(graph_data, dialogue_container);

            // Unity save
            SaveAssets(graph_data);
            SaveAssets(dialogue_container);
        }


        #region Groups
        private static void SaveGroups(CTGraphSaveDataSO _graph_data, CTDialogueContainerSO _dialogue_container)
        {
            List<string> group_names = new List<string>();

            foreach (CTGroup group in groups)
            {
                SaveGroupToGraph(group, _graph_data);
                SaveGroupToSO(group, _dialogue_container);

                group_names.Add(group.title);
            }

            UpdateOldGroups(group_names, _graph_data);
        }

        private static void SaveGroupToGraph(CTGroup _group, CTGraphSaveDataSO _graph_data)
        {
            CTGroupSaveData group_data = new CTGroupSaveData()
            {
                ID = _group.ID,
                Name = _group.title,
                Position = _group.GetPosition().position
            };

            _graph_data.Groups.Add(group_data);

        }

        private static void SaveGroupToSO(CTGroup _group, CTDialogueContainerSO _dialogue_container)
        {
            string group_name = _group.title;
            CreateFolder($"{container_folder_path}/Groups", group_name);
            CreateFolder($"{container_folder_path}/Groups/{ group_name }", "Dialogues");
            CTDialogueGroupSO dialogue_group = CreateAsset<CTDialogueGroupSO>($"{container_folder_path}/Groups/{group_name}", group_name);
            dialogue_group.Initialise(group_name);
            created_dialogue_groups.Add(_group.ID, dialogue_group);
            _dialogue_container.DialogueGroups.Add(dialogue_group, new List<CTDialogueSO>());

            // Perform unity asset save to prevent data loss when not manually saving
            SaveAssets(dialogue_group);
        }

        private static void UpdateOldGroups(List<string> _current_group_names, CTGraphSaveDataSO _graph_data)
        {
            /*
                Compare old and new group names
                Any old group names that are not present in new group names should be deleted
             */

            if (_graph_data.OldGroupNames != null && _graph_data.OldGroupNames.Count != 0)
            {
                List<string> groups_to_remove = _graph_data.OldGroupNames.Except(_current_group_names).ToList();

                foreach (string group_to_remove in groups_to_remove)
                {
                    RemoveFolder($"{container_folder_path}/Groups/{group_to_remove}");
                }
            }

            _graph_data.OldGroupNames = new List<string>(_current_group_names);
        }




        #endregion

        #region Nodes
        private static void SaveNodes(CTGraphSaveDataSO _graph_data, CTDialogueContainerSO _dialogue_container)
        {
            SerializableDictionary<string, List<string>> grouped_node_names = new SerializableDictionary<string, List<string>>();
            List<string> ungrouped_node_names = new List<string>();

            foreach (CTNode node in nodes) 
            {
                SaveNodeToGraph(node, _graph_data);
                SaveNodeToSO(node, _dialogue_container);

                if (node.group != null)
                {
                    grouped_node_names.AddItem(node.group.title, node.DialogueName);
                    continue;
                }

                ungrouped_node_names.Add(node.DialogueName);
            }

            UpdateDialogueChoicesConnections();
            UpdateOldGroupedNodes(grouped_node_names, _graph_data);
            UpdateOldUngroupedNodes(ungrouped_node_names, _graph_data);
        }

        private static void SaveNodeToGraph(CTNode _node, CTGraphSaveDataSO _graph_data)
        {
            /*
                Iterate through original list
                Set variable values to be same as current List
                Add thew variable to new list
             */
            List<CTChoiceSaveData> choices = new List<CTChoiceSaveData>();

            foreach (CTChoiceSaveData choice in _node.Choices)
            {
                CTChoiceSaveData choice_data = new CTChoiceSaveData()
                {
                    // Typed data
                    Text = choice.Text,
                    NodeID = choice.NodeID
                };

                choices.Add(choice_data);
            }



            CTNodeSaveData node_data = new CTNodeSaveData()
            {
                ID = _node.ID,
                Name = _node.name,
                Choices = choices,
                Text = _node.Text,
                TipText = _node.TipText,

                // _node.group can be null.
                // use null-conditional operator to prevent attempt to read from null
                GroupID = _node.group?.ID,
                DialogueType = _node.DialogueType,
                Position = _node.GetPosition().position
            };

            _graph_data.Nodes.Add(node_data);

        }

        private static void SaveNodeToSO(CTNode _node, CTDialogueContainerSO _dialogue_container)
        {
            CTDialogueSO dialogue;

            // Grouped
            if (_node.group != null) 
            {
                dialogue = CreateAsset<CTDialogueSO>($"{container_folder_path}/Groups/{_node.group.title}/Dialogues", _node.DialogueName);
                _dialogue_container.DialogueGroups.AddItem(created_dialogue_groups[_node.group.ID], dialogue);
            }
            // Ungrouped
            else
            {
                dialogue = CreateAsset<CTDialogueSO>($"{container_folder_path}/Global/Dialogues", _node.DialogueName);
                _dialogue_container.UngroupedDialogues.Add(dialogue);
            }

            dialogue.Initialise(
                _node.DialogueName,
                _node.Text,
                _node.TipText,
                ConvertChoiceType(_node.Choices),
                _node.DialogueType,
                _node.IsStartNode()
            );

            created_dialogues.Add(_node.ID, dialogue);
            SaveAssets(dialogue);

        }

        private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> _current_grouped_node_names, CTGraphSaveDataSO _graph_data)
        {
            if (_graph_data.OldGroupedNodeNames != null && _graph_data.OldGroupedNodeNames.Count != 0)
            {
                foreach (KeyValuePair<string, List<string>> old_grouped_node in _graph_data.OldGroupedNodeNames)
                {
                    List<string> nodes_to_remove = new List<string>();

                    if (_current_grouped_node_names.ContainsKey(old_grouped_node.Key))
                    {
                        nodes_to_remove = old_grouped_node.Value.Except(_current_grouped_node_names[old_grouped_node.Key]).ToList();
                    }

                    foreach (string node_to_remove in nodes_to_remove)
                    {
                        RemoveAsset($"{container_folder_path}/Groups/{old_grouped_node.Key}/Dialogues", node_to_remove);
                    }
                }
            }
        }

        private static void UpdateOldUngroupedNodes(List<string> _current_ungrouped_node_names, CTGraphSaveDataSO _graph_data)
        {
            if (_graph_data.OldUngroupedNodeNames != null && _graph_data.OldUngroupedNodeNames.Count != 0)
            {
                List<string> nodes_to_remove = _graph_data.OldUngroupedNodeNames.Except(_current_ungrouped_node_names).ToList();

                foreach (string node_to_remove in nodes_to_remove)
                {
                    RemoveAsset($"{container_folder_path}/Global/Dialogues", node_to_remove);
                }
            }

            _graph_data.OldUngroupedNodeNames = new List<string>(_current_ungrouped_node_names);

        }



        private static List<CTDialogueChoiceData> ConvertChoiceType(List<CTChoiceSaveData> _node_choices)
        {
            List<CTDialogueChoiceData> dialogue_choices = new List<CTDialogueChoiceData>();

            foreach (CTChoiceSaveData node_choice in _node_choices) 
            {
                CTDialogueChoiceData choice_data = new CTDialogueChoiceData()
                {
                    Text = node_choice.Text,
                    TipText= node_choice.TipText
                };

                dialogue_choices.Add(choice_data);
            }

            return dialogue_choices;
        }

        private static void UpdateDialogueChoicesConnections()
        {
            foreach (CTNode node in nodes)
            {
                CTDialogueSO dialogue = created_dialogues[node.ID];

                for (int choice_index = 0; choice_index < node.Choices.Count; choice_index++)
                {
                    CTChoiceSaveData node_choice = node.Choices[choice_index];

                    // Check if empty
                    if (string.IsNullOrEmpty(node_choice.NodeID))
                    {
                        continue;
                    }

                    dialogue.Choices[choice_index].NextDialogue = created_dialogues[node_choice.NodeID];

                    SaveAssets(dialogue);
                }
            }
        }

        #endregion

        #endregion

        #region Creation Methods
        private static void CreateStaticFolders()
        {
            CreateFolder("Assets/Editor/DialogueSystem", "Graphs");
            CreateFolder("Assets", "DialogueSystem");
            CreateFolder("Assets/DialogueSystem", "Dialogues");
            CreateFolder("Assets/DialogueSystem/Dialogues", graph_filename);
            CreateFolder(container_folder_path, "Global");
            CreateFolder(container_folder_path, "Groups");
            CreateFolder($"{container_folder_path}/Global", "Dialogues");
        }



        #endregion

        #region Get
        private static void GetElementsFromGraphView()
        {
            Type group_type = typeof(CTGroup);

            graph_view.graphElements.ForEach(graph_element =>
            {
                if (graph_element is CTNode node)
                {
                    nodes.Add(node);
                    return;
                }

                if (graph_element.GetType() == group_type)
                {
                    CTGroup group = (CTGroup) graph_element;
                    groups.Add(group);
                    return;
                }
            });
        }


        #endregion

        #region Utility
        private static void CreateFolder(string _path, string _folder_name)
        {
            // Exit if folder already exists
            if (AssetDatabase.IsValidFolder($"{_path}/{_folder_name}"))
            {
                return;
            }

            // Create folder
            AssetDatabase.CreateFolder(_path, _folder_name);

            //Debug.Log("Creating folder");
        }
        private static void RemoveFolder(string _fullpath)
        {
            FileUtil.DeleteFileOrDirectory($"{_fullpath}.meta");
            FileUtil.DeleteFileOrDirectory($"{_fullpath}/");
        }

        // Create Asset of any type that inherits from the ScriptableObject class
        private static _T CreateAsset<_T>(string _path, string _name) where _T : ScriptableObject
        {
            string full_path = $"{_path}/{_name}.asset";

            _T asset = AssetDatabase.LoadAssetAtPath<_T>(full_path);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<_T>();

                AssetDatabase.CreateAsset(asset, full_path);
            }

            return asset;
        }

        private static void RemoveAsset(string _path, string  _asset_name)
        {
            AssetDatabase.DeleteAsset($"{_path}/{_asset_name}.asset");
        }

        private static void SaveAssets(UnityEngine.Object _asset)
        {
            // Set assets as "dirty"
            EditorUtility.SetDirty(_asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }



        #endregion
    }
}
