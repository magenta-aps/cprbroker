using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using CprBroker.EventBroker;
using CprBroker.Schemas;
using System.Web.Services.Protocols;

namespace CprBroker.EventBroker.Web.Services
{
    /*
    /// <summary>
    /// Summary description for Notification
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/", Name = CprBroker.Schemas.Part.ServiceNames.NotificationQueue.Service, Description = CprBroker.Schemas.Part.ServiceDescription.NotificationQueue.Service)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class NotificationQueue : System.Web.Services.WebService
    {
        public ApplicationHeader applicationHeader;
        private const string ApplicationHeaderName = "applicationHeader";


        [SoapHeader(ApplicationHeaderName)]
        [WebMethod(MessageName = CprBroker.Schemas.Part.ServiceNames.NotificationQueue.Methods.Enqueue, Description = CprBroker.Schemas.Part.ServiceDescription.NotificationQueue.Enqueue)]
        public bool Enqueue(Guid personUuid)
        {
            return Manager.Subscriptions.Enqueue(applicationHeader.UserToken, applicationHeader.ApplicationToken, personUuid);
        }
    }
     */
}
