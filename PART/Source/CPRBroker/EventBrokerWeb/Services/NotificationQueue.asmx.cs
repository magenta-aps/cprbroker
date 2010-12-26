using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using CprBroker.EventBroker;
using CPRBroker.Schemas;
using System.Web.Services.Protocols;

namespace CprBroker.EventBroker.Web.Services
{
    /// <summary>
    /// Summary description for Notification
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/", Name = CPRBroker.Schemas.Part.ServiceNames.NotificationQueue.Service, Description = CPRBroker.Schemas.Part.ServiceDescription.NotificationQueue.Service)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class NotificationQueue : System.Web.Services.WebService
    {
        public ApplicationHeader applicationHeader;
        private const string ApplicationHeaderName = "applicationHeader";


        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CPRBroker.Schemas.Part.ServiceNames.NotificationQueue.Methods.Enqueue, Description = CPRBroker.Schemas.Part.ServiceDescription.NotificationQueue.Enqueue)]
        public bool Enqueue(Guid personUuid)
        {
            return Manager.Subscriptions.Enqueue(applicationHeader.UserToken, applicationHeader.ApplicationToken, personUuid);
        }
    }
}
