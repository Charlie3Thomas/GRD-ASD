using System.Collections.Generic;
using UnityEngine;

namespace CT.Data.Save
{
    public class CTTreeSaveDataSO : ScriptableObject
    {
        [field: SerializeField] public string file_name { get; set; }
        [field: SerializeField] public List<CTGroupSaveData> groups { get; set; }
        [field: SerializeField] public List<CTNoteSaveData> nodes { get; set; }
        [field: SerializeField] public List<string> previous_group_names { get; set; }
        [field: SerializeField] public List<string> previous_ungrouped_node_names { get; set; }
        [field: SerializeField] public SerializableDictionary<string, List<string>> previous_grouped_node_names { get; set; }

        public void Initialise(string _name)
        {
            file_name = _name;

            groups = new List<CTGroupSaveData>();
            nodes = new List<CTNoteSaveData>();
        }
    }
}