using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace ProtectionTypeTests
    {
        public class ValuesEnumerator
        {
            public static ProtectionType.ProtectionCategoryCodes[] GetValues()
            {
                return Enum.GetValues(typeof(ProtectionType.ProtectionCategoryCodes)).OfType<ProtectionType.ProtectionCategoryCodes>().ToArray();
            }
        }

        [TestFixture]
        public class HasProtection
        {
            [Test]
            public void HasProtection_CorrectType_True(
                [ValueSource(typeof(ValuesEnumerator), "GetValues")]ProtectionType.ProtectionCategoryCodes category)
            {
                var pro = new ProtectionType { ProtectionCategoryCode = category };
                var ret = pro.HasProtection(DateTime.Today, category);
                Assert.True(ret);
            }

            [Test]
            public void HasProtection_CorrectTypeWithDateRange_True(
                [ValueSource(typeof(ValuesEnumerator), "GetValues")]ProtectionType.ProtectionCategoryCodes category,
                [Values(0, 1, 2)]int offset)
            {
                DateTime today = DateTime.Today;
                var pro = new ProtectionType { ProtectionCategoryCode = category, StartDate = today.AddDays(-offset), EndDate = today.AddDays(offset) };
                var ret = pro.HasProtection(today, category);
                Assert.True(ret);
            }

            [Test]
            public void HasProtection_CorrectTypeOutOfDateRange_False(
                [ValueSource(typeof(ValuesEnumerator), "GetValues")]ProtectionType.ProtectionCategoryCodes category)
            {
                DateTime today = DateTime.Today;
                var pro = new ProtectionType { ProtectionCategoryCode = category, StartDate = today.AddDays(1), EndDate = today.AddDays(2) };
                var ret = pro.HasProtection(today, category);
                Assert.False(ret);
            }
        }

        [TestFixture]
        public class HasProtection2
        {
            [Test]
            [ExpectedException]
            public void HasProtection_Null_Exception(
                [ValueSource(typeof(ValuesEnumerator), "GetValues")]ProtectionType.ProtectionCategoryCodes category)
            {
                ProtectionType.HasProtection(null, DateTime.Today, category);
            }

            [Test]
            public void HasProtection_CorrectCategory_OK(
                [ValueSource(typeof(ValuesEnumerator), "GetValues")]ProtectionType.ProtectionCategoryCodes category)
            {
                var ret = ProtectionType.HasProtection(new ProtectionType[] { new ProtectionType() { ProtectionCategoryCode = category } }, DateTime.Today, category);
                Assert.True(ret);
            }
        }
    }
}
