using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Engine
{
    /// <summary>
    /// Contains text messages that are logged to the system's log
    /// </summary>
    public static class TextMessages
    {
        public static readonly string NotificationSentSuccessfully = "Notification sent successfully";
        public static readonly string NotificationSendingFailed = "Notification sending failed";
        public static readonly string NoDataProvidersFound = "No data providers found";
        public static readonly string AllDataProvidersFailed = "All available data providers failed";

        public static readonly string BackendServiceStarted = "CPR Broker backend service has been started";
        public static readonly string BackendTimerEventStarted = "CPR Broker backend service timer has started";
        public static readonly string BackendTimerEventFinished = "CPR Broker backend service timer has just finished";

        public static readonly string UuidNotFound =  "No such UUID found";
    }
}
