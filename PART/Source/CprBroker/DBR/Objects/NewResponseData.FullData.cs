using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;
using System;
using System.Linq;
using CprBroker.Utilities;
using System.Text.RegularExpressions;

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
            CivilStatus civilStatus = personInfo.CurrentCivilStatus;
            PersonAddress personAddress = personInfo.Address;
            Disappearance disappearance = personInfo.Disappearance;

            ParentalAuthority parentalAuthority35 = personInfo.ParentalAuthority.FirstOrDefault(s => s.RelationType == 3 || s.RelationType == 5);
            ParentalAuthority parentalAuthority46 = personInfo.ParentalAuthority.FirstOrDefault(s => s.RelationType == 4 || s.RelationType == 6);

            ParentalAuthority parentalAuthority5 = personInfo.ParentalAuthority.FirstOrDefault(s => s.RelationType == 5);
            ParentalAuthority parentalAuthority6 = personInfo.ParentalAuthority.FirstOrDefault(s => s.RelationType == 6);

            Relation relation5 = personInfo.CustodyHolderRelations.FirstOrDefault(r => r.RelationType == 5);
            Relation relation6 = personInfo.CustodyHolderRelations.FirstOrDefault(r => r.RelationType == 6);

            GuardianAndParentalAuthorityRelation guardianAndParentalAuthorityRelation = personInfo.GuardianAndParentalRelation;
            GuardianAddress guardianAddress = personInfo.GuardianNoPNR;
            Separation separation = personInfo.Separation;
            Departure depature = personInfo.Departure;
            ContactAddress contactAddress = personInfo.ContactAddress;
            var children = personInfo.Children;

            Func<decimal?, decimal> decimalOf = (d) => d.HasValue ? d.Value : 0m;
            Func<char?, char> uncertaintyCharOf = (c) => c.HasValue ? c.Value : ' ';
            Func<char?, char> charOf = c => c.HasValue ? c.Value : ' ';

            // Init ConteNts with an empty string
            Contents = new string(' ', this.Length);

            // Data fields
            PNR = personTotal.PNR.ToPnrDecimalString().TrimStart('0');
            AJFDTO_NAVNE = null;
            AJFDTO_NAVNEDecimal = decimalOf(personName?.CprUpdateDate);
            MYNKOD_NAVNE = decimalOf(personName?.NameAuthorityCode);
            STATUS = personTotal.Status.ToDecimalString();
            STATUSHAENSTART = null;
            STATUSHAENSTARTDecimal = decimalOf(personTotal.StatusDate);
            FORNVNMARK = uncertaintyCharOf(personName?.FirstNameMarker);
            EFTERNVNMARK = uncertaintyCharOf(personName?.SurnameMarker);
            NVNHAENSTART = null;
            NVNHAENSTARTDecimal = decimalOf(personName?.NameStartDate);
            NVNADRBESKMRK = charOf(personTotal.AddressProtectionMarker);
            ADRNVN = personName?.AddressingName;
            ADRNVNHAENSTART = null;
            ADRNVNHAENSTARTDecimal = decimalOf(personName?.AddressingNameDate);
            INDRAP = personName?.AddressingNameReportingMarker; // Also consider Protection.INDRAP (ReportingMarker)
            MYNTXTAJFDTODecimal = decimalOf(personName?.AuthorityTextUpdateDate);
            FORNVN = personName?.FirstName;
            EFTERNVN = personName?.LastName;
            SFORNVN = personName?.FirstName.ToUpper();
            SEFTERNVN = personName?.LastName.ToUpper();
            MYNTXT_NAVNE = personName?.NameAuthorityText;
            AJFDTO_PERSON = null;
            AJFDTO_PERSONDecimal = person.CprUpdateDate;
            FOEDDTO = personTotal.DateOfBirth;
            KOEN = charOf(personTotal.Sex);
            FOEDMYNKOD = person.BirthRegistrationAuthorityCode;
            FOEDMYNHAENSTART = null;
            FOEDMYNHAENSTARTDecimal = decimalOf(person.BirthRegistrationDate);
            FOEDMYNAJFDTO = null;
            FOEDMYNTXT = person.BirthplaceText;
            FOEDTXTAJFDTO = null;
            FOEDMYNAJFDTODecimal = person.BirthRegistrationPlaceUpdateDate;
            FKIRK = charOf(person.ChristianMark.FirstOrDefault());
            FKIRKAJFDTO = null;
            FKIRKAJFDTODecimal = person.ChurchRelationUpdateDate;
            FKIRKMYNKOD = decimalOf(person.ChurchAuthorityCode);
            FKIRKHAENSTART = null;
            FKIRKHAENSTARTDecimal = decimalOf(person.ChurchDate);
            UMYNMYNKOD = decimalOf(person.UnderGuardianshipAuthorityCode);
            UMYNAJFDTO = null;
            UMYNAJFDTODecimal = decimalOf(person.GuardianshipUpdateDate);
            UMYNMYNHAENSTART = null;
            UMYNMYNHAENSTARTDecimal = decimalOf(person.UnderGuardianshipDate);
            PNRMRKHAENSTART = null;
            PNRMRKHAENSTARTDecimal = decimalOf(personTotal.PnrMarkingDate);
            PNRHAENSTART = null;
            PNRHAENSTARTDecimal = person.PnrDate;
            AJFDTO_PNRGAELD = null;
            AJFDTO_PNRGAELDDecimal = decimalOf(person.CurrentPnrUpdateDate);
            PNRGAELD = decimalOf(person.CurrentPnr);
            PNRHAENSLUT = null;
            PNRHAENSLUTDecimal = decimalOf(person.PnrDeletionDate);
            STILLINGDTO = null;
            STILLINGDTODecimal = decimalOf(person.JobDate);
            FOEDTXTAJFDTODecimal = decimalOf(person.BirthplaceTextUpdateDate);
            KUNDENR = decimalOf(person.CustomerNumber);
            AJFDTO_MORFAR = null;
            AJFDTO_MORFARDecimal = decimalOf(person.KinshipUpdateDate);
            PNRMOR = person.MotherPnr;
            MOR = personTotal.MotherPersonalOrBirthDate.PadLeft(11, '0');
            MORFOEDDTO = decimalOf(person.MotherBirthdate);
            MORDOK = person.MotherDocumentation;
            MORMRK = charOf(personTotal.MotherMarker);
            PNRFAR = person.FatherPnr;
            FARFOEDDTO = decimalOf(person.FatherBirthdate);
            FARDOK = person.FatherDocumentation;
            FAR = personTotal.FatherPersonalOrBirthdate.PadLeft(11, '0');
            FARMRK = charOf(personTotal.FatherMarker);
            FARSKABMYNNVN = personTotal.PaternityAuthorityName;
            FARSKABHAENSTART = null;
            FARSKABHAENSTARTDecimal = decimalOf(person.PaternityDate);
            FARSKABMYNKOD = decimalOf(person.PaternityAuthorityCode);
            MORNVN = person.MotherName;
            FARNVN = person.FatherName;
            AEGTEFOEDDTO = decimalOf(civilStatus?.SpouseBirthdate);
            AEGTEDOK = civilStatus?.SpouseDocumentation;
            AEGTEPNR = decimalOf(civilStatus?.SpousePNR);
            AEGTENVN = civilStatus?.SpouseName;
            AEGTEMRK = charOf(personTotal.SpouseMarker);
            AEGTE = personTotal.SpousePersonalOrBirthdate;
            if (AEGTE.Count(c => c == '-') == 2) // If a birth date
                AEGTE = AEGTE.Replace('-', ' ');
            HAENSTART_CIV = null;
            HAENSTART_CIVDecimal = decimalOf(civilStatus?.MaritalStatusDate);
            BNR = personAddress?.GreenlandConstructionNumber;

            BARNMRK = charOf(personTotal.ChildMarker);
            AKTKOMNVN = personTotal.CurrentMunicipalityName;
            BYNVN = personTotal.CityName;
            ETAGE = personTotal.Floor;
            CIVST = charOf(personTotal.MaritalStatus);
            CIVMYN = personTotal.MaritalAuthorityName;
            CONVN = personTotal.CareOfName;
            HUSNR = personTotal.HouseNumber;
            KOMKOD = personTotal.MunicipalityCode;
            KOMNVN = personTotal.CurrentMunicipalityName;
            HAENSTART_STAT = null;
            HAENSTART_STATDecimal = decimalOf(personInfo.Nationality?.NationalityStartDate);
            POSTNR = personTotal.PostCode;
            POSTDISTTXT = personTotal.PostDistrictName != "Ukendt" ? personTotal.PostDistrictName : "";
            VEJADRNVN = personAddress?.StreetAddressingName;
            VEJKOD = decimalOf(personAddress?.StreetCode);
            SIDEDOER = personTotal.Door;
            FORSVINDMRK = charOf(personTotal.DisappearedMarker);
            FORSVINDDTO = null;
            FORSVINDDTODecimal = decimalOf(disappearance?.DisappearanceDate);
            FOEDREGSTED = personTotal.BirthPlaceOfRegistration;
            AJFDTO_CIV = null;
            AJFDTO_CIVDecimal = decimalOf(civilStatus?.CprUpdateDate);
            AJFDTO_FORALD_35Decimal = 0;
            AJFDTO_FORALD_35 = Providers.DPR.Utilities.DateFromDecimal(parentalAuthority35?.CprUpdateDate);
            AJFDTO_FORALD_46Decimal = 0;
            AJFDTO_FORALD_46 = Providers.DPR.Utilities.DateFromDecimal(parentalAuthority46?.CprUpdateDate);
            AJFDTO_FORSVIND = null;
            AJFDTO_FORSVINDDecimal = decimalOf(disappearance?.CprUpdateDate);
            AJFDTO_KONTAKTADR = null;
            AJFDTO_KONTAKTADRDecimal = decimalOf(contactAddress?.CprUpdateDate);
            AJFDTO_PERSONBOLIG = null;
            AJFDTO_PERSONBOLIGDecimal = decimalOf(personAddress?.CprUpdateDate); // Is this one correct?
            AJFDTO_RELPNR_1 = null;
            AJFDTO_RELPNR_1Decimal = decimalOf(guardianAndParentalAuthorityRelation?.CprUpdateDate);
            AJFDTO_RELPNR_5Decimal = 0;
            AJFDTO_RELPNR_5 = Providers.DPR.Utilities.DateFromDecimal(parentalAuthority5?.CprUpdateDate);
            AJFDTO_RELPNR_6Decimal = 0;
            AJFDTO_RELPNR_6 = Providers.DPR.Utilities.DateFromDecimal(parentalAuthority6?.CprUpdateDate);
            AJFDTO_RELTXT = null;
            AJFDTO_RELTXTDecimal = decimalOf(guardianAddress?.CprUpdateDate);
            AJFDTO_SEP = null;
            AJFDTO_SEPDecimal = decimalOf(separation?.CprUpdateDate);
            AJFDTO_STAT = null;
            AJFDTO_STATDecimal = decimalOf(personInfo.Nationality?.CprUpdateDate);
            AJFDTO_UDRINDR = null;
            AJFDTO_UDRINDRDecimal = decimalOf(depature?.CprUpdateDate);
            KONTAKTADR1 = contactAddress?.ContactAddressLine1;
            KONTAKTADR2 = contactAddress?.ContactAddressLine2;
            KONTAKTADR3 = contactAddress?.ContactAddressLine3;
            KONTAKTADR4 = contactAddress?.ContactAddressLine4;
            KONTAKTADR5 = contactAddress?.ContactAddressLine5;
            FRAFLYKOMDTO = null;
            FRAFLYKOMDTODecimal = decimalOf(personTotal.MunicipalityLeavingDate);
            FRAFLYKOMKOD = decimalOf(personAddress?.LeavingFromMunicipalityCode);
            KONTAKTADRMRK = charOf(personTotal.ContactAddressMarker);
            KONTAKTADR_KOMKOD = decimalOf(contactAddress?.MunicipalityCode);
            KONTAKTADRSTART = null;
            KONTAKTADRSTARTDecimal = decimalOf(contactAddress?.AddressDate);
            LOKALITET = personTotal.Location;
            LOKBESKMRK = charOf(personTotal.DirectoryProtectionMarker);
            MYNKOD_CIV = decimalOf(civilStatus?.MaritalStatusAuthorityCode);
            MYNTXT_CIV = civilStatus?.MaritalStatusAuthorityText;
            MYNTXTAJFDTO = null;
            MYNTXTAJFDTO_CIV = null;
            MYNTXTAJFDTO_CIVDecimal = decimalOf(civilStatus?.AuthorityTextUpdateDate);
            // if guardian address is present we know the guardian has no pnr 
            RELPNR_1 = decimalOf(guardianAndParentalAuthorityRelation?.PNR);
            RELTYP_1 = decimalOf(guardianAndParentalAuthorityRelation?.RelationType);
            MYNKOD_5_1 = decimalOf(guardianAndParentalAuthorityRelation?.AuthorityCode);
            STARTDATE_1Decimal = 0m;
            STARTDATE_1 = guardianAndParentalAuthorityRelation?.StartDate;
            SLETDATE_1 = guardianAndParentalAuthorityRelation?.EndDate;
            RELTXT1 = guardianAddress?.AddressLine1;
            RELTXT2 = guardianAddress?.AddressLine2;
            RELTXT3 = guardianAddress?.AddressLine3;
            RELTXT4 = guardianAddress?.AddressLine4;
            RELTXT5 = guardianAddress?.AddressLine5;
            RELPNR_5 = decimalOf(relation5?.RelationPNR);
            RELTYP_5 = decimalOf(relation5?.RelationType);
            MYNKOD_5_5 = decimalOf(parentalAuthority5?.CustodyStartAuthorityCode);
            STARTDATE_5Decimal = 0m;
            STARTDATE_5 = parentalAuthority5?.StartDate;
            SLETDATE_5 = parentalAuthority5?.EndDate;
            RELPNR_6 = decimalOf(relation6?.RelationPNR);
            RELTYP_6 = decimalOf(parentalAuthority6?.RelationType);
            STARTDATE_6Decimal = 0m;
            STARTDATE_6 = parentalAuthority6?.StartDate;
            SLETDATE_6 = parentalAuthority6?.EndDate;
            MYNKOD_5_6 = decimalOf(parentalAuthority6?.CustodyStartAuthorityCode);
            MYNKOD_5_TXT = decimalOf(parentalAuthority6?.CustodyStartAuthorityCode);
            RELADRSAT = guardianAddress?.Address;
            // STARTDATE-TXT
            //SLETDATE_TXT = 
            // RELTYP_TXT = 
            // MYNKOD_STAT 
            //SLETDATE_1Decimal
            //SLETDATE_5Decimal
            //SLETDATE_6Decimal
            //STARTDATE_TXTDecimal
            //UMYNSLETDATEDecimal
            RELTYP_FORALD_35 = decimalOf(parentalAuthority35?.RelationType);
            STARTDATE_FORALD_UMRK_35 = charOf(parentalAuthority35?.StartDateMarker);
            STARTDATE_FORALD_35Decimal = 0m;
            STARTDATE_FORALD_35 = parentalAuthority35?.StartDate;
            STARTMYNKOD_FORALD_35 = decimalOf(parentalAuthority35?.CustodyStartAuthorityCode);
            SLETDATE_FORALD_35Decimal = 0m;
            SLETDATE_FORALD_35 = parentalAuthority35?.EndDate;
            RELTYP_FORALD_46 = decimalOf(parentalAuthority46?.RelationType);
            STARTDATE_FORALD_UMRK_46 = charOf(parentalAuthority46?.StartDateMarker);
            STARTDATE_FORALD_46Decimal = 0m;
            STARTDATE_FORALD_46 = parentalAuthority46?.StartDate;
            STARTMYNKOD_FORALD_46 = decimalOf(parentalAuthority46?.CustodyStartAuthorityCode);
            SLETDATE_FORALD_46Decimal = 0m;
            SLETDATE_FORALD_46 = parentalAuthority46?.EndDate;
            STARTDATE_SEP_UMRK = charOf(separation?.StartDateMarker);
            STARTDATE_SEPDecimal = 0m;
            STARTDATE_SEP = separation?.StartDate;
            SEP_HENVIS_TS = separation?.SeparationReferalTimestamp;
            STATSBORGER = personTotal.NationalityRight;
            STILLING = person.Job;
            STANDARDADR = personTotal.StandardAddress;
            START_MYNKOD_SEP = decimalOf(separation?.StartAuthorityCode);
            UDLANDADRDTO = null;
            UDLANDADRDTODecimal = decimalOf(depature?.ForeignAddressDate);
            UDLANDSADR1 = depature?.ForeignAddressLine1;
            UDLANDSADR2 = depature?.ForeignAddressLine2;
            UDLANDSADR3 = depature?.ForeignAddressLine3;
            UDLANDSADR4 = depature?.ForeignAddressLine4;
            UDLANDSADR5 = depature?.ForeignAddressLine5;
            UDRAJFDTO = null;
            UDRAJFDTODecimal = decimalOf(depature?.CprUpdateDate);
            UDRDTO = null;
            UDRDTODecimal = decimalOf(depature?.ExitDate);
            UDRMYNKOD = decimalOf(depature?.EntryCountryCode);
            UDRINDRMRK = charOf(personTotal.ExitEntryMarker);
            UMYNRELTYP = decimalOf(person.UnderGuardianshipRelationType);
            UMYNSLETDATE = person.UnderGuardianshipDeleteDate;
            SUPLADR1 = personAddress?.AdditionalAddressLine1;
            SUPLADR2 = personAddress?.AdditionalAddressLine2;
            SUPLADR3 = personAddress?.AdditionalAddressLine3;
            SUPLADR4 = personAddress?.AdditionalAddressLine4;
            SUPLADR5 = personAddress?.AdditionalAddressLine5;
            SUPLADRHAENSTART = null;
            SUPLADRHAENSTARTDecimal = decimalOf(personAddress?.AdditionalAddressDate);
            SUPLADRMRK = charOf(personTotal.SupplementaryAddressMarker);
            VALGRETDTO = null;
            VALGRETDTODecimal = decimalOf(personTotal.VotingDate);
            TIDLKOMNVN = personTotal.PreviousMunicipalityName;
            TIDLPNRMRK = charOf(personTotal.FormerPersonalMarker);
            TILFLYDTO = null;
            TILFLYDTODecimal = personTotal.AddressDate;
            TILFLYDTOMRK = charOf(personTotal.AddressDateMarker);
            TILFLYKOMDTO = null;
            TILFLYKOMDTODecimal = decimalOf(personTotal.MunicipalityArrivalDate);

            ANTAL_BOERN = children.Count();
            NewResponseFullChild = children
                .OrderBy(c=> PartInterface.Strings.PersonNumberToDate(c.ChildPNR.Value.ToPnrDecimalString()))
                .Select(c =>
                new NewResponseFullChildType()
                {
                    AJFDTO = null,
                    //AJFDTODecimal = person.CprUpdateDate,
                    PNR = c.ChildPNR.Value.ToPnrDecimalString(),
                    DOK = c.MotherOrFatherDocumentation
                })
                .ToList();
            PNR_BORN = string.Join(
                    "",
                    this.NewResponseFullChild.Select(c => c.ContentsWithSeparator(trimLeftZeros: true)).ToArray()
                    );
        }

        public override string ToString()
        {
            var ret = ContentsWithSeparator(trimLeftZeros: true);
            return ret.Substring(0, ret.Length - 1);// Trim last semi colon
        }

    }
}
