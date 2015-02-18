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
    public static partial class CprConverterExtensions
    {
        public static GuardianAndParentalAuthorityRelation ToDpr(this DisempowermentType disempowerment)
        {
            if (disempowerment.GuardianshipType == DisempowermentType.GuardianshipTypes.ParentOrGuardianPnrFound)
            {
                GuardianAndParentalAuthorityRelation gapa = new GuardianAndParentalAuthorityRelation();
                gapa.PNR = decimal.Parse(disempowerment.PNR);

                if (!string.IsNullOrEmpty(disempowerment.RelationPNR))
                    gapa.RelationPnr = decimal.Parse(disempowerment.RelationPNR);

                gapa.RelationType = disempowerment.GuardianRelationType;
                gapa.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(disempowerment.Registration.RegistrationDate, 12);

                if (disempowerment.DisempowermentStartDate.HasValue)
                    gapa.StartDate = disempowerment.DisempowermentStartDate.Value;

                if (disempowerment.DisempowermentEndDate.HasValue)
                    gapa.EndDate = disempowerment.DisempowermentEndDate.Value;

                gapa.AuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod
                return gapa;
            }
            return null;
        }

        public static GuardianAddress ToDprAddress(this DisempowermentType disempowerment)
        {
            if (disempowerment.GuardianshipType == DisempowermentType.GuardianshipTypes.ParentOrGuardianAddressExists)
            {
                GuardianAddress ga = new GuardianAddress();
                ga.PNR = decimal.Parse(disempowerment.PNR);
                ga.Address = disempowerment.GuardianName;
                ga.RelationType = disempowerment.GuardianRelationType;
                ga.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(disempowerment.Registration.RegistrationDate, 12);
                ga.AddressLine1 = disempowerment.RelationText1;
                ga.AddressLine2 = disempowerment.RelationText2;
                ga.AddressLine3 = disempowerment.RelationText3;
                ga.AddressLine4 = disempowerment.RelationText4;
                ga.AddressLine5 = disempowerment.RelationText5;

                // TODO: Sample PNR 709614126 has start date equal to 1/1/1 !!!
                if (disempowerment.GuardianAddressStartDate.HasValue)
                    ga.StartDate = disempowerment.GuardianAddressStartDate.Value;

                ga.EndDate = disempowerment.DisempowermentEndDate;
                ga.AuthorityCode = 0; //TODO: Can be fetched in CPR Services, mynkod
                return ga;
            }
            return null;
        }
    }
}
