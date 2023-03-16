using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CT
{
    public class CTGroup : Group
    {
        public string ID { get; set; }
        public string old_title { get; set; }
        private Color def_boarder_colour;
        private float def_boarder_width;

        public CTGroup(string _group_title, Vector2 _position)
        {
            ID = Guid.NewGuid().ToString();
            title = _group_title;
            old_title = _group_title;

            SetPosition(new Rect(_position, Vector2.zero));

            def_boarder_colour = contentContainer.style.borderBottomColor.value;
            def_boarder_width = contentContainer.style.borderBottomWidth.value;

        }

        public void SetErrStyle(Color _colour)
        {
            contentContainer.style.borderBottomColor = _colour;
            contentContainer.style.borderBottomWidth = 1.5f;
        }

        public void ResetStyle()
        {
            contentContainer.style.borderBottomColor = def_boarder_colour;
            contentContainer.style.borderBottomWidth = def_boarder_width;
        }
    }
}
