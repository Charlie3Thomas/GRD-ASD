using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CT.Components
{
    using Data.Save;
    using Enumerations;
    using Utils;
    using Windows;

    public class CTSingleChoiceNode : CTNode
    {
        public override void Initialise(string _node_name, CTGraphView _ct_graph_view, Vector2 _pos)
        {
            base.Initialise(_node_name, _ct_graph_view, _pos);

            dlog_type = CTDialogueType.Narration;

            CTChoiceSaveData choice_data = new CTChoiceSaveData()
            {
                text = "Output port leading to next node :"
            };

            list_dlog_choices.Add(choice_data);
        }

        public override void Draw()
        {
            base.Draw();

            foreach (CTChoiceSaveData choice in list_dlog_choices)
            {
                Port choice_port = this.CreatePort(choice.text);

                choice_port.userData = choice;

                outputContainer.Add(choice_port);
            }

            RefreshExpandedState();
        }
    }
}
