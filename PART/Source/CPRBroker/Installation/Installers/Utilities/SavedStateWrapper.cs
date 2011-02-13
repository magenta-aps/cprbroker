using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Installers
{
    public class SavedStateWrapper
    {
        private IDictionary savedState;

        private SavedStateWrapper()
        { }

        public SavedStateWrapper(IDictionary state)
        {
            savedState = state;
        }

        private static readonly string ApplicationPathKeyName = "ApplicationPath";
        public string ApplicationPath
        {
            get
            {
                return Convert.ToString(this.savedState[ApplicationPathKeyName]);
            }
            set
            {
                this.savedState[ApplicationPathKeyName] = value;
            }
        }

        private static readonly string ApplicationInstalledKeyName = "ApplicationInstalled";
        public bool ApplicationInstalled
        {
            get
            {
                return Convert.ToBoolean(this.savedState[ApplicationInstalledKeyName]);
            }
            set
            {
                this.savedState[ApplicationInstalledKeyName] = value;
            }
        }

        public DatabaseSetupInfo GetDatabaseSetupInfo()
        {
            var xml = string.Format("{0}", savedState["DatabaseSetupInfo"]);
            return DAL.Utilities.Deserialize<DatabaseSetupInfo>(xml);
        }

        public void SetDatabaseSetupInfo(DatabaseSetupInfo value)
        {
            savedState["DatabaseSetupInfo"] = DAL.Utilities.SerializeObject(value);
        }

        public void ClearDatabaseSensitiveDate()
        {
            var dbSetupInfo = GetDatabaseSetupInfo();
            dbSetupInfo.ClearSensitiveDate();
        }

        public WebInstallationInfo WebInstallationInfo
        {
            get
            {
                return savedState["WebInstallationInfo"] as WebInstallationInfo;
            }
            set
            {
                savedState["WebInstallationInfo"] = value;
            }
        }

    }
}
