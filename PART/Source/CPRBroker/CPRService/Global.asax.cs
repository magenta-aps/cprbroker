using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace CprBroker.Web
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            if (CprBroker.Config.Properties.Settings.Default.EncryptConnectionStrings)
            {
                try
                {
                    CprBroker.Engine.Util.Security.EncryptConnectionStrings();
                }
                catch (Exception ex)
                { }
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex is HttpUnhandledException && ex.InnerException != null)
            {
                Engine.BrokerContext.Initialize(DAL.Applications.Application.BaseApplicationToken.ToString(), Engine.Constants.UserToken);
                Engine.Local.Admin.LogCriticalException(ex.InnerException);
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}