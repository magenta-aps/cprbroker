using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.DAL.Part
{
    public partial class ContactChannelType
    {
        public enum ChannelTypes
        {
            Email = 1,
            Telephone = 2,
            Other = 3
        }
    }
}
