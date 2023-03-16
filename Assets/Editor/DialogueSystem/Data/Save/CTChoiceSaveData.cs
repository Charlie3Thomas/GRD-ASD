using System;
using UnityEngine;

namespace CT.Data.Save
{
    [Serializable]
    public class CTChoiceSaveData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public string NodeID { get; set; }

    }
}
