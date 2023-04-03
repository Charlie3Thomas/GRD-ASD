using System.Collections.Generic;
using UnityEngine;

namespace CT.Data.Save
{
    public class CTGraphSaveDataSO : ScriptableObject
    {
        [field: SerializeField] public string file_name { get; set; }
        [field: SerializeField] public List<CTGroupSaveData> list_groups { get; set; }
        [field: SerializeField] public List<CTNodeSaveData> list_nodes { get; set; }
        [field: SerializeField] public List<string> list_previous_group_titles { get; set; }
        [field: SerializeField] public List<string> list_previous_ungrouped_titles { get; set; }
        [field: SerializeField] public SerializableDictionary<string, List<string>> list_previous_grouped_titles { get; set; }

        public void Initialise(string _file_name)
        {
            file_name = _file_name;

            list_groups = new List<CTGroupSaveData>();

            list_nodes = new List<CTNodeSaveData>();
        }
    }
}