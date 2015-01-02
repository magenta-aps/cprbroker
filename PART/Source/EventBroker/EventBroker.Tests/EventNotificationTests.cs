using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.EventBroker.Data;

namespace CprBroker.Tests.EventBroker
{
    namespace EventNotificationTests
    {
        [TestFixture]
        public class GetNext
        {
            [Test]
            public void GetNext_Empty_Empty()
            {
                var inp = new EventNotification[0].AsQueryable();
                var ret = EventNotification.GetNext(inp, DateTime.Now.AddDays(-1),int.MaxValue, 100);
                Assert.IsEmpty(ret);
            }

            [Test]
            public void GetNext_NonTried_All([Values(1, 5, 10, 100)]int c)
            {
                var inp = new EventNotification[c];
                for (int i = 0; i < c; i++)
                    inp[i] = new EventNotification() { CreatedDate = DateTime.Now.AddDays(-i), Succeeded = null };

                var ret = EventNotification.GetNext(inp.AsQueryable(), DateTime.Now.AddDays(-1), int.MaxValue, 100);
                Assert.AreEqual(c, ret.Length);
            }

            [Test]
            public void GetNext_AllFreshFailed_None([Values(1, 5, 10, 100)]int c)
            {
                var inp = new EventNotification[c];
                for (int i = 0; i < c; i++)
                    inp[i] = new EventNotification() { CreatedDate = DateTime.Now.AddDays(-i), Succeeded = false, NotificationDate = DateTime.Now.AddMinutes(-1) };

                var ret = EventNotification.GetNext(inp.AsQueryable(), DateTime.Now.AddMinutes(-5), int.MaxValue, 100);
                Assert.IsEmpty(ret);
            }

            [Test]
            public void GetNext_TriedWithOneToRetry_One([Values(2, 5, 10, 100)]int c)
            {
                var inp = new EventNotification[c];
                for (int i = 0; i < c; i++)
                    inp[i] = new EventNotification() { CreatedDate = DateTime.Now.AddDays(-i), Succeeded = false, NotificationDate = DateTime.Now.AddMinutes(-1) };

                var r = new Random().Next(0, c);
                inp[r].NotificationDate = DateTime.Now.AddMinutes(-10);

                var ret = EventNotification.GetNext(inp.AsQueryable(), DateTime.Now.AddMinutes(-5), int.MaxValue, 100);
                Assert.AreEqual(1, ret.Length);
            }

            [Test]
            public void GetNext_AllSucceeded_None([Values(1, 5, 10, 100)]int c)
            {
                var inp = new EventNotification[c];
                for (int i = 0; i < c; i++)
                    inp[i] = new EventNotification() { CreatedDate = DateTime.Now.AddDays(-i), Succeeded = true };

                var ret = EventNotification.GetNext(inp.AsQueryable(), DateTime.Now.AddDays(-1), int.MaxValue, 100);
                Assert.IsEmpty(ret);
            }

            [Test]
            public void GetNext_AllFailedMaxAttempt_None([Values(1, 5, 10, 100)]int c)
            {
                var inp = new EventNotification[c];
                for (int i = 0; i < c; i++)
                    inp[i] = new EventNotification() { CreatedDate = DateTime.Now.AddDays(-i), Succeeded = false, AttemptCount = 100 };

                var ret = EventNotification.GetNext(inp.AsQueryable(), DateTime.Now.AddMinutes(-1), 50, 100);
                Assert.IsEmpty(ret);
            }
        }
    }
}
