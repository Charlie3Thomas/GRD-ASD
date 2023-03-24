using System;
using UnityEngine;

namespace CT.Data.Save
{
    [Serializable]
    public class CTChoiceSaveData
    {
        [field: SerializeField] public string text { get; set; }
        [field: SerializeField] public string node_ID { get; set; }
    }
}