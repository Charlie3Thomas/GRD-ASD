using System;
using UnityEngine;

namespace CT.Data
{
    using ScriptableObjects;

    [Serializable]
    public class CTDialogueChoiceData
    {
        [field: SerializeField] public string text { get; set; }
        [field: SerializeField] public CTDialogueSO next_dlog_node { get; set; }
    }
}