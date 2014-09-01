﻿using System;
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
            /*
             * TODO: implement INDLAESDTO             * 
             */
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
                        if (!string.IsNullOrEmpty(resp.CurrentAddressInformation.Floor))
                            pt.Floor = resp.CurrentAddressInformation.Floor;
                        else
                            pt.Floor = null;
                        if (!string.IsNullOrEmpty(resp.CurrentAddressInformation.Door))
                        {
                            if (resp.CurrentAddressInformation.Door.Equals("th") || resp.CurrentAddressInformation.Door.Equals("tv"))
                                pt.Door = "  " + resp.CurrentAddressInformation.Door;
                            else
                                pt.Door = resp.CurrentAddressInformation.Door;
                        }
                        else
                            pt.Door = null;
                        if (!string.IsNullOrEmpty(resp.CurrentAddressInformation.BuildingNumber))
                            pt.ConstructionNumber = resp.CurrentAddressInformation.BuildingNumber;
                        else
                            pt.ConstructionNumber = null;
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
                        if (!string.IsNullOrEmpty(resp.CurrentAddressInformation.CareOfName))
                            pt.CareOfName = resp.CurrentAddressInformation.CareOfName;
                        else
                            pt.CareOfName = null;
                        if (!string.IsNullOrEmpty(resp.ClearWrittenAddress.CityName))
                            pt.CityName = resp.ClearWrittenAddress.CityName;
                        else
                            pt.CityName = null;
                    }
                }
                /*
                 * TODO: FIX THE METHOD IN AUTHORITY!!!
                 */
                //pt.CurrentMunicipalityName = CprBroker.Providers.CPRDirect.Authority.GetNameByCode(pt.MunicipalityCode.ToString());
            }

            // TODO: Get from protection records
            pt.AddressProtectionMarker = null; //DPR SPECIFIC
            pt.DirectoryProtectionMarker = null; //DPR SPECIFIC
            pt.ArrivalDateMarker = null; //DPR SPECIFIC


            pt.ChristianMark = resp.ChurchInformation.ChurchRelationship;
            if (string.IsNullOrEmpty(resp.BirthRegistrationInformation.AdditionalBirthRegistrationText))
                pt.BirthPlaceOfRegistration = resp.BirthRegistrationInformation.AdditionalBirthRegistrationText; //TODO: validate whether this is correct...
            else
                pt.BirthPlaceOfRegistration = null;
            if (string.IsNullOrEmpty(resp.BirthRegistrationInformation.AdditionalBirthRegistrationText))
                pt.BirthplaceText = resp.BirthRegistrationInformation.AdditionalBirthRegistrationText; //TODO: validate whether this is correct...
            else
                pt.BirthplaceText = null;

            pt.PnrMarkingDate = null; // Seems to be always null in DPR.

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
                pt.PaternityDate = CprBroker.Utilities.Dates.DateToDecimal(resp.ParentsInformation.FatherDate.Value, 12);
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
            if (!string.IsNullOrEmpty(resp.PersonInformation.Job))
                pt.Occupation = resp.PersonInformation.Job;
            else
                pt.Occupation = null;
            pt.NationalityRight = null; // TODO: HOW DO WE OBTAIN THE COUNTRY NAME??
            /*
             * WE DON'T SET THE PreviousAddress FIELD, BECAUSE IT IS NOT USED, AT THE MOMENT, AND WILL TAKE SOME TIME TO IMPLEMENT.
            var prevAdr = resp.HistoricalAddress
                .Where(e => (e as CprBroker.Schemas.Part.IHasCorrectionMarker).CorrectionMarker == CprBroker.Schemas.Part.CorrectionMarker.OK)
                .OrderByDescending(e => e.ToStartTS())
                .FirstOrDefault();
            var prevMun = resp.HistoricalAddress.OrderByDescending(e => e.MunicipalityCode).FirstOrDefault();
            // TODO: Find municipality name in GeoLookup, based on mun. code
            pt.PreviousAddress = prevAdr.ToAddressCompleteType() + "(" + prevMun + ")";
            //pt.PreviousMunicipalityName = prevMun;
             */
            // In DPR SearchName contains both the first name and the middlename.
            if (!string.IsNullOrEmpty(resp.CurrentNameInformation.MiddleName))
                pt.SearchName = resp.CurrentNameInformation.FirstName_s.ToUpper() + " " + resp.CurrentNameInformation.MiddleName.ToUpper();
            else if (!string.IsNullOrEmpty(pt.SearchName = resp.CurrentNameInformation.FirstName_s))
                pt.SearchName = resp.CurrentNameInformation.FirstName_s.ToUpper();
            else
                pt.SearchName = null;
            pt.SearchSurname = resp.CurrentNameInformation.LastName.ToUpper();
            pt.AddressingName = resp.ClearWrittenAddress.AddressingName;
            pt.StandardAddress = resp.ClearWrittenAddress.LabelledAddress;
            if (!string.IsNullOrEmpty(resp.ClearWrittenAddress.Location))
                pt.Location = resp.ClearWrittenAddress.Location;
            else
                pt.Location = null;
            pt.ContactAddressMarker = null; //DPR SPECIFIC
            return pt;
        }
    }
}
