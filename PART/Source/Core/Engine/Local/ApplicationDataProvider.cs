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
