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
using CprBroker.Data;
using CprBroker.Data.Queues;
using CprBroker.Utilities;

namespace CprBroker.Engine.Queues
{
    public abstract class Queue : IHasConfigurationProperties
    {
        private CprBroker.Data.Queues.DbQueue _Impl;

        public virtual Engine.DataProviderConfigPropertyInfo[] ConfigurationKeys { get { return new DataProviderConfigPropertyInfo[] { }; } }
        public Dictionary<string, string> ConfigurationProperties { get; set; }

        public abstract void RunAll();
        public abstract void RunOneBatch();

        public CprBroker.Data.Queues.DbQueue Impl
        {
            get
            {
                return _Impl;
            }
            internal set
            {
                _Impl = value;
                if (_Impl != null)
                {
                    this.FillFromEncryptedStorage(_Impl);
                }
            }
        }

        public static Queue ToQueue(DbQueue impl)
        {
            var ret = CprBroker.Utilities.Reflection.CreateInstance<Queue>(impl.TypeName);
            if (ret != null)
            {
                ret.Impl = impl;
            }
            return ret;
        }

        public static TQueue[] GetQueues<TQueue>()
            where TQueue : class
        {
            using (var dataContext = new QueueDataContext())
            {
                return dataContext.Queues
                    .ToArray()
                    .Select(q => Queue.ToQueue(q) as TQueue)
                    .Where(q => q != null)
                    .ToArray();
            }
        }

        public static TQueue[] GetQueues<TQueue>(int typeId)
            where TQueue : Queue, new()
        {
            using (var dataContext = new QueueDataContext())
            {
                return dataContext.Queues
                    .Where(q => q.TypeId.HasValue && q.TypeId.Value == typeId)
                    .ToArray()
                    .Select(q => new TQueue() { Impl = q })
                    .ToArray();
            }
        }

        public static TQueue GetById<TQueue>(Guid queueId)
            where TQueue : Queue
        {
            var db = DbQueue.GetById(queueId);
            return ToQueue(db) as TQueue;
        }

        public static TQueue AddQueue<TQueue>(int queueTypeId, Dictionary<string, string> values, int batchSize, int maxRetry)
            where TQueue : Queue, new()
        {
            var ret = new TQueue();
            ret.Impl = new DbQueue()
            {
                QueueId = Guid.NewGuid(),
                BatchSize = batchSize,
                MaxRetry = maxRetry,
                TypeId = queueTypeId,
                TypeName = typeof(TQueue).IdentifyableName(),

                Attributes = new List<Schemas.AttributeType>()
            };
            if (ret is IHasConfigurationProperties)
            {
                (ret as IHasConfigurationProperties).ConfigurationProperties = values;
                (ret as IHasConfigurationProperties).CopyToEncryptedStorage(ret.Impl);
            }

            using (var dataContext = new QueueDataContext())
            {
                dataContext.Queues.InsertOnSubmit(ret.Impl);
                dataContext.SubmitChanges();
            }

            return ret;
        }


        public static void UpdateAttributesById(Guid queueId, Dictionary<string, string> props)
        {
            using (var dataContext = new QueueDataContext())
            {
                var db = dataContext.Queues.Single(dbq => dbq.QueueId == queueId);
                db.SetAll(props);

                dataContext.SubmitChanges();
            }
        }

        public static void DeleteById(Guid queueId)
        {
            using (var dataContext = new QueueDataContext())
            {
                var db = dataContext.Queues.Single(dbq => dbq.QueueId == queueId);
                dataContext.Queues.DeleteOnSubmit(db);
                dataContext.SubmitChanges();
            }
        }

        public Guid QueueId
        {
            get { return Impl.QueueId; }
        }
    }
}
