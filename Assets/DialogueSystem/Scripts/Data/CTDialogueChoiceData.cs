using System;
using UnityEngine;

namespace CT.Data
{
    using ScriptableObjects;

    [Serializable]
    public class CTDialogueChoiceData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public CTDialogueSO NextDialogue { get; set; }
    }
}