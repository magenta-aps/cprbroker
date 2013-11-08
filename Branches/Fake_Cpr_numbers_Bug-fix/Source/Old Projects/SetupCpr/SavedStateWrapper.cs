using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.SetupCpr
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

    }
}
