using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Installers;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using CprBroker.Utilities;

namespace CprBroker.Tests.Installers
{
    namespace InstallationTests
    {
        [TestFixture]
        class RunCommand
        {
            public static void Main()
            {
                //new InstallationTests().RunCommandWithResult_C_NotNullOrEmpty();
                var info = new ProcessStartInfo(
                    @"C:\Magenta Workspace\PART\Source\Core\Installers\bin\Debug\ElevatedDllRunner.exe",
                    string.Format("\"{0}\" PopulateWebSites", typeof(WebInstallationInfo).AssemblyQualifiedName))
                    {
                        Verb = "runas", // indicates to elevate privileges
                        UseShellExecute = false,
                        //RedirectStandardOutput = true,
                        //RedirectStandardError = true
                    };

                var process = new Process
                {
                    EnableRaisingEvents = true, // enable WaitForExit()
                    StartInfo = info
                };

                process.Start();
                process.WaitForExit(); // sleep calling process thread until evoked process exit
            }

            [Test]
            public void RunCommandWithResult_C_NotNullOrEmpty()
            {
                string error;
                System.Diagnostics.Debugger.Launch();
                var sitesDataStr = RunCommandWithResult(
                        @"C:\Magenta Workspace\PART\Source\Core\Installers\bin\Debug\ElevatedDllRunner.exe",
                        string.Format("\"{0}\" PopulateWebSites", typeof(WebInstallationInfo).AssemblyQualifiedName),
                        out error);
                string s = sitesDataStr;
                //var sitesDataStr = session.GetPropertyValue("SitesXML");
                //var sitesData = Utilities.Strings.Deserialize<WebsiteCustomAction.WebSiteInfo[]>(sitesDataStr);
                //var output = Installation.RunCommandWithResult("dir.exe", "c:\\", out error);
                //Assert.IsNotNullOrEmpty(output);
            }

            [Test]
            public void RunCommandWithResult_C_MultipleLines()
            {
                string error;
                var output = RunCommandWithResult("dir", "c:\\", out error);
                var arr = output.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                Assert.Greater(arr.Length, 3);
            }

            public static string RunCommandWithResult(string fileName, string args, out string strStandardError)
            {
                var _StandardOutput = new StringBuilder();
                var _StandardError = new StringBuilder();

                DataReceivedEventHandler OutputHandler = (object sendingProcess, DataReceivedEventArgs outLine) =>
                {
                    if (outLine.Data != null)
                    {
                        _StandardOutput.AppendLine(outLine.Data);
                    }
                };
                DataReceivedEventHandler ErrorHandler = (object sendingProcess, DataReceivedEventArgs outLine) =>
                {
                    if (outLine.Data != null)
                    {
                        _StandardError.AppendLine(outLine.Data);
                    }
                };

                using (var process = new System.Diagnostics.Process())
                {
                    process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                    //process.StartInfo.CreateNoWindow = true;
                    process.EnableRaisingEvents = true;
                    process.StartInfo.Arguments = args;
                    process.StartInfo.FileName = fileName;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.Verb = "runas";
                    //process.StartInfo.RedirectStandardInput = true;
                    process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                    process.ErrorDataReceived += new DataReceivedEventHandler(ErrorHandler);

                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    strStandardError = _StandardError.ToString();

                    return _StandardOutput.ToString();
                }
            }
        }

        [TestFixture]
        public class CopyConfigNode
        {
            [Test]
            public void CopyConfigNode_Overwrite_OK()
            {
                System.Diagnostics.Debugger.Launch();
                var sourceName = "Src" + Strings.NewRandomString(5);
                var targetName = "Target" + Strings.NewRandomString(5);

                File.WriteAllText(sourceName, Properties.Resources.WebConfig);
                File.WriteAllText(targetName, Properties.Resources.CprBroker_EventBroker_Backend_exe);

                Installation.CopyConfigNode("configuration", "configSections", sourceName, targetName, Installation.MergeOption.Overwrite);

                File.Delete(sourceName);
                File.Delete(targetName);
            }
        }
    }
}