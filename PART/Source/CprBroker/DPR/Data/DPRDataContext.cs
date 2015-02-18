using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.DPR
{
    public partial class DPRDataContext
    {
        public System.Data.Linq.Table<PersonTotal7> PersonTotal7s
        {
            get
            {
                return this.GetTable<PersonTotal7>();
            }
        }
    }
}
