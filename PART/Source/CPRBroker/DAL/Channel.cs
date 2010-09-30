using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.DAL
{
    public partial class ChannelType
    {
        public enum ChannelTypes
        {
            WebService=1,
            GPAC=2,
            FileShare=3
        }    
    }

}
