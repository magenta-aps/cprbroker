using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Engine;
using CprBroker.Data.DataProviders;
using CprBroker.Tests.Engine.Stubs;
using System.Diagnostics;

namespace CprBroker.Tests.Engine
{
    [TestFixture]
    class FacadeMethodInfoTests
    {
        uint[] GuidLargeCounts = new uint[] {100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };

        [Test]
        public void GetDataProviderList_Valid_ExecutesFast(
            [ValueSource("GuidLargeCounts")]uint count)
        {
            var facade = new GuidFacade(count);
            Type t = typeof(GuidDataProvider);
            DataProvider[] dbProviders;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            using (var dataContext = new CprBroker.Data.DataProviders.DataProvidersDataContext())
            {
                dbProviders = (from prov in dataContext.DataProviders
                               where prov.IsEnabled
                               orderby prov.Ordinal
                               select prov).ToArray();
            }


            DataProvidersConfigurationSection section = DataProvidersConfigurationSection.GetCurrent();


            bool missingDataProvidersExist;
            var result = facade.CreateSubMethodRunStates(out missingDataProvidersExist);

            watch.Stop();


            var expectedDuration = TimeSpan.FromMilliseconds(1 * count);

            Console.WriteLine(string.Format("{0} ms for {1} entries - {2} ms/item", watch.Elapsed.TotalMilliseconds, count, watch.Elapsed.TotalMilliseconds / count));

            Assert.LessOrEqual(watch.Elapsed, expectedDuration);

        }
    }
}
