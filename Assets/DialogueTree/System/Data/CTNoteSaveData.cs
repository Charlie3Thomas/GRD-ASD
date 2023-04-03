using System;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data.Save
{
    using Enums;

    [Serializable]
    public class CTNoteSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string name { get; set; }
        [field: SerializeField] public string text { get; set; }
        [field: SerializeField] public List<CTOptionSaveData> options { get; set; }
        [field: SerializeField] public string group_ID { get; set; }
        [field: SerializeField] public CTNodeType node_type { get; set; }
        [field: SerializeField] public Vector2 pos { get; set; }
        [field: SerializeField] public string character { get; set; }
        [field: SerializeField] public string background { get; set; }
        [field: SerializeField] public int char_dropdown_index { get; set; }
        [field: SerializeField] public int bg_dropdown_index { get; set; }
        [field: SerializeField] public string dlog_tip_text { get; set; }
    }
}