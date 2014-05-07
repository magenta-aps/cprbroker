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
            //pt.MunicipalityName = TODO: Find municipality name based on the municipality code
            pt.StreetCode = resp.CurrentAddressInformation.StreetCode;
            pt.HouseNumber = resp.CurrentAddressInformation.HouseNumber;
            pt.Floor = resp.CurrentAddressInformation.Floor;
            pt.Door = resp.CurrentAddressInformation.Door;
            pt.ConstructionNumber = resp.CurrentAddressInformation.BuildingNumber;
            //return pt;
            throw new NotImplementedException();
        }

        public static Person ToPerson(this IndividualResponseType person)
        {
            throw new NotImplementedException();
        }

        public static Child ToDpr(this ChildType child)
        {
            Child ch = new Child();
            ch.ChildPNR = Decimal.Parse(child.ChildPNR);
            ch.ParentPNR = Decimal.Parse(child.PNR);
            //ch.MotherOrFatherDocumentation = TODO: find origin for childrens mother or father documentation.
            //return ch;
            throw new NotImplementedException();
        }

        public static PersonName ToDpr(this CurrentNameInformationType currentName)
        {
            PersonName pn = new PersonName();
            pn.PNR = Decimal.Parse(currentName.PNR);
            //TODO: Figure out where the 'CprUpdateDate' originates.
            pn.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentName.Registration.RegistrationDate, 12);
            pn.NameAuthorityCode = null; //TODO: Find origin for NameAuthorityCode
            pn.Status = null; //TODO: Find origin for Status
            pn.StatusDate = null; //TODO: Find origin for StatusDate
            pn.FirstNameMarker = currentName.FirstNameMarker;
            pn.SurnameMarker = currentName.LastNameMarker;
            pn.NameStartDate = CprBroker.Utilities.Dates.DateToDecimal(currentName.NameStartDate.Value ,12);
            pn.NameTerminationDate = null; //TODO: Find origin for NameTerminationDate
            pn.AddressingNameDate = null; //TODO: Find origin for AddressingNameDate
            pn.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker
            pn.AddressingNameReportingMarker = null; //TODO: Find origin for AddressingNameReportingMarker
            pn.AuthorityTextUpdateDate = null; //TODO: Find origin for AuthorityTextUpdateDate
            pn.SearchNameDate = null; //TODO: Find origin for SearchNameDate
            pn.FirstName = currentName.FirstName_s;
            pn.LastName = currentName.LastName;
            pn.AddressingName = currentName.AddressingName;
            pn.SearchName = null; //TODO: Find origin for SearchName
            pn.NameAuthorityText = null; //TODO: Find origin for NameAuthorityText
            //return pn;
            throw new NotImplementedException();
        }

        public static PersonName ToDpr(this HistoricalNameType historicalName)
        {
            PersonName pn = new PersonName();
            pn.PNR = Decimal.Parse(historicalName.PNR);
            //TODO: Figure out where the 'CprUpdateDate' originates.
            pn.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalName.Registration.RegistrationDate, 12);
            pn.NameAuthorityCode = null; //TODO: Find origin for NameAuthorityCode
            pn.Status = null; //TODO: Find origin for Status
            pn.StatusDate = null; //TODO: Find origin for StatusDate
            pn.FirstNameMarker = historicalName.FirstNameMarker;
            pn.SurnameMarker = historicalName.LastNameMarker;
            pn.NameStartDate = CprBroker.Utilities.Dates.DateToDecimal(historicalName.NameStartDate.Value, 12);
            pn.NameTerminationDate = CprBroker.Utilities.Dates.DateToDecimal(historicalName.NameEndDate.Value, 12);
            pn.AddressingNameDate = null; //TODO: Find origin for AddressingNameDate
            pn.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker
            pn.AddressingNameReportingMarker = null; //TODO: Find origin for AddressingNameReportingMarker
            pn.AuthorityTextUpdateDate = null; //TODO: Find origin for AuthorityTextUpdateDate
            pn.SearchNameDate = null; //TODO: Find origin for SearchNameDate
            pn.FirstName = historicalName.FirstName_s;
            pn.LastName = historicalName.LastName;
            pn.AddressingName = null; //TODO: Find origin for AddressingName
            pn.SearchName = null; //TODO: Find origin for SearchName
            pn.NameAuthorityText = null; //TODO: Find origin for NameAuthorityText
            //return pn;
            throw new NotImplementedException();
        }

        public static CivilStatus ToDpr(this CurrentCivilStatusType currentCivilStatus)
        {
            CivilStatus cs = new CivilStatus();
            cs.PNR = Decimal.Parse(currentCivilStatus.PNR);
            cs.UpdateDateOfCpr = CprBroker.Utilities.Dates.DateToDecimal(currentCivilStatus.Registration.RegistrationDate, 12);
            cs.MaritalStatus = currentCivilStatus.CivilStatusCode;
            cs.MaritalStatusAuthorityCode = null; //TODO: Find origin for MaritalStatusAuthorityCode
            cs.SpousePNR = Decimal.Parse(currentCivilStatus.SpousePNR);
            cs.SpouseBirthdate = CprBroker.Utilities.Dates.DateToDecimal(currentCivilStatus.SpouseBirthDate.Value, 8);
            cs.SpouseDocumentation = null; //TODO: Find origin for SpouseDocumentation
            cs.MaritalStatusDate = CprBroker.Utilities.Dates.DateToDecimal(currentCivilStatus.CivilStatusStartDate.Value, 12);
            cs.MaritalEndDate = null; //This is the current status
            cs.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker
            cs.AuthorityTextUpdateDate = null; //TODO: Find origin for AuthorityTextUpdateDate
            cs.MaritalStatusAuthorityText = null; //TODO: Find origin for MaritalStatusAuthorityText
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
            cs.MaritalStatusAuthorityCode = null; //TODO: Find origin for MaritalStatusAuthorityText
            cs.SpousePNR = Decimal.Parse(historicalCivilStatus.SpousePNR);
            cs.SpouseBirthdate = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.SpouseBirthdate.Value, 8);
            cs.SpouseDocumentation = null; //TODO: Find origin for SpouseDocumentation
            cs.MaritalStatusDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.CivilStatusStartDate.Value, 12);
            cs.MaritalEndDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.CivilStatusEndDate.Value, 12);
            cs.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker
            cs.AuthorityTextUpdateDate = null; //TODO: Find origin for AuthorityTextUpdateDate
            cs.MaritalStatusAuthorityText = null; //TODO: Find origin for MaritalStatusAuthorityText
            cs.SpouseName = historicalCivilStatus.SpouseName;
            cs.SeparationReferralTimestamp = historicalCivilStatus.ReferenceToAnySeparation.Value.ToString();
            //return cs;
            throw new NotImplementedException();
        }

        public static Separation ToDpr(this CurrentSeparationType currentSeparation)
        {
            Separation s = new Separation();
            s.PNR = Decimal.Parse(currentSeparation.PNR);
            //TODO: Figure out where the 'CprUpdateDate' originates.
            s.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentSeparation.Registration.RegistrationDate, 12);
            s.SeparationReferalTimestamp = currentSeparation.ReferenceToAnyMaritalStatus.Value.ToString();
            s.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker
            s.StartAuthorityCode = 0; //TODO: Find origin for StartAuthorityCode
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
            //TODO: Figure out where the 'CprUpdateDate' originates.
            s.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalSeparation.Registration.RegistrationDate, 12);
            s.SeparationReferalTimestamp = historicalSeparation.ReferenceToAnyMaritalStatus.Value.ToString();
            s.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker
            s.StartAuthorityCode = 0; //TODO: Find origin for StartAuthorityCode
            s.StartDate = historicalSeparation.SeparationStartDate.Value;
            s.StartDateMarker = historicalSeparation.SeparationStartDateUncertainty;
            s.EndAuthorityCode = 0; //TODO: Find origin for EndAuthorityCode
            s.EndDate = historicalSeparation.SeparationEndDate.Value;
            s.EndDateMarker = historicalSeparation.SeparationEndDateUncertainty;
            //return s;
            throw new NotImplementedException();
        }

        public static Nationality ToDpr(this CurrentCitizenshipType currentCitizenship)
        {
            Nationality n = new Nationality();
            n.PNR = Decimal.Parse(currentCitizenship.PNR);
            //TODO: Figure out where the 'CprUpdateDate' originates.
            n.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentCitizenship.Registration.RegistrationDate, 12);
            n.CountryCode = currentCitizenship.CountryCode;
            n.NationalityStartDate = CprBroker.Utilities.Dates.DateToDecimal(currentCitizenship.CitizenshipStartDate.Value, 12);
            n.NationalityEndDate = null; // This is the current nationality
            n.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker
            //return n;
            throw new NotImplementedException();
        }

        public static Nationality ToDpr(this HistoricalCitizenshipType historicalCitizenship)
        {
            Nationality n = new Nationality();
            n.PNR = Decimal.Parse(historicalCitizenship.PNR);
            //TODO: Figure out where the 'CprUpdateDate' originates.
            n.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCitizenship.Registration.RegistrationDate, 12);
            n.CountryCode = historicalCitizenship.CountryCode;
            n.NationalityStartDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCitizenship.CitizenshipStartDate.Value, 12);
            n.NationalityEndDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCitizenship.CitizenshipEndDate.Value, 12);
            n.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker
            //return n;
            throw new NotImplementedException();
        }

        public static Departure ToDpr(this CurrentDepartureDataType currentDeparture)
        {
            Departure d = new Departure();
            d.PNR = Decimal.Parse(currentDeparture.PNR);
            //TODO: Figure out where the 'CprUpdateDate' originates.
            d.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentDeparture.Registration.RegistrationDate, 12);
            d.ExitCountryCode = currentDeparture.ExitCountryCode;
            d.ExitDate = CprBroker.Utilities.Dates.DateToDecimal(currentDeparture.ExitDate.Value, 12);
            d.ExitUpdateDate = null; //TODO: Find origin for ExitUpdateDate
            d.ForeignAddressDate = null; //TODO: Find origin for ForeignAddressDate
            d.VotingDate = null; //TODO: Find origin for VotingDate
            d.EntryCountryCode = null; //TODO: Find origin for EntryCountryCode
            d.EntryDate = null; // This is the current departure
            d.EntryUpdateDate = null; //TODO: Find origin for EntryUpdateDate
            d.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker
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
            //TODO: Figure out where the 'CprUpdateDate' originates.
            d.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalDeparture.Registration.RegistrationDate, 12);
            d.ExitCountryCode = historicalDeparture.ExitCountryCode;
            d.ExitDate = CprBroker.Utilities.Dates.DateToDecimal(historicalDeparture.ExitDate.Value, 12);
            d.ExitUpdateDate = null; //TODO: Find origin for ExitUpdateDate
            d.ForeignAddressDate = null; //TODO: Find origin for ForeignAddressDate
            d.VotingDate = null; //TODO: Find origin for VotingDate
            d.EntryCountryCode = historicalDeparture.EntryCountryCode;
            d.EntryDate = CprBroker.Utilities.Dates.DateToDecimal(historicalDeparture.EntryDate.Value, 12);
            d.EntryUpdateDate = null; //TODO: Find origin for EntryUpdateDate
            d.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker
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
            //TODO: Figure out where the 'CprUpdateDate' originates.
            ca.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(contactAddress.Registration.RegistrationDate, 12);
            ca.MunicipalityCode = 0; //TODO: Find origin for MunicipalityCode
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
            //TODO: Figure out where the 'CprUpdateDate' originates.
            pa.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.Registration.RegistrationDate, 12);
            pa.MunicipalityCode = currentAddress.CurrentAddressInformation.MunicipalityCode;
            pa.StreetCode = currentAddress.CurrentAddressInformation.StreetCode;
            pa.HouseNumber = currentAddress.CurrentAddressInformation.HouseNumber;
            pa.Floor = currentAddress.CurrentAddressInformation.Floor;
            pa.DoorNumber = currentAddress.CurrentAddressInformation.Door;
            pa.GreenlandConstructionNumber = null; //TODO: Find origin for GreenlandConstructionNumber
            pa.PostCode = 0; //TODO: Find origin for PostCode
            pa.MunicipalityName = null; //TODO: Find origin for MunicipalityName
            pa.StreetAddressingName = null; //TODO: Find origin for StreetAddressingName
            pa.AddressStartDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.StartDate.Value, 8);
            pa.AddressStartDateMarker = null; //TODO: Find origin for AddressStartDateMarker
            pa.AddressEndDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.EndDate.Value, 8);
            pa.LeavingFromMunicipalityCode = currentAddress.CurrentAddressInformation.LeavingMunicipalityCode;
            pa.LeavingFromMunicipalityDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.LeavingMunicipalityDepartureDate.Value, 12);
            pa.MunicipalityArrivalDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.MunicipalityArrivalDate.Value, 12);
            pa.AlwaysNull1 = null;
            pa.AlwaysNull2 = null;
            pa.AlwaysNull3 = null;
            pa.AlwaysNull4 = null;
            pa.AlwaysNull5 = null;
            pa.AdditionalAddressDate = null; //TODO: Find origin for AdditionalAddressDate
            pa.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker
            pa.CareOfName = currentAddress.CurrentAddressInformation.CareOfName;
            pa.Town = null; //TODO: Find origin for Town
            pa.Location = null; //TODO: Find origin for Location
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
            //TODO: Figure out where the 'CprUpdateDate' originates.
            pa.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.Registration.RegistrationDate, 12);
            pa.MunicipalityCode = historicalAddress.MunicipalityCode;
            pa.StreetCode = historicalAddress.StreetCode;
            pa.HouseNumber = historicalAddress.HouseNumber;
            pa.Floor = historicalAddress.Floor;
            pa.DoorNumber = historicalAddress.Door;
            pa.GreenlandConstructionNumber = null; //TODO: Find origin for GreenlandConstructionNumber
            pa.PostCode = 0; //TODO: Find origin for PostCode
            pa.MunicipalityName = null; //TODO: Find origin for MunicipalityName
            pa.StreetAddressingName = null; //TODO: Find origin for StreetAddressingName
            pa.AddressStartDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.RelocationDate.Value, 8);
            pa.AddressStartDateMarker = historicalAddress.RelocationDateUncertainty;
            pa.AddressEndDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.LeavingDate.Value, 8);
            pa.LeavingFromMunicipalityCode = null; //TODO: Find origin for LeavingFromMunicipalityCode
            pa.LeavingFromMunicipalityDate = null; //TODO: Find origin for LeavingFromMunicipalityDate
            pa.MunicipalityArrivalDate = null; //TODO: Find origin for MunicipalityArrivalDate
            pa.AlwaysNull1 = null;
            pa.AlwaysNull2 = null;
            pa.AlwaysNull3 = null;
            pa.AlwaysNull4 = null;
            pa.AlwaysNull5 = null;
            pa.AdditionalAddressDate = null; //TODO: Find origin for AdditionalAddressDate
            pa.CorrectionMarker = historicalAddress.CorrectionMarker;
            pa.CareOfName = historicalAddress.CareOfName;
            pa.Town = null; //TODO: Find origin for Town
            pa.Location = null; //TODO: Find origin for Location
            pa.AdditionalAddressLine1 = null; //TODO: Find origin for AdditionalAddressLine1
            pa.AdditionalAddressLine2 = null; //TODO: Find origin for AdditionalAddressLine2
            pa.AdditionalAddressLine3 = null; //TODO: Find origin for AdditionalAddressLine3
            pa.AdditionalAddressLine4 = null; //TODO: Find origin for AdditionalAddressLine4
            pa.AdditionalAddressLine5 = null; //TODO: Find origin for AdditionalAddressLine5
            //return pa;
            throw new NotImplementedException();
        }

        public static Protection ToDpr(this ProtectionType protection)
        {
            Protection p = new Protection();
            p.PNR = Decimal.Parse(protection.PNR);
            p.ProtectionType = protection.ProtectionType_;
            //TODO: Figure out where the 'CprUpdateDate' originates.
            p.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(protection.Registration.RegistrationDate, 12);
            p.StartDate = protection.StartDate.Value;
            p.EndDate = protection.EndDate;
            p.ReportingMarker = null; //TODO: Find origin for ReportingMarker
            throw new NotImplementedException();
        }

        public static Disappearance ToDpr(this CurrentDisappearanceInformationType disappearance)
        {
            Disappearance d = new Disappearance();
            d.PNR = decimal.Parse(disappearance.PNR);
            //TODO: Figure out where the 'CprUpdateDate' originates.
            d.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(disappearance.Registration.RegistrationDate, 12);
            d.DisappearanceDate = CprBroker.Utilities.Dates.DateToDecimal(disappearance.DisappearanceDate.Value, 12);
            d.RetrievalDate = null; // It is the current disappearance
            d.CorrectionMarker = null; //TODO: Find origin for CorrectionMarker
            //return d;
            throw new NotImplementedException();
        }

        public static Disappearance ToDpr(this HistoricalDisappearanceType disappearance)
        {
            Disappearance d = new Disappearance();
            d.PNR = decimal.Parse(disappearance.PNR);
            //TODO: Figure out where the 'CprUpdateDate' originates.
            d.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(disappearance.Registration.RegistrationDate, 12);
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
            //TODO: Figure out where the 'CprUpdateDate' originates.
            e.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(events.CprUpdateDate.Value, 12);
            e.Event_ = events.Event_;
            e.AFLMRK = events.DerivedMark;
            //return e;
            throw new NotImplementedException();
        }

        public static Note ToDpr(this NotesType notes)
        {
            Note n = new Note();
            n.PNR = decimal.Parse(notes.PNR);
            n.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(notes.Registration.RegistrationDate, 12);
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
            //TODO: Figure out where the 'CprUpdateDate' originates.
            m.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(condition.Registration.RegistrationDate, 12);
            m.ConditionType = condition.MunicipalConditionType;
            m.ConditionMarker = null; //TODO: Find origin for ConditionMarker
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
            //TODO: Figure out where the 'CprUpdateDate' originates.
            p.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(auth.Registration.RegistrationDate, 12);
            p.ParentalAuthorityCode = 0; //TODO: Find origin for ParentalAuthorityCode
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
            //TODO: Figure out where the 'CprUpdateDate' originates.
            gapa.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(disempowerment.Registration.RegistrationDate, 12);
            gapa.StartDate = disempowerment.DisempowermentStartDate.Value;
            gapa.EndDate = disempowerment.DisempowermentEndDate.Value;
            gapa.AuthorityCode = 0; //TODO: Find origin for AuthorityCode
            //return gapa;
            throw new NotImplementedException();
        }

        public static GuardianAddress ToDprAddress(this DisempowermentType disempowerment)
        {
            GuardianAddress ga = new GuardianAddress();
            ga.PNR = decimal.Parse(disempowerment.PNR);
            ga.Address = null; //TODO: Find origin for Address
            ga.RelationType = disempowerment.GuardianRelationType;
            //TODO: Figure out where the 'CprUpdateDate' originates.
            ga.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(disempowerment.Registration.RegistrationDate, 12);
            ga.AddressLine1 = disempowerment.RelationText1;
            ga.AddressLine2 = disempowerment.RelationText2;
            ga.AddressLine3 = disempowerment.RelationText3;
            ga.AddressLine4 = disempowerment.RelationText4;
            ga.AddressLine5 = disempowerment.RelationText5;
            ga.StartDate = disempowerment.GuardianAddressStartDate.Value;
            ga.EndDate = null; //TODO: Find origin for EndDate
            ga.AuthorityCode = 0; //TODO: Find origin for AuthorityCode
            //return ga;
            throw new NotImplementedException();
        }


    }
}
