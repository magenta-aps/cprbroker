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
        string Contents { get; set; }
        string ContentsWithSeparator(string separator = ";", bool trimLeftZeros = false);
        bool TrimLeftZeros { get; }
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

        public bool TrimLeftZeros
        {
            get { return false; }
        }

    }

    public partial class NewResponseBasicDataType : INewResponseData
    {
        public NewResponseBasicDataType()
        { }

        public NewResponseBasicDataType(IndividualResponseType resp, PersonInfoExtended personInfo, string dprConnectionString)
            : base(resp, personInfo, dprConnectionString)
        {
            var addressProtection = ProtectionType.FindProtection(resp.Protection, DateTime.Now, ProtectionType.ProtectionCategoryCodes.NameAndAddress);
            this.AddressProtectionMarker = addressProtection == null ? ' ' : '1';

            this.Status = personInfo.PersonTotal.Status;
        }

        public bool TrimLeftZeros
        {
            get { return false; }
        }
    }
}
