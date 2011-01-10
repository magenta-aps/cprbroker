using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class Effect
    {
        public VirkningType ToXmlType()
        {
            return new VirkningType() 
            {
                AktoerTekst=this.ActorText,
                CommentText=this.CommentText,
                FraTidspunkt=TidspunktType.Create(this.FromDate),
                TilTidspunkt=TidspunktType.Create(this.ToDate)
            };
        }

    }
}
