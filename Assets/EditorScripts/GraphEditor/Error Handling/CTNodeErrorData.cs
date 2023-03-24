using System.Collections.Generic;

namespace CT.Data.Error
{
    using Components;

    public class CTNodeErrorData
    {
        public CTErrorData err_data { get; set; }
        public List<CTNode> list_nodes { get; set; }

        public CTNodeErrorData()
        {
            err_data = new CTErrorData();
            list_nodes = new List<CTNode>();
        }
    }
}