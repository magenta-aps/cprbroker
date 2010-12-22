using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CPRBroker.Engine.Notifications
{
    /// <summary>
    /// Base class for notification channels
    /// </summary>
    public abstract class Channel
    {
        /// <summary>
        /// Maps channel types (enum) to actual CLR types
        /// </summary>
        static Dictionary<DAL.Events.ChannelType.ChannelTypes, Type> TypesMap;
        static Channel()
        {
            TypesMap = new Dictionary<DAL.Events.ChannelType.ChannelTypes, Type>();
            TypesMap[DAL.Events.ChannelType.ChannelTypes.FileShare] = typeof(FileShareChannel);
            TypesMap[DAL.Events.ChannelType.ChannelTypes.WebService] = typeof(WebServiceChannel);            
        }

        /// <summary>
        /// Creates a new Channel from its database representation
        /// </summary>
        /// <param name="dbChannel"></param>
        /// <returns></returns>
        public static Channel Create(DAL.Events.Channel dbChannel)
        {
            DAL.Events.ChannelType.ChannelTypes channelType = (CPRBroker.DAL.Events.ChannelType.ChannelTypes)dbChannel.ChannelTypeId;

            Type clrChannelType = TypesMap[channelType];
            Channel channel = clrChannelType.InvokeMember(
                null,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance,
                null,
                null,
                null
            ) as Channel;

            channel.DatabaseObject = dbChannel;
            return channel;
        }

        protected DAL.Events.Channel DatabaseObject { get; private set; }

        /// <summary>
        /// Pings the channel
        /// </summary>
        /// <returns>True is the channel is working, false otherwise</returns>
        public abstract bool IsAlive();

        /// <summary>
        /// Send the supplied notification through the channel
        /// </summary>
        /// <param name="notification"></param>
        public abstract void Notify(DAL.Events.Notification notification);
    }

    /// <summary>
    /// CPR Notification implementation of Channel
    /// </summary>
    public class WebServiceChannel : Channel
    {
        public override bool IsAlive()
        {
            NotificationService.Notification notification = new NotificationService.Notification();
            notification.Url = DatabaseObject.Url;
            notification.Ping();
            return true;
        }

        /// <summary>
        /// Notifies by calling a CPR Notification web service
        /// </summary>
        /// <param name="notification"></param>
        public override void Notify(DAL.Events.Notification notification)
        {
            NotificationService.Notification notificationService = new NotificationService.Notification();
            notificationService.Url = DatabaseObject.Url;
            Schemas.BaseNotificationType oioNotification = notification.ToOioNotification();
            NotificationService.BaseNotificationType wsdlNotif = oioNotification.ToWsdl();
            notificationService.Notify(DatabaseObject.Subscription.Application.Token, wsdlNotif);
        }
        
    }
        
    /// <summary>
    /// File share implementation of notification channel
    /// </summary>
    public class FileShareChannel : Channel
    {
        public override bool IsAlive()
        {
            if (!Directory.Exists(DatabaseObject.Url))
            {
                Directory.CreateDirectory(DatabaseObject.Url);
            }
            return Directory.Exists(DatabaseObject.Url);
        }

        /// <summary>
        /// Notifies by serializing the notification into an XML file
        /// </summary>
        /// <param name="notification"></param>
        public override void Notify(DAL.Events.Notification notification)
        {
            string folder = DatabaseObject.Url;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            Schemas.BaseNotificationType oioNotif = notification.ToOioNotification();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(oioNotif.GetType());
            string filePath = Util.Strings.NewUniquePath(folder, "xml");
            System.IO.StreamWriter w = new System.IO.StreamWriter(filePath);
            serializer.Serialize(w, oioNotif);
            w.Close();
        }
    }

}
