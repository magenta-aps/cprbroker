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
    public abstract class ClientDataProvider : BaseProvider
    {
        enum InquiryType
        {
            DataNotUpdatedAutomatically = 0,
            DataUpdatedAutomaticallyFromCpr = 1,
            DeleteAutomaticDataUpdateFromCpr = 3
        }

        enum DetailType
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
            // TODO: include BirthDate in the search
            decimal cprNum = Convert.ToDecimal(cprNumber);
            using (DPRDataContext dataContext = new DPRDataContext(ConnectionString))
            {
                var exists = (from personName in dataContext.PersonNames
                              select personName.PNR).Contains(cprNum);

                if (!exists)
                {
                    GetPersonData(InquiryType.DataUpdatedAutomaticallyFromCpr, DetailType.ExtendedData, cprNumber);
                    // TODO: make sure that deleting the subscription is a good decision
                    GetPersonData(InquiryType.DeleteAutomaticDataUpdateFromCpr, DetailType.ExtendedData, cprNumber);
                }
            }
        }

        public override bool IsAlive()
        {
            // Try to open a socket on the server
            System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
            try
            {
                // TODO : Put a maximum timeout for DPR connection
                client.Connect(this.Address, this.Port);
                client.GetStream().Close();
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
                client.Close();
                conn.Close();
            }
        }

        private string CreateMessage(InquiryType inquiryType, DetailType detailType, string cprNumber)
        {
            return string.Format("{0}{1}{2}",
                 (int)inquiryType,
                 (int)detailType,
                 cprNumber
             );
        }

        private string GetPersonData(InquiryType inquiryType, DetailType detailType, string cprNumber)
        {
            string message = CreateMessage(inquiryType, detailType, cprNumber);
            return Send(message);
        }

    }
}

