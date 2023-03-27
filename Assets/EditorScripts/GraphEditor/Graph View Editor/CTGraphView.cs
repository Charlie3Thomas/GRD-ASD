using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CT.Windows
{
    using Data.Error;
    using Data.Save;
    using Components;
    using Enumerations;
    using Utils;

    public class CTGraphView : GraphView
    {
        private CTEditorWindow editor_window;

        private SerializableDictionary<string, CTGroupErrorData> groups;
        private SerializableDictionary<string, CTNodeErrorData> ungrouped_nodes;
        private SerializableDictionary<Group, SerializableDictionary<string, CTNodeErrorData>> grouped_nodes;

        private int total_title_errs;

        public int NameErrorsAmount
        {
            get
            {
                return total_title_errs;
            }

            set
            {
                // Set value
                total_title_errs = value;

                // Allow saving when there are no title conflics
                if (total_title_errs == 0)
                {
                    editor_window.AllowSaving();
                }

                // Deny saving when there are title conflics
                if (total_title_errs == 1)
                {
                    editor_window.DenySaving();
                }
            }
        }


        #region Initialisation

        public CTGraphView(CTEditorWindow _ct_editor_window)
        {
            editor_window = _ct_editor_window;

            ungrouped_nodes = new SerializableDictionary<string, CTNodeErrorData>();
            groups = new SerializableDictionary<string, CTGroupErrorData>();
            grouped_nodes = new SerializableDictionary<Group, SerializableDictionary<string, CTNodeErrorData>>();

            AddManipulators();
            AddGridBackground();
            OnElementsDeleted();
            OnGroupComponentsAdded();
            OnGroupComponentsRemoved();
            OnGroupRenamed();
            OnGraphViewChanged();
            AddStyles();
        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateNodeContextualMenu("Add narration Node", CTDialogueType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add user-choice Node", CTDialogueType.MultipleChoice));

            this.AddManipulator(CreateGroupContextualMenu());
        }

        #endregion


        #region Component change tracking

        public CTGroup CreateGroup(string _title, Vector2 _pos)
        {
            CTGroup group = new CTGroup(_title, _pos);

            AddGroup(group);

            AddElement(group);

            foreach (GraphElement selected_element in selection)
            {
                if (!(selected_element is CTNode))
                {
                    continue;
                }

                CTNode node = (CTNode)selected_element;

                group.AddElement(node);
            }

            return group;
        }

        public CTNode CreateNode(string _node_name, CTDialogueType _dlog_type, Vector2 _pos, bool _draw = true)
        {
            Type node_type = Type.GetType($"CT.Components.CT{_dlog_type}Node");

            CTNode node = (CTNode)Activator.CreateInstance(node_type);

            node.Initialise(_node_name, this, _pos);

            if (_draw)
            {
                node.Draw();
            }

            AddUngroupedNode(node);

            return node;
        }

        private void OnElementsDeleted()
        {
            deleteSelection = (operation_name, ask_user) =>
            {
                Type group_type = typeof(CTGroup);
                Type edge_type = typeof(Edge);

                List<CTGroup> groups_to_delete = new List<CTGroup>();
                List<CTNode> nodes_to_delete = new List<CTNode>();
                List<Edge> edges_to_delete = new List<Edge>();

                foreach (GraphElement selected_element in selection)
                {
                    if (selected_element is CTNode node)
                    {
                        // Add selected nodes to deletion list
                        nodes_to_delete.Add(node);

                        continue;
                    }

                    if (selected_element.GetType() == edge_type)
                    {
                        Edge edge = (Edge)selected_element;

                        // Add selected edges to deletion list
                        edges_to_delete.Add(edge);

                        continue;
                    }

                    if (selected_element.GetType() != group_type)
                    {
                        continue;
                    }

                    CTGroup group = (CTGroup)selected_element;

                    // Add selected groups to deletion list
                    groups_to_delete.Add(group);
                }

                // Iterate through all groups for deletion
                foreach (CTGroup group_to_delete in groups_to_delete)
                {
                    List<CTNode> group_nodes = new List<CTNode>();

                    foreach (GraphElement group_element in group_to_delete.containedElements)
                    {
                        if (!(group_element is CTNode))
                        {
                            continue;
                        }

                        CTNode group_node = (CTNode)group_element;

                        group_nodes.Add(group_node);
                    }

                    group_to_delete.RemoveElements(group_nodes);

                    RemoveGroup(group_to_delete);

                    RemoveElement(group_to_delete);
                }

                DeleteElements(edges_to_delete);

                // Iterate through all nodes for deletion
                foreach (CTNode node_to_delete in nodes_to_delete)
                {
                    if (node_to_delete.group != null)
                    {
                        node_to_delete.group.RemoveElement(node_to_delete);
                    }

                    RemoveUngroupedNode(node_to_delete);

                    // Remove port connections
                    node_to_delete.DisconnectAllPorts();

                    RemoveElement(node_to_delete);
                }
            };
        }

        private void OnGroupComponentsAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is CTNode))
                    {
                        continue;
                    }

                    CTGroup ct_group = (CTGroup) group;
                    CTNode node = (CTNode) element;

                    RemoveUngroupedNode(node);
                    AddGroupedNode(node, ct_group);
                }
            };
        }

        private void OnGroupComponentsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is CTNode))
                    {
                        continue;
                    }

                    CTGroup ct_group = (CTGroup) group;
                    CTNode node = (CTNode) element;

                    RemoveGroupedNode(node, ct_group);
                    AddUngroupedNode(node);
                }
            };
        }

        private void OnGroupRenamed()
        {
            groupTitleChanged = (group, new_title) =>
            {
                CTGroup ct_group = (CTGroup) group;

                // Use of 3rd party utility
                ct_group.title = new_title.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(ct_group.title))
                {
                    if (!string.IsNullOrEmpty(ct_group.previous_title))
                    {
                        ++NameErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(ct_group.previous_title))
                    {
                        --NameErrorsAmount;
                    }
                }

                RemoveGroup(ct_group);

                ct_group.previous_title = ct_group.title;

                AddGroup(ct_group);
            };
        }

        private void OnGraphViewChanged()
        {
            graphViewChanged = (changes) =>
            {
                if (changes.edgesToCreate != null)
                {
                    foreach (Edge edge in changes.edgesToCreate)
                    {
                        CTNode next_node = (CTNode) edge.input.node;

                        CTChoiceSaveData choice_data = (CTChoiceSaveData) edge.output.userData;

                        choice_data.node_ID = next_node.ID;
                    }
                }

                if (changes.elementsToRemove != null)
                {
                    Type edge_type = typeof(Edge);

                    foreach (GraphElement element in changes.elementsToRemove)
                    {
                        if (element.GetType() != edge_type)
                        {
                            continue;
                        }

                        Edge edge = (Edge) element;

                        CTChoiceSaveData choice_data = (CTChoiceSaveData) edge.output.userData;

                        choice_data.node_ID = "";
                    }
                }

                return changes;
            };
        }

        public void AddUngroupedNode(CTNode _node)
        {
            string node_name = _node.dlog_title.ToLower();

            if (!ungrouped_nodes.ContainsKey(node_name))
            {
                CTNodeErrorData node_err_data = new CTNodeErrorData();

                node_err_data.list_nodes.Add(_node);

                ungrouped_nodes.Add(node_name, node_err_data);

                return;
            }

            List<CTNode> list_ungrouped_nodes = ungrouped_nodes[node_name].list_nodes;

            list_ungrouped_nodes.Add(_node);

            Color err_colour = ungrouped_nodes[node_name].err_data.colour;

            _node.SetErrorStyle(err_colour);

            if (list_ungrouped_nodes.Count == 2)
            {
                ++NameErrorsAmount;

                list_ungrouped_nodes[0].SetErrorStyle(err_colour);
            }
        }

        public void RemoveUngroupedNode(CTNode _node)
        {
            string node_name = _node.dlog_title.ToLower();

            List<CTNode> list_ungrouped_nodes = ungrouped_nodes[node_name].list_nodes;

            list_ungrouped_nodes.Remove(_node);

            _node.ResetStyle();

            if (list_ungrouped_nodes.Count == 1)
            {
                --NameErrorsAmount;

                list_ungrouped_nodes[0].ResetStyle();

                return;
            }

            if (list_ungrouped_nodes.Count == 0)
            {
                ungrouped_nodes.Remove(node_name);
            }
        }

        private void AddGroup(CTGroup _group)
        {
            string group_name = _group.title.ToLower();

            if (!groups.ContainsKey(group_name))
            {
                CTGroupErrorData group_err_data = new CTGroupErrorData();

                group_err_data.list_groups.Add(_group);

                groups.Add(group_name, group_err_data);

                return;
            }

            List<CTGroup> list_groups = groups[group_name].list_groups;

            list_groups.Add(_group);

            Color err_colour = groups[group_name].err_data.colour;

            _group.SetErrorStyle(err_colour);

            if (list_groups.Count == 2)
            {
                ++NameErrorsAmount;

                list_groups[0].SetErrorStyle(err_colour);
            }
        }

        private void RemoveGroup(CTGroup _group)
        {
            string old_group_name = _group.previous_title.ToLower();

            List<CTGroup> list_groups = groups[old_group_name].list_groups;

            list_groups.Remove(_group);

            _group.ResetStyle();

            if (list_groups.Count == 1)
            {
                --NameErrorsAmount;

                list_groups[0].ResetStyle();

                return;
            }

            if (list_groups.Count == 0)
            {
                groups.Remove(old_group_name);
            }
        }

        public void AddGroupedNode(CTNode _node, CTGroup _group)
        {
            string node_name = _node.dlog_title.ToLower();

            _node.group = _group;

            if (!grouped_nodes.ContainsKey(_group))
            {
                grouped_nodes.Add(_group, new SerializableDictionary<string, CTNodeErrorData>());
            }

            if (!grouped_nodes[_group].ContainsKey(node_name))
            {
                CTNodeErrorData node_err_data = new CTNodeErrorData();

                node_err_data.list_nodes.Add(_node);

                grouped_nodes[_group].Add(node_name, node_err_data);

                return;
            }

            List<CTNode> list_grouped_nodes = grouped_nodes[_group][node_name].list_nodes;

            list_grouped_nodes.Add(_node);

            Color errorColor = grouped_nodes[_group][node_name].err_data.colour;

            _node.SetErrorStyle(errorColor);

            if (list_grouped_nodes.Count == 2)
            {
                ++NameErrorsAmount;

                list_grouped_nodes[0].SetErrorStyle(errorColor);
            }
        }

        public void RemoveGroupedNode(CTNode _node, CTGroup _group)
        {
            string _node_name = _node.dlog_title.ToLower();

            _node.group = null;

            List<CTNode> list_grouped_nodes = grouped_nodes[_group][_node_name].list_nodes;

            list_grouped_nodes.Remove(_node);

            _node.ResetStyle();

            if (list_grouped_nodes.Count == 1)
            {
                --NameErrorsAmount;

                list_grouped_nodes[0].ResetStyle();

                return;
            }

            if (list_grouped_nodes.Count == 0)
            {
                grouped_nodes[_group].Remove(_node_name);

                if (grouped_nodes[_group].Count == 0)
                {
                    grouped_nodes.Remove(_group);
                }
            }
        }

        #endregion


        #region Graph view elements

        private IManipulator CreateNodeContextualMenu(string _action_title, CTDialogueType _dlog_type)
        {
            ContextualMenuManipulator context_menu_manipulator = new ContextualMenuManipulator(
                menu_event => menu_event.menu.AppendAction(_action_title, action_event => AddElement(CreateNode("DialogueTitle", _dlog_type, GetLocalMousePosition(action_event.eventInfo.localMousePosition))))
            );

            return context_menu_manipulator;
        }

        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator context_menu_manipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Group", action_event => CreateGroup("GroupTitle", GetLocalMousePosition(action_event.eventInfo.localMousePosition)))
            );

            return context_menu_manipulator;
        }
        private void AddStyles()
        {
            this.AddStyleSheets("Assets/EditorScripts/GraphEditor/StyleSheets/CTGraphViewStyles.uss");
        }

        private void AddGridBackground()
        {
            GridBackground grid_background = new GridBackground();

            grid_background.StretchToParentSize();

            Insert(0, grid_background);
        }

        #endregion


        #region Util functions

        public override List<Port> GetCompatiblePorts(Port _start_port, NodeAdapter _node_adapter)
        {
            // Function to limit which ports the user can connect.

            List<Port> compatible_ports = new List<Port>();

            ports.ForEach(port =>
            {
                // Port should not connect with itself
                if (_start_port == port)
                {
                    return;
                }

                // Port should not connect with it's parent node
                if (_start_port.node == port.node)
                {
                    return;
                }

                // Port should not connect with ports of the same type
                if (_start_port.direction == port.direction)
                {
                    return;
                }

                compatible_ports.Add(port);
            });

            return compatible_ports;
        }

        public Vector2 GetLocalMousePosition(Vector2 _mouse_pos, bool _is_search_window = false)
        {
            Vector2 world_mouse_pos = _mouse_pos;

            if (_is_search_window)
            {
                world_mouse_pos = editor_window.rootVisualElement.ChangeCoordinatesTo(editor_window.rootVisualElement.parent, _mouse_pos - editor_window.position.position);
            }

            Vector2 local_mouse_pos = contentViewContainer.WorldToLocal(world_mouse_pos);

            return local_mouse_pos;
        }

        public void ClearGraph()
        {
            graphElements.ForEach(graph_element => RemoveElement(graph_element));

            groups.Clear();
            grouped_nodes.Clear();
            ungrouped_nodes.Clear();

            NameErrorsAmount = 0;
        }


        #endregion
    }
}