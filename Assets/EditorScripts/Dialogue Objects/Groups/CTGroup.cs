using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CT.Components
{
    public class CTGroup : Group
    {
        public string ID { get; set; }
        public string previous_title { get; set; }

        private Color default_boarder_colour;
        private float default_boarder_width;

        public CTGroup(string _group_title, Vector2 _pos)
        {
            ID = Guid.NewGuid().ToString();

            title = _group_title;
            previous_title = _group_title;

            SetPosition(new Rect(_pos, Vector2.zero));

            default_boarder_colour = contentContainer.style.borderBottomColor.value;
            default_boarder_width = contentContainer.style.borderBottomWidth.value;
        }

        public void SetErrorStyle(Color _colour)
        {
            contentContainer.style.borderBottomColor = _colour;
            contentContainer.style.borderBottomWidth = 3.0f;
        }

        public void ResetStyle()
        {
            contentContainer.style.borderBottomColor = default_boarder_colour;
            contentContainer.style.borderBottomWidth = default_boarder_width;
        }
    }
}