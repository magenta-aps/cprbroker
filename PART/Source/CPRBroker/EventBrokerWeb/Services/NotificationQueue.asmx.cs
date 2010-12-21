using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using CprBroker.EventBroker;

namespace CprBroker.EventBroker.Web.Services
{
    /// <summary>
    /// Summary description for Notification
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class NotificationQueue : System.Web.Services.WebService
    {

        [WebMethod]
        public void Enqueue(Guid personUuid)
        {
            EventBroker.NotificationManager.Enqueue(personUuid);
        }
    }
}
