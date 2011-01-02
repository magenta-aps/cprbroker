using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public partial class VirkningType
    {
        public static VirkningType Create(DateTime? fromDate, DateTime? toDate)
        {
            return new VirkningType()
            {
                //TODO: Fill actor text
                AktoerTekst = null,
                //TODO: Fill comment text
                CommentText = null,
                //TODO: Add correct parameters
                FraTidspunkt = TidspunktType.Create(fromDate),
                //TODO: Add correct parameters
                TilTidspunkt = TidspunktType.Create(toDate)
            };
        }
    }
}
