using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.DBR;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;
using System.Net;

namespace CprBroker.Tests.DBR
{
    namespace ClassicRequestTypeTests
    {
        public class ClassicRequestTypeTestsBase
        {
            public static string CprBrokerConnectionString = Comparison.ComparisonTest<object, ExtractDataContext>.CprBrokerConnectionString;
            public static string FakeDprDatabaseConnectionString = Comparison.ComparisonTest<object, ExtractDataContext>.FakeDprDatabaseConnectionString;

            public static string[] PNRs
            {
                get
                {
                    using (var dataContext = new ExtractDataContext())
                    {
                        return dataContext.ExtractItems.Select(ei => ei.PNR).Distinct().Take(10).ToArray();
                    }
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
            public override IEnumerable<ICprDirectPersonDataProvider> LoadDataProviders()
            {
                return new ICprDirectPersonDataProvider[] { new CprDirectExtractDataProviderStub() };
            }
        }

        [TestFixture]
        public class Process : ClassicRequestTypeTestsBase
        {
            public string address = "localhost";
            public int port = 999;



            public ClassicRequestType CreateRequest(string pnr)
            {
                var req = new ClassicRequestTypeStub();
                Console.WriteLine(req.Length);
                req.Contents = new string(' ', 12);
                req.Type = Providers.DPR.InquiryType.DataUpdatedAutomaticallyFromCpr;
                req.PNR = pnr;
                req.LargeData = Providers.DPR.DetailType.ExtendedData;
                return req;
            }

            [Test]
            [TestCaseSource(typeof(ClassicRequestTypeTestsBase), "PNRs")]
            public void Process_NormalPerson_OK(string pnr)
            {
                var req = CreateRequest(pnr);
                var resp = req.Process(FakeDprDatabaseConnectionString) as ClassicResponseType;
                Assert.AreEqual("00", resp.ErrorNumber);
            }

            [Test]
            [TestCaseSource(typeof(ClassicRequestTypeTestsBase), "PNRs")]
            public void Process_NormalPerson_GoesToDbr(string pnr)
            {
                using (var dataContext = new DPRDataContext(FakeDprDatabaseConnectionString))
                {
                    CprConverter.DeletePersonRecords(pnr, dataContext);
                }
                var req = CreateRequest(pnr);
                var resp = req.Process(FakeDprDatabaseConnectionString) as ClassicResponseType;
                using (var dataContext = new DPRDataContext(FakeDprDatabaseConnectionString))
                {
                    var t = dataContext.PersonTotals.Where(pt => pt.PNR == decimal.Parse(pnr)).FirstOrDefault();
                    Assert.NotNull(t);
                }
            }
        }
    }
}
