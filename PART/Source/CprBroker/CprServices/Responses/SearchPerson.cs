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
using System.Xml;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;

namespace CprBroker.Providers.CprServices.Responses
{
    /// <summary>
    /// Data tuple representing a person that is returned from a search operation
    /// </summary>
    public partial class SearchPerson : Responses.RowItem, IEquatable<SearchPerson>
    {
        public bool Equals(SearchPerson other)
        {
            return string.Equals(other.ToPnr(), this.ToPnr());
        }

        public SearchPerson(XmlElement elm, XmlNamespaceManager nsMgr)
            : base(elm, nsMgr)
        {

        }

        public string ToPnr()
        {
            return GetFieldValue(_Node, "PNR");
        }

        public string ToSourceXml()
        {
            return this._Node.OuterXml;
        }

        public DateTime? ToStartDate()
        {
            DateTime ret;
            if (DateTime.TryParseExact(
                GetFieldValue(_Node, "STARTDATO"),
                "yyyyMMddHHmm", // 198112131338                    
                null,
                 System.Globalization.DateTimeStyles.None,
                 out ret))
                return ret;
            else
                return null;
        }

        public virtual string ToNameString()
        {
            return CprBroker.Utilities.Strings.FirstNonEmpty(
                GetFieldValue(_Node, "CNVN_ADRNVN"),
                GetFieldValue(_Node, "ADRNVN")
                );
        }

        public NavnStrukturType ToNavnStrukturType()
        {
            var name = this.ToNameString();
            if (!string.IsNullOrEmpty(name))
                return NavnStrukturType.Create(new string[] { name }, name);
            else
                return null;
        }

        public bool NameMatches(params NavnStrukturType[] navnStrukturTypes)
        {
            return Array.TrueForAll<NavnStrukturType>(
                navnStrukturTypes,
                navnStrukturType =>
                {
                    if (navnStrukturType != null && navnStrukturType.PersonNameStructure != null && !navnStrukturType.PersonNameStructure.IsEmpty)
                    {
                        var nameFromSearch = this.ToNameString().ToLower();
                        var namePartsArray = navnStrukturType.PersonNameStructure.ToArray();
                        return Array.TrueForAll<string>(namePartsArray, namePart => nameFromSearch.Contains(namePart.ToLower()));
                    }
                    else
                    {
                        return true;
                    }
                }
            );
        }

        public LaesResultatType ToLaesResultatType(Func<string, Guid> uuidGetter, SoegInputType1 soegInput)
        {
            var ret = new LaesResultatType()
            {
                Item = new FiltreretOejebliksbilledeType()
                {
                    AttributListe = new AttributListeType()
                    {
                        Egenskab = new EgenskabType[]{
                            new EgenskabType(){ NavnStruktur = ToNavnStrukturType()}
                        },
                        RegisterOplysning = new RegisterOplysningType[]
                        {
                            new RegisterOplysningType() { Item = new CprBorgerType(){ 
                                PersonCivilRegistrationIdentifier = ToPnr(),
                                FolkeregisterAdresse =  ToAdresseType(),
                            }}
                        }
                    },
                    BrugervendtNoegleTekst = ToPnr(),
                    RelationListe = null,
                    TilstandListe = this.ToTilstandListeType(),
                    UUID = uuidGetter(ToPnr()).ToString()
                }
            };

            if (soegInput != null && soegInput.SoegObjekt != null && soegInput.SoegObjekt.SoegAttributListe != null && soegInput.SoegObjekt.SoegAttributListe.SoegRegisterOplysning != null)
            {
                CprBroker.Utilities.Reflection.CopyMissingDetailStrings(
                    typeof(RegisterOplysningType),
                    soegInput.SoegObjekt.SoegAttributListe.SoegRegisterOplysning.FirstOrDefault(),
                    (ret.Item as FiltreretOejebliksbilledeType).AttributListe.RegisterOplysning.FirstOrDefault()
                    );
            }
            return ret;
        }

        public AttributListeType ToAttributListeType()
        {
            return new AttributListeType()
            {
                Egenskab = new EgenskabType[]
                {
                    new EgenskabType()
                    { 
                        NavnStruktur = ToNavnStrukturType(), 
                        Virkning = VirkningType.Create(ToStartDate(),null), 
                        BirthDate = ToBirthdate().Value,
                        AndreAdresser = null,
                        FoedestedNavn = null,
                        FoedselsregistreringMyndighedNavn = null,
                        KontaktKanal = null,
                        NaermestePaaroerende = null, 
                        PersonGenderCode = default(PersonGenderCodeType) // TODO: Should be in stam+
                    }
                },
                RegisterOplysning = new RegisterOplysningType[]
                {
                    new RegisterOplysningType() 
                    { 
                        Item = new CprBorgerType()
                        { 
                            PersonCivilRegistrationIdentifier = ToPnr(),
                            FolkeregisterAdresse =  ToAdresseType(), 

                            // Assumed values
                            PersonNummerGyldighedStatusIndikator = true,

                            // Unsupported
                            AdresseNoteTekst = null,
                            FolkekirkeMedlemIndikator = false,
                            ForskerBeskyttelseIndikator = false,
                            NavneAdresseBeskyttelseIndikator = false,
                            PersonNationalityCode = null,
                            TelefonNummerBeskyttelseIndikator = false
                        },
                        Virkning = VirkningType.Create(ToStartDate(),null)
                    }
                },
                // Unsupported
                LokalUdvidelse = null,
                SundhedOplysning = null
            };
        }

        public DateTime? ToBirthdate()
        {
            // TODO: Get birthdate from rrelevant field
            return PartInterface.Strings.PersonNumberToDate(this.ToPnr()).Value;
        }

        public RegistreringType1 ToRegistreringType1()
        {
            return new RegistreringType1()
            {
                AttributListe = ToAttributListeType(),
                RelationListe = null,
                TilstandListe = null,
                // TODO: Is this correct registration time?
                Tidspunkt = TidspunktType.Create(DateTime.Now),
                LivscyklusKode = LivscyklusKodeType.Rettet,
                AktoerRef = UnikIdType.Create(Constants.ActorId),
                CommentText = null,
                SourceObjectsXml = ToSourceXml()
            };
        }
    }
}
