using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CT.Windows
{
    using Elements;
    using Enumerations;

    public class CTSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private CTGraphView graph_view;
        private Texture2D indent_icon;

        public void Initialize(CTGraphView _ct_graph_view)
        {
            graph_view = _ct_graph_view;

            indent_icon = new Texture2D(1, 1);
            indent_icon.SetPixel(0, 0, Color.clear);
            indent_icon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext _context)
        {
            List<SearchTreeEntry> search_tree_entries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Elements")),
                new SearchTreeGroupEntry(new GUIContent("Dialogue Nodes"), 1),
                new SearchTreeEntry(new GUIContent("Single Choice", indent_icon))
                {
                    userData = CTDialogueType.SingleChoice,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Multiple Choice", indent_icon))
                {
                    userData = CTDialogueType.MultipleChoice,
                    level = 2
                },
                new SearchTreeGroupEntry(new GUIContent("Dialogue Groups"), 1),
                new SearchTreeEntry(new GUIContent("Single Group", indent_icon))
                {
                    userData = new Group(),
                    level = 2
                }
            };

            return search_tree_entries;
        }

        public bool OnSelectEntry(SearchTreeEntry _search_tree_entry, SearchWindowContext _context)
        {
            Vector2 local_mouse_pos = graph_view.GetLocalMousePosition(_context.screenMousePosition, true);

            switch (_search_tree_entry.userData)
            {
                case CTDialogueType.SingleChoice:
                {
                    CTSingleChoiceNode single_node_choice = (CTSingleChoiceNode) graph_view.CreateNode("DialogueName", CTDialogueType.SingleChoice, local_mouse_pos);

                    graph_view.AddElement(single_node_choice);

                    return true;
                }

                case CTDialogueType.MultipleChoice:
                {
                    CTMultipleChoiceNode multi_choice_node = (CTMultipleChoiceNode) graph_view.CreateNode("DialogueName", CTDialogueType.MultipleChoice, local_mouse_pos);

                    graph_view.AddElement(multi_choice_node);

                    return true;
                }

                case Group _:
                {
                    graph_view.CreateGroup("DialogueGroup", local_mouse_pos);

                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}