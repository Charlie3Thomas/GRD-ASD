using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace CT.Elements
{
    using Windows;
    using Enumerations;
    using Utilities;
    using Data.Save;
    using System.Linq;

    public class CTNode : Node
    {
        public string ID;
        public string DialogueName { get; set; }
        public List<CTChoiceSaveData> Choices { get; set; }
        public string Text { get; set; }
        public CTDialogueType DialogueType { get; set; }

        public CTGroup group { get; set; }

        protected CTGraphView graph_view;

        private Color default_bg;

        public virtual void Initialise(CTGraphView _ct_graph_view, Vector2 _position)
        {
            ID = Guid.NewGuid().ToString();
            DialogueName = "DialogueName";
            Choices = new List<CTChoiceSaveData>();
            Text = "Dialogue text";

            graph_view = _ct_graph_view;

            default_bg = new Color(29.0f / 255.0f, 29.0f / 255.0f, 30.0f / 255.0f);

            SetPosition(new Rect(_position, Vector2.zero));
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect all Input Ports", actionEvent => DisconnectAllInputPorts());
            evt.menu.AppendAction("Disconnect all Output Ports", actionEvent => DisconnectAllOutputPorts());

            base.BuildContextualMenu(evt);
        }

        public virtual void Draw()
        {
            // Title container
            TextField dialogue_name_textfield = CTElementUtility.CreateTextField(DialogueName, null, callback =>
            {
                TextField target = (TextField) callback.target;

                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                // Check filename valid for node
                if (string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(DialogueName))
                    {
                        ++graph_view.ID_error_total;
                    }
                }
                else
                {
                    // If filename is valid
                    if (string.IsNullOrEmpty(DialogueName))
                    {
                        --graph_view.ID_error_total;
                    }
                }

                if (group == null) 
                {
                    graph_view.RemoveUngroupedNode(this);
                    DialogueName = target.value;
                    graph_view.AddUngroupedNode(this);
                    return;
                }

                CTGroup current_group = group;

                graph_view.RemoveGroupedNode(this, group);

                DialogueName = callback.newValue;

                graph_view.AddGroupedNode(this, current_group);
            });


            // Input container
            titleContainer.Insert(0, dialogue_name_textfield);

            Port input_port = this.CreatePort("Dialogue connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            input_port.portName = "Dialogue Connection";

            inputContainer.Add(input_port);


            // Extensions container
            VisualElement custom_data_container = new VisualElement();

            Foldout text_foldout = CTElementUtility.CreateFoldout("Dialogue Text");

            TextField text_textfield = CTElementUtility.CreateTextArea(Text, null, callback =>
            {
                Text = callback.newValue;
            });

            text_foldout.Add(text_textfield);
            custom_data_container.Add(text_foldout);
            extensionContainer.Add(custom_data_container);

            RefreshExpandedState();

        }

        public void DisconnectAllPorts()
        {
            DisconnectAllInputPorts();
            DisconnectAllOutputPorts();
        }

        public void DisconnectAllInputPorts()
        {
            DisconnectPorts(inputContainer);    
        }

        public void DisconnectAllOutputPorts()
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

        public bool IsStartNode()
        {
            // If input port is not connected, this is a start node
            Port inputPort = (Port) inputContainer.Children().First();
            return !inputPort.connected;
        }

        public void SetErrorStyle(Color _colour)
        {
            mainContainer.style.backgroundColor = _colour;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = default_bg;
        }

    }
}
