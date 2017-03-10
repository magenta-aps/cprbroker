/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using CprBroker.Schemas;
using System.Data.SqlClient;
using CprBroker.Utilities;

namespace CprBroker.Data.Queues
{
    public partial class DbQueue : IHasEncryptedAttributes
    {
        public System.Security.Cryptography.RijndaelManaged EncryptionAlgorithm { get; set; }
        public List<AttributeType> Attributes { get; set; }

        partial void OnLoaded()
        {
            this.PreLoadAttributes();
        }

        public static DbQueue GetById(Guid queueId)
        {
            using (var dataContext = new QueueDataContext())
            {
                return dataContext.Queues
                    .Where(q => q.QueueId == queueId)
                    .FirstOrDefault();
            }
        }

        public DbQueueItem[] GetNext(int maxCount)
        {
            using (var dataContext = new QueueDataContext())
            {
                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<DbQueueItem>(qi => qi.Semaphore);
                dataContext.LoadOptions = loadOptions;

                return dataContext.QueueItems
                    .Where(qi =>
                        qi.QueueId == this.QueueId
                        && qi.AttemptCount < this.MaxRetry
                        && (qi.Semaphore == null || qi.Semaphore.SignaledDate.HasValue))
                    .OrderBy(qi => qi.QueueItemId)
                    .Take(maxCount)
                    .ToArray();
            }
        }

        public void Remove(DbQueueItem[] items)
        {
            using (var dataContext = new QueueDataContext())
            {
                var ids = items.Select(it => it.QueueItemId).ToArray();
                var itemsToDelete = dataContext.QueueItems.Where(it => ids.Contains(it.QueueItemId));
                dataContext.QueueItems.DeleteAllOnSubmit(itemsToDelete);
                dataContext.SubmitChanges();
            }
        }

        public void MarkFailure(DbQueueItem[] items)
        {
            using (var dataContext = new QueueDataContext())
            {
                var ids = items.Select(it => it.QueueItemId).ToArray();
                var itemsToMark = dataContext.QueueItems.Where(it => ids.Contains(it.QueueItemId));
                foreach (var item in itemsToMark)
                {
                    item.AttemptCount++;
                }
                dataContext.SubmitChanges();
            }
        }

        public void Enqueue(string[] itemKeys, DbSemaphore semaphore)
        {
            var items = itemKeys.Select(ik => new DbQueueItem(ik, this, semaphore));
            using (var dataContext = new QueueDataContext())
            {
                using (var conn = dataContext.Connection as SqlConnection)
                {
                    conn.Open();
                    conn.BulkInsertAll(items);
                }
            }
        }

        public void MultiplyTo(IEnumerable<DbQueue> targetQueues, int maxCount)
        {
            using (var dataContext = new QueueDataContext())
            {
                var items = this.GetNext(maxCount);

                foreach (var q in targetQueues)
                {
                    q.QueueItems.AddRange(items.Select(i => i.Clone(q)));
                }

                dataContext.QueueItems.DeleteAllOnSubmit(items);
                dataContext.SubmitChanges();
            }
        }



    }
}
