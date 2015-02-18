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
            using (var dataContext = new QueueDataContext())
            {
                var ret = new DbSemaphore()
                {
                    SemaphoreId = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
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

        public void Signal()
        {
            using (var dataContext = new QueueDataContext())
            {
                var semaphore = dataContext.Semaphores.Where(s => s.SemaphoreId == this.Impl.SemaphoreId).Single();
                if (!semaphore.SignaledDate.HasValue)
                {
                    semaphore.SignaledDate = DateTime.Now;
                    dataContext.SubmitChanges();
                }
            }
        }
    }
}
