using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;

namespace CPRDirectToDPR
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
            pt.MunicipalityName = null; //TODO: Find municipality name based on the municipality name - DPR SPECIFIC
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
            pt.PnrMarkingDate = null; //TODO: Find origin for PnrMarkingDate - SHOULD BE IN CPR...
            pt.MotherPersonalOrBirthDate = null; //TODO: Find origin for MotherPersonalOrBirthDate - DPR SPECIFIC
            pt.MotherMarker = null; //TODO: Find origin for MotherMarker - DPR SPECIFIC
            pt.FatherPersonalOrBirthdate = null; //TODO: Find origin for FatherPersonalOrBirthDate - DPR SPECIFIC
            pt.FatherMarker = null; //TODO: Find origin for FatherMarker - DPR SPECIFIC
            pt.ExitEntryMarker = null; //TODO: Find origin for ExitEntryMarker - DPR SPECIFIC
            pt.DisappearedMarker = null; //TODO: Find origin for DisappearedMarker - DPR SPECIFIC
            pt.UnderGuardianshipDate = CprBroker.Utilities.Dates.DateToDecimal(resp.Disempowerment.DisempowermentStartDate.Value, 8);
            pt.PaternityDate = null; //TODO: Find origin for PaternityDate - SHOULD BE IN CPR...
            pt.MaritalStatus = resp.CurrentCivilStatus.CivilStatusCode;
            pt.MaritalStatusDate = CprBroker.Utilities.Dates.DateToDecimal(resp.CurrentCivilStatus.CivilStatusStartDate.Value, 12);
            pt.SpousePersonalOrBirthdate = null; //TODO: Find origin for SpousePersonalOrBirthdate - DPR SPECIFIC
            pt.SpouseMarker = null; //TODO: Find origin for SpouseMarker - DPR SPECIFIC
            pt.PostCode = resp.ClearWrittenAddress.PostCode;
            pt.PostDistrictName = resp.ClearWrittenAddress.PostDistrictText;
            //pt.VotingDate = CprBroker.Utilities.Dates.DateToDecimal(resp.ElectionInformation.VotingDate.value, 8);
            pt.ChildMarker = null; //TODO: Find origin for ChildMarker - DPR SPECIFIC
            pt.SupplementaryAddressMarker = null; //TODO: Find origin for SupplementaryAddressMarker - DPR SPECIFIC
            pt.MunicipalRelationMarker = null; //TODO: Find origin for MunicipalRelationMarker - DPR SPECIFIC
            pt.NationalMemoMarker = null; //TODO: Find origin for NationalMemoMarker - DPR SPECIFIC
            pt.FormerPersonalMarker = null; //TODO: Find origin for FormerPersonalMarker - DPR SPECIFIC
            pt.PaternityAuthorityName = null; //TODO: Find origin for PaternityAuthorityName - DPR SPECIFIC
            pt.MaritalAuthorityName = null; //TODO: Find origin for MaritalAuthorityName - DPR SPECIFIC
            pt.BirthPlaceOfRegistration = resp.BirthRegistrationInformation.AdditionalBirthRegistrationText; //TODO: validate that this is correct
            pt.Occupation = resp.PersonInformation.Job;
            pt.CareOfName = resp.CurrentAddressInformation.CareOfName;
            pt.CityName = resp.ClearWrittenAddress.CityName;
            pt.NationalityRight = null; //TODO: Find origin for NationalityRight - DPR SPECIFIC
            pt.PreviousAddress = null; //TODO: Find origin for PreviousAddress - DPR SPECIFIC
            pt.PreviousMunicipalityName = null; //TODO: Find origin for PreviousMunicipalityName - DPR SPECIFIC
            pt.SearchName = null; //TODO: Find origin for SearchName - DPR SPECIFIC
            pt.SearchSurname = null; //TODO: Find origin for SearchSurname - DPR SPECIFIC
            pt.AddressingName = resp.ClearWrittenAddress.AddressingName;
            pt.StandardAddress = null; //TODO: Find origin for StandardAddress - DPR SPECIFIC
            pt.Location = resp.ClearWrittenAddress.Location;
            pt.ContactAddressMarker = null; //TODO: Find origin for ContactAddressMarker - DPR SPECIFIC
            //return pt;
            throw new NotImplementedException();
        }

        public static Person ToPerson(this IndividualResponseType person)
        {
            Person p = new Person();
            p.PNR = decimal.Parse(person.PersonInformation.PNR);
            //p.CprUpdateDate = null; //TODO: Find origin for CprUpdateDate - DPR SPECIFIC
            p.Birthdate = CprBroker.Utilities.Dates.DateToDecimal(person.PersonInformation.Birthdate.Value, 8);
            p.Gender = person.PersonInformation.Gender.ToString();
            p.CustomerNumber = null; //TODO: Find origin for CustomerNumber - DPR SPECIFIC
            /*
             * Birth date related
             */
            p.BirthRegistrationAuthorityCode = decimal.Parse(person.BirthRegistrationInformation.BirthRegistrationAuthorityCode);
            p.BirthRegistrationDate = CprBroker.Utilities.Dates.DateToDecimal(person.BirthRegistrationInformation.Registration.RegistrationDate, 12);
            p.BirthRegistrationPlaceUpdateDate = 0; //TODO: Find origin for BirthRegistrationPlaceUpdateDate - SHOULD BE IN CPR...
            p.BirthplaceTextUpdateDate = null; //TODO: Find origin for BirthplaceTextUpdateDate - SHOULD BE IN CPR...
            p.BirthplaceText = person.BirthRegistrationInformation.AdditionalBirthRegistrationText; //TODO: validate that this is correct
            /*
             * Religious related
             */
            p.ChristianMark = person.ChurchInformation.ChurchRelationship.ToString();
            p.ChurchRelationUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(person.ChurchInformation.Registration.RegistrationDate, 12);
            p.ChurchAuthorityCode = 0; //TODO: Find origin for ChurchAuthorityCode -SHOULD BE IN CPR...
            p.ChurchDate = CprBroker.Utilities.Dates.DateToDecimal(person.ChurchInformation.StartDate.Value, 8);
            /*
             * Guardianship related
             */
            p.UnderGuardianshipAuthprityCode = 0; //TODO: Find origin for UnderGuardianshipAuthprityCode - SHOULD BE IN CPR...
            p.GuardianshipUpdateDate = null; //TODO: Find origin for GuardianshipUpdateDate - DPR SPECIFIC
            p.UnderGuardianshipDate = CprBroker.Utilities.Dates.DateToDecimal(person.Disempowerment.DisempowermentStartDate.Value, 8);
            /*
             * PNR related
             */
            p.PnrMarkingDate = null; //TODO: Find origin for PnrMarkingDate - SHOULD BE IN CPR...
            p.PnrDate = 0; //TODO: Find origin for PnrDate - SHOULD BE IN CPR...
            p.CurrentPnrUpdateDate = null; //TODO: Find origin for CurrentPnrUpdateDate - DPR SPECIFIC
            p.CurrentPnr = decimal.Parse(person.PersonInformation.CurrentCprNumber);
            p.PnrDeletionDate = null; //TODO: Find origin for PnrDeletionDate - SHOULD BE IN CPR...
            /*
             * Position related
             */
            p.JobDate = null; //TODO: Find origin for JobDate - SHOULD BE IN CPR...
            p.Job = person.PersonInformation.Job;
            /*
             * Relations related
             */
            p.KinshipUpdateDate = 0; //TODO: Find origin for KinshipUpdateDate - DPR SPECIFIC
            p.MotherPnr = decimal.Parse(person.ParentsInformation.MotherPNR);
            p.MotherBirthdate = CprBroker.Utilities.Dates.DateToDecimal(person.ParentsInformation.MotherBirthDate.Value, 8);
            p.MotherDocumentation = null; //TODO: Find origin for MotherDocumentation - SHOULD BE IN CPR...
            p.FatherPnr = decimal.Parse(person.ParentsInformation.FatherPNR);
            p.FatherBirthdate = CprBroker.Utilities.Dates.DateToDecimal(person.ParentsInformation.FatherBirthDate.Value, 8);
            p.FatherDocumentation = null; //TODO: Find origin for FatherDocumentation - SHOULD BE IN CPR...
            p.PaternityDate = null; //TODO: Find origin for PaternityDate - SHOULD BE IN CPR...
            p.PaternityAuthorityCode = null; //TODO: Find origin for PaternityAuthorityCode - SHOULD BE IN CPR...
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
            ch.ChildPNR = null; //TODO: find origin for ChildPNR - DPR SPECIFIC
            ch.MotherOrFatherDocumentation = null; //TODO: find origin for MotherOrFatherDocumentation - DPR SPECIFIC
            //return ch;
            throw new NotImplementedException();
        }

        public static PersonName ToDpr(this CurrentNameInformationType currentName)
        {
            PersonName pn = new PersonName();
            pn.PNR = Decimal.Parse(currentName.PNR);
            pn.CprUpdateDate = 0; //TODO: Find origin for CprUpdateDate - DPR SPECIFIC
            pn.NameAuthorityCode = null; //TODO: Find origin for NameAuthorityCode - SHOULD BE IN CPR...
            pn.Status = null; //TODO: Find origin for Status - SHOULD BE IN CPR...
            pn.StatusDate = null; //TODO: Find origin for StatusDate - SHOULD BE IN CPR...
            pn.FirstNameMarker = currentName.FirstNameMarker;
            pn.SurnameMarker = currentName.LastNameMarker;
            pn.NameStartDate = CprBroker.Utilities.Dates.DateToDecimal(currentName.NameStartDate.Value ,12);
            pn.NameTerminationDate = null; //TODO: Find origin for NameTerminationDate - SHOULD BE IN CPR...
            pn.AddressingNameDate = null; //TODO: Find origin for AddressingNameDate - SHOULD BE IN CPR...
            pn.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker - SHOULD BE IN CPR...
            pn.AddressingNameReportingMarker = null; //TODO: Find origin for AddressingNameReportingMarker - SHOULD BE IN CPR...
            pn.AuthorityTextUpdateDate = null; //TODO: Find origin for AuthorityTextUpdateDate - DPR SPECIFIC
            pn.SearchNameDate = 0; //Said to be always 0
            pn.FirstName = currentName.FirstName_s;
            pn.LastName = currentName.LastName;
            pn.AddressingName = currentName.AddressingName;
            pn.SearchName = ""; //Said to be always blank
            pn.NameAuthorityText = null; //TODO: Find origin for NameAuthorityText - SHOULD BE IN CPR...
            //return pn;
            throw new NotImplementedException();
        }

        public static PersonName ToDpr(this HistoricalNameType historicalName)
        {
            PersonName pn = new PersonName();
            pn.PNR = Decimal.Parse(historicalName.PNR);
            pn.CprUpdateDate = 0; //TODO: Find origin for CprUpdateDate - DPR SPECIFIC
            pn.NameAuthorityCode = null; //TODO: Find origin for NameAuthorityCode - SHOULD BE IN CPR...
            pn.Status = null; //TODO: Find origin for Status - SHOULD BE IN CPR...
            pn.StatusDate = null; //TODO: Find origin for StatusDate - SHOULD BE IN CPR...
            pn.FirstNameMarker = historicalName.FirstNameMarker;
            pn.SurnameMarker = historicalName.LastNameMarker;
            pn.NameStartDate = CprBroker.Utilities.Dates.DateToDecimal(historicalName.NameStartDate.Value, 12);
            pn.NameTerminationDate = CprBroker.Utilities.Dates.DateToDecimal(historicalName.NameEndDate.Value, 12);
            pn.AddressingNameDate = null; //TODO: Find origin for AddressingNameDate - SHOULD BE IN CPR...
            pn.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker - SHOULD BE IN CPR...
            pn.AddressingNameReportingMarker = null; //TODO: Find origin for AddressingNameReportingMarker - SHOULD BE IN CPR...
            pn.AuthorityTextUpdateDate = null; //TODO: Find origin for AuthorityTextUpdateDate - DPR SPECIFIC
            pn.SearchNameDate = 0; //Said to be always 0
            pn.FirstName = historicalName.FirstName_s;
            pn.LastName = historicalName.LastName;
            pn.AddressingName = null; //TODO: Find origin for AddressingName - SHOULD BE IN CPR...
            pn.SearchName = ""; //Said to be always blank
            pn.NameAuthorityText = null; //TODO: Find origin for NameAuthorityText - SHOULD BE IN CPR...
            //return pn;
            throw new NotImplementedException();
        }

        public static CivilStatus ToDpr(this CurrentCivilStatusType currentCivilStatus)
        {
            CivilStatus cs = new CivilStatus();
            cs.PNR = Decimal.Parse(currentCivilStatus.PNR);
            cs.UpdateDateOfCpr = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
            cs.MaritalStatus = currentCivilStatus.CivilStatusCode;
            cs.MaritalStatusAuthorityCode = null; //TODO: Find origin for MaritalStatusAuthorityCode - SHOULD BE IN CPR...
            cs.SpousePNR = Decimal.Parse(currentCivilStatus.SpousePNR);
            cs.SpouseBirthdate = CprBroker.Utilities.Dates.DateToDecimal(currentCivilStatus.SpouseBirthDate.Value, 8);
            cs.SpouseDocumentation = null; //TODO: Find origin for SpouseDocumentation - SHOULD BE IN CPR...
            cs.MaritalStatusDate = CprBroker.Utilities.Dates.DateToDecimal(currentCivilStatus.CivilStatusStartDate.Value, 12);
            cs.MaritalEndDate = null; //This is the current status
            cs.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker - SHOULD BE IN CPR...
            cs.AuthorityTextUpdateDate = null; //TODO: Find origin for AuthorityTextUpdateDate - DPR SPECIFIC
            cs.MaritalStatusAuthorityText = null; //TODO: Find origin for MaritalStatusAuthorityText - SHOULD BE IN CPR...
            cs.SpouseName = currentCivilStatus.SpouseName;
            cs.SeparationReferralTimestamp = currentCivilStatus.ReferenceToAnySeparation.Value.ToString();
            //return cs;
            throw new NotImplementedException();
        }

        public static CivilStatus ToDpr(this HistoricalCivilStatusType historicalCivilStatus)
        {
            CivilStatus cs = new CivilStatus();
            cs.PNR = Decimal.Parse(historicalCivilStatus.PNR);
            cs.UpdateDateOfCpr = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.Registration.RegistrationDate, 12);
            cs.MaritalStatus = historicalCivilStatus.CivilStatusCode;
            cs.MaritalStatusAuthorityCode = null; //TODO: Find origin for MaritalStatusAuthorityText - SHOULD BE IN CPR...
            cs.SpousePNR = Decimal.Parse(historicalCivilStatus.SpousePNR);
            cs.SpouseBirthdate = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.SpouseBirthdate.Value, 8);
            cs.SpouseDocumentation = null; //TODO: Find origin for SpouseDocumentation - SHOULD BE IN CPR...
            cs.MaritalStatusDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.CivilStatusStartDate.Value, 12);
            cs.MaritalEndDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.CivilStatusEndDate.Value, 12);
            cs.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker - SHOULD BE IN CPR...
            cs.AuthorityTextUpdateDate = null; //TODO: Find origin for AuthorityTextUpdateDate - DPR SPECIFIC
            cs.MaritalStatusAuthorityText = null; //TODO: Find origin for MaritalStatusAuthorityText - SHOULD BE IN CPR...
            cs.SpouseName = historicalCivilStatus.SpouseName;
            cs.SeparationReferralTimestamp = historicalCivilStatus.ReferenceToAnySeparation.Value.ToString();
            //return cs;
            throw new NotImplementedException();
        }

        public static Separation ToDpr(this CurrentSeparationType currentSeparation)
        {
            Separation s = new Separation();
            s.PNR = Decimal.Parse(currentSeparation.PNR);
            s.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
            s.SeparationReferalTimestamp = currentSeparation.ReferenceToAnyMaritalStatus.Value.ToString();
            s.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker - SHOULD BE IN CPR...
            s.StartAuthorityCode = 0; //TODO: Find origin for StartAuthorityCode - SHOULD BE IN CPR...
            s.StartDate = currentSeparation.SeparationStartDate.Value;
            s.StartDateMarker = currentSeparation.SeparationStartDateUncertainty;
            s.EndAuthorityCode = null; //This is the current separation
            s.EndDate = null; //This is the current separation
            s.EndDateMarker = null; //This is the current separation
            //return s;
            throw new NotImplementedException();
        }

        public static Separation ToDpr(this HistoricalSeparationType historicalSeparation)
        {
            Separation s = new Separation();
            s.PNR = Decimal.Parse(historicalSeparation.PNR);
            s.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
            s.SeparationReferalTimestamp = historicalSeparation.ReferenceToAnyMaritalStatus.Value.ToString();
            s.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker - SHOULD BE IN CPR...
            s.StartAuthorityCode = 0; //TODO: Find origin for StartAuthorityCode - SHOULD BE IN CPR...
            s.StartDate = historicalSeparation.SeparationStartDate.Value;
            s.StartDateMarker = historicalSeparation.SeparationStartDateUncertainty;
            s.EndAuthorityCode = 0; //TODO: Find origin for EndAuthorityCode - SHOULD BE IN CPR...
            s.EndDate = historicalSeparation.SeparationEndDate.Value;
            s.EndDateMarker = historicalSeparation.SeparationEndDateUncertainty;
            //return s;
            throw new NotImplementedException();
        }

        public static Nationality ToDpr(this CurrentCitizenshipType currentCitizenship)
        {
            Nationality n = new Nationality();
            n.PNR = Decimal.Parse(currentCitizenship.PNR);
            n.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
            n.CountryCode = currentCitizenship.CountryCode;
            n.NationalityStartDate = CprBroker.Utilities.Dates.DateToDecimal(currentCitizenship.CitizenshipStartDate.Value, 12);
            n.NationalityEndDate = null; // This is the current nationality
            n.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker - SHOULD BE IN CPR...
            //return n;
            throw new NotImplementedException();
        }

        public static Nationality ToDpr(this HistoricalCitizenshipType historicalCitizenship)
        {
            Nationality n = new Nationality();
            n.PNR = Decimal.Parse(historicalCitizenship.PNR);
            n.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
            n.CountryCode = historicalCitizenship.CountryCode;
            n.NationalityStartDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCitizenship.CitizenshipStartDate.Value, 12);
            n.NationalityEndDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCitizenship.CitizenshipEndDate.Value, 12);
            n.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker - SHOULD BE IN CPR...
            //return n;
            throw new NotImplementedException();
        }

        public static Departure ToDpr(this CurrentDepartureDataType currentDeparture)
        {
            Departure d = new Departure();
            d.PNR = Decimal.Parse(currentDeparture.PNR);
            d.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
            d.ExitCountryCode = currentDeparture.ExitCountryCode;
            d.ExitDate = CprBroker.Utilities.Dates.DateToDecimal(currentDeparture.ExitDate.Value, 12);
            d.ExitUpdateDate = null; //TODO: Find origin for ExitUpdateDate - DPR SPECIFIC
            d.ForeignAddressDate = null; //TODO: Find origin for ForeignAddressDate - SHOULD BE IN CPR...
            d.VotingDate = null; //TODO: Find origin for VotingDate - SHOULD BE IN CPR...
            d.EntryCountryCode = null; //TODO: Find origin for EntryCountryCode - SHOULD BE IN CPR...
            d.EntryDate = null; // This is the current departure
            d.EntryUpdateDate = null; //TODO: Find origin for EntryUpdateDate - DPR SPECIFIC
            d.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker - SHOULD BE IN CPR...
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
            d.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
            d.ExitCountryCode = historicalDeparture.ExitCountryCode;
            d.ExitDate = CprBroker.Utilities.Dates.DateToDecimal(historicalDeparture.ExitDate.Value, 12);
            d.ExitUpdateDate = null; //TODO: Find origin for ExitUpdateDate - DPR SPECIFIC
            d.ForeignAddressDate = null; //TODO: Find origin for ForeignAddressDate - SHOULD BE IN CPR...
            d.VotingDate = null; //TODO: Find origin for VotingDate - SHOULD BE IN CPR...
            d.EntryCountryCode = historicalDeparture.EntryCountryCode;
            d.EntryDate = CprBroker.Utilities.Dates.DateToDecimal(historicalDeparture.EntryDate.Value, 12);
            d.EntryUpdateDate = null; //TODO: Find origin for EntryUpdateDate - DPR SPECIFIC
            d.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker - SHOULD BE IN CPR...
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
            ca.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
            ca.MunicipalityCode = 0; //TODO: Find origin for MunicipalityCode - SHOULD BE IN CPR...
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
            pa.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
            pa.MunicipalityCode = currentAddress.CurrentAddressInformation.MunicipalityCode;
            pa.StreetCode = currentAddress.CurrentAddressInformation.StreetCode;
            pa.HouseNumber = currentAddress.CurrentAddressInformation.HouseNumber;
            pa.Floor = currentAddress.CurrentAddressInformation.Floor;
            pa.DoorNumber = currentAddress.CurrentAddressInformation.Door;
            pa.GreenlandConstructionNumber = null; //TODO: Find origin for GreenlandConstructionNumber - SHOULD BE IN CPR...
            pa.PostCode = 0; //TODO: Find origin for PostCode - SHOULD BE IN CPR...
            pa.MunicipalityName = null; //TODO: Find origin for MunicipalityName - SHOULD BE IN CPR...
            pa.StreetAddressingName = null; //TODO: Find origin for StreetAddressingName - SHOULD BE IN CPR...
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
            pa.AdditionalAddressDate = null; //TODO: Find origin for AdditionalAddressDate - SHOULD BE IN CPR...
            pa.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker - SHOULD BE IN CPR...
            pa.CareOfName = currentAddress.CurrentAddressInformation.CareOfName;
            pa.Town = null; //TODO: Find origin for Town - SHOULD BE IN CPR...
            pa.Location = null; //TODO: Find origin for Location - SHOULD BE IN CPR...
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
            pa.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
            pa.MunicipalityCode = historicalAddress.MunicipalityCode;
            pa.StreetCode = historicalAddress.StreetCode;
            pa.HouseNumber = historicalAddress.HouseNumber;
            pa.Floor = historicalAddress.Floor;
            pa.DoorNumber = historicalAddress.Door;
            pa.GreenlandConstructionNumber = null; //TODO: Find origin for GreenlandConstructionNumber - SHOULD BE IN CPR...
            pa.PostCode = 0; //TODO: Find origin for PostCode - SHOULD BE IN CPR...
            pa.MunicipalityName = null; //TODO: Find origin for MunicipalityName - SHOULD BE IN CPR...
            pa.StreetAddressingName = null; //TODO: Find origin for StreetAddressingName - SHOULD BE IN CPR...
            pa.AddressStartDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.RelocationDate.Value, 8);
            pa.AddressStartDateMarker = historicalAddress.RelocationDateUncertainty;
            pa.AddressEndDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.LeavingDate.Value, 8);
            pa.LeavingFromMunicipalityCode = null; //TODO: Find origin for LeavingFromMunicipalityCode - SHOULD BE IN CPR...
            pa.LeavingFromMunicipalityDate = null; //TODO: Find origin for LeavingFromMunicipalityDate - SHOULD BE IN CPR...
            pa.MunicipalityArrivalDate = null; //TODO: Find origin for MunicipalityArrivalDate - SHOULD BE IN CPR...
            pa.AlwaysNull1 = null;
            pa.AlwaysNull2 = null;
            pa.AlwaysNull3 = null;
            pa.AlwaysNull4 = null;
            pa.AlwaysNull5 = null;
            pa.AdditionalAddressDate = null; //TODO: Find origin for AdditionalAddressDate - SHOULD BE IN CPR...
            pa.CorrectionMarker = historicalAddress.CorrectionMarker;
            pa.CareOfName = historicalAddress.CareOfName;
            pa.Town = null; //TODO: Find origin for Town - SHOULD BE IN CPR...
            pa.Location = null; //TODO: Find origin for Location - SHOULD BE IN CPR...
            pa.AdditionalAddressLine1 = null; //TODO: Find origin for AdditionalAddressLine1 - SHOULD BE IN CPR...
            pa.AdditionalAddressLine2 = null; //TODO: Find origin for AdditionalAddressLine2 - SHOULD BE IN CPR...
            pa.AdditionalAddressLine3 = null; //TODO: Find origin for AdditionalAddressLine3 - SHOULD BE IN CPR...
            pa.AdditionalAddressLine4 = null; //TODO: Find origin for AdditionalAddressLine4 - SHOULD BE IN CPR...
            pa.AdditionalAddressLine5 = null; //TODO: Find origin for AdditionalAddressLine5 - SHOULD BE IN CPR...
            //return pa;
            throw new NotImplementedException();
        }

        public static Protection ToDpr(this ProtectionType protection)
        {
            Protection p = new Protection();
            p.PNR = Decimal.Parse(protection.PNR);
            p.ProtectionType = protection.ProtectionType_;
            p.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
            p.StartDate = protection.StartDate.Value;
            p.EndDate = protection.EndDate;
            p.ReportingMarker = null; //TODO: Find origin for ReportingMarker - SHOULD BE IN CPR...
            throw new NotImplementedException();
        }

        public static Disappearance ToDpr(this CurrentDisappearanceInformationType disappearance)
        {
            Disappearance d = new Disappearance();
            d.PNR = decimal.Parse(disappearance.PNR);
            d.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
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
            d.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
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
            e.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
            e.Event_ = events.Event_;
            e.AFLMRK = events.DerivedMark;
            //return e;
            throw new NotImplementedException();
        }

        public static Note ToDpr(this NotesType notes)
        {
            Note n = new Note();
            n.PNR = decimal.Parse(notes.PNR);
            n.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
            n.NationalRegisterMemoDate = CprBroker.Utilities.Dates.DateToDecimal(notes.StartDate.Value, 8);
            n.DeletionDate = CprBroker.Utilities.Dates.DateToDecimal(notes.EndDate.Value, 8);
            n.NoteNumber = notes.NoteNumber;
            n.NationalRegisterNoteLine = notes.NoteText;
            n.MunicipalityCode = 0; //TODO: Find origin for MunicipalityCode
            //return n;
            throw new NotImplementedException();
        }

        public static MunicipalCondition ToDpr(this MunicipalConditionsType condition)
        {
            MunicipalCondition m = new MunicipalCondition();
            m.PNR = decimal.Parse(condition.PNR);
            m.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
            m.ConditionType = condition.MunicipalConditionType;
            m.ConditionMarker = null; //TODO: Find origin for ConditionMarker - SHOULD BE IN CPR...
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
            p.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
            p.ParentalAuthorityCode = 0; //TODO: Find origin for ParentalAuthorityCode - SHOULD BE IN CPR...
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
            gapa.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
            gapa.StartDate = disempowerment.DisempowermentStartDate.Value;
            gapa.EndDate = disempowerment.DisempowermentEndDate.Value;
            gapa.AuthorityCode = 0; //TODO: Find origin for AuthorityCode - SHOULD BE IN CPR...
            //return gapa;
            throw new NotImplementedException();
        }

        public static GuardianAddress ToDprAddress(this DisempowermentType disempowerment)
        {
            GuardianAddress ga = new GuardianAddress();
            ga.PNR = decimal.Parse(disempowerment.PNR);
            ga.Address = null; //TODO: Find origin for Address - SHOULD BE IN CPR...
            ga.RelationType = disempowerment.GuardianRelationType;
            ga.CprUpdateDate = 0; //TODO: Find origin for UpdateDateOfCpr - DPR SPECIFIC
            ga.AddressLine1 = disempowerment.RelationText1;
            ga.AddressLine2 = disempowerment.RelationText2;
            ga.AddressLine3 = disempowerment.RelationText3;
            ga.AddressLine4 = disempowerment.RelationText4;
            ga.AddressLine5 = disempowerment.RelationText5;
            ga.StartDate = disempowerment.GuardianAddressStartDate.Value;
            ga.EndDate = null; //TODO: Find origin for EndDate - SHOULD BE IN CPR...
            ga.AuthorityCode = 0; //TODO: Find origin for AuthorityCode - SHOULD BE IN CPR...
            //return ga;
            throw new NotImplementedException();
        }


    }
}
