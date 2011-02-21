using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{

    public enum HttpErrorCode
    {
        OK = 200,
        UNSPECIFIED = 500, /* Server */
        DATASOURCE_UNAVAILABLE = 503, /* Server unavailable*/
        BAD_CLIENT_REQUEST = 400, /* Client */
        NOT_FOUND = 404, /*  */
        NOT_IMPLEMENTED = 501
    }

    public partial class StandardReturType
    {
        public static StandardReturType Create(HttpErrorCode code)
        {
            return Create(code, code.ToString());

        }
        public static StandardReturType Create(HttpErrorCode code, string text)
        {
            return new StandardReturType()
            {
                StatusKode = ((int)code).ToString(),
                FejlbeskedTekst = text
            };
        }

        public static bool IsSucceeded(StandardReturType ret)
        {
            if (ret != null)
            {
                int code;
                if (int.TryParse(ret.StatusKode, out code))
                {
                    return code == (int)HttpErrorCode.OK;
                }
            }
            return false;
        }

        public static StandardReturType OK()
        {
            return Create(HttpErrorCode.OK);
        }

        public static StandardReturType UnspecifiedError()
        {
            return Create(HttpErrorCode.UNSPECIFIED);
        }

        public static StandardReturType NullInput()
        {
            return Create(HttpErrorCode.BAD_CLIENT_REQUEST, "Input cannot be null");
        }

        public static StandardReturType NullInput(string valueName)
        {
            return Create(HttpErrorCode.BAD_CLIENT_REQUEST, string.Format("Input cannot be null: {0}", valueName));
        }

        public static StandardReturType UnknownObject(string value)
        {
            return Create(HttpErrorCode.BAD_CLIENT_REQUEST, string.Format("Unknown: {0}", value));
        }

        public static StandardReturType UnknownObject(string name, string value)
        {
            return Create(HttpErrorCode.BAD_CLIENT_REQUEST, string.Format("Unknown {0}: {1}", name, value));
        }

        public static StandardReturType ValueOutOfRange(object value)
        {
            return Create(HttpErrorCode.BAD_CLIENT_REQUEST, string.Format("Value \"{0}\" is out of valid range", value));
        }

        public static StandardReturType ValueOutOfRange(string name, object value)
        {
            return Create(HttpErrorCode.BAD_CLIENT_REQUEST, string.Format("Value \"{0}\" for \"{1}\" is out of valid range", value, name));
        }

        public static StandardReturType InvalidUuid(string uuid)
        {
            return Create(HttpErrorCode.BAD_CLIENT_REQUEST, string.Format("Invalid UUID: {0}", uuid));
        }

        public static StandardReturType InvalidCprNumber(string cpr)
        {
            return Create(HttpErrorCode.BAD_CLIENT_REQUEST, string.Format("Invalid CPR number: {0}", cpr));
        }

        public static StandardReturType UnknownUuid(string uuid)
        {
            return Create(HttpErrorCode.NOT_FOUND, string.Format("UUID valid but not found : {0}", uuid));
        }

        public static StandardReturType UnreachableChannel()
        {
            return Create(HttpErrorCode.NOT_FOUND, "NotificationChannel unreachable");
        }
    }
}
