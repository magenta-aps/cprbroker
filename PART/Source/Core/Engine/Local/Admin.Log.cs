using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CprBroker.Schemas;
using CprBroker.Engine;
using CprBroker.DAL;
using CprBroker.Utilities;

namespace CprBroker.Engine.Local
{
    public partial class Admin
    {
        /// <summary>
        /// Writes a success message (Information) to the log
        /// </summary>
        /// <param name="text"></param>
        public static void LogSuccess(string text)
        {
            AddNewLog(TraceEventType.Information, null, text, null, null);
        }

        /// <summary>
        /// Writes an exception to the log
        /// </summary>
        /// <param name="ex"></param>
        public static void LogException(Exception ex)
        {
            AddNewLog(TraceEventType.Error, ex.Source, ex.ToString(), null, null);
        }

        public static void LogCriticalException(Exception ex)
        {
            AddNewLog(TraceEventType.Critical, ex.Source, ex.ToString(), null, null);
        }

        public static void LogException(Exception ex, string moreText)
        {
            AddNewLog(TraceEventType.Error, ex.Source, string.Format("{0}{1}{2}", moreText, Environment.NewLine, ex.ToString()), null, null);
        }

        public static void LogException(Exception ex, Exception[] moreExceptions)
        {
            string[] msgs = Array.ConvertAll<Exception, string>(moreExceptions, e => e.ToString());
            AddNewLog(TraceEventType.Error, ex.Source, string.Format("{0}{1}{2}", ex.ToString(), Environment.NewLine, string.Join(Environment.NewLine, msgs)), null, null);
        }

        /// <summary>
        /// Writes a notification success to the log
        /// </summary>
        /// <param name="applicationToken"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="notificationUrl"></param>
        public static void LogNotificationSuccess(string applicationToken, Guid subscriptionId, string notificationUrl)
        {
            LogSuccess(string.Format("{0}. Application={1}; Subscription={2} sent to {3}", TextMessages.NotificationSentSuccessfully, applicationToken, subscriptionId, notificationUrl));
        }
        /// <summary>
        /// Writes a notification exception to the log
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="applicationToken"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="notificationUrl"></param>
        public static void LogNotificationException(Exception ex, string applicationToken, Guid subscriptionId, string notificationUrl)
        {
            LogException(ex, string.Format("{0}. Application={1}; Subscription={2} sent to {3}", TextMessages.NotificationSentSuccessfully, applicationToken, subscriptionId, notificationUrl));
        }

        /// <summary>
        /// Writes a new entry to the system log
        /// Other methods are used as some sort of overload fro different logging usages
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="methodName"></param>
        /// <param name="text"></param>
        /// <param name="objectType"></param>
        /// <param name="xml"></param>
        public static void AddNewLog(TraceEventType logType, string methodName, string text, string objectType, string xml)
        {
            Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry ent = new Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry();
            ent.ActivityId = BrokerContext.Current.ActivityId;
            ent.Categories.Add(Constants.Logging.Category);
            ent.Message = text;
            ent.RelatedActivityId = null;
            ent.Severity = logType;

            // Unused properties
            //ent.EventId = ;
            //ent.ManagedThreadName = ;
            //ent.Priority = ;
            //ent.Title = ;

            // TODO: Extended properties cause a problem

            Dictionary<string, object> extendedProperties = ent.ExtendedProperties as Dictionary<string, object>;
            extendedProperties[Constants.Logging.ApplicationId] = BrokerContext.Current.ApplicationId;
            extendedProperties[Constants.Logging.ApplicationName] = BrokerContext.Current.ApplicationName;
            extendedProperties[Constants.Logging.ApplicationToken] = BrokerContext.Current.ApplicationToken;
            extendedProperties[Constants.Logging.UserToken] = BrokerContext.Current.UserToken;
            extendedProperties[Constants.Logging.UserName] = BrokerContext.Current.UserName;
            extendedProperties[Constants.Logging.MethodName] = methodName;
            extendedProperties[Constants.Logging.DataObjectType] = objectType;
            extendedProperties[Constants.Logging.DataObjectXml] = xml;

            var keys = extendedProperties.Keys.ToArray();
            foreach (var key in keys)
            {
                if (extendedProperties[key] == null)
                {
                    extendedProperties[key] = "";
                }
            }

            Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(ent);
        }

    }
}
