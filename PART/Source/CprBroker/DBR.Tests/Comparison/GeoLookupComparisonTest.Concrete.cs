using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Framework;
using CprBroker.Providers.DPR;

namespace CprBroker.Tests.DBR.Comparison
{

    [TestFixture]
    public class GeoLookupStreetComparisonTest : GeoLookupComparisonTest<Street>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.Streets.Select(s => s.VEJKOD.ToString()).ToArray();
            }
        }

        public override IQueryable<Street> Get(LookupDataContext dataContext, decimal key)
        {
            return dataContext.Streets.Where(s => s.VEJKOD == key).OrderBy(s => s.KOMKOD).ThenBy(s => s.VEJADNVN);
        }
    }

    [TestFixture]
    public class CityComparisonTests : GeoLookupComparisonTest<City>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.Cities.Select(c => c.BYNVN).ToArray();
            }
        }

        public override IQueryable<City> Get(LookupDataContext dataContext, decimal cn)
        {
            return dataContext.Cities.Where(c => c.BYNVN == cn.ToString()).OrderBy(c => c.KOMKOD).ThenBy(c => c.BYNVN);
        }
    }

    [TestFixture]
    public class AreaRestorationDistrictComparisonTests : GeoLookupComparisonTest<AreaRestorationDistrict>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.AreaRestorationDistricts.Select(a => a.BYFORNYKOD).ToArray();
            }
        }

        public override IQueryable<AreaRestorationDistrict> Get(LookupDataContext dataContext, decimal ardc)
        {
            return dataContext.AreaRestorationDistricts.Where(a => a.BYFORNYKOD == ardc.ToString()).OrderBy(a => a.KOMKOD).ThenBy(a => a.BYFORNYKOD);
        }
    }

    [TestFixture]
    public class DiverseDistrictComparisonTests : GeoLookupComparisonTest<DiverseDistrict>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.DiverseDistricts.Select(d => d.DIVDISTKOD).ToArray();
            }
        }

        public override IQueryable<DiverseDistrict> Get(LookupDataContext dataContext, decimal ddc)
        {
            return dataContext.DiverseDistricts.Where(d => d.DIVDISTKOD == ddc.ToString()).OrderBy(d => d.KOMKOD).ThenBy(d => d.DIVDISTKOD);
        }
    }

    [TestFixture]
    public class EvacuationDistrictComparisonTests : GeoLookupComparisonTest<EvacuationDistrict>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.EvacuationDistricts.Select(e => e.EVAKUERKOD.ToString()).ToArray();
            }
        }

        public override IQueryable<EvacuationDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.EvacuationDistricts.Where(e => e.EVAKUERKOD == edc).OrderBy(e => e.KOMKOD).ThenBy(e => e.EVAKUERKOD);
        }
    }

    [TestFixture]
    public class ChurchDistrictComparisonTests : GeoLookupComparisonTest<ChurchDistrict>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.ChurchDistricts.Select(c => c.KIRKEKOD.ToString()).ToArray();
            }
        }

        public override IQueryable<ChurchDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.ChurchDistricts.Where(c => c.KIRKEKOD == edc).OrderBy(c => c.KOMKOD).ThenBy(c => c.KIRKEKOD);
        }
    }

    [TestFixture]
    public class SchoolDistrictComparisonTests : GeoLookupComparisonTest<SchoolDistrict>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.SchoolDistricts.Select(s => s.SKOLEKOD.ToString()).ToArray();
            }
        }

        public override IQueryable<SchoolDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.SchoolDistricts.Where(s => s.SKOLEKOD == edc).OrderBy(s => s.KOMKOD).OrderBy(s => s.SKOLEKOD);
        }
    }

    [TestFixture]
    public class PopulationDistrictComparisonTests : GeoLookupComparisonTest<PopulationDistrict>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.PopulationDistricts.Select(p => p.BEFOLKKOD).ToArray();
            }
        }

        public override IQueryable<PopulationDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.PopulationDistricts.Where(p => p.BEFOLKKOD == edc.ToString()).OrderBy(p => p.KOMKOD).ThenBy(p => p.BEFOLKKOD);
        }
    }

    [TestFixture]
    public class SocialDistrictComparisonTests : GeoLookupComparisonTest<SocialDistrict>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.SocialDistricts.Select(s => s.SOCIALKOD.ToString()).ToArray();
            }
        }

        public override IQueryable<SocialDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.SocialDistricts.Where(s => s.SOCIALKOD == edc).OrderBy(s => s.KOMKOD).ThenBy(s => s.SOCIALKOD);
        }
    }

    [TestFixture]
    public class ChurchAdministrationDistrictComparisonTests : GeoLookupComparisonTest<ChurchAdministrationDistrict>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.ChurchAdministrationDistricts.Select(c => c.MYNKOD.ToString()).ToArray();
            }
        }

        public override IQueryable<ChurchAdministrationDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.ChurchAdministrationDistricts.Where(c => c.MYNKOD == edc).OrderBy(c => c.KOMKOD).ThenBy(c => c.MYNKOD);
        }
    }

    [TestFixture]
    public class ElectionDistrictComparisonTests : GeoLookupComparisonTest<ElectionDistrict>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.ElectionDistricts.Select(e => e.VALGKOD.ToString()).ToArray();
            }
        }

        public override IQueryable<ElectionDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.ElectionDistricts.Where(e => e.VALGKOD == edc).OrderBy(e => e.KOMKOD).ThenBy(e => e.VALGKOD);
        }
    }

    [TestFixture]
    public class HeatingDistrictComparisonTests : GeoLookupComparisonTest<HeatingDistrict>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.HeatingDistricts.Select(h => h.VARMEKOD.ToString()).ToArray();
            }
        }

        public override IQueryable<HeatingDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.HeatingDistricts.Where(h => h.VARMEKOD == edc).OrderBy(h => h.KOMKOD).ThenBy(h => h.VARMEKOD);
        }
    }

    [TestFixture]
    public class PostNumberComparisonTests : GeoLookupComparisonTest<PostNumber>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.PostNumbers.Select(p => p.POSTNR.ToString()).ToArray();
            }
        }

        public override IQueryable<PostNumber> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.PostNumbers.Where(p => p.POSTNR == edc).OrderBy(p => p.POSTNR);
        }
    }
}
