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
    using SO;
    using GraphView;

    public static class CTIOUtility
    {
        private static CTGraphView graph_view;

        private static string tree_file_name;
        private static string tree_folder_path;

        private static List<CTNode> nodes;
        private static List<CTGroup> groups;

        private static Dictionary<string, CTNodeGroupingSO> new_groups;
        private static Dictionary<string, CTNodeDataSO> new_nodes;

        private static Dictionary<string, CTGroup> loaded_groups;
        private static Dictionary<string, CTNode> loaded_nodes;

        public static void Initialise(CTGraphView _view, string _name)
        {
            graph_view = _view;

            tree_file_name = _name;
            tree_folder_path = $"Assets/DialogueTree/Dialogues/{_name}";

            nodes = new List<CTNode>();
            groups = new List<CTGroup>();

            new_groups = new Dictionary<string, CTNodeGroupingSO>();
            new_nodes = new Dictionary<string, CTNodeDataSO>();

            loaded_groups = new Dictionary<string, CTGroup>();
            loaded_nodes = new Dictionary<string, CTNode>();
        }

        #region Save
        public static void Save()
        {
            CreateDefaultFolders();

            GetGraphViewComponents();

            CTTreeSaveDataSO data = CreateAsset<CTTreeSaveDataSO>("Assets/DialogueTree/Trees", $"{tree_file_name}Graph");

            data.Initialise(tree_file_name);

            CTTreeDataSO tree = CreateAsset<CTTreeDataSO>(tree_folder_path, tree_file_name);

            tree.Initialise(tree_file_name);

            SaveGroups(data, tree);
            SaveNodes(data, tree);

            SaveAsset(data);
            SaveAsset(tree);
        }

        private static void SaveNodes(CTTreeSaveDataSO _data, CTTreeDataSO _tree)
        {
            List<string> ungrouped_node_titles = new List<string>();
            SerializableDictionary<string, List<string>> grouped_node_titles = new SerializableDictionary<string, List<string>>();

            foreach (CTNode n in nodes)
            {
                SaveNodeToTree(n, _data);
                SaveNodeToSO(n, _tree);

                if (n.group != null)
                {
                    grouped_node_titles.AddItem(n.group.title, n.node_name);

                    continue;
                }

                ungrouped_node_titles.Add(n.node_name);
            }

            RefreshNodeOptionConnections();

            RefreshGroupedNodes(grouped_node_titles, _data);
            RefreshUngroupedNodes(ungrouped_node_titles, _data);
        }

        private static void SaveNodeToTree(CTNode _node, CTTreeSaveDataSO _tree)
        {
            List<CTOptionSaveData> options = CopyNodeOptions(_node.options);

            CTNoteSaveData node_data = new CTNoteSaveData()
            {
                ID = _node.ID,
                name = _node.node_name,
                options = options,
                text = _node.text,
                group_ID = _node.group?.ID,
                node_type = _node.node_type,
                pos = _node.GetPosition().position,
                character = _node.character,
                background = _node.background,
                char_dropdown_index = _node.char_dropdown_index,
                bg_dropdown_index = _node.bg_dropdown_index

            };

            _tree.nodes.Add(node_data);
        }

        private static void SaveNodeToSO(CTNode _node, CTTreeDataSO _tree)
        {
            CTNodeDataSO node;

            if (_node.group != null)
            {
                node = CreateAsset<CTNodeDataSO>($"{tree_folder_path}/Groups/{_node.group.title}/Dialogues", _node.node_name);

                _tree.node_groups.AddItem(new_groups[_node.group.ID], node);
            }
            else
            {
                node = CreateAsset<CTNodeDataSO>($"{tree_folder_path}/Global/Dialogues", _node.node_name);

                _tree.ungrouped_nodes.Add(node);
            }

            node.Initialise(
                _node.node_name,
                _node.text,
                _node.tip_text,
                ConvertOptions(_node.options),
                _node.node_type,
                _node.IsStartingNode(),
                _node.character,
                _node.background,
                _node.char_dropdown_index,
                _node.bg_dropdown_index
            );

            new_nodes.Add(_node.ID, node);

            SaveAsset(node);
        }

        private static void SaveGroups(CTTreeSaveDataSO _data, CTTreeDataSO _tree)
        {
            List<string> group_titles = new List<string>();

            foreach (CTGroup g in groups)
            {
                SaveGroupToTree(g, _data);
                SaveGroupSO(g, _tree);

                group_titles.Add(g.title);
            }

            RefreshOldGroups(group_titles, _data);
        }

        private static void SaveGroupToTree(CTGroup _group, CTTreeSaveDataSO _data)
        {
            CTGroupSaveData data = new CTGroupSaveData()
            {
                ID = _group.ID,
                name = _group.title,
                pos = _group.GetPosition().position
            };

            _data.groups.Add(data);
        }

        private static void SaveGroupSO(CTGroup _group, CTTreeDataSO _tree)
        {
            string title = _group.title;

            CreateFolder($"{tree_folder_path}/Groups", title);
            CreateFolder($"{tree_folder_path}/Groups/{title}", "Dialogues");

            CTNodeGroupingSO group = CreateAsset<CTNodeGroupingSO>($"{tree_folder_path}/Groups/{title}", title);

            group.Initialise(title);

            new_groups.Add(_group.ID, group);

            _tree.node_groups.Add(group, new List<CTNodeDataSO>());

            SaveAsset(group);
        }

        public static void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        #endregion



        #region Load
        public static void Load()
        {
            CTTreeSaveDataSO tree = LoadAsset<CTTreeSaveDataSO>("Assets/DialogueTree/Trees", tree_file_name);

            if (tree == null)
            {
                Debug.LogError($"Unable to load {tree_file_name} tree! :(");

                return;
            }

            CTGraphViewWindow.ChangeTitle(tree.file_name);

            LoadGroups(tree.groups);
            LoadNodes(tree.nodes);
            LoadNodesConnections();
        }

        private static void LoadNodes(List<CTNoteSaveData> _nodes)
        {
            foreach (CTNoteSaveData n in _nodes)
            {
                List<CTOptionSaveData> options = CopyNodeOptions(n.options);

                CTNode node = graph_view.CreateNode(n.name, n.node_type, n.pos, false);

                node.ID = n.ID;
                node.options = options;
                node.text = n.text;
                node.tip_text = n.dlog_tip_text;
                node.character = n.character;
                node.background = n.background;
                node.char_dropdown_index = n.char_dropdown_index;
                node.bg_dropdown_index = n.bg_dropdown_index;

                node.Draw();

                graph_view.AddElement(node);

                loaded_nodes.Add(node.ID, node);

                if (string.IsNullOrEmpty(n.group_ID))
                {
                    continue;
                }

                CTGroup group = loaded_groups[n.group_ID];

                node.group = group;

                group.AddElement(node);
            }
        }

        private static void LoadGroups(List<CTGroupSaveData> _groups)
        {
            foreach (CTGroupSaveData g in _groups)
            {
                CTGroup group = graph_view.CreateGroup(g.name, g.pos);

                group.ID = g.ID;

                loaded_groups.Add(group.ID, group);
            }
        }

        private static void LoadNodesConnections()
        {
            foreach (KeyValuePair<string, CTNode> n in loaded_nodes)
            {
                foreach (Port p in n.Value.outputContainer.Children())
                {
                    CTOptionSaveData option_data = (CTOptionSaveData) p.userData;

                    if (string.IsNullOrEmpty(option_data.node_ID))
                    {
                        continue;
                    }

                    CTNode nextNode = loaded_nodes[option_data.node_ID];

                    Port nextNodeInputPort = (Port) nextNode.inputContainer.Children().First();

                    Edge edge = p.ConnectTo(nextNodeInputPort);

                    graph_view.AddElement(edge);

                    n.Value.RefreshPorts();
                }
            }
        }

        public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }

        #endregion



        #region Utility
        public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            T asset = LoadAsset<T>(path, assetName);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();

                AssetDatabase.CreateAsset(asset, fullPath);
            }

            return asset;
        }

        public static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }

        private static List<CTOptionSaveData> CopyNodeOptions(List<CTOptionSaveData> _options)
        {
            List<CTOptionSaveData> options = new List<CTOptionSaveData>();

            foreach (CTOptionSaveData o in _options)
            {
                CTOptionSaveData option_data = new CTOptionSaveData()
                {
                    text = o.text,
                    node_ID = o.node_ID
                };

                options.Add(option_data);
            }

            return options;
        }
        public static void CreateFolder(string parentFolderPath, string newFolderName)
        {
            if (AssetDatabase.IsValidFolder($"{parentFolderPath}/{newFolderName}"))
            {
                return;
            }

            AssetDatabase.CreateFolder(parentFolderPath, newFolderName);
        }

        public static void RemoveFolder(string path)
        {
            FileUtil.DeleteFileOrDirectory($"{path}.meta");
            FileUtil.DeleteFileOrDirectory($"{path}/");
        }

        private static void CreateDefaultFolders()
        {
            // Folder_path
            CreateFolder("Assets", "DialogueTree");
            CreateFolder("Assets/DialogueTree", "Trees");
            CreateFolder("Assets/DialogueTree", "Dialogues");
            CreateFolder("Assets/DialogueTree/Dialogues", tree_file_name);

            // Path subfolders
            CreateFolder(tree_folder_path, "Global");
            CreateFolder(tree_folder_path, "Groups");
            CreateFolder($"{tree_folder_path}/Global", "Dialogues");
        }

        private static void GetGraphViewComponents()
        {
            Type groupType = typeof(CTGroup);

            graph_view.graphElements.ForEach(e =>
            {
                if (e is CTNode node)
                {
                    nodes.Add(node);

                    return;
                }

                if (e.GetType() == groupType)
                {
                    CTGroup group = (CTGroup)e;

                    groups.Add(group);

                    return;
                }
            });
        }

        private static List<CTNodeOptionData> ConvertOptions(List<CTOptionSaveData> _options)
        {
            List<CTNodeOptionData> options = new List<CTNodeOptionData>();

            foreach (CTOptionSaveData o in _options)
            {
                CTNodeOptionData option = new CTNodeOptionData()
                {
                    text = o.text
                };

                options.Add(option);
            }

            return options;
        }

        private static void RefreshOldGroups(List<string> _names, CTTreeSaveDataSO _data)
        {
            if (_data.previous_group_names != null && _data.previous_group_names.Count != 0)
            {
                List<string> dead_groups = _data.previous_group_names.Except(_names).ToList();

                foreach (string dead in dead_groups)
                {
                    RemoveFolder($"{tree_folder_path}/Groups/{dead}");
                }
            }

            _data.previous_group_names = new List<string>(_names);
        }

        private static void RefreshNodeOptionConnections()
        {
            foreach (CTNode n in nodes)
            {
                CTNodeDataSO node = new_nodes[n.ID];

                for (int i = 0; i < n.options.Count; ++i)
                {
                    CTOptionSaveData option = n.options[i];

                    if (string.IsNullOrEmpty(option.node_ID))
                        continue;


                    node.options[i].next_node = new_nodes[option.node_ID];

                    SaveAsset(node);
                }
            }
        }

        private static void RefreshGroupedNodes(SerializableDictionary<string, List<string>> _current_names, CTTreeSaveDataSO _tree)
        {
            if (_tree.previous_grouped_node_names != null && _tree.previous_grouped_node_names.Count != 0)
            {
                foreach (KeyValuePair<string, List<string>> old_node in _tree.previous_grouped_node_names)
                {
                    List<string> dead_noedes = new List<string>();

                    if (_current_names.ContainsKey(old_node.Key))
                    {
                        dead_noedes = old_node.Value.Except(_current_names[old_node.Key]).ToList();
                    }

                    foreach (string dead in dead_noedes)
                    {
                        RemoveAsset($"{tree_folder_path}/Groups/{old_node.Key}/Dialogues", dead);
                    }
                }
            }

            _tree.previous_grouped_node_names = new SerializableDictionary<string, List<string>>(_current_names);
        }

        private static void RefreshUngroupedNodes(List<string> _names, CTTreeSaveDataSO _tree)
        {
            if (_tree.previous_ungrouped_node_names != null && _tree.previous_ungrouped_node_names.Count != 0)
            {
                List<string> dead_nodes = _tree.previous_ungrouped_node_names.Except(_names).ToList();

                foreach (string dead in dead_nodes)
                {
                    RemoveAsset($"{tree_folder_path}/Global/Dialogues", dead);
                }
            }

            _tree.previous_ungrouped_node_names = new List<string>(_names);
        }

        #endregion
    }

}