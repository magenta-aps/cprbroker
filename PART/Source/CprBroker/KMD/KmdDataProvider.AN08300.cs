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
using CprBroker.Engine;
using CprBroker.Providers.KMD.WS_AN08300;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.KMD
{
    public partial class KmdDataProvider
    {
        /// <summary>
        /// Calls the AN08300 web service
        /// </summary>
        /// <param name="cprNumber"></param>
        /// <returns></returns>
        private WS_AN08300.ReplyPeople[] CallAN08300(PersonNameStructureType name, Schemas.Part.PersonGenderCodeType? gender)
        {
            // TODO: use correct service objects
            //var service = new CprBroker.Providers.KMD.WS_AN08300.WS_AN08300();
            //SetServiceUrl(service, ServiceTypes.AN08300);

            //service.userinfoValue = new userinfo()
            //{
            //    userid = DatabaseObject.UserName,
            //    password = DatabaseObject.Password
            //};

            SearchData searchData = new SearchData();
            if (!name.IsEmpty)
            {
                searchData.FirstName = name.PersonGivenName;
                searchData.LastName = name.ToMiddleAndLastNameString();                
            }
            if (gender.HasValue)
            {
                searchData.Gender = Utilities.FromPartGender(gender).ToString();
            }

            AN08300 param = new AN08300()
            {
                InputRecord = searchData.InnerObject
            };
            // TODO: use correct objects
            var svar = new WS_AN08300.SVAR();
            ValidateReturnCode(svar.RETURKODE.ToString(), svar.RETURTEKST);

            // TODO: use correct object
            var resp = new WS_AN08300.SVARPERSONER[]{};            

            return Array.ConvertAll<WS_AN08300.SVARPERSONER, WS_AN08300.ReplyPeople>(resp, (p)=>new WS_AN08300.ReplyPeople(p));
        }
    }
    namespace WS_AN08300
    {
        public class SearchData
        {
            public SOEGEDATA InnerObject = new SOEGEDATA();

            public string FirstName
            {
                get { return this.InnerObject.FORNAVN; }
                set { this.InnerObject.FORNAVN = value; }
            }

            public string LastName
            {
                get { return this.InnerObject.EFTERNAVN; }
                set { this.InnerObject.EFTERNAVN = value; }
            }

            public string FamilyName
            {
                get { return this.InnerObject.SLAEGTSNAVN; }
                set { this.InnerObject.SLAEGTSNAVN = value; }
            }

            public string MiddleName
            {
                get { return this.InnerObject.MELLEMNAVN; }
                set { this.InnerObject.MELLEMNAVN = value; }
            }

            public string SearchName
            {
                get { return this.InnerObject.SOEGENAVN; }
                set { this.InnerObject.SOEGENAVN = value; }
            }

            public string Status
            {
                get { return this.InnerObject.STATUS; }
                set { this.InnerObject.STATUS = value; }
            }

            public string Search
            {
                get { return this.InnerObject.SOEGNING; }
                set { this.InnerObject.SOEGNING = value; }
            }

            public string Gender
            {
                get { return this.InnerObject.KOEN; }
                set { this.InnerObject.KOEN = value; }
            }

            public string StreetName
            {
                get { return this.InnerObject.VEJNAVN; }
                set { this.InnerObject.VEJNAVN = value; }
            }

            public string DateFrom
            {
                get { return this.InnerObject.HISTFRA; }
                set { this.InnerObject.HISTFRA = value; }
            }
            public string DateTo
            {
                get { return this.InnerObject.HISTTIL; }
                set { this.InnerObject.HISTTIL = value; }
            }

            public string AgeFrom
            {
                get { return this.InnerObject.ALDERINT1; }
                set { this.InnerObject.ALDERINT1 = value; }
            }
            public string AgeTo
            {
                get { return this.InnerObject.ALDERINT2; }
                set { this.InnerObject.ALDERINT2 = value; }
            }

            public string YearsFrom
            {
                get { return this.InnerObject.AARINT1; }
                set { this.InnerObject.AARINT1 = value; }
            }
            public string YearsTo
            {
                get { return this.InnerObject.AARINT2; }
                set { this.InnerObject.AARINT2 = value; }
            }

            public string StreetCode
            {
                get { return this.InnerObject.VEJKODE; }
                set { this.InnerObject.VEJKODE = value; }
            }
        }

        public class ReplyPeople
        {
            public SVARPERSONER InnerObject;

            public ReplyPeople(SVARPERSONER innerObject)
            {
                InnerObject = innerObject;
            }

            public string PNR
            {
                get
                {
                    return this.InnerObject.PERSONNUMMER;
                }
            }

            public string AddressingName
            {
                get
                {
                    return this.InnerObject.ADRESSERINGSNAVN;
                }
            }

            public string Address
            {
                get
                {
                    return this.InnerObject.ADRESSE;
                }
            }

            public string LastPersonIncidentCode
            {
                get
                {
                    return this.InnerObject.BEMAERKNING;
                }
            }
        }
    }
}
