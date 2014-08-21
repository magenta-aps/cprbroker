using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Framework;
using CprBroker.Providers.DPR;

namespace CprBroker.Tests.DBR.Comparison
{

    public class GeoLookupStreetComparisonTest : GeoLookupComparisonTest<Street>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.Streets.Select(ei => ei.VEJKOD.ToString()).ToArray();
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
        public override IQueryable<City> Get(LookupDataContext dataContext, decimal cn)
        {
            return dataContext.Cities.Where(o => o.BYNVN == cn.ToString());
        }
    }

    [TestFixture]
    public class AreaRestorationDistrictComparisonTests : GeoLookupComparisonTest<AreaRestorationDistrict>
    {
        public override IQueryable<AreaRestorationDistrict> Get(LookupDataContext dataContext, decimal ardc)
        {
            return dataContext.AreaRestorationDistricts.Where(o => o.BYFORNYKOD == ardc.ToString());
        }
    }

    [TestFixture]
    public class DiverseDistrictComparisonTests : GeoLookupDiverseDistrictComparisonTest<DiverseDistrict>
    {
        public override IQueryable<DiverseDistrict> Get(LookupDataContext dataContext, decimal ddc)
        {
            return dataContext.DiverseDistricts.Where(o => o.DIVDISTKOD == ddc.ToString());
        }
    }

    [TestFixture]
    public class EvacuationDistrictComparisonTests : GeoLookupEvacuationDistrictComparisonTest<EvacuationDistrict>
    {
        public override IQueryable<EvacuationDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.EvacuationDistricts.Where(o => o.EVAKUERKOD == edc);
        }
    }

    [TestFixture]
    public class ChurchDistrictComparisonTests : GeoLookupChurchDistrictComparisonTest<ChurchDistrict>
    {
        public override IQueryable<ChurchDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.ChurchDistricts.Where(o => o.KIRKEKOD == edc);
        }
    }

    [TestFixture]
    public class SchoolDistrictComparisonTests : GeoLookupSchoolDistrictComparisonTest<SchoolDistrict>
    {
        public override IQueryable<SchoolDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.SchoolDistricts.Where(o => o.SKOLEKOD == edc);
        }
    }

    [TestFixture]
    public class PopulationDistrictComparisonTests : GeoLookupPopulationDistrictComparisonTest<PopulationDistrict>
    {
        public override IQueryable<PopulationDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.PopulationDistricts.Where(o => o.BEFOLKKOD == edc.ToString());
        }
    }

    [TestFixture]
    public class SocialDistrictComparisonTests : GeoLookupSocialDistrictComparisonTest<SocialDistrict>
    {
        public override IQueryable<SocialDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.SocialDistricts.Where(o => o.SOCIALKOD == edc);
        }
    }

    [TestFixture]
    public class ChurchAdministrationDistrictComparisonTests : GeoLookupChurchAdministrationDistrictComparisonTest<ChurchAdministrationDistrict>
    {
        public override IQueryable<ChurchAdministrationDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.ChurchAdministrationDistricts.Where(o => o.MYNKOD == edc);
        }
    }

    [TestFixture]
    public class ElectionDistrictComparisonTests : GeoLookupElectionDistrictComparisonTest<ElectionDistrict>
    {
        public override IQueryable<ElectionDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.ElectionDistricts.Where(o => o.VALGKOD == edc);
        }
    }

    [TestFixture]
    public class HeatingDistrictComparisonTests : GeoLookupHeatingDistrictComparisonTest<HeatingDistrict>
    {
        public override IQueryable<HeatingDistrict> Get(LookupDataContext dataContext, decimal edc)
        {
            return dataContext.HeatingDistricts.Where(o => o.VARMEKOD == edc);
        }
    }
}
