using CprBroker.Schemas.Part;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Providers.DPR
{
    public partial class Disappearance : IHasNullableCorrectionMarker
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
                return DataTypeTags.Address;
            }
        }

        public DateTime? ToStartTS()
        {
            return Utilities.DateFromDecimal(DisappearanceDate);
        }

        public bool ToStartTSCertainty()
        {
            return true;
        }

        public DateTime? ToEndTS()
        {
            return Utilities.DateFromDecimal(RetrievalDate);
        }

        public bool ToEndTSCertainty()
        {
            return true;
        }

    }
}
