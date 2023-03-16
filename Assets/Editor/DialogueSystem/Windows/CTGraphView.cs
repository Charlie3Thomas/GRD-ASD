using CT.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace CT.Windows
{
    using Elements;
    using Enumerations;
    using Utilities;
    using Data.Error;
    using Data.Save;
    using static UnityEngine.GraphicsBuffer;

    public class CTGraphView : GraphView
    {
        private CTEditorWindow editor_window;
        private CTSearchWindow search_window;

        public SerializableDictionary<string, CTNodeErrorData> ungrouped_nodes;
        public SerializableDictionary<string, CTGroupErrorData> groups;
        public SerializableDictionary<Group, SerializableDictionary<string, CTNodeErrorData>> grouped_nodes;

        private int ID_errors;
        public int ID_error_total
        {
            get 
            { 
                return ID_errors; 
            }
            set 
            {
                ID_errors = value;

                if (ID_error_total == 0)
                {
                    // Enable save button
                    editor_window.AllowSave();
                }
                if (ID_error_total == 1)
                {
                    // Disable save button
                    editor_window.DenySave();
                }
            }
        }

        public CTGraphView(CTEditorWindow _editor_window)
        {
            editor_window = _editor_window;

            ungrouped_nodes = new SerializableDictionary<string, CTNodeErrorData>();
            groups = new SerializableDictionary<string, CTGroupErrorData>();
            grouped_nodes = new SerializableDictionary<Group, SerializableDictionary<string, CTNodeErrorData>>();

            // Manipulators
            AddManipulators();
            AddSearchWindow();
            AddGridBackground();


            // Changes
            OnElementsDeleted();
            OnGroupElementsAdded();
            OnGroupElementsRemoved();
            OnGroupRenamed();
            OnGraphViewChanged();
            //CreateNode();

            // Style
            AddStyles();
        }        

        #region Overrides
        public override List<Port> GetCompatiblePorts(Port _start_port, NodeAdapter _node_adapter)
        {
            List<Port> compatible_ports = new List<Port>();

            /*
                OUTPUT
                must connect with other nodes input ports
                must not connect with it's own ports
                must not connect output port with any output port

                INPUT
                must connect with other nodes output ports
                must not connect with it's own ports
                must not connect input port with any input port
                
             */
            ports.ForEach(port =>
            {
                if (_start_port == port) { return; }
                if (_start_port.node == port.node) { return; }
                if (_start_port.direction == port.direction) { return; }

                compatible_ports.Add(port);

            });

            return compatible_ports;
        }

        #endregion

        #region Manipulators

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            ContentDragger cd = new ContentDragger();
            cd.target = this;
            cd.clampToParentEdges = true;
            this.AddManipulator(cd);

            this.AddManipulator(CreateNodeContextualMenu("Add Node (Single choice)", CTDialogueType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple choice)", CTDialogueType.MultipleChoice));

            this.AddManipulator(CreateGroupContextualMenu());

        }

        #endregion

        #region Elements

        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextual_menu_manipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add group", actionEvent => CreateGroup("DialogueGroup", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
                );

            return contextual_menu_manipulator;
        }

        private IManipulator CreateNodeContextualMenu(string _action_title, CTDialogueType _dialogue_type)
        {
            ContextualMenuManipulator contextual_menu_manipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(_action_title, actionEvent => AddElement(CreateNode(_dialogue_type, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
                );

            return contextual_menu_manipulator;
        }

        public CTGroup CreateGroup(string _title, Vector2 _local_mouse_pos)
        {
            CTGroup group = new CTGroup(_title, _local_mouse_pos);

            AddGroup(group);

            AddElement(group);

            foreach (GraphElement selectedElement in selection)
            {
                if (!(selectedElement is CTNode))
                {
                    continue;
                }

                CTNode node = (CTNode)selectedElement;

                group.AddElement(node);
            }

            return group;
        }

        public CTNode CreateNode(CTDialogueType _dialogue_type, Vector2 _position)
        {
            Type node_type = Type.GetType($"CT.Elements.CT{_dialogue_type}Node");

            CTNode node = (CTNode) Activator.CreateInstance(node_type);

            node.Initialise(this, _position);
            node.Draw();

            //AddElement(node);

            AddUngroupedNode(node);

            return node;
        }

        #endregion

        #region Grouping
        private void OnGroupElementsAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is CTNode))
                    {
                        continue;
                    }

                    CTGroup node_group = (CTGroup) group;
                    CTNode node = (CTNode)element;

                    //Debug.Log("Called");

                    RemoveUngroupedNode(node);
                    AddGroupedNode(node, node_group);
                }

            };
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

                foreach (GraphElement element in selection)
                {
                    if (element is CTNode node)
                    {
                        nodes_to_delete.Add(node);
                        continue;
                    }

                    if (element.GetType() == edge_type)
                    {
                        Edge edge = (Edge) element;

                        edges_to_delete.Add(edge);

                        continue;
                    }

                    if (element.GetType() != group_type)
                    {
                        continue;
                    }

                    CTGroup group = (CTGroup) element;
                    
                    groups_to_delete.Add(group);
                }

                foreach(CTGroup group in groups_to_delete)
                {
                    List<CTNode> group_nodes = new List<CTNode>();

                    foreach (GraphElement group_element in group.containedElements)
                    {
                        if (!(group_element is CTNode))
                        {
                            continue;
                        }

                        CTNode group_node = (CTNode) group_element;

                        group_nodes.Add(group_node);

                    }

                    group.RemoveElements(group_nodes);

                    RemoveGroup(group);

                    RemoveElement(group);
                }

                DeleteElements(edges_to_delete);

                foreach (CTNode node in nodes_to_delete)
                {
                    if (node.group != null)
                    {
                        node.group.RemoveElement(node);
                    }

                    RemoveUngroupedNode(node);
                    node.DisconnectAllPorts();
                    RemoveElement(node);
                }

            };
        }

        private void OnGroupElementsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is CTNode))
                    {
                        continue;
                    }

                    CTNode node = (CTNode)element;

                    RemoveGroupedNode(node, group);
                    AddUngroupedNode(node);

                }
            };
        }

        private void OnGroupRenamed()
        {
            groupTitleChanged = (group, new_ID) =>
            {
                CTGroup ct_group = (CTGroup) group;

                ct_group.title = new_ID.RemoveWhitespaces().RemoveSpecialCharacters();

                // Check filename valid for node
                if (string.IsNullOrEmpty(ct_group.title))
                {
                    if (!string.IsNullOrEmpty(ct_group.old_title))
                    {
                        ++ID_error_total;
                    }
                }
                else
                {
                    // If filename is valid
                    if (string.IsNullOrEmpty(ct_group.old_title))
                    {
                        --ID_error_total;
                    }
                }

                RemoveGroup(ct_group);

                ct_group.old_title = ct_group.title;
               
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

                        choice_data.NodeID = next_node.ID;
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

                        choice_data.NodeID = "";

                    }
                }


                return changes;
            };
        }

        public void AddUngroupedNode(CTNode _node)
        {
            string node_name = _node.DialogueName.ToLower();

            if (!ungrouped_nodes.ContainsKey(node_name))
            {
                CTNodeErrorData node_err_data = new CTNodeErrorData();
                node_err_data.nodes.Add(_node);
                ungrouped_nodes.Add(node_name, node_err_data);
                return;
            }

            List<CTNode> ungrouped_list = ungrouped_nodes[node_name].nodes;

            ungrouped_list.Add(_node);

            Color err_colour = ungrouped_nodes[node_name].error_data.color;

            _node.SetErrorStyle(err_colour);

            if (ungrouped_list.Count == 2)
            {
                ++ID_error_total;
                ungrouped_list[0].SetErrorStyle(err_colour);
            }

        }

        public void RemoveUngroupedNode(CTNode _node)
        {
            string node_name = _node.DialogueName.ToLower();

            List<CTNode> ungrouped_list = ungrouped_nodes[node_name].nodes;

            ungrouped_list.Remove(_node);

            _node.ResetStyle();

            if (ungrouped_list.Count == 1)
            {
                --ID_error_total;
                ungrouped_list[0].ResetStyle();
                return;
            }

            if (ungrouped_list.Count == 0)
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

                group_err_data.groups.Add(_group);

                groups.Add(group_name, group_err_data);

                return;
            }

            List<CTGroup> groups_list = groups[group_name].groups;

            groups_list.Add(_group);

            Color err_col = groups[group_name].err_data.color;

            _group.SetErrStyle(err_col);

            if (groups_list.Count == 2)
            {
                ++ID_error_total;
                groups_list[0].SetErrStyle(err_col);
            }

        }

        private void RemoveGroup(CTGroup _group)
        {
            string old_group_ID = _group.old_title.ToLower();

            List<CTGroup> groups_list = groups[old_group_ID].groups;

            groups_list.Remove(_group);

            _group.ResetStyle();

            if (groups_list.Count == 1)
            {
                --ID_error_total;
                groups_list[0].ResetStyle();
                return;
            }

            if (groups_list.Count == 0)
            {
                groups.Remove(old_group_ID);
            }

        }

        public void AddGroupedNode(CTNode _node, CTGroup _group)
        {
            string node_name = _node.DialogueName.ToLower();

            _node.group = _group;

            if (!grouped_nodes.ContainsKey(_group))
            {
                grouped_nodes.Add(_group, new SerializableDictionary<string, CTNodeErrorData>());
            }

            if (!grouped_nodes[_group].ContainsKey(node_name)) 
            { 
                CTNodeErrorData node_err_data = new CTNodeErrorData();
                node_err_data.nodes.Add(_node);
                grouped_nodes[_group].Add(node_name, node_err_data);
                return;
            }

            List<CTNode> grouped_nodes_list = grouped_nodes[_group][node_name].nodes;

            grouped_nodes_list.Add(_node);
            Color err_colour = grouped_nodes[_group][node_name].error_data.color;
            _node.SetErrorStyle(err_colour);

            if (grouped_nodes_list.Count == 2)
            {
                ++ID_error_total;
                grouped_nodes_list[0].SetErrorStyle(err_colour);
            }

        }

        public void RemoveGroupedNode(CTNode _node, Group _group)
        {
            string node_name = _node.DialogueName.ToLower();            

            List<CTNode> grouped_list = grouped_nodes[_group][node_name].nodes;

            grouped_list.Remove(_node);

            _node.ResetStyle();

            if (grouped_list.Count == 1)
            {
                --ID_error_total;
                grouped_list[0].ResetStyle();
                return;
            }

            if (grouped_list.Count == 0)
            {
                grouped_nodes[_group].Remove(node_name);

                if (grouped_nodes[_group].Count == 0)
                {
                    grouped_nodes.Remove(_group);
                }
            }

            // assign no group
            _node.group = null;

        }





        #endregion

        #region Style
        private void AddSearchWindow()
        {
            if (search_window == null) 
            { 
                search_window = ScriptableObject.CreateInstance<CTSearchWindow>();
                search_window.Initialise(this);
            }

            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), search_window);
        }

        private void AddGridBackground()
        {
            GridBackground grid_background = new GridBackground();

            grid_background.StretchToParentSize();

            Insert(0, grid_background);
        }

        private void AddStyles()
        {
            StyleSheet style_sheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/CTGraphViewStyles.uss");

            styleSheets.Add(style_sheet);
        }


        #endregion

        #region Utility

        public Vector2 GetLocalMousePosition(Vector2 _mouse_pos, bool _is_search_window = false)
        {
            Vector2 world_pos = _mouse_pos;

            if (_is_search_window) 
            {
                world_pos -= editor_window.position.position;
            }

            Vector2 local_pos = contentViewContainer.WorldToLocal(world_pos);
            return local_pos;
        }

        #endregion
    }
}
