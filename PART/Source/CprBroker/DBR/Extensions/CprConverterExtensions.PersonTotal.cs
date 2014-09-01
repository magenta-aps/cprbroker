using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;

namespace CprBroker.DBR.Extensions
{
    public partial class CprConverterExtensions
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

            // Special logic for addressing name
            if (!string.IsNullOrEmpty(resp.ClearWrittenAddress.AddressingName))
            {
                var lastNamePartCount = resp.CurrentNameInformation.LastName.Split(' ').Length;
                var addressingNameParts = resp.ClearWrittenAddress.AddressingName.Split(' ');
                var otherNamesPartCount = addressingNameParts.Length - lastNamePartCount;
                pt.AddressingName = string.Format("{0},{1}",
                    string.Join(" ", addressingNameParts.Skip(otherNamesPartCount).ToArray()),
                    string.Join(" ", addressingNameParts.Take(otherNamesPartCount).ToArray())
                );
            }

            pt.StandardAddress = resp.ClearWrittenAddress.LabelledAddress;
            pt.Location = resp.ClearWrittenAddress.Location;
            pt.ContactAddressMarker = null; //DPR SPECIFIC
            return pt;
        }
    }
}
