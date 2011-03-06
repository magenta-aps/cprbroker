using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.ServiceDescription
{
    public static class Notification
    {
        public const string Service = "Template for a notification listener web service";

        public const string Notify = @"
Called to tell the client system that a notification has been fired
This instance creates a new entry inthe system log.

<br><br><b><u>Signature:</u></b>
<br>void Notify(Schemas.Part.Events.CommonEventStructureType notification)

<br><br><b><u>Parameter Description:</u></b>
<br><table>
<tr><td>notification:</td><td>The notification object</td></tr>
</table>

<br>==============================
";

        public const string Ping = @"            
Called to make sure the service is online

<br><br><b><u>Signature:</u></b>
<br>void Ping()

<br>==============================
";


    }
}
