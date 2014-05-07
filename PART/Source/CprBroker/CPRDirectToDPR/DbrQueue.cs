using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data.Queues;
using CprBroker.Utilities;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;

namespace CPRDirectToDPR
{
    public class DbrQueue : CprBroker.Data.Queues.Queue<CprDirectExtractQueueItem>
    {
        public DbrQueue()
        { }

        public string ConnectionString
        {
            get { return Encryption.DecryptObject<string>(this.Impl.EncryptedData.ToArray()); }
            set { this.Impl.EncryptedData = Encryption.EncryptObject(value); }
        }

        public override CprDirectExtractQueueItem[] Process(CprDirectExtractQueueItem[] items)
        {
            var ret = new List<CprDirectExtractQueueItem>();

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
                }
            }
            return ret.ToArray();
        }
    }
}
