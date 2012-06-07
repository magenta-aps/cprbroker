using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace CurrentCivilStatusTests
    {
        [TestFixture]
        public class ToCivilStatusType
        {
            public void ToCivilStatusType_NoSeparation_StatusDate(
                [Values('D', 'E', 'F', 'G', 'L', 'O', 'P', 'U')]char maritalStatus)
            {
                var status = new CurrentCivilStatusType() { CivilStatusStartDate = DateTime.Today, CivilStatusStartDateUncertainty = ' ', CivilStatus = maritalStatus };
                var ret = status.ToCivilStatusType(null);
                Assert.AreEqual(DateTime.Today, ret.TilstandVirkning.FraTidspunkt.ToDateTime());
            }

            public void ToCivilStatusType_WithSeparation_StatusDate(
                [Values('D', 'E', 'F', 'G', 'L', 'O', 'P', 'U')]char maritalStatus)
            {
                var status = new CurrentCivilStatusType() { CivilStatusStartDate = DateTime.Today.AddDays(-1), CivilStatusStartDateUncertainty = ' ', CivilStatus = maritalStatus };
                var sep = new CurrentSeparationType() { SeparationStartDate = DateTime.Today, SeparationStartDateUncertainty = ' ' };
                var ret = status.ToCivilStatusType(null);
                Assert.AreEqual(sep.ToSeparationStartDate(), ret.TilstandVirkning.FraTidspunkt.ToDateTime());
            }
        }
    }
}
