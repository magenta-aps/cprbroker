//------------------------------------------------------------------------------
// Gentofte Kommune application framework 2011.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

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
