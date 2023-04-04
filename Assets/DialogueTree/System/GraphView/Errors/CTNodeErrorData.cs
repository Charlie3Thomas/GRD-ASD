using System.Collections.Generic;

namespace CT.Data.Error
{
    using Components;

    public class CTNodeErrorData
    {
        public CTError err_data { get; set; }
        public List<CTNode> nodes { get; set; }

        public CTNodeErrorData()
        {
            err_data = new CTError();
            nodes = new List<CTNode>();
        }
    }
}