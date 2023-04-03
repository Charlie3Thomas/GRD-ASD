using System;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data.Save
{
    using Enumerations;

    [Serializable]
    public class CTNodeSaveData
    {
        [SerializeField] public string ID { get; set; }
        [SerializeField] public string title { get; set; }
        [SerializeField] public string dlog_text { get; set; }
        [SerializeField] public string dlog_tip_text { get; set; }
        [SerializeField] public List<CTChoiceSaveData> list_choices { get; set; }
        [SerializeField] public string group_ID { get; set; }
        [SerializeField] public string character { get; set; }
        [SerializeField] public string background { get; set; }
        [SerializeField] public int char_dropdown_index { get; set; }
        [SerializeField] public int bg_dropdown_index { get; set; }
        [SerializeField] public CTDialogueType dlog_type { get; set; }
        [SerializeField] public Vector2 pos { get; set; }
    }
}