using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace CprBroker.Data.Part
{
    /// <summary>
    /// Represents the data context for all PART preson tables
    /// </summary>
    public partial class PartDataContext
    {
        public PartDataContext()
            : base(Config.Properties.Settings.Default.CprBrokerConnectionString)
        {
            OnCreated();
            /*if (!Directory.Exists(@"C:\Log"))
            {
                Directory.CreateDirectory(@"C:\Log");
            }
            var sw = new System.IO.StreamWriter(string.Format(@"C:\Log\{0} - {1}.log", DateTime.Now.ToString("yyyy-MM-dd HH-mm"), this.GetHashCode()));
            sw.AutoFlush = true;
            sw.Write(new System.Diagnostics.StackTrace().ToString());
            Log = sw;*/
        }
    }
}