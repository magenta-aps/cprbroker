using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using CPRNotificationServiceDemo.CPRAdministration;

namespace CPRNotificationServiceDemo
{
    public class CprAdministrationAdapter
    {
        public CprAdministrationAdapter()
        {
            UserToken = ConfigurationManager.AppSettings["UserToken"];
            AppToken = ConfigurationManager.AppSettings["AppToken"];
            AppName = ConfigurationManager.AppSettings["ApplicationName"];
        }


        public void Subscribe(string cpr, string notificationUrl)
        {
            try
            {
                var cprAdmin = GetCprAdmin();
                var notificationChannel = new WebServiceChannelType
                                              {
                                                  WebServiceUrl = notificationUrl
                                              };
                var cprNumbers = new string[] { cpr };
                var subscription = cprAdmin.Subscribe(GetHeader(), notificationChannel, cprNumbers);
            }
            catch (Exception)
            {
                throw;
            }
        }


        private CPRAdministration.CPRAdministrationWSSoapClient GetCprAdmin()
        {
            var administrationHandler = new CPRAdministrationWSSoapClient();
            return administrationHandler;
        }


        ApplicationHeader GetHeader()
        {
            return new ApplicationHeader()
                {
                    UserToken = UserToken,
                    ApplicationToken = AppToken
                };
        }

        private readonly string UserToken;
        private readonly string AppToken;
        private string AppName;
    }
}
