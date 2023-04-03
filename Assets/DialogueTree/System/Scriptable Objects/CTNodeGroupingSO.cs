using UnityEngine;

namespace CT.SO
{
    public class CTNodeGroupingSO : ScriptableObject
    {
        [field: SerializeField] public string title { get; set; }

        public void Initialise(string _name)
        {
            title = _name;
        }
    }
}