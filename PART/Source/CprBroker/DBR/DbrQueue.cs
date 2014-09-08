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

                // Ignore those that cannot be converted
                items = items.Where(item => IsDbrConvertible(item)).ToArray();

                // Group in batches
                var itemGroups = new List<List<ExtractQueueItem>>();
                itemGroups.Add(new List<ExtractQueueItem>());

                foreach (var item in items)
                {
                    var currentGroup = itemGroups.Last();
                    if (currentGroup.Where(gi => gi.PNR == item.PNR).FirstOrDefault() != null)
                    {
                        currentGroup = new List<ExtractQueueItem>();
                        itemGroups.Add(currentGroup);
                    }
                    currentGroup.Add(item);
                }
                itemGroups = itemGroups.Where(g => g.Count > 0).ToList();

                // Now run the batches
                foreach (var itemGroup in itemGroups)
                {
                    try
                    {
                        using (var conn = new SqlConnection(this.ConnectionString))
                        {
                            using (var dprDataContext = new DPRDataContext(conn))
                            {
                                foreach (var item in itemGroup)
                                {
                                    var person = Extract.ToIndividualResponseType(item.Extract, item.ExtractItems, CprBroker.Providers.CPRDirect.Constants.DataObjectMap);
                                    CprConverter.AppendPerson(person, dprDataContext);
                                }
                                conn.Open();
                                using (var trans = conn.BeginTransaction())
                                {
                                    dprDataContext.Transaction = trans;
                                    CprConverter.DeletePersonRecords(itemGroup.Select(i => i.PNR).ToArray(), dprDataContext);
                                    var changes = dprDataContext.GetChangeSet();
                                    Console.WriteLine("Inserts {0}, Deletes {1}, Updates {2}", changes.Inserts.Count, changes.Deletes.Count, changes.Updates.Count);
                                    conn.BulkInsertChanges(dprDataContext, trans);

                                    trans.Commit();
                                    ret.AddRange(itemGroup);
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

        public bool IsDbrConvertible(ExtractQueueItem item)
        {
            try
            {
                if (item.Extract != null)
                {
                    using (var dprDataContext = new DPRDataContext(""))
                    {
                        var response = Extract.ToIndividualResponseType(item.Extract, item.ExtractItems, CprBroker.Providers.CPRDirect.Constants.DataObjectMap);
                        CprConverter.AppendPerson(response, dprDataContext);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                CprBroker.Engine.Local.Admin.LogException(ex, string.Format("<{0}>", item.PNR));
            }
            return false;
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
