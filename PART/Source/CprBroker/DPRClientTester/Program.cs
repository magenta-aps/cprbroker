using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CprBroker.Utilities.ConsoleApps;

namespace DPRClientTester
{
    public class Program
    {
        public static void Main(params string[] args)
        {
            ConsoleEnvironment env = ConsoleEnvironment.Create(args);
            env.Run();
        }
    }
}
