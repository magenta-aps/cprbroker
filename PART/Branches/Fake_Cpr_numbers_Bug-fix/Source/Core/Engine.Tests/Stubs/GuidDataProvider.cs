using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;

namespace CprBroker.Tests.Engine.Stubs
{
    public class GuidDataProvider : IDataProvider
    {
        public GuidDataProvider()
        {
            //System.Diagnostics.Debugger.Launch();
        }

        public Guid AAAA(Guid guid)
        {
            return guid;
        }
        public Version Version { get { return new Version(1, 0); } }
        public bool IsAlive() { return true; }
    }

    public class GuidDataProvider2 : GuidDataProvider
    { }
    public class GuidDataProvider3 : GuidDataProvider
    { }
    public class GuidDataProvider4 : GuidDataProvider
    { }
    public class GuidDataProvider5 : GuidDataProvider
    { }
}
