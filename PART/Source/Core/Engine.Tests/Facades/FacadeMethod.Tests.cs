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
            public void Aggregate_Mixed_ResultPropagatedOnlyForSuccess()
            {
                var facade = new FacadeStub();
                var elements = new FacadeStub.Element[] { new FacadeStub.ElementSub() { Output = "0", Succeeded = false }, new FacadeStub.ElementSub() { Output = "1", Succeeded = true } };
                var ret = facade.Aggregate<BasicOutputType<string[]>>(elements);
                Assert.Null(ret.Item[0]);
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
            public void BaseUpdateDatabase_Normal_InOutPassed()
            {
                var facade = new FacadeStub();
                facade.BaseUpdateDatabase(new FacadeStub.Element[] { new FacadeStub.Element() { Input = "Inp", Output = "Out" } });
                Assert.AreEqual("Inp", facade.inputs[0]);
                Assert.AreEqual("Out", facade.outputs[0]);
            }

        }

        [TestFixture]
        public class CallSingle
        {
            public class FacadeStub : FacadeMethod<ISingleDataProvider<string, string>, string, string>
            {
                public class ElementStub : Element
                {
                    public bool Updatable;
                    public bool Succeeded;
                }

                public override bool IsElementSucceeded(FacadeMethod<ISingleDataProvider<string, string>, string, string>.Element element)
                {
                    return (element as ElementStub).Succeeded;
                }

                public override bool IsElementUpdatable(FacadeMethod<ISingleDataProvider<string, string>, string, string>.Element element)
                {
                    return (element as ElementStub).Updatable;
                }

                public List<string> updatedInputs = new List<string>();
                public List<string> updatedOutputs = new List<string>();
                public override void UpdateDatabase(string[] input, string[] output)
                {
                    updatedInputs.AddRange(input);
                    updatedOutputs.AddRange(output);
                }

                public class ProviderStub : ISingleDataProvider<string, string>, IBatchDataProvider<string, string>, IExternalDataProvider
                {
                    public Func<string, string> _GetOne;
                    public string GetOne(string input)
                    {
                        if (_GetOne != null)
                        {
                            return _GetOne(input);
                        }
                        else
                        {
                            return null;
                        }
                    }

                    public bool _ImmediateUpdatePreferred = false;
                    public bool ImmediateUpdatePreferred
                    {
                        get { return _ImmediateUpdatePreferred; }
                    }

                    Dictionary<string, string> IExternalDataProvider.ConfigurationProperties
                    {
                        get
                        {
                            throw new NotImplementedException();
                        }
                        set
                        {
                            throw new NotImplementedException();
                        }
                    }

                    DataProviderConfigPropertyInfo[] IExternalDataProvider.ConfigurationKeys
                    {
                        get { throw new NotImplementedException(); }
                    }

                    bool IDataProvider.IsAlive()
                    {
                        throw new NotImplementedException();
                    }

                    Version IDataProvider.Version
                    {
                        get { throw new NotImplementedException(); }
                    }

                    public Func<string[], string[]> _GetBatch = null;
                    public string[] GetBatch(string[] inputs)
                    {
                        if (_GetBatch != null)
                            return _GetBatch(inputs);
                        else
                            return inputs;
                    }
                }
            }

            [Test]
            public void CallSingle_Normal_Success()
            {
                var facade = new FacadeStub();
                var prov = new FacadeStub.ProviderStub() { _GetOne = (s) => "ddd" + s };
                FacadeStub.Element[] ret;
                facade.CallSingle(prov, new FacadeStub.ElementStub[] { new FacadeStub.ElementStub() { Input = "DDD", Succeeded = true, Updatable = true } }, out ret);
                Assert.AreEqual(1, facade.updatedInputs.Count + ret.Length);
                Assert.AreEqual(1, facade.updatedOutputs.Count + ret.Length);
            }

            [Test]
            public void CallSingle_Normal_CorrectOuput()
            {
                var facade = new FacadeStub();
                var prov = new FacadeStub.ProviderStub() { _GetOne = (s) => "ddd" + s, _ImmediateUpdatePreferred = false };
                FacadeStub.Element[] ret;
                facade.CallSingle(prov, new FacadeStub.ElementStub[] { new FacadeStub.ElementStub() { Input = "DDD", Succeeded = true, Updatable = true } }, out ret);
                Assert.AreEqual("dddDDD", ret[0].Output);
            }

            [Test]
            public void CallSingle_SucceededImmediateUpdatePreferred_UpdateCalled()
            {
                var facade = new FacadeStub();
                var prov = new FacadeStub.ProviderStub() { _GetOne = (s) => "ddd" + s, _ImmediateUpdatePreferred = true };
                FacadeStub.Element[] ret;
                facade.CallSingle(prov, new FacadeStub.ElementStub[] { new FacadeStub.ElementStub() { Input = "DDD", Succeeded = true, Updatable = true } }, out ret);
                Assert.AreEqual(0, ret.Length);
                Assert.AreEqual(1, facade.updatedInputs.Count);
                Assert.AreEqual(1, facade.updatedOutputs.Count);
            }

            [Test]
            public void CallSingle_SucceededImmediateUpdateNotPreferred_UpdatePostponed()
            {
                var facade = new FacadeStub();
                var prov = new FacadeStub.ProviderStub() { _GetOne = (s) => "ddd" + s, _ImmediateUpdatePreferred = false };
                FacadeStub.Element[] ret;
                facade.CallSingle(prov, new FacadeStub.ElementStub[] { new FacadeStub.ElementStub() { Input = "DDD", Succeeded = true, Updatable = true } }, out ret);
                Assert.AreEqual(1, ret.Length);
                Assert.AreEqual(0, facade.updatedInputs.Count);
                Assert.AreEqual(0, facade.updatedOutputs.Count);
            }

            [Test]
            public void CallSingle_Failed_ResultPropagated()
            {
                var facade = new FacadeStub();
                var prov = new FacadeStub.ProviderStub() { _GetOne = (s) => s, _ImmediateUpdatePreferred = false };
                FacadeStub.Element[] ret;
                var elements = new FacadeStub.ElementStub[] { new FacadeStub.ElementStub() { Input = "DDD", Succeeded = false, Updatable = false } };
                facade.CallSingle(prov, elements, out ret);
                Assert.AreEqual("DDD", elements[0].Output);
            }

            [Test]
            public void CallSingle_Exception_PassedAndNoOutput()
            {
                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
                var facade = new FacadeStub();
                var prov = new FacadeStub.ProviderStub()
                {
                    _GetOne = (s) =>
                    {
                        throw new Exception();
                    }
                };
                FacadeStub.Element[] ret;
                var elements = new FacadeStub.ElementStub[] { 
                    new FacadeStub.ElementStub() { Input = "DDD", Succeeded = true, Updatable = true } ,
                    new FacadeStub.ElementStub() { Input = "SSS", Succeeded = true, Updatable = true } };
                facade.CallSingle(prov, elements, out ret);
                Assert.AreEqual("DDD", elements[0].Input);
                Assert.Null(elements[0].Output);

                Assert.AreEqual("SSS", elements[1].Input);
                Assert.Null(elements[1].Output);
            }
        }

        [TestFixture]
        public class CallBatch
        {
            [Test]
            public void CallBatch_NormalSuccess_CorrectOutputUpdate()
            {
                var facade = new CallSingle.FacadeStub();
                CallSingle.FacadeStub.Element[] ret;
                var prov = new CallSingle.FacadeStub.ProviderStub();
                var elements = new CallSingle.FacadeStub.ElementStub[] { 
                    new CallSingle.FacadeStub.ElementStub() { Input = "DDD", Succeeded = true, Updatable = true } ,
                    new CallSingle.FacadeStub.ElementStub() { Input = "SSS", Succeeded = true, Updatable = true } };

                facade.CallBatch(prov, elements, out ret);
                Assert.AreEqual(2, ret.Length);

                Assert.AreEqual("DDD", ret[0].Input);
                Assert.AreEqual("DDD", ret[0].Output);

                Assert.AreEqual("SSS", ret[1].Input);
                Assert.AreEqual("SSS", ret[1].Output);
            }

            [Test]
            public void CallBatch_UnmatchedCount_ExceptionPassedAndEmptyReturn()
            {
                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
                var facade = new CallSingle.FacadeStub();
                CallSingle.FacadeStub.Element[] ret;
                var prov = new CallSingle.FacadeStub.ProviderStub() { _GetBatch = (s) => new string[] { "SSS" } };
                var elements = new CallSingle.FacadeStub.ElementStub[] { 
                    new CallSingle.FacadeStub.ElementStub() { Input = "DDD", Succeeded = true, Updatable = true } ,
                    new CallSingle.FacadeStub.ElementStub() { Input = "SSS", Succeeded = true, Updatable = true } };

                facade.CallBatch(prov, elements, out ret);
                Assert.NotNull(ret);
                Assert.IsEmpty(ret);
            }

            [Test]
            public void CallBatch_MixedSuccess_Correct()
            {
                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
                var facade = new CallSingle.FacadeStub();
                CallSingle.FacadeStub.Element[] ret;
                var prov = new CallSingle.FacadeStub.ProviderStub();
                var elements = new CallSingle.FacadeStub.ElementStub[] { 
                    new CallSingle.FacadeStub.ElementStub() { Input = "DDD", Succeeded = false, Updatable = false } ,
                    new CallSingle.FacadeStub.ElementStub() { Input = "SSS", Succeeded = true, Updatable = true } };

                facade.CallBatch(prov, elements, out ret);
                Assert.AreEqual(1, ret.Length);
            }
        }

        [TestFixture]
        public class CallDataProviders
        {
            public class FacadeStub : FacadeMethod<ISingleDataProvider<string, string>, string, string>
            {
                public class ProviderStub : IBatchDataProvider<string, string>
                {
                    public Func<string[], string[]> _GetBatch = null;
                    public string[] GetBatch(string[] inputs)
                    {
                        if (_GetBatch != null)
                            return _GetBatch(inputs);
                        else
                            return inputs;
                    }

                    public string GetOne(string input)
                    {
                        throw new NotImplementedException();
                    }

                    public bool ImmediateUpdatePreferred
                    {
                        get { throw new NotImplementedException(); }
                    }
                }
            }

            [Test]
            public void CallDataProviders_NoProv_Fail()
            {
                var facade = new FacadeStub();
                var provs = new ISingleDataProvider<string, string>[] { };
                var ret = facade.CallDataProviders(provs, new string[] { "SSS", "DDD" });
                Assert.AreEqual(2, ret.Length);

                Assert.AreEqual("SSS", ret[0].Input);
                Assert.Null(ret[0].Output);

                Assert.AreEqual("DDD", ret[1].Input);
                Assert.Null(ret[1].Output);
            }

            [Test]
            public void CallDataProviders_ProvWithPartial_ProvidersCascaded()
            {
                var facade = new FacadeStub();
                var provs = new ISingleDataProvider<string, string>[] { 
                    new FacadeStub.ProviderStub() { _GetBatch = (i) => new string[] { "SSS", null }} ,
                    new FacadeStub.ProviderStub() { _GetBatch = (i) => new string[] { "DDD"}} 
                };

                var ret = facade.CallDataProviders(provs, new string[] { "SSS", "DDD" });

                Assert.AreEqual(2, ret.Length);

                Assert.AreEqual("SSS", ret[0].Input);
                Assert.AreEqual("SSS", ret[0].Output);

                Assert.AreEqual("DDD", ret[1].Input);
                Assert.AreEqual("DDD", ret[1].Output);
            }

            [Test]
            public void CallDataProviders_FirstProviderException_ProvidersCascaded()
            {
                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
                var facade = new FacadeStub();
                bool called = false;
                var provs = new ISingleDataProvider<string, string>[] { 
                    new FacadeStub.ProviderStub() { _GetBatch = (i) => {called=true; throw new Exception();}} ,
                    new FacadeStub.ProviderStub() { _GetBatch = (i) => new string[] {null, "DDD"}} 
                };

                Assert.False(called);
                var ret = facade.CallDataProviders(provs, new string[] { "SSS", "DDD" });
                Assert.True(called);

                Assert.AreEqual(2, ret.Length);

                Assert.AreEqual("SSS", ret[0].Input);
                Assert.Null(ret[0].Output);

                Assert.AreEqual("DDD", ret[1].Input);
                Assert.AreEqual("DDD", ret[1].Output);
            }
        }

        [TestFixture]
        public class CreateDataProviders
        {
            public class FunkyDataProvider:ISingleDataProvider<string, string>
            {
                public string GetOne(string input)
                {
                    throw new NotImplementedException();
                }

                public bool ImmediateUpdatePreferred
                {
                    get { throw new NotImplementedException(); }
                }
            }
            class FacadeStub : FacadeMethod<FunkyDataProvider, string, string>
            { }

            [Test]
            public void CreateDataProviders_FunkyType_None()
            {
                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
                var facade = new FacadeStub();
                IEnumerable<FunkyDataProvider> provs;
                var ret = facade.CreateDataProviders(out provs);
                Assert.IsEmpty(provs.ToArray());
                Assert.AreEqual("503",ret.StatusKode);
            }            
        }
    }
}
