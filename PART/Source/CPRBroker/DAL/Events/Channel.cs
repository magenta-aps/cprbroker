using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.DAL.Events
{
    public partial class ChannelType
    {
        public enum ChannelTypes
        {
            WebService=1,
            //TODO: Remove GPAC
            GPAC = 2,
            FileShare=3
        }    
    }

}
