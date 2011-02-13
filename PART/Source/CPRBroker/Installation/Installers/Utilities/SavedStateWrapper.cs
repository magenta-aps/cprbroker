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

        public WebInstallationInfo GetWebInstallationInfo()
        {
            var xml = string.Format("{0}", savedState["WebInstallationInfo"]);
            return DAL.Utilities.Deserialize<WebInstallationInfo>(xml);
        }
        public void SetWebInstallationInfo(WebInstallationInfo value)
        {
            savedState["WebInstallationInfo"] = DAL.Utilities.SerializeObject(value);
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
