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
    public abstract class Admin
    {
        public const string Service = @"
This service contains methods related to administrative functions of CPR Broker. Basically it is used to manage applications, data providers and query system capabilities.
";

        public const string RequestAppRegistration = @"
Creates a new un-approved application in the system.

<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfApplicationType RequestAppRegisteration(string ApplicationName)

<br><br><b><u>Parameter Description:</u></b>
<br><table>
<tr><td>ApplicationName (input):</td><td>Name of the application that is wanted to be registered. This name should be different from all application names registered in CPR Broker.</td></tr>
</table>

<br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item : The new ApplicationType object. A new application token will be assigned and passed in this object. Null on failure.

<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";

        public const string ApproveAppRegistration = @"
Approves an application's registration. This makes it possible to pass the application's token in the application header of future requests.
<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfBoolean ApproveAppRegisteration(string ApplicationToken)

<br><br><b><u>Parameter Description:</u></b>
<br><table>
<tr><td>ApplicationToken:</td><td>Token for the business client application to be approved.</td></tr>
</table>

<br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item : A boolean value specifying whether the operation has succeeded.

<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";

        public const string ListAppRegistrations = @"
Lists all registered business applications.
<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfApplicationType ListAppRegisterations()

<br><br><b><u>Parameter Description:</u></b>
<br>(none)

<br><br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item : Array of ApplicationType containing the list of applications.


<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";

        public const string UnregisterApp = @"
Unregister an application registration. An application can unregister itself, but its token would then be unusable until another application approves it.
<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfBoolean UnregisterApp(string ApplicationToken)

<br><br><b><u>Parameter Description:</u></b>
<br><table>
<tr><td>ApplicationToken (input):</td><td>Security Token for the application the should be unregistered.</td></tr>
</table>

<br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item : Boolean specifying whether the operation has succeeded.

<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";

        public const string GetCapabilities = @"
Gets a list of service operations.
<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfArrayOfServiceVersionType GetCapabilities()

<br><br><b><u>Parameter Description:</u></b>
<br>(none)

<br><br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item : Array of ServiceVersionType with the supported service list.

<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";

        public const string IsImplementing = @"
A diirect (easier) alternative to GetCapabilities().

<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfBoolean IsImplementing(string serviceName, string serviceVersion)

<br><br><b><u>Parameter Description:</u></b>
<br><table>
<tr><td>'serviceName' (input):</td><td>Service Name</td></tr>
<tr><td>'serviceVersion' (input):</td><td>Service Version</td></tr>
</table>

<br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item : A boolean value specifying whether the service is implemented.


<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";

        public const string GetDataProviderList = @"
Returns a list of objects that contain information about the external data providers that are currently used by the system.

<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfArrayOfDataProviderType GetDataProviderList()

<br><br><b><u>Parameter Description:</u></b>
<br>(none)

<br><br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item : Array of DataProviderType, sensitive information will be null.


<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";

        public const string SetDataProviderList = @"
Sets the list of data providers that are used by the service,e.g., DPR, KMD & PersonMaster. This method will overwrite all data providers currently registered in the system.

<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfBoolean SetDataProviderList(DataProviderType[] dataProviders)

<br><br><b><u>Parameter Description:</u></b>
<br><table>
<tr><td>DataProviders (input):</td><td>Array of data providers that the system should use. Existing list will be overwritten with this new list.</td></tr>
</table>

<br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item : Boolean specifying whether the operation has succeeded.

<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";

        public const string Log = @"
Writes a text string to the system's log. The text will be logged as type 'Information'.

<br><br><b><u>Signature:</u></b>
<br>BasicOutputTypeOfBoolean Log(string Text)

<br><br><b><u>Parameter Description:</u></b>
<br><table>
<tr><td>Text (input):</td><td>The text value to be logged.</td></tr>
</table>

<br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Item : Boolean specifying whether the opeation has succeeded.

<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";
    }
}
