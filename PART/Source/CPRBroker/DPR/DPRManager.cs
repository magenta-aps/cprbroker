using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace CPRBroker.Providers.DPR
{
    public class DPRManager
    {
        public static string Logon(string customer, string userId)
        {
            string message = GenerateRequest(Variables.TransactionCode, Variables.Comma, customer, ((int)Enums.SubscriptionType.LogonTransaction).ToString(),
                ((int)Enums.ReturnedDataType.SimpleData).ToString(), userId, Variables.Password, Variables.NewPassword);
            string response = SendRequestAndGetResponse(Variables.Server, Variables.Port, message);
            // get error code
            string errorCode = response.Substring(22, 2);
            if(errorCode == ((Enums.EnumInfoAttribute)(typeof(Enums.ErrorCode).GetField(Enums.ErrorCode.NoError.ToString()).GetCustomAttributes(true)[0])).EnumDescription)
            {
                // return the token if the errorcode is "No Error"
                return response.Substring(6, 8);
            }
            return string.Empty;
        }

        /// <summary>
        /// Take the input parameter and generate the request string that will be send to the server
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GenerateRequest(params string[] parameters)
        {
            StringBuilder requestData = new StringBuilder();
            Array.ForEach<string>(parameters, new Action<string>(delegate(string param)
            {
                requestData.Append(param);
            }));
            return requestData.ToString();
        }

        /// <summary>
        /// Send the message to the specified server and get the response
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string SendRequestAndGetResponse(string server, int port, string message)
        {
            string responseData = string.Empty;
            TcpClient client = null;
            NetworkStream stream = null;
            try
            {
                client = new TcpClient(server, port);
                Byte[] data = System.Text.Encoding.UTF7.GetBytes(message);

                stream = client.GetStream();
                stream.Write(data, 0, data.Length);

                data = new Byte[3500];

                int bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.UTF7.GetString(data, 0, bytes);
            }
            catch (ArgumentNullException e)
            {
                responseData = "ArgumentNullException: {0} " + e;
            }
            catch (SocketException e)
            {
                responseData = "SocketException: {0} " + e;
            }
            return responseData;
        }

        public class Variables
        {
            public static readonly string TransactionCode = "PDPR";
            public static readonly string Comma = ",";
            public static readonly string Customer = "9999";
            public static readonly string Password = "PWD";
            public static readonly string NewPassword = "NEWPWD";
            public static readonly int Port = 6003;
            public static readonly string Server = "10.10.1.178";
        }

        public class Enums
        {
            public enum SubscriptionType
            {
                NotUpdatedAutomaticallyFromCPR=0,
                UpdatedAutomaticallyFromCPR=1,
                SubscriptionCanceled=3,
                LogonTransaction=9
            }
            public enum ReturnedDataType
            {
                SimpleData=0,
                ExtendedData=1
            }
            public enum ErrorCode
            {
                [EnumInfo(EnumDescription="00")]
                NoError = 00,
                [EnumInfo(EnumDescription = "01")]
                UserIDORPWDNotTrue = 01,
                [EnumInfo(EnumDescription = "02")]
                PWDExpiredNEWPWDCalled = 02,
                [EnumInfo(EnumDescription = "03")]
                NewPWDDoesNotMeetTheFormat = 03,
                [EnumInfo(EnumDescription = "04")]
                CannotAccessCPR = 04,
                [EnumInfo(EnumDescription = "05")]
                PNRUnknownInCPR = 05,
                [EnumInfo(EnumDescription = "06")]
                UnknownCustomer = 06,
                [EnumInfo(EnumDescription = "07")]
                TimeoutNewLogonNeeded = 07,
                [EnumInfo(EnumDescription = "08")]
                DeadlockWhenReadingCPRSystem = 08,
                [EnumInfo(EnumDescription = "09")]
                ServerProblems = 09,
                [EnumInfo(EnumDescription = "10")]
                SubscriptionTypeUnknown = 10,
                [EnumInfo(EnumDescription = "11")]
                DataTypeUnknown = 11,
                [EnumInfo(EnumDescription = "12")]
                ReservedErrorNumbers = 12 | 99    
            }
            public class EnumInfoAttribute : Attribute
            {
                public string EnumDescription
                {
                    get;
                    set;
                }
            }
        }
    }
}
