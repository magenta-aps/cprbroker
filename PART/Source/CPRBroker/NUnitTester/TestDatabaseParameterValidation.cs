using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.SetupDatabase;

namespace NUnitTester
{
    class TestDatabaseParameterValidation
    {
        public static void Test()
        {
            SetupInfo info = new SetupInfo();

            var serverNames = new string[] { "10.20.1.20" };
            var databaseNames = new string[] { "CPRBroker", "klajfdkljq" };
            //var sameAs = new bool[] { true, false };
            var sameAs = new bool[] { false };

            var adminIntegrated = new bool[] { true, false };
            //var adminIntegrated = new bool[] { false };

            var adminUsers = new string[] { "sa", "hjkk", "cpr" };
            //var adminUsers = new string[] { "sa" };

            var adminPasswords = new string[] { "Dlph10t", "skldjfkl", "cpr" };
            //var adminPasswords = new string[] { "Dlph10t"};

            var appIntegrated = new bool[] { true, false };
            //var appIntegrated = new bool[] { false };

            var appUsers = new string[] { "cpr", "hjkk" };
            //var appUsers = new string[] { "cpr", "hjkk" };

            var appPasswords = new string[] { "cpr", "skldjfkl" };
            Console.WriteLine("Start");

            string lastAdminConn = "";
            string lastAppConn = "";

            foreach (var serverName in serverNames)
            {
                info.ServerName = serverName;

                foreach (var dbName in databaseNames)
                {
                    info.DatabaseName = dbName;

                    foreach (var sAs in sameAs)
                    {
                        info.ApplicationAuthenticationSameAsAdmin = sAs;

                        foreach (var adminInt in adminIntegrated)
                        {
                            info.AdminAuthenticationInfo.IntegratedSecurity = adminInt;

                            foreach (var adminUser in adminUsers)
                            {
                                info.AdminAuthenticationInfo.UserName = adminUser;

                                foreach (var adminPassword in adminPasswords)
                                {
                                    info.AdminAuthenticationInfo.Password = adminPassword;

                                    foreach (var appInt in appIntegrated)
                                    {
                                        info.ApplicationAuthenticationInfo.IntegratedSecurity = appInt;

                                        foreach (var appUser in appUsers)
                                        {
                                            info.ApplicationAuthenticationInfo.UserName = appUser;

                                            foreach (var appPassword in appPasswords)
                                            {
                                                info.ApplicationAuthenticationInfo.Password = appPassword;

                                                string message = "";
                                                var result = info.Validate(ref message);

                                                string adminConn = info.CreateConnectionString(true, true);
                                                string appConn = info.CreateConnectionString(false, true);
                                                bool isNew = !lastAdminConn.Equals(adminConn) || !lastAppConn.Equals(appConn);
                                                if (isNew)
                                                {
                                                    Console.WriteLine("----------------------------------------------------------------------------------------------------------------------------------------------------------");
                                                    Console.ReadLine();
                                                }

                                                string msg = string.Format("{0}\r\n{1}\r\n=> {2}\t{3}\r\n",
                                                    adminConn,
                                                    sAs ? "\r" : appConn,
                                                    result,
                                                    message);
                                                Console.WriteLine(msg);

                                                lastAdminConn = adminConn;
                                                lastAppConn = appConn;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Console.ReadLine();
        }
    }
}



