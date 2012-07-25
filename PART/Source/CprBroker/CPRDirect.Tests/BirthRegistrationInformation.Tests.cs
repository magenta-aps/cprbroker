using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace BirthRegistrationInformationTests
    {
        [TestFixture]
        public class SSS
        {
            [Test]
            public void SSSTTT()
            {
                var lines = LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var wrappers = lines.Select(w => w.ToWrapper(Constants.DataObjectMap));
                var myWrappers = wrappers.Where(w => w is BirthRegistrationInformationType).Select(w => w as BirthRegistrationInformationType).ToArray();
                var withText = myWrappers.Where(w => !string.IsNullOrEmpty(w.AdditionalBirthRegistrationText)).ToArray();
                var noText = myWrappers.Where(w => string.IsNullOrEmpty(w.AdditionalBirthRegistrationText)).ToArray();
                object o = "";
            }
        }
    }
}
