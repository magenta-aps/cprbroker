using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.E_M;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.E_M
{
    [TestFixture]
    class CitizenPotReadyAddressTests
    {
        CitizenPotReadyAddress[] AddressTestValues = new CitizenPotReadyAddress[] 
            {
                null,
                new CitizenPotReadyAddress()
            };

        [TestCaseSource("AddressTestValues")]
        public void TestAdresseType(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAdresseType(address);
            UnitTests.ValidateNulls<CitizenPotReadyAddress, AdresseType>(address, result);
        }

        [TestCaseSource("AddressTestValues")]
        public void TestDanskAdresseType(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToDanskAdresseType(address);
            UnitTests.ValidateNulls<CitizenPotReadyAddress, DanskAdresseType>(address, result);
        }

        [TestCaseSource("AddressTestValues")]
        public void TestToAddressCompleteType(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressCompleteType(address);
            UnitTests.ValidateNulls<CitizenPotReadyAddress, AddressCompleteType>(address, result);
        }

        [TestCaseSource("AddressTestValues")]
        public void TestToAddressAccessType(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressAccessType(address);
            UnitTests.ValidateNulls<CitizenPotReadyAddress, AddressAccessType>(address, result);
        }

        [TestCaseSource("AddressTestValues")]
        public void TestToAddressPostalType(CitizenPotReadyAddress address)
        {
            var result = CitizenPotReadyAddress.ToAddressPostalType(address);
            UnitTests.ValidateNulls<CitizenPotReadyAddress, AddressPostalType>(address, result);
        }
    }
}
