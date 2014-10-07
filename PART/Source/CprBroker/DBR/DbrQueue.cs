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

        class Batch
        {
            public List<ExtractQueueItem> Items = new List<ExtractQueueItem>();
            public List<Object> Inserts = new List<object>();
        }

        public override ExtractQueueItem[] Process(ExtractQueueItem[] items)
        {
            var ret = new List<ExtractQueueItem>();

            using (var cprDataContext = new ExtractDataContext())
            {
                items.LoadExtractAndItems(cprDataContext);

                var itemGroups = new List<Batch>();
                itemGroups.Add(new Batch());

                // Group in batches
                foreach (var item in items)
                {
                    try
                    {
                        if (item.Extract != null)
                        {
                            using (var dprDataContext = new DPRDataContext(this.ConnectionString))
                            {
                                var response = Extract.ToIndividualResponseType(item.Extract, item.ExtractItems, CprBroker.Providers.CPRDirect.Constants.DataObjectMap);
                                CprConverter.AppendPerson(response, dprDataContext);

                                var currentGroup = itemGroups.Last();
                                if (currentGroup.Items.Where(gi => gi.PNR == item.PNR).FirstOrDefault() != null)
                                {
                                    currentGroup = new Batch();
                                    itemGroups.Add(currentGroup);
                                }
                                currentGroup.Items.Add(item);
                                currentGroup.Inserts.AddRange(dprDataContext.GetChangeSet().Inserts);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CprBroker.Engine.Local.Admin.LogException(ex, string.Format("<{0}>", item.PNR));
                    }
                }
                itemGroups = itemGroups.Where(g => g.Items.Count > 0).ToList();

                // Now run the batches
                foreach (var itemGroup in itemGroups)
                {
                    try
                    {
                        using (var conn = new SqlConnection(this.ConnectionString))
                        {
                            using (var dprDataContext = new DPRDataContext(conn))
                            {
                                conn.Open();
                                using (var trans = conn.BeginTransaction())
                                {
                                    dprDataContext.Transaction = trans;

                                    CprConverter.DeletePersonRecords(itemGroup.Items.Select(i => i.PNR).ToArray(), dprDataContext);
                                    conn.BulkInsertChanges(itemGroup.Inserts, trans);

                                    trans.Commit();
                                    ret.AddRange(itemGroup.Items);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CprBroker.Engine.Local.Admin.LogException(ex);
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
