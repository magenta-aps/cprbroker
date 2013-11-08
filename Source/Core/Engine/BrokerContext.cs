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
using System.Runtime.Remoting.Messaging;
using System.Diagnostics;
using System.Web.Services;
using CprBroker.Data.Applications;
using CprBroker.Engine.Exceptions;
using CprBroker.Utilities;

namespace CprBroker.Engine
{
    /// <summary>
    /// Contains data related to the current method call
    /// An object of this class is created and stored in CallContext by means of a call to Initialize()
    /// </summary>
    public class BrokerContext
    {
        private BrokerContext()
        { }

        public Guid ActivityId { get; private set; }
        public string ApplicationToken { get; private set; }
        public string UserToken { get; private set; }
        public string UserName { get; private set; }
        public Nullable<Guid> ApplicationId { get; private set; }
        public string ApplicationName { get; private set; }
        public string WebMethodMessageName { get; private set; }

        public static readonly string ContextKey = typeof(BrokerContext).ToString();

        // Current context stored in CallContext
        public static BrokerContext Current
        {
            get
            {
                BrokerContext ret = CallContext.GetData(ContextKey) as BrokerContext;
                return ret;
            }
            internal set
            {
                CallContext.SetData(ContextKey, value);
            }
        }

        /// <summary>
        /// Initializes the current context and sets various members to the values supplied in parameters
        /// Also checks whether the app token is valid and whether the current user is authorized to access the called web method
        /// </summary>
        /// <param name="appToken">Application token supplied by the client</param>
        /// <param name="userToken">User token supplied by the client</param>
        /// <param name="userName">Current user name</param>
        /// <param name="failInNoApp">Whether to throw an exception if no approved application is found to match the token</param>
        /// 
        public static void Initialize(string appToken, string userToken)
        {
            if (Current != null)
            {
                return;
            }
            using (ApplicationDataContext dataContext = new ApplicationDataContext())
            {
                Current = new BrokerContext();
                Current.ActivityId = Guid.NewGuid();
                Current.ApplicationToken = appToken;
                Current.UserToken = userToken;
                Current.UserName = Security.CurrentUser;

                Application currentApplication = dataContext.Applications.SingleOrDefault(app => app.Token == appToken && app.IsApproved == true);
                if (currentApplication != null)
                {
                    Current.ApplicationId = currentApplication.ApplicationId;
                    Current.ApplicationName = currentApplication.Name;
                }
                else
                {
                    throw new InvalidTokenException(appToken, userToken);
                }

                // Now find if current user is authorized to access the current method
                if (System.Web.HttpContext.Current != null)
                {
                    var ctx = System.Web.HttpContext.Current;
                    // Search up the stack to find the Method's message name
                    System.Reflection.MethodBase method = null;
                    for (int i = 1; true; i++)
                    {
                        StackFrame stackFrame = new StackFrame(i);
                        method = stackFrame.GetMethod();
                        if (method ==null || method.IsDefined(typeof(WebMethodAttribute), false) && method.DeclaringType.IsSubclassOf(typeof(System.Web.Services.WebService)))
                        {
                            break;
                        }
                    }
                    if (method != null)
                    {
                        // Now get the method's message name
                        WebMethodAttribute webMethodAttribute = (from attr in method.GetCustomAttributes(typeof(WebMethodAttribute), false) select attr).First() as WebMethodAttribute;
                        Current.WebMethodMessageName = string.IsNullOrEmpty(webMethodAttribute.MessageName) ? method.Name : webMethodAttribute.MessageName;
                        bool isAuthorized = System.Web.Security.UrlAuthorizationModule.CheckUrlAccessForPrincipal(
                            string.Format("{0}/{1}", System.Web.HttpContext.Current.Request.Path, Current.WebMethodMessageName),
                            System.Web.HttpContext.Current.User,
                            "GET"
                            );
                        if (!isAuthorized)
                        {
                            Console.Write("not authoriced exception" + Current.WebMethodMessageName);
                            throw new System.Security.Authentication.AuthenticationException();
                        }
                    }
                }
            }
        }
    }
}
