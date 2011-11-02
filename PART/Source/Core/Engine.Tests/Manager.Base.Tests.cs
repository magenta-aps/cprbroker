using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Engine;

namespace CprBroker.Tests.Engine
{
    [TestFixture]
    class ManagerTests
    {
        class GuidFacade : GenericFacadeMethodInfo<Guid[]>
        {
            public Guid[] InputGuids;

            private GuidFacade() { }
            public GuidFacade(uint count)
            {
                ApplicationToken = CprBroker.Utilities.Constants.BaseApplicationToken.ToString();

                InputGuids = new Guid[count];
                for (int i = 0; i < count; i++)
                {
                    InputGuids[i] = Guid.NewGuid();
                }
            }

            public override void Initialize()
            {
                SubMethodInfos = InputGuids.Select(id => new GuidMethodInfo() { Input = id }).ToArray();
            }

            public override Guid[] Aggregate(object[] results)
            {
                return results.Select(r => (Guid)r).ToArray();
            }

        }

        class GuidDataProvider : IDataProvider
        {
            public Guid AAAA(Guid guid)
            {
                return guid;
            }
            public Version Version { get { return new Version(1, 0); } }
            public bool IsAlive() { return true; }
        }

        class GuidMethodInfo : SubMethodInfo<GuidDataProvider, Guid>
        {
            public Guid Input;
            public GuidMethodInfo()
            {
                this.LocalDataProviderOption = LocalDataProviderUsageOption.UseFirst;
            }

            public override Guid RunMainMethod(GuidDataProvider prov)
            {
                System.Threading.Thread.Sleep(50);
                return prov.AAAA(Input);
            }
        }

        uint[] GuidCounts = new uint[] { 0, 1, 10, 50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };

        [Test]
        public void GetMethodOutput_Valid_OK(
            [ValueSource("GuidCounts")]uint count)
        {
            var facade = new GuidFacade(count);
            var result = Manager.GetMethodOutput<Guid[]>(facade);
            Assert.AreEqual("200", result.StandardRetur.StatusKode);
        }

        [Test]
        public void GetMethodOutput_Valid_CorrectLength(
            [ValueSource("GuidCounts")]uint count)
        {
            var facade = new GuidFacade(count);
            var result = Manager.GetMethodOutput<Guid[]>(facade);
            Assert.AreEqual(count, result.Item.Length);
        }

        [Test]
        public void GetMethodOutput_Valid_MatchingUuids(
            [ValueSource("GuidCounts")]uint count)
        {
            var facade = new GuidFacade(count);
            var result = Manager.GetMethodOutput<Guid[]>(facade);
            for (uint i = 0; i < count; i++)
            {
                Assert.AreEqual(facade.InputGuids[i], result.Item[i]);
            }
        }
    }
}
