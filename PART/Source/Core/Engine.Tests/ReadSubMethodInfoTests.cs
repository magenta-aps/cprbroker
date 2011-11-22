using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Engine;
using CprBroker.Engine.Part;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.Engine
{
    [TestFixture]
    class ReadSubMethodInfoTests
    {
        class ReadDataProvider : IPartReadDataProvider
        {
            RegistreringType1 _Read = new RegistreringType1();

            public RegistreringType1 Read(CprBroker.Schemas.PersonIdentifier uuid, LaesInputType input, Func<string, Guid> cpr2uuidFunc, out CprBroker.Schemas.QualityLevel? ql)
            {
                ql = QualityLevel.DataProvider;
                return _Read;
            }
            #region IDataProvider Members

            bool IDataProvider.IsAlive()
            {
                return true;
            }

            Version IDataProvider.Version
            {
                get { return new Version(); }
            }

            #endregion
        }

        class ReadSubMethodInfoStub : ReadSubMethodInfo
        {
            public ReadSubMethodInfoStub()
                : base(new LaesInputType() { UUID = Guid.NewGuid().ToString() }, LocalDataProviderUsageOption.UseFirst)
            { }

            public RegistreringType1 RunMainMethod_ = new RegistreringType1();
            public bool RunMainMethod_UaseBase = false;
            public override RegistreringType1 RunMainMethod(IPartReadDataProvider prov)
            {
                if (RunMainMethod_UaseBase)
                    return base.RunMainMethod(prov);
                return RunMainMethod_;
            }

            public PersonIdentifier UuidToPersonIdentifier_ = new PersonIdentifier();
            protected override PersonIdentifier UuidToPersonIdentifier(string uuidString)
            {
                return UuidToPersonIdentifier_;
            }
        }

        [Test]
        public void RunMainMethod_NullMapping_ReturnsNull()
        {
            var subMethodInfo = new ReadSubMethodInfoStub() { UuidToPersonIdentifier_ = null, RunMainMethod_UaseBase = true };
            var result = subMethodInfo.RunMainMethod(new ReadDataProvider());
            Assert.Null(result);
        }

        [Test]
        public void RunMainMethod_WlMapping_ReturnsValue()
        {
            var subMethodInfo = new ReadSubMethodInfoStub();
            var result = subMethodInfo.RunMainMethod(new ReadDataProvider());
            Assert.NotNull(result);
        }
    }
}
