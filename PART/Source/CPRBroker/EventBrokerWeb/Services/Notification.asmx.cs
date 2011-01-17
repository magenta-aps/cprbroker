using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Web
{
    /// <summary>
    /// Template for a notification listener web service
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/", Name = Schemas.ServiceNames.Notification.Service, Description = Schemas.ServiceDescription.Notification.Service)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class Notification : System.Web.Services.WebService
    {

        public Notification()
        {

        }

        [WebMethod(MessageName = Schemas.ServiceNames.Notification.MethodNames.Notify, Description = Schemas.ServiceDescription.Notification.Notify)]
        public void Notify(string appToken, Schemas.Part.Events.CommonEventStructureType notification)
        {

        }

        [WebMethod(MessageName = Schemas.ServiceNames.Notification.MethodNames.Ping, Description = Schemas.ServiceDescription.Notification.Ping)]
        public void Ping()
        {
        }

    }

}
