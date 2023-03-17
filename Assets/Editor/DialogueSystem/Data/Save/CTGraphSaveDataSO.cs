using System.Collections.Generic;
using UnityEngine;

namespace CT.Data.Save
{
    public class CTGraphSaveDataSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public List<CTGroupSaveData> Groups { get; set; }
        [field: SerializeField] public List<CTNodeSaveData> Nodes { get; set; }
        [field: SerializeField] public List<string> OldGroupNames { get; set; }
        [field: SerializeField] public List<string> OldUngroupedNodeNames { get; set; }
        [field: SerializeField] public SerializableDictionary<string, List<string>> OldGroupedNodeNames { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;

            Groups = new List<CTGroupSaveData>();
            Nodes = new List<CTNodeSaveData>();
        }
    }
}