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
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
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

using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using System;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;

namespace CprBroker.Providers.DPR
{
    /// <summary>
    /// Represents the DTTOTAL table
    /// </summary>
    public partial class PersonTotal
    {
        public DateTime? ToBirthdate()
        {
            // Attempt birthdate field
            var ret = Utilities.DateFromDecimal(this.DateOfBirth);

            // If failed, get it from PNR
            if (!ret.HasValue)
                ret = CprBroker.Utilities.Strings.PersonNumberToDate(this.PNR.ToPnrDecimalString());

            return ret;
        }

        public PersonInfo ToPersonInfo()
        {
            return new PersonInfo()
            {
                // Main object
                PersonTotal = this,

                // Get the latest active nationality (if possible)
                Nationality = this.Nationalities.Where(pn => pn.CorrectionMarker == null && pn.NationalityEndDate == null).OrderByDescending(pn => pn.NationalityStartDate).FirstOrDefault(),

                // Get the latest valid address (if possible)
                Address = this.PersonAddresses.Where(pa => pa.CorrectionMarker == null).OrderByDescending(pa => pa.AddressStartDate).FirstOrDefault(),

                // Get the current departure record (if possible)
                Departure = this.Departures.Where(d => d.EntryDate != null).FirstOrDefault(),

                // No need to filtrate by null NameTerminationDate because we are sorting by NameStartDate
                PersonName = this.PersonNames.Where(pn => pn.CorrectionMarker == null).OrderByDescending(pn => pn.NameStartDate).FirstOrDefault(),

                // No need to filtrate by null EndDate bacause this is done in PersonTotal.ToCivilStatusCodeType
                Separation = this.Separations.Where(s => s.CorrectionMarker == null).OrderByDescending(s => s.StartDate).FirstOrDefault(),

                // Get all valid (active and inactive) civil states
                CivilStates = this.CivilStatus.Where(civ => civ.CorrectionMarker == null).OrderBy(civ => civ.MaritalStatusDate).ToArray(),

                // Get All Children
                Children = this.Children.ToArray()
            };
        }

        public CprBorgerType ToCprBorgerType(Nationality dbNationality, PersonAddress dbAddress)
        {
            return new CprBorgerType()
            {
                // Address note - not supported
                AdresseNoteTekst = ToAdresseNoteTekst(),
                // Get address in separate method
                FolkeregisterAdresse = ToFolkeregisterAdresse(dbAddress),
                // Directory protection
                ForskerBeskyttelseIndikator = ToDirectoryProtectionIndicator(),
                // PNR
                PersonCivilRegistrationIdentifier = PNR.ToPnrDecimalString(),
                // Fill nationality
                PersonNationalityCode = dbNationality != null ? dbNationality.ToCountryIdentificationCodeType() : null,
                //PNR validity status
                PersonNummerGyldighedStatusIndikator = ToCivilRegistrationValidityStatusIndicator(),
                // Address protection
                NavneAdresseBeskyttelseIndikator = ToAddressProtectionIndicator(),
                // Church membership
                FolkekirkeMedlemIndikator = ToChurchMembershipIndicator(),
                //Use false since we do not have telephone numbers here
                TelefonNummerBeskyttelseIndikator = ToTelephoneNumberProtectionIndicator(),
            };
        }

        public AdresseType ToFolkeregisterAdresse(PersonAddress dbAddress)
        {
            // Fill address from PersonAddress table if possible
            return dbAddress != null ? dbAddress.ToAdresseType(this) : null;           
        }

        public IAddressSource GetFolkeregisterAdresseSource(PersonAddress address, Departure departure)
        {
            IAddressSource ret = null;

            switch ((PersonCivilRegistrationStatusCode)this.Status)
            {
                case PersonCivilRegistrationStatusCode.RegisteredWithResidenceInDanishPopulationRegister: // 1
                    // Get from PeronAddress
                    break;

                case PersonCivilRegistrationStatusCode.RegisteredWithHighStreetcodeInDanishPopulationRegister: // 2
                    // Case unknown - nobody in syddjurs
                    break;

                case PersonCivilRegistrationStatusCode.RegisteredWithResidenceInGreenlandicPopulationRegister: // 3
                    // Greenlandic Address from PersonAddress, however, address fields seem empty                    
                    break;

                case PersonCivilRegistrationStatusCode.RegisteredWithHighStreetcodeIn_GreenlandicPopulationRegister: // 7
                    // Greenlandic Address ????????
                    break;

                case PersonCivilRegistrationStatusCode.RegisteredWithoutResidenceInDanishOrGreenlandicPopulationRegisterAndAdministrativeCivilRegistrationNumbers: // 20
                    // No Address ????????
                    break;

                case PersonCivilRegistrationStatusCode.CancelledCivilRegistrationNumbers: // 30
                    // No Address ????????
                    break;

                case PersonCivilRegistrationStatusCode.DeletedCivilRegistrationNumbers://50
                    // No Address ????????
                    break;

                case PersonCivilRegistrationStatusCode.ChangedCivilRegistrationNumbers: // 60
                    // No Address ????????
                    break;

                case PersonCivilRegistrationStatusCode.Disappeared: // 70
                    // No Address ????????
                    break;

                case PersonCivilRegistrationStatusCode.Emigrated: // 80
                    // Get from departure (DTUDRIND)(could be null in all fields) - rely on ForeignAddressDate (ULADRDTO) to know if values exist
                    ret = departure;
                    break;

                case PersonCivilRegistrationStatusCode.dead: // 90
                    // No Address ????????
                    break;
            }
            if (ret == null)
            {
                ret = new DummyAddressSource();
            }
            return ret;
        }

        public VirkningType ToCprBorgerTypeVirkning(Nationality dbNationality, PersonAddress dbAddress)
        {
            List<decimal?> effects = new List<decimal?>();
            effects.AddRange(new decimal?[] { AddressDate, StatusDate });
            if (dbNationality != null)
            {
                effects.AddRange(new decimal?[] { dbNationality.NationalityStartDate, dbNationality.NationalityEndDate });
            }
            if (dbAddress != null)
            {
                effects.AddRange(new decimal?[] { dbAddress.AddressStartDate, dbAddress.CprUpdateDate, dbAddress.LeavingFromMunicipalityDate, dbAddress.MunicipalityArrivalDate });
            }
            return VirkningType.Create(Utilities.GetMaxDate(effects.ToArray()), null);
        }
    }
}