using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using CPRBroker.Engine;
using CPRBroker.Schemas;

namespace CPRService.Services
{
    /// <summary>
    /// Allows web access for certain administrative functions
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/", Name = ServiceNames.Access.Service, Description = ServiceDescription.Access.Service)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class Access : System.Web.Services.WebService
    {

        [WebMethod(MessageName = ServiceNames.Access.MethodNames.SendNotifications, Description = ServiceDescription.Access.SendNotifications)]
        public NotificationEngine.SendNotificationsResult SendNotifications(DateTime today)
        {
            return NotificationEngine.SendNotifications(today);
        }

        [WebMethod(MessageName = ServiceNames.Access.MethodNames.RefreshPersonsData, Description = ServiceDescription.Access.RefreshPersonsData)]
        public NotificationEngine.RefreshPersonsDataResult RefreshPersonsData()
        {
            return NotificationEngine.RefreshPersonsData();
        }
    }
}
