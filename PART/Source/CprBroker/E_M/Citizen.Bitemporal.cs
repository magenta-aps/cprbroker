using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.E_M
{
    public partial class Citizen
    {
        public virtual TidspunktType ToTidspunktType()
        {
            // TODO: Find more fields for registration date
            return TidspunktType.Create(
                Converters.GetMaxDate(
                    Converters.ToDateTime(this.BirthRegistrationDate, this.BirthRegistrationDateUncertainty),
                    this.CprChurchTimestamp,
                    this.CprPersonTimestamp
                )
            );
        }

    }
}