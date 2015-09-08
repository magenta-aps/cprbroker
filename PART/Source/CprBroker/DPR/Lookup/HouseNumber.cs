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
using System.Text.RegularExpressions;

namespace CprBroker.Providers.DPR
{
    public class HouseNumber
    {
        public int? Number;
        public char? Letter;
        public char? EvenOddVal;

        public decimal? IntValue
        {
            get
            {
                if (Number.HasValue)
                {
                    var ret = Number.Value * 100;
                    if (Letter.HasValue)
                        ret += (int)Letter.Value;
                    return ret;
                }
                else
                {
                    return null;
                }
            }
        }

        public HouseNumber(string houseNumber)
        {
            houseNumber = string.Format("{0}", houseNumber).Trim();

            var pat = @"\A(?<num>\d+)(?<char>[a-zA-Z]*)\Z";
            var m = Regex.Match(houseNumber, pat);

            if (m.Success)
            {
                int num;
                if (int.TryParse(m.Groups["num"].Value, out num))
                {
                    Number = num;
                    EvenOddVal = EvenOdd.Even;
                    if (num % 2 == 1)
                        EvenOddVal = EvenOdd.Odd;
                }

                if (m.Groups["char"].Length > 0)
                    Letter = m.Groups["char"].Value[0];
            }
        }

        public bool Between(HouseNumber from, HouseNumber to, char evenOdd)
        {
            var fromVal = from.IntValue.HasValue ? from.IntValue.Value : 0;
            var toVal = to.IntValue.HasValue ? to.IntValue.Value : int.MaxValue;
            return
                true
                && this.Number.HasValue
                && this.EvenOddVal.Value.Equals(evenOdd)
                && this.IntValue >= fromVal && this.IntValue <= toVal
                ;
        }
    }
}
