using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Tests.DPR
{
    public class BaseTests
    {
        public decimal[] RandomCprNumbers5()
        {
            return Utilities.RandomCprNumbers(5);
        }

        public decimal[] RandomDecimalDates5()
        {
            return Utilities.RandomDecimalDates(5);
        }

        public decimal[] RandomDecimals5()
        {
            return Utilities.RandomShorts(5).Select(s => (decimal)s).ToArray();
        }

        public decimal[] RandomCountryCodes5()
        {
            return Utilities.RandomShorts(5).Select(s => (decimal)s).ToArray();
        }

        public decimal[] RandomMunicipalityCodes5()
        {
            return Utilities.RandomShorts(5).Select(s => (decimal)s).ToArray();
        }

        public decimal[] RandomStreetCodes5()
        {
            return Utilities.RandomShorts(5).Select(s => (decimal)s).ToArray();
        }

        public string[] RandomHouseNumbers5()
        {
            return Utilities.RandomStrings(5);
        }

        public string[] RandomStrings5()
        {
            return Utilities.RandomStrings(5);
        }

        public decimal[] AllCivilRegistrationStates()
        {
            var values = Enum.GetValues(typeof(Schemas.PersonCivilRegistrationStatusCode)).AsQueryable();
            return values.Cast<Schemas.PersonCivilRegistrationStatusCode>().Select(e => (decimal)e).ToArray();

        }
    }
}
