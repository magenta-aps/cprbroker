using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;

namespace CprBroker.Providers.DPR
{
    public partial class CivilStatus : ICivilStatus
    {
        public bool IsValid()
        {
            return CorrectionMarker == null
                && !string.IsNullOrEmpty(this.ToSpousePnr());
        }

        string ICivilStatus.PNR
        {
            get { return this.PNR.ToPnrDecimalString(); }
        }

        public DateTime? ToStartTS()
        {
            return Utilities.DateFromDecimal(this.MaritalStatusDate);
        }
        
        public bool ToStartTSCertainty()
        {
            return true;
        }

        public DateTime? ToEndTS()
        {
            return Utilities.DateFromDecimal(this.MaritalEndDate);
        }

        public bool ToEndTSCertainty()
        {
            return true;
        }

        DataTypeTags ITimedType.Tag
        {
            get { return DataTypeTags.CivilStatus; }
        }

        public string ToSpousePnr()
        {
            return this.SpousePNR.HasValue && this.SpousePNR > 0 ?
                this.SpousePNR.Value.ToPnrDecimalString()
                : null;
        }

        public char CivilStatusCode
        {
            get { return this.MaritalStatus.Value; }
        }


        public IRegistrationInfo Registration
        {
            get { return null; }
        }
    }
}
