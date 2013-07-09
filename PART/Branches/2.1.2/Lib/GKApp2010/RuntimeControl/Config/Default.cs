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
using System.Configuration;
using System.Diagnostics;

namespace GKApp2010.Config
{
    // ================================================================================
    public sealed class Default
    {
        static readonly Default _instance = new Default();

        Configuration _config = null;

        #region singleton constructor stuff
        // -----------------------------------------------------------------------------
        // Simgleton pattern used is no. 4 from ref http://csharpindepth.com/Articles/General/Singleton.aspx ?
        // -----------------------------------------------------------------------------
        static Default()
        {
        }

        // -----------------------------------------------------------------------------
        Default()
        {
        }

        // -----------------------------------------------------------------------------
        public static Default Instance
        {
            get { return _instance; }
        }
        #endregion

        #region public methods
        // -----------------------------------------------------------------------------
        public static string GetConnectionString(string connName)
        {
            Configuration config = Instance.VerifyConfig();
            ConnectionStringSettings conSettings = config.ConnectionStrings.ConnectionStrings[connName];

            if (conSettings == null)
            {
                string msg = "Connection string setting [" + connName + "] was NOT found in configuration (*.config)";
                throw new GKApp2010.Core.GKAConfigException(msg);
            }

            return conSettings.ConnectionString;
        }

        // -----------------------------------------------------------------------------
        public static string GetLogfilename()
        {
            string filename = "";

            filename = Process.GetCurrentProcess().MainModule.FileName;

            // Remove any trailing .exe or .dll extension
            int stripLen = ".exe".Length;
            string ext = filename.Substring(filename.Length - stripLen);

            if (string.Equals(ext, ".exe", System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(ext, ".dll", System.StringComparison.OrdinalIgnoreCase))
            {
                filename = filename.Substring(0, filename.Length - stripLen);
            }

            // Simply add .log to filename ...
            filename += ".log";

            return filename;
        }
        #endregion

        #region private methods
        // -----------------------------------------------------------------------------
        private Configuration VerifyConfig()
        {
            lock (_instance)
            {
                if (_config == null)
                {
                    string cfgFile = System.AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

                    ExeConfigurationFileMap exfm = new ExeConfigurationFileMap();
                    exfm.ExeConfigFilename = cfgFile;

                    _config = ConfigurationManager.OpenMappedExeConfiguration(exfm, ConfigurationUserLevel.None);
                }
            }

            return _config;
        }
        #endregion
    }
}
