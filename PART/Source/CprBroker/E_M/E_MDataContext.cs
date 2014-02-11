using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.E_M
{
    public partial class E_MDataContext
    {
        public void SetCitizenLoadOptions()
        {
            var loadOptions = new System.Data.Linq.DataLoadOptions();
            loadOptions.LoadWith<Citizen>(cit => cit.ChildrenAsFather);
            loadOptions.LoadWith<Citizen>(cit => cit.ChildrenAsMother);
            loadOptions.LoadWith<Citizen>(cit => cit.BirthRegistrationAuthority);
            loadOptions.LoadWith<Citizen>(cit => cit.CitizenReadyAddress);
            loadOptions.LoadWith<Citizen>(cit => cit.CitizenReadyAddress.Roads);
            this.LoadOptions = loadOptions;
        }
    }
}
