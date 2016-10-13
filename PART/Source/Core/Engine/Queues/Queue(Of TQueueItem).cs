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
using CprBroker.Data.Queues;

namespace CprBroker.Engine.Queues
{
    public abstract class Queue<TQueueItem> : Queue
        where TQueueItem : IQueueItem, new()
    {
        public Queue()
        {
        }

        public Queue(Guid queueId)
        {
            this.Impl = DbQueue.GetById(queueId);
        }

        public virtual TQueueItem[] GetNext(int maxCount)
        {
            return Impl.GetNext(maxCount)
                .Select(
                (qi) =>
                {
                    var ret = new TQueueItem() { Impl = qi };
                    ret.DeserializeFromKey(qi.ItemKey);
                    return ret;
                })
                .ToArray();
        }

        public virtual void Remove(TQueueItem[] items)
        {
            Impl.Remove(items.Select(i => i.Impl).ToArray());
        }

        public virtual void MarkFailure(TQueueItem[] items)
        {
            Impl.MarkFailure(items.Select(i => i.Impl).ToArray());
        }

        public void Enqueue(TQueueItem item, Semaphore semaphore = null)
        {
            Enqueue(new TQueueItem[] { item }, semaphore);
        }

        public virtual void Enqueue(TQueueItem[] items, Semaphore semaphore = null)
        {
            var itemKeys = items.Select(it => it.SerializeToKey()).ToArray();
            Impl.Enqueue(
                itemKeys,
                semaphore != null ? semaphore.Impl : null);
        }

        /// <summary>
        /// Implements the actual task that is supposed to be implemented for each queue item
        /// Successful
        /// </summary>
        /// <param name="items">The queue items</param>
        /// <returns>A subset if the input that was processed successfully</returns>
        public abstract TQueueItem[] Process(TQueueItem[] items);

        public override void RunAll()
        {
            var items = GetNext(Impl.BatchSize);
            while (items.FirstOrDefault() != null)
            {
                CprBroker.Engine.Local.Admin.LogFormattedSuccess("Queue <{0}><{1}>, Processing <{2}> items", GetType().Name, Impl.QueueId, items.Length);
                RunItems(items);
                items = GetNext(Impl.BatchSize);
            }
        }

        public override void RunOneBatch()
        {
            var items = GetNext(Impl.BatchSize);
            CprBroker.Engine.Local.Admin.LogFormattedSuccess("Queue <{0}><{1}>, Processing <{2}> items", GetType().Name, Impl.QueueId, items.Length);
            if (items.FirstOrDefault() != null)
            {
                RunItems(items);
            }
            CprBroker.Engine.Local.Admin.LogFormattedSuccess("Queue <{0}><{1}>, batch completed", GetType().Name, Impl.QueueId);
        }

        private void RunItems(TQueueItem[] items)
        {
            var succeeded = new TQueueItem[0];
            try
            {
                succeeded = Process(items);
            }
            catch (Exception ex)
            {
                CprBroker.Engine.Local.Admin.LogException(ex);
            }
            Remove(succeeded);

            var failedItems = items.Except(succeeded).ToArray();
            MarkFailure(failedItems);
        }
    }

}