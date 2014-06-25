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

namespace CprBroker.Tests.PartInterface
{
    namespace CivilStatusWrapperTests.Tests
    {
        class CivilStatusStub : ICivilStatus
        {
            public string PNR { get; set; }

            public char CivilStatusCode { get; set; }

            public DateTime? CivilStatusStartDate { get; set; }
            public DateTime? ToStartTS()
            {
                return CivilStatusStartDate;
            }
            public DateTime? CivilStatusEndDate { get; set; }
            public DateTime? ToEndTS()
            {
                return CivilStatusEndDate;
            }

            public string SpousePnr { get; set; }
            public string ToSpousePnr()
            {
                return SpousePnr;
            }

            public bool Valid { get; set; }
            public bool IsValid()
            {
                return Valid;
            }

            public DataTypeTags Tag { get; set; }
            public IRegistrationInfo Registration { get; set; }


            public bool _StartTSCertainty;
            public bool ToStartTSCertainty()
            { return _StartTSCertainty; }
            public bool _EndTSCertainty;
            public bool ToEndTSCertainty()
            { return _EndTSCertainty; }
        }

        class SeparationStub : ISeparation
        {
            public CivilStatusType ToCivilStatusType()
            {
                return new CivilStatusType()
                {
                    CivilStatusKode = CivilStatusKodeType.Separeret,
                    TilstandVirkning = TilstandVirkningType.Create(ToStartTS())
                };
            }
            
            public DateTime? ToStartTS()
            { return null; }

            public DateTime? ToEndTS()
            { return null; }

            public bool _StartTSCertainty;
            public bool ToStartTSCertainty()
            { return _StartTSCertainty; }
            public bool _EndTSCertainty;
            public bool ToEndTSCertainty()
            { return _EndTSCertainty; }
            
            public DataTypeTags Tag { get; set; }
            public IRegistrationInfo Registration { get; set; }
        }

        [TestFixture]
        public class ToCivilStatusType
        {
            [Test]
            public void ToCivilStatusType_Normal_NotNull()
            {
                var w = new CivilStatusWrapper(new CivilStatusStub() { CivilStatusCode = 'U' });
                var ret = w.ToCivilStatusType(null);
                Assert.NotNull(ret);
            }

            [Test]
            public void ToCivilStatusType_G_Married()
            {
                var w = new CivilStatusWrapper(new CivilStatusStub() { CivilStatusCode = 'G' });
                var ret = w.ToCivilStatusType(null);
                Assert.AreEqual(CivilStatusKodeType.Gift, ret.CivilStatusKode);
            }

            [Test]
            public void ToCivilStatusType_GwithSeparation_Separated()
            {
                var w = new CivilStatusWrapper(new CivilStatusStub() { CivilStatusCode = 'G' });
                var ret = w.ToCivilStatusType(new SeparationStub() { });
                Assert.AreEqual(CivilStatusKodeType.Separeret, ret.CivilStatusKode);
            }

        }
    }
}
