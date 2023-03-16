using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CT.Utilities
{
    using CT.Elements;
    public static class CTElementUtility
    {
        public static Button CreateButton(string _text, Action _on_click = null)
        {
            Button button = new Button(_on_click)
            {
                text = _text
            };

            return button;

        }
        public static Foldout CreateFoldout(string _title, bool _collapsed = false)
        {
            Foldout foldout = new Foldout()
            {
                text = _title,
                value = _collapsed
            };

            return foldout; 
        }

        public static Port CreatePort(this CTNode _node, string _port_name = "", 
                                        Orientation _orientation = Orientation.Horizontal, 
                                        Direction _direction = Direction.Output, 
                                        Port.Capacity _capacity = Port.Capacity.Single)
        {
            Port port = _node.InstantiatePort(_orientation, _direction, _capacity, typeof(bool));
            port.portName = _port_name;


            return port;
        }

        public static TextField CreateTextField(string _value = null, string _label = null, EventCallback<ChangeEvent<string>> _on_value_changed = null)
        {
            TextField text_field = new TextField()
            {
                value = _value,
                label = _label
            };

            if (_on_value_changed != null)
            {
                text_field.RegisterCallback(_on_value_changed);
            }

            return text_field;
        }

        public static TextField CreateTextArea(string _value = null, string _label = null, EventCallback<ChangeEvent<string>> _on_value_changed = null)
        {
            TextField text_area = CreateTextField(_value, _label, _on_value_changed);
            text_area.multiline = true;
            return text_area;
        }
    }
}
