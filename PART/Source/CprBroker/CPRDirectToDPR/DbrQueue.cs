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
        }

        public override ExtractQueueItem[] Process(ExtractQueueItem[] items)
        {
            var ret = new List<ExtractQueueItem>();

            using (var cprDataContext = new ExtractDataContext())
            {
                items.LoadExtractAndItems(cprDataContext);

                using (var dprDataContext = new DPRDataContext(this.ConnectionString))
                {
                    foreach (var item in items)
                    {
                        try
                        {
                            var person = Extract.ToIndividualResponseType(item.Extract, item.ExtractItems, CprBroker.Providers.CPRDirect.Constants.DataObjectMap);
                            CprConverter.AppendPerson(person, dprDataContext);
                            ret.Add(item);
                        }
                        catch (Exception ex)
                        {
                            CprBroker.Engine.Local.Admin.LogException(ex);
                        }
                    }
                    dprDataContext.SubmitChanges();
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

        public DiversionListener CreateListener()
        {
            var listener = new DiversionListener() { Port = this.Port.Value };
            return listener;
        }
        
    }
}
