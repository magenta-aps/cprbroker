using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using CprBroker.DAL.Applications;
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
                    DAL.Applications.LogEntry dbLogEntry = new DAL.Applications.LogEntry();

                    dbLogEntry.LogEntryId = Guid.NewGuid();
                    dbLogEntry.LogTypeId = (int)logEntry.Severity;
                    dbLogEntry.ApplicationId = BrokerContext.Current.ApplicationId;
                    dbLogEntry.UserToken = BrokerContext.Current.UserToken;
                    dbLogEntry.UserId = BrokerContext.Current.UserName;

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
