using CprBroker.Providers.CPRDirect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.DBR
{
    public interface INewResponseData
    {

    }

    public partial class NewResponseNoDataType : INewResponseData
    {
        public NewResponseNoDataType()
        {

        }

        public NewResponseNoDataType(IndividualResponseType resp)
        {
            this.PNR = resp.PersonInformation.PNR;
            this.OkOrError = "OK";
        }
    }

    public partial class NewResponseBasicDataType : INewResponseData
    {
        public NewResponseBasicDataType()
        { }

        public NewResponseBasicDataType(IndividualResponseType resp)
            : base(resp)
        { }
    }
}
