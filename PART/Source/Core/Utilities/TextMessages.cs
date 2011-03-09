﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Utilities
{
    /// <summary>
    /// Text messages used by the broker
    /// </summary>
    public static class TextMessages
    {
        public static readonly string NameOrTokenAlreadyExists = "Application name or token already exists";
        public static readonly string CannotDeleteApplicationBecauseItHasSubscriptions = "Cannot delete application because it has subscriptions";

        public static readonly string NotificationSentSuccessfully = "Notification sent successfully";
        public static readonly string NotificationSendingFailed = "Notification sending failed";
        public static readonly string InvalidInput = "Invalid input";
        public static readonly string NoDataProvidersFound = "No data providers found";
        public static readonly string AllDataProvidersFailed = "All available data providers failed";
        public static readonly string ResultGatheringFailed = "Result gathering has failed";

        public static string Succeeded = "Succeeded";

        public static readonly string BackendServiceStarting = "CPR Broker backend service is starting";
        public static readonly string BackendServiceStarted = "CPR Broker backend service has been started";
        public static readonly string TimerEventStarted = "Timer event started";
        public static readonly string TimerEventFinished = "Timer event finished";

        public static readonly string UuidNotFound = "No such UUID found";
    }
}
