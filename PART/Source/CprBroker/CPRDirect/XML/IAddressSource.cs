using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public interface IAddressSource
    {
        AdresseType ToAdresseType();
        VirkningType[] ToVirkningTypeArray();
        string ToAddressNoteTekste();
    }

    public class DummyAddressSource : IAddressSource
    {
        public AdresseType ToAdresseType()
        {
            return null;
        }

        public VirkningType[] ToVirkningTypeArray()
        {
            return new VirkningType[0];
        }


        public string ToAddressNoteTekste()
        {
            return null;
        }
    }


}
