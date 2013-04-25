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
using CprBroker.Providers.CPRDirect;
using NUnit.Framework;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace CprBroker.Tests.CPRDirect.Objects
{
    namespace WrapperTests
    {
        public class WrapperStub : Wrapper
        {
            public int _Length;
            public override int Length
            {
                get { return _Length; }
            }

            public WrapperStub()
            {
            }
            public WrapperStub(int len)
                : base(len)
            {
            }
        }

        [TestFixture]
        public class Contents
        {
            [Test]
            [ExpectedException(typeof(ArgumentOutOfRangeException))]
            public void Contents_InvalidLength_ThrowsException(
                [Random(0, 10000, 5)]int len,
                [Values(1, 30, -12)]int diff)
            {
                var w = new WrapperStub() { _Length = len };
                w.Contents = new string('s', len + diff);
            }

            [Test]
            public void Contents_InvalidLength_ThrowsException(
                [Random(0, 10000, 5)]int len)
            {
                var w = new WrapperStub() { _Length = len };
                var val = new string('s', len);
                w.Contents = val;
                Assert.AreEqual(val, w.Contents);
            }
        }

        class WrapperStub1 : Wrapper
        {
            public override int Length
            {
                get { return 6; }
            }
        }
        class WrapperStub2 : Wrapper
        {
            public override int Length
            {
                get { return 6; }
            }
        }

        class ParentWrapperStub : Wrapper
        {
            public override int Length
            {
                get { return 12; }
            }
            [MinMaxOccurs(0, 1)]
            public WrapperStub1 WrapperStub1 { get; set; }


            private List<WrapperStub2> _WrapperStub2 = new List<WrapperStub2>();
            [MinMaxOccurs(0, 1000)]
            public List<WrapperStub2> WrapperStub2
            {
                get { return _WrapperStub2; }
                set { _WrapperStub2 = value; }
            }
        }

        [TestFixture]
        public class Parse
        {
            Dictionary<string, Type> WrapperMap;

            [SetUp]
            public void Initialize()
            {
                WrapperMap = new Dictionary<string, Type>();
                WrapperMap["001"] = typeof(WrapperStub1);
                WrapperMap["002"] = typeof(WrapperStub2);
            }

            [Test]
            [ExpectedException]
            public void Parse_RandomCode_Exception()
            {
                string data = Guid.NewGuid().ToString();
                var wrappers = Wrapper.Parse(data, new Dictionary<string, Type>());
                Assert.IsEmpty(wrappers);
            }
            [Test]
            public void Parse_Normal_CorrectCount()
            {
                string data = "001AAA002BBB";

                var wrappers = Wrapper.Parse(data, WrapperMap);
                Assert.AreEqual(2, wrappers.Count);
            }

            [Test]
            [ExpectedException]
            public void Parse_TooShortString_Exception()
            {
                string data = "001";
                Wrapper.Parse(data, WrapperMap);
            }

            [Test]
            [ExpectedException]
            public void Parse_TooLongString_Exception()
            {
                string data = "001" + Guid.NewGuid().ToString();
                Wrapper.Parse(data, WrapperMap);
            }

            Random Random = new Random();
            [Test]
            public void Parse_MixedCount_CorrectCount(
                [Random(10, 200, 5)]int w1Count,
                [Random(10, 200, 5)]int w2Count)
            {
                List<bool> arr = new List<bool>(new bool[w1Count + w2Count]);
                for (int i = 0; i < w1Count; i++)
                {
                    arr[i] = true;
                }
                for (int i = 0; i < w2Count; i++)
                {
                    arr[i + w1Count] = false;
                }
                string data = "";
                while (arr.Count > 0)
                {
                    var index = Random.Next(0, arr.Count);
                    string code = (arr[index]) ? "001" : "002";
                    data += code + Guid.NewGuid().ToString().Substring(0, 3);
                    arr.RemoveAt(index);
                }
                var wrappers = Wrapper.Parse(data, WrapperMap);
                Assert.AreEqual(w1Count, wrappers.Where(w => w is WrapperStub1).Count());
                Assert.AreEqual(w2Count, wrappers.Where(w => w is WrapperStub2).Count());
            }
        }

        [TestFixture]
        public class FillFrom
        {
            [Test]
            public void FillFrom_Zero_OK()
            {
                var wrappers = new Wrapper[0];
                new ParentWrapperStub().FillFrom(wrappers);
            }
            [Test]
            [ExpectedException]
            public void FillFrom_TwoForSinglePosition_Exception()
            {
                var wrappers = new Wrapper[] { new WrapperStub1(), new WrapperStub1() };
                new ParentWrapperStub().FillFrom(wrappers);
            }

            [Test]
            public void FillFrom_Multiple_CorrectCounts(
                [Random(10, 200, 5)]int w2Count)
            {
                var wrappers = new List<Wrapper>();
                wrappers.Add(new WrapperStub1());
                for (int i = 0; i < w2Count; i++)
                {
                    wrappers.Add(new WrapperStub2());
                }
                var par = new ParentWrapperStub();
                par.FillFrom(wrappers);
                Assert.NotNull(par.WrapperStub1);
                Assert.AreEqual(w2Count, par.WrapperStub2.Count);
                Assert.AreEqual(w2Count, par.WrapperStub2.Where(w => w is WrapperStub2).Count());
            }


            private Wrapper CreateParentWrapper(int minOccurs, int maxOccurs)
            {
                AssemblyBuilder asmBuilder;
                ModuleBuilder modBuilder;
                AssemblyName assemblyName = new AssemblyName();
                assemblyName.Name = "DynamicORMapper";
                AppDomain thisDomain = Thread.GetDomain();
                asmBuilder = thisDomain.DefineDynamicAssembly(assemblyName,
                             AssemblyBuilderAccess.Run);

                modBuilder = asmBuilder.DefineDynamicModule(
                             asmBuilder.GetName().Name, false);

                TypeBuilder typeBuilder = modBuilder.DefineType("ParentWrapperRuntimeType",
                    TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout,
                    typeof(WrapperStub),
                    Type.EmptyTypes
                    );
                
                FieldBuilder fieldBuilder = typeBuilder.DefineField("_ChildWrapper", typeof(List<WrapperStub1>),FieldAttributes.Private);

                var propertyBuilder = typeBuilder.DefineProperty("ChildWrapper", PropertyAttributes.None, typeof(List<WrapperStub1>), new Type[] { });
                
                MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

                // Define the "get" accessor method for ChildWrapper.
                MethodBuilder custNameGetPropMthdBldr = typeBuilder.DefineMethod("get_ChildWrapper", getSetAttr, typeof(List<WrapperStub1>),Type.EmptyTypes);
                ILGenerator childWrapperGetIL = custNameGetPropMthdBldr.GetILGenerator();

                childWrapperGetIL.Emit(OpCodes.Ldarg_0);
                childWrapperGetIL.Emit(OpCodes.Ldfld, fieldBuilder);
                childWrapperGetIL.Emit(OpCodes.Ret);

                // Define the "set" accessor method for ChildWrapper.
                MethodBuilder custNameSetPropMthdBldr = typeBuilder.DefineMethod("set_ChildWrapper",getSetAttr,null,new Type[] { typeof(List<WrapperStub1>) });

                ILGenerator childWrapperSetIL = custNameSetPropMthdBldr.GetILGenerator();

                childWrapperSetIL.Emit(OpCodes.Ldarg_0);
                childWrapperSetIL.Emit(OpCodes.Ldarg_1);
                childWrapperSetIL.Emit(OpCodes.Stfld, fieldBuilder);
                childWrapperSetIL.Emit(OpCodes.Ret);

                // Last, we must map the two methods created above to our PropertyBuilder to 
                // their corresponding behaviors, "get" and "set" respectively. 
                propertyBuilder.SetGetMethod(custNameGetPropMthdBldr);
                propertyBuilder.SetSetMethod(custNameSetPropMthdBldr);


                var attrCon = typeof(MinMaxOccurs).GetConstructor(new Type[] { typeof(int), typeof(int) });
                var attr = new CustomAttributeBuilder(attrCon, new object[] { minOccurs, maxOccurs });
                propertyBuilder.SetCustomAttribute(attr);

                var parentWrapperType = typeBuilder.CreateType();
                var parentWrapper = parentWrapperType.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                parentWrapperType.InvokeMember("ChildWrapper", BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance, null, parentWrapper, new object[] { new List<WrapperStub1>() });
                return parentWrapper as Wrapper;
            }


            private List<Wrapper> CreateWrapperArray(Type t, int count)
            {
                var ret = new List<Wrapper>();
                for (int i = 0; i < count; i++)
                {
                    ret.Add(t.GetConstructor(Type.EmptyTypes).Invoke(new object[0]) as Wrapper);
                }
                return ret;
            }


            [Test]
            [ExpectedException(typeof(ArgumentOutOfRangeException))]
            public void Fillfrom_LessThanMinOccurs_Exception(
                [Random(1, 100, 5)] int minOccurs,
                [Random(0, 100, 5)] int maxOccursDiff)
            {
                var parentWrapper = CreateParentWrapper(minOccurs, minOccurs + maxOccursDiff);
                var childWrappers = CreateWrapperArray(typeof(WrapperStub1), minOccurs - 1);
                parentWrapper.FillFrom(childWrappers);
            }

            [Test]
            [ExpectedException(typeof(ArgumentOutOfRangeException))]
            public void Fillfrom_GreaterThanMaxOccurs_Exception(
                [Random(1, 100, 5)] int minOccurs,
                [Random(0, 100, 5)] int maxOccursDiff)
            {
                var parentWrapper = CreateParentWrapper(minOccurs, minOccurs + maxOccursDiff);
                var childWrappers = CreateWrapperArray(typeof(WrapperStub1), minOccurs + maxOccursDiff + 1);
                parentWrapper.FillFrom(childWrappers);
            }

            public class StartRecordWrapperStub : Wrapper
            {
                [MinMaxOccurs(1, 1)]
                public StartRecordType StartRecord { get; set; }

                public override int Length
                {
                    get { return 0; }
                }
                public StartRecordWrapperStub()
                { }
            }

            [Test]
            [ExpectedException(typeof(InvalidOperationException))]
            public void FillFrom_MultipleStartRecords_Exception()
            {
                var w = new StartRecordWrapperStub();
                w.FillFrom(new Wrapper[] { new StartRecordType() }, new StartRecordType(), null);
            }
        }

        [TestFixture]
        public class GetDateTime
        {
            [Test]
            public void GetDateTime_Normal_OK()
            {
                var format = "yyyyMMdd";
                var s = DateTime.Today.ToString(format);
                var w = new WrapperStub { _Length = 8 };
                w.Contents = s;
                var ret = w.GetDateTime(1, 8, format);
                Assert.AreEqual(DateTime.Today, ret);
            }
            [Test]
            public void GetDateTime_EmptyDate_Null()
            {
                var format = "yyyyMMdd";
                var w = new WrapperStub(8);
                var ret = w.GetDateTime(1, 8, format);
                Assert.Null(ret);
            }
        }
    }
}
