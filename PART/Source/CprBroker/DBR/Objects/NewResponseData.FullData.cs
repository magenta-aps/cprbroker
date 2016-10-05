using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;
using System;
using System.Linq;
using CprBroker.Utilities;

namespace CprBroker.DBR
{
    public partial class NewResponseFullDataType : INewResponseData
    {
        public NewResponseFullDataType()
        { }

        public NewResponseFullDataType(IndividualResponseType resp, PersonInfoExtended personInfo)
        {
            PersonName personName = personInfo.PersonName;
            PersonTotal personTotal = personInfo.PersonTotal;
            Person person = personInfo.Person;
            CivilStatus civilStatus = null;
            PersonAddress personAddress = personInfo.Address;
            Disappearance disappearance = personInfo.Disappearance;
            ParentalAuthority parentalAuthority = null;
            GuardianAndParentalAuthorityRelation guardianAndParentalAuthorityRelation = null;
            GuardianAddress guardianAddress = null;
            Separation separation = null;
            Departure depature = null;

            Func<decimal?, decimal> decimalOf = (d) => d.HasValue ? d.Value : 0m;
            Func<char?, char> uncertaintyCharOf = (c) => c.HasValue ? c.Value : ' ';
            Func<char?, char> charOf = c => c.HasValue ? c.Value : ' ';

            // TODO: Implement this
            var ret = new NewResponseFullDataType()
            {
                // Init Contents with an empty string
                Contents = new string(' ', this.Length),

                // Data fields
                PNR = personName.PNR.ToPnrDecimalString(),
                AJFDTO_NAVNE = null,
                AJFDTO_NAVNEDecimal = personName.CprUpdateDate,
                MYNKOD_NAVNE = decimalOf(personName.NameAuthorityCode),
                STATUS = personTotal.Status,
                STATUSHAENSTARTDecimal = decimalOf(personTotal.StatusDate),
                FORNVNMARK = uncertaintyCharOf(personName.FirstNameMarker),
                EFTERNVNMARK = uncertaintyCharOf(personName.SurnameMarker),
                NVNHAENSTART = resp.CurrentNameInformation.NameStartDate,
                ADRNVN = personName.AddressingName,
                ADRNVNHAENSTART = null,
                ADRNVNHAENSTARTDecimal = decimalOf(personName.AddressingNameDate),
                INDRAP = personName.AddressingNameReportingMarker, // Also consider Protection.INDRAP (ReportingMarker)
                MYNTXTAJFDTODecimal = decimalOf(personName.AuthorityTextUpdateDate),
                FORNVN = personName.FirstName,
                EFTERNVN = personName.LastName,
                SFORNVN = personName.FirstName.ToUpper(),
                SEFTERNVN = personName.LastName.ToUpper(),
                MYNTXT_NAVNE = personName.NameAuthorityText,
                AJFDTO_PERSON = null,
                AJFDTO_PERSONDecimal = person.CprUpdateDate,
                FOEDDTO = personTotal.DateOfBirth,
                KOEN = charOf(personTotal.Sex),
                FOEDMYNKOD = person.BirthRegistrationAuthorityCode,
                FOEDMYNHAENSTART = null,
                FOEDMYNHAENSTARTDecimal = decimalOf(person.BirthRegistrationDate),
                FOEDMYNAJFDTO = null,
                FOEDMYNTXT = person.BirthplaceText,
                FOEDTXTAJFDTO = null,
                FOEDMYNAJFDTODecimal = person.BirthRegistrationPlaceUpdateDate,
                FKIRK = charOf(person.ChristianMark.FirstOrDefault()),
                FKIRKAJFDTO = null,
                FKIRKAJFDTODecimal = person.ChurchRelationUpdateDate,
                FKIRKMYNKOD = decimalOf(person.ChurchAuthorityCode),
                FKIRKHAENSTART = null,
                FKIRKHAENSTARTDecimal = decimalOf(person.ChurchDate),
                UMYNMYNKOD = decimalOf(person.UnderGuardianshipAuthorityCode),
                UMYNAJFDTODecimal = decimalOf(person.GuardianshipUpdateDate),
                UMYNMYNHAENSTARTDecimal = resp.Disempowerment.DisempowermentStartDateDecimal,
                PNRMRKHAENSTARTDecimal = decimalOf(personTotal.PnrMarkingDate),
                PNRHAENSTARTDecimal = person.PnrDate,
                AJFDTO_PNRGAELD = null,
                AJFDTO_PNRGAELDDecimal = decimalOf(person.CurrentPnrUpdateDate),
                PNRGAELD = decimalOf(person.CurrentPnr),
                PNRHAENSLUTDecimal = decimalOf(person.PnrDeletionDate),
                STILLINGDTODecimal = decimalOf(person.JobDate),
                FOEDTXTAJFDTODecimal = decimalOf(person.BirthplaceTextUpdateDate),
                KUNDENR = decimalOf(person.CustomerNumber),
                AJFDTO_MORFAR = null,
                AJFDTO_MORFARDecimal = decimalOf(person.KinshipUpdateDate),
                PNRMOR = person.MotherPnr,
                MOR = person.MotherPnr.ToPnrDecimalString(),
                MORFOEDDTO = decimalOf(person.MotherBirthdate),
                MORDOK = person.MotherDocumentation,
                MORMRK = charOf(personTotal.MotherMarker),
                PNRFAR = person.FatherPnr,
                FARFOEDDTO = decimalOf(person.FatherBirthdate),
                FARDOK = person.FatherDocumentation,
                FAR = person.FatherPnr.ToPnrDecimalString(),
                FARMRK = charOf(personTotal.FatherMarker),
                FARSKABMYNNVN = personTotal.PaternityAuthorityName,
                FARSKABHAENSTART = null,
                FARSKABHAENSTARTDecimal = decimalOf(person.PaternityDate),
                FARSKABMYNKOD = decimalOf(person.PaternityAuthorityCode),
                MORNVN = person.MotherName,
                FARNVN = person.FatherName,
                AEGTEFOEDDTO = decimalOf(resp.CurrentCivilStatus.SpouseBirthDateDecimal),
                AEGTEDOK = civilStatus.SpouseDocumentation,
                AEGTEPNR = decimalOf(civilStatus.SpousePNR),
                AEGTENVN = resp.CurrentCivilStatus.SpouseName,
                AEGTEMRK = resp.CurrentCivilStatus.SpouseNameMarker,
                AEGTE = resp.CurrentCivilStatus.SpousePNR,
                HAENSTART_CIVDecimal = resp.CurrentCivilStatus.CivilStatusStartDateDecimal,
                HAENSTART_CIV = resp.CurrentCivilStatus.CivilStatusStartDate,
                BNR = resp.CurrentAddressInformation.BuildingNumber,
                ANTAL_BOERN = personTotal.Children.Count(),
                BARNMRK = charOf(personTotal.ChildMarker),
                AKTKOMNVN = personTotal.CurrentMunicipalityName,
                BYNVN = personTotal.CityName,
                ETAGE = personTotal.Floor,
                CIVST = charOf(personTotal.MaritalStatus),
                CIVMYN = personTotal.MaritalAuthorityName,
                CONVN = personTotal.CareOfName,
                HUSNR = personTotal.HouseNumber,
                KOMKOD = personTotal.MunicipalityCode,
                KOMNVN = personTotal.CurrentMunicipalityName,
                HAENSTART_STAT = resp.CurrentCitizenship.CitizenshipStartDate,
                HAENSTART_STATDecimal = resp.CurrentCitizenship.CitizenshipStartDateDecimal,
                POSTNR = personTotal.PostCode,
                POSTDISTTXT = personTotal.PostDistrictName,
                VEJADRNVN = personAddress.StreetAddressingName,
                VEJKOD = personAddress.StreetCode,
                FORSVINDMRK = charOf(personTotal.DisappearedMarker),
                FORSVINDDTO = null,
                FORSVINDDTODecimal = disappearance.DisappearanceDate,
                FOEDREGSTED = personTotal.BirthPlaceOfRegistration,
                AJFDTO_CIV = null,
                AJFDTO_CIVDecimal = civilStatus.CprUpdateDate,
                AJFDTO_FORALD_35 = null,
                AJFDTO_FORALD_35Decimal = parentalAuthority.CprUpdateDate,
                AJFDTO_FORALD_46 = null,
                AJFDTO_FORALD_46Decimal = parentalAuthority.CprUpdateDate, // TODO: Investigate the difference between 35 and 46
                AJFDTO_FORSVIND = null,
                AJFDTO_FORSVINDDecimal = disappearance.CprUpdateDate,
                AJFDTO_KONTAKTADR = null,
                AJFDTO_KONTAKTADRDecimal = personTotal.ContactAddress.CprUpdateDate,
                AJFDTO_PERSONBOLIG = null,
                AJFDTO_PERSONBOLIGDecimal = personAddress.CprUpdateDate, // Is this one correct?
                AJFDTO_RELPNR_1 = null,
                AJFDTO_RELPNR_1Decimal = guardianAndParentalAuthorityRelation.CprUpdateDate,
                AJFDTO_RELPNR_5 = null,
                AJFDTO_RELPNR_5Decimal = guardianAndParentalAuthorityRelation.CprUpdateDate, // TODO: Investigate difference
                AJFDTO_RELPNR_6 = null,
                AJFDTO_RELPNR_6Decimal = guardianAndParentalAuthorityRelation.CprUpdateDate,
                AJFDTO_RELTXT = null,
                AJFDTO_RELTXTDecimal = guardianAddress.CprUpdateDate,
                AJFDTO_SEP = null,
                AJFDTO_SEPDecimal = separation.CprUpdateDate,
                AJFDTO_STATDecimal = 0m,
                AJFDTO_STAT = resp.CurrentCitizenship.Registration.RegistrationDate,
                AJFDTO_UDRINDR = null,
                AJFDTO_UDRINDRDecimal = depature.CprUpdateDate,
                
                





            };

            // Copy the contents
            this.Contents = ret.Contents;
        }
    }
}
