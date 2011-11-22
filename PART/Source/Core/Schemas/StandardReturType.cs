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

namespace CprBroker.Schemas.Part
{

    public enum HttpErrorCode
    {
        OK = 200,
        PARTIAL_SUCCESS = 206,
        UNSPECIFIED = 500, /* Server */
        DATASOURCE_UNAVAILABLE = 503, /* Server unavailable*/
        BAD_CLIENT_REQUEST = 400, /* Client */
        NOT_FOUND = 404, /*  */
        NOT_IMPLEMENTED = 501
    }

    public partial class StandardReturType
    {
        public static string UnknownUuidText = "Unknown UUID";
        public static string DataProviderFailedText = "Data provider failed";

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
                    return code / 100 == (int)HttpErrorCode.OK / 100;
                }
            }
            return false;
        }

        public static StandardReturType OK()
        {
            return Create(HttpErrorCode.OK);
        }

        public static StandardReturType PartialSuccess(string[] failedInputs)
        {
            string text = "Partial success. Failed = " + string.Join(",", failedInputs);
            return Create(HttpErrorCode.PARTIAL_SUCCESS, text);
        }

        public static StandardReturType PartialSuccess(Dictionary<string, IEnumerable<string>> failures)
        {
            var allReasons = failures.Select(ff => string.Format("{0} : {1}", ff.Key, string.Join(",", ff.Value.ToArray()))).ToArray();
            string text = "Partial success. Failed = " + string.Join(";", allReasons);
            return Create(HttpErrorCode.PARTIAL_SUCCESS, text);
        }

        public static StandardReturType UnspecifiedError()
        {
            return Create(HttpErrorCode.UNSPECIFIED);
        }

        public static StandardReturType UnspecifiedError(string text)
        {
            return Create(HttpErrorCode.UNSPECIFIED, text);
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

        public static StandardReturType InvalidApplicationToken(string appToken)
        {
            return Create(HttpErrorCode.BAD_CLIENT_REQUEST, string.Format("Invalid application token: {0}", appToken));
        }

        public static StandardReturType InvalidUuid(string uuid)
        {
            return Create(HttpErrorCode.BAD_CLIENT_REQUEST, string.Format("Invalid UUID: {0}", uuid));
        }

        public static StandardReturType InvalidCprNumber(string cpr)
        {
            return Create(HttpErrorCode.BAD_CLIENT_REQUEST, string.Format("Invalid CPR number: {0}", cpr));
        }

        public static StandardReturType InvalidValue(string name, string val)
        {
            return Create(HttpErrorCode.BAD_CLIENT_REQUEST, string.Format("Invalid {0}: {1}", name, val));
        }
        public static StandardReturType MalformedXml()
        {
            return Create(HttpErrorCode.BAD_CLIENT_REQUEST, "Malformed XML");
        }

        public static StandardReturType RequestUnreadable(string message)
        {
            return Create(HttpErrorCode.BAD_CLIENT_REQUEST, message);
        }

        public static StandardReturType ApplicationNameExists(string applicationName)
        {
            return StandardReturType.Create(HttpErrorCode.BAD_CLIENT_REQUEST, string.Format("ApplicationName '{0}' already exists", applicationName));
        }

        public static StandardReturType UnknownUuid(string uuid)
        {
            return Create(HttpErrorCode.NOT_FOUND, string.Format("{0} : {1}", UnknownUuidText, uuid));
        }

        public static StandardReturType UnreachableChannel()
        {
            return Create(HttpErrorCode.NOT_FOUND, "NotificationChannel unreachable");
        }
    }
}
