using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Engine.Exceptions
{
    /// <summary>
    /// Exception to be raised if the application token supplied to the system is invalid
    /// </summary>
    public class InvalidTokenException : Exception
    {
        public string ApplicationToken { get; set; }
        public string UserToken { get; set; }

        public override string Message
        {
            get
            {
                return string.Format("The token is invalid: appToken: {0} - userToken: {1}", ApplicationToken, UserToken);
            }
        }

        public InvalidTokenException(string appToken, string userToken)
        {
            ApplicationToken = appToken;
            UserToken = userToken;
        }
    }
}
