using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CT.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class CTNode : Node
    {
        public string ID { get; set; }
        public string DialogueName { get; set; }
        public List<CTChoiceSaveData> Choices { get; set; }
        public string Text { get; set; }
        public string TipText { get; set; }
        public CTDialogueType DialogueType { get; set; }
        public CTGroup Group { get; set; }

        protected CTGraphView graph_view;
        private Color default_background_colour;

        public override void BuildContextualMenu(ContextualMenuPopulateEvent _evt)
        {
            _evt.menu.AppendAction("Disconnect Input Ports", action_event => DisconnectInputPorts());
            _evt.menu.AppendAction("Disconnect Output Ports", action_event => DisconnectOutputPorts());

            base.BuildContextualMenu(_evt);
        }

        public virtual void Initialize(string _node_name, CTGraphView _ct_graph_view, Vector2 _pos)
        {
            ID = Guid.NewGuid().ToString();

            DialogueName = _node_name;
            Choices = new List<CTChoiceSaveData>();
            Text = "Dialogue text.";
            TipText = "Tip text.";

            SetPosition(new Rect(_pos, Vector2.zero));

            graph_view = _ct_graph_view;
            default_background_colour = new Color(30f / 255f, 30f / 255f, 30f / 255f);
        }

        public virtual void Draw()
        {
            // Title
            TextField tf_dialogue_name = CTElementUtility.CreateTextField(DialogueName, null, callback =>
            {
                TextField target = (TextField) callback.target;
                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(DialogueName))
                    {
                        ++graph_view.NameErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(DialogueName))
                    {
                        --graph_view.NameErrorsAmount;
                    }
                }

                if (Group == null)
                {
                    graph_view.RemoveUngroupedNode(this);
                    DialogueName = target.value;
                    graph_view.AddUngroupedNode(this);

                    return;
                }

                CTGroup current_group = Group;
                graph_view.RemoveGroupedNode(this, Group);
                DialogueName = target.value;
                graph_view.AddGroupedNode(this, current_group);
            });

            titleContainer.Insert(0, tf_dialogue_name);

            // Input
            Port input_port = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(input_port);

            // Extension
            VisualElement custom_data_container = new VisualElement();

            // Dialogue text
            Foldout text_foldout = CTElementUtility.CreateFoldout("Dialogue Text");
            TextField tf_text = CTElementUtility.CreateTextArea(Text, null, callback => Text = callback.newValue);
            text_foldout.Add(tf_text);
            custom_data_container.Add(text_foldout);

            // Tip text
            Foldout tip_text_foldout = CTElementUtility.CreateFoldout("Tip Text");
            TextField tf_tip_text = CTElementUtility.CreateTextArea(TipText, null, callback => TipText = callback.newValue);
            tip_text_foldout.Add(tf_tip_text);
            custom_data_container.Add(tip_text_foldout);

            extensionContainer.Add(custom_data_container);
        }

        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }

        private void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }

        private void DisconnectPorts(VisualElement _container)
        {
            foreach (Port port in _container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }

                graph_view.DeleteElements(port.connections);
            }
        }

        public bool IsStartingNode()
        {
            Port input_port = (Port) inputContainer.Children().First();

            return !input_port.connected;
        }

        public void SetErrorStyle(Color _colour)
        {
            mainContainer.style.backgroundColor = _colour;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = default_background_colour;
        }
    }
}