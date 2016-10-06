using CprBroker.Schemas.Part;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Providers.DPR
{
    public partial class Separation : IHasNullableCorrectionMarker
    {
        public IRegistrationInfo Registration
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DataTypeTags Tag
        {
            get
            {
                return DataTypeTags.Separation;
            }
        }

        public DateTime? ToStartTS()
        {
            return this.StartDate;
        }

        public bool ToStartTSCertainty()
        {
            return true;
        }

        public DateTime? ToEndTS()
        {
            return this.EndDate;
        }

        public bool ToEndTSCertainty()
        {
            return true;
        }
    }
}
