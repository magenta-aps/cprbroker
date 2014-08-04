using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;

namespace CprBroker.DBR
{
    public static partial class CprConverterExtensions
    {
        public static PersonTotal ToPersonTotal(this IndividualResponseType resp)
        {
            PersonTotal pt = new PersonTotal();
            /*
             * PERSON DETAILS
             */
            pt.PNR = Decimal.Parse(resp.PersonInformation.PNR);
            if (resp.PersonInformation.StatusStartDate != null)
            {
                pt.StatusDate = CprBroker.Utilities.Dates.DateToDecimal(resp.PersonInformation.StatusStartDate.Value, 12);
            }
            else
            {
                Console.WriteLine("resp.PersonInformation.StatusStartDate was NULL");
                pt.StatusDate = null;//CprBroker.Utilities.Dates.DateToDecimal(resp.PersonInformation.StatusStartDate.Value, 12);
            }
            pt.Status = resp.PersonInformation.Status;

            if (resp.PersonInformation.Birthdate.HasValue)
                pt.DateOfBirth = CprBroker.Utilities.Dates.DateToDecimal(resp.PersonInformation.Birthdate.Value, 8);

            pt.Sex = resp.PersonInformation.Gender;

            /*
             * RESIDENTIAL DETAILS
             */
            var adr = resp.GetFolkeregisterAdresseSource(false);
            if (adr != null)
            {
                var schemaAdr = adr.ToAdresseType();
                if (schemaAdr != null)
                {
                    if (schemaAdr.Item is DanskAdresseType || schemaAdr.Item is GroenlandAdresseType)
                    {
                        pt.MunicipalityCode = resp.CurrentAddressInformation.MunicipalityCode;
                        pt.StreetCode = resp.CurrentAddressInformation.StreetCode;
                        pt.HouseNumber = resp.CurrentAddressInformation.HouseNumber;
                        pt.Floor = resp.CurrentAddressInformation.Floor;
                        pt.Door = resp.CurrentAddressInformation.Door;
                        pt.ConstructionNumber = resp.CurrentAddressInformation.BuildingNumber;

                        if (resp.CurrentAddressInformation.RelocationDate.HasValue)
                            pt.AddressDate = CprBroker.Utilities.Dates.DateToDecimal(resp.CurrentAddressInformation.RelocationDate.Value, 12);
                        if (resp.CurrentAddressInformation.MunicipalityArrivalDate.HasValue)
                            pt.MunicipalityArrivalDate = CprBroker.Utilities.Dates.DateToDecimal(resp.CurrentAddressInformation.MunicipalityArrivalDate.Value, 12);
                        if (resp.CurrentAddressInformation.LeavingMunicipalityDepartureDate.HasValue)
                        {
                            pt.MunicipalityLeavingDate = CprBroker.Utilities.Dates.DateToDecimal(resp.CurrentAddressInformation.LeavingMunicipalityDepartureDate.Value, 12);
                        }
                        else
                        {
                            Console.WriteLine("resp.CurrentAddressInformation.LeavingMunicipalityDepartureDate was NULL");
                            pt.MunicipalityLeavingDate = null;
                        }
                        pt.CareOfName = resp.CurrentAddressInformation.CareOfName;
                        pt.CityName = resp.ClearWrittenAddress.CityName;
                    }
                }
                /*
                 * TODO: FIX THE METHOD IN AUTHORITY!!!
                 */
                pt.CurrentMunicipalityName = null;//CprBroker.Providers.CPRDirect.Authority.GetNameByCode(pt.MunicipalityCode.ToString());
            }

            // TODO: Get from protection records
            pt.AddressProtectionMarker = null; //DPR SPECIFIC
            pt.DirectoryProtectionMarker = null; //DPR SPECIFIC
            pt.ArrivalDateMarker = null; //DPR SPECIFIC


            pt.ChristianMark = resp.ChurchInformation.ChurchRelationship;
            pt.BirthPlaceOfRegistration = resp.BirthRegistrationInformation.AdditionalBirthRegistrationText; //TODO: validate whether this is correct...

            if (resp.PersonInformation.PersonStartDate.HasValue)
                pt.PnrMarkingDate = CprBroker.Utilities.Dates.DateToDecimal(resp.PersonInformation.PersonStartDate.Value, 12);

            pt.MotherPersonalOrBirthDate = resp.ParentsInformation.MotherPNR.Substring(0, 6) + "-" + resp.ParentsInformation.MotherPNR.Substring(6, 4);
            pt.MotherMarker = null; //DPR SPECIFIC
            pt.FatherPersonalOrBirthdate = resp.ParentsInformation.FatherPNR.Substring(0, 6) + "-" + resp.ParentsInformation.FatherPNR.Substring(6, 4);
            pt.FatherMarker = null; //DPR SPECIFIC
            pt.ExitEntryMarker = null; //DPR SPECIFIC
            pt.DisappearedMarker = null; //DPR SPECIFIC

            if (resp.Disempowerment != null && resp.Disempowerment.DisempowermentStartDate.HasValue)
            {
                pt.UnderGuardianshipDate = CprBroker.Utilities.Dates.DateToDecimal(resp.Disempowerment.DisempowermentStartDate.Value, 8);
            }
            else
            {
                Console.WriteLine("resp.Disempowerment was NULL");
                pt.UnderGuardianshipDate = null;
            }

            if (resp.ParentsInformation.FatherDate.HasValue)
                pt.PaternityDate = CprBroker.Utilities.Dates.DateToDecimal(resp.ParentsInformation.FatherDate.Value, 8);

            pt.MaritalStatus = resp.CurrentCivilStatus.CivilStatusCode;

            if (resp.CurrentCivilStatus.CivilStatusStartDate.HasValue)
                pt.MaritalStatusDate = CprBroker.Utilities.Dates.DateToDecimal(resp.CurrentCivilStatus.CivilStatusStartDate.Value, 12);

            if (!string.IsNullOrEmpty(resp.CurrentCivilStatus.SpousePNR))
            {
                // TODO: Shall the target include the leading zeros? 
                pt.SpousePersonalOrBirthdate = resp.CurrentCivilStatus.SpousePNR.Substring(0, 6) + "-" + resp.CurrentCivilStatus.SpousePNR.Substring(6, 4);
            }
            else if (resp.CurrentCivilStatus.SpouseBirthDate.HasValue)
            {
                pt.SpousePersonalOrBirthdate = resp.CurrentCivilStatus.SpouseBirthDate.Value.ToString("dd-MM-yyyy");
            }
            
            pt.SpouseMarker = null; //DPR SPECIFIC
            pt.PostCode = resp.ClearWrittenAddress.PostCode;
            pt.PostDistrictName = resp.ClearWrittenAddress.PostDistrictText;
            var voting = resp.ElectionInformation.OrderByDescending(e => e.ElectionInfoStartDate).FirstOrDefault();

            if (voting != null && voting.VotingDate.HasValue)
                pt.VotingDate = CprBroker.Utilities.Dates.DateToDecimal(voting.VotingDate.Value, 8);
            else
                pt.VotingDate = null;

            pt.ChildMarker = null; //DPR SPECIFIC
            pt.SupplementaryAddressMarker = null; //DPR SPECIFIC
            pt.MunicipalRelationMarker = null; //DPR SPECIFIC
            pt.NationalMemoMarker = null; //DPR SPECIFIC
            pt.FormerPersonalMarker = null; //DPR SPECIFIC
            pt.PaternityAuthorityName = null; //TODO: Retrieve this from the CPR Service field far_mynkod
            pt.MaritalAuthorityName = null; //TODO: Retrieve this from the CPR Service field mynkod
            pt.Occupation = resp.PersonInformation.Job;
            pt.NationalityRight = null; // Find the mun. name by country code via method Authority.GetNameByCountryCode - how?
            var prevAdr = resp.HistoricalAddress
                .Where(e => (e as CprBroker.Schemas.Part.IHasCorrectionMarker).CorrectionMarker == CprBroker.Schemas.Part.CorrectionMarker.OK)
                .OrderByDescending(e => e.ToStartTS())
                .FirstOrDefault();
            
            // TODO: What to do with previous address??
            /*
            var prevMun = resp.HistoricalAddress.OrderByDescending(e => e.MunicipalityCode).FirstOrDefault(); //Find municipality name in GeoLookup, based on mun. code
            pt.PreviousAddress = prevAdr + "(" + prevMun + ")";
            //pt.PreviousMunicipalityName = prevMun;
            */
            pt.SearchName = resp.CurrentNameInformation.FirstName_s.ToUpper();
            pt.SearchSurname = resp.CurrentNameInformation.LastName.ToUpper();
            pt.AddressingName = resp.ClearWrittenAddress.AddressingName;
            pt.StandardAddress = resp.ClearWrittenAddress.LabelledAddress;
            pt.Location = resp.ClearWrittenAddress.Location;
            pt.ContactAddressMarker = null; //DPR SPECIFIC
            return pt;
        }

