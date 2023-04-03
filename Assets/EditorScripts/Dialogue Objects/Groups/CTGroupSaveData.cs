using System;
using UnityEngine;

namespace CT.Data.Save
{
    [Serializable]
    public class CTGroupSaveData
    {
        [SerializeField] public string ID { get; set; }
        [SerializeField] public string title { get; set; }
        [SerializeField] public Vector2 pos { get; set; }
    }
}