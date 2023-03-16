using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CT.Elements
{
    using Windows;
    using Enumerations;
    using Utilities;
    using Data.Save;

    public class CTSingleChoiceNode : CTNode
    {
        public override void Initialise(CTGraphView _ct_graph_view, Vector2 _position)
        {
            base.Initialise(_ct_graph_view, _position);

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

            // Output container
            foreach (CTChoiceSaveData choice in Choices) 
            {
                Port choice_port = this.CreatePort(choice.Text);

                //choice_port.portName = choice.Text;

                choice_port.userData = choice;

                outputContainer.Add(choice_port);
            }

            RefreshExpandedState();
        }

    }
}
