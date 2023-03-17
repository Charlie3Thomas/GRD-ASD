using System.Collections.Generic;

namespace CT.Data.Error
{
    using Elements;

    public class CTGroupErrorData
    {
        public CTErrorData ErrorData { get; set; }
        public List<CTGroup> Groups { get; set; }

        public CTGroupErrorData()
        {
            ErrorData = new CTErrorData();
            Groups = new List<CTGroup>();
        }
    }
}