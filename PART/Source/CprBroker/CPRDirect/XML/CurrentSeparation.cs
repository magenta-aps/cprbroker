using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class CurrentSeparationType
    {
        // TODO: Addd unit tests
        public CivilStatusType ToCivilStatusType()
        {
            return new CivilStatusType()
            {
                CivilStatusKode = CivilStatusKodeType.Separeret,
                TilstandVirkning = TilstandVirkningType.Create(ToSeparationStartDate()),
            };
        }

        public DateTime? ToSeparationStartDate()
        {
            return Converters.ToDateTime(this.SeparationStartDate, this.SeparationStartDateUncertainty);
        }
    }
}
