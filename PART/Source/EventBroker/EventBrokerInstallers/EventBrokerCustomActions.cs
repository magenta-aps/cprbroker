using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using CprBroker.Utilities;
using CprBroker.Installers;
using CprBroker.Engine;
using CprBroker.EventBroker.Data;

namespace CprBroker.Installers.EventBrokerInstallers
{
    public static class EventBrokerCustomActions
    {
        static readonly string WebsiteDirectoryRelativePath = "EventBroker\\Website\\";
        public static readonly string ServiceName = "CPR broker backend service";

        private static string GetServiceExeFullFileName(Session session)
        {
            return string.Format("{0}{1}bin\\CprBroker.EventBroker.Backend.exe", session.GetInstallDirProperty(), WebsiteDirectoryRelativePath);
        }

        [CustomAction]
        public static ActionResult InstallBackendService(Session session)
        {
            try
            {
                Installation.SetConnectionStringInConfigFile(GetServiceExeFullFileName(session) + ".config", "CprBroker.Config.Properties.Settings.CprBrokerConnectionString", DatabaseSetupInfo.CreateFromFeature(session, "CPR").CreateConnectionString(false, true));
                Installation.SetConnectionStringInConfigFile(GetServiceExeFullFileName(session) + ".config", "CprBroker.Config.Properties.Settings.EventBrokerConnectionString", DatabaseSetupInfo.CreateFromFeature(session, "EVENT").CreateConnectionString(false, true));

                CprBroker.Installers.Installation.RunCommand(
                    string.Format("{0}installutil.exe", CprBroker.Installers.Installation.GetNetFrameworkDirectory(new Version(2, 0))),
                    string.Format("/i \"{0}\"", GetServiceExeFullFileName(session))
                );

                System.ServiceProcess.ServiceController controller = new System.ServiceProcess.ServiceController(new CprBroker.EventBroker.Backend.BackendService().ServiceName);
                controller.Start();
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult RollbackBackendService(Session session)
        {
            try
            {
                return UnInstallBackendService(session);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult UnInstallBackendService(Session session)
        {
            try
            {
                var controller = new System.ServiceProcess.ServiceController(ServiceName);
                if (controller.CanStop)
                    controller.Stop();

                CprBroker.Installers.Installation.RunCommand(
                    string.Format("{0}installutil.exe", CprBroker.Installers.Installation.GetNetFrameworkDirectory(new Version(2, 0))),
                    string.Format("/u \"{0}\"", GetServiceExeFullFileName(session))
                );
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult SetCprBrokerUrl(Session session)
        {
            try
            {
                WebInstallationInfo cprBrokerWebInstallationInfo = WebInstallationInfo.CreateFromFeature(session, "CPR");

                var urls = cprBrokerWebInstallationInfo.CalculateWebUrls();
                Func<string, string> urlFunc = u => string.Format("{0}Services/Events.asmx", u);
                var bestUrl = urlFunc(urls[0]);

                for (int i = 0; i < urls.Length; i++)
                {
                    string url = urlFunc(urls[i]);
                    var client = new System.Net.WebClient();
                    try
                    {
                        client.DownloadData(url);
                        bestUrl = url;
                        break;
                    }
                    catch (Exception ex)
                    {
                        object o = ex;
                    }
                }

                Installation.SetApplicationSettingInConfigFile(
                    GetServiceExeFullFileName(session) + ".config",
                    typeof(CprBroker.Config.Properties.Settings),
                    "EventsServiceUrl",
                    bestUrl
                    );
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }
    }
}
