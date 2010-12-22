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
            WebService = 1,
            //TODO: See if possible to change value to 2
            FileShare = 3
        }
    }

}
