using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Tests.CprServices
{
    public class SearchMethodTestsBase
    {
        public string[] AvailableInputs
        {
            get
            {
                return new string[]{
                        CprBroker.Providers.CprServices.Properties.Resources.ADRSOG1,
                        CprBroker.Providers.CprServices.Properties.Resources.ADRESSE3,
                        CprBroker.Providers.CprServices.Properties.Resources.NVNSOG2
                    };
            }
        }

        public string[] AvailableOutputs
        {
            get
            {
                return new string[]{
                        Properties.Resources.ADRSOG1_Response_OK,
                        Properties.Resources.NVNSOG2_Response_OK,
                    };
            }
        }

    }

}
