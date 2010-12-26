using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part
{
    public static class ServiceDescription
    {
        public static class Part
        {
            public const string Service = "Allows accesss to CPR data through a standard PART interface";
            public static class Methods
            {


                public const string Read =
@"Find and return object (Always latest registration). Looks in the local database first
Input : ObjectID
Output : Object
Parameters : EffectDate
";

                public const string RefreshRead =
@"Find and return object (Always latest registration). Looks first in the fresh data at data providers
Input : ObjectID
Output : Object
Parameters : EffectDate
";


                public const string List =
@"Find and return several objects that match the ID List supplied
Input: ID List
Output : Object List
Parameters : RegistrationDate, EffectDate
";


                public const string Search =
@"Find and return several objects that match the search criteria
Input : Search Criteria
Output : ID List
Parameters : RegistrationDate, EffectDate
";

                public const string GetPersonUuid =
@"Gets the person's UUID from his CPR number. Calls the UUID assigning authority if not found locally.
Input : CPR Number
Output : Person UUID
";
            }
        }

        public static class Subscriptions
        {
            public const string Service = "Allows client applications to subscribe to data events";

            public const string Subscribe = @"
                                                Allows a client business application to be notified when there is a change in one or more person's data

                                                <br><br><b><u>Signature:</u></b>
                                                <br>ChangeSubscriptionType Subscribe(ChannelBaseType NotificationChannel, string[] PersonCivilRegistrationIdentifiers)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>NotificationChannel (input):</td><td>Channel to send notification through it (Web service, FileShare...) .</td></tr>
                                                <tr><td>PersonCivilRegistrationIdentifiers[] (input):</td><td>Array of persons number that you want to subscribe the event to them. Null for all persons</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>ChangeSubscriptionType that represents the newly created subscription object

                                                <br><br><b><u>Notes:</u></b>
                                                <br> The component will keep track on any person that has already has been queried
                                                once. If the user creates a subscription, it will only be changes to the selected list of
                                                persons the  subscription will return. 

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string Unsubscribe = @"
                                                Removes a subscription.

                                                <br><br><b><u>Signature:</u></b>
                                                <br>bool Unsubscribe(Guid SubscriptionId)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>SubscriptionId (input):</td><td>Subscription Id that you want to remove its subscription.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return bool that represents whether the operation has succeeded.

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string SubscribeOnBirthdate = @"
                                                The user (application) can subscribe to extended birthday events.
                                                In case the business application needs to send a message to a citizen 3 weeks before
                                                the 65th. birthday (retirement), the user can subscribe to this event 65 birthday
                                                minus 3 weeks. This subscription can be created for all persons or for a specific list of persons. 
                                                It is possible to create several subscriptions
                                                from the same user application to the same person or list of indiviuals.

                                                <br><br><b><u>Signature:</u></b>
                                                <br>BirthdateSubscriptionType SubscribeOnBirthdate(ChannelBaseType NotificationChannel, Nullable<int> Years, int PriorDays, string[] PersonCivilRegistrationIdentifiers)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>NotificationChannel (input):</td><td>Channel to send notification through it (Web service, FileShare...) .</td></tr>
                                                <tr><td>Years (input):</td><td>Years.</td></tr>
                                                <tr><td>PriorDays (input):</td><td>Prior days.</td></tr>
                                                <tr><td>PersonCivilRegistrationIdentifiers[] (input):</td><td>Array of persons number that you want to subscribe the event to them.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>BirthdateSubscriptionType that represents the created subscription object.

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string RemoveBirthDateSubscriptions = @"
                                                Removes one extended subscription for a user application.

                                                <br><br><b><u>Signature:</u></b>
                                                <br>bool RemoveBirthDateSubscription(Guid SubscriptionId)

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>'SubscriptionId' (input):</td><td>Subscription Id that you want to remove its subscription.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>return bool that represents whether the operation has succeeded.

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string GetActiveSubscriptionsList = @"
                                                Allows a business application to get a list of all subscriptions

                                                <br><br><b><u>Signature:</u></b>
                                                <br>SubscriptionType[] GetActiveSubsciptionsList()

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>Array of SubscriptionType that represents the current subscriptions

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
            public const string GetLatestNotification = @"
                                                Allows a business application to get the latest notification that has been fired for a given subscription
                                                <br>Usually called as a callback after a notification event is fired

                                                <br><br><b><u>Signature:</u></b>
                                                <br>SubscriptionType[] GetLatestNotification()

                                                <br><br><b><u>Parameter Description:</u></b>
                                                <br><table>
                                                <tr><td>'SubscriptionId' (input):</td><td>Subscription Id for which the latest notification is required.</td></tr>
                                                </table>

                                                <br><b><u>Return Value:</u></b>
                                                <br>BaseNotificationType that represents the latest notification fired for the subscription

                                                <br><br><b><u>Review:</u></b>
                                                <table>
                                                <tr><td width='30%'>2009-08-20, CPR Broker </td><td width='10%'></td><td width='60%'>First release.</td></tr>
                                                </table>
                                                <br>==============================
                                                ";
        }

        public static class NotificationQueue
        {
            public const string Service = "Allows the event broker to be notified that some event has occured";

            public const string Enqueue = @"
                                                Allows the event broker to be notified that a person's data has changed                                                
                                                ";
        }
    }
}
