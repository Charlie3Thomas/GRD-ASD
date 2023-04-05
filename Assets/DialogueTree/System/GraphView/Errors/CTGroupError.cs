#if UNITY_EDITOR
using System.Collections.Generic;

namespace CT.Data.Error
{
    using Components;

    public class CTGroupError
    {
        public CTError error { get; set; }
        public List<CTGroup> groups { get; set; }

        public CTGroupError()
        {
            error = new CTError();
            groups = new List<CTGroup>();
        }
    }
}
#endif