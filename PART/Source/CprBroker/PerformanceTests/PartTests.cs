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
