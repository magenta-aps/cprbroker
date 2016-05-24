using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine
{
    public abstract class BrokerEnvironment
    {
        public ConfigModule Config;
        public object GetData();
        public DataModule DataModule;
        public Session CreateSession();
    }

    public class Session
    {

    }

    public class DataModule
    {

    }

    public class ConfigModule
    {

    }

    public class DataProviderModule
    {
        public void AddDataProvider();
        public void RemoveDataProvider();
        public void UpdateDataProvider();
    }
}
