using System;
using UnityEngine;

namespace CT.Data.Save
{
    [Serializable]
    public class CTGroupSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string name { get; set; }
        [field: SerializeField] public Vector2 pos { get; set; }
    }
}