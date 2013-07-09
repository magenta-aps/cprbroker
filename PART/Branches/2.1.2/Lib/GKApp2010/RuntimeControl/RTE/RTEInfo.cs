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
using System.Reflection;
using System.Security.Principal;
using System.Threading;

using System.ServiceModel;

namespace GKApp2010.RTE
{
    // ================================================================================
    public static class Info
    {
        // -----------------------------------------------------------------------------
        public static string GetRuntimeInfo(Assembly assembly)
        {
            string result = "";
            string title = "";
            string description = "";
            string product = "";
            string company = "";
            string copyright = "";

            //Assembly assembly = Assembly.GetAssembly(t);

            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (attributes.Length > 0)
            {
                title = ((AssemblyTitleAttribute)attributes[0]).Title;
            }

            attributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                description = ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }

            attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            if (attributes.Length > 0)
            {
                company = ((AssemblyCompanyAttribute)attributes[0]).Company;
            }

            attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (attributes.Length > 0)
            {
                product = ((AssemblyProductAttribute)attributes[0]).Product;
            }

            attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attributes.Length > 0)
            {
                copyright = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }

            result += "Assembly name=[" + assembly.GetName().Name + "], description=[" + description + "], version=[" + GetRuntimeVersion(assembly) + "], location=[" + assembly.Location + "]. ";

            result += "\n" + GetRuntimeContext();

            return result;
        }

        // -----------------------------------------------------------------------------
        public static string GetRuntimeInfo()
        {
            return GetRuntimeInfo(Assembly.GetCallingAssembly());
        }

        // -----------------------------------------------------------------------------
        public static string GetGKAppRuntimeInfo()
        {
            return GetRuntimeInfo(Assembly.GetExecutingAssembly());
        }

        // -----------------------------------------------------------------------------
        public static string GetRuntimeVersion(Assembly assembly)
        {
            string version = "";

            Version ver = assembly.GetName().Version;
            version = ver.Major.ToString() + "." + ver.Minor.ToString() + "." + ver.Build.ToString() + "." + ver.Revision.ToString();

            return version;
        }

        // -----------------------------------------------------------------------------
        public static string GetRuntimeVersion()
        {
            return GetRuntimeVersion(Assembly.GetCallingAssembly());
        }

        // -----------------------------------------------------------------------------
        public static string GetRuntimeContext()
        {
            string exeContext = "";

            // TODO Must do something, when app is ASP.NET, ie return HttpContext.Current.User;

            IPrincipal threadPrincipal = Thread.CurrentPrincipal;
            if (threadPrincipal != null)
            {
                IIdentity principalIdentity = threadPrincipal.Identity;
                if (principalIdentity != null)
                {
                    exeContext += "Current thread identity=[" + principalIdentity.Name + "], ";
                    exeContext += "is authenticated=[" + principalIdentity.IsAuthenticated.ToString() + "] ";
                    exeContext += "with authenticationtype=[" + principalIdentity.AuthenticationType + "]. ";
                }
            }

            WindowsIdentity curIdentity = WindowsIdentity.GetCurrent();
            if (curIdentity != null)
            {
                exeContext += "Current Windows identity=[" + curIdentity.Name+ "], ";
                exeContext += "is authenticated=[" + curIdentity.IsAuthenticated.ToString() + "] ";
                exeContext += "with authenticationtype=[" + curIdentity.AuthenticationType + "] and ";
                exeContext += "impersonationlevel=[" + System.Enum.GetName(typeof(TokenImpersonationLevel), curIdentity.ImpersonationLevel) + "]. ";
            }

            ServiceSecurityContext ssc = ServiceSecurityContext.Current;
            if (ssc != null)
            {
                IIdentity sscPrimaryIdentity = ssc.PrimaryIdentity;
                IIdentity sscWindowsIdentity = ssc.WindowsIdentity;

                if (sscPrimaryIdentity != null)
                {
                    exeContext += "Current ServiceSecurityContext.PrimaryIdentity=[" + sscPrimaryIdentity.Name + "], ";
                    exeContext += "is authenticated=[" + sscPrimaryIdentity.IsAuthenticated.ToString() + "] ";
                    exeContext += "with authenticationtype=[" + sscPrimaryIdentity.AuthenticationType + "]. ";
                }
                else
                {
                    exeContext += "Current ServiceSecurityContext.PrimaryIdentity=[null]. ";
                }

                if (sscWindowsIdentity != null)
                {
                    exeContext += "Current ServiceSecurityContext.WindowsIdentity=[" + sscWindowsIdentity.Name + "], ";
                    exeContext += "is authenticated=[" + sscWindowsIdentity.IsAuthenticated.ToString() + "] ";
                    exeContext += "with authenticationtype=[" + sscWindowsIdentity.AuthenticationType + "]. ";
                }
                else
                {
                    exeContext += "Current ServiceSecurityContext.WindowsIdentity=[null]. ";
                }
            }

            if (exeContext.Length == 0)
            {
                exeContext = "Execution context UNRESOLVED! ";
            }

            exeContext += "Host=[" + System.Environment.MachineName + "]. ";

            return exeContext;
        }

        // -----------------------------------------------------------------------------
        public static string GetApplicationName()
        {
            return AppDomain.CurrentDomain.FriendlyName;
        }

        // -----------------------------------------------------------------------------
        public static string ApplicationDisplayName
        {
            // Absolutly not thread safe construtcion, but it won't break
            get;
            set;
        }
    }
}
