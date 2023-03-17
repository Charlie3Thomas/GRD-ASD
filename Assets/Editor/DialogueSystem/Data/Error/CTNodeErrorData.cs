using System.Collections.Generic;

namespace CT.Data.Error
{
    using Elements;

    public class CTNodeErrorData
    {
        public CTErrorData ErrorData { get; set; }
        public List<CTNode> Nodes { get; set; }

        public CTNodeErrorData()
        {
            ErrorData = new CTErrorData();
            Nodes = new List<CTNode>();
        }
    }
}