using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities;

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

        public DatabaseSetupInfo GetDatabaseSetupInfo()
        {
            var xml = string.Format("{0}", savedState["DatabaseSetupInfo"]);
            return Strings.Deserialize<DatabaseSetupInfo>(xml);
        }

        public void SetDatabaseSetupInfo(DatabaseSetupInfo value)
        {
            savedState["DatabaseSetupInfo"] = Strings.SerializeObject(value);
        }

        public void ClearDatabaseSensitiveDate()
        {
            var dbSetupInfo = GetDatabaseSetupInfo();
            dbSetupInfo.ClearSensitiveDate();
        }

        public WebInstallationInfo GetWebInstallationInfo()
        {
            var xml = string.Format("{0}", savedState["WebInstallationInfo"]);
            return Strings.Deserialize<WebInstallationInfo>(xml);
        }
        public void SetWebInstallationInfo(WebInstallationInfo value)
        {
            savedState["WebInstallationInfo"] = Strings.SerializeObject(value);
        }

        public string ServiceName
        {
            get
            {
                return string.Format("{0}", savedState["ServiceName"]);
            }
            set
            {
                savedState["ServiceName"] = value;
            }
        }

    }
}
