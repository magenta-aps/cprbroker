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

namespace CprBroker.NUnitTester
{
    public abstract class BaseTest
    {
        [TestFixtureSetUp]
        public void Initialize()
        {
            TestRunner.Initialize();
            TestData.Initialize();
        }

        public void Validate(string code, string text)
        {
            Assert.IsNotNullOrEmpty(code, "Status Code");
            Assert.AreEqual("200", code, "Status Code. Text = <{0}>",text);
        }

        public void ValidateInvalid(string code, string text)
        {
            Assert.IsNotNullOrEmpty(code, "Status Code");
            Assert.AreNotEqual("200", code, "Status Code");
        }

    }

    public abstract class PartBaseTest : BaseTest
    {
        #region Utility methods

        protected void Validate(Part.StandardReturType ret)
        {
            Assert.IsNotNull(ret);
            base.Validate(ret.StatusKode, ret.FejlbeskedTekst);
        }

        protected void ValidateInvalid(Part.StandardReturType ret)
        {
            Assert.IsNotNull(ret);
            base.ValidateInvalid(ret.StatusKode, ret.FejlbeskedTekst);
        }

        protected void Validate(Guid uuid, Part.LaesOutputType laesOutput, Part.Part service)
        {
            Assert.IsNotNull(laesOutput, "Laes output is null{0}", uuid);
            Validate(laesOutput.StandardRetur);
            Validate(uuid, laesOutput.LaesResultat, service);
        }

        protected void Validate(Guid uuid, Part.LaesResultatType laesResultat, Part.Part service)
        {
            Assert.IsNotNull(laesResultat, "Person not found : {0}", uuid);
            if (laesResultat.Item is Part.RegistreringType1)
            {
                var reg = laesResultat.Item as Part.RegistreringType1;
                Assert.NotNull(reg.AktoerRef, "Empty actor");
                Assert.AreNotEqual("", reg.AktoerRef.Item, "Empty actor text");
                Assert.AreNotEqual(Guid.Empty.ToString(), reg.AktoerRef.Item, "Empty actor text");

                Assert.NotNull(reg.AttributListe, "Attributes");
                Assert.NotNull(reg.AttributListe.Egenskab, "AttributListe.Egenskab");
                Assert.Greater(reg.AttributListe.Egenskab.Length, 0, "AttributListe.Egenskab.Length");

                Assert.NotNull(reg.AttributListe.Egenskab[0].BirthDate, "Birthdate");
                Assert.NotNull(reg.AttributListe.Egenskab[0].NavnStruktur, "Name");
                //Assert.NotNull(reg.AttributListe.Egenskaber[0].RegisterOplysninger, "Birthdate");
                Assert.NotNull(reg.AttributListe.Egenskab[0].Virkning, "Effect");

                Assert.NotNull(reg.TilstandListe, "States");

                Assert.NotNull(reg.RelationListe, "Relations");
            }
            else if (laesResultat.Item is Part.FiltreretOejebliksbilledeType)
            {
                var reg = laesResultat.Item as Part.FiltreretOejebliksbilledeType;
                Assert.NotNull(reg.AttributListe, "Attributes");
                Assert.NotNull(reg.AttributListe.Egenskab, "AttributListe.Egenskab");
                Assert.Greater(reg.AttributListe.Egenskab.Length, 0, "AttributListe.Egenskab.Length");

                Assert.NotNull(reg.AttributListe.Egenskab[0].BirthDate, "Birthdate");
                Assert.NotNull(reg.AttributListe.Egenskab[0].NavnStruktur, "Name");
                //Assert.NotNull(reg.AttributListe.Egenskaber[0].RegisterOplysninger, "Birthdate");
                Assert.NotNull(reg.AttributListe.Egenskab[0].Virkning, "Effect");

                Assert.NotNull(reg.TilstandListe, "States");

                Assert.NotNull(reg.RelationListe, "Relations");
            }
            else
            {
                Assert.Fail(String.Format("Unknown laesResultat object type:{0}", laesResultat.Item));
            }

            //Assert.AreNotEqual(String.Empty, laesResultat.LaesResultat..ActorId);

            //Assert.IsNotNull(service.QualityHeaderValue, "Quality header");
            //Assert.IsNotNull(service.QualityHeaderValue.QualityLevel, "Quality header value");
        }

        protected void Validate(Part.GetUuidOutputType uuid)
        {
            Assert.NotNull(uuid);
            Validate(uuid.StandardRetur);
            Assert.NotNull(uuid.UUID);
            Assert.AreNotEqual(uuid.UUID, Guid.Empty.ToString());
        }

        protected void ValidateInvalid(Part.GetUuidOutputType uuid)
        {
            Assert.NotNull(uuid);
            ValidateInvalid(uuid.StandardRetur);
            Assert.IsNullOrEmpty(uuid.UUID);
        }

        #endregion

    }
}
