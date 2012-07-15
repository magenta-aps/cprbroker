using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class CurrentAddressInformationType
    {
        public VirkningType ToArrivalDepartureVirkning()
        {
            return VirkningType.Create(
                Converters.ToDateTime(this.MunicipalityArrivalDate, this.MunicipalityArrivalDateUncertainty),
                null
                );
        }

        public VirkningType ToVirkningType()
        {
            return VirkningType.Create(
                this.StartDate,
                this.EndDate
                );
        }

        public VirkningType ToRelocationVirkning()
        {

            return VirkningType.Create(
                Converters.ToDateTime(this.RelocationDate, this.RelocationDateUncertainty),
                null
                );
        }

        public VirkningType[] ToVirkningTypeArray()
        {
            return new VirkningType[]
            {
                ToArrivalDepartureVirkning(),
                ToRelocationVirkning(),
                ToVirkningType()
            };
        }
    }
}
