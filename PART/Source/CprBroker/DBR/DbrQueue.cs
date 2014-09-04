using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using CprBroker.Engine.Queues;
using CprBroker.Utilities;
using CprBroker.Engine;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;

namespace CprBroker.DBR
{
    public class DbrQueue : CprBroker.Engine.Queues.Queue<ExtractQueueItem>
    {
        public DbrQueue()
        { }

        public string ConnectionString
        {
            get { return DataProviderConfigProperty.Templates.GetConnectionString(this.ConfigurationProperties); }
            set { DataProviderConfigProperty.Templates.SetConnectionString(value, this.ConfigurationProperties); }
        }

        public int? Port
        {
            get
            {
                var s = this.ConfigurationProperties["Port"];
                int ret;
                if (int.TryParse(s, out ret))
                    return ret;
                else
                    return null;
            }
            set
            {
                this.ConfigurationProperties["Port"] = value.HasValue ? "" : value.Value.ToString();
            }
        }

        public bool DiversionEnabled
        {
            get { return Port.HasValue; }
        }

        public override ExtractQueueItem[] Process(ExtractQueueItem[] items)
        {
            var ret = new List<ExtractQueueItem>();

            using (var cprDataContext = new ExtractDataContext())
            {
                items.LoadExtractAndItems(cprDataContext);

                foreach (var item in items)
                {
                    if (item.Extract != null)
                    {
                        using (var dprDataContext = new DPRDataContext(this.ConnectionString))
                        {
                            CprConverter.DeletePersonRecords(item.PNR, dprDataContext);

                            var person = Extract.ToIndividualResponseType(item.Extract, item.ExtractItems, CprBroker.Providers.CPRDirect.Constants.DataObjectMap);
                            CprConverter.AppendPerson(person, dprDataContext);
                            dprDataContext.SubmitChanges();
                            ret.Add(item);
                        }
                    }
                }
            }
            return ret.ToArray();
        }

        public override Engine.DataProviderConfigPropertyInfo[] ConfigurationKeys
        {
            get
            {
                var ret = new List<Engine.DataProviderConfigPropertyInfo>(DataProviderConfigPropertyInfo.Templates.ConnectionStringKeys);
                ret.Add(new DataProviderConfigPropertyInfo() { Confidential = false, Name = "Port", Type = DataProviderConfigPropertyInfoTypes.Integer, Required = false });
                return ret.ToArray();
            }
        }

        public bool IsAlive()
        {
            try
            {
                using (var conn = new SqlConnection(this.ConnectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                CprBroker.Engine.Local.Admin.LogException(ex);
                return false;
            }
        }

        public DprDiversionServer CreateListener()
        {
            if (DiversionEnabled)
            {
                var listener = new DprDiversionServer() { Port = this.Port.Value, DbrQueue = this };
                return listener;
            }
            return null;
        }

    }
}
