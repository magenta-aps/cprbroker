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

        public override string ToString()
        {
            return ContentsWithSeparator(trimLeftZeros: false);
        }

    }

    public partial class NewResponseBasicDataType : INewResponseData
    {
        public NewResponseBasicDataType()
        { }

        public NewResponseBasicDataType(IndividualResponseType resp, PersonInfoExtended personInfo, string dprConnectionString)
            : base(resp, personInfo, dprConnectionString)
        {
            this.Floor = personInfo.Address?.Floor;
            this.Door = personInfo.Address?.DoorNumber?.Trim();

            var addressProtection = ProtectionType.FindProtection(resp.Protection, DateTime.Now, ProtectionType.ProtectionCategoryCodes.NameAndAddress);
            this.AddressProtectionMarker = addressProtection == null ? ' ' : '1';
            this.AddressProtectionDate = addressProtection?.StartDate;
            this.Status = personInfo.PersonTotal.Status;
        }

        /// <summary>
        /// To be used in the constructor or ClassicResponseBasicDataType
        /// </summary>
        public DateTime? AddressProtectionDate { get; private set; }

        public override string ToString()
        {
            return ContentsWithSeparator(trimLeftZeros: false);
        }
    }
}
