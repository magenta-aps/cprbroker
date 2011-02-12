using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CprBroker.SetupDatabase
{
    /// <summary>
    /// Contains text messages displayed by the application
    /// </summary>
    public class Messages
    {
        public static readonly string Required = "Required";
        public static readonly string MaxLength = "Maximum allowed length is ";
        public static readonly string InvalidInput = "Invalid input";

        public static readonly string Succeeded = "Succeeded";
        public static readonly string Unsuccessful = "Unsuccessful";


        public static readonly string AdminConnectionFailed = "Admin connection failed";
        public static readonly string ApplicationConnectionFailed = "Application connection failed";

        public static readonly string DatabaseAlreadyExists = "The database already exists, would you like to use the exisitng database?";
        public static readonly string DoYouWantToDropDatabase = "Do you want to drop the system's database?";

        public static readonly string AdminConnectionHasInsufficientRights = "Admin connection does not have sufficient rights to create a database";

        public static readonly string WindowsAuthenticationContactAdmin = "The web site and backend service are configured to use Windows authentication to connect to the database. \r\nYou may need to contact your database administrator to add a new user for the windows identity used by the web site and backend service";

        public static readonly string CancelSetup = "Cancel setup";
        public static readonly string AreYouSureYouWantToCancel = "Are you sure you want to cancel setup?";

        public static readonly string AnErrorHasOccurredAndInstallationWillBeCancelled = "An error has occurred. Installation will be cancelled";
        public static readonly string AnErrorHasOccurredAndItWillBeIgnored = "An error has occurred. The process will ignore it and continue.";

        public static void ShowException(Exception ex)
        {
            MessageBox.Show(AnErrorHasOccurredAndItWillBeIgnored);
        }
    }
}
