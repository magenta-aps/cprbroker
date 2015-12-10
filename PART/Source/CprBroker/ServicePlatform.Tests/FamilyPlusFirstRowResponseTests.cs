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
using NUnit.Framework;
using CprBroker.Tests.CPRDirect;
using CprBroker.Schemas.Part;
using System.IO;
using CprBroker.Providers.ServicePlatform;
using CprBroker.Providers.ServicePlatform.Responses;
using System.Xml;

namespace CprBroker.Tests.ServicePlatform
{
    public class FamilyPlusFirstRowResponseTestBase : BaseResponseTests
    {
        public FamilyPlusFirstRowResponse GetResponse(string pnr)
        {
            var txt = GetResponse(pnr, "Familie+");
            var w = new FamilyPlusFirstRowResponse(txt);
            return w;
        }
    }

    [TestFixture]
    public class ToBirthDateTests : FamilyPlusFirstRowResponseTestBase
    {
        [Test]
        [TestCaseSource("PNRs")]
        public void ToNameStartDate_NotNull(string pnr)
        {
            var birthDate = GetResponse(pnr).MainItem.ToBirthdate();
            Assert.True(birthDate.HasValue);
        }
    }

    [TestFixture]
    public class ToNameStartDateTests : FamilyPlusFirstRowResponseTestBase
    {
        [Test]
        [TestCaseSource("PNRs")]
        public void ToNameStartDate_NotNull(string pnr)
        {
            var nameDate = GetResponse(pnr).MainItem.ToNameStartDate();
            Console.WriteLine(nameDate);

            if (nameDate.HasValue)
            {
                Assert.True(nameDate.HasValue);
            }
            else
            {
                var name3HasValue = false;
                name3HasValue = new FamilyPlusFirstRowResponse(GetResponse(pnr, ServiceInfo.NAVNE3_Local.Name)).MainItem.ToNameStartDate().HasValue;
                Assert.False(name3HasValue);
            }
        }
    }

}
