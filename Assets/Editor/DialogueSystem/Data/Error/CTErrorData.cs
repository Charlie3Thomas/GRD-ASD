using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data.Error
{
    public class CTErrorData
    {
        public Color color { get; set; }

        public CTErrorData()
        {
            GenerateRandomColour();
        }

        private void GenerateRandomColour()
        {
            color = new Color32
            (
                (byte)Random.Range(0, 256),
                (byte)Random.Range(0, 256),
                (byte)Random.Range(0, 256),
                255
            );
        }
    }
}
