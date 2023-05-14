using System;
using UnityEngine;

namespace CT.Data
{
    using SO;

    [Serializable]
    public class CTNodeOptionData
    {
        [field: SerializeField] public string text { get; set; }
        [field: SerializeField] public CTNodeDataSO next_node { get; set; }
    }
}