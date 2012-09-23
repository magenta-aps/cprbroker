using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PerformanceTests.Part;

namespace PerformanceTests
{
    [TestFixture]
    public class PartTests
    {
        [Test]
        public void ListDPR()
        {
            var service = new Part.Part() { ApplicationHeaderValue = new ApplicationHeader() { ApplicationToken = "07059250-E448-4040-B695-9C03F9E59E38", UserToken = "" } };

            var input = new ListInputType() { UUID = Properties.Resources.Successful_DPR_UUIDs.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries).Take(100).ToArray() };
            //input.UUID = new string[] { "{f37779c4-358a-4c89-87e4-03a235a87f5f}" }; // "0101531538"
            var start = DateTime.Now;
            var output = service.List(input);
            var end = DateTime.Now;
            int success = output.LaesResultat.Where(p => p.Item != null).Count();
            int fail = output.LaesResultat.Where(p => p.Item == null).Count();
            Console.WriteLine(string.Format("List <{0}> persons - Time: <{1}>, Success = <{2}>, Failed = <{3}>", input.UUID.Length, end - start, success, fail));
            
        }

        [Test]
        public void ListRandom500()
        {
            System.Diagnostics.Debugger.Launch();
            var service = new Part.Part() { ApplicationHeaderValue = new ApplicationHeader() { ApplicationToken = "07059250-E448-4040-B695-9C03F9E59E38", UserToken = "" } };
            var uuids = new List<string>(Properties.Resources.All_UUIDs.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries));
            Random r = new Random();
            while (uuids.Count > 500)
            {
                int index = r.Next(0, uuids.Count);
                uuids.RemoveAt(index);
            }
            var input = new ListInputType() { UUID = uuids.ToArray() };
            
            var start = DateTime.Now;
            var output = service.List(input);
            var end = DateTime.Now;
            int success = output.LaesResultat.Where(p => p.Item != null).Count();
            int fail = output.LaesResultat.Where(p => p.Item == null).Count();
            Console.WriteLine(string.Format("List <{0}> persons - Time: <{1}>, Success = <{2}>, Failed = <{3}>", input.UUID.Length, end - start, success, fail));

        }

        [Test]
        public void GetUuid()
        {
            var service = new Part.Part() { ApplicationHeaderValue = new ApplicationHeader() { ApplicationToken = "07059250-E448-4040-B695-9C03F9E59E38", UserToken = "" } };
            var cprNumbers = Properties.Resources.DPR_PNRs.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var start = DateTime.Now;
            var ret = new List<GetUuidOutputType>();
            foreach (var pnr in cprNumbers)
            {
                var pnr2 = pnr.PadLeft(10,'0');
                ret.Add(service.GetUuid(pnr2));
            }            
            var end = DateTime.Now;
            
            
            int success = ret.Where(p => !string.IsNullOrEmpty(p.UUID)).Count();
            int fail = ret.Where(p => string.IsNullOrEmpty(p.UUID)).Count();
            Console.WriteLine(string.Format("List <{0}> persons - Time: <{1}>, Success = <{2}>, Failed = <{3}>", cprNumbers.Length, end - start, success, fail));
        }

        [Test]
        public void GetUuidArray()
        {
            var service = new Part.Part() { ApplicationHeaderValue = new ApplicationHeader() { ApplicationToken = "07059250-E448-4040-B695-9C03F9E59E38", UserToken = "" } };
            var cprNumbers = Properties.Resources.DPR_PNRs.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(pnr=>pnr).ToArray();
            var start = DateTime.Now;
            var ret = service.GetUuidArray(cprNumbers);
                        
            var end = DateTime.Now;
            
            
            int success = ret.Where(p => !string.IsNullOrEmpty(p.UUID)).Count();
            int fail = ret.Where(p => string.IsNullOrEmpty(p.UUID)).Count();
            Console.WriteLine(string.Format("List <{0}> persons - Time: <{1}>, Success = <{2}>, Failed = <{3}>", cprNumbers.Length, end - start, success, fail));
        }

    }
}
