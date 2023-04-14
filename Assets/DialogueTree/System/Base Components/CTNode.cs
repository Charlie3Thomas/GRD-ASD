#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CT.Components
{
    using Data.Save;
    using Enums;
    using Utils;
    using GraphView;

    public class CTNode : Node
    {
        public string ID { get; set; }
        public string node_name { get; set; }
        public string character_name { get; set; }
        public List<CTOptionSaveData> options { get; set; }
        public string text { get; set; }
        public string tip_text { get; set; }
        public CTNodeType node_type { get; set; }
        public CTGroup group { get; set; }
        public string character { get; set; }
        public string background { get; set; }
        public int char_dropdown_index { get; set; }
        public int bg_dropdown_index { get; set; }

        protected CTGraphView graph_view;
        private Color bg_colour;

        // Character selection
        private List<string> characters;
        private List<string> backgrounds;

        public virtual void Initialise(string _name, CTGraphView _view, Vector2 _pos)
        {
            ID = Guid.NewGuid().ToString();

            node_name = _name;
            options = new List<CTOptionSaveData>();
            text = "Dialogue text.";

            SetPosition(new Rect(_pos, Vector2.zero));

            graph_view = _view;
            bg_colour = new Color(30.0f / 255f, 30.0f / 255f, 30.0f / 255f);

            characters = new List<string>();
            backgrounds = new List<string>();

            foreach (NodeCharacter c in (NodeCharacter[])Enum.GetValues(typeof(NodeCharacter)))
                characters.Add(c.ToString());

            foreach (Backgrounds bg in (Backgrounds[])Enum.GetValues(typeof(Backgrounds)))
                backgrounds.Add(bg.ToString());

        }

        public virtual void Draw()
        {

            TextField input = CTComponentUtility.CreateTextField(node_name, null, callback =>
            {
                TextField target = (TextField)callback.target;

                target.value = callback.newValue;

                if (string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(node_name))
                    {
                        ++graph_view.TotalErrors;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(node_name))
                    {
                        --graph_view.TotalErrors;
                    }
                }

                if (group == null)
                {
                    graph_view.RemoveUngroupedNode(this);

                    node_name = target.value;

                    graph_view.AddUngroupedNode(this);

                    return;
                }

                CTGroup this_group = group;

                graph_view.RemoveGroupedNode(this, group);

                node_name = target.value;

                graph_view.AddGroupedNode(this, this_group);
            });


            titleContainer.Insert(0, input);

            Port in_port = this.CreatePort("Node Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(in_port);

            VisualElement visual_element = new VisualElement();

            // Background drop down selection
            DropdownField bg_drop_down = CTComponentUtility.CreateDropdownField("Background", backgrounds, bg_dropdown_index);
            bg_drop_down.RegisterValueChangedCallback(OnBGDropDownValueChanged);
            visual_element.Add(bg_drop_down);

            // Character drop down selection
            //Debug.Log($"CTNode.Draw. Dropdown index is {dropdown_index}");
            DropdownField char_drop_down = CTComponentUtility.CreateDropdownField("Character", characters, char_dropdown_index);
            char_drop_down.RegisterValueChangedCallback(OnCharacterDropDownValueChanged);
            //DropdownField drop_down = new DropdownField();
            //drop_down = CTElementUtility.CreateDropdownField("Character", characters, dropdown_index, callback => drop_down.index = callback.newValue.index);
            //drop_down.index = dropdown_index;
            //drop_down.RegisterValueChangedCallback(OnDropDownValueChanged);
            visual_element.Add(char_drop_down);

            // Character name text field
            Foldout character_name_foldout = CTComponentUtility.CreateFoldout("Character Name");
            TextField tf_character_name = CTComponentUtility.CreateTextField(character_name, null, callback => character_name = callback.newValue);
            character_name_foldout.Add(tf_character_name);
            visual_element.Add(character_name_foldout);

            Foldout textFoldout = CTComponentUtility.CreateFoldout("Node Text");
            TextField textTextField = CTComponentUtility.CreateTextArea(text, null, callback => text = callback.newValue);
            textFoldout.Add(textTextField);
            visual_element.Add(textFoldout);

            // Tip text
            Foldout tip_text_foldout = CTComponentUtility.CreateFoldout("Node Tip");
            TextField tf_tip_text = CTComponentUtility.CreateTextArea(tip_text, null, callback => tip_text = callback.newValue);
            tip_text_foldout.Add(tf_tip_text);
            visual_element.Add(tip_text_foldout);

            extensionContainer.Add(visual_element);
        }

        public bool IsStartingNode()
        {
            Port in_port = (Port)inputContainer.Children().First();

            return !in_port.connected;
        }

        public void SetErrorColour(Color _colour)
        {
            mainContainer.style.backgroundColor = _colour;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = bg_colour;
        }

        private void OnCharacterDropDownValueChanged(ChangeEvent<string> _event)
        {
            //Debug.LogError($"Dropdown value changed to {_event.newValue}");
            character = _event.newValue;

            for (int i = 0; i < characters.Count; i++)
            {
                //Debug.LogError($"{active_character} {characters[i]}");
                if (character == characters[i])
                {
                    //Debug.Log("Match found!");
                    char_dropdown_index = i;
                    //Debug.Log("Dropdown index set to " + dropdown_index);
                }
            }
        }

        private void OnBGDropDownValueChanged(ChangeEvent<string> _event)
        {
            //Debug.LogError($"Dropdown value changed to {_event.newValue}");
            background = _event.newValue;

            for (int i = 0; i < backgrounds.Count; i++)
            {
                //Debug.LogError($"{active_character} {characters[i]}");
                if (background == backgrounds[i])
                {
                    //Debug.Log("Match found!");
                    bg_dropdown_index = i;
                    //Debug.Log("Dropdown index set to " + dropdown_index);
                }
            }
        }

        #region Context menu
        public override void BuildContextualMenu(ContextualMenuPopulateEvent _event)
        {
            _event.menu.AppendAction("Disconnect Inputs", actionEvent => DisconnectInPorts());
            _event.menu.AppendAction("Disconnect Outputs", actionEvent => DisconnectOutPorts());

            base.BuildContextualMenu(_event);
        }

        public void DisconnectAllPorts()
        {
            DisconnectInPorts();
            DisconnectOutPorts();
        }

        private void DisconnectInPorts()
        {
            DisconnectPorts(inputContainer);
        }

        private void DisconnectOutPorts()
        {
            DisconnectPorts(outputContainer);
        }

        private void DisconnectPorts(VisualElement _element)
        {
            foreach (Port p in _element.Children())
            {
                if (!p.connected)
                {
                    continue;
                }

                graph_view.DeleteElements(p.connections);
            }
        }

        #endregion
    }
}

public enum NodeCharacter
{
    None,
    Narrator,
    Character_0,
    Character_1,
    Character_2,
    Character_3,
    Character_4,
    Character_5,
    Character_6,
    Character_7,
    Character_8,
    Character_9,
}

public enum Backgrounds
{
    None,
    Background_0,
    Background_1,
    Background_2,
    Background_3,
    Background_4
}
#endif