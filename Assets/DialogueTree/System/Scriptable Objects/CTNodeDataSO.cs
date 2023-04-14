using System.Collections.Generic;
using UnityEngine;

namespace CT.SO
{
    using Data;
    using Enums;

    public class CTNodeDataSO : ScriptableObject
    {
        [field: SerializeField] public string node_name { get; set; }
        [field: SerializeField] public string text { get; set; }
        [field: SerializeField] public string character_name { get; set; }
        [field: SerializeField] public List<CTNodeOptionData> options { get; set; }
        [field: SerializeField] public CTNodeType type { get; set; }
        [field: SerializeField] public bool is_start { get; set; }
        [field: SerializeField] public string tip_text { get; set; }
        [field: SerializeField] public string character { get; set; }
        [field: SerializeField] public string background { get; set; }
        [field: SerializeField] public int char_dropdown_index { get; set; }
        [field: SerializeField] public int bg_dropdown_index { get; set; }

        public void Initialise(string _name, string _text, string _character_name, string _tip,
            List<CTNodeOptionData> _options, CTNodeType _type, bool _is_start,
            string _character, string _background, int _char_dropdown_index,
            int _bg_dropdown_index)
        {
            this.node_name = _name;
            this.text = _text;
            this.character_name = _character_name;
            this.options = _options;
            this.type = _type;
            this.is_start = _is_start;
            this.tip_text = _tip;
            this.character = _character;
            this.background = _background;
            this.char_dropdown_index = _char_dropdown_index;
            this.bg_dropdown_index = _bg_dropdown_index;
        }
    }
}