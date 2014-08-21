using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.DPR;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;
using CprBroker.DBR;
using System.Reflection;
using CprBroker.Utilities;
using CprBroker.PartInterface;
namespace CprBroker.Tests.DBR.Comparison
{
    public abstract class ComparisonTest<TObject, TDataContext>
    {
        public string CprBrokerConnectionString = "data source=localhost\\sqlexpress;initial catalog=part;integrated security=sspi";
        public string RealDprDatabaseConnectionString = "";
        public string FakeDprDatabaseConnectionString = "";

        public abstract string[] LoadKeys();
        public abstract IQueryable<TObject> Get(TDataContext dataContext, string key);
    }

}
