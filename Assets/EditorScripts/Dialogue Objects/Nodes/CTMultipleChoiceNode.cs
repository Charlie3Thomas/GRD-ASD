using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CT.Components
{
    using Data.Save;
    using Enumerations;
    using Utils;
    using Windows;

    public class CTMultipleChoiceNode : CTNode
    {
        public override void Initialise(string _node_name, CTGraphView _ct_graph_view, Vector2 _pos)
        {
            base.Initialise(_node_name, _ct_graph_view, _pos);

            dlog_type = CTDialogueType.Choice;

            CTChoiceSaveData choice_data = new CTChoiceSaveData()
            {
                text = "\'Dialogue option leading to next node :\'"
            };

            list_dlog_choices.Add(choice_data);
        }

        public override void Draw()
        {
            base.Draw();

            // Container

            Button btn_add_choice = CTElementUtility.CreateButton("Add dialogue choice", () =>
            {
                CTChoiceSaveData choice_data = new CTChoiceSaveData()
                {
                    text = "\'Dialogue option leading to next node :\'"
                };

                list_dlog_choices.Add(choice_data);

                Port choice_port = CreateChoicePort(choice_data);

                outputContainer.Add(choice_port);
            });

            mainContainer.Insert(1, btn_add_choice);

            // Outputs

            foreach (CTChoiceSaveData choice in list_dlog_choices)
            {
                Port choice_port = CreateChoicePort(choice);

                outputContainer.Add(choice_port);
            }

            RefreshExpandedState();
        }

        private Port CreateChoicePort(object _user_data)
        {
            Port choice_port = this.CreatePort();

            choice_port.userData = _user_data;

            CTChoiceSaveData choice_data = (CTChoiceSaveData) _user_data;

            Button btn_delete_choice = CTElementUtility.CreateButton("Remove", () =>
            {
                if (list_dlog_choices.Count == 1)
                {
                    return;
                }

                if (choice_port.connected)
                {
                    graph_view.DeleteElements(choice_port.connections);
                }

                list_dlog_choices.Remove(choice_data);

                graph_view.RemoveElement(choice_port);
            });

            TextField tf_choice = CTElementUtility.CreateTextField(choice_data.text, null, callback =>
            {
                choice_data.text = callback.newValue;
            });

            choice_port.Add(tf_choice);
            choice_port.Add(btn_delete_choice);

            return choice_port;
        }
    }
}