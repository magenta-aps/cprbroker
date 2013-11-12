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
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using CprBroker.Data.Applications;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using CprBroker.Utilities;

namespace CprBroker.Engine.Trace
{
    /// <summary>
    /// Writes log messages to the system's database
    /// </summary>
    public class LocalTraceListener : CustomTraceListener
    {
        public override void Write(string message)
        {
            Debug.Write(message);
        }

        public override void WriteLine(string message)
        {
            Debug.WriteLine(message);
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry logEntry = data as Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry;
            if (logEntry != null)
            {
                using (ApplicationDataContext context = new ApplicationDataContext())
                {
                    Data.Applications.LogEntry dbLogEntry = new Data.Applications.LogEntry();

                    dbLogEntry.LogEntryId = Guid.NewGuid();
                    dbLogEntry.LogTypeId = (int)logEntry.Severity;
                    dbLogEntry.ApplicationId = BrokerContext.Current.ApplicationId;
                    dbLogEntry.UserToken = BrokerContext.Current.UserToken;
                    dbLogEntry.UserId = BrokerContext.Current.UserName;
                    dbLogEntry.ActivityID = BrokerContext.Current.ActivityId;

                    dbLogEntry.MethodName = Strings.ObjectToString(logEntry.ExtendedProperties[Constants.Logging.MethodName]);
                    dbLogEntry.Text = logEntry.Message;

                    dbLogEntry.DataObjectType = Strings.ObjectToString(logEntry.ExtendedProperties[Constants.Logging.DataObjectType]);
                    dbLogEntry.DataObjectXml = Strings.ObjectToString(logEntry.ExtendedProperties[Constants.Logging.DataObjectXml]);
                    dbLogEntry.LogDate = logEntry.TimeStamp;
                    

                    context.LogEntries.InsertOnSubmit(dbLogEntry);
                    context.SubmitChanges();
                }
            }
        }
    }
}
