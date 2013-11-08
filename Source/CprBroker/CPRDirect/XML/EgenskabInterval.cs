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
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class EgenskabInterval : Interval
    {
        // Name intervals are OK
        public INameSource Name
        {
            get { return this.GetData<INameSource>(); }
        }

        // Does not change by interval
        public IBasicInformation BasicInformation 
        { 
            get 
            {
                return (this.Name.Registration as IndividualResponseType).PersonInformation;
            }
        }

        // Does not change by interval
        public IBirthRegistrationInformation BirthRegistrationInformation 
        {
            get
            {
                return (this.Name.Registration as IndividualResponseType).BirthRegistrationInformation;
            }
        }

        public EgenskabType ToEgenskabType()
        {
            return new EgenskabType()
            {
                AndreAdresser = ToAndreAdresser(),
                BirthDate = ToBirthDate(),
                FoedestedNavn = ToFoedestedNavn(),
                FoedselsregistreringMyndighedNavn = ToFoedselsregistreringMyndighedNavn(),
                KontaktKanal = ToKontaktKanalType(),
                NaermestePaaroerende = ToNaermestePaaroerende(),
                // Cross Product
                NavnStruktur = ToNavnStrukturType(),
                PersonGenderCode = ToPersonGenderCodeType(),
                Virkning = this.ToVirkningType()
            };
        }

        public DateTime ToBirthDate()
        {
            return this.BasicInformation.ToBirthdate(true).Value;
        }

        public string ToFoedestedNavn()
        {
            // TODO: Change this to birthplace            
            return null;
        }

        public string ToFoedselsregistreringMyndighedNavn()
        {
            return this.BirthRegistrationInformation.ToFoedselsregistreringMyndighedNavn();
        }

        public NavnStrukturType ToNavnStrukturType()
        {
            return this.Name.ToNavnStrukturType();
        }

        public PersonGenderCodeType ToPersonGenderCodeType()
        {
            return this.BasicInformation.ToPersonGenderCodeType();
        }

        public static DataTypeTags[] Tags
        {
            get { return new DataTypeTags[] { DataTypeTags.Name }; }
        }
    }
}
