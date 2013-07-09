/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CprBroker.Utilities;

namespace CprBroker.EventBroker.Notifications
{
    /// <summary>
    /// Base class for notification channels
    /// </summary>
    public abstract class Channel
    {
        /// <summary>
        /// Maps channel types (enum) to actual CLR types
        /// </summary>
        static Dictionary<Data.ChannelType.ChannelTypes, Type> TypesMap;
        static Channel()
        {
            TypesMap = new Dictionary<Data.ChannelType.ChannelTypes, Type>();
            TypesMap[Data.ChannelType.ChannelTypes.FileShare] = typeof(FileShareChannel);
            TypesMap[Data.ChannelType.ChannelTypes.WebService] = typeof(WebServiceChannel);
        }

        /// <summary>
        /// Creates a new Channel from its database representation
        /// </summary>
        /// <param name="dbChannel"></param>
        /// <returns></returns>
        public static Channel Create(Data.Channel dbChannel)
        {
            Data.ChannelType.ChannelTypes channelType = (Data.ChannelType.ChannelTypes)dbChannel.ChannelTypeId;

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

        protected Data.Channel DatabaseObject { get; private set; }

        /// <summary>
        /// Pings the channel
        /// </summary>
        /// <returns>True is the channel is working, false otherwise</returns>
        public abstract bool IsAlive();

        /// <summary>
        /// Send the supplied notification through the channel
        /// </summary>
        /// <param name="notification"></param>
        public abstract void Notify(Data.EventNotification notification);
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
            notification.Credentials = System.Net.CredentialCache.DefaultCredentials;
            notification.Ping();
            return true;
        }

        /// <summary>
        /// Notifies by calling a CPR Notification web service
        /// </summary>
        /// <param name="notification"></param>
        public override void Notify(Data.EventNotification notification)
        {
            NotificationService.Notification notificationService = new NotificationService.Notification();
            notificationService.Url = DatabaseObject.Url;
            notificationService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            var wsdlNotif = notification.ToWsdl();
            notificationService.Notify(wsdlNotif);
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
        public override void Notify(Data.EventNotification notification)
        {
            string folder = DatabaseObject.Url;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var oioNotif = notification.ToOioNotification();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(oioNotif.GetType());
            string filePath = Strings.NewUniquePath(folder, "xml");
            System.IO.StreamWriter w = new System.IO.StreamWriter(filePath);
            serializer.Serialize(w, oioNotif);
            w.Close();
        }
    }

}
