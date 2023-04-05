#if UNITY_EDITOR
using UnityEngine;

namespace CT.Data.Error
{
    public class CTError
    {
        public Color colour { get; set; }

        public CTError()
        {
            SetRandomColour();
        }

        private void SetRandomColour()
        {
            colour = new Color32(
                (byte)Random.Range(0, 256),
                (byte)Random.Range(0, 256),
                (byte)Random.Range(0, 256),
                255
            );
        }
    }
}
#endif