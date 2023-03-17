using System;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data.Save
{
    using Enumerations;

    [Serializable]
    public class CTNodeSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public string TipText { get; set; }
        [field: SerializeField] public List<CTChoiceSaveData> Choices { get; set; }
        [field: SerializeField] public string GroupID { get; set; }
        [field: SerializeField] public CTDialogueType DialogueType { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
    }
}