using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    [TestFixture]
    class HistoricalNameTests
    {
        public class LoadAll
        {
            [Test]
            public void SSS()
            {
                var all = LineWrapper.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var map = new Dictionary<string, Type>();
                map["026"] = typeof(HistoricalNameType);

                var rel = all
                    .Where(l => l.Code == "026")
                    .Select(l => l.ToWrapper(map) as HistoricalNameType)
                    .OrderBy(l => l.NameStartDate)
                    .GroupBy(l => l.PNR)
                    .Select(g => g.OrderBy(l => l.NameStartDate).First())
                    .GroupBy(p =>
                        {
                            var key = "";
                            if (!p.NameStartDate.HasValue)
                                return "Null";

                            var date = CprBroker.Utilities.Strings.PersonNumberToDate(p.PNR).Value;
                            if (p.NameStartDate.Value.Date == date)
                                return "Same Day";
                            if ((date - p.NameStartDate.Value).TotalDays < 30)
                                return "Month or less";
                            return "More than month";
                        })
                    .Select(g => new { Text = g.Key, Data = g.ToArray() })
                    .ToArray();
                var oo = "";
            }

            [Test]
            public void AAA()
            {
                var all = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);

                var grouped = all
                    .GroupBy(
                        p =>
                        {
                            var date = CprBroker.Utilities.Strings.PersonNumberToDate(p.PersonInformation.PNR).Value;
                            //DateTime? nameDate


                            if (p.HistoricalName.Count() == 0)
                            {
                                var n = p.CurrentNameInformation;
                                if (!n.NameStartDate.HasValue)
                                    return "1-1         Null         ";

                                if (n.NameStartDate.Value.Date == date)
                                    return "1-2         Same Day     ";
                                if ((date - n.NameStartDate.Value).TotalDays < 15)
                                    return "1-3       2 Weeks or less";
                                if ((date - n.NameStartDate.Value).TotalDays < 30)
                                    return "1-3         Month or less";
                                return "1-4         More than month";
                            }
                            var h = p.HistoricalName.OrderBy(n => n.NameStartDate).First();

                            if (!h.NameStartDate.HasValue)
                                return "2-1         Null       ";

                            if (h.NameStartDate.Value.Date == date)
                                return "2-2         Same Day   ";
                            if ((date - h.NameStartDate.Value).TotalDays < 15)
                                return "2-3     2 Weeks or less";
                            if ((date - h.NameStartDate.Value).TotalDays < 30)
                                return "2-3     Month or less  ";
                            return "2-4       More than month";
                        })
                        .Select(g => new { Status = g.Key, Data = g.ToArray() })
                        .OrderBy(g => g.Status)
                        .ToArray();
                object o = "";
            }
        }

        [TestFixture]
        public class GetOldestName
        {
            [Test]
            public void GetOldestName_Mask_Null()
            {
                var arr = new HistoricalNameType[]{ 
                    new HistoricalNameType(){CorrectionMarker = '*'}
                };
                var ret = HistoricalNameType.GetOldestName(arr);
                Assert.Null(ret);
            }

            [Test]
            public void GetOldestName_OK_GetsOldest()
            {
                var today = DateTime.Today;
                var arr = new HistoricalNameType[]{ 
                    new HistoricalNameType(){NameStartDate = today},
                    new HistoricalNameType(){NameStartDate = today.AddDays(-20)},
                };
                var ret = HistoricalNameType.GetOldestName(arr);
                Assert.AreEqual(arr[1], ret);
            }
        }

    }
}
