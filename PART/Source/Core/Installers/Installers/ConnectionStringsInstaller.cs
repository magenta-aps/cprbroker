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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration.Install;
using CprBroker.Utilities;

namespace CprBroker.Installers
{
    /// <summary>
    /// Responsible for setting and encryption of connection strings in config files
    /// This installer is usually the last installer of a series.
    /// Requested connection strings are registered here and then set in the Install method
    /// </summary>
    public class ConnectionStringsInstaller : Installer
    {
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
            foreach (var configFile in _Files)
            {
                string configFileName = configFile.FileName;
                foreach (var connectionString in configFile.ConnectionStrings)
                {
                    Installation.SetConnectionStringInConfigFile(configFileName, connectionString.Name, connectionString.Value);
                }                
                if (configFile.CommitAction != null)
                {
                    try
                    {
                        configFile.CommitAction();
                    }
                    catch (Exception ex)
                    {
                        string message = string.Format(
                            "Could not encrypt connection strings in config file \"{0}\", you may need to look at the file.",
                            configFile.FileName
                        );
                        Messages.ShowException(this, message, ex);
                    }
                }
            }
        }

        private static List<File> _Files = new List<File>();

        public static void RegisterConnectionString(string fileName, string name, string value)
        {
            var file = _Files.FirstOrDefault(f => f.FileName == fileName);
            if (file == null)
            {
                file = new File() { FileName = fileName };
                _Files.Add(file);
            }

            var connectionString = file.ConnectionStrings.FirstOrDefault(cs => cs.Name == name);
            if (connectionString == null)
            {
                connectionString = new ConnectionString() { Name = name };
                file.ConnectionStrings.Add(connectionString);
            }
            connectionString.Value = value;
        }

        public static void RegisterCommitAction(string fileName, Action action)
        {
            var file = _Files.FirstOrDefault(f => f.FileName == fileName);
            if (file == null)
            {
                file = new File() { FileName = fileName };
                _Files.Add(file);
            }
            file.CommitAction = action;
        }

        public class ConnectionString
        {
            public string Name;
            public string Value;
        }

        public class File
        {
            public string FileName;
            public List<ConnectionString> ConnectionStrings = new List<ConnectionString>();
            public Action CommitAction = null;
        }
    }
}