        public static Person ToPerson(this IndividualResponseType person)
        {
            Person p = new Person();
            p.PNR = decimal.Parse(person.PersonInformation.PNR);
            p.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(person.RegistrationDate, 12);
            p.Birthdate = CprBroker.Utilities.Dates.DateToDecimal(person.PersonInformation.Birthdate.Value, 8);
            p.Gender = person.PersonInformation.Gender.ToString();
            p.CustomerNumber = null; //DPR SPECIFIC
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
            if (person.Disempowerment != null)
            {
                p.UnderGuardianshipDate = CprBroker.Utilities.Dates.DateToDecimal(person.Disempowerment.DisempowermentStartDate.Value, 8);
            }
            else
            {
                Console.WriteLine("person.Disempowerment was NULL");
                p.UnderGuardianshipDate = null;
            }
            /*
             * PNR related
             */
            p.PnrMarkingDate = null; //TODO: Can be fetched in CPR Services: pnrhaenstart
            p.PnrDate = 0; //TODO: Can be fetched in CPR Services: pnrmrkhaenstart 
            p.CurrentPnrUpdateDate = null; //TODO: Can be fetched in CPR Services: timestamp
            if (!string.IsNullOrEmpty(person.PersonInformation.CurrentCprNumber))
            {
                p.CurrentPnr = decimal.Parse(person.PersonInformation.CurrentCprNumber);
            }
            else
            {
                Console.WriteLine("person.PersonInformation.CurrentCprNumber was NULL or empty");
                p.CurrentPnr = null;
            }
            if (person.PersonInformation.PersonEndDate != null)
            {
                p.PnrDeletionDate = CprBroker.Utilities.Dates.DateToDecimal(person.PersonInformation.PersonEndDate.Value, 8);
            }
            else
            {
                Console.WriteLine("person.PersonInformation.PersonEndDate was NULL");
                p.PnrDeletionDate = null;
            }
            /*
             * Position related
             */
            p.JobDate = null; //TODO: Can be fetched in CPR Services: stillingsdato
            p.Job = person.PersonInformation.Job;
            /*
             * Relations related
             */
            p.KinshipUpdateDate = 0; //TODO: Can be fetched in CPR Services: timestamp
            if (!string.IsNullOrEmpty(person.ParentsInformation.MotherPNR))
            {
                p.MotherPnr = decimal.Parse(person.ParentsInformation.MotherPNR);
            }
            else
            {
                Console.WriteLine("person.ParentsInformation.MotherPNR was NULL or empty");
                p.MotherPnr = 0;
            }
            p.KinshipUpdateDate = 0; //TODO: Can be fetched in CPR Services: timestamp
            if (person.ParentsInformation.MotherBirthDate != null)
            {
                p.MotherBirthdate = CprBroker.Utilities.Dates.DateToDecimal(person.ParentsInformation.MotherBirthDate.Value, 8);
            }
            else
            {
                Console.WriteLine("person.ParentsInformation.MotherBirthDate was NULL or empty");
                p.MotherBirthdate = null;
            }
            p.MotherDocumentation = null; //TODO: Can be fetched in CPR Services: mor_dok
            p.FatherPnr = decimal.Parse(person.ParentsInformation.FatherPNR);
            if (person.ParentsInformation.FatherBirthDate != null)
            {
                p.FatherBirthdate = CprBroker.Utilities.Dates.DateToDecimal(person.ParentsInformation.FatherBirthDate.Value, 8);
            }
            else
            {
                Console.WriteLine("person.ParentsInformation.FatherBirthDate was NULL or empty");
                p.FatherBirthdate = null;
            }
            p.FatherDocumentation = null; //TODO: Can be fetched in CPR Services: far_dok
            p.PaternityDate = null; //TODO: Can be fetched in CPR Services: farhaenstart
            p.PaternityAuthorityCode = null; //TODO: Can be fetched in CPR Services: far_mynkod
            p.MotherName = person.ParentsInformation.MotherName;
            p.FatherName = person.ParentsInformation.FatherName;
            if (person.Disempowerment != null)
            {
                if (person.Disempowerment.DisempowermentEndDate.HasValue)
                    p.UnderGuardianshipDeleteDate = person.Disempowerment.DisempowermentEndDate.Value;
                p.UnderGuardianshipRelationType = person.Disempowerment.GuardianRelationType;
            }
            else
            {
                Console.WriteLine("person.Disempowerment was NULL");
                p.UnderGuardianshipDeleteDate = null;
                p.UnderGuardianshipRelationType = null;
            }
            return p;
        }

