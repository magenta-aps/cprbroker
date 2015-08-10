using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Framework;
using CprBroker.Providers.DPR;
using System.Data.Linq;

namespace CprBroker.Tests.DBR.Comparison.Geo
{
    public abstract class GeoLookupComparisonTest<T> : ComparisonTest<T, LookupDataContext>
        where T : class
    {
        int timesRun = 0;
        int randomTestNumber;

        public override IQueryable<T> Get(Providers.DPR.LookupDataContext dataContext, string key)
        {
            return Get(dataContext, decimal.Parse(key));
        }

        public virtual IQueryable<T> Get(Providers.DPR.LookupDataContext dataContext, decimal key)
        {
            throw new NotImplementedException();
        }

        public override sealed Providers.DPR.LookupDataContext CreateDataContext(string connectionString)
        {
            return new Providers.DPR.LookupDataContext(connectionString);
        }

        public override sealed string[] LoadKeys()
        {
            using (var dataContext = new LookupDataContext(Properties.Settings.Default.RealDprConnectionString))
            {
                if (timesRun < 1)
                {
                    Random random = new Random();
                    randomTestNumber = random.Next(8416);
                    timesRun++;
                }
                else
                    timesRun = 0;
                Console.WriteLine("NUMBER: " + randomTestNumber);
                var propType = typeof(Table<>).MakeGenericType(typeof(T));
                var prop = dataContext.GetType().GetProperties().Where(p => p.PropertyType == propType).Single();
                var objects = prop.GetValue(dataContext, null) as Table<T>;
                var arr = objects.Skip(randomTestNumber).Take(10).ToArray();
                return arr.Select(o => GetKey(o)).ToArray();
            }
        }

        public abstract string GetKey(T obj);

    }
}
