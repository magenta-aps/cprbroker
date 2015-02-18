using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Engine.Tasks;

namespace CprBroker.Engine.Tests
{
    namespace PeriodicTaskExecuterTests
    {

        [TestFixture]
        public class Create 
        {
            [Test]
            public void Create_NoArgs_GoesOK([Range(1, 100)] int pass)
            {
                var pte = new PeriodicTaskExecuter();
            }
        }
    }
}
