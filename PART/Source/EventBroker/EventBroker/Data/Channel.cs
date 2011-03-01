using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;

namespace CprBroker.EventBroker.Data
{
    /// <summary>
    /// Represents the Channel table
    /// </summary>
    public partial class Channel
    {
        public static Channel FromXmlType(ChannelBaseType oio)
        {
            if (oio != null)
            {
                Channel dbChannel = new Channel();
                dbChannel.ChannelId = Guid.NewGuid();                

                if (oio is WebServiceChannelType)
                {
                    WebServiceChannelType webServiceChannel = oio as WebServiceChannelType;
                    dbChannel.ChannelTypeId = (int)ChannelType.ChannelTypes.WebService;
                    dbChannel.Url = webServiceChannel.WebServiceUrl;
                }
                else if (oio is FileShareChannelType)
                {
                    FileShareChannelType fileShareChannel = oio as FileShareChannelType;
                    dbChannel.ChannelTypeId = (int)ChannelType.ChannelTypes.FileShare;
                    dbChannel.Url = fileShareChannel.Path;
                }
                else
                {
                    return null;
                }
                return dbChannel;
            }
            return null;
        }
    }
}