        public static Child ToDpr(this ChildType child)
        {
            Child ch = new Child();
            ch.ParentPNR = Decimal.Parse(child.PNR);
            ch.ChildPNR = Decimal.Parse(child.ChildPNR);
            ch.MotherOrFatherDocumentation = null; //TODO: This is a concatenation of mother documentation and father documentation - can be retrieved from CPR Services
            return ch;
            //throw new NotImplementedException();
        }

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
            pn.FirstName = currentName.FirstName_s;
            pn.LastName = currentName.LastName;
            pn.AddressingName = currentName.AddressingName;
            pn.SearchName = ""; //Said to be always blank
            pn.NameAuthorityText = null; //TODO: Can be fetched in CPR Services, myntxt
            return pn;
            //throw new NotImplementedException();
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
            pn.FirstName = historicalName.FirstName_s;
            pn.LastName = historicalName.LastName;
            pn.AddressingName = null; // Seems not available in historical records....
            pn.SearchName = ""; //Said to be always blank
            pn.NameAuthorityText = null; //TODO: Can be fetched in CPR Services, myntxt
            return pn;
            //throw new NotImplementedException();
        }

        public static CivilStatus ToDpr(this CurrentCivilStatusType currentCivilStatus)
        {
            CivilStatus cs = new CivilStatus();
            cs.PNR = Decimal.Parse(currentCivilStatus.PNR);
            cs.UpdateDateOfCpr = CprBroker.Utilities.Dates.DateToDecimal(currentCivilStatus.Registration.RegistrationDate, 12);
            cs.MaritalStatus = currentCivilStatus.CivilStatusCode;
            cs.MaritalStatusAuthorityCode = null; //TODO: Can be fetched in CPR Services, mynkod

            if (!string.IsNullOrEmpty(currentCivilStatus.SpousePNR))
                cs.SpousePNR = Decimal.Parse(currentCivilStatus.SpousePNR);

            if (currentCivilStatus.SpouseBirthDate.HasValue)
                cs.SpouseBirthdate = CprBroker.Utilities.Dates.DateToDecimal(currentCivilStatus.SpouseBirthDate.Value, 8);

            cs.SpouseDocumentation = null; //TODO: Can be fetched in CPR Services, aegtedok

            if (currentCivilStatus.CivilStatusStartDate.HasValue)
                cs.MaritalStatusDate = CprBroker.Utilities.Dates.DateToDecimal(currentCivilStatus.CivilStatusStartDate.Value, 12);

            cs.MaritalEndDate = null; //This is the current status
            cs.CorrectionMarker = null; //This is the current status
            cs.AuthorityTextUpdateDate = null; //TODO: Can be fetched in CPR Services,  myntxttimestamp
            cs.MaritalStatusAuthorityText = null; //TODO: Can be fetched in CPR Services,  myntxt
            cs.SpouseName = currentCivilStatus.SpouseName;

            if (currentCivilStatus.ReferenceToAnySeparation.HasValue)
            {
                cs.SeparationReferralTimestamp = currentCivilStatus.ReferenceToAnySeparation.Value.ToString();
            }
            else
            {
                Console.WriteLine("resp.PersonInformation.StatusStartDate was NULL");
                cs.SeparationReferralTimestamp = null;
            }
            return cs;
            //throw new NotImplementedException();
        }

