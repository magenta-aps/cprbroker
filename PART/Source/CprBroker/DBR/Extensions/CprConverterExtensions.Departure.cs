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
        public static Departure ToDpr(this CurrentDepartureDataType currentDeparture, ElectionInformationType electionInfo)
        {
            Departure d = new Departure();
            if (currentDeparture != null)
            {
                d.PNR = Decimal.Parse(currentDeparture.PNR);
                if (currentDeparture.ExitDate.HasValue)
                    d.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentDeparture.ExitDate.Value, 12);
                d.ExitCountryCode = currentDeparture.ExitCountryCode;

                if (currentDeparture.ExitDate.HasValue)
                    d.ExitDate = CprBroker.Utilities.Dates.DateToDecimal(currentDeparture.ExitDate.Value, 12);

                d.ExitUpdateDate = null; //TODO: Can be fetched in CPR Services, udrtimestamp
                d.ForeignAddressDate = null; //TODO: Can be fetched in CPR Services, udlandadrdto

                if (electionInfo != null && electionInfo.VotingDate.HasValue)
                    d.VotingDate = CprBroker.Utilities.Dates.DateToDecimal(electionInfo.VotingDate.Value, 12);

                d.EntryCountryCode = null; //This is the current status
                d.EntryDate = null; //This is the current date
                d.EntryUpdateDate = null; //TODO: Can be fetched in CPR Services, indrtimestamp
                d.CorrectionMarker = null; //This is the current status
                if (!string.IsNullOrEmpty(currentDeparture.ForeignAddress1))
                    d.ForeignAddressLine1 = currentDeparture.ForeignAddress1;
                if (!string.IsNullOrEmpty(currentDeparture.ForeignAddress2))
                    d.ForeignAddressLine2 = currentDeparture.ForeignAddress2;
                if (!string.IsNullOrEmpty(currentDeparture.ForeignAddress3))
                    d.ForeignAddressLine3 = currentDeparture.ForeignAddress3;
                if (!string.IsNullOrEmpty(currentDeparture.ForeignAddress4))
                    d.ForeignAddressLine4 = currentDeparture.ForeignAddress4;
                if (!string.IsNullOrEmpty(currentDeparture.ForeignAddress5))
                    d.ForeignAddressLine5 = currentDeparture.ForeignAddress5;
            }
            else
            {
                Console.WriteLine("currentDeparture was NULL");
            }
            return d;
        }

        public static Departure ToDpr(this HistoricalDepartureType historicalDeparture/*, ElectionInformationType electionInfo*/)
        {
            Departure d = new Departure();
            d.PNR = Decimal.Parse(historicalDeparture.PNR);
            if (historicalDeparture.EntryDate.HasValue)
                d.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalDeparture.EntryDate.Value, 12);
            if (historicalDeparture.ExitCountryCode > 0)
            {
                d.ExitCountryCode = historicalDeparture.ExitCountryCode;
            }
            else
            {
                Console.WriteLine("historicalDeparture.ExitCountryCode was NULL");
                d.ExitCountryCode = null;
            }
            d.ExitCountryCode = historicalDeparture.ExitCountryCode;
            if (historicalDeparture.ExitDate != null)
            {
                d.ExitDate = CprBroker.Utilities.Dates.DateToDecimal(historicalDeparture.ExitDate.Value, 12);
            }
            else
            {
                Console.WriteLine("historicalDeparture.ExitDate was NULL");
                d.ExitDate = null;
            }
            d.ExitUpdateDate = null; //TODO: Can be fetched in CPR Services, udrtimestamp
            d.ForeignAddressDate = null; //TODO: Can be fetched in CPR Services, udlandadrdto
            //TODO: Tjek om valg dato findes i historiske data
            d.VotingDate = null;//CprBroker.Utilities.Dates.DateToDecimal(electionInfo.VotingDate.Value, 12);
            d.EntryCountryCode = historicalDeparture.EntryCountryCode;

            if (historicalDeparture.EntryDate.HasValue)
                d.EntryDate = CprBroker.Utilities.Dates.DateToDecimal(historicalDeparture.EntryDate.Value, 12);

            d.EntryUpdateDate = null; //TODO: Can be fetched in CPR Services, indrtimestamp
            d.CorrectionMarker = historicalDeparture.CorrectionMarker.ToString();
            if (!string.IsNullOrEmpty(historicalDeparture.ForeignAddress1))
                d.ForeignAddressLine1 = historicalDeparture.ForeignAddress1;
            if (!string.IsNullOrEmpty(historicalDeparture.ForeignAddress2))
                d.ForeignAddressLine2 = historicalDeparture.ForeignAddress2;
            if (!string.IsNullOrEmpty(historicalDeparture.ForeignAddress3))
                d.ForeignAddressLine3 = historicalDeparture.ForeignAddress3;
            if (!string.IsNullOrEmpty(historicalDeparture.ForeignAddress4))
                d.ForeignAddressLine4 = historicalDeparture.ForeignAddress4;
            if (!string.IsNullOrEmpty(historicalDeparture.ForeignAddress5))
                d.ForeignAddressLine5 = historicalDeparture.ForeignAddress5;
            return d;
        }

    }
}
