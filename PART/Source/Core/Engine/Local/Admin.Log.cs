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
using CprBroker.Schemas;
using CprBroker.Engine;
using CprBroker.Data;
using CprBroker.Utilities;
using log4net;
using log4net.Core;
using log4net.Util;
using System.IO;
using CprBroker.Utilities.Config;

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

        public static void LogFormattedSuccess(string text, params object[] args)
        {
            AddNewLog(TraceEventType.Information, null, string.Format(text, args), null, null);
        }

        public static void LogError(string text)
        {
            AddNewLog(TraceEventType.Error, null, text, null, null);
        }

        public static void LogFormattedError(string format, params object[] args)
        {
            AddNewLog(TraceEventType.Error, null, string.Format(format, args), null, null);
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

        public static void AddNewLog(TraceEventType logType, string methodName, string text, string objectType, string xml)
        {
            if (ConfigManager.Current.Settings.Log4NetEnabled)
                AddNewLog_Log4net(logType, methodName, text, objectType, xml);
            else
                AddNewLog_EntLib(logType, methodName, text, objectType, xml);
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(object));
        static Admin()
        {
            var mainConfigPath = ConfigUtils.GetConfigFile().FilePath;
            var path = Path.Combine(
                new FileInfo(mainConfigPath).Directory.FullName,
                string.Format("{0}", ConfigManager.Current.Settings.Log4NetConfig)
                );

            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(path));
        }

        public static void AddNewLog_Log4net(TraceEventType logType, string methodName, string text, string objectType, string xml)
        {
            var props = new PropertiesDictionary();
            foreach (var kvp in Props(methodName, objectType, xml))
                props[kvp.Key] = kvp.Value == null ? "" : kvp.Value;

            var logEvent = new LoggingEvent(new LoggingEventData()
            {
                Level = new Level((int)logType, logType.ToString()),
                Message = text,
                TimeStamp = DateTime.Now,
                UserName = BrokerContext.Current.UserName,
                ExceptionString = null,
                Domain = null,
                Identity = null,
                LocationInfo = null,
                LoggerName = null,
                Properties = props,
                ThreadName = null
            });
            log.Logger.Log(logEvent);
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
        public static void AddNewLog_EntLib(TraceEventType logType, string methodName, string text, string objectType, string xml)
        {
            // Fill default values
            if (string.IsNullOrEmpty(methodName))
                methodName = BrokerContext.Current.WebMethodMessageName;

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


            var extendedProperties = ent.ExtendedProperties as Dictionary<string, object>;
            foreach (var kvp in Props(methodName, objectType, xml))
                extendedProperties[kvp.Key] = kvp.Value;

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

        public static Dictionary<string, object> Props(string methodName, string objectType, string xml)
        {
            Dictionary<string, object> extendedProperties = new Dictionary<string, object>();
            extendedProperties[Constants.Logging.ApplicationId] = BrokerContext.Current.ApplicationId;
            extendedProperties[Constants.Logging.ActivityId] = BrokerContext.Current.ActivityId;
            extendedProperties[Constants.Logging.ApplicationName] = BrokerContext.Current.ApplicationName;
            extendedProperties[Constants.Logging.ApplicationToken] = BrokerContext.Current.ApplicationToken;
            extendedProperties[Constants.Logging.UserToken] = BrokerContext.Current.UserToken;
            extendedProperties[Constants.Logging.UserName] = BrokerContext.Current.UserName;
            extendedProperties[Constants.Logging.MethodName] = methodName;
            extendedProperties[Constants.Logging.DataObjectType] = objectType;
            extendedProperties[Constants.Logging.DataObjectXml] = xml;

            return extendedProperties;
        }

    }
}
