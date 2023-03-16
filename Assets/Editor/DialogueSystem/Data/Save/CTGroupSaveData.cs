using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data.Save
{
    [Serializable]
    public class CTGroupSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
    }
}