        public static CivilStatus ToDpr(this HistoricalCivilStatusType historicalCivilStatus)
        {
            CivilStatus cs = new CivilStatus();
            cs.PNR = Decimal.Parse(historicalCivilStatus.PNR);
            cs.UpdateDateOfCpr = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.Registration.RegistrationDate, 12);
            cs.MaritalStatus = historicalCivilStatus.CivilStatusCode;
            cs.MaritalStatusAuthorityCode = null; //TODO: Can be fetched in CPR Services, mynkod

            if (!string.IsNullOrEmpty(historicalCivilStatus.SpousePNR))
                cs.SpousePNR = Decimal.Parse(historicalCivilStatus.SpousePNR);

            if (historicalCivilStatus.SpouseBirthdate.HasValue)
                cs.SpouseBirthdate = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.SpouseBirthdate.Value, 8);

            cs.SpouseDocumentation = null; //This is the current status

            if (historicalCivilStatus.CivilStatusStartDate.HasValue)
                cs.MaritalStatusDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.CivilStatusStartDate.Value, 12);

            if (historicalCivilStatus.CivilStatusEndDate.HasValue)
                cs.MaritalEndDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCivilStatus.CivilStatusEndDate.Value, 12);

            cs.CorrectionMarker = historicalCivilStatus.CorrectionMarker;
            cs.AuthorityTextUpdateDate = null; //TODO: Can be fetched in CPR Services, myntxttimestamp
            cs.MaritalStatusAuthorityText = null; //TODO: Can be fetched in CPR Services, myntxt
            cs.SpouseName = historicalCivilStatus.SpouseName;

            if (historicalCivilStatus.ReferenceToAnySeparation.HasValue)
                cs.SeparationReferralTimestamp = historicalCivilStatus.ReferenceToAnySeparation.Value.ToString();

            return cs;
        }

        public static Separation ToDpr(this CurrentSeparationType currentSeparation)
        {
            Separation s = new Separation();
            s.PNR = Decimal.Parse(currentSeparation.PNR);
            s.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentSeparation.Registration.RegistrationDate, 12);
            s.SeparationReferalTimestamp = currentSeparation.ReferenceToAnyMaritalStatus.Value.ToString();
            s.CorrectionMarker = null; //This is the current status
            s.StartAuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod_start
            s.StartDate = currentSeparation.SeparationStartDate.Value;
            s.StartDateMarker = currentSeparation.SeparationStartDateUncertainty;
            s.EndAuthorityCode = null; //TODO: Can be fetched in CPR Services, mynkod_slut
            s.EndDate = null; //This is the current separation
            s.EndDateMarker = null; //This is the current separation            
            return s;
        }

        public static Separation ToDpr(this HistoricalSeparationType historicalSeparation)
        {
            Separation s = new Separation();
            s.PNR = Decimal.Parse(historicalSeparation.PNR);
            s.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalSeparation.Registration.RegistrationDate, 12);
            s.SeparationReferalTimestamp = historicalSeparation.ReferenceToAnyMaritalStatus.Value.ToString();
            s.CorrectionMarker = historicalSeparation.CorrectionMarker;
            s.StartAuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod_start
            s.StartDate = historicalSeparation.SeparationStartDate.Value;
            s.StartDateMarker = historicalSeparation.SeparationStartDateUncertainty;
            s.EndAuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod_slut
            s.EndDate = historicalSeparation.SeparationEndDate.Value;
            s.EndDateMarker = historicalSeparation.SeparationEndDateUncertainty;
            return s;
            //throw new NotImplementedException();
        }

        public static Nationality ToDpr(this CurrentCitizenshipType currentCitizenship)
        {
            Nationality n = new Nationality();
            n.PNR = Decimal.Parse(currentCitizenship.PNR);
            n.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentCitizenship.Registration.RegistrationDate, 12);
            n.CountryCode = currentCitizenship.CountryCode;

            if (currentCitizenship.CitizenshipStartDate.HasValue)
                n.NationalityStartDate = CprBroker.Utilities.Dates.DateToDecimal(currentCitizenship.CitizenshipStartDate.Value, 12);

            n.NationalityEndDate = null; // This is the current nationality
            n.CorrectionMarker = null; //This is the current status
            return n;
        }

        public static Nationality ToDpr(this HistoricalCitizenshipType historicalCitizenship)
        {
            Nationality n = new Nationality();
            n.PNR = Decimal.Parse(historicalCitizenship.PNR);
            n.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCitizenship.Registration.RegistrationDate, 12);
            n.CountryCode = historicalCitizenship.CountryCode;

            if (historicalCitizenship.CitizenshipStartDate.HasValue)
                n.NationalityStartDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCitizenship.CitizenshipStartDate.Value, 12);

            if (historicalCitizenship.CitizenshipEndDate.HasValue)
                n.NationalityEndDate = CprBroker.Utilities.Dates.DateToDecimal(historicalCitizenship.CitizenshipEndDate.Value, 12);

            n.CorrectionMarker = historicalCitizenship.CorrectionMarker;
            return n;
        }

        public static Departure ToDpr(this CurrentDepartureDataType currentDeparture, ElectionInformationType electionInfo)
        {
            Departure d = new Departure();
            if (currentDeparture != null)
            {
                d.PNR = Decimal.Parse(currentDeparture.PNR);
                d.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentDeparture.Registration.RegistrationDate, 12);
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
                d.ForeignAddressLine1 = currentDeparture.ForeignAddress1;
                d.ForeignAddressLine2 = currentDeparture.ForeignAddress2;
                d.ForeignAddressLine3 = currentDeparture.ForeignAddress3;
                d.ForeignAddressLine4 = currentDeparture.ForeignAddress4;
                d.ForeignAddressLine5 = currentDeparture.ForeignAddress5;
            }
            else
            {
                Console.WriteLine("currentDeparture was NULL");
            }
            return d;
            //throw new NotImplementedException();
        }

        public static Departure ToDpr(this HistoricalDepartureType historicalDeparture/*, ElectionInformationType electionInfo*/)
        {
            Departure d = new Departure();
            d.PNR = Decimal.Parse(historicalDeparture.PNR);
            d.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalDeparture.Registration.RegistrationDate, 12);
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
            d.ForeignAddressLine1 = historicalDeparture.ForeignAddress1;
            d.ForeignAddressLine2 = historicalDeparture.ForeignAddress2;
            d.ForeignAddressLine3 = historicalDeparture.ForeignAddress3;
            d.ForeignAddressLine4 = historicalDeparture.ForeignAddress4;
            d.ForeignAddressLine5 = historicalDeparture.ForeignAddress5;
            return d;
        }

        public static ContactAddress ToDpr(this ContactAddressType contactAddress)
        {
            ContactAddress ca = new ContactAddress();
            ca.PNR = Decimal.Parse(contactAddress.PNR);
            ca.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(contactAddress.Registration.RegistrationDate, 12);
            ca.MunicipalityCode = 0; //TODO: Can be fetched in CPR Services, CATX_STARTMYNKOD

            if (contactAddress.StartDate.HasValue)
                ca.AddressDate = CprBroker.Utilities.Dates.DateToDecimal(contactAddress.StartDate.Value, 8);

            ca.ContactAddressLine1 = contactAddress.Line1;
            ca.ContactAddressLine2 = contactAddress.Line2;
            ca.ContactAddressLine3 = contactAddress.Line3;
            ca.ContactAddressLine4 = contactAddress.Line4;
            ca.ContactAddressLine5 = contactAddress.Line5;

            return ca;
        }

        public static PersonAddress ToDpr(this CurrentAddressWrapper currentAddress)
        {
            PersonAddress pa = new PersonAddress();
            pa.PNR = Decimal.Parse(currentAddress.CurrentAddressInformation.PNR);
            pa.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.Registration.RegistrationDate, 12);
            pa.MunicipalityCode = currentAddress.CurrentAddressInformation.MunicipalityCode;
            pa.StreetCode = currentAddress.CurrentAddressInformation.StreetCode;
            pa.HouseNumber = currentAddress.CurrentAddressInformation.HouseNumber;
            pa.Floor = currentAddress.CurrentAddressInformation.Floor;
            pa.DoorNumber = currentAddress.CurrentAddressInformation.Door;
            pa.GreenlandConstructionNumber = currentAddress.ClearWrittenAddress.BuildingNumber;
            pa.PostCode = currentAddress.ClearWrittenAddress.PostCode;
            pa.MunicipalityName = null; // CprBroker.Providers.CPRDirect.Authority.GetNameByCode(pa.MunicipalityCode.ToString());
            pa.StreetAddressingName = currentAddress.ClearWrittenAddress.StreetAddressingName;
            if (currentAddress.CurrentAddressInformation.StartDate != null)
            {
                pa.AddressStartDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.StartDate.Value, 8);
            }
            else
            {
                Console.WriteLine("currentAddress.CurrentAddressInformation.StartDate was NULL");
                pa.AddressStartDate = 0;
            }
            pa.AddressStartDateMarker = ' '; //Defines whether the address is trustable (updated from CPR) - default is ' ', which means that the address is being updated
            if (currentAddress.CurrentAddressInformation.EndDate != null)
            {
                pa.AddressEndDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.EndDate.Value, 8);
            }
            else
            {
                Console.WriteLine("currentAddress.CurrentAddressInformation.EndDate was NULL");
                pa.AddressEndDate = null;
            }
            pa.LeavingFromMunicipalityCode = currentAddress.CurrentAddressInformation.LeavingMunicipalityCode;
            if (currentAddress.CurrentAddressInformation.LeavingMunicipalityDepartureDate != null)
            {
                pa.LeavingFromMunicipalityDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.LeavingMunicipalityDepartureDate.Value, 12);
            }
            else
            {
                Console.WriteLine("currentAddress.CurrentAddressInformation.LeavingMunicipalityDepartureDate was NULL");
                pa.LeavingFromMunicipalityDate = null;
            }
            if (currentAddress.CurrentAddressInformation.MunicipalityArrivalDate != null)
            {
                pa.MunicipalityArrivalDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.MunicipalityArrivalDate.Value, 12);
            }
            else
            {
                Console.WriteLine("currentAddress.CurrentAddressInformation.MunicipalityArrivalDate was NULL");
                pa.MunicipalityArrivalDate = null;
            }
            pa.AlwaysNull1 = null;
            pa.AlwaysNull2 = null;
            pa.AlwaysNull3 = null;
            pa.AlwaysNull4 = null;
            pa.AlwaysNull5 = null;
            if (currentAddress.CurrentAddressInformation.StartDate != null)
            {
                pa.AdditionalAddressDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.StartDate.Value, 8);
            }
            else
            {
                Console.WriteLine("currentAddress.CurrentAddressInformation.StartDate was NULL");
                pa.AdditionalAddressDate = null;
            }

            pa.CorrectionMarker = null; //This is the current status
            pa.CareOfName = currentAddress.CurrentAddressInformation.CareOfName;
            pa.Town = currentAddress.ClearWrittenAddress.CityName;
            pa.Location = currentAddress.ClearWrittenAddress.Location;
            pa.AdditionalAddressLine1 = currentAddress.CurrentAddressInformation.SupplementaryAddress1;
            pa.AdditionalAddressLine2 = currentAddress.CurrentAddressInformation.SupplementaryAddress2;
            pa.AdditionalAddressLine3 = currentAddress.CurrentAddressInformation.SupplementaryAddress3;
            pa.AdditionalAddressLine4 = currentAddress.CurrentAddressInformation.SupplementaryAddress4;
            pa.AdditionalAddressLine5 = currentAddress.CurrentAddressInformation.SupplementaryAddress5;
            return pa;
        }

        public static PersonAddress ToDpr(this HistoricalAddressType historicalAddress)
        {
            PersonAddress pa = new PersonAddress();
            pa.PNR = Decimal.Parse(historicalAddress.PNR);
            pa.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.Registration.RegistrationDate, 12);
            pa.MunicipalityCode = historicalAddress.MunicipalityCode;
            pa.StreetCode = historicalAddress.StreetCode;
            pa.HouseNumber = historicalAddress.HouseNumber;
            pa.Floor = historicalAddress.Floor;
            pa.DoorNumber = historicalAddress.Door;
            pa.GreenlandConstructionNumber = historicalAddress.BuildingNumber;
            pa.PostCode = 0; //Find in GeoLookup, based on streetCode and houseNumber
            pa.MunicipalityName = CprBroker.Providers.CPRDirect.Authority.GetNameByCode(pa.MunicipalityCode.ToString());
            pa.StreetAddressingName = null; //TODO: Can be fetched in CPR Services, vejadrnvn

            // TODO: Shall we use length 12 or 13?
            if (historicalAddress.RelocationDate.HasValue)
                pa.AddressStartDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.RelocationDate.Value, 12);

            pa.AddressStartDateMarker = historicalAddress.RelocationDateUncertainty;

            if (historicalAddress.LeavingDate.HasValue)
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
            pa.Town = null; //Find in GoeLookup, based on street code and house number
            pa.Location = null; //Find in GoeLookup, based on street code and house number
            pa.AdditionalAddressLine1 = null; // Seems not available in historical records....
            pa.AdditionalAddressLine2 = null; // Seems not available in historical records....
            pa.AdditionalAddressLine3 = null; // Seems not available in historical records....
            pa.AdditionalAddressLine4 = null; // Seems not available in historical records....
            pa.AdditionalAddressLine5 = null; // Seems not available in historical records....
            return pa;
        }

        public static Protection ToDpr(this ProtectionType protection)
        {
            Protection p = new Protection();
            p.PNR = Decimal.Parse(protection.PNR);
            p.ProtectionType = protection.ProtectionType_;
            p.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(protection.Registration.RegistrationDate, 12);

            if (protection.StartDate.HasValue)
                p.StartDate = protection.StartDate.Value;

            p.EndDate = protection.EndDate;
            p.ReportingMarker = null; //TODO: Can be fetched in CPR Services, indrap
            return p;
        }

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

        public static Event ToDpr(this EventsType events)
        {
            Event e = new Event();
            e.PNR = decimal.Parse(events.PNR);
            if (events.CprUpdateDate.HasValue)
                e.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(events.CprUpdateDate.Value, 12);
            e.Event_ = events.Event_;
            e.AFLMRK = events.DerivedMark;
            return e;
        }

        public static Note ToDpr(this NotesType notes)
        {
            Note n = new Note();
            n.PNR = decimal.Parse(notes.PNR);
            n.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(notes.Registration.RegistrationDate, 12);

            if (notes.StartDate.HasValue)
                n.NationalRegisterMemoDate = CprBroker.Utilities.Dates.DateToDecimal(notes.StartDate.Value, 8);

            if (notes.EndDate.HasValue)
                n.DeletionDate = CprBroker.Utilities.Dates.DateToDecimal(notes.EndDate.Value, 8);

            n.NoteNumber = notes.NoteNumber;
            n.NationalRegisterNoteLine = notes.NoteText;
            n.MunicipalityCode = 0; //TODO: Can be fetched in CPR Services, komkod
            return n;
        }

        public static MunicipalCondition ToDpr(this MunicipalConditionsType condition)
        {
            MunicipalCondition m = new MunicipalCondition();
            m.PNR = decimal.Parse(condition.PNR);
            m.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(condition.Registration.RegistrationDate, 12);
            m.ConditionType = condition.MunicipalConditionType;
            m.ConditionMarker = condition.MunicipalConditionCode;

            if (condition.MunicipalConditionStartDate.HasValue)
                m.ConditionDate = CprBroker.Utilities.Dates.DateToDecimal(condition.MunicipalConditionStartDate.Value, 8);

            m.ConditionComments = condition.MunicipalConditionComment;
            return m;
        }

        public static ParentalAuthority ToDpr(this ParentalAuthorityType auth)
        {
            ParentalAuthority p = new ParentalAuthority();
            p.ChildPNR = decimal.Parse(auth.PNR);
            p.RelationType = auth.RelationshipType;
            p.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(auth.Registration.RegistrationDate, 12);
            p.ParentalAuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod_start

            if (auth.CustodyStartDate.HasValue)
                p.StartDate = auth.CustodyStartDate.Value;

            p.StartDateUncertainty = auth.CustodyStartDateUncertainty;

            if (auth.CustodyEndDate.HasValue)
                p.EndDate = auth.CustodyEndDate.Value;

            return p;
        }

        public static GuardianAndParentalAuthorityRelation ToDpr(this DisempowermentType disempowerment)
        {
            GuardianAndParentalAuthorityRelation gapa = new GuardianAndParentalAuthorityRelation();
            gapa.PNR = decimal.Parse(disempowerment.PNR);

            if (!string.IsNullOrEmpty(disempowerment.RelationPNR))
                gapa.RelationPnr = decimal.Parse(disempowerment.RelationPNR);

            gapa.RelationType = disempowerment.GuardianRelationType;
            gapa.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(disempowerment.Registration.RegistrationDate, 12);

            if (disempowerment.DisempowermentStartDate.HasValue)
                gapa.StartDate = disempowerment.DisempowermentStartDate.Value;

            if (disempowerment.DisempowermentEndDate.HasValue)
                gapa.EndDate = disempowerment.DisempowermentEndDate.Value;

            gapa.AuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod
            return gapa;
        }

        public static GuardianAddress ToDprAddress(this DisempowermentType disempowerment)
        {
            GuardianAddress ga = new GuardianAddress();
            ga.PNR = decimal.Parse(disempowerment.PNR);
            ga.Address = disempowerment.GuardianName;
            ga.RelationType = disempowerment.GuardianRelationType;
            ga.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(disempowerment.Registration.RegistrationDate, 12);
            ga.AddressLine1 = disempowerment.RelationText1;
            ga.AddressLine2 = disempowerment.RelationText2;
            ga.AddressLine3 = disempowerment.RelationText3;
            ga.AddressLine4 = disempowerment.RelationText4;
            ga.AddressLine5 = disempowerment.RelationText5;

            // TODO: Sample PNR 709614126 has start date equal to 1/1/1 !!!
            if (disempowerment.GuardianAddressStartDate.HasValue)
                ga.StartDate = disempowerment.GuardianAddressStartDate.Value;

            ga.EndDate = disempowerment.DisempowermentEndDate;
            ga.AuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod
            return ga;
        }

        public static Street ToDprStreet(this StreetType s)
        {
            Street st = new Street();
            st.KOMKOD = s.MunicipalityCode;
            st.SVEJADRNVN = s.StreetAddressingName.ToUpper();
            st.VEJKOD = s.StreetCode;
            st.VEJADNVN = s.StreetAddressingName;
            return st;
        }

        public static City ToDprCity(this CityType city)
        {
            City c = new City();
            c.AJFDTO = city.Timestamp;
            c.BYNVN = city.CityName;
            c.HUSNRFRA = city.HouseNumberFrom;
            c.HUSNRTIL = city.HouseNumberTo;
            c.KOMKOD = city.MunicipalityCode;
            c.LIGEULIGE = city.EvenOrOdd;
            c.VEJKOD = city.StreetCode;
            return c;
        }

        public static PostDistrict ToDprPostDistrict(this PostDistrictType pd)
        {
            PostDistrict p = new PostDistrict();
            p.AJFDTO = pd.Timestamp;
            p.DISTTXT = pd.PostDistrictText;
            p.HUSNRFRA = pd.HouseNumberFrom;
            p.HUSNRTIL = pd.HouseNumberTo;
            p.KOMKOD = pd.MunicipalityCode;
            p.LIGEULIGE = pd.EvenOrOdd;
            p.VEJKOD = pd.StreetCode;
            p.POSTNR = pd.PostNumber;
            return p;
        }

        public static AreaRestorationDistrict ToDprAreaRestorationDistrict(this AreaRestorationDistrictType ardt)
        {
            AreaRestorationDistrict a = new AreaRestorationDistrict();
            a.AJFDTO = ardt.Timestamp;
            a.BYFORNYKOD = ardt.AreaRestorationCode;
            a.DISTTXT = ardt.DistrictText;
            a.HUSNRFRA = ardt.HouseNumberFrom;
            a.HUSNRTIL = ardt.HouseNumberTo;
            a.KOMKOD = ardt.MunicipalityCode;
            a.LIGEULIGE = ardt.EvenOrOdd;
            a.VEJKOD = ardt.StreetCode;
            return a;
        }

        public static DiverseDistrict ToDprDiverseDistrict(this DiverseDistrictType ddt)
        {
            DiverseDistrict d = new DiverseDistrict();
            d.AJFDTO = ddt.Timestamp;
            d.DISTTXT = ddt.DistrictText;
            d.DISTTYP = ddt.DistrictType;
            d.DIVDISTKOD = ddt.DivDistrictCode;
            d.HUSNRFRA = ddt.HouseNumberFrom;
            d.HUSNRTIL = ddt.HouseNumberTo;
            d.KOMKOD = ddt.MunicipalityCode;
            d.LIGEULIGE = ddt.EvenOrOdd;
            d.VEJKOD = ddt.StreetCode;
            return d;
        }

        public static EvacuationDistrict ToDprEvacuationDistrict(this EvacuationDistrictType edt)
        {
            EvacuationDistrict e = new EvacuationDistrict();
            e.AJFDTO = edt.Timestamp;
            e.DISTTXT = edt.DistrictText;
            e.EVAKUERKOD = edt.EvacuationCode;
            e.HUSNRFRA = edt.HouseNumberFrom;
            e.HUSNRTIL = edt.HouseNumberTo;
            e.KOMKOD = edt.MunicipalityCode;
            e.LIGEULIGE = edt.EvenOrOdd;
            e.VEJKOD = edt.StreetCode;
            return e;
        }

        public static ChurchDistrict ToDprChurchDistrict(this ChurchDistrictType cdt)
        {
            ChurchDistrict c = new ChurchDistrict();
            c.AJFDTO = cdt.Timestamp;
            c.DISTTXT = cdt.DistrictText;
            c.HUSNRFRA = cdt.HouseNumberFrom;
            c.HUSNRTIL = cdt.HouseNumberTo;
            c.KIRKEKOD = cdt.ChurchDistrictCode;
            c.KOMKOD = cdt.MunicipalityCode;
            c.LIGEULIGE = cdt.EvenOrOdd;
            c.VEJKOD = cdt.StreetCode;
            return c;
        }

        public static SchoolDistrict ToDprSchoolDistrict(this SchoolDistrictType sdt)
        {
            SchoolDistrict s = new SchoolDistrict();
            s.AJFDTO = sdt.Timestamp;
            s.DISTTXT = sdt.DistrictText;
            s.HUSNRFRA = sdt.HouseNumberFrom;
            s.HUSNRTIL = sdt.HouseNumberTo;
            s.KOMKOD = sdt.MunicipalityCode;
            s.LIGEULIGE = sdt.EvenOrOdd;
            s.SKOLEKOD = sdt.SchoolCode;
            s.VEJKOD = sdt.StreetCode;
            return s;
        }

        public static PopulationDistrict ToDprPopulationDistrict(this PopulationDistrictType pdt)
        {
            PopulationDistrict p = new PopulationDistrict();
            p.AJFDTO = pdt.Timestamp;
            p.BEFOLKKOD = pdt.PopulationDistrictCode;
            p.DISTTXT = pdt.DistrictText;
            p.HUSNRFRA = pdt.HouseNumberFrom;
            p.HUSNRTIL = pdt.HouseNumberTo;
            p.KOMKOD = pdt.MunicipalityCode;
            p.LIGEULIGE = pdt.EvenOrOdd;
            p.VEJKOD = pdt.StreetCode;
            return p;
        }

        public static SocialDistrict ToDprSocialDistrict(this SocialDistrictType sdt)
        {
            SocialDistrict s = new SocialDistrict();
            s.AJFDTO = sdt.Timestamp;
            s.DISTTXT = sdt.DistrictText;
            s.HUSNRFRA = sdt.HouseNumberFrom;
            s.HUSNRTIL = sdt.HouseNumberTo;
            s.KOMKOD = sdt.MunicipalityCode;
            s.LIGEULIGE = sdt.EvenOrOdd;
            s.SOCIALKOD = sdt.SocialCode;
            s.VEJKOD = sdt.StreetCode;
            return s;
        }

        public static ChurchAdministrationDistrict ToDprChurchAdministrationDistrict(this ChurchAdministrationDistrictType cadt)
        {
            ChurchAdministrationDistrict c = new ChurchAdministrationDistrict();
            c.AJFDTO = cadt.Timestamp;
            c.HUSNRFRA = cadt.HouseNumberFrom;
            c.HUSNRTIL = cadt.HouseNumberTo;
            c.KOMKOD = cadt.MunicipalityCode;
            c.LIGEULIGE = cadt.EvenOrOdd;
            c.MYNKOD = cadt.AuthorityAndChurchCode;
            c.VEJKOD = cadt.StreetCode;
            return c;
        }

        public static ElectionDistrict ToDprElectionDistrict(this ElectionDistrictType edt)
        {
            ElectionDistrict e = new ElectionDistrict();
            e.AJFDTO = edt.Timestamp;
            e.DISTTXT = edt.DistrictText;
            e.HUSNRFRA = edt.HouseNumberFrom;
            e.HUSNRTIL = edt.HouseNumberTo;
            e.KOMKOD = edt.MunicipalityCode;
            e.LIGEULIGE = edt.EvenOrOdd;
            e.VALGKOD = edt.ElectionCode;
            e.VEJKOD = edt.StreetCode;
            return e;
        }

        public static HeatingDistrict ToDprHeatingDistrict(this HeatingDistrictType hdt)
        {
            HeatingDistrict h = new HeatingDistrict();
            h.AJFDTO = hdt.Timestamp;
            h.DISTTXT = hdt.DistrictText;
            h.HUSNRFRA = hdt.HouseNumberFrom;
            h.HUSNRTIL = hdt.HouseNumberTo;
            h.KOMKOD = hdt.MunicipalityCode;
            h.LIGEULIGE = hdt.EvenOrOdd;
            h.VARMEKOD = hdt.HeatingDistrictCode;
            h.VEJKOD = hdt.StreetCode;
            return h;
        }

        public static PostNumber ToDprPostNumber(this PostNumberType pnt)
        {
            PostNumber p = new PostNumber();
            p.POSTNR = pnt.PostCode;
            p.POSTTXT = pnt.PostText;
            return p;
        }
    }
}
