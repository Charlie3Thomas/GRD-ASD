using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace CT.Components
{
    using CT.Utilis;
    using Data.Save;
    using Enumerations;
    using Utils;
    using Windows;

    public class CTNode : Node
    {
        public string ID { get; set; }
        public string dlog_title { get; set; }
        public List<CTChoiceSaveData> list_dlog_choices { get; set; }
        public string dlog_text { get; set; }
        public string dlog_tip_text { get; set; }
        public CTDialogueType dlog_type { get; set; }
        public CTGroup group { get; set; }
        public string active_character { get; set; }
        public int dropdown_index { get; set; }

        protected CTGraphView graph_view;
        private Color default_background_colour;

        // Character selection
        List<string> characters;

        #region Initialise

        public virtual void Initialise(string _node_name, CTGraphView _ct_graph_view, Vector2 _pos)
        {
            ID = Guid.NewGuid().ToString();

            dlog_title = _node_name;
            list_dlog_choices = new List<CTChoiceSaveData>();

            // Default text for dialogue input field in graph view
            dlog_text = "\'Dialogue prompt for choice.\'.";

            // Default text for tip input field in graph view
            // Should remain an empty string as will be used for checking if Tip UI should be viewable in game
            dlog_tip_text = "";

            characters = new List<string>();

            foreach (NodeCharacter c in (NodeCharacter[]) Enum.GetValues(typeof(NodeCharacter)))
                characters.Add(c.ToString());

            SetPosition(new Rect(_pos, Vector2.zero));

            graph_view = _ct_graph_view;
            default_background_colour = new Color(30f / 255f, 30f / 255f, 30f / 255f);

        }

        public virtual void Draw()
        {
            // Title
            TextField tf_dialogue_name = CTElementUtility.CreateTextField(dlog_title, null, callback =>
            {
                TextField target = (TextField) callback.target;
                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(dlog_title))
                    {
                        ++graph_view.NameErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(dlog_title))
                    {
                        --graph_view.NameErrorsAmount;
                    }
                }

                if (group == null)
                {
                    graph_view.RemoveUngroupedNode(this);
                    dlog_title = target.value;
                    graph_view.AddUngroupedNode(this);

                    return;
                }

                CTGroup current_group = group;
                graph_view.RemoveGroupedNode(this, group);
                dlog_title = target.value;
                graph_view.AddGroupedNode(this, current_group);
            });

            titleContainer.Insert(0, tf_dialogue_name);

            // Input
            Port input_port = this.CreatePort("Node connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(input_port);

            // Extension
            VisualElement custom_data_container = new VisualElement();

            // Character drop down selection
            //Debug.Log($"CTNode.Draw. Dropdown index is {dropdown_index}");
            DropdownField drop_down = CTElementUtility.CreateDropdownField("Character", characters, dropdown_index);
            drop_down.RegisterValueChangedCallback(OnDropDownValueChanged);
            //DropdownField drop_down = new DropdownField();
            //drop_down = CTElementUtility.CreateDropdownField("Character", characters, dropdown_index, callback => drop_down.index = callback.newValue.index);
            //drop_down.index = dropdown_index;
            //drop_down.RegisterValueChangedCallback(OnDropDownValueChanged);
            custom_data_container.Add(drop_down);

            // Dialogue text
            Foldout text_foldout = CTElementUtility.CreateFoldout("Node Dialogue");
            TextField tf_text = CTElementUtility.CreateTextArea(dlog_text, null, callback => dlog_text = callback.newValue);
            text_foldout.Add(tf_text);
            custom_data_container.Add(text_foldout);

            // Tip text
            Foldout tip_text_foldout = CTElementUtility.CreateFoldout("Node Tip");
            TextField tf_tip_text = CTElementUtility.CreateTextArea(dlog_tip_text, null, callback => dlog_tip_text = callback.newValue);
            tip_text_foldout.Add(tf_tip_text);
            custom_data_container.Add(tip_text_foldout);            

            extensionContainer.Add(custom_data_container);
        }

        

        // Creates override context menu on right click for nodes
        public override void BuildContextualMenu(ContextualMenuPopulateEvent _event)
        {
            // Context menu options with coresponding callback functions
            _event.menu.AppendAction("Disconnect Input Port", action_event => DisconnectInputPorts());
            _event.menu.AppendAction("Disconnect Output Port(s)", action_event => DisconnectOutputPorts());

            base.BuildContextualMenu(_event);
        }

        #endregion

        #region Util functions

        public bool IsStartingNode()
        {
            // Returns whether a node is the starting node in a tree
            Port input_port = (Port) inputContainer.Children().First();

            // If the input port is not connected, it should be the starting node
            // There should be only one node with no connected inpur port
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

        public void DisconnectAllPorts()
        {
            // Disconnects all IO ports on a node/selected nodes
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        private void DisconnectInputPorts()
        {
            // Disconnects all input ports on a node/selected nodes
            DisconnectPorts(inputContainer);
        }

        private void DisconnectOutputPorts()
        {
            // Disconnects all output ports on a node/selected nodes
            DisconnectPorts(outputContainer);
        }

        private void OnDropDownValueChanged(ChangeEvent<string> _event)
        {
            //Debug.LogError($"Dropdown value changed to {_event.newValue}");
            active_character = _event.newValue;

            for (int i = 0; i < characters.Count; i++)
            {
                //Debug.LogError($"{active_character} {characters[i]}");
                if (active_character == characters[i])
                {
                    //Debug.Log("Match found!");
                    dropdown_index = i;
                    //Debug.Log("Dropdown index set to " + dropdown_index);
                }
            }
        }

        #endregion
    }
}