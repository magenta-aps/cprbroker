using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    class CurrentAddressWrapper : IAddressSource
    {
        private CurrentAddressWrapper()
        { }

        public CurrentAddressWrapper(CurrentAddressInformationType currentAddress, ClearWrittenAddressType clearAddress)
        {
            this.ClearWrittenAddress = clearAddress;
            this.CurrentAddressInformation = currentAddress;
        }

        public ClearWrittenAddressType ClearWrittenAddress;
        public CurrentAddressInformationType CurrentAddressInformation;

        public AdresseType ToAdresseType()
        {
            return ClearWrittenAddress.ToAdresseType();
        }

        public VirkningType[] ToVirkningTypeArray()
        {
            return this.CurrentAddressInformation.ToVirkningTypeArray();
        }


        public string ToAddressNoteTekste()
        {
            return this.ClearWrittenAddress.ToAddressNoteTekste();
        }
    }
}
