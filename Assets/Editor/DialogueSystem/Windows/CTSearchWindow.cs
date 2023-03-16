using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace CT.Windows
{
    using CT.Elements;
    using CT.Enumerations;
    

    public class CTSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private CTGraphView graph_view;
        public void Initialise(CTGraphView _ct_graph_view)
        {
            graph_view = _ct_graph_view;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext _context)
        {
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>()
            { 
                new SearchTreeGroupEntry(new GUIContent("Create Element")),
                new SearchTreeGroupEntry(new GUIContent("Dialogue Node"), 1),
                new SearchTreeEntry(new GUIContent("Single Choice"))
                {
                    userData = CTDialogueType.SingleChoice,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Multiple Choice"))
                {
                    userData = CTDialogueType.MultipleChoice,
                    level = 2
                },

                new SearchTreeGroupEntry(new GUIContent("Dialogue Group"), 1),
                new SearchTreeEntry(new GUIContent("Single Group"))
                {
                    userData = new Group(),
                    level = 2
                }


            };

            return entries;
        }

        public bool OnSelectEntry(SearchTreeEntry _search_tree_entry, SearchWindowContext _context)
        {
            Vector2 local_mouse_pos = graph_view.GetLocalMousePosition(_context.screenMousePosition, true);

            switch (_search_tree_entry.userData) 
            {
                case CTDialogueType.SingleChoice:
                    CTSingleChoiceNode sc_node = (CTSingleChoiceNode) graph_view.CreateNode(CTDialogueType.SingleChoice, local_mouse_pos);
                    graph_view.AddElement(sc_node);
                    return true;

                case CTDialogueType.MultipleChoice:
                    CTMultipleChoiceNode mc_node = (CTMultipleChoiceNode)graph_view.CreateNode(CTDialogueType.MultipleChoice, local_mouse_pos);
                    graph_view.AddElement(mc_node);
                    return true;

                case Group _:
                    /*CTGroup group = */graph_view.CreateGroup("DialogueGroup", local_mouse_pos);
                    //graph_view.AddElement(group);
                    return true;

                default: 
                    return false;
            }
        }
    }
}
