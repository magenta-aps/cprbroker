using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.DBR
{
    public static class CprConverterExtensions
    {
        public static PersonTotal ToPersonTotal(this IndividualResponseType resp)
        {
            PersonTotal pt = new PersonTotal();
            /*
             * PERSON DETAILS
             */
            pt.PNR = Decimal.Parse(resp.PersonInformation.PNR);
            pt.StatusDate = CprBroker.Utilities.Dates.DateToDecimal(resp.PersonInformation.StatusStartDate.Value, 12);
            pt.Status = resp.PersonInformation.Status;
            pt.DateOfBirth = CprBroker.Utilities.Dates.DateToDecimal(resp.PersonInformation.Birthdate.Value, 8);
            pt.Sex = resp.PersonInformation.Gender;

            /*
             * RESIDENTIAL DETAILS
             */
            pt.MunicipalityCode = resp.CurrentAddressInformation.MunicipalityCode;
            pt.CurrentMunicipalityName = CprBroker.Providers.CPRDirect.Authority.GetNameByCode(pt.MunicipalityCode.ToString());
            pt.StreetCode = resp.CurrentAddressInformation.StreetCode;
            pt.HouseNumber = resp.CurrentAddressInformation.HouseNumber;
            pt.Floor = resp.CurrentAddressInformation.Floor;
            pt.Door = resp.CurrentAddressInformation.Door;
            pt.ConstructionNumber = resp.CurrentAddressInformation.BuildingNumber;
            pt.AddressProtectionMarker = null; //TODO: Find origin for AddressProtectionMarker - DPR SPECIFIC
            pt.DirectoryProtectionMarker = null; //TODO: Find origin for DirectoryProtectionMarker - DPR SPECIFIC
            pt.ArrivalDateMarker = null; //TODO: Find origin for ArrivalDateMarker - DPR SPECIFIC
            pt.AddressDate = CprBroker.Utilities.Dates.DateToDecimal(resp.CurrentAddressInformation.RelocationDate.Value, 12);
            pt.MunicipalityArrivalDate = CprBroker.Utilities.Dates.DateToDecimal(resp.CurrentAddressInformation.MunicipalityArrivalDate.Value, 12);
            pt.MunicipalityLeavingDate = CprBroker.Utilities.Dates.DateToDecimal(resp.CurrentAddressInformation.LeavingMunicipalityDepartureDate.Value, 12);
            pt.ChristianMark = resp.ChurchInformation.ChurchRelationship;
            pt.BirthPlaceOfRegistration = resp.BirthRegistrationInformation.AdditionalBirthRegistrationText; //TODO: validate whether this is correct...
            pt.PnrMarkingDate = CprBroker.Utilities.Dates.DateToDecimal(resp.PersonInformation.PersonStartDate.Value, 12);
            pt.MotherPersonalOrBirthDate = resp.ParentsInformation.MotherPNR.Substring(0, 6) + "-" + resp.ParentsInformation.MotherPNR.Substring(6, 4);
            pt.MotherMarker = null; //TODO: Find origin for MotherMarker - DPR SPECIFIC
            pt.FatherPersonalOrBirthdate = resp.ParentsInformation.FatherPNR.Substring(0, 6) + "-" + resp.ParentsInformation.FatherPNR.Substring(6, 4);
            pt.FatherMarker = null; //TODO: Find origin for FatherMarker - DPR SPECIFIC
            pt.ExitEntryMarker = null; //TODO: Find origin for ExitEntryMarker - DPR SPECIFIC
            pt.DisappearedMarker = null; //TODO: Find origin for DisappearedMarker - DPR SPECIFIC
            pt.UnderGuardianshipDate = CprBroker.Utilities.Dates.DateToDecimal(resp.Disempowerment.DisempowermentStartDate.Value, 8);
            pt.PaternityDate = CprBroker.Utilities.Dates.DateToDecimal(resp.ParentsInformation.FatherDate.Value, 8);
            pt.MaritalStatus = resp.CurrentCivilStatus.CivilStatusCode;
            pt.MaritalStatusDate = CprBroker.Utilities.Dates.DateToDecimal(resp.CurrentCivilStatus.CivilStatusStartDate.Value, 12);
            pt.SpousePersonalOrBirthdate = resp.CurrentCivilStatus.SpousePNR.Substring(0, 6) + "-" + resp.CurrentCivilStatus.SpousePNR.Substring(6, 4);
            pt.SpouseMarker = null; //TODO: Find origin for SpouseMarker - DPR SPECIFIC
            pt.PostCode = resp.ClearWrittenAddress.PostCode;
            pt.PostDistrictName = resp.ClearWrittenAddress.PostDistrictText;
            //TODO: Retrieve data from ElectionInformation, how?
            //pt.VotingDate = CprBroker.Utilities.Dates.DateToDecimal(resp.ElectionInformation, 8);
            pt.ChildMarker = null; //TODO: Find origin for ChildMarker - DPR SPECIFIC
            pt.SupplementaryAddressMarker = null; //TODO: Find origin for SupplementaryAddressMarker - DPR SPECIFIC
            pt.MunicipalRelationMarker = null; //TODO: Find origin for MunicipalRelationMarker - DPR SPECIFIC
            pt.NationalMemoMarker = null; //TODO: Find origin for NationalMemoMarker - DPR SPECIFIC
            pt.FormerPersonalMarker = null; //TODO: Find origin for FormerPersonalMarker - DPR SPECIFIC
            pt.PaternityAuthorityName = null; //TODO: Retrieve this from the CPR Service field far_mynkod
            pt.MaritalAuthorityName = null; //TODO: Retrieve this from the CPR Service field mynkod
            pt.BirthPlaceOfRegistration = resp.BirthRegistrationInformation.AdditionalBirthRegistrationText; //TODO: validate that this is correct
            pt.Occupation = resp.PersonInformation.Job;
            pt.CareOfName = resp.CurrentAddressInformation.CareOfName;
            pt.CityName = resp.ClearWrittenAddress.CityName;
            pt.NationalityRight = null; //TODO: Implement method in CprBroker.Providers.CPRDirect.Authority to retrieve the country by its country code (NumericCountryCode).
            pt.PreviousAddress = null; //TODO: Must contain the previous standard address and its municipality name...
            pt.PreviousMunicipalityName = null; //TODO: Must contain the previous municipality name...
            pt.SearchName = resp.CurrentNameInformation.FirstName_s.ToUpper();
            pt.SearchSurname = resp.CurrentNameInformation.LastName.ToUpper();
            pt.AddressingName = resp.ClearWrittenAddress.AddressingName;
            pt.StandardAddress = resp.ClearWrittenAddress.LabelledAddress;
            pt.Location = resp.ClearWrittenAddress.Location;
            pt.ContactAddressMarker = null; //TODO: Find origin for ContactAddressMarker - DPR SPECIFIC
            //return pt;
            throw new NotImplementedException();
        }

        public static Person ToPerson(this IndividualResponseType person)
        {
            Person p = new Person();
            p.PNR = decimal.Parse(person.PersonInformation.PNR);
            p.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            p.Birthdate = CprBroker.Utilities.Dates.DateToDecimal(person.PersonInformation.Birthdate.Value, 8);
            p.Gender = person.PersonInformation.Gender.ToString();
            p.CustomerNumber = null; //TODO: Find origin for CustomerNumber - DPR SPECIFIC
            /*
             * Birth date related
             */
            p.BirthRegistrationAuthorityCode = decimal.Parse(person.BirthRegistrationInformation.BirthRegistrationAuthorityCode);
            p.BirthRegistrationDate = CprBroker.Utilities.Dates.DateToDecimal(person.BirthRegistrationInformation.Registration.RegistrationDate, 12);
            p.BirthRegistrationPlaceUpdateDate = 0; //TODO: Can be retrieved from CPR Services: foedmynhaenstart
            p.BirthplaceTextUpdateDate = null; //TODO: Can be retrieved from CPR Services: foedtxttimestamp
            p.BirthplaceText = person.BirthRegistrationInformation.AdditionalBirthRegistrationText; //TODO: validate that this is correct
            /*
             * Religious related
             */
            p.ChristianMark = person.ChurchInformation.ChurchRelationship.ToString();
            p.ChurchRelationUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(person.ChurchInformation.Registration.RegistrationDate, 12);
            p.ChurchAuthorityCode = 0; //TODO: Can be retrieved from CPR Services: fkirkmynkod
            p.ChurchDate = CprBroker.Utilities.Dates.DateToDecimal(person.ChurchInformation.StartDate.Value, 8);
            /*
             * Guardianship related
             */
            p.UnderGuardianshipAuthprityCode = 0; //TODO: Can be retrieved from CPR Services: mynkod-ctumyndig
            p.GuardianshipUpdateDate = null; //TODO: Can be fetched in CPR Services: timestamp
            p.UnderGuardianshipDate = CprBroker.Utilities.Dates.DateToDecimal(person.Disempowerment.DisempowermentStartDate.Value, 8);
            /*
             * PNR related
             */
            p.PnrMarkingDate = null; //TODO: Can be fetched in CPR Services: pnrhaenstart
            p.PnrDate = 0; //TODO: Can be fetched in CPR Services: pnrmrkhaenstart 
            p.CurrentPnrUpdateDate = null; //TODO: Can be fetched in CPR Services: timestamp
            p.CurrentPnr = decimal.Parse(person.PersonInformation.CurrentCprNumber);
            p.PnrDeletionDate = CprBroker.Utilities.Dates.DateToDecimal(person.PersonInformation.PersonEndDate.Value, 8);
            /*
             * Position related
             */
            p.JobDate = null; //TODO: Can be fetched in CPR Services: stillingsdato
            p.Job = person.PersonInformation.Job;
            /*
             * Relations related
             */
            p.KinshipUpdateDate = 0; //TODO: Can be fetched in CPR Services: timestamp
            p.MotherPnr = decimal.Parse(person.ParentsInformation.MotherPNR);
            p.MotherBirthdate = CprBroker.Utilities.Dates.DateToDecimal(person.ParentsInformation.MotherBirthDate.Value, 8);
            p.MotherDocumentation = null; //TODO: Can be fetched in CPR Services: mor_dok
            p.FatherPnr = decimal.Parse(person.ParentsInformation.FatherPNR);
            p.FatherBirthdate = CprBroker.Utilities.Dates.DateToDecimal(person.ParentsInformation.FatherBirthDate.Value, 8);
            p.FatherDocumentation = null; //TODO: Can be fetched in CPR Services: far_dok
            p.PaternityDate = null; //TODO: Can be fetched in CPR Services: farhaenstart
            p.PaternityAuthorityCode = null; //TODO: Can be fetched in CPR Services: far_mynkod
            p.MotherName = person.ParentsInformation.MotherName;
            p.FatherName = person.ParentsInformation.FatherName;
            p.UnderGuardianshipDeleteDate = person.Disempowerment.DisempowermentEndDate.Value;
            p.UnderGuardianshipRelationType = person.Disempowerment.GuardianRelationType;
            //return p;
            throw new NotImplementedException();
        }

        public static Child ToDpr(this ChildType child)
        {
            Child ch = new Child();
            ch.ParentPNR = Decimal.Parse(child.PNR);
            ch.ChildPNR = Decimal.Parse(child.ChildPNR);
            ch.MotherOrFatherDocumentation = null; //TODO: This is a concatenation of mother documentation and father documentation - can be retrieved from CPR Services
            //return ch;
            throw new NotImplementedException();
        }

        public static PersonName ToDpr(this CurrentNameInformationType currentName)
        {
            PersonName pn = new PersonName();
            pn.PNR = Decimal.Parse(currentName.PNR);
            pn.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            pn.NameAuthorityCode = null; //TODO: Can be fetched in CPR Services, mynkod
            pn.Status = null; //TODO: Find in PersonInformation
            pn.StatusDate = null; //TODO: Find in PersonInformation
            pn.FirstNameMarker = currentName.FirstNameMarker;
            pn.SurnameMarker = currentName.LastNameMarker;
            pn.NameStartDate = CprBroker.Utilities.Dates.DateToDecimal(currentName.NameStartDate.Value ,12);
            pn.NameTerminationDate = null; //This is the current name
            pn.AddressingNameDate = null; //TODO: Can be fetched in CPR Services, adrnvnhaenstart
            pn.CorrectionMarker = null; // This is the current name
            pn.AddressingNameReportingMarker = null; //TODO: Can be fetched in CPR Services, indrap
            pn.AuthorityTextUpdateDate = null; //TODO: Can be fetched in CPR Services, myntxttimestamp
            pn.SearchNameDate = 0; //Said to be always 0
            pn.FirstName = currentName.FirstName_s;
            pn.LastName = currentName.LastName;
            pn.AddressingName = currentName.AddressingName;
            pn.SearchName = ""; //Said to be always blank
            pn.NameAuthorityText = null; //TODO: Can be fetched in CPR Services, myntxt
            //return pn;
            throw new NotImplementedException();
        }

        public static PersonName ToDpr(this HistoricalNameType historicalName)
        {
            PersonName pn = new PersonName();
            pn.PNR = Decimal.Parse(historicalName.PNR);
            pn.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            pn.NameAuthorityCode = null; //TODO: Can be fetched in CPR Services, mynkod
            pn.Status = null; //TODO: Find in PersonInformation
            pn.StatusDate = null; //TODO: Find in PersonInformation
            pn.FirstNameMarker = historicalName.FirstNameMarker;
            pn.SurnameMarker = historicalName.LastNameMarker;
            pn.NameStartDate = CprBroker.Utilities.Dates.DateToDecimal(historicalName.NameStartDate.Value, 12);
            pn.NameTerminationDate = CprBroker.Utilities.Dates.DateToDecimal(historicalName.NameEndDate.Value, 12);
            pn.AddressingNameDate = null; //TODO: Can be fetched in CPR Services, adrnvnhaenstart
            pn.CorrectionMarker = historicalName.CorrectionMarker;
            pn.AddressingNameReportingMarker = null; //TODO: Can be fetched in CPR Services, indrap
            pn.AuthorityTextUpdateDate = null; //TODO: Can be fetched in CPR Services, myntxttimestamp
            pn.SearchNameDate = 0; //Said to be always 0
            pn.FirstName = historicalName.FirstName_s;
            pn.LastName = historicalName.LastName;
            pn.AddressingName = null; // Seems not available in historical records....
            pn.SearchName = ""; //Said to be always blank
            pn.NameAuthorityText = null; //TODO: Can be fetched in CPR Services, myntxt
            //return pn;
            throw new NotImplementedException();
        }

        public static CivilStatus ToDpr(this CurrentCivilStatusType currentCivilStatus)
        {
            CivilStatus cs = new CivilStatus();
            cs.PNR = Decimal.Parse(currentCivilStatus.PNR);
            cs.UpdateDateOfCpr = 0; //TODO: Can be fetched in CPR Services, timestamp
            cs.MaritalStatus = currentCivilStatus.CivilStatusCode;
            cs.MaritalStatusAuthorityCode = null; //TODO: Can be fetched in CPR Services, mynkod
            cs.SpousePNR = Decimal.Parse(currentCivilStatus.SpousePNR);
            cs.SpouseBirthdate = CprBroker.Utilities.Dates.DateToDecimal(currentCivilStatus.SpouseBirthDate.Value, 8);
            cs.SpouseDocumentation = null; //TODO: Can be fetched in CPR Services, aegtedok
            cs.MaritalStatusDate = CprBroker.Utilities.Dates.DateToDecimal(currentCivilStatus.CivilStatusStartDate.Value, 12);
            cs.MaritalEndDate = null; //This is the current status
            cs.CorrectionMarker = null; //This is the current status
            cs.AuthorityTextUpdateDate = null; //TODO: Can be fetched in CPR Services,  myntxttimestamp
            cs.MaritalStatusAuthorityText = null; //TODO: Can be fetched in CPR Services,  myntxt
            cs.SpouseName = currentCivilStatus.SpouseName;
            cs.SeparationReferralTimestamp = currentCivilStatus.ReferenceToAnySeparation.Value.ToString();
            //return cs;
            throw new NotImplementedException();
        }

        public static CivilStatus ToDpr(this HistoricalCivilStatusType historicalCivilStatus)
        {
            CivilStatus cs = new CivilStatus();
            cs.PNR = Decimal.Parse(historicalCivilStatus.PNR);
            cs.UpdateDateOfCpr = 0; //TODO: Can be fetched in CPR Services, timestamp
            cs.MaritalStatus = historicalCivilStatus.CivilStatusCode;
            cs.MaritalStatusAuthorityCode = null; //TODO: Can be fetched in CPR Services, mynkod
            cs.SpousePNR = Decimal.Parse(historicalCivilStatus.SpousePNR);
            cs.SpouseBirthdate = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.SpouseBirthdate.Value, 8);
            cs.SpouseDocumentation = null; //This is the current status
            cs.MaritalStatusDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.CivilStatusStartDate.Value, 12);
            cs.MaritalEndDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.CivilStatusEndDate.Value, 12);
            cs.CorrectionMarker = historicalCivilStatus.CorrectionMarker;
            cs.AuthorityTextUpdateDate = null; //TODO: Can be fetched in CPR Services, myntxttimestamp
            cs.MaritalStatusAuthorityText = null; //TODO: Can be fetched in CPR Services, myntxt
            cs.SpouseName = historicalCivilStatus.SpouseName;
            cs.SeparationReferralTimestamp = historicalCivilStatus.ReferenceToAnySeparation.Value.ToString();
            //return cs;
            throw new NotImplementedException();
        }

        public static Separation ToDpr(this CurrentSeparationType currentSeparation)
        {
            Separation s = new Separation();
            s.PNR = Decimal.Parse(currentSeparation.PNR);
            s.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            s.SeparationReferalTimestamp = currentSeparation.ReferenceToAnyMaritalStatus.Value.ToString();
            s.CorrectionMarker = null; //This is the current status
            s.StartAuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod_start
            s.StartDate = currentSeparation.SeparationStartDate.Value;
            s.StartDateMarker = currentSeparation.SeparationStartDateUncertainty;
            s.EndAuthorityCode = null; //TODO: Can be fetched in CPR Services, mynkod_slut
            s.EndDate = null; //This is the current separation
            s.EndDateMarker = null; //This is the current separation
            //return s;
            throw new NotImplementedException();
        }

        public static Separation ToDpr(this HistoricalSeparationType historicalSeparation)
        {
            Separation s = new Separation();
            s.PNR = Decimal.Parse(historicalSeparation.PNR);
            s.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            s.SeparationReferalTimestamp = historicalSeparation.ReferenceToAnyMaritalStatus.Value.ToString();
            s.CorrectionMarker = historicalSeparation.CorrectionMarker;
            s.StartAuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod_start
            s.StartDate = historicalSeparation.SeparationStartDate.Value;
            s.StartDateMarker = historicalSeparation.SeparationStartDateUncertainty;
            s.EndAuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod_slut
            s.EndDate = historicalSeparation.SeparationEndDate.Value;
            s.EndDateMarker = historicalSeparation.SeparationEndDateUncertainty;
            //return s;
            throw new NotImplementedException();
        }

        public static Nationality ToDpr(this CurrentCitizenshipType currentCitizenship)
        {
            Nationality n = new Nationality();
            n.PNR = Decimal.Parse(currentCitizenship.PNR);
            n.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            n.CountryCode = currentCitizenship.CountryCode;
            n.NationalityStartDate = CprBroker.Utilities.Dates.DateToDecimal(currentCitizenship.CitizenshipStartDate.Value, 12);
            n.NationalityEndDate = null; // This is the current nationality
            n.CorrectionMarker = null; //This is the current status
            //return n;
            throw new NotImplementedException();
        }

        public static Nationality ToDpr(this HistoricalCitizenshipType historicalCitizenship)
        {
            Nationality n = new Nationality();
            n.PNR = Decimal.Parse(historicalCitizenship.PNR);
            n.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            n.CountryCode = historicalCitizenship.CountryCode;
            n.NationalityStartDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCitizenship.CitizenshipStartDate.Value, 12);
            n.NationalityEndDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCitizenship.CitizenshipEndDate.Value, 12);
            n.CorrectionMarker = historicalCitizenship.CorrectionMarker;
            //return n;
            throw new NotImplementedException();
        }

        public static Departure ToDpr(this CurrentDepartureDataType currentDeparture)
        {
            Departure d = new Departure();
            d.PNR = Decimal.Parse(currentDeparture.PNR);
            d.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            d.ExitCountryCode = currentDeparture.ExitCountryCode;
            d.ExitDate = CprBroker.Utilities.Dates.DateToDecimal(currentDeparture.ExitDate.Value, 12);
            d.ExitUpdateDate = null; //TODO: Can be fetched in CPR Services, udrtimestamp
            d.ForeignAddressDate = null; //TODO: Can be fetched in CPR Services, udlandadrdto
            d.VotingDate = null; //TODO: Can be fetched in ElectionData.VotingDate
            d.EntryCountryCode = null; //This is the current status
            d.EntryDate = null; //This is the current date
            d.EntryUpdateDate = null; //TODO: Can be fetched in CPR Services, indrtimestamp
            d.CorrectionMarker = null; //This is the current status
            d.ForeignAddressLine1 = currentDeparture.ForeignAddress1;
            d.ForeignAddressLine2 = currentDeparture.ForeignAddress2;
            d.ForeignAddressLine3 = currentDeparture.ForeignAddress3;
            d.ForeignAddressLine4 = currentDeparture.ForeignAddress4;
            d.ForeignAddressLine5 = currentDeparture.ForeignAddress5;
            //return d;
            throw new NotImplementedException();
        }

        public static Departure ToDpr(this HistoricalDepartureType historicalDeparture)
        {
            Departure d = new Departure();
            d.PNR = Decimal.Parse(historicalDeparture.PNR);
            d.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            d.ExitCountryCode = historicalDeparture.ExitCountryCode;
            d.ExitDate = CprBroker.Utilities.Dates.DateToDecimal(historicalDeparture.ExitDate.Value, 12);
            d.ExitUpdateDate = null; //TODO: Can be fetched in CPR Services, udrtimestamp
            d.ForeignAddressDate = null; //TODO: Can be fetched in CPR Services, udlandadrdto
            d.VotingDate = null; //TODO: Can be fetched in CPR Services, valgretdto
            d.EntryCountryCode = historicalDeparture.EntryCountryCode;
            d.EntryDate = CprBroker.Utilities.Dates.DateToDecimal(historicalDeparture.EntryDate.Value, 12);
            d.EntryUpdateDate = null; //TODO: Can be fetched in CPR Services, indrtimestamp
            d.CorrectionMarker = historicalDeparture.CorrectionMarker.ToString();
            d.ForeignAddressLine1 = historicalDeparture.ForeignAddress1;
            d.ForeignAddressLine2 = historicalDeparture.ForeignAddress2;
            d.ForeignAddressLine3 = historicalDeparture.ForeignAddress3;
            d.ForeignAddressLine4 = historicalDeparture.ForeignAddress4;
            d.ForeignAddressLine5 = historicalDeparture.ForeignAddress5;
            //return d;
            throw new NotImplementedException();
        }

        public static ContactAddress ToDpr(this ContactAddressType contactAddress)
        {
            ContactAddress ca = new ContactAddress();
            ca.PNR = Decimal.Parse(contactAddress.PNR);
            ca.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            ca.MunicipalityCode = 0; //TODO: Retrieve from CurrentAddressInformation or  ClearWrittenAddress
            ca.AddressDate = CprBroker.Utilities.Dates.DateToDecimal(contactAddress.StartDate.Value, 8);
            ca.ContactAddressLine1 = contactAddress.Line1;
            ca.ContactAddressLine2 = contactAddress.Line2;
            ca.ContactAddressLine3 = contactAddress.Line3;
            ca.ContactAddressLine4 = contactAddress.Line4;
            ca.ContactAddressLine5 = contactAddress.Line5;
            //return ca;
            throw new NotImplementedException();
        }

        public static PersonAddress ToDpr(this CurrentAddressWrapper currentAddress)
        {
            PersonAddress pa = new PersonAddress();
            pa.PNR = Decimal.Parse(currentAddress.CurrentAddressInformation.PNR);
            pa.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            pa.MunicipalityCode = currentAddress.CurrentAddressInformation.MunicipalityCode;
            pa.StreetCode = currentAddress.CurrentAddressInformation.StreetCode;
            pa.HouseNumber = currentAddress.CurrentAddressInformation.HouseNumber;
            pa.Floor = currentAddress.CurrentAddressInformation.Floor;
            pa.DoorNumber = currentAddress.CurrentAddressInformation.Door;
            pa.GreenlandConstructionNumber = null; //Can be found in CurrentAddressInformation and ClearWrittenAddress
            pa.PostCode = 0; //Can be found in ClearWrittenAddress
            pa.MunicipalityName = CprBroker.Providers.CPRDirect.Authority.GetNameByCode(pa.MunicipalityCode.ToString());
            pa.StreetAddressingName = null; //TODO: Can be fetched in ClearWrittenAddressStreetAddressingName
            pa.AddressStartDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.StartDate.Value, 8);
            pa.AddressStartDateMarker = null; //TODO: Find origin for AddressStartDateMarker - DPR SPECIFIC
            pa.AddressEndDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.EndDate.Value, 8);
            pa.LeavingFromMunicipalityCode = currentAddress.CurrentAddressInformation.LeavingMunicipalityCode;
            pa.LeavingFromMunicipalityDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.LeavingMunicipalityDepartureDate.Value, 12);
            pa.MunicipalityArrivalDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.MunicipalityArrivalDate.Value, 12);
            pa.AlwaysNull1 = null;
            pa.AlwaysNull2 = null;
            pa.AlwaysNull3 = null;
            pa.AlwaysNull4 = null;
            pa.AlwaysNull5 = null;
            pa.AdditionalAddressDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.StartDate.Value, 8);
            pa.CorrectionMarker = null; //This is the current status
            pa.CareOfName = currentAddress.CurrentAddressInformation.CareOfName;
            pa.Town = null; //Can be found in ClearWrittenAddress
            pa.Location = null; //Can be found in ClearWrittenAddress as LOKALITET
            pa.AdditionalAddressLine1 = currentAddress.CurrentAddressInformation.SupplementaryAddress1;
            pa.AdditionalAddressLine2 = currentAddress.CurrentAddressInformation.SupplementaryAddress2;
            pa.AdditionalAddressLine3 = currentAddress.CurrentAddressInformation.SupplementaryAddress3;
            pa.AdditionalAddressLine4 = currentAddress.CurrentAddressInformation.SupplementaryAddress4;
            pa.AdditionalAddressLine5 = currentAddress.CurrentAddressInformation.SupplementaryAddress5;
            //return pa;
            throw new NotImplementedException();
        }

        public static PersonAddress ToDpr(this HistoricalAddressType historicalAddress)
        {
            PersonAddress pa = new PersonAddress();
            pa.PNR = Decimal.Parse(historicalAddress.PNR);
            pa.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            pa.MunicipalityCode = historicalAddress.MunicipalityCode;
            pa.StreetCode = historicalAddress.StreetCode;
            pa.HouseNumber = historicalAddress.HouseNumber;
            pa.Floor = historicalAddress.Floor;
            pa.DoorNumber = historicalAddress.Door;
            pa.GreenlandConstructionNumber = null; //Can be found in CurrentAddressInformation and ClearWrittenAddress
            pa.PostCode = 0; //Can be found in ClearWrittenAddress
            pa.MunicipalityName = CprBroker.Providers.CPRDirect.Authority.GetNameByCode(pa.MunicipalityCode.ToString());
            pa.StreetAddressingName = null; //TODO: Can be fetched in CPR Services, vejadrnvn
            pa.AddressStartDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.RelocationDate.Value, 8);
            pa.AddressStartDateMarker = historicalAddress.RelocationDateUncertainty; // Validate whether this is correct
            pa.AddressEndDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.LeavingDate.Value, 8);
            pa.LeavingFromMunicipalityCode = null; // Seems not available in historical records....
            pa.LeavingFromMunicipalityDate = null; // Seems not available in historical records....
            pa.MunicipalityArrivalDate = null;  //Seems only available for current address
            pa.AlwaysNull1 = null;
            pa.AlwaysNull2 = null;
            pa.AlwaysNull3 = null;
            pa.AlwaysNull4 = null;
            pa.AlwaysNull5 = null;
            pa.AdditionalAddressDate = null; //TODO: Can be fetched in CPR Services, supladrhaenstart
            pa.CorrectionMarker = historicalAddress.CorrectionMarker;
            pa.CareOfName = historicalAddress.CareOfName;
            pa.Town = null; //Can be found in ClearWrittenAddress
            pa.Location = null; //Can be found in ClearWrittenAddress as LOKALITET
            pa.AdditionalAddressLine1 = null; // Seems not available in historical records....
            pa.AdditionalAddressLine2 = null; // Seems not available in historical records....
            pa.AdditionalAddressLine3 = null; // Seems not available in historical records....
            pa.AdditionalAddressLine4 = null; // Seems not available in historical records....
            pa.AdditionalAddressLine5 = null; // Seems not available in historical records....
            //return pa;
            throw new NotImplementedException();
        }

        public static Protection ToDpr(this ProtectionType protection)
        {
            Protection p = new Protection();
            p.PNR = Decimal.Parse(protection.PNR);
            p.ProtectionType = protection.ProtectionType_;
            p.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            p.StartDate = protection.StartDate.Value;
            p.EndDate = protection.EndDate;
            p.ReportingMarker = null; //TODO: Find origin for ReportingMarker - SHOULD BE IN CPR...
            throw new NotImplementedException();
        }

        public static Disappearance ToDpr(this CurrentDisappearanceInformationType disappearance)
        {
            Disappearance d = new Disappearance();
            d.PNR = decimal.Parse(disappearance.PNR);
            d.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            d.DisappearanceDate = CprBroker.Utilities.Dates.DateToDecimal(disappearance.DisappearanceDate.Value, 12);
            d.RetrievalDate = null; // It is the current disappearance
            d.CorrectionMarker = null; // It is the current disappearance
            //return d;
            throw new NotImplementedException();
        }

        public static Disappearance ToDpr(this HistoricalDisappearanceType disappearance)
        {
            Disappearance d = new Disappearance();
            d.PNR = decimal.Parse(disappearance.PNR);
            d.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            d.DisappearanceDate = CprBroker.Utilities.Dates.DateToDecimal(disappearance.DisappearanceDate.Value, 12);
            d.RetrievalDate = CprBroker.Utilities.Dates.DateToDecimal(disappearance.RetrievalDate.Value, 12);
            d.CorrectionMarker = disappearance.CorrectionMarker.ToString();
            //return d;
            throw new NotImplementedException();
        }

        public static Event ToDpr(this EventsType events)
        {
            Event e = new Event();
            e.PNR = decimal.Parse(events.PNR);
            e.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            e.Event_ = events.Event_;
            e.AFLMRK = events.DerivedMark;
            //return e;
            throw new NotImplementedException();
        }

        public static Note ToDpr(this NotesType notes)
        {
            Note n = new Note();
            n.PNR = decimal.Parse(notes.PNR);
            n.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            n.NationalRegisterMemoDate = CprBroker.Utilities.Dates.DateToDecimal(notes.StartDate.Value, 8);
            n.DeletionDate = CprBroker.Utilities.Dates.DateToDecimal(notes.EndDate.Value, 8);
            n.NoteNumber = notes.NoteNumber;
            n.NationalRegisterNoteLine = notes.NoteText;
            n.MunicipalityCode = 0; //TODO: Can be fetched in CPR Services, komkod
            //return n;
            throw new NotImplementedException();
        }

        public static MunicipalCondition ToDpr(this MunicipalConditionsType condition)
        {
            MunicipalCondition m = new MunicipalCondition();
            m.PNR = decimal.Parse(condition.PNR);
            m.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            m.ConditionType = condition.MunicipalConditionType;
            m.ConditionMarker = condition.MunicipalConditionCode;
            m.ConditionDate = CprBroker.Utilities.Dates.DateToDecimal(condition.MunicipalConditionStartDate.Value, 8);
            m.ConditionComments = condition.MunicipalConditionComment;
            //return m;
            throw new NotImplementedException();
        }

        public static ParentalAuthority ToDpr(this ParentalAuthorityType auth)
        {
            ParentalAuthority p = new ParentalAuthority();
            p.ChildPNR = decimal.Parse(auth.PNR);
            p.RelationType = auth.RelationshipType;
            p.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            p.ParentalAuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod_start
            p.StartDate = auth.CustodyStartDate.Value;
            p.StartDateUncertainty = auth.CustodyStartDateUncertainty;
            p.EndDate = auth.CustodyEndDate.Value;
            //return p;
            throw new NotImplementedException();
        }
        
        public static GuardianAndParentalAuthorityRelation ToDpr(this DisempowermentType disempowerment)
        {
            GuardianAndParentalAuthorityRelation gapa = new GuardianAndParentalAuthorityRelation();
            gapa.PNR = decimal.Parse(disempowerment.PNR);
            gapa.RelationPnr = decimal.Parse(disempowerment.RelationPNR);
            gapa.RelationType = disempowerment.GuardianRelationType;
            gapa.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            gapa.StartDate = disempowerment.DisempowermentStartDate.Value;
            gapa.EndDate = disempowerment.DisempowermentEndDate.Value;
            gapa.AuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod
            //return gapa;
            throw new NotImplementedException();
        }

        public static GuardianAddress ToDprAddress(this DisempowermentType disempowerment)
        {
            GuardianAddress ga = new GuardianAddress();
            ga.PNR = decimal.Parse(disempowerment.PNR);
            ga.Address = disempowerment.GuardianName;
            ga.RelationType = disempowerment.GuardianRelationType;
            ga.CprUpdateDate = 0; //TODO: Can be fetched in CPR Services, timestamp
            ga.AddressLine1 = disempowerment.RelationText1;
            ga.AddressLine2 = disempowerment.RelationText2;
            ga.AddressLine3 = disempowerment.RelationText3;
            ga.AddressLine4 = disempowerment.RelationText4;
            ga.AddressLine5 = disempowerment.RelationText5;
            ga.StartDate = disempowerment.GuardianAddressStartDate.Value;
            ga.EndDate = disempowerment.DisempowermentEndDate;
            ga.AuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod
            //return ga;
            throw new NotImplementedException();
        }
    }
}
