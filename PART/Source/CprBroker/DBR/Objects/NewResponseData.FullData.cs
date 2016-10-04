using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.DBR
{
    public partial class NewResponseFullDataType : INewResponseData
    {
        public NewResponseFullDataType()
        { }

        public NewResponseFullDataType(IndividualResponseType resp, PersonInfo personInfo)
        {
            PersonName personName = personInfo.PersonName;
            PersonTotal personTotal = personInfo.PersonTotal;
            Person person = personInfo.Person;

            Func<decimal?, decimal> decimalOf = (d) => d.HasValue ? d.Value : 0m;
            Func<char?, char> uncertaintyCharOf = (c) => c.HasValue ? c.Value : ' ';
            Func<char?, char> charOf = c => c.HasValue ? c.Value : ' ';

            // TODO: Implement this
            var ret = new NewResponseFullDataType()
            {
                // Init Contents with an empty string
                Contents = new string(' ', this.Length),

                // Data fields
                PNR = resp.PersonInformation.PNR,
                AJFDTO_NAVNEDecimal = personName.CprUpdateDate,
                MYNKOD_NAVNE = decimalOf(personName.NameAuthorityCode),
                STATUS = personTotal.Status,
                STATUSHAENSTARTDecimal = decimalOf(personTotal.StatusDate),
                FORNVNMARK = uncertaintyCharOf(personName.FirstNameMarker),
                EFTERNVNMARK = uncertaintyCharOf(personName.SurnameMarker),
                NVNHAENSTART = resp.CurrentNameInformation.NameStartDate,
                ADRNVNHAENSTART = null,
                INDRAP = personName.AddressingNameReportingMarker, // Also consider Protection.INDRAP (ReportingMarker)
                MYNTXTAJFDTODecimal = decimalOf(personName.AuthorityTextUpdateDate),
                FORNVN = personName.FirstName,
                EFTERNVN = personName.LastName,
                SFORNVN = personName.FirstName.ToUpper(),
                SEFTERNVN = personName.LastName.ToUpper(),
                ADRNVN = personName.AddressingName,
                MYNTXT_NAVNE = personName.NameAuthorityText,
                AJFDTO_PERSONDecimal = person.CprUpdateDate,
                FOEDDTO = personTotal.DateOfBirth,
                KOEN = charOf(personTotal.Sex),
                FOEDMYNKOD = person.BirthRegistrationAuthorityCode,
                FOEDMYNHAENSTARTDecimal = decimalOf(person.BirthRegistrationDate),
                FOEDMYNAJFDTODecimal = person.BirthRegistrationPlaceUpdateDate,
                FKIRK = charOf(person.ChristianMark.FirstOrDefault()),
                FKIRKAJFDTODecimal = person.ChurchRelationUpdateDate,
                FKIRKMYNKOD = decimalOf(person.ChurchAuthorityCode),
                FKIRKHAENSTARTDecimal = decimalOf(person.ChurchDate),
                UMYNMYNKOD = decimalOf(person.UnderGuardianshipAuthorityCode),
                UMYNAJFDTODecimal = decimalOf(person.GuardianshipUpdateDate),
                UMYNMYNHAENSTARTDecimal = default(decimal), // TODO: fill this field
                PNRMRKHAENSTARTDecimal = decimalOf(personTotal.PnrMarkingDate),
                PNRHAENSTARTDecimal = person.PnrDate,
                AJFDTO_PNRGAELDDecimal = decimalOf(person.CurrentPnrUpdateDate),
                PNRGAELD = decimalOf(person.CurrentPnr),
                PNRHAENSLUTDecimal = decimalOf(person.PnrDeletionDate),
                STILLINGDTODecimal = decimalOf(person.JobDate),
                FOEDTXTAJFDTODecimal = decimalOf(person.BirthplaceTextUpdateDate),
                KUNDENR = decimalOf(person.CustomerNumber),
                AJFDTO_MORFARDecimal = decimalOf(person.KinshipUpdateDate),
                PNRMOR = person.MotherPnr,
                MORFOEDDTO = decimalOf(person.MotherBirthdate),
                MORDOK = person.MotherDocumentation,
                PNRFAR = person.FatherPnr,
                FARFOEDDTO = decimalOf(person.FatherBirthdate),
                FARDOK = person.FatherDocumentation,
                FARSKABHAENSTARTDecimal = decimalOf(person.PaternityDate),
                FARSKABMYNKOD = decimalOf(person.PaternityAuthorityCode),
                MORNVN = person.MotherName,
                FARNVN = person.FatherName,
            };

            // Copy the contents
            this.Contents = ret.Contents;
        }
    }
}
