using System.Collections.Generic;
using UnityEngine;

namespace CT.ScriptableObjects
{
    public class CTDialogueContainerSO : ScriptableObject
    {
        [field: SerializeField] public string file_name { get; set; }
        [field: SerializeField] public SerializableDictionary<CTDialogueGroupSO, List<CTDialogueSO>> dictionary_dlog_groups { get; set; }
        [field: SerializeField] public List<CTDialogueSO> list_ungrouped_dlogs { get; set; }

        public void Initialise(string _filename)
        {
            file_name = _filename;

            dictionary_dlog_groups = new SerializableDictionary<CTDialogueGroupSO, List<CTDialogueSO>>();
            list_ungrouped_dlogs = new List<CTDialogueSO>();
        }
    }
}