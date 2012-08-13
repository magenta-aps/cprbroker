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

namespace CprBroker.Providers.CPRDirect
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MinMaxOccurs : Attribute
    {
        public MinMaxOccurs()
            : this(1, 1)
        { }
        public MinMaxOccurs(int min, int max)
        {
            if (min > max)
            {
                throw new ArgumentException(string.Format("Parameter min <{0}> must be less than or equal to max <{1}>"));
            }
            MinOccurs = min;
            MaxOccurs = max;
        }

        public int MinOccurs { get; set; }
        public int MaxOccurs { get; set; }

        public void ValidateSingleObject(Wrapper w)
        {
            var actualOccurs = (w == null) ? 0 : 1;
            ValidateCount(actualOccurs);
        }

        public void ValidateList(IList<Wrapper> w)
        {
            if (w.Where(a => a == null).Count() > 0)
            {
                throw new ArgumentNullException();
            }
            var actualOccurs = w.Count;
            ValidateCount(actualOccurs);
        }

        public void ValidateCount(int actualCount)
        {
            if (actualCount < MinOccurs || actualCount > MaxOccurs)
            {
                throw new ArgumentOutOfRangeException(
                    "Occurences",
                    actualCount,
                    string.Format("Should be between <{0}> and <{1}>", MinOccurs, MaxOccurs)
                    );
            }
        }
    }
}
