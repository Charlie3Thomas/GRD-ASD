using System.Collections.Generic;

namespace CT.Data.Error
{
    using Components;

    public class CTGroupErrorData
    {
        public CTErrorData err_data { get; set; }
        public List<CTGroup> list_groups { get; set; }

        public CTGroupErrorData()
        {
            err_data = new CTErrorData();
            list_groups = new List<CTGroup>();
        }
    }
}