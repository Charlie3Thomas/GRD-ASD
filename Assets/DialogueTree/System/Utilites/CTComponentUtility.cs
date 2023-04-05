using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace CT.Utils
{
    using Components;

    public static class CTComponentUtility
    {
        public static TextField CreateTextField(string _value = null, string _lable = null, EventCallback<ChangeEvent<string>> _on_changed = null)
        {
            TextField text_field = new TextField()
            {
                value = _value,
                label = _lable
            };

            if (_on_changed != null)
            {
                text_field.RegisterValueChangedCallback(_on_changed);
            }

            return text_field;
        }
        public static TextField CreateTextArea(string _value = null, string _label = null, EventCallback<ChangeEvent<string>> _on_changed = null)
        {
            TextField text_field = CreateTextField(_value, _label, _on_changed);

            text_field.multiline = true;

            return text_field;
        }
        public static Foldout CreateFoldout(string _title, bool _folded = false)
        {
            Foldout foldout = new Foldout();
            foldout.text = _title;
            foldout.value = !_folded;

            return foldout;
        }
        public static DropdownField CreateDropdownField(string _title, List<string> _options, int _index, EventCallback<ChangeEvent<DropdownField>> _on_value_changed = null)
        {
            DropdownField drop_down = new DropdownField()
            {
                choices = _options,
                index = _index
            };

            drop_down.label = _title;

            return drop_down;
        }
        public static Button CreateButton(string _text, Action _action = null)
        {
            Button button = new Button(_action);
            button.text = _text;

            return button;
        }
        public static Port CreatePort(this CTNode _node, string _name = "", Orientation _orientation = Orientation.Horizontal, Direction _direction = Direction.Output, Port.Capacity _capacity = Port.Capacity.Single)
        {
            if (_name == null)
                _name = string.Empty;

            Port port = _node.InstantiatePort(_orientation, _direction, _capacity, typeof(bool));

            port.portName = _name;

            return port;
        }

    }
}