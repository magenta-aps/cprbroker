using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Data.Part
{
    /// <summary>
    /// Represents the ContactChannelType table
    /// </summary>
    public partial class ContactChannelType
    {
        /// <summary>
        /// Possible types of contact channels
        /// </summary>
        public enum ChannelTypes
        {
            Email = 0,
            Telephone = 1,
            Other = 2
        }
    }
}
