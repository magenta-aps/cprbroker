using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.ServiceDescription
{
    public static class Subscriptions
    {
        public const string Service = "Allows client applications to subscribe to data events";

        public const string Subscribe = @"
Allows a client business application to be notified when there is a change in one or more person's data

<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfChangeSubscriptionType Subscribe(ChannelBaseType NotificationChannel, Guid[] personUuids)

<br><br><b><u>Parameter Description:</u></b>
<br><table>
<tr><td>NotificationChannel:</td><td>Channel to send notification through it (Web service, FileShare...) .</td></tr>
<tr><td>personUuids:</td><td>Array of persons UUIDs that you want to subscribe their events. Null for all persons</td></tr>
</table>

<br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item: ChangeSubscriptionType that represents the newly created subscription object

<br><br><b><u>Notes:</u></b>
<br> The component will keep track on any person that has already has been queried
once.

<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, Event Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";
        public const string Unsubscribe = @"
Removes a subscription.

<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfBoolean Unsubscribe(Guid SubscriptionId)

<br><br><b><u>Parameter Description:</u></b>
<br><table>
<tr><td>SubscriptionId:</td><td>Subscription Id that you want to remove its subscription.</td></tr>
</table>

<br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item: boolean that represents whether the operation has succeeded.

<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, Event Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";
        public const string SubscribeOnBirthdate = @"
Subscribes to extended birthday events.
In case the business application needs to send a message to a citizen 3 weeks before
the 65th. birthday (retirement), the user can subscribe to this event 65 birthday
minus 3 weeks. This subscription can be created for all persons or for a specific list of persons.

<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfBirthdateSubscriptionType SubscribeOnBirthdate(ChannelBaseType NotificationChannel, Nullable<int> Years, int PriorDays, Guid[] personUuids)

<br><br><b><u>Parameter Description:</u></b>
<br><table>
<tr><td>NotificationChannel:</td><td>Channel to send notification through it (Web service, FileShare...) .</td></tr>
<tr><td>Years:</td><td>Years.</td></tr>
<tr><td>PriorDays:</td><td>Prior days.</td></tr>
<tr><td>personUuids:</td><td>Array of persons uuids that you want to subscribe the their events. Null for all persons</td></tr>
</table>

<br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item: BirthdateSubscriptionType that represents the created subscription object.

<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, Event Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";
        public const string RemoveBirthDateSubscriptions = @"
Removes one extended subscription for a user application.

<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfBoolean RemoveBirthDateSubscription(Guid SubscriptionId)

<br><br><b><u>Parameter Description:</u></b>
<br><table>
<tr><td>SubscriptionId:</td><td>Subscription Id that you want to remove its subscription.</td></tr>
</table>

<br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item: boolean that represents whether the operation has succeeded.

<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, Event Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";
        public const string GetActiveSubscriptionsList = @"
Allows a business application to get a list of all subscriptions

<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfArrayOfSubscriptionType GetActiveSubsciptionsList()

<br><br><b><u>Parameter Description:</u></b>
<br>(none)

<br><br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item: Array of SubscriptionType that represents the current subscriptions

<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, Event Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";
        
    }
}
