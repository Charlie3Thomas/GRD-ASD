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
        [field: SerializeField] public string title { get; set; }
        [field: SerializeField] public string dlog_text { get; set; }
        [field: SerializeField] public string dlog_tip_text { get; set; }
        [field: SerializeField] public List<CTChoiceSaveData> list_choices { get; set; }
        [field: SerializeField] public string group_ID { get; set; }
        [field: SerializeField] public CTDialogueType dlog_type { get; set; }
        [field: SerializeField] public Vector2 pos { get; set; }
    }
}