using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public partial class TilstandVirkningType
    {
        public static TilstandVirkningType Create(DateTime? fromDate)
        {
            return new TilstandVirkningType()
            {
                //TODO: Fill actor
                AktoerRef = null,
                //TODO: Fill comment text
                CommentText = null,
                FraTidspunkt = TidspunktType.Create(fromDate)
            };
        }
    }
}
