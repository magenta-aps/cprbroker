using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Framework;

namespace CprBroker.Tests.DBR.Comparison
{

    public abstract class GeoLookupCityComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.Cities.Select(ei => ei.BYNVN).Distinct().ToArray();
            }
        }

        public override IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public abstract IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, decimal pnr);

    }

    public abstract class GeoLookupAreaRestorationDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.AreaRestorationDistricts.Select(ei => ei.BYFORNYKOD).Distinct().ToArray();
            }
        }

        public override IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public abstract IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, decimal pnr);
    }

    public abstract class GeoLookupPostDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.PostDistricts.Select(ei => ei.POSTNR.ToString()).Distinct().ToArray();
            }
        }

        public override IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public abstract IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, decimal pnr);
    }

    public abstract class GeoLookupDiverseDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.DiverseDistricts.Select(ei => ei.DIVDISTKOD).Distinct().ToArray();
            }
        }

        public override IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public abstract IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, decimal pnr);
    }

    public abstract class GeoLookupEvacuationDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.EvacuationDistricts.Select(ei => ei.EVAKUERKOD.ToString()).Distinct().ToArray();
            }
        }

        public override IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public abstract IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, decimal pnr);
    }

    public abstract class GeoLookupChurchDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.ChurchDistricts.Select(ei => ei.KIRKEKOD.ToString()).Distinct().ToArray();
            }
        }

        public override IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public abstract IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, decimal pnr);
    }

    public abstract class GeoLookupSchoolDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.SchoolDistricts.Select(ei => ei.SKOLEKOD.ToString()).Distinct().ToArray();
            }
        }

        public override IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public abstract IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, decimal pnr);
    }

    public abstract class GeoLookupPopulationDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.PopulationDistricts.Select(ei => ei.BEFOLKKOD).Distinct().ToArray();
            }
        }

        public override IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public abstract IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, decimal pnr);
    }

    public abstract class GeoLookupSocialDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.SocialDistricts.Select(ei => ei.SOCIALKOD.ToString()).Distinct().ToArray();
            }
        }

        public override IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public abstract IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, decimal pnr);
    }

    public abstract class GeoLookupChurchAdministrationDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.ChurchAdministrationDistricts.Select(ei => ei.MYNKOD.ToString()).Distinct().ToArray();
            }
        }

        public override IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public abstract IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, decimal pnr);
    }

    public abstract class GeoLookupElectionDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.ElectionDistricts.Select(ei => ei.VALGKOD.ToString()).Distinct().ToArray();
            }
        }

        public override IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public abstract IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, decimal pnr);
    }

    public abstract class GeoLookupHeatingDistrictComparisonTest<T> : ComparisonTest<T, CprBroker.Providers.DPR.LookupDataContext>
    {
        public override string[] LoadKeys()
        {
            using (var dataContext = new CprBroker.Providers.DPR.LookupDataContext(RealDprDatabaseConnectionString))
            {
                return dataContext.HeatingDistricts.Select(ei => ei.VARMEKOD.ToString()).Distinct().ToArray();
            }
        }

        public override IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public abstract IQueryable<T> Get(CprBroker.Providers.DPR.LookupDataContext dataContext, decimal pnr);
    }
}
