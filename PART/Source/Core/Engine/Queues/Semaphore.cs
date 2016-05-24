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
    public class Semaphore
    {
        public DbSemaphore Impl { get; private set; }

        private Semaphore()
        { }

        private Semaphore(DbSemaphore db)
        {
            Impl = db;
        }

        public static Semaphore Create()
        {
            return Create(1);
        }

        public static Semaphore Create(int waitCount)
        {
            using (var dataContext = new QueueDataContext())
            {
                var ret = new DbSemaphore()
                {
                    SemaphoreId = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    WaitCount = waitCount,
                    SignaledDate = null
                };
                dataContext.Semaphores.InsertOnSubmit(ret);
                dataContext.SubmitChanges();
                return new Semaphore(ret);
            }
        }

        public static Semaphore GetById(Guid id)
        {
            using (var dataContext = new QueueDataContext())
            {
                var db = dataContext.Semaphores.Where(s => s.SemaphoreId == id).SingleOrDefault();
                if (db != null)
                    return new Semaphore(db);
            }
            return null;
        }

        public void SignalAll()
        {
            Signal(Impl.WaitCount.HasValue ? Impl.WaitCount.Value : 1);
        }

        public void Signal()
        {
            Signal(1);
        }

        public void Signal(int count)
        {
            using (var dataContext = new QueueDataContext())
            {
                var semaphore = dataContext.Semaphores.Where(s => s.SemaphoreId == this.Impl.SemaphoreId).Single();
                if (!semaphore.SignaledDate.HasValue)
                {
                    if (!semaphore.WaitCount.HasValue || semaphore.WaitCount.Value <= count)
                    {
                        semaphore.WaitCount = 0;
                        semaphore.SignaledDate = DateTime.Now;
                    }
                    else
                    {
                        semaphore.WaitCount--;
                    }
                    dataContext.SubmitChanges();
                }
            }
        }

        public void Wait()
        {
            Wait(1);
        }

        public void Wait(int waitCount)
        {
            using (var dataContext = new QueueDataContext())
            {
                var semaphore = dataContext.Semaphores.Where(s => s.SemaphoreId == this.Impl.SemaphoreId).Single();
                semaphore.SignaledDate = null;
                if (semaphore.WaitCount.HasValue)
                    semaphore.WaitCount += waitCount;
                else
                    semaphore.WaitCount = waitCount;

                dataContext.SubmitChanges();
            }
        }
    }
}
