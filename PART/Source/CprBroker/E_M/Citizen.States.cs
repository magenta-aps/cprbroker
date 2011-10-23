using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using NUnit.Framework;

namespace CprBroker.Providers.E_M
{
    public partial class Citizen
    {
        private static TilstandListeType ToTilstandListeType(Citizen citizen)
        {

            return new TilstandListeType()
            {
                CivilStatus = ToCivilStatusType(citizen),
                LivStatus = ToLivStatusType(citizen),
                LokalUdvidelse = ToLokalUdvidelseType(citizen)
            };
        }

        private static CivilStatusType ToCivilStatusType(Citizen citizen)
        {
            if (citizen != null)
            {
                return new CivilStatusType()
                {
                    CivilStatusKode = Converters.ToCivilStatusKodeType(citizen.MaritalStatus),
                    //TODO: Check if this is the mariage start or end date
                    TilstandVirkning = TilstandVirkningType.Create(Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty))
                };
            }
            return null;
        }

        private static LivStatusType ToLivStatusType(Citizen citizen)
        {
            if (citizen != null)
            {
                return new LivStatusType()
                {
                    LivStatusKode = Converters.ToLivStatusKodeType(citizen.CitizenStatusCode, Converters.ToDateTime(citizen.Birthdate, citizen.BirthdateUncertainty)),
                    //TODO: Ensure the dates are correct
                    TilstandVirkning = TilstandVirkningType.Create(Converters.ToDateTime(citizen.CitizenStatusTimestamp, citizen.CitizenStatusTimestampUncertainty))
                };
            }
            return null;
        }

        [TestFixture]
        private partial class Tests
        {
            [TestFixtureSetUp]
            public void InitializeTestCitizens()
            {

            }
            private Citizen[] TestCitizens = new Citizen[]
            {
                null,
                new Citizen(){CitizenStatusCode = 1, CitizenStatusTimestamp = DateTime.Today, CitizenStatusTimestampUncertainty='T',MaritalStatus='U',MaritalStatusTimestamp=DateTime.Today, MaritalStatusTimestampUncertainty='T'}
            };

            [Test]
            [TestCaseSource("TestCitizens")]
            public void TestToCivilStatusType(Citizen citizen)
            {
                var result = ToCivilStatusType(citizen);
                UnitTests.ValidateNulls<Citizen, CivilStatusType>(citizen, result);
            }

            [Test]
            [TestCaseSource("TestCitizens")]
            public void TestToLivStatusType(Citizen citizen)
            {
                var result = ToLivStatusType(citizen);
                UnitTests.ValidateNulls<Citizen, LivStatusType>(citizen, result);
            }
        }
    }
}