using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.DAL.Part
{
    public partial class PartDataContext
    {
        public PartDataContext()
            : base(CPRBroker.Config.Properties.Settings.Default.CPRConnectionString)
        {
            OnCreated();
            //var sw = new System.IO.StreamWriter("c:\\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm") + ".log");            
            //sw.AutoFlush = true;
            //Log = sw;
        }
    }
}