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
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class ParentalAuthorityType : IReversibleRelationship, IOverwritable, IParentalAuthority
    {
        public enum CustodyTypes
        {
            Mother = 3,
            Father = 4,
            OtherHolder1 = 5,
            OtherHolder2 = 6
        }

        public PersonRelationType ToPersonRelationType(ParentsInformationType parents, Func<string, Guid> cpr2uuidFunc)
        {
            // TODO: A few persons have custody with father/mother but there is no father(45)/mother(6) PNR
            // Is it possible to get a UUID from a person name for a person not in CPR?
            var type = (CustodyTypes)(int)this.RelationshipType;
            switch (type)
            {
                case CustodyTypes.Mother:
                    return parents.ToMother(cpr2uuidFunc).FirstOrDefault();

                case CustodyTypes.Father:
                    return parents.ToFather(cpr2uuidFunc).FirstOrDefault();

                default:
                    if (type == CustodyTypes.OtherHolder1 || type == CustodyTypes.OtherHolder2)
                    {
                        var relPnr = ToCustodyOwnerPnr();
                        if (!string.IsNullOrEmpty(relPnr))
                        {
                            return PersonRelationType.Create(
                                cpr2uuidFunc(relPnr),
                                this.CustodyStartDate,
                                this.CustodyEndDate);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        throw new ArgumentException(
                            string.Format("Invalied value <{0}>, must be between 0003 and 0006", this.RelationshipType),
                            "RelationshipType"
                        );
                    }
            }
        }

        public PersonRelationType ToPersonRelationType(Func<string, Guid> cpr2uuidFunc)
        {
            var registration = this.Registration as IndividualResponseType;
            return ToPersonRelationType(new ParentalAuthorityType[] { this }, registration.ParentsInformation, cpr2uuidFunc).FirstOrDefault();
        }

        public static PersonRelationType[] ToPersonRelationType(IList<ParentalAuthorityType> parentalAuthorities, ParentsInformationType parents, Func<string, Guid> cpr2uuidFunc)
        {
            return parentalAuthorities
                .Select(p => p.ToPersonRelationType(parents, cpr2uuidFunc))
                .Where(rel => rel != null)
                .ToArray();
        }

        public string ToCustodyOwnerPnr()
        {
            return Converters.ToPnrStringOrNull(this.RelationPNR);
        }

        public bool IsOverwrittenBy(ITimedType newObject)
        {
            var newParentalAutority = newObject as ParentalAuthorityType;
            return this.RelationshipType == newParentalAutority.RelationshipType
                && this.RelationPNR == newParentalAutority.RelationPNR;
        }

        public DataTypeTags Tag
        {
            get { return DataTypeTags.ParentalAuthority; }
        }

        public DateTime? ToStartTS()
        {
            return this.CustodyStartDate;
        }

        public bool ToStartTSCertainty()
        {
            return Converters.ToDateTimeUncertainty(CustodyStartDateUncertainty);
        }

        public DateTime? ToEndTS()
        {
            return this.CustodyEndDate;
        }

        public bool ToEndTSCertainty()
        {
            return true;
        }

    }
}
