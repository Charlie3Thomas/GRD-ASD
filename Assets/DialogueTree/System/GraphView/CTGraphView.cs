#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CT.GraphView
{
    using Data.Error;
    using Data.Save;
    using Components;
    using Enums;
    using Utils;

    public class CTGraphView : UnityEditor.Experimental.GraphView.GraphView
    {
        private CTGraphViewWindow graph_view_window;

        private SerializableDictionary<string, CTGroupError> group_errors;
        private SerializableDictionary<string, CTNodeErrorData> ungrouped_node_errors;
        private SerializableDictionary<Group, SerializableDictionary<string, CTNodeErrorData>> grouped_node_errors;

        private int total_errors;
        public int TotalErrors
        {
            get
            {
                return total_errors;
            }
            set
            {
                total_errors = value;

                if (total_errors == 0)
                {
                    graph_view_window.AllowSaving();
                }

                if (total_errors == 1)
                {
                    graph_view_window.DenySaving();
                }
            }
        }

        public CTGraphView(CTGraphViewWindow _window)
        {
            graph_view_window = _window;

            group_errors = new SerializableDictionary<string, CTGroupError>();

            ungrouped_node_errors = new SerializableDictionary<string, CTNodeErrorData>();

            grouped_node_errors = new SerializableDictionary<Group, SerializableDictionary<string, CTNodeErrorData>>();

            SetManipulators();

            CleanupComponents();

            GroupElementsAdded();

            GroupElementsRemoved();

            GroupRenamed();

            IfGraphViewChanged();
        }

        public CTNode CreateNode(string _name, CTNodeType _type, Vector2 _pos, bool _draw = true)
        {
            Type nodeType = Type.GetType($"CT.Components.CT{_type}Node");

            CTNode node = (CTNode)Activator.CreateInstance(nodeType);

            node.Initialise(_name, this, _pos);

            if (_draw)
            {
                node.Draw();
            }

            AddUngroupedNode(node);

            return node;
        }

        private void SetManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(NodeContextualMenu("Narration node.", CTNodeType.Narration));
            this.AddManipulator(NodeContextualMenu("Choice node", CTNodeType.Choice));

            this.AddManipulator(GroupContextualMenu());
        }

        private IManipulator NodeContextualMenu(string _name, CTNodeType _type)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(_name, actionEvent => AddElement(CreateNode("NodeName", _type, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );

            return contextualMenuManipulator;
        }

        private IManipulator GroupContextualMenu()
        {
            ContextualMenuManipulator context_menu = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Group", actionEvent => CreateGroup("NodeGroup", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
            );

            return context_menu;
        }



        #region Grouping
        public CTGroup CreateGroup(string _name, Vector2 _pos)
        {
            CTGroup group = new CTGroup(_name, _pos);

            AddGroup(group);

            AddElement(group);

            foreach (GraphElement ge in selection)
            {
                if (!(ge is CTNode))
                {
                    continue;
                }

                CTNode node = (CTNode)ge;

                group.AddElement(node);
            }

            return group;
        }

        private void AddGroup(CTGroup _group)
        {
            string name = _group.title.ToLower();

            if (!group_errors.ContainsKey(name))
            {
                CTGroupError groupErrorData = new CTGroupError();

                groupErrorData.groups.Add(_group);

                group_errors.Add(name, groupErrorData);

                return;
            }

            List<CTGroup> groups = group_errors[name].groups;

            groups.Add(_group);

            Color colour = group_errors[name].error.colour;

            _group.SetErrorColour(colour);

            if (groups.Count == 2)
            {
                ++TotalErrors;

                groups[0].SetErrorColour(colour);
            }
        }

        private void RemoveGroup(CTGroup _group)
        {
            string old_name = _group.old_name.ToLower();

            List<CTGroup> groups = group_errors[old_name].groups;

            groups.Remove(_group);

            _group.ClearErrorColour();

            if (groups.Count == 1)
            {
                --TotalErrors;

                groups[0].ClearErrorColour();

                return;
            }

            if (groups.Count == 0)
            {
                group_errors.Remove(old_name);
            }
        }

        public void AddGroupedNode(CTNode _node, CTGroup _group)
        {
            string name = _node.node_name.ToLower();

            _node.group = _group;

            if (!grouped_node_errors.ContainsKey(_group))
            {
                grouped_node_errors.Add(_group, new SerializableDictionary<string, CTNodeErrorData>());
            }

            if (!grouped_node_errors[_group].ContainsKey(name))
            {
                CTNodeErrorData node_error = new CTNodeErrorData();

                node_error.nodes.Add(_node);

                grouped_node_errors[_group].Add(name, node_error);

                return;
            }

            List<CTNode> grouped_nodes = grouped_node_errors[_group][name].nodes;

            grouped_nodes.Add(_node);

            Color colour = grouped_node_errors[_group][name].err_data.colour;

            _node.SetErrorColour(colour);

            if (grouped_nodes.Count == 2)
            {
                ++TotalErrors;

                grouped_nodes[0].SetErrorColour(colour);
            }
        }

        public void RemoveGroupedNode(CTNode _node, CTGroup _group)
        {
            string name = _node.node_name.ToLower();

            _node.group = null;

            List<CTNode> grouped_nodes = grouped_node_errors[_group][name].nodes;

            grouped_nodes.Remove(_node);

            _node.ResetStyle();

            if (grouped_nodes.Count == 1)
            {
                --TotalErrors;

                grouped_nodes[0].ResetStyle();

                return;
            }

            if (grouped_nodes.Count == 0)
            {
                grouped_node_errors[_group].Remove(name);

                if (grouped_node_errors[_group].Count == 0)
                {
                    grouped_node_errors.Remove(_group);
                }
            }
        }

        public void AddUngroupedNode(CTNode _node)
        {
            string name = _node.node_name.ToLower();

            if (!ungrouped_node_errors.ContainsKey(name))
            {
                CTNodeErrorData error = new CTNodeErrorData();

                error.nodes.Add(_node);

                ungrouped_node_errors.Add(name, error);

                return;
            }

            List<CTNode> ungrouped_nodes = ungrouped_node_errors[name].nodes;

            ungrouped_nodes.Add(_node);

            Color colour = ungrouped_node_errors[name].err_data.colour;

            _node.SetErrorColour(colour);

            if (ungrouped_nodes.Count == 2)
            {
                ++TotalErrors;

                ungrouped_nodes[0].SetErrorColour(colour);
            }
        }

        public void RemoveUngroupedNode(CTNode _node)
        {
            string nodeName = _node.node_name.ToLower();

            List<CTNode> ungroupedNodesList = ungrouped_node_errors[nodeName].nodes;

            ungroupedNodesList.Remove(_node);

            _node.ResetStyle();

            if (ungroupedNodesList.Count == 1)
            {
                --TotalErrors;

                ungroupedNodesList[0].ResetStyle();

                return;
            }

            if (ungroupedNodesList.Count == 0)
            {
                ungrouped_node_errors.Remove(nodeName);
            }
        }

        #endregion


        #region Utility
        public void ClearTree()
        {
            graphElements.ForEach(component => RemoveElement(component));

            group_errors.Clear();
            grouped_node_errors.Clear();
            ungrouped_node_errors.Clear();

            TotalErrors = 0;
        }

        public Vector2 GetLocalMousePosition(Vector2 _pos)
        {
            Vector2 worldspace_mouse_pos = _pos;

            return contentViewContainer.WorldToLocal(worldspace_mouse_pos);
        }

        public override List<Port> GetCompatiblePorts(Port _port, NodeAdapter _adapter)
        {
            List<Port> compatible = new List<Port>();

            ports.ForEach(p =>
            {
                if (_port == p)
                {
                    return;
                }

                if (_port.node == p.node)
                {
                    return;
                }

                if (_port.direction == p.direction)
                {
                    return;
                }

                compatible.Add(p);
            });

            return compatible;
        }

        private void CleanupComponents()
        {
            deleteSelection = (operationName, askUser) =>
            {
                Type t_group = typeof(CTGroup);
                Type t_edge = typeof(Edge);

                List<CTGroup> dead_groups = new List<CTGroup>();
                List<CTNode> dead_nodes = new List<CTNode>();
                List<Edge> dead_edges = new List<Edge>();

                foreach (GraphElement ge in selection)
                {
                    if (ge is CTNode node)
                    {
                        dead_nodes.Add(node);

                        continue;
                    }

                    if (ge.GetType() == t_edge)
                    {
                        Edge edge = (Edge)ge;

                        dead_edges.Add(edge);

                        continue;
                    }

                    if (ge.GetType() != t_group)
                    {
                        continue;
                    }

                    CTGroup group = (CTGroup)ge;

                    dead_groups.Add(group);
                }

                foreach (CTGroup g in dead_groups)
                {
                    List<CTNode> nodes = new List<CTNode>();

                    foreach (GraphElement ge in g.containedElements)
                    {
                        if (!(ge is CTNode))
                        {
                            continue;
                        }

                        CTNode node = (CTNode)ge;

                        nodes.Add(node);
                    }

                    g.RemoveElements(nodes);

                    RemoveGroup(g);

                    RemoveElement(g);
                }

                DeleteElements(dead_edges);

                foreach (CTNode dead in dead_nodes)
                {
                    if (dead.group != null)
                    {
                        dead.group.RemoveElement(dead);
                    }

                    RemoveUngroupedNode(dead);

                    dead.DisconnectAllPorts();

                    RemoveElement(dead);
                }
            };
        }

        private void GroupRenamed()
        {
            groupTitleChanged = (_group, _title) =>
            {
                CTGroup group = (CTGroup)_group;

                group.title = _title;

                if (string.IsNullOrEmpty(group.title))
                {
                    if (!string.IsNullOrEmpty(group.old_name))
                    {
                        ++TotalErrors;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(group.old_name))
                    {
                        --TotalErrors;
                    }
                }

                RemoveGroup(group);

                group.old_name = group.title;

                AddGroup(group);
            };
        }

        private void GroupElementsAdded()
        {
            elementsAddedToGroup = (_group, _elements) =>
            {
                foreach (GraphElement e in _elements)
                {
                    if (!(e is CTNode))
                    {
                        continue;
                    }

                    CTGroup group = (CTGroup)_group;
                    CTNode node = (CTNode)e;

                    RemoveUngroupedNode(node);
                    AddGroupedNode(node, group);
                }
            };
        }

        private void GroupElementsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement e in elements)
                {
                    if (!(e is CTNode))
                    {
                        continue;
                    }

                    CTGroup dsGroup = (CTGroup)group;
                    CTNode node = (CTNode)e;

                    RemoveGroupedNode(node, dsGroup);
                    AddUngroupedNode(node);
                }
            };
        }

        private void IfGraphViewChanged()
        {
            graphViewChanged = (_changes) =>
            {
                if (_changes.edgesToCreate != null)
                {
                    foreach (Edge e in _changes.edgesToCreate)
                    {
                        CTNode next_node = (CTNode)e.input.node;

                        CTOptionSaveData option_data = (CTOptionSaveData)e.output.userData;

                        option_data.node_ID = next_node.ID;
                    }
                }

                if (_changes.elementsToRemove != null)
                {
                    Type type = typeof(Edge);

                    foreach (GraphElement ge in _changes.elementsToRemove)
                    {
                        if (ge.GetType() != type)
                        {
                            continue;
                        }

                        Edge edge = (Edge)ge;

                        CTOptionSaveData option_data = (CTOptionSaveData)edge.output.userData;

                        option_data.node_ID = "";
                    }
                }

                return _changes;
            };
        }

        #endregion
    }
}
#endif