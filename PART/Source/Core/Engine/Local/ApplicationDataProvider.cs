using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data.Applications;
using CprBroker.Schemas;
using CprBroker.Utilities;

namespace CprBroker.Engine.Local
{
    /// <summary>
    /// Implements application management methods
    /// </summary>
    public class ApplicationDataProvider : IApplicationManager
    {
        #region IApplicationManager Members

        public ApplicationType RequestAppRegistration(string name)
        {
            // Create a new application and assign a new app token
            using (ApplicationDataContext context = new ApplicationDataContext())
            {
                var application = new Application();

                application.ApplicationId = Guid.NewGuid();
                application.Token = Guid.NewGuid().ToString();
                context.Applications.InsertOnSubmit(application);

                application.Name = name;
                application.RegistrationDate = DateTime.Now;
                application.IsApproved = false;
                Console.WriteLine("Request App reg apT=" + application.Token + ", AppID=" + application.ApplicationId + " nm=" + application.Name);

                context.SubmitChanges();
                return application.ToXmlType();
            }
        }

        public bool ApproveAppRegistration(string targetAppToken)
        {
            // Mark the application as approved
            using (ApplicationDataContext context = new ApplicationDataContext())
            {
                var application = context.Applications.SingleOrDefault(app => app.Token == targetAppToken);

                if (application != null)
                {
                    application.IsApproved = true;
                    application.ApprovedDate = DateTime.Now;
                    context.SubmitChanges();
                    return true;
                }
                return false;
            }
        }

        public ApplicationType[] ListAppRegistration()
        {
            List<ApplicationType> applications = new List<ApplicationType>();
            // Retrieve list of applications and convert to XML
            using (ApplicationDataContext context = new ApplicationDataContext())
            {
                var businessApps = from app in context.Applications where app.IsApproved == true select app;
                foreach (var application in businessApps)
                {
                    applications.Add(application.ToXmlType());
                }
                return applications.ToArray();
            }
        }

        public bool UnregisterApp(string targetAppToken)
        {
            // Mark the application as unregistered
            using (ApplicationDataContext context = new ApplicationDataContext())
            {
                var application = context.Applications.SingleOrDefault(app => app.Token == targetAppToken);
                if (application != null)
                {
                    application.IsApproved = false;
                    context.SubmitChanges();
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region IDataProvider Members

        public bool IsAlive()
        {
            return true;
        }

        public Version Version
        {
            get { return new Version(Constants.Versioning.Major, Constants.Versioning.Minor); }
        }

        #endregion
    }
}
