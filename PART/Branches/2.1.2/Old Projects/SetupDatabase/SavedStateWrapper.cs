using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.SetupDatabase
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

        public string AdminConnectionString
        {
            get
            {
                return Convert.ToString(savedState["AdminConnectionString"]);
            }
            set
            {
                savedState["AdminConnectionString"] = value;
            }
        }

        public bool DatabaseCreated
        {
            get
            {
                return Convert.ToBoolean(savedState["DatabaseCreated"]);
            }
            set
            {
                savedState["DatabaseCreated"] = value;
            }
        }

        
    }
}
