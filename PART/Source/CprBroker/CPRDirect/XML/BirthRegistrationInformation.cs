using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    public partial class BirthRegistrationInformationType
    {

        public string ToFoedselsregistreringMyndighedNavn()
        {
            if (!string.IsNullOrEmpty(this.AdditionalBirthRegistrationText))
            {
                return this.AdditionalBirthRegistrationText;
            }
            else
            {
                return Authority.GetNameByCode(this.BirthRegistrationAuthorityCode);
            }
        }
    }
}
