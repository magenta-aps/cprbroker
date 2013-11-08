using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using CPRBroker.Schemas;

namespace CPRNotificationServiceDemo
{
    /// <summary>
    /// Template for a notification listener web service
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/", Name = ServiceNames.Notification.Service, Description = ServiceDescription.Notification.Service)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class Notification : System.Web.Services.WebService
    {

        public Notification()
        {

        }

        [WebMethod(MessageName = ServiceNames.Notification.MethodNames.Notify, Description = ServiceDescription.Notification.Notify)]
        public void Notify(string appToken, BaseNotificationType notification)
        {
            NotificationHandler.HandleNotification(notification);            
        }

        [WebMethod(MessageName = ServiceNames.Notification.MethodNames.Ping, Description = ServiceDescription.Notification.Ping)]
        public void Ping()
        {
        }

    }

}
