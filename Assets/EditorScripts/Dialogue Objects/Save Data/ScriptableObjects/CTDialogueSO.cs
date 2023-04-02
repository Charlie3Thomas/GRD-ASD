using System.Collections.Generic;
using UnityEngine;

namespace CT.ScriptableObjects
{
    using Data;
    using Enumerations;
    using System.Runtime.CompilerServices;

    public class CTDialogueSO : ScriptableObject
    {
        [field: SerializeField] public string dlog_title { get; set; }
        [field: SerializeField] [field: TextArea()] public string dlog_text { get; set; }
        [field: SerializeField] [field: TextArea()] public string dlog_tip_text { get; set; }
        [field: SerializeField] public List<CTDialogueChoiceData> list_dlog_choices { get; set; }
        [field: SerializeField] public CTDialogueType dlog_type { get; set; }
        [field: SerializeField] public string active_character { get; set; }
        [field: SerializeField] public string active_bg { get; set; }
        [field: SerializeField] public int char_dropdown_index { get; set; }
        [field: SerializeField] public int bg_dropdown_index { get; set; }
        [field: SerializeField] public bool is_starting_dlog { get; set; }

        public void Initialise(string _dlog_title, string _dlog, string _tip, List<CTDialogueChoiceData> _choices, CTDialogueType _type, string _active_character, int _char_dropdown_index, string _active_bg, int _bg_dropdown_index, bool _is_start)
        {
            dlog_title = _dlog_title;
            dlog_text = _dlog;
            dlog_tip_text = _tip;
            list_dlog_choices = _choices;
            dlog_type = _type;
            active_character = _active_character;
            char_dropdown_index = _char_dropdown_index;
            active_bg = _active_bg;
            bg_dropdown_index = _bg_dropdown_index;
            is_starting_dlog = _is_start;
        }
    }
}