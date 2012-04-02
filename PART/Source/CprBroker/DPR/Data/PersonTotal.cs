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

namespace CprBroker.Providers.DPR
{
    /// <summary>
    /// Represents the DTTOTAL table
    /// </summary>
    public partial class PersonTotal
    {
        public CivilStatusKodeType PartCivilStatus
        {
            get
            {
                if (this.MaritalStatus.HasValue)
                {
                    switch (this.MaritalStatus)
                    {
                        case Constants.MaritalStatus.Unmarried:
                            return CivilStatusKodeType.Ugift;
                        case Constants.MaritalStatus.Married:
                            return CivilStatusKodeType.Gift;
                        case Constants.MaritalStatus.Divorced:
                            return CivilStatusKodeType.Skilt;
                        case Constants.MaritalStatus.Widow:
                            return CivilStatusKodeType.Enke;
                        case Constants.MaritalStatus.RegisteredPartnership:
                            return CivilStatusKodeType.RegistreretPartner;
                        case Constants.MaritalStatus.AbolitionOfRegisteredPartnership:
                            return CivilStatusKodeType.OphaevetPartnerskab;
                        case Constants.MaritalStatus.LongestLivingPartner:
                            return CivilStatusKodeType.Laengstlevende;
                        // TODO : GetPropertyValuesOfType fromDate latest marital status before this record
                        case Constants.MaritalStatus.Deceased:
                            return CivilStatusKodeType.Ugift;
                        // TODO: When to use CivilStatusKode.Separeret?
                    }
                }
                throw new NotSupportedException("Unknown marital status");
            }
        }

        public LivStatusKodeType PartLifeStatus
        {
            get
            {
                return Schemas.Util.Enums.ToLifeStatus(this.Status, Utilities.DateFromDecimal(DateOfBirth));
            }
        }

        public bool ForskerBeskyttelseIndikator()
        {
            return DirectoryProtectionMarker == '1';
        }

        public bool ToPersonNummerGyldighedStatusIndikator()
        {
            return Schemas.Util.Enums.IsActiveCivilRegistrationStatus(this.Status);
        }

        public bool ToNavneAdresseBeskyttelseIndikator()
        {
            return AddressProtectionMarker == '1';
        }

        public bool ToChurchMembershipIndicator()
        {
            return ChristianMark.HasValue;
        }

        public bool ToTelephoneNumberProtectionIndicator()
        {
            return false;
        }

        //TODO: Add logic to get value from DTBOERN if not found in DTTOTAL
        public static decimal? GetParent(char? parentMarker, string parentPnrOrBirthdate)
        {
            if (parentMarker.HasValue && parentMarker.Value == '*')
            {
                string parentPnr = parentPnrOrBirthdate.Trim().Replace("-", "");
                decimal ret;
                if (parentPnr.Length == 10 && decimal.TryParse(parentPnr, out ret) && ret > 0)
                {
                    return ret;
                }
            }
            return null;
        }

        public CprBorgerType ToCprBorgerType(Nationality dbNationality, PersonAddress dbAddress)
        {
            return new CprBorgerType()
            {
                AdresseNoteTekst = null,
                FolkeregisterAdresse = dbAddress != null ? dbAddress.ToAdresseType(this) : null,
                ForskerBeskyttelseIndikator = ForskerBeskyttelseIndikator(),
                PersonCivilRegistrationIdentifier = PNR.ToPnrDecimalString(),
                PersonNationalityCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, dbNationality.CountryCode.ToDecimalString()),

                //PNR validity status
                // TODO: Make sure that true is the correct value
                PersonNummerGyldighedStatusIndikator = ToPersonNummerGyldighedStatusIndikator(),
                // Address protection
                NavneAdresseBeskyttelseIndikator = ToNavneAdresseBeskyttelseIndikator(),
                // Church membership
                FolkekirkeMedlemIndikator = ToChurchMembershipIndicator(),
                //Use false since we do not have telephone numbers here
                // TODO: Check if this is correct
                TelefonNummerBeskyttelseIndikator = ToTelephoneNumberProtectionIndicator(),
            };
        }

        public VirkningType ToCprBorgerTypeVirkning(Nationality dbNationality, PersonAddress dbAddress)
        {
            List<decimal?> effects = new List<decimal?>();
            effects.AddRange(new decimal?[] { AddressDate, StatusDate, dbNationality.NationalityStartDate });
            if (dbAddress != null)
            {
                effects.AddRange(new decimal?[] { dbAddress.AddressStartDate, dbAddress.CprUpdateDate, dbAddress.LeavingFromMunicipalityDate, dbAddress.MunicipalityArrivalDate });
            }
            return VirkningType.Create(Utilities.GetMaxDate(effects.ToArray()), null);
        }

        public UkendtBorgerType ToUkendtBorgerType()
        {
            return new UkendtBorgerType()
            {
                PersonCivilRegistrationReplacementIdentifier = PNR.ToPnrDecimalString(),
            };
        }

        public VirkningType ToUkendtBorgerTypeVirkning()
        {
            return VirkningType.Create(Utilities.GetMaxDate(StatusDate), null);
        }

        public UdenlandskBorgerType ToUdenlandskBorgerType(Nationality dbNationality)
        {
            return new UdenlandskBorgerType()
            {
                // Birth country.Not in DPR
                FoedselslandKode = null,
                // TODO: What is that?
                PersonIdentifikator = "",
                // Languages. Not implemented here
                SprogKode = new CountryIdentificationCodeType[] { },
                // Citizenships
                PersonNationalityCode = new CountryIdentificationCodeType[] 
                { 
                    CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, dbNationality.CountryCode.ToDecimalString()) 
                },
                PersonCivilRegistrationReplacementIdentifier = PNR.ToPnrDecimalString(),
            };
        }

        public VirkningType ToUdenlandskBorgerTypeVirkning()
        {
            return VirkningType.Create(Utilities.GetMaxDate(StatusDate), null);
        }

    }
}