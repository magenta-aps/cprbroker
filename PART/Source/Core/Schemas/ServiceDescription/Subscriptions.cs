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

namespace CprBroker.Schemas.ServiceDescription
{
    public static class Subscriptions
    {
        public const string Service = "This service is part of Event Broker. It allows client applications to subscribe to data events. Client applications send information on how they want to be notified of the event. This can be via a web service call or a new XML file placed at a certain directory.";

        public const string Subscribe = @"
Allows a client business application to be notified when there is a change in one or more person's data.

<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfChangeSubscriptionType Subscribe(ChannelBaseType NotificationChannel, Guid[] personUuids)

<br><br><b><u>Parameter Description:</u></b>
<br><table>
<tr><td>NotificationChannel:</td><td>Channel to send notification through it (Web service, FileShare...) .</td></tr>
<tr><td>personUuids:</td><td>Array of persons UUIDs that you want to subscribe their events. Null for all persons.</td></tr>
</table>

<br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item: An object of type ChangeSubscriptionType that represents the newly created subscription object.

<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, Event Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";
        public const string Unsubscribe = @"
Removes a subscription. The subscription can not be used any further, and will be deleted with all its notification history.

<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfBoolean Unsubscribe(Guid SubscriptionId)

<br><br><b><u>Parameter Description:</u></b>
<br><table>
<tr><td>SubscriptionId:</td><td>The UUID of the subscription to be removed.</td></tr>
</table>

<br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item: A boolean value that represents whether the operation has succeeded.

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
<tr><td>Years:</td><td>Age of the person (in years) at which the notification should be fired. Null for every year.</td></tr>
<tr><td>PriorDays:</td><td>The period before the actual birthdate event (in days) at which the notification should be fired.</td></tr>
<tr><td>personUuids:</td><td>Array of persons uuids that you want to subscribe the their events. Null for all persons.</td></tr>
</table>

<br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item: An object of type BirthdateSubscriptionType that represents the created subscription object.

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
<tr><td>SubscriptionId:</td><td>The UUID of the subscription to be removed.</td></tr>
</table>

<br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item: A boolean value that represents whether the operation has succeeded.

<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, Event Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";
        public const string SubscribeOnCriteria = @"
Subscribes to events based on criteria.
In case a municipality need to subscribe to events in their municipality code they can do that through this method.

<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfBirthdateSubscriptionType SubscribeOnBirthdate(ChannelBaseType NotificationChannel, SoegInputType1 criteria)

<br><br><b><u>Parameter Description:</u></b>
<br><table>
<tr><td>NotificationChannel:</td><td>Channel to send notification through it (Web service, FileShare...) .</td></tr>
<tr><td>criteria:</td><td>The criteria to trigger events.</td></tr>
</table>

<br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item: An object of type CriteriaSubscriptionType that represents the created subscription object.

<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, Event Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";
        public const string RemoveCriteriaSubscriptions = @"
Removes one extended subscription for a user application.

<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfBoolean RemoveCriteriaSubscription(Guid SubscriptionId)

<br><br><b><u>Parameter Description:</u></b>
<br><table>
<tr><td>SubscriptionId:</td><td>The UUID of the subscription to be removed.</td></tr>
</table>

<br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item: A boolean value that represents whether the operation has succeeded.

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
<br>Item: Array of SubscriptionType that represents the current subscriptions. Only subscriptions belonging to the caller application are retrieved.

<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, Event Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";
        
    }
}
