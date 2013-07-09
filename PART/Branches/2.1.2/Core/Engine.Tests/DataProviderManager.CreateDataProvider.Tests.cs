using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Data.DataProviders;
using CprBroker.Engine;

namespace CprBroker.Tests.Engine
{
    namespace DataProviderManagerTests
    {
        [TestFixture]
        public class CreateDataProvider : Base
        {
            [Test]
            [ExpectedException]
            public void CreateDataProvider_Null_ThrowsException()
            {
                DataProviderManager.CreateDataProvider(null);
            }

            [Test]
            public void CreateDataProvider_FakeType_ReturnsNull()
            {
                var result = DataProviderManager.CreateDataProvider(new DataProvider() { TypeName = "kaaklsdflksah" });
                Assert.Null(result);
            }

            [Test]
            public void CreateDataProvider_RealInvalidType_ReturnsNull(
                [Values(typeof(object), typeof(LocalDataProviderStub))]Type type)
            {
                var result = DataProviderManager.CreateDataProvider(new DataProvider() { TypeName = type.AssemblyQualifiedName });
                Assert.Null(result);
            }

            [Test]
            public void CreateDataProvider_RealCorrectType_ReturnsNotNull(
                [Values(typeof(CustomExternalDataProviderStub))]Type type)
            {
                var result = DataProviderManager.CreateDataProvider(new DataProvider() { TypeName = type.AssemblyQualifiedName });
                Assert.NotNull(result);
            }

            [Test]
            public void CreateDataProvider_NewProperty_NoException()
            {
                var dbProvider = new Data.DataProviders.DataProvider()
                {
                    TypeName = typeof(DataProviderWithNewConfigProperty).AssemblyQualifiedName
                };
                var createdProvider = DataProviderManager.CreateDataProvider(dbProvider) as DataProviderWithNewConfigProperty;
                createdProvider.PropertyType = DataProviderConfigPropertyInfoTypes.Boolean;
                var result = createdProvider.BooleanPropertyValue;

            }
        }

        public class DataProviderWithNewConfigProperty : IExternalDataProvider
        {
            public Dictionary<string, string> ConfigurationProperties { get; set; }

            public DataProviderConfigPropertyInfo[] ConfigurationKeys
            {
                get
                {
                    return new DataProviderConfigPropertyInfo[] { new DataProviderConfigPropertyInfo() { Confidential = false, Name = PropertyName, Required = false, Type = PropertyType } };
                }
            }

            public bool IsAlive()
            {
                return true;
            }

            public Version Version
            {
                get { return new Version(); }
            }


            public DataProviderConfigPropertyInfoTypes PropertyType;
            public string PropertyName = Guid.NewGuid().ToString();
            private Dictionary<string, string> _ConfigurationProperties = null;
            public bool BooleanPropertyValue
            {
                get
                {
                    return Convert.ToBoolean(ConfigurationProperties[PropertyName]);
                }
            }

        }

    }
}
