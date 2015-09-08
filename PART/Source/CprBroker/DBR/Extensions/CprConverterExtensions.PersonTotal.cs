/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Dennis Amdi Skov Isaksen
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

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

        public static PersonTotal7 ToPersonTotal(this IndividualResponseType resp, DPRDataContext dataContext)
        {
            /*
             * TODO: implement INDLAESDTO             * 
             */
            var pt = new PersonTotal7();
            /*
             * PERSON DETAILS
             */
            pt.DprLoadDate = resp.RegistrationDate;
            pt.PNR = Decimal.Parse(resp.PersonInformation.PNR);
            if (resp.PersonInformation.StatusStartDate != null)
            {
                pt.StatusDate = CprBroker.Utilities.Dates.DateToDecimal(resp.PersonInformation.StatusStartDate.Value, 12);
            }
            else
            {
                pt.StatusDate = null;//CprBroker.Utilities.Dates.DateToDecimal(resp.PersonInformation.StatusStartDate.Value, 12);
            }
            pt.Status = resp.PersonInformation.Status;

            if (resp.PersonInformation.Birthdate.HasValue)
                pt.DateOfBirth = CprBroker.Utilities.Dates.DateToDecimal(resp.PersonInformation.Birthdate.Value, 8);

            pt.Sex = resp.PersonInformation.Gender;

            /*
             * RESIDENTIAL DETAILS
             */


            // Zero initialization
            pt.MunicipalityArrivalDate = 0;
            pt.MunicipalityLeavingDate = null;

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
                            if (new string[] { "th", "tv", "mf" }.Contains(resp.CurrentAddressInformation.Door))
                                pt.Door = resp.CurrentAddressInformation.Door.PadLeft(4, ' ');
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

                        if (!string.IsNullOrEmpty(resp.CurrentAddressInformation.CareOfName))
                            pt.CareOfName = resp.CurrentAddressInformation.CareOfName;
                        else
                            pt.CareOfName = null;
                        if (!string.IsNullOrEmpty(resp.ClearWrittenAddress.CityName))
                            pt.CityName = resp.ClearWrittenAddress.CityName;
                        else
                            pt.CityName = null;
                        if (pt.MunicipalityCode > 0)
                            pt.CurrentMunicipalityName = CprBroker.Providers.CPRDirect.Authority.GetAuthorityNameByCode(pt.MunicipalityCode.ToString());
                    }
                }
            }

            // TODO: Get from protection records
            pt.AddressProtectionMarker = null; // TODO: Lookup in protection records //DPR SPECIFIC
            pt.DirectoryProtectionMarker = null; // TODO: Lookup in protection records //DPR SPECIFIC
            pt.AddressDateMarker = null; // TODO: Fill from address date marker //DPR SPECIFIC            

            pt.ChristianMark = resp.ChurchInformation.ChurchRelationship;
            if (!string.IsNullOrEmpty(resp.BirthRegistrationInformation.BirthRegistrationAuthorityCode))
                pt.BirthPlaceOfRegistration = Authority.GetAuthorityNameByCode(resp.BirthRegistrationInformation.BirthRegistrationAuthorityCode);
            else
                pt.BirthPlaceOfRegistration = null;

            pt.BirthplaceText = null; //TODO: Implement BirthplaceText

            pt.PnrMarkingDate = null; // Seems to be always null in DPR.

            pt.MotherPersonalOrBirthDate = resp.ParentsInformation.MotherPNR.Substring(0, 6) + "-" + resp.ParentsInformation.MotherPNR.Substring(6, 4);
            pt.MotherMarker = null; //DPR SPECIFIC
            pt.FatherPersonalOrBirthdate = resp.ParentsInformation.FatherPNR.Substring(0, 6) + "-" + resp.ParentsInformation.FatherPNR.Substring(6, 4);
            pt.FatherMarker = null; //DPR SPECIFIC
            if (resp.CurrentDepartureData != null && !resp.CurrentDepartureData.IsEmpty)
                pt.ExitEntryMarker = '1'; //DPR SPECIFIC
            pt.DisappearedMarker = null; //DPR SPECIFIC

            if (resp.Disempowerment != null && resp.Disempowerment.DisempowermentStartDate.HasValue)
            {
                pt.UnderGuardianshipDate = CprBroker.Utilities.Dates.DateToDecimal(resp.Disempowerment.DisempowermentStartDate.Value, 8);
            }
            else
            {
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

            pt.SpouseMarker = null; //TODO: Lookup in current civil status //DPR SPECIFIC
            pt.PostCode = resp.ClearWrittenAddress.PostCode;

            pt.PostDistrictName = resp.ClearWrittenAddress.PostDistrictText.NullIfEmpty();

            var voting = resp.ElectionInformation.OrderByDescending(e => e.ElectionInfoStartDate).FirstOrDefault();

            if (voting != null && voting.VotingDate.HasValue)
                pt.VotingDate = CprBroker.Utilities.Dates.DateToDecimal(voting.VotingDate.Value, 8);
            else
                pt.VotingDate = null;

            pt.ChildMarker = resp.Child.Count > 0 ?
                '1' : null as char?;

            if (resp.CurrentAddressInformation != null)
            {
                if (
                    !string.IsNullOrEmpty(resp.CurrentAddressInformation.SupplementaryAddress1) ||
                    !string.IsNullOrEmpty(resp.CurrentAddressInformation.SupplementaryAddress2) ||
                    !string.IsNullOrEmpty(resp.CurrentAddressInformation.SupplementaryAddress3) ||
                    !string.IsNullOrEmpty(resp.CurrentAddressInformation.SupplementaryAddress4) ||
                    !string.IsNullOrEmpty(resp.CurrentAddressInformation.SupplementaryAddress5)
                    )
                    pt.SupplementaryAddressMarker = '1'; //DPR SPECIFIC
            }
            pt.MunicipalRelationMarker = resp.MunicipalConditions.Count() > 0 ? '1' : null as char?;

            pt.NationalMemoMarker = null; // TODO: Should rely on DTNOTAT
            pt.FormerPersonalMarker = null; //DPR SPECIFIC
            pt.PaternityAuthorityName = null; //TODO: Retrieve this from the CPR Service field far_mynkod
            pt.MaritalAuthorityName = null; //TODO: Retrieve this from the CPR Service field mynkod
            if (!string.IsNullOrEmpty(resp.PersonInformation.Job))
                pt.Occupation = resp.PersonInformation.Job;
            else
                pt.Occupation = null;
            pt.NationalityRight = Authority.GetAuthorityNameByCode(resp.CurrentCitizenship.CountryCode.ToString());

            var previousAddresses = resp.HistoricalAddress
                .Where(e => (e as CprBroker.Schemas.Part.IHasCorrectionMarker).CorrectionMarker == CprBroker.Schemas.Part.CorrectionMarker.OK)
                .OrderByDescending(e => e.RelocationDate);

            var prevAddress = previousAddresses.FirstOrDefault();
            if (prevAddress != null)
            {
                var prevAdrStr = string.Format("{0} {1}",
                        Street.GetAddressingName(dataContext.Connection.ConnectionString, prevAddress.MunicipalityCode, prevAddress.StreetCode),
                        System.Text.RegularExpressions.Regex.Replace(
                            prevAddress.HouseNumber.TrimStart('0', ' '),
                            "(?<num>\\d+)(?<char>[a-zA-Z]+)",
                            "${num} ${char}"));

                var floorDoor = string.Format("{0} {1}",
                    prevAddress.Floor.TrimStart('0', ' '),
                    prevAddress.Door.TrimStart('0', ' '))
                 .Trim();

                if (!string.IsNullOrEmpty(floorDoor))
                    prevAdrStr += "," + floorDoor;

                var kom = Authority.GetAuthorityAddressByCode(prevAddress.MunicipalityCode.ToString());
                if (!string.IsNullOrEmpty(kom))
                    prevAdrStr += string.Format(" ({0})", kom);

                pt.PreviousAddress = prevAdrStr;

                if (string.IsNullOrEmpty(pt.CurrentMunicipalityName))
                    pt.CurrentMunicipalityName = Authority.GetAuthorityNameByCode(prevAddress.MunicipalityCode.ToString());

                pt.PreviousMunicipalityName = Authority.GetAuthorityNameByCode(prevAddress.MunicipalityCode.ToString());
                /*
                 * // Another algorithm
                var previousMunicipalityAddress = previousAddresses.Where(e => e.MunicipalityCode != resp.ClearWrittenAddress.MunicipalityCode).FirstOrDefault();
                if (previousMunicipalityAddress != null)
                {
                    pt.PreviousMunicipalityName = Authority.GetAuthorityNameByCode(previousMunicipalityAddress.MunicipalityCode.ToString());    
                }
                */
            }

            

            // In DPR SearchName contains both the first name and the middlename.
            pt.SearchName = ToDprFirstName(resp.CurrentNameInformation.FirstName_s, resp.CurrentNameInformation.MiddleName, true);
            if (!string.IsNullOrEmpty(resp.CurrentNameInformation.LastName))
                pt.SearchSurname = resp.CurrentNameInformation.LastName.ToUpper();
            else
                pt.SearchSurname = null;

            // Special logic for addressing name
            pt.AddressingName = ToDprAddressingName(resp.ClearWrittenAddress.AddressingName, resp.CurrentNameInformation.LastName);

            pt.StandardAddress = resp.ClearWrittenAddress.LabelledAddress.NullIfEmpty();
            pt.Location = resp.ClearWrittenAddress.Location.NullIfEmpty();

            pt.ContactAddressMarker = resp.ContactAddress == null ?
                null as char? : '1';

            pt.DprLoadDate = DateTime.Now;
            pt.ApplicationCode = null; // DPR Specific

            return pt;
        }

    }
}
