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
    public static class Part
    {
        public const string Service = "This is the main service of CPR Broker. It allows access to CPR data through a standard PART interface.  Methods of this service are executed via local data provider whenever possible (except RefreshRead). Otherwise, an external data provider is used to implement the request.";
        public static class Methods
        {


            public const string Read =
@"Finds and returns a single person object. It will return the latest registration within the specified range. It first looks in the local database, and attempts external data providers if no data is found locally.
<br><br><b><u>Signature:</u></b>
<br>LaesOutputType Read(LaesInputType input)
<br><br><b><u>Parameter Description:</u></b>
<br>input: Contains the person UUID and (optionally) requested registration & effect date ranges.
<br><br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>LaesResultat: If succeeded, its Item property will be a Registrering object with the found person registration. 
<br>If failed, contains null.
<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";

            public const string RefreshRead =
@"This method is the same as Read, except that it does not look in the broker's local database to load the data. Instead, it queries external data providers directly for the data.
<br><br><b><u>Signature:</u></b>
<br>LaesOutputType RefreshRead(LaesInputType input)
<br><br><b><u>Parameter Description:</u></b>
<br>input: Contains the person UUID and (optionally) requested registration & effect date ranges. Only the UUID matters here.
<br><br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>LaesResultat: Read operation result.
<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";


            public const string List =
@"Finds and returns several person objects that match the ID List supplied. Just like Read, every person is first attempted locally, and if not found, an external data provider is attempted.
<br><br><b><u>Signature:</u></b>
<br>ListOutputType1 List(ListInputType input)
<br><br><b><u>Parameter Description:</u></b>
<br>input: Contains the UUIDs of the person objects to be retrieved.
<br><br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>LaesResultat: Array of Read operation results. In case of full or partial success, each element at index i in the array corresponds to the UUID at index i in the input array. Contains null in case of failure.
<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
<tr><td width='30%'>2011-11-29, CPR Broker </td><td width='10%'></td><td width='60%'>Allowed partial success.</td></tr>
</table>
<br>==============================
";


            public const string Search =
@"Searches the local database for matching persons. The current implementation can only search by UUID, CPR number, first name, middle  name or last name. Search is performed for whole words only.
<br><br><b><u>Signature:</u></b>
<br>SoegOutputType Search(SoegInputType1 searchCriteria)
<br><br><b><u>Parameter Description:</u></b>
<br>searchCriteria: The search criteria. 
<br><br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Idliste: List of UUID's of the found persons. You can then call Read or List to get detailed data.
<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";

            public const string GetUuid =
@"Gets the person's UUID from his CPR number. If no mapping is found locally, it calls the UUID assignment authority (PersonMaster service).
<br><br><b><u>Signature:</u></b>
<br>GetUuidOutputType GetUuid(string cprNumber)
<br><br><b><u>Parameter Description:</u></b>
<br>cprNumber: CPR number of person needed.
<br><br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>UUID: The person's UUID.
<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";

            public const string GetUuidArray =
@"Gets persons' UUIDs from their CPR numbers. If no mapping is found locally, it calls the UUID assignment authority (PersonMaster service).
<br><br><b><u>Signature:</u></b>
<br>GetUuidOutputType GetUuid(string[] cprNumberArray)
<br><br><b><u>Parameter Description:</u></b>
<br>cprNumberArray: CPR numbers of persons needed.
<br><br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>UUID: The persons' UUIDs.
<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2012-11-25, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";
        }
    }
}

