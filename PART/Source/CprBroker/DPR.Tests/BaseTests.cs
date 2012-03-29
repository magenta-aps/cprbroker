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

        public decimal[] RandomCountryCodes5()
        {
            return Utilities.RandomShorts(5).Select(s => (decimal)s).ToArray();
        }
    }
}
