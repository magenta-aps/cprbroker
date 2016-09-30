using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.DBR
{
    public partial class NewResponseFullDataType : INewResponseData
    {
        public NewResponseFullDataType(IndividualResponseType resp)
        {
            // TODO: Implement this
            this.PNR = resp.PersonInformation.PNR;
            //this.AJFDTO_NAVNEDecimal = new PersonName().CprUpdateDate;
        }
    }
}
