using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    /// <summary>
    /// A wrapper that gathers ClearWrittenAddress and CurrentAddressInformation
    /// Reason is that ClearWrittenAddress has more address details
    /// But CurrentAddressInformation has the address dates
    /// The test data suggest that ClearWrittenAddress is empty when CurrentAddressInformation is null
    /// </summary>
    public class CurrentAddressWrapper : IAddressSource
    {
        private CurrentAddressWrapper()
        {
        }

        public CurrentAddressWrapper(CurrentAddressInformationType currentAddress, ClearWrittenAddressType clearAddress)
        {
            this.ClearWrittenAddress = clearAddress;
            this.CurrentAddressInformation = currentAddress;
        }

        public ClearWrittenAddressType ClearWrittenAddress { get; private set; }
        public CurrentAddressInformationType CurrentAddressInformation { get; private set; }

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
