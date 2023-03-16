using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CT.Data.Error
{
    using CT.Elements;
    public class CTGroupErrorData
    {
        public CTErrorData err_data { get; set; }
        public List<CTGroup> groups { get; set; } 

        public CTGroupErrorData()
        {
            err_data = new CTErrorData();
            groups = new List<CTGroup>();
        }
    }
}
