using System.Collections.Generic;
using UnityEngine;

namespace CT.ScriptableObjects
{
    public class CTDialogueContainerSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public SerializableDictionary<CTDialogueGroupSO, List<CTDialogueSO>> DialogueGroups { get; set; }
        [field: SerializeField] public List<CTDialogueSO> UngroupedDialogues { get; set; }

        public void Initialize(string _filename)
        {
            FileName = _filename;

            DialogueGroups = new SerializableDictionary<CTDialogueGroupSO, List<CTDialogueSO>>();
            UngroupedDialogues = new List<CTDialogueSO>();
        }

        public List<string> GetDialogueGroupNames()
        {
            List<string> dlog_group_names = new List<string>();

            foreach (CTDialogueGroupSO dlog_group in DialogueGroups.Keys)
            {
                dlog_group_names.Add(dlog_group.GroupName);
            }

            return dlog_group_names;
        }

        public List<string> GetGroupedDialogueNames(CTDialogueGroupSO _dlog_group, bool _start_dlogs_only)
        {
            List<CTDialogueSO> grouped_dlogs = DialogueGroups[_dlog_group];
            List<string> grouped_dlog_names = new List<string>();

            foreach (CTDialogueSO grouped_dlog in grouped_dlogs)
            {
                if (_start_dlogs_only && !grouped_dlog.IsStartingDialogue)
                {
                    continue;
                }

                grouped_dlog_names.Add(grouped_dlog.DialogueName);
            }

            return grouped_dlog_names;
        }

        public List<string> GetUngroupedDialogueNames(bool _start_dlogs_only)
        {
            List<string> ungrouped_dlog_names = new List<string>();

            foreach (CTDialogueSO ungrouped_dlog in UngroupedDialogues)
            {
                if (_start_dlogs_only && !ungrouped_dlog.IsStartingDialogue)
                {
                    continue;
                }

                ungrouped_dlog_names.Add(ungrouped_dlog.DialogueName);
            }

            return ungrouped_dlog_names;
        }
    }
}