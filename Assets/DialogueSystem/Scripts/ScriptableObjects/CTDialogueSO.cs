using System.Collections.Generic;
using UnityEngine;

namespace CT.ScriptableObjects
{
    using Data;
    using Enumerations;

    public class CTDialogueSO : ScriptableObject
    {
        [field: SerializeField] public string DialogueName { get; set; }
        [field: SerializeField] [field: TextArea()] public string Text { get; set; }
        [field: SerializeField] [field: TextArea()] public string TipText { get; set; }
        [field: SerializeField] public List<CTDialogueChoiceData> Choices { get; set; }
        [field: SerializeField] public CTDialogueType DialogueType { get; set; }
        [field: SerializeField] public bool IsStartingDialogue { get; set; }

        public void Initialize(string _dlog_name, string _text, string _tip_text, List<CTDialogueChoiceData> _choices, CTDialogueType _dlog_type, bool _is_dlog_start)
        {
            DialogueName = _dlog_name;
            Text = _text;
            TipText = _tip_text;
            Choices = _choices;
            DialogueType = _dlog_type;
            IsStartingDialogue = _is_dlog_start;
        }
    }
}