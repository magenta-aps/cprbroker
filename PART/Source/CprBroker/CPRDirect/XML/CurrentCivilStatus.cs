using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class CurrentCivilStatusType
    {
        // TODO: Addd unit tests
        public CivilStatusType ToCivilStatusType(CurrentSeparationType currentSeparation)
        {
            if (currentSeparation != null)
            {
                return currentSeparation.ToCivilStatusType();
            }
            else
            {
                return new CivilStatusType()
                {
                    CivilStatusKode = new CivilStatusLookupMap().Map(this.CivilStatus),
                    TilstandVirkning = TilstandVirkningType.Create(Converters.ToDateTime(this.CivilStatusStartDate, this.CivilStatusStartDateUncertainty)),
                };
            }
        }
    }
}
