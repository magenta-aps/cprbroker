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
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.Engine.Facades
{
    namespace FacadeMethodTests
    {
        [TestFixture]
        public class Aggregate
        {
            class FacadeStub : FacadeMethod<ISingleDataProvider<string, string>, string, string>
            {
                public override bool IsElementSucceeded(Element element)
                {
                    if (element is ElementSub)
                        return (element as ElementSub).Succeeded;
                    else
                        return base.IsElementSucceeded(element);
                }

                public class ElementSub : Element
                {
                    public bool Succeeded = false;
                }
            }

            [Test]
            [ExpectedException]
            public void Aggregate_Null_Exception()
            {
                var facade = new FacadeStub();
                facade.Aggregate<BasicOutputType<string[]>>(null);
            }

            [Test]
            public void Aggregate_AllFailed_503()
            {
                var facade = new FacadeStub();
                var elements = new FacadeStub.Element[] { new FacadeStub.ElementSub() { Succeeded = false } };
                var ret = facade.Aggregate<BasicOutputType<string[]>>(elements);
                Assert.AreEqual("503", ret.StandardRetur.StatusKode);
            }

            [Test]
            public void Aggregate_AllSuccess_200()
            {
                var facade = new FacadeStub();
                var elements = new FacadeStub.Element[] { new FacadeStub.ElementSub() { Succeeded = true } };
                var ret = facade.Aggregate<BasicOutputType<string[]>>(elements);
                Assert.AreEqual("200", ret.StandardRetur.StatusKode);
            }

            [Test]
            public void Aggregate_Mixed_206()
            {
                var facade = new FacadeStub();
                var elements = new FacadeStub.Element[] { new FacadeStub.ElementSub() { Succeeded = false }, new FacadeStub.ElementSub() { Succeeded = true } };
                var ret = facade.Aggregate<BasicOutputType<string[]>>(elements);
                Assert.AreEqual("206", ret.StandardRetur.StatusKode);
            }

            [Test]
            public void Aggregate_Mixed_ResultPropagated()
            {
                var facade = new FacadeStub();
                var elements = new FacadeStub.Element[] { new FacadeStub.ElementSub() { Output = "0", Succeeded = false }, new FacadeStub.ElementSub() { Output = "1", Succeeded = true } };
                var ret = facade.Aggregate<BasicOutputType<string[]>>(elements);
                Assert.AreEqual("0", ret.Item[0]);
                Assert.AreEqual("1", ret.Item[1]);
            }

            [Test]
            public void Aggregate_Mixed_CorrectReasonAndInput()
            {
                var facade = new FacadeStub();
                var elements = new FacadeStub.Element[] { new FacadeStub.ElementSub() { Input = "0Inp", Output = "0", Succeeded = false, PossibleErrorReason = "SSSSS" }, new FacadeStub.ElementSub() { Output = "1", Succeeded = true } };
                var ret = facade.Aggregate<BasicOutputType<string[]>>(elements);

                Assert.Greater(ret.StandardRetur.FejlbeskedTekst.IndexOf("SSSSS"), -1);
                Assert.Greater(ret.StandardRetur.FejlbeskedTekst.IndexOf("0Inp"), -1);
            }
        }

        [TestFixture]
        public class BaseUpdateDatabase
        {
            class FacadeStub : FacadeMethod<ISingleDataProvider<string, string>, string, string>
            {
                public string[] inputs, outputs;
                public override void UpdateDatabase(string[] input, string[] output)
                {
                    inputs = input;
                    outputs = output;
                }
            }

            [Test]
            [ExpectedException]
            public void BaseUpdateDatabase_Null_Exception()
            {
                var facade = new FacadeStub();
                facade.BaseUpdateDatabase(null);
            }

            [Test]
            public void BaseUpdateDatabase_Normal_InOutPassed()
            {
                var facade = new FacadeStub();
                facade.BaseUpdateDatabase(new FacadeStub.Element[] { new FacadeStub.Element() { Input = "Inp", Output = "Out" } });
                Assert.AreEqual("Inp",facade.inputs[0]);
                Assert.AreEqual("Out", facade.outputs[0]);
            }

        }
    }
}
