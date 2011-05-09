/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 *
 * The Initial Developer of the Original Code is
 * IT- og Telestyrelsen / Danish National IT and Telecom Agency.
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
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
        public const string Service = "Allows accesss to CPR data through a standard PART interface";
        public static class Methods
        {


            public const string Read =
@"Find and return object (Always latest registration). Looks in the local database first
<br><br><b><u>Signature:</u></b>
<br>LaesOutputType Read(LaesInputType input)
<br><br><b><u>Parameter Description:</u></b>
<br>input: Contains the person UUID and (optionally) requested registration & effect date ranges
<br><br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>LaesResultat: Read operation result.
<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";

            public const string RefreshRead =
@"Reads person information from external data sources
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
@"Find and return several objects that match the ID List supplied
<br><br><b><u>Signature:</u></b>
<br>ListOutputType1 List(ListInputType input)
<br><br><b><u>Parameter Description:</u></b>
<br>input: List of person UUIDs to get
<br><br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>LaesResultat: Array of Read operation results.
<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";


            public const string Search =
@"Searches the local database for matching persons.
<br><br><b><u>Signature:</u></b>
<br>SoegOutputType Search(SoegInputType1 searchCriteria)
<br><br><b><u>Parameter Description:</u></b>
<br>searchCriteria: The search criteria. Only CPR numbers and name are implemented.
<br><br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>Idliste: List of UUID's of the found persons.
<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";

            public const string GetUuid =
@"Gets the person's UUID from his CPR number. Calls the UUID assignment authority (PersonMaster service) if not found locally.
<br><br><b><u>Signature:</u></b>
<br>GetUuidOutputType GetUuid(string cprNumber)
<br><br><b><u>Parameter Description:</u></b>
<br>cprNumber: CPR number of person needed
<br><br><b><u>Return value copmponents:</u></b>
<br>StandardRetur: Detailed status code and text.
<br>UUID: The person's UUID.
<br><br><b><u>Review:</u></b>
<table>
<tr><td width='30%'>2011-03-06, CPR Broker </td><td width='10%'></td><td width='60%'>Part release.</td></tr>
</table>
<br>==============================
";
        }
    }
}

