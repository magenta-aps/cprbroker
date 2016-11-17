using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.PartInterface.Tracking
{
    [Serializable]
    public class CleanupSettings
    {
        public CleanupSettings()
        { }

        public string MunicipalityCode { get; set; }
        public int DprDays { get; set; }
    }
}
