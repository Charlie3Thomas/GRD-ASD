using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CT.Utils
{
    using Data;
    using Data.Save;
    using Components;
    using ScriptableObjects;
    using Windows;
    using System.Xml.Linq;

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

        public static void Initialise(CTGraphView _ct_graph_view, string _graph_name)
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

        #region Saving

        public static void Save()
        {
            CreateDefaultFolders();

            GetElementsFromGraphView();

            CTGraphSaveDataSO graph_data = CreateAsset<CTGraphSaveDataSO>("Assets/Editor/DialogueSystem/Graphs", $"{graph_file_name}Graph");

            graph_data.Initialise(graph_file_name);

            CTDialogueContainerSO dlog_container = CreateAsset<CTDialogueContainerSO>(container_folder_path, graph_file_name);

            dlog_container.Initialise(graph_file_name);

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
                title = _group.title,
                pos = _group.GetPosition().position
            };

            _graph_data.list_groups.Add(group_data);
        }

        private static void SaveGroupToScriptableObject(CTGroup _group, CTDialogueContainerSO _dlog_container)
        {
            string group_name = _group.title;

            CreateFolder($"{container_folder_path}/Groups", group_name);
            CreateFolder($"{container_folder_path}/Groups/{group_name}", "Dialogues");

            CTDialogueGroupSO dlog_group = CreateAsset<CTDialogueGroupSO>($"{container_folder_path}/Groups/{group_name}", group_name);

            dlog_group.Initialise(group_name);

            created_dlog_groups.Add(_group.ID, dlog_group);

            _dlog_container.dictionary_dlog_groups.Add(dlog_group, new List<CTDialogueSO>());

            SaveAsset(dlog_group);
        }

        private static void SaveNodes(CTGraphSaveDataSO _graph_data, CTDialogueContainerSO _dialogue_container)
        {
            SerializableDictionary<string, List<string>> grouped_node_names = new SerializableDictionary<string, List<string>>();
            List<string> ungrouped_node_names = new List<string>();

            foreach (CTNode node in nodes)
            {
                SaveNodeToGraph(node, _graph_data);
                SaveNodeToScriptableObject(node, _dialogue_container);

                if (node.group != null)
                {
                    grouped_node_names.AddItem(node.group.title, node.dlog_title);

                    continue;
                }

                ungrouped_node_names.Add(node.dlog_title);
            }

            UpdateDialoguesChoicesConnections();

            UpdateOldGroupedNodes(grouped_node_names, _graph_data);
            UpdateOldUngroupedNodes(ungrouped_node_names, _graph_data);
        }

        private static void SaveNodeToGraph(CTNode _node, CTGraphSaveDataSO _graph_data)
        {
            List<CTChoiceSaveData> choices = CloneNodeChoices(_node.list_dlog_choices);

            CTNodeSaveData node_data = new CTNodeSaveData()
            {
                ID = _node.ID,
                title = _node.dlog_title,
                list_choices = choices,
                dlog_text = _node.dlog_text,
                dlog_tip_text = _node.dlog_tip_text,
                group_ID = _node.group?.ID,
                character = _node.active_character,
                background = _node.active_bg,
                char_dropdown_index = _node.char_dropdown_index,
                bg_dropdown_index = _node.bg_dropdown_index,
                dlog_type = _node.dlog_type,
                pos = _node.GetPosition().position
            };

            _graph_data.list_nodes.Add(node_data);
        }

        private static void SaveNodeToScriptableObject(CTNode _node, CTDialogueContainerSO _dlog_container)
        {
            CTDialogueSO dialogue;

            if (_node.group != null)
            {
                dialogue = CreateAsset<CTDialogueSO>($"{container_folder_path}/Groups/{_node.group.title}/Dialogues", _node.dlog_title);

                _dlog_container.dictionary_dlog_groups.AddItem(created_dlog_groups[_node.group.ID], dialogue);
            }
            else
            {
                dialogue = CreateAsset<CTDialogueSO>($"{container_folder_path}/Global/Dialogues", _node.dlog_title);

                _dlog_container.list_ungrouped_dlogs.Add(dialogue);
            }

            dialogue.Initialise(
                _node.dlog_title,
                _node.dlog_text,
                _node.dlog_tip_text,
                ConvertNodeChoicesToDialogueChoices(_node.list_dlog_choices),
                _node.dlog_type,
                _node.active_character,
                _node.char_dropdown_index,
                _node.active_bg,
                _node.bg_dropdown_index,
                _node.IsStartingNode()
            );

            created_dlogs.Add(_node.ID, dialogue);

            SaveAsset(dialogue);
        }

        #endregion


        #region Changes
        private static List<CTDialogueChoiceData> ConvertNodeChoicesToDialogueChoices(List<CTChoiceSaveData> _node_choices)
        {
            List<CTDialogueChoiceData> _dlog_choices = new List<CTDialogueChoiceData>();

            foreach (CTChoiceSaveData node_choice in _node_choices)
            {
                CTDialogueChoiceData choice_data = new CTDialogueChoiceData()
                {
                    text = node_choice.text
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

                for (int choice_index = 0; choice_index < node.list_dlog_choices.Count; ++choice_index)
                {
                    CTChoiceSaveData node_choice = node.list_dlog_choices[choice_index];

                    if (string.IsNullOrEmpty(node_choice.node_ID))
                    {
                        continue;
                    }

                    dialogue.list_dlog_choices[choice_index].next_dlog_node = created_dlogs[node_choice.node_ID];

                    SaveAsset(dialogue);
                }
            }
        }

        private static void UpdateOldGroups(List<string> _current_group_names, CTGraphSaveDataSO _graph_data)
        {
            if (_graph_data.list_previous_group_titles != null && _graph_data.list_previous_group_titles.Count != 0)
            {
                List<string> groups_for_removal = _graph_data.list_previous_group_titles.Except(_current_group_names).ToList();

                foreach (string groupToRemove in groups_for_removal)
                {
                    RemoveFolder($"{container_folder_path}/Groups/{groupToRemove}");
                }
            }

            _graph_data.list_previous_group_titles = new List<string>(_current_group_names);
        }

        private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> _current_grouped_node_names, CTGraphSaveDataSO _graph_data)
        {
            if (_graph_data.list_previous_grouped_titles != null && _graph_data.list_previous_grouped_titles.Count != 0)
            {
                foreach (KeyValuePair<string, List<string>> old_grouped_node in _graph_data.list_previous_grouped_titles)
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

            _graph_data.list_previous_grouped_titles = new SerializableDictionary<string, List<string>>(_current_grouped_node_names);
        }

        private static void UpdateOldUngroupedNodes(List<string> _current_ungrouped_node_names, CTGraphSaveDataSO _graph_data)
        {
            if (_graph_data.list_previous_ungrouped_titles != null && _graph_data.list_previous_ungrouped_titles.Count != 0)
            {
                List<string> nodes_to_remove = _graph_data.list_previous_ungrouped_titles.Except(_current_ungrouped_node_names).ToList();

                foreach (string node_to_remove in nodes_to_remove)
                {
                    RemoveAsset($"{container_folder_path}/Global/Dialogues", node_to_remove);
                }
            }

            _graph_data.list_previous_ungrouped_titles = new List<string>(_current_ungrouped_node_names);
        }

        #endregion


        #region Loading

        public static void Load()
        {
            CTGraphSaveDataSO graph_data = LoadAsset<CTGraphSaveDataSO>("Assets/Editor/DialogueSystem/Graphs", graph_file_name);

            if (graph_data == null)
            {
                EditorUtility.DisplayDialog(
                    "Could not load the graph data!",
                    "Either the wrong file was chosen, or the file does not exist at that path.",
                    "Okay"
                );

                return;
            }

            CTEditorWindow.UpdateFileName(graph_data.file_name);

            LoadGroups(graph_data.list_groups);
            LoadNodes(graph_data.list_nodes);
            LoadNodesConnections();
        }

        private static void LoadGroups(List<CTGroupSaveData> _groups)
        {
            foreach (CTGroupSaveData group_data in _groups)
            {
                CTGroup group = graph_view.CreateGroup(group_data.title, group_data.pos);

                group.ID = group_data.ID;

                loaded_groups.Add(group.ID, group);

                Debug.Log("Loaded Group");
            }
        }

        private static void LoadNodes(List<CTNodeSaveData> nodes)
        {
            foreach (CTNodeSaveData node_data in nodes)
            {
                List<CTChoiceSaveData> choices = CloneNodeChoices(node_data.list_choices);

                CTNode node = graph_view.CreateNode(node_data.title, node_data.dlog_type, node_data.pos, false);

                node.ID = node_data.ID;
                node.list_dlog_choices = node_data.list_choices;
                node.dlog_text = node_data.dlog_text;
                node.dlog_tip_text = node_data.dlog_tip_text;
                node.dlog_type = node_data.dlog_type; //
                node.active_character = node_data.character;
                node.active_bg = node_data.background;
                node.char_dropdown_index = node_data.char_dropdown_index;
                node.bg_dropdown_index = node_data.bg_dropdown_index;


                node.Draw();

                graph_view.AddElement(node);

                loaded_nodes.Add(node.ID, node);

                if (string.IsNullOrEmpty(node_data.group_ID))
                {
                    continue;
                }

                CTGroup group = loaded_groups[node_data.group_ID];

                node.group = group;

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

                    if (string.IsNullOrEmpty(choice_data.node_ID))
                    {
                        continue;
                    }

                    CTNode next_node = loaded_nodes[choice_data.node_ID];

                    Port next_node_input_port = (Port) next_node.inputContainer.Children().First();

                    Edge edge = choice_port.ConnectTo(next_node_input_port);

                    graph_view.AddElement(edge);

                    loaded_node.Value.RefreshPorts();
                }
            }
        }

        #endregion


        #region Util
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

        public static void CreateFolder(string _parent_folder_path, string _new_folder_name)
        {
            // If folder exists
            if (AssetDatabase.IsValidFolder($"{_parent_folder_path}/{_new_folder_name}"))
            {
                // Exit
                return;
            }

            // Crease tolder at path
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

            // Try load asset
            _T asset = LoadAsset<_T>(_path, _asset_name);

            // If asset is null it does not currently exist
            if (asset == null)
            {
                // Create asset if it does not exist
                asset = ScriptableObject.CreateInstance<_T>();

                AssetDatabase.CreateAsset(asset, full_path);
            }

            return asset;
        }

        public static _T LoadAsset<_T>(string _path, string _asset_name) where _T : ScriptableObject
        {
            string full_path = $"{_path}/{_asset_name}.asset";

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
                    text = choice.text,
                    node_ID = choice.node_ID
                };

                choices.Add(choice_data);
            }

            return choices;
        }

        #endregion
    }
}