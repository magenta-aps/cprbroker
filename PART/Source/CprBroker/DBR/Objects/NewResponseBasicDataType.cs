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
        public NewResponseNoDataType(IndividualResponseType resp)
        {
            this.PNR = resp.PersonInformation.PNR;
            this.Ok = "OK";
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

    public partial class NewResponseFullDataType : INewResponseData
    {
        public NewResponseFullDataType(IndividualResponseType resp)
        {
            // TODO: Implement this
            throw new NotImplementedException();
        }
    }
}
