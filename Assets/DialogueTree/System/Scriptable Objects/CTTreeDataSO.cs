using System.Collections.Generic;
using UnityEngine;

namespace CT.SO
{
    public class CTTreeDataSO : ScriptableObject
    {
        [field: SerializeField] public string file_name { get; set; }
        [field: SerializeField] public SerializableDictionary<CTNodeGroupingSO, List<CTNodeDataSO>> node_groups { get; set; }
        [field: SerializeField] public List<CTNodeDataSO> ungrouped_nodes { get; set; }

        public void Initialise(string _name)
        {
            file_name = _name;

            node_groups = new SerializableDictionary<CTNodeGroupingSO, List<CTNodeDataSO>>();
            ungrouped_nodes = new List<CTNodeDataSO>();
        }
    }
}