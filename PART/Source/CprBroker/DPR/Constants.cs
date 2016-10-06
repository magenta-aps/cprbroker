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
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.DPR
{
    /// <summary>
    /// Contains constants that are used in DPR
    /// </summary>
    public class Constants
    {
        public static UnikIdType Actor
        {
            get
            {
                return UnikIdType.Create(new Guid("{4A953CF9-B4C1-4ce9-BF09-2BF655DC61C7}"));
            }
        }


        public const string CprNationalityKmdCode = "5001";
        public const string StatelessKmdCode = "5103";
        public const string DenmarkKmdCode = "5100";

        public const string DiversionOperationName = "Diversion";
        public static readonly Encoding DiversionEncoding = Encoding.UTF7;
    }

    public enum InquiryType
    {
        DataNotUpdatedAutomatically = 0,
        DataUpdatedAutomaticallyFromCpr = 1,
        DeleteAutomaticDataUpdateFromCpr = 3
    }

    public enum DetailType
    {
        MasterData = 0, // Only to client
        ExtendedData = 1 // Put to DPR database
    }

    public class ResponseType
    {
        /// <summary>
        /// Ingen
        /// </summary>
        public const char None = 'I';

        /// <summary>
        /// Stam
        /// </summary>
        public const char Basic = 'S';

        /// <summary>
        /// Udvidede
        /// </summary>
        public const char Enriched = 'U';
    }

    public class EvenOdd
    {
        public const char Even = 'L';
        public const char Odd = 'U';
    }

    public class DataRetrievalTypes
    {
        public const char Extract = '*'; // also 'U' or null
        public const char Extract2 = 'U'; // alternative
        public const char CprDirectWithoutSubscription = 'I';
        public const char CprDirectWithSubscription = 'D';
        public const char CprDirectWithDeletedSubscription = 'R';
    }

    public class UpdatingProgram
    {
        public const char DprUpdate = 'A';
        public const char DprDiversion = 'V';
    }

    public class DiversionErrorCodes
    {
        private static Dictionary<string, string> _ErrorCodes_Dk = new Dictionary<string, string>();
        public static Dictionary<string, string> ErrorCodes_Dk()
        {
            return _ErrorCodes_Dk;
        }

        private static Dictionary<string, string> _ErrorCodes_En = new Dictionary<string, string>();
        public static Dictionary<string, string> ErrorCodes_En()
        {
            return _ErrorCodes_En;
        }

        static DiversionErrorCodes()
        {
            _ErrorCodes_En["01"] = "PNR unknown in CPR";
            _ErrorCodes_En["02"] = "UserID / pwd is not correct";
            _ErrorCodes_En["03"] = "pwd expired NEWPWD required";
            _ErrorCodes_En["04"] = "NEWPWD does not meet the format" + Environment.NewLine + "(6-8 digits and letters and not previously used) ";
            _ErrorCodes_En["05"] = "No access to CPR";
            _ErrorCodes_En["06"] = "Unknown bruid";
            _ErrorCodes_En["07"] = "Timeout - New LOGON necessary";
            _ErrorCodes_En["08"] = "DEAD-LOCK when read in the CPR system";
            _ErrorCodes_En["09"] = "Errors in CPR's reply application. Contact supplier";
            _ErrorCodes_En["10"] = "Unknown subscription type";
            _ErrorCodes_En["11"] = "Unknown data type";
            _ErrorCodes_En["16"] = "Incorrect IP Address";
            _ErrorCodes_En["20"] = "Unforeseen error in database update";
            _ErrorCodes_En["21"] = "Login information for database missing";
            _ErrorCodes_En["22"] = "Error login to database";
            _ErrorCodes_En["30"] = "Error connection to CPR." + Environment.NewLine + "Check the port number and IP address ";
            _ErrorCodes_En["31"] = "Communication Error";
            _ErrorCodes_En["40"] = "Incorrect PNR. Call rejected";
            //ErrorCodes_En["41"] = "Person data already updated. Call rejected"; -- Not an error
            _ErrorCodes_En["42"] = "New downloaded in advance. Call rejected.";
            _ErrorCodes_En["43"] = "Subscription call without data retrieval. Call rejected.";


            _ErrorCodes_Dk["01"] = "PNR ukendt i CPR";
            _ErrorCodes_Dk["02"] = "USERID/PWD ikke korrekt";
            _ErrorCodes_Dk["03"] = "pwd expired NEWPWD required";
            _ErrorCodes_Dk["04"] = "PWD udløbet, NEWPWD krævet" + Environment.NewLine + "(6-8 tal og bogstaver og ikke tidligere brugt)";
            _ErrorCodes_Dk["05"] = "Ikke adgang til CPR";
            _ErrorCodes_Dk["06"] = "Ukendt BRUID";
            _ErrorCodes_Dk["07"] = "Timeout - ny LOGON nødvendig";
            _ErrorCodes_Dk["08"] = "DEAD - LOCK ved læs i CPR-systemet";
            _ErrorCodes_Dk["09"] = "Fejl i CPR's svarprogram. Kontakt leverandøren";
            _ErrorCodes_Dk["10"] = "ABON_TYPE ukendt";
            _ErrorCodes_Dk["11"] = "DATA_TYPE ukendt";
            _ErrorCodes_Dk["16"] = "IP-adressen forkert";
            _ErrorCodes_Dk["20"] = "Uforudset fejl ved databaseopdatering";
            _ErrorCodes_Dk["21"] = "Loginoplysninger til database mangle";
            _ErrorCodes_Dk["22"] = "Fejl ved login til database";
            _ErrorCodes_Dk["30"] = "Fejl ved forbindelse til CPR" + Environment.NewLine + "Kontroller portnummer og IP-adresse";
            _ErrorCodes_Dk["31"] = "Kommunikationsfejl";
            _ErrorCodes_Dk["40"] = "Personnummer forkert opbygget. Kald afvist";
            //ErrorCodes_Dk["41"] = " Personens data i forvejen opdaterede. Kald afvist"; -- Not an error
            _ErrorCodes_Dk["42"] = "Ny er hentet i forvejen. Kald afvist";
            _ErrorCodes_Dk["43"] = "Abonnementsætning uden datahentning. Kald afvist";
        }
    }
}
