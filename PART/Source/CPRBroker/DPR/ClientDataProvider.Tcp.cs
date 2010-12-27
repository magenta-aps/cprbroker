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
    public partial class ClientDataProvider : BaseProvider, IPersonNameAndAddressDataProvider, IPersonBasicDataProvider, IPersonFullDataProvider, IPersonChildrenDataProvider, IPersonRelationsDataProvider
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

        public override bool IsAlive()
        {
            // Try to open a socket on the server
            System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
            try
            {
                // TODO : Put a maximum timeout for DPR connection
                client.Connect(this.DatabaseObject.Address, this.DatabaseObject.Port.Value);
                client.GetStream().Close();
                conn.ConnectionString = this.DatabaseObject.ConnectionString;
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

        [Obsolete]
        protected override string TestResponse
        {
            get
            {
                return "11471926741444Rkzmqvyjtyt                             Jpgdu                                                                               Sgk S?wuscdsxnbg                        579D485671    3744Btjaoxuwbsoxr       2005197001";
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

