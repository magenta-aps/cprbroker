using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public class CurrentPnrTypeAdaptor : IPnr
    {
        public PersonInformationType PersonInformation { get; private set; }

        public CurrentPnrTypeAdaptor(PersonInformationType info)
        {
            PersonInformation = info;
        }

        public string ToPnr()
        {
            return this.PersonInformation.PNR;
        }

        public string Tag
        {
            get { return CprBroker.Utilities.Constants.DataTypeTags.PNR; }
        }

        public DateTime? ToEndTS()
        {
            return Converters.ToDateTime(this.PersonInformation.PersonEndDate, this.PersonInformation.PersonEndDateUncertainty);
        }

        public DateTime? ToStartTS()
        {
            return Converters.ToDateTime(this.PersonInformation.PersonStartDate, this.PersonInformation.PersonStartDateUncertainty);
        }
    }
}
