using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;
using CprBroker.Data.Applications;
using CprBroker.Utilities;

namespace CprBroker.Engine.Trace
{
    public class DatabaseAppender : BufferingAppenderSkeleton
    {
        protected override void SendBuffer(LoggingEvent[] events)
        {
            using (var tracsactionScope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Suppress))
            {
                using (ApplicationDataContext context = new ApplicationDataContext())
                {
                    var dbEntries = events.Select(logEntry =>
                    {
                        Data.Applications.LogEntry dbLogEntry = new Data.Applications.LogEntry();

                        dbLogEntry.LogEntryId = Guid.NewGuid();
                        dbLogEntry.LogTypeId = logEntry.Level.Value; // Change
                        dbLogEntry.ApplicationId = (Guid) logEntry.LookupProperty(Constants.Logging.ApplicationId);
                        dbLogEntry.UserToken = (string)logEntry.LookupProperty(Constants.Logging.UserToken);
                        dbLogEntry.UserId = (string)logEntry.LookupProperty(Constants.Logging.UserName) ;
                        dbLogEntry.ActivityID = (Guid)logEntry.LookupProperty(Constants.Logging.ActivityId);

                        dbLogEntry.MethodName = Strings.ObjectToString(logEntry.LookupProperty(Constants.Logging.MethodName)); //Change
                        dbLogEntry.Text = logEntry.RenderedMessage; // Change
                        
                        dbLogEntry.DataObjectType = Strings.ObjectToString(logEntry.LookupProperty(Constants.Logging.DataObjectType)); //Change
                        dbLogEntry.DataObjectXml = Strings.ObjectToString(logEntry.LookupProperty(Constants.Logging.DataObjectXml)); //Change
                        dbLogEntry.LogDate = logEntry.TimeStamp;

                        return dbLogEntry;
                    });

                    context.LogEntries.InsertAllOnSubmit(dbEntries);
                    context.SubmitChanges();
                }
            }
        }
    }
}
