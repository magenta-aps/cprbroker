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
using CprBroker.DBR;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;
using System.Net;
using Moq;

namespace CprBroker.Tests.DBR
{
    namespace ClassicRequestTypeTests
    {
        public class ClassicRequestTypeTestsBase
        {
            public static string[] PNRs
            {
                get
                {
                    return CprBroker.Tests.CPRDirect.Utilities.PNRs.Take(10).ToArray();
                }
            }

            static ClassicRequestTypeTestsBase()
            {
                // Comparison tests already sets connection string in config
            }
        }

        public class CprDirectExtractDataProviderStub : CprBroker.Providers.CPRDirect.ICprDirectPersonDataProvider
        {

            public IndividualResponseType GetPerson(string cprNumber)
            {
                return ExtractManager.GetPerson(cprNumber);
            }

            public Schemas.Part.RegistreringType1 Read(Schemas.PersonIdentifier uuid, Schemas.Part.LaesInputType input, Func<string, Guid> cpr2uuidFunc, out Schemas.QualityLevel? ql)
            {
                throw new NotImplementedException();
            }

            public bool IsAlive()
            {
                throw new NotImplementedException();
            }

            public Version Version
            {
                get { throw new NotImplementedException(); }
            }
        }

        public class ClassicRequestTypeStub : ClassicRequestType
        {
            public override IEnumerable<T> LoadDataProviders<T>()
            {
                return new T[] { new CprDirectExtractDataProviderStub() as T };
            }
        }

        [TestFixture]
        public partial class Process : ClassicRequestTypeTestsBase
        {

            int DatabaseUpdateCalls = 0;

            [SetUp]
            public void Setup()
            {
                DatabaseUpdateCalls = 0;
            }


            public ClassicRequestType CreateRequest(string pnr)
            {
                var mock = new Mock<ClassicRequestType>();
                
                var req = mock.Object;
                
                req.Contents = new string(' ', 12);
                req.Type = Providers.DPR.InquiryType.DataUpdatedAutomaticallyFromCpr;
                req.PNR = pnr;
                req.LargeData = Providers.DPR.DetailType.ExtendedData;
                CPRDirectClientDataProvider prov = new CPRDirectClientDataProvider() { ConfigurationProperties = new Dictionary<string, string>(), DisableSubscriptions = false };
                mock.Setup(r => r.GetPerson(out prov)).Returns(
                    CprBroker.Tests.CPRDirect.Persons.Person.GetPerson(pnr)
                    );
                mock.Setup(r => r.UpdateDatabase(It.IsAny<IndividualResponseType>(), It.IsAny<string>()))
                    .Callback(() => 
                        DatabaseUpdateCalls++
                        );
                mock.Setup(r => r.Process(It.IsAny<string>())).Callback(
                () =>
                    Console.WriteLine("Here")
                    )
                .CallBase();
                //return mock;
                return req;
            }

            [Test]
            [TestCaseSource(typeof(ClassicRequestTypeTestsBase), "PNRs")]
            public void Process_NormalPerson_OK(string pnr)
            {
                var req = CreateRequest(pnr);
                var resp = req.Process("") as ClassicResponseType;
                Assert.AreEqual("00", resp.ErrorNumber);
            }

            [Test]
            [TestCaseSource(typeof(ClassicRequestTypeTestsBase), "PNRs")]
            public void Process_NormalPerson_UpdateDatabaseCalled(string pnr)
            {
                DatabaseUpdateCalls = 0;
                var req = CreateRequest(pnr);
                var resp = req.Process(Properties.Settings.Default.ImitatedDprConnectionString) as ClassicResponseType;
                Assert.AreEqual(1, DatabaseUpdateCalls);
            }
        }
    }
}
