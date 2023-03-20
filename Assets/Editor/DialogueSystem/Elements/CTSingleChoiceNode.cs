using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CT.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class CTSingleChoiceNode : CTNode
    {
        public override void Initialise(string _node_name, CTGraphView _ct_graph_view, Vector2 _pos)
        {
            base.Initialise(_node_name, _ct_graph_view, _pos);

            DialogueType = CTDialogueType.SingleChoice;

            CTChoiceSaveData choice_data = new CTChoiceSaveData()
            {
                Text = "Next Dialogue"
            };

            Choices.Add(choice_data);
        }

        public override void Draw()
        {
            base.Draw();

            /* OUTPUT CONTAINER */

            foreach (CTChoiceSaveData choice in Choices)
            {
                Port choice_port = this.CreatePort(choice.Text);

                choice_port.userData = choice;

                outputContainer.Add(choice_port);
            }

            RefreshExpandedState();
        }
    }
}
