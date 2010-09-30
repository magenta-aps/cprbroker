using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Engine.Util
{
    public static class Security
    {
        public static string CurrentUser
        {
            get
            {
                if (System.Web.HttpContext.Current != null)
                    return System.Web.HttpContext.Current.User.Identity.Name;
                else
                    return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
        }
    }
}
