using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CT.Utilities
{
    using Data;
    using Data.Save;
    using Elements;
    using ScriptableObjects;
    using Windows;

    public static class CTIOUtility
    {
        private static CTGraphView graph_view;

        private static string graph_file_name;
        private static string container_folder_path;

        private static List<CTNode> nodes;
        private static List<CTGroup> groups;

        private static Dictionary<string, CTDialogueGroupSO> created_dlog_groups;
        private static Dictionary<string, CTDialogueSO> created_dlogs;

        private static Dictionary<string, CTGroup> loaded_groups;
        private static Dictionary<string, CTNode> loaded_nodes;

        public static void Initialize(CTGraphView _ct_graph_view, string _graph_name)
        {
            graph_view = _ct_graph_view;

            graph_file_name = _graph_name;
            container_folder_path = $"Assets/DialogueSystem/Dialogues/{_graph_name}";

            nodes = new List<CTNode>();
            groups = new List<CTGroup>();

            created_dlog_groups = new Dictionary<string, CTDialogueGroupSO>();
            created_dlogs = new Dictionary<string, CTDialogueSO>();

            loaded_groups = new Dictionary<string, CTGroup>();
            loaded_nodes = new Dictionary<string, CTNode>();
        }

        public static void Save()
        {
            CreateDefaultFolders();

            GetElementsFromGraphView();

            CTGraphSaveDataSO graph_data = CreateAsset<CTGraphSaveDataSO>("Assets/Editor/DialogueSystem/Graphs", $"{graph_file_name}Graph");

            graph_data.Initialize(graph_file_name);

            CTDialogueContainerSO dlog_container = CreateAsset<CTDialogueContainerSO>(container_folder_path, graph_file_name);

            dlog_container.Initialize(graph_file_name);

            SaveGroups(graph_data, dlog_container);
            SaveNodes(graph_data, dlog_container);

            SaveAsset(graph_data);
            SaveAsset(dlog_container);
        }

        private static void SaveGroups(CTGraphSaveDataSO _graph_data, CTDialogueContainerSO _dlog_container)
        {
            List<string> group_names = new List<string>();

            foreach (CTGroup group in groups)
            {
                SaveGroupToGraph(group, _graph_data);
                SaveGroupToScriptableObject(group, _dlog_container);

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

        private static void SaveGroupToScriptableObject(CTGroup _group, CTDialogueContainerSO _dlog_container)
        {
            string group_name = _group.title;

            CreateFolder($"{container_folder_path}/Groups", group_name);
            CreateFolder($"{container_folder_path}/Groups/{group_name}", "Dialogues");

            CTDialogueGroupSO dlog_group = CreateAsset<CTDialogueGroupSO>($"{container_folder_path}/Groups/{group_name}", group_name);

            dlog_group.Initialize(group_name);

            created_dlog_groups.Add(_group.ID, dlog_group);

            _dlog_container.DialogueGroups.Add(dlog_group, new List<CTDialogueSO>());

            SaveAsset(dlog_group);
        }

        private static void UpdateOldGroups(List<string> _current_group_names, CTGraphSaveDataSO _graph_data)
        {
            if (_graph_data.OldGroupNames != null && _graph_data.OldGroupNames.Count != 0)
            {
                List<string> groups_for_removal = _graph_data.OldGroupNames.Except(_current_group_names).ToList();

                foreach (string groupToRemove in groups_for_removal)
                {
                    RemoveFolder($"{container_folder_path}/Groups/{groupToRemove}");
                }
            }

            _graph_data.OldGroupNames = new List<string>(_current_group_names);
        }

        private static void SaveNodes(CTGraphSaveDataSO _graph_data, CTDialogueContainerSO _dialogue_container)
        {
            SerializableDictionary<string, List<string>> _grouped_node_names = new SerializableDictionary<string, List<string>>();
            List<string> _ungrouped_node_names = new List<string>();

            foreach (CTNode node in nodes)
            {
                SaveNodeToGraph(node, _graph_data);
                SaveNodeToScriptableObject(node, _dialogue_container);

                if (node.Group != null)
                {
                    _grouped_node_names.AddItem(node.Group.title, node.DialogueName);

                    continue;
                }

                _ungrouped_node_names.Add(node.DialogueName);
            }

            UpdateDialoguesChoicesConnections();

            UpdateOldGroupedNodes(_grouped_node_names, _graph_data);
            UpdateOldUngroupedNodes(_ungrouped_node_names, _graph_data);
        }

        private static void SaveNodeToGraph(CTNode _node, CTGraphSaveDataSO _graph_data)
        {
            List<CTChoiceSaveData> choices = CloneNodeChoices(_node.Choices);

            CTNodeSaveData node_data = new CTNodeSaveData()
            {
                ID = _node.ID,
                Name = _node.DialogueName,
                Choices = choices,
                Text = _node.Text,
                TipText = _node.TipText,
                GroupID = _node.Group?.ID,
                DialogueType = _node.DialogueType,
                Position = _node.GetPosition().position
            };

            _graph_data.Nodes.Add(node_data);
        }

        private static void SaveNodeToScriptableObject(CTNode _node, CTDialogueContainerSO _dlog_container)
        {
            CTDialogueSO dialogue;

            if (_node.Group != null)
            {
                dialogue = CreateAsset<CTDialogueSO>($"{container_folder_path}/Groups/{_node.Group.title}/Dialogues", _node.DialogueName);

                _dlog_container.DialogueGroups.AddItem(created_dlog_groups[_node.Group.ID], dialogue);
            }
            else
            {
                dialogue = CreateAsset<CTDialogueSO>($"{container_folder_path}/Global/Dialogues", _node.DialogueName);

                _dlog_container.UngroupedDialogues.Add(dialogue);
            }

            dialogue.Initialize(
                _node.DialogueName,
                _node.Text,
                _node.TipText,
                ConvertNodeChoicesToDialogueChoices(_node.Choices),
                _node.DialogueType,
                _node.IsStartingNode()
            );

            created_dlogs.Add(_node.ID, dialogue);

            SaveAsset(dialogue);
        }

        private static List<CTDialogueChoiceData> ConvertNodeChoicesToDialogueChoices(List<CTChoiceSaveData> _node_choices)
        {
            List<CTDialogueChoiceData> _dlog_choices = new List<CTDialogueChoiceData>();

            foreach (CTChoiceSaveData node_choice in _node_choices)
            {
                CTDialogueChoiceData choice_data = new CTDialogueChoiceData()
                {
                    Text = node_choice.Text
                };

                _dlog_choices.Add(choice_data);
            }

            return _dlog_choices;
        }

        private static void UpdateDialoguesChoicesConnections()
        {
            foreach (CTNode node in nodes)
            {
                CTDialogueSO dialogue = created_dlogs[node.ID];

                for (int choice_index = 0; choice_index < node.Choices.Count; ++choice_index)
                {
                    CTChoiceSaveData node_choice = node.Choices[choice_index];

                    if (string.IsNullOrEmpty(node_choice.NodeID))
                    {
                        continue;
                    }

                    dialogue.Choices[choice_index].NextDialogue = created_dlogs[node_choice.NodeID];

                    SaveAsset(dialogue);
                }
            }
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

            _graph_data.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(_current_grouped_node_names);
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

        public static void Load()
        {
            CTGraphSaveDataSO graph_data = LoadAsset<CTGraphSaveDataSO>("Assets/Editor/DialogueSystem/Graphs", graph_file_name);

            if (graph_data == null)
            {
                EditorUtility.DisplayDialog(
                    "Could not load the requested file!",
                    "The file at the following path could not be found:\n\n" +
                    $"\"Assets/Editor/DialogueSystem/Graphs/{graph_file_name}\".\n\n" +
                    "Ensure you chose the right file type from the folder path mentioned above.",
                    "Okay"
                );

                return;
            }

            CTEditorWindow.UpdateFileName(graph_data.FileName);

            LoadGroups(graph_data.Groups);
            LoadNodes(graph_data.Nodes);
            LoadNodesConnections();
        }

        private static void LoadGroups(List<CTGroupSaveData> _groups)
        {
            foreach (CTGroupSaveData group_data in _groups)
            {
                CTGroup group = graph_view.CreateGroup(group_data.Name, group_data.Position);

                group.ID = group_data.ID;

                loaded_groups.Add(group.ID, group);
            }
        }

        private static void LoadNodes(List<CTNodeSaveData> nodes)
        {
            foreach (CTNodeSaveData node_data in nodes)
            {
                List<CTChoiceSaveData> choices = CloneNodeChoices(node_data.Choices);

                CTNode node = graph_view.CreateNode(node_data.Name, node_data.DialogueType, node_data.Position, false);

                node.ID = node_data.ID;
                node.Choices = choices;
                node.Text = node_data.Text;
                node.TipText = node_data.TipText;

                node.Draw();

                graph_view.AddElement(node);

                loaded_nodes.Add(node.ID, node);

                if (string.IsNullOrEmpty(node_data.GroupID))
                {
                    continue;
                }

                CTGroup group = loaded_groups[node_data.GroupID];

                node.Group = group;

                group.AddElement(node);
            }
        }

        private static void LoadNodesConnections()
        {
            foreach (KeyValuePair<string, CTNode> loaded_node in loaded_nodes)
            {
                foreach (Port choice_port in loaded_node.Value.outputContainer.Children())
                {
                    CTChoiceSaveData choice_data = (CTChoiceSaveData) choice_port.userData;

                    if (string.IsNullOrEmpty(choice_data.NodeID))
                    {
                        continue;
                    }

                    CTNode next_node = loaded_nodes[choice_data.NodeID];

                    Port next_node_input_port = (Port) next_node.inputContainer.Children().First();

                    Edge edge = choice_port.ConnectTo(next_node_input_port);

                    graph_view.AddElement(edge);

                    loaded_node.Value.RefreshPorts();
                }
            }
        }

        private static void CreateDefaultFolders()
        {
            CreateFolder("Assets/Editor/DialogueSystem", "Graphs");

            CreateFolder("Assets", "DialogueSystem");
            CreateFolder("Assets/DialogueSystem", "Dialogues");

            CreateFolder("Assets/DialogueSystem/Dialogues", graph_file_name);
            CreateFolder(container_folder_path, "Global");
            CreateFolder(container_folder_path, "Groups");
            CreateFolder($"{container_folder_path}/Global", "Dialogues");
        }

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

        public static void CreateFolder(string _parent_folder_path, string _new_folder_name)
        {
            if (AssetDatabase.IsValidFolder($"{_parent_folder_path}/{_new_folder_name}"))
            {
                return;
            }

            AssetDatabase.CreateFolder(_parent_folder_path, _new_folder_name);
        }

        public static void RemoveFolder(string _path)
        {
            FileUtil.DeleteFileOrDirectory($"{_path}.meta");
            FileUtil.DeleteFileOrDirectory($"{_path}/");
        }

        public static _T CreateAsset<_T>(string _path, string _asset_name) where _T : ScriptableObject
        {
            string full_path = $"{_path}/{_asset_name}.asset";

            _T asset = LoadAsset<_T>(_path, _asset_name);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<_T>();

                AssetDatabase.CreateAsset(asset, full_path);
            }

            return asset;
        }

        public static _T LoadAsset<_T>(string _path, string assetName) where _T : ScriptableObject
        {
            string full_path = $"{_path}/{assetName}.asset";

            return AssetDatabase.LoadAssetAtPath<_T>(full_path);
        }

        public static void SaveAsset(UnityEngine.Object _asset)
        {
            EditorUtility.SetDirty(_asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void RemoveAsset(string _path, string _asset_name)
        {
            AssetDatabase.DeleteAsset($"{_path}/{_asset_name}.asset");
        }

        private static List<CTChoiceSaveData> CloneNodeChoices(List<CTChoiceSaveData> _node_choices)
        {
            List<CTChoiceSaveData> choices = new List<CTChoiceSaveData>();

            foreach (CTChoiceSaveData choice in _node_choices)
            {
                CTChoiceSaveData choice_data = new CTChoiceSaveData()
                {
                    Text = choice.Text,
                    NodeID = choice.NodeID
                };

                choices.Add(choice_data);
            }

            return choices;
        }
    }
}