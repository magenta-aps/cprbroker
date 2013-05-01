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
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Util;
using System.Linq.Expressions;

namespace CprBroker.Providers.DPR
{
    /// <summary>
    /// Contains code for access of DPR diversion
    /// </summary>
    public abstract class ClientDataProvider : BaseProvider
    {
        public enum InquiryType
        {
            DataNotUpdatedAutomatically = 0,
            DataUpdatedAutomaticallyFromCpr = 1,
            DeleteAutomaticDataUpdateFromCpr = 3
        }

        public enum DetailType
        {
            MasterData = 0,
            ExtendedData = 1
        }

        static ClientDataProvider()
        {
            ErrorCodes["01"] = "PNR unknown in CPR";
            ErrorCodes["02"] = "UserID / pwd is not correct";
            ErrorCodes["03"] = "pwd expired NEWPWD required";
            ErrorCodes["04"] = "NEWPWD does not meet the format" + Environment.NewLine + "(6-8 digits and letters and not previously used) ";
            ErrorCodes["05"] = "Not access to CPR";
            ErrorCodes["06"] = "Unknown bruid";
            ErrorCodes["07"] = "Timeout - New LOGON necessary";
            ErrorCodes["08"] = "DEAD-LOCK when read in the CPR system";
            ErrorCodes["09"] = "Errors in CPR's reply application. Contact supplier";
            ErrorCodes["20"] = "Unforeseen error in database update";
            ErrorCodes["21"] = "Login information for database missing";
            ErrorCodes["22"] = "Error login to database";
            ErrorCodes["30"] = "Error connection to CPR." + Environment.NewLine + "Check the port number and IP address ";
            ErrorCodes["31"] = "Communication Error";
        }

        /// <summary>
        /// Ensures that the DPR database contains the given person
        /// </summary>
        /// <param name="cprNumber"></param>
        protected void EnsurePersonDataExists(string cprNumber)
        {
            if (!this.DisableDiversion)
            {
                decimal cprNum = Convert.ToDecimal(cprNumber);
                using (DPRDataContext dataContext = new DPRDataContext(ConnectionString))
                {
                    var exists = (from personTotal in dataContext.PersonTotals
                                  select personTotal.PNR).Contains(cprNum);

                    if (exists)
                    {
                        Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, "EnsurePersonDataExists", string.Format("PNR {0} Exists in DPR, DPR Diversion not called", cprNumber), null, null);
                    }
                    else
                    {
                        Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, "EnsurePersonDataExists", string.Format("Calling DPR Diversion : {0}", cprNumber), null, null);
                        CallDiversion(InquiryType.DataUpdatedAutomaticallyFromCpr, DetailType.ExtendedData, cprNumber);
                    }
                }
            }
            else
            {
                Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, "EnsurePersonDataExists", string.Format("DPR Diversion is disabled: {0}", cprNumber), null, null);
            }
        }

        public bool IsDiversionAlive()
        {
            System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
            try
            {
                if (!DisableDiversion)
                {
                    // Try to open a socket on the server
                    // TODO: Do more complex testing for Write() and Read()
                    client.Connect(this.Address, this.Port);
                    client.GetStream().Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                client.Close();
            }
        }

        public bool IsDatabaseAlive()
        {
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
            try
            {
                conn.ConnectionString = this.ConnectionString;
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public override bool IsAlive()
        {
            return IsDiversionAlive() && IsDatabaseAlive();
        }

        public string CreateMessage(InquiryType inquiryType, DetailType detailType, string cprNumber)
        {
            return string.Format("{0}{1}{2}",
                 (int)inquiryType,
                 (int)detailType,
                 cprNumber
             );
        }

        public string CallDiversion(InquiryType inquiryType, DetailType detailType, string cprNumber)
        {
            string message = CreateMessage(inquiryType, detailType, cprNumber);
            return Send(message);
        }

    }
}

