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
        public static PersonName ToDpr(this CurrentNameInformationType currentName, PersonInformationType personInformation)
        {
            PersonName pn = new PersonName();
            pn.PNR = Decimal.Parse(currentName.PNR);
            pn.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentName.Registration.RegistrationDate, 12);
            pn.NameAuthorityCode = null; //TODO: Can be fetched in CPR Services, mynkod
            pn.Status = personInformation.Status;
            if (personInformation.StatusStartDate != null)
            {
                pn.StatusDate = CprBroker.Utilities.Dates.DateToDecimal(personInformation.StatusStartDate.Value, 12);
            }
            else
            {
                Console.WriteLine("personInformation.StatusStartDate was NULL");
                pn.StatusDate = null;//CprBroker.Utilities.Dates.DateToDecimal(resp.PersonInformation.StatusStartDate.Value, 12);
            }
            pn.FirstNameMarker = currentName.FirstNameMarker;
            pn.SurnameMarker = currentName.LastNameMarker;
            if (currentName.NameStartDate.HasValue)
                pn.NameStartDate = CprBroker.Utilities.Dates.DateToDecimal(currentName.NameStartDate.Value, 12);
            pn.NameTerminationDate = null; //This is the current name
            pn.AddressingNameDate = null; //TODO: Can be fetched in CPR Services, adrnvnhaenstart
            pn.CorrectionMarker = null; // This is the current name
            pn.AddressingNameReportingMarker = null; //TODO: Can be fetched in CPR Services, indrap
            pn.AuthorityTextUpdateDate = null; //TODO: Can be fetched in CPR Services, myntxttimestamp
            pn.SearchNameDate = 0; //Said to be always 0
            pn.FirstName = string.Format("{0} {1}", currentName.FirstName_s, currentName.MiddleName).Trim();
            pn.LastName = currentName.LastName;
            // Special logic for addressing name
            pn.AddressingName = ToDprAddressingName(currentName.AddressingName, currentName.LastName);
            pn.SearchName = ""; //Said to be always blank
            pn.NameAuthorityText = null; //TODO: Can be fetched in CPR Services, myntxt
            return pn;
        }

        public static PersonName ToDpr(this HistoricalNameType historicalName/*, PersonInformationType personInformation*/)
        {
            PersonName pn = new PersonName();
            pn.PNR = Decimal.Parse(historicalName.PNR);
            pn.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalName.Registration.RegistrationDate, 12);
            pn.NameAuthorityCode = null; //TODO: Can be fetched in CPR Services, mynkod
            //TODO: Tjek om status angives på historiske data
            pn.Status = null;//personInformation.Status;
            //pn.StatusDate = CprBroker.Utilities.Dates.DateToDecimal(personInformation.StatusStartDate.Value, 13);
            pn.FirstNameMarker = historicalName.FirstNameMarker;
            pn.SurnameMarker = historicalName.LastNameMarker;

            if (historicalName.NameStartDate.HasValue)
                pn.NameStartDate = CprBroker.Utilities.Dates.DateToDecimal(historicalName.NameStartDate.Value, 12);

            pn.NameTerminationDate = CprBroker.Utilities.Dates.DateToDecimal(historicalName.NameEndDate.Value, 12);
            pn.AddressingNameDate = null; //TODO: Can be fetched in CPR Services, adrnvnhaenstart
            pn.CorrectionMarker = historicalName.CorrectionMarker;
            pn.AddressingNameReportingMarker = null; //TODO: Can be fetched in CPR Services, indrap
            pn.AuthorityTextUpdateDate = null; //TODO: Can be fetched in CPR Services, myntxttimestamp
            pn.SearchNameDate = 0; //Said to be always 0
            pn.FirstName = string.Format("{0} {1}", historicalName.FirstName_s, historicalName.MiddleName).Trim();
            pn.LastName = historicalName.LastName;
            pn.AddressingName = null; // Seems not available in historical records....
            pn.SearchName = ""; //Said to be always blank
            pn.NameAuthorityText = null; //TODO: Can be fetched in CPR Services, myntxt
            return pn;
        }

        public static string ToDprAddressingName(string addressingName, string lastName)
        {
            if (!string.IsNullOrEmpty(addressingName))
            {
                var lastNamePartCount = lastName.Split(' ').Length;
                var addressingNameParts = addressingName.Split(' ');
                var otherNamesPartCount = addressingNameParts.Length - lastNamePartCount;
                return string.Format("{0},{1}",
                    string.Join(" ", addressingNameParts.Skip(otherNamesPartCount).ToArray()),
                    string.Join(" ", addressingNameParts.Take(otherNamesPartCount).ToArray())
                );
            }
            return null;
        }

    }
}
