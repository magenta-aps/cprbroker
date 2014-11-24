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
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CprServices
{
    /// <summary>
    /// Data tuple representing a person that is returned from a search operation
    /// </summary>
    public class SearchPerson : IEquatable<SearchPerson>
    {
        public string PNR;
        public NavnStrukturType Name;
        public AdresseType Address;
        public DateTime Timestamp;
        public string SourceXml;

        public bool Equals(SearchPerson other)
        {
            return string.Equals(other.PNR, PNR);
        }

        public LaesResultatType ToLaesResultatType(Func<string, Guid> uuidGetter)
        {
            return new LaesResultatType()
            {
                Item = new FiltreretOejebliksbilledeType()
                {
                    AttributListe = new AttributListeType()
                    {
                        Egenskab = new EgenskabType[]{
                            new EgenskabType(){ NavnStruktur = Name}
                        },
                        RegisterOplysning = new RegisterOplysningType[]
                        {
                            new RegisterOplysningType() { Item = new CprBorgerType(){ 
                                PersonCivilRegistrationIdentifier = PNR,
                                FolkeregisterAdresse =  Address
                            }}
                        }
                    },
                    BrugervendtNoegleTekst = PNR,
                    RelationListe = null,
                    TilstandListe = null,
                    UUID = uuidGetter(PNR).ToString()
                }
            };
        }

        public RegistreringType1 ToRegistreringType1(Func<string, Guid> uuidGetter)
        {
            return new RegistreringType1()
            {

                AttributListe = new AttributListeType()
                {
                    Egenskab = new EgenskabType[]
                    {
                        new EgenskabType()
                        { 
                            NavnStruktur = Name, 
                            Virkning = VirkningType.Create(Timestamp,null), 
                            BirthDate = PartInterface.Strings.PersonNumberToDate(this.PNR).Value
                        }
                    },
                    RegisterOplysning = new RegisterOplysningType[]
                    {
                        new RegisterOplysningType() 
                        { 
                            Item = new CprBorgerType()
                            { 
                                PersonCivilRegistrationIdentifier = PNR,
                                FolkeregisterAdresse =  Address
                            },
                            Virkning = VirkningType.Create(Timestamp,null)
                        }
                    }
                },
                RelationListe = null,
                TilstandListe = null,
                Tidspunkt = TidspunktType.Create(DateTime.Now),
                LivscyklusKode = LivscyklusKodeType.Rettet,
                AktoerRef = UnikIdType.Create(Constants.ActroId),
                CommentText = null,
                SourceObjectsXml = this.SourceXml
            };
        }


    }
}
