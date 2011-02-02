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
                FraTidspunkt = TidspunktType.Create(fromDate),
                TilTidspunkt = TidspunktType.Create(toDate)
            };
        }

        public static VirkningType Compose(params VirkningType[] partialEffects)
        {
            // TODO: What is the default value for DateTime? in case input array is empty?
            var fromDate =
                partialEffects
                .Select(pe => pe.FraTidspunkt.ToDateTime())
                .Select(d => d.HasValue ? d.Value : DateTime.MinValue)
                .OrderBy(d => d)
                .FirstOrDefault();

            var to =
                partialEffects
                .Select(pe => pe.FraTidspunkt.ToDateTime())
                .Select(d => d.HasValue ? d.Value : DateTime.MaxValue)
                .OrderByDescending(d => d)
                .FirstOrDefault();
            return VirkningType.Create(fromDate, to);

        }

        public static bool IsOpen(VirkningType v)
        {
            if (v == null)
                return true;
            if (!TidspunktType.ToDateTime(v.FraTidspunkt).HasValue && !TidspunktType.ToDateTime(v.TilTidspunkt).HasValue)
                return true;
            return false;
        }
    }
}
