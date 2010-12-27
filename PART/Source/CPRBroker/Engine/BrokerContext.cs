using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics;
using System.Web.Services;
using CPRBroker.DAL.Applications;
using CPRBroker.Engine.Exceptions;

namespace CPRBroker.Engine
{
    /// <summary>
    /// Contains data related to the current method call
    /// An object of this class is created and stored in CallContext my means of a call to Initialize()
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
        public bool IsFixed { get; private set; }
        public string WebMethodMessageName { get; private set; }

        public static readonly string ContextKey = typeof(BrokerContext).ToString();

        // Current context stored in CallContext
        public static BrokerContext Current
        {
            get
            {
                BrokerContext ret = CallContext.GetData(ContextKey) as BrokerContext;
                //TODO: Create a default context if none has been created. This will help actions from backend service
                return ret;
            }
            //TODO : This was originally 'internal'
            set
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
        /// <param name="authenticateWebMethod">Whether to validate the permissions of the current user allows him to access the current method'd message name</param>
        public static void Initialize(string appToken, string userToken, bool failInNoApp, bool authenticateWebMethod, bool initializeAsFixed)
        {
            if (Current != null && Current.IsFixed)
            {
                return;
            }
            using (ApplicationDataContext dataContext = new ApplicationDataContext())
            {
                Current = new BrokerContext();
                Current.ActivityId = Guid.NewGuid();
                Current.ApplicationToken = appToken;
                Current.UserToken = userToken;
                Current.UserName = Util.Security.CurrentUser;
                Current.IsFixed = initializeAsFixed;

                Application currentApplication = dataContext.Applications.SingleOrDefault(app => app.Token.ToString() == appToken && app.IsApproved == true);
                if (currentApplication != null)
                {
                    Current.ApplicationId = currentApplication.ApplicationId;
                    Current.ApplicationName = currentApplication.Name;
                }
                else if (failInNoApp)
                {
                    throw new InvalidTokenException(appToken, userToken);
                }

                // Now find if current user is authorized to access the current method
                if (authenticateWebMethod)
                {
                    // Search up the stack to find the Method's message name
                    System.Reflection.MethodBase method = null;
                    for (int i = 1; true; i++)
                    {
                        StackFrame stackFrame = new StackFrame(i);
                        method = stackFrame.GetMethod();
                        if (method.IsDefined(typeof(WebMethodAttribute), false) && method.DeclaringType.IsSubclassOf(typeof(System.Web.Services.WebService)))
                        {
                            break;
                        }
                    }
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
                        throw new System.Security.Authentication.AuthenticationException();
                    }
                }
            }
        }
    }
}
