//------------------------------------------------------------------------------
// Gentofte Kommune application framework 2011.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

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
