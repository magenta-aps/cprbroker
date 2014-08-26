using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Framework;
using CprBroker.Providers.DPR;

namespace CprBroker.Tests.DBR.Comparison.Geo
{

    [TestFixture]
    public class StreetComparisonTest : GeoLookupComparisonTest<Street>
    {
        public override string GetKey(Street obj)
        {
            return obj.VEJKOD.ToString();
        }

        public override IQueryable<Street> Get(LookupDataContext dataContext, decimal key)
        {
            return dataContext.Streets.Where(s => s.VEJKOD == key).OrderBy(s => s.KOMKOD).ThenBy(s => s.VEJADNVN);
        }
    }

    [TestFixture]
    public class CityComparisonTests : GeoLookupComparisonTest<City>
    {
        public override string GetKey(City obj)
        {
            return obj.BYNVN;
        }

        public override IQueryable<City> Get(LookupDataContext dataContext, decimal cn)
        {
            return dataContext.Cities.Where(c => c.BYNVN == cn.ToString()).OrderBy(c => c.KOMKOD).ThenBy(c => c.BYNVN);
        }
    }

    [TestFixture]
    public class AreaRestorationDistrictComparisonTests : GeoLookupComparisonTest<AreaRestorationDistrict>
    {
        public override string GetKey(AreaRestorationDistrict obj)
        {
            return obj.BYFORNYKOD;
        }

        public override IQueryable<AreaRestorationDistrict> Get(LookupDataContext dataContext, decimal ardc)
        {
            return dataContext.AreaRestorationDistricts.Where(a => a.BYFORNYKOD == ardc.ToString()).OrderBy(a => a.KOMKOD).ThenBy(a => a.BYFORNYKOD);
        }
    }

    [TestFixture]
    public class DiverseDistrictComparisonTests : GeoLookupComparisonTest<DiverseDistrict>
    {
        public override string GetKey(DiverseDistrict obj)
        {
            return obj.DIVDISTKOD;
        }

        public override IQueryable<DiverseDistrict> Get(LookupDataContext dataContext, decimal ddc)
        {
            return dataContext.DiverseDistricts.Where(d => d.DIVDISTKOD == ddc.ToString()).OrderBy(d => d.KOMKOD).ThenBy(d => d.DIVDISTKOD);
        }
    }

    [TestFixture]
    public class EvacuationDistrictComparisonTests : GeoLookupComparisonTest<EvacuationDistrict>
    {
        public override string GetKey(EvacuationDistrict obj)
        {
            return obj.EVAKUERKOD.ToString();
        }

        public override IQueryable<EvacuationDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.EvacuationDistricts.Where(e => e.EVAKUERKOD == edc).OrderBy(e => e.KOMKOD).ThenBy(e => e.EVAKUERKOD);
        }
    }

    [TestFixture]
    public class ChurchDistrictComparisonTests : GeoLookupComparisonTest<ChurchDistrict>
    {
        public override string GetKey(ChurchDistrict obj)
        {
            return obj.KIRKEKOD.ToString();
        }

        public override IQueryable<ChurchDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.ChurchDistricts.Where(c => c.KIRKEKOD == edc).OrderBy(c => c.KOMKOD).ThenBy(c => c.KIRKEKOD);
        }
    }

    [TestFixture]
    public class SchoolDistrictComparisonTests : GeoLookupComparisonTest<SchoolDistrict>
    {
        public override string GetKey(SchoolDistrict obj)
        {
            return obj.SKOLEKOD.ToString();
        }

        public override IQueryable<SchoolDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.SchoolDistricts.Where(s => s.SKOLEKOD == edc).OrderBy(s => s.KOMKOD).OrderBy(s => s.SKOLEKOD);
        }
    }

    [TestFixture]
    public class PopulationDistrictComparisonTests : GeoLookupComparisonTest<PopulationDistrict>
    {
        public override string GetKey(PopulationDistrict obj)
        {
            return obj.BEFOLKKOD;
        }

        public override IQueryable<PopulationDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.PopulationDistricts.Where(p => p.BEFOLKKOD == edc.ToString()).OrderBy(p => p.KOMKOD).ThenBy(p => p.BEFOLKKOD);
        }
    }

    [TestFixture]
    public class SocialDistrictComparisonTests : GeoLookupComparisonTest<SocialDistrict>
    {
        public override string GetKey(SocialDistrict obj)
        {
            return obj.SOCIALKOD.ToString();
        }
        public override IQueryable<SocialDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.SocialDistricts.Where(s => s.SOCIALKOD == edc).OrderBy(s => s.KOMKOD).ThenBy(s => s.SOCIALKOD);
        }
    }

    [TestFixture]
    public class ChurchAdministrationDistrictComparisonTests : GeoLookupComparisonTest<ChurchAdministrationDistrict>
    {
        public override string GetKey(ChurchAdministrationDistrict obj)
        {
            return obj.MYNKOD.ToString();
        }

        public override IQueryable<ChurchAdministrationDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.ChurchAdministrationDistricts.Where(c => c.MYNKOD == edc).OrderBy(c => c.KOMKOD).ThenBy(c => c.MYNKOD);
        }
    }

    [TestFixture]
    public class ElectionDistrictComparisonTests : GeoLookupComparisonTest<ElectionDistrict>
    {
        public override string GetKey(ElectionDistrict obj)
        {
            return obj.VALGKOD.ToString();
        }

        public override IQueryable<ElectionDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.ElectionDistricts.Where(e => e.VALGKOD == edc).OrderBy(e => e.KOMKOD).ThenBy(e => e.VALGKOD);
        }
    }

    [TestFixture]
    public class HeatingDistrictComparisonTests : GeoLookupComparisonTest<HeatingDistrict>
    {
        public override string GetKey(HeatingDistrict obj)
        {
            return obj.VARMEKOD.ToString();
        }

        public override IQueryable<HeatingDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.HeatingDistricts.Where(h => h.VARMEKOD == edc).OrderBy(h => h.KOMKOD).ThenBy(h => h.VARMEKOD);
        }
    }

    [TestFixture]
    public class PostNumberComparisonTests : GeoLookupComparisonTest<PostNumber>
    {
        public override string GetKey(PostNumber obj)
        {
            return obj.POSTNR.ToString();
        }

        public override IQueryable<PostNumber> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.PostNumbers.Where(p => p.POSTNR == edc).OrderBy(p => p.POSTNR);
        }
    }

}
