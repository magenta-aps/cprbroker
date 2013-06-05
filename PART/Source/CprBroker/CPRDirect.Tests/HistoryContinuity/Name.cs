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
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
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
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect.HistoryContinuity
{
    [TestFixture]
    public class Names : Base<INameSource>
    {
        protected override INameSource GetCurrent(IndividualResponseType pers)
        {
            return pers.CurrentNameInformation;
        }

        protected override List<INameSource> GetHistorical(IndividualResponseType pers)
        {
            return pers.HistoricalName.Where(n => n.CorrectionMarker == ' ').Select(n => n as INameSource).ToList();
        }


        // It is now clear that historical records come in the order from older to newer
        // Those with a correction marker seem OK to drop

        [Test]
        [TestCaseSource(typeof(Utilities), "PNRs")]
        public override void HistoryContinues(string pnr)
        {
            base.HistoryContinues(pnr);

            // Invalid data. Second row in historical data has start date > end date !!!!!!!!!!
            /*
             * 026 0709614096 Bettina    Kristensen      1961-11-07 12:00   1961-10-07 20:00
             * 026 0709614096 Bettina    Christiansen    1961-10-07 20:00   2000-12-31 10:20
             * 008 0709614096 Bettina    Folmersen       2000-12-31 10:20
            */


            // TODO: Start date for first name is uncertain and unconvertible to date time, what do we do
            // No problem here, but needs to be thought of
            /*            
             * 026 0709614118 Gitte    Nielsen   19610000 1299 * 19611107 1200
             * 026 0709614118 Gitte    Jensen    19611107 1200   19790501 2000
             * 008 0709614118 Gitte    Sander    19790501 2000
            */

        }



    }
}
