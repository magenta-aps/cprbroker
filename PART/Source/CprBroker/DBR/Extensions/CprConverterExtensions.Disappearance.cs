using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;

namespace CprBroker.DBR.Extensions
{
    public static partial class CprConverterExtensions
    {
        public static Disappearance ToDpr(this CurrentDisappearanceInformationType disappearance)
        {
            Disappearance d = new Disappearance();
            d.PNR = decimal.Parse(disappearance.PNR);
            d.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(disappearance.Registration.RegistrationDate, 12);

            if (disappearance.DisappearanceDate.HasValue)
                d.DisappearanceDate = CprBroker.Utilities.Dates.DateToDecimal(disappearance.DisappearanceDate.Value, 12);

            d.RetrievalDate = null; // It is the current disappearance
            d.CorrectionMarker = null; // It is the current disappearance
            return d;
        }

        public static Disappearance ToDpr(this HistoricalDisappearanceType disappearance)
        {
            Disappearance d = new Disappearance();
            d.PNR = decimal.Parse(disappearance.PNR);
            d.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(disappearance.Registration.RegistrationDate, 12);

            if (disappearance.DisappearanceDate.HasValue)
                d.DisappearanceDate = CprBroker.Utilities.Dates.DateToDecimal(disappearance.DisappearanceDate.Value, 12);

            if (disappearance.RetrievalDate.HasValue)
                d.RetrievalDate = CprBroker.Utilities.Dates.DateToDecimal(disappearance.RetrievalDate.Value, 12);

            d.CorrectionMarker = disappearance.CorrectionMarker.ToString();
            return d;
        }

    }
}
