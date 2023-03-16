using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Permissions;

namespace CT.Data.Error
{
    using CT.Elements;

    public class CTNodeErrorData
    {
        public CTErrorData error_data { get; set; }
        public List<CTNode> nodes { get; set; }

        public CTNodeErrorData() 
        { 
            error_data = new CTErrorData();
            nodes = new List<CTNode>();
        }
    }
}
