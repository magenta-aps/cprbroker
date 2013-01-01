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

namespace CprBroker.Tests.CPRDirect.Objects
{
    namespace CurrentNameInformationTests
    {
        [TestFixture]
        public class TestFlags
        {
            [Test]
            public void TestLines()
            {
                var lines = LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var wrappers = lines.Select(w => w.ToWrapper(Constants.DataObjectMap));
                var myWrappers = wrappers.Where(w => w is CurrentNameInformationType).Select(w => w as CurrentNameInformationType);
                var first = myWrappers.Select(w => w.FirstNameMarker).Distinct().ToArray();
                var middle = myWrappers.Select(w => w.MiddleNameMarker).Distinct().ToArray();
                var last = myWrappers.Select(w => w.LastNameMarker).Distinct().ToArray();
                var allText = myWrappers.Select(w => w.Contents).ToArray();
                var found = myWrappers.Where(w => w.MiddleNameMarker == '*').First();
            }
        }

        [TestFixture]
        public class ToNavnStrukturType
        {
            [Test]
            public void ToNavnStrukturType_FirstNameValueNoFlag_EmptyFirstName()
            {
                var name = new CurrentNameInformationType() { FirstName_s = "klasjlkdfakl", FirstNameMarker = '*' };
                var partName = name.ToNavnStrukturType();
                Assert.IsNullOrEmpty(partName.PersonNameStructure.PersonGivenName);
            }

            [Test]
            public void ToNavnStrukturType_FirstNameValueWFlag_CorrectEmptyFirstName(
                [Values("klasdjkl", "iqiiii")]string firstName)
            {
                var name = new CurrentNameInformationType() { FirstName_s = firstName, FirstNameMarker = ' ' };
                var partName = name.ToNavnStrukturType();
                Assert.AreEqual(firstName, partName.PersonNameStructure.PersonGivenName);
            }

            [Test]
            public void ToNavnStrukturType_AddressingName_Correct()
            {
                string strName = Guid.NewGuid().ToString().Substring(0, 34);
                var name = new CurrentNameInformationType() { AddressingName = strName };
                var partName = name.ToNavnStrukturType();
                Assert.AreEqual(strName, partName.PersonNameForAddressingName);
            }
        }
    }
}
