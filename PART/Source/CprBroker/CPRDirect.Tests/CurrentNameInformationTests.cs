using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace CurrentNameInformationTests
    {
        [TestFixture]
        public class TestFlags
        {
            [Test]
            public void TestLines()
            {
                var lines = Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var wrappers = Wrapper.ParseBatch(lines, Constants.DataObjectMap);
                var myWrappers = wrappers.Where(w => w is CurrentNameInformationType).Select(w => w as CurrentNameInformationType);
                var first = myWrappers.Select(w => w.FirstNameMarker).Distinct().ToArray();
                var middle = myWrappers.Select(w => w.MiddleNameMarker).Distinct().ToArray();
                var last = myWrappers.Select(w => w.LastNameMarker).Distinct().ToArray();
                var allText = myWrappers.Select(w => w.Contents).ToArray();
                var found = myWrappers.Where(w => w.MiddleNameMarker == '*').First();
            }
        }

        [TestFixture]
        public class ToNavnStrukturType
        {
            [Test]
            public void ToNavnStrukturType_FirstNameValueNoFlag_EmptyFirstName()
            {
                var name = new CurrentNameInformationType() { FirstName_s = "klasjlkdfakl", FirstNameMarker = '*' };
                var partName = name.ToNavnStrukturType();
                Assert.IsNullOrEmpty(partName.PersonNameStructure.PersonGivenName);
            }

            [Test]
            public void ToNavnStrukturType_FirstNameValueWFlag_CorrectEmptyFirstName(
                [Values("klasdjkl", "iqiiii")]string firstName)
            {
                var name = new CurrentNameInformationType() { FirstName_s = firstName, FirstNameMarker = ' ' };
                var partName = name.ToNavnStrukturType();
                Assert.AreEqual(firstName, partName.PersonNameStructure.PersonGivenName);
            }

            [Test]
            public void ToNavnStrukturType_AddressingName_Correct()
            {
                string strName = Guid.NewGuid().ToString().Substring(0, 34);
                var name = new CurrentNameInformationType() { AddressingName = strName };
                var partName = name.ToNavnStrukturType();
                Assert.AreEqual(strName, partName.PersonNameForAddressingName);
            }
        }
    }
}
