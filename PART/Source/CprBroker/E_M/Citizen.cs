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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.E_M
{
    public partial class Citizen
    {

        public RegistreringType1 ToRegistreringType1(DateTime effectDate, Func<string, Guid> cpr2uuidFunc)
        {
            var ret = new RegistreringType1()
            {
                AktoerRef = UnikIdType.Create(E_MDataProvider.ActorId),
                AttributListe = this.ToAttributListeType(effectDate),
                CommentText = null,
                LivscyklusKode = LivscyklusKodeType.Rettet,
                RelationListe = this.ToRelationListeType(cpr2uuidFunc),
                Tidspunkt = this.ToTidspunktType(),
                TilstandListe = this.ToTilstandListeType(),
                Virkning = null
            };
            ret.CalculateVirkning();
            return ret;
        }

        public virtual AttributListeType ToAttributListeType(DateTime effectDate)
        {
            return new AttributListeType()
            {
                Egenskab = new EgenskabType[] { this.ToEgenskabType() },
                LokalUdvidelse = this.ToLokalUdvidelseType(),
                RegisterOplysning = new RegisterOplysningType[] { this.ToRegisterOplysningType(effectDate) },
                SundhedOplysning = new SundhedOplysningType[] { this.ToSundhedOplysningType() }
            };
        }

        public virtual EgenskabType ToEgenskabType()
        {
            var ret = new EgenskabType()
            {
                AndreAdresser = this.ToAndreAdresse(),
                BirthDate = this.ToBirthdate(),
                FoedestedNavn = Converters.ToNeutralString(this.BirthPlaceText),
                FoedselsregistreringMyndighedNavn = ToBirthRegistrationAuthority(),
                KontaktKanal = this.ToKontaktKanalType(),
                NaermestePaaroerende = this.ToNextOfKin(),
                NavnStruktur = this.ToNavnStrukturType(),
                PersonGenderCode = Converters.ToPersonGenderCodeType(this.Gender),
                Virkning = this.ToEgenskabTypeVirkning()
            };
            return ret;
        }

        public virtual VirkningType ToEgenskabTypeVirkning()
        {
            return VirkningType.Create(
               Converters.GetMaxDate(
                    this.ToBirthdate(),
                    Converters.ToDateTime(this.AddressingNameDate, this.AddressingNameDateUncertainty)
               ),
               null
           );
        }

        public virtual NavnStrukturType ToNavnStrukturType()
        {
            var addressingName = Converters.ToNeutralString(this.AddressingName);
            var ret = NavnStrukturType.Create(addressingName);
            ret.PersonNameForAddressingName = addressingName;
            return ret;
        }

        public virtual RegisterOplysningType ToRegisterOplysningType(DateTime effectDate)
        {
            return new RegisterOplysningType()
            {
                Item = this.ToCprBorgerType(effectDate),
                Virkning = this.ToCprBorgerTypeVirkning(),
            };
        }

        public CprBorgerType ToCprBorgerType(DateTime effectDate)
        {
            return new CprBorgerType()
            {
                // Not supported
                AdresseNoteTekst = null,
                // Set church membership
                FolkekirkeMedlemIndikator = this.ToChurchMembershipIndicator(),
                // Set address
                FolkeregisterAdresse = ToAdresseType(),
                // Set address indicator
                ForskerBeskyttelseIndikator = this.ToDirectoryProtectionIndicator(effectDate),
                // Set ddress protection indicator
                NavneAdresseBeskyttelseIndikator = this.ToAddressProtectionIndicator(effectDate),
                // Set CPR number
                PersonCivilRegistrationIdentifier = Converters.ToCprNumber(this.PNR),
                // Set nationality (always Danish in this case)
                PersonNationalityCode = this.ToCountryIdentificationCodeType(),
                // Set CPR number validity
                PersonNummerGyldighedStatusIndikator = this.ToCivilRegistrationValidityStatusIndicator(),
                // Telphone numbers are not supported
                TelefonNummerBeskyttelseIndikator = false
            };
        }

        public virtual AdresseType ToAdresseType()
        {
            if (this.CitizenReadyAddress != null)
            {
                return this.CitizenReadyAddress.ToAdresseType();
            }
            else
            {
                return null;
            }
        }

        public VirkningType ToCprBorgerTypeVirkning()
        {
            return VirkningType.Create(
               Converters.GetMaxDate(
                    Converters.ToDateTime(this.ChurchMarkerDate, this.ChurchMarkerDateUncertainty),
                    Converters.ToDateTime(this.MunicipalityArrivalDate, this.MunicipalityArrivalDateUncertainty),
                    Converters.ToDateTime(this.AddressEndTS, this.DepartureTimestampUncertainty),
                    Converters.ToDateTime(this.AddtessStartTS, this.RelocationTimestampUncertainty),
                    Converters.ToDateTime(this.DirectoryProtectionDate, this.DirectoryProtectionDateUncertainty),
                    Converters.ToDateTime(this.DirectoryProtectionEndDate, this.DirectoryProtectionEndDateUncertainty),
                    Converters.ToDateTime(this.AddressProtectionDate, this.AddressProtectionDateUncertainty),
                    Converters.ToDateTime(this.AddressProtectionEndDate, this.AddressProtectionEndDateUncertainty),
                    Converters.ToDateTime(this.NationalityChangeTimestamp, this.NationalityChangeTimestampUncertainty),
                    Converters.ToDateTime(this.NationalityTerminationTimestamp, this.NationalityTerminationTimestampUncertainty),
                    Converters.ToDateTime(this.PNRCreationDate, this.PNRCreationdateUncertainty)
               ),
               null
           );
        }

        public virtual TidspunktType ToTidspunktType()
        {
            return TidspunktType.Create(
                Converters.GetMaxDate(
                    Converters.ToDateTime(this.BirthRegistrationDate, this.BirthRegistrationDateUncertainty),
                    Converters.ToDateTime(this.CprChurchTimestamp),
                    Converters.ToDateTime(this.CprPersonTimestamp),
                    Converters.ToDateTime(this.PNRMarkingDate, this.PNRMarkingDateUncertainty),
                    Converters.ToDateTime(this.MunicipalityArrivalDate, MunicipalityArrivalDateUncertainty)
                )
            );
        }

    }
}
