using UnityEngine;

namespace CT.Data.Error
{
    public class CTErrorData
    {
        public Color colour { get; set; }

        public CTErrorData()
        {
            GenerateRandomColor();
        }

        private void GenerateRandomColor()
        {
            colour = new Color32(
                (byte) Random.Range(0, 256),
                (byte) Random.Range(0, 256),
                (byte) Random.Range(0, 256),
                255 // Alpha
            );
        }
    }
}