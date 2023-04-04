using System;
using UnityEngine;

namespace CT.Data.Save
{
    [Serializable]
    public class CTOptionSaveData
    {
        [field: SerializeField] public string text { get; set; }
        [field: SerializeField] public string node_ID { get; set; }
    }
}