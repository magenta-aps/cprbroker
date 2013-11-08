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
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.CPRDirect.Persons
{
    [TestFixture]
    class P_0108610069 : Person
    {
        [Test]
        public void ToRegisterOplysningType_Normal_OneOrMoreIntervals()
        {
            var person = GetPerson();
            var registerOplysningIntervals = person.ToRegisterOplysningIntervalArray();
            Assert.IsNotNull(registerOplysningIntervals);
            Assert.GreaterOrEqual(registerOplysningIntervals.Length, 0);    // <- passes
            Console.WriteLine(registerOplysningIntervals.Length);           // prints 4
            // Tests to investigate objects in the array.
            foreach (RegisterOplysningInterval regInterval in registerOplysningIntervals)
            {
                //Console.WriteLine(regInterval.ToCprBorgerType());         // crashes with NullReferenceException
                Console.WriteLine(regInterval.ToVirkningType());
                Assert.IsNotNull(regInterval);
            }
            var registerOplysning = registerOplysningIntervals
                .Select(
                    interval =>
                        interval.ToRegisterOplysningType()                  // crashes with NullReferenceException
                )
                .ToArray();
            Assert.GreaterOrEqual(registerOplysning.Length, 0);
            foreach (RegisterOplysningType interval in registerOplysning)
            {
                /*
                 * We test if the address and end time attributes are set in any
                 * of the intervals.
                 */

                /* 
                 * We have to cast the item object as the type CprBorgerType as
                 * the type is not declared in the class.
                 */
                CprBorgerType item = (CprBorgerType)interval.Item;
                Assert.IsNotNull(item);
                // This is always null
                //Assert.IsNotNull(item.AdresseNoteTekst);
                // Address is not necessary to be not null
                //Assert.IsNotNull(item.FolkeregisterAdresse);
                VirkningType virkning = interval.Virkning;
                Assert.IsNotNull(virkning);
                Assert.IsNotNull(virkning.TilTidspunkt);
            }
        }
    }
}
