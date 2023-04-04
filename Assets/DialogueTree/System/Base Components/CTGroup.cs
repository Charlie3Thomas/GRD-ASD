using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CT.Components
{
    public class CTGroup : Group
    {
        public string ID { get; set; }
        public string old_name { get; set; }

        private Color boarder_colour;
        private float boarder_thickness;

        public CTGroup(string _title, Vector2 _pos)
        {
            ID = Guid.NewGuid().ToString();

            title = _title;
            old_name = _title;

            SetPosition(new Rect(_pos, Vector2.zero));

            boarder_colour = contentContainer.style.borderBottomColor.value;
            boarder_thickness = contentContainer.style.borderBottomWidth.value;
        }

        public void SetErrorColour(Color _colour)
        {
            contentContainer.style.borderBottomColor = _colour;
            contentContainer.style.borderBottomWidth = 5.0f;
        }

        public void ClearErrorColour()
        {
            contentContainer.style.borderBottomColor = boarder_colour;
            contentContainer.style.borderBottomWidth = boarder_thickness;
        }
    }
}