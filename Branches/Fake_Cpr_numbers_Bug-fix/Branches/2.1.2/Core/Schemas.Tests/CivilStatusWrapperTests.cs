using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.Schemas
{
    namespace CivilStatusWrapperTests.Tests
    {
        class CivilStatusStub : ICivilStatus
        {
            public string PNR { get; set; }

            public char CivilStatusCode { get; set; }

            public DateTime? CivilStatusStartDate { get; set; }
            public DateTime? ToStartTS()
            {
                return CivilStatusStartDate;
            }
            public DateTime? CivilStatusEndDate { get; set; }
            public DateTime? ToEndTS()
            {
                return CivilStatusEndDate;
            }

            public string SpousePnr { get; set; }
            public string ToSpousePnr()
            {
                return SpousePnr;
            }

            public bool Valid { get; set; }
            public bool IsValid()
            {
                return Valid;
            }

            public DataTypeTags Tag { get; set; }
        }

        class SeparationStub : ISeparation
        {
            public CivilStatusType ToCivilStatusType()
            {
                return new CivilStatusType()
                {
                    CivilStatusKode = CivilStatusKodeType.Separeret,
                    TilstandVirkning = TilstandVirkningType.Create(ToStartTS())
                };
            }
            
            public DateTime? ToStartTS()
            { return null; }

            public DateTime? ToEndTS()
            { return null; }

            public DataTypeTags Tag { get; set; }
        }

        [TestFixture]
        public class ToCivilStatusType
        {
            [Test]
            public void ToCivilStatusType_Normal_NotNull()
            {
                var w = new CivilStatusWrapper(new CivilStatusStub() { CivilStatusCode = 'U' });
                var ret = w.ToCivilStatusType(null);
                Assert.NotNull(ret);
            }

            [Test]
            public void ToCivilStatusType_G_Married()
            {
                var w = new CivilStatusWrapper(new CivilStatusStub() { CivilStatusCode = 'G' });
                var ret = w.ToCivilStatusType(null);
                Assert.AreEqual(CivilStatusKodeType.Gift, ret.CivilStatusKode);
            }

            [Test]
            public void ToCivilStatusType_GwithSeparation_Separated()
            {
                var w = new CivilStatusWrapper(new CivilStatusStub() { CivilStatusCode = 'G' });
                var ret = w.ToCivilStatusType(new SeparationStub() { });
                Assert.AreEqual(CivilStatusKodeType.Separeret, ret.CivilStatusKode);
            }

        }
    }
}
