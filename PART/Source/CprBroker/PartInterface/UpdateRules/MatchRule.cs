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
using CprBroker.Utilities;
using CprBroker.Schemas.Part;
using CprBroker.Data.Part;

namespace CprBroker.Engine.UpdateRules
{
    public abstract class MatchRule
    {

        /// <summary>
        /// Checks if current rule Updates <paramref name="existingReg"/> with the corresponding values from <paramref name="newReg"/>
        /// </summary>
        /// <param name="existingReg">PART registration object to be changed, usually obtained from local database and subject to the bug fix</param>
        /// <param name="newReg">Updated PART registration obtained from the data source, with the fix implemented</param>
        /// <returns>True </returns>
        public abstract bool UpdateXmlTypeIfPossible(RegistreringType1 existingReg, RegistreringType1 newReg);

        private readonly static MatchRule[] _AllRules = new MatchRule[] { 
            new AddressingNameMatchRule(), new SpecielVejkodeIndikatorMatchRule(), new CityNameMatchRule(),
            new ParentalAuthorityChildrenUpdateRule(), new ParentalAuthorityHoldersUpdateRule()
        };

        public static MatchRule[] AllRules()
        {
            return new List<MatchRule>(_AllRules).ToArray();
        }

        public static bool ApplyRules(PersonRegistration dbReg, RegistreringType1 newReg)
        {
            return ApplyRules(dbReg, newReg, AllRules());
        }

        public static bool ApplyRules(PersonRegistration dbReg, RegistreringType1 newReg, IEnumerable<MatchRule> matchRules)
        {
            // TODO: Clear the effect dates directly under the registration object before applying the rules, restore them after applying
            // These dates are calculated from other dates
            var existingReg = PersonRegistration.ToXmlType(dbReg);
            var appliedRules = new List<MatchRule>();

            // Attempt rule application to OIO objects
            foreach (var rule in matchRules)
            {
                if (rule.UpdateXmlTypeIfPossible(existingReg, newReg))
                {
                    appliedRules.Add(rule);
                }
            }

            if (appliedRules.Count > 0)
            {
                var existingXml = Strings.SerializeObject(existingReg);
                var newXml = Strings.SerializeObject(newReg);

                if (string.Equals(existingXml, newXml))
                {
                    dbReg.BrokerUpdateDate = DateTime.Now;
                    dbReg.SetContents(newReg);
                    return true;
                }
            }
            return false;
        }
    }
}
