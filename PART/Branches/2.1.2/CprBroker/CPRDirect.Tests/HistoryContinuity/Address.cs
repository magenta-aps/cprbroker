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
using CprBroker.Schemas.Part;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect.HistoryContinuity
{
    [TestFixture]
    public class Address : Base<IAddressSource>
    {
        protected override IAddressSource GetCurrent(IndividualResponseType pers)
        {
            return pers.GetFolkeregisterAdresseSource(false);
        }

        protected override List<IAddressSource> GetHistorical(IndividualResponseType pers)
        {
            return pers.HistoricalAddress.Where(a => a.CorrectionMarker == ' ').Select(a => a as IAddressSource)
                .Concat(pers.HistoricalDeparture.Where(d => d.CorrectionMarker == ' ').Select(d => d as IAddressSource))
                .Concat(pers.HistoricalDisappearance.Where(d => d.CorrectionMarker == ' ').Select(d => d as IAddressSource))
                .ToList();
        }


        [Test]
        [TestCaseSource("PNRs")]
        public override void HistoryContinues(string pnr)
        {
            base.HistoryContinues(pnr);
        
            /*
             * // Person entry date has no timestamp, current address started on same date but with a timestamp
             * // Historical address(023) has correction marker and is ignored        
             * 0230708610089A00110071001                                             200110241527 200110241527
             * 0240708610089 0000000000000000 5150200111110000 Via di Sole 49                    I 13589 Bari                      Italien
             * 002070861008908518511033 02  tv                                      200111111452 200111111452 0011000000000000 0000        
            */
        }

        [Test]
        [TestCaseSource("PNRs")]
        public override void Current_NotNull(string pnr)
        {
            base.Current_NotNull(pnr);


            /*
             * // Deleted PNR
             * 0010101520013010752903950200111071450 M1952-01-01 1997-09-09 2001-11-07 Translatør
             * 
             * // Dead
             * 0010101980014          90200006300000 K1998-01-01 1998-01-01
             * 
             * Changed PNR
             * 0010108610069070961001560200111130859 M1961-08-01 2001-10-22 2001-11-13 Forlægger
             * 
             * Changed PNR
             * 0010708614246030761007860200111071551 K1961-08-07 1961-08-07*2001-11-07 Designer
             * 
             * //Dead
             * 0010708614327          90200006301004 M1961-08-07 1961-08-07*           Blikkensl.
             * 
             * Changed PNR
             * 0010709614398090961003660200111071559 K1961-09-07 1961-09-07*2001-11-07 Sygehjælper
             * 
             * Dead
             * 0010901414084          90196103132000 K1941-01-09 1941-01-09*           Ekspedient
             */
        }
    }
}
