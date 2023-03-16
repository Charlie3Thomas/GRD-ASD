using CT.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CT.Elements
{
    using Windows;
    using Enumerations;
    using Utilities;
    using Data.Save;

    public class CTMultipleChoiceNode : CTNode
    {
        public override void Initialise(CTGraphView _ct_graph_view, Vector2 _position)
        {
            base.Initialise(_ct_graph_view, _position);

            DialogueType = CTDialogueType.MultipleChoice;

            CTChoiceSaveData choice_data = new CTChoiceSaveData()
            {
                Text = "New Choice"
            };

            Choices.Add(choice_data);
        }
        public override void Draw()
        {
            base.Draw();

            // Main container
            Button add_choice_button = CTElementUtility.CreateButton("Add choice", () =>
            {
                CTChoiceSaveData choice_data = new CTChoiceSaveData()
                {
                    Text = "New Choice"
                };

                Choices.Add(choice_data);

                Port choice_port = CreateChoicePort(choice_data);

                outputContainer.Add(choice_port);
            });

            mainContainer.Insert(1, add_choice_button);


            // Output container
            foreach (CTChoiceSaveData choice in Choices)
            {
                Port choice_port = CreateChoicePort(choice);

                outputContainer.Add(choice_port);
            }

            RefreshExpandedState();
        }

        #region Elements
        private Port CreateChoicePort(object _userdata)
        {
            Port choice_port = this.CreatePort();

            choice_port.userData = _userdata;

            CTChoiceSaveData choice_data = (CTChoiceSaveData) _userdata;

            //choice_port.portName = "";

            Button delete_choice_button = CTElementUtility.CreateButton("X", () =>
            {
                /*
                    Remove choices only if there are more than one
                    If removed choice port is connected, disconnect it
                    Remove removed choice from choices list
                    Remove removed choice port from graph
                 */

                if (Choices.Count == 1)
                {
                    return;
                }

                if (choice_port.connected) 
                {
                    graph_view.DeleteElements(choice_port.connections);
                }

                Choices.Remove(choice_data);

                graph_view.RemoveElement(choice_port);

            });

            TextField choice_text_field = CTElementUtility.CreateTextField(choice_data.Text, null, callback =>
            {
                choice_data.Text = callback.newValue;
            });

            choice_port.Add(choice_text_field);
            choice_port.Add(delete_choice_button);

            outputContainer.Add(choice_port);

            return choice_port;
        }
        #endregion
    }
}