using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Data.DataProviders;
using CprBroker.Engine;

namespace CprBroker.Tests.Engine
{
    [TestFixture]
    class DataProviderManagerTests
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
        public void GetAvailableDataProviderTypes_NullSection_ReturnsNotNull(
            [Values(true, false)] bool isExternal)
        {
            var result = DataProviderManager.GetAvailableDataProviderTypes(null, typeof(IDataProvider), isExternal);
            Assert.NotNull(result);
        }

        [Test]
        public void GetAvailableDataProviderTypes_NullSection_ReturnsEmpty(
            [Values(true, false)] bool isExternal)
        {
            var result = DataProviderManager.GetAvailableDataProviderTypes(null, typeof(IDataProvider), isExternal);
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void GetAvailableDataProviderTypes_EmptySection_ReturnsNotNull(
            [Values(true, false)] bool isExternal)
        {
            var result = DataProviderManager.GetAvailableDataProviderTypes(new DataProvidersConfigurationSection(), typeof(IDataProvider), isExternal);
            Assert.NotNull(result);
        }

        [Test]
        public void GetAvailableDataProviderTypes_EmptySection_ReturnsEmpty(
            [Values(true, false)] bool isExternal)
        {
            var result = DataProviderManager.GetAvailableDataProviderTypes(new DataProvidersConfigurationSection(), typeof(IDataProvider), isExternal);
            Assert.AreEqual(0, result.Count());
        }


        public static int ProviderStubCreatedCount = 0;

        interface IDataProviderStub : IDataProvider
        { }

        class LocalDataProviderStub : IDataProviderStub
        {
            public LocalDataProviderStub()
            {
                ProviderStubCreatedCount++;
            }

            public bool IsAlive()
            {
                return true;
            }

            public Version Version
            {
                get { return new Version(); }
            }
        }

        class CustomExternalDataProviderStub : IDataProviderStub, IExternalDataProvider
        {

            public CustomExternalDataProviderStub()
            {
                ProviderStubCreatedCount++;
            }

            public bool IsAlive()
            {
                return true;
            }

            public Version Version
            {
                get { return new Version(); }
            }

            public Dictionary<string, string> ConfigurationProperties
            {
                get { return new Dictionary<string, string>(); }
                set { }
            }

            public DataProviderConfigPropertyInfo[] ConfigurationKeys
            {
                get { return new DataProviderConfigPropertyInfo[0]; }
            }
        }

        private DataProvidersConfigurationSection CreateConfigSection()
        {
            var section = new DataProvidersConfigurationSection();
            section.Types.Add(new TypeElement() { TypeName = typeof(LocalDataProviderStub).AssemblyQualifiedName });
            section.Types.Add(new TypeElement() { TypeName = typeof(CustomExternalDataProviderStub).AssemblyQualifiedName });
            return section;
        }

        [Test]
        public void GetAvailableDataProviderType_List_ReturnsOneSection(
            [Values(true, false)] bool isExternal)
        {
            var result = DataProviderManager.GetAvailableDataProviderTypes(CreateConfigSection(), typeof(IDataProviderStub), isExternal);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void GetAvailableDataProviderType_List_ReturnsCorrectExternal(
            [Values(true, false)] bool isExternal)
        {
            var result = DataProviderManager.GetAvailableDataProviderTypes(CreateConfigSection(), typeof(IDataProviderStub), isExternal);
            Assert.AreEqual(isExternal, typeof(IExternalDataProvider).IsAssignableFrom(result.First()));
        }

        [SetUp]
        public void ClearCreatedCount()
        {
            ProviderStubCreatedCount = 0;
        }

        [Test]
        public void LoadLocalDataProviders_UnusedObjects_NoCreatedObjects()
        {
            var result = DataProviderManager.LoadLocalDataProviders(CreateConfigSection(), typeof(IDataProviderStub));
            Assert.AreEqual(0, ProviderStubCreatedCount);
        }

        [Test]
        public void LoadLocalDataProviders_UsedObjects_CorrectNoOfLocal()
        {
            var result = DataProviderManager.LoadLocalDataProviders(CreateConfigSection(), typeof(IDataProviderStub));
            var arr = result.ToArray();
            Assert.AreEqual(1, arr.Length);
        }

        [Test]
        public void LoadLocalDataProviders_UsedObjects_CorrectNoOfCreatedObjects()
        {
            var result = DataProviderManager.LoadLocalDataProviders(CreateConfigSection(), typeof(IDataProviderStub));
            var arr = result.ToArray();
            Assert.AreEqual(1, ProviderStubCreatedCount);
        }

        [Test]
        [ExpectedException(ExpectedMessage = "null", MatchType = MessageMatch.Contains)]
        public void LoadExternalDataProviders_Null_ThrowsException()
        {
            var result = DataProviderManager.LoadExternalDataProviders(null, typeof(IDataProviderStub));
        }

        [Test]
        public void LoadExternalDataProviders_EmptyArr_ReturnsNotNull()
        {
            var result = DataProviderManager.LoadExternalDataProviders(new DataProvider[0], typeof(IDataProviderStub));
            Assert.NotNull(result);
        }

        DataProvider[] CreateDatabaseObjects(int randomCount, params Type[] types)
        {
            return CreateDatabaseObjects(randomCount, 1, types);
        }
        DataProvider[] CreateDatabaseObjects(int randomCount, int repeatTypes, params Type[] types)
        {
            types = types.Join(new int[repeatTypes], t => 1, i => 1, (t, i) => t).ToArray();
            return types
                .Select(t => t.AssemblyQualifiedName)
                .Concat(Utilities.RandomGuidStrings(randomCount))
                .Select(n => new DataProvider() { TypeName = n })
                .ToArray()
                ;
        }

        [Test]
        public void LoadExternalDataProviders_FakeTypes_ReturnsNotNull(
            [Range(0, 5)]int count)
        {
            var result = DataProviderManager.LoadExternalDataProviders(CreateDatabaseObjects(count), typeof(IDataProviderStub));
            Assert.NotNull(result);
        }

        [Test]
        public void LoadExternalDataProviders_FakeTypes_ReturnsEmpty(
            [Range(0, 5)]int count)
        {
            var result = DataProviderManager.LoadExternalDataProviders(CreateDatabaseObjects(count), typeof(IDataProviderStub));
            Assert.IsEmpty(result.ToArray());
        }

        [Test]
        public void LoadExternalDataProviders_MixedFakeAndReal_ReturnsOneReal(
            [Range(0, 5)]int count)
        {
            var result = DataProviderManager.LoadExternalDataProviders(CreateDatabaseObjects(count, typeof(CustomExternalDataProviderStub), typeof(LocalDataProviderStub)), typeof(IDataProviderStub));
            Assert.AreEqual(1, result.ToArray().Count());
        }

        [Test]
        public void LoadExternalDataProviders_MixedFakeAndReal_ReturnsCorrectType(
            [Range(0, 5)]int count)
        {
            var result = DataProviderManager.LoadExternalDataProviders(CreateDatabaseObjects(count, typeof(CustomExternalDataProviderStub), typeof(LocalDataProviderStub)), typeof(IDataProviderStub));
            Assert.IsInstanceOf<CustomExternalDataProviderStub>(result.First());
        }

        [Test]
        public void LoadExternalDataProviders_MixedFakeAndRealNotUsed_DoesNotCreate(
            [Range(0, 5)]int count)
        {
            var result = DataProviderManager.LoadExternalDataProviders(CreateDatabaseObjects(count, typeof(CustomExternalDataProviderStub), typeof(LocalDataProviderStub)), typeof(IDataProviderStub));
            Assert.AreEqual(0, ProviderStubCreatedCount);
        }

        [Test]
        public void LoadExternalDataProviders_MixedFakeAndRealUsed_CreatesObject(
            [Range(0, 3)]int fakeCount,
            [Range(1, 4)]int realCount)
        {
            var result = DataProviderManager.LoadExternalDataProviders(CreateDatabaseObjects(fakeCount, realCount, typeof(CustomExternalDataProviderStub), typeof(LocalDataProviderStub)), typeof(IDataProviderStub));
            var p = result.ToArray();
            Assert.AreEqual(realCount, ProviderStubCreatedCount);
        }



    }
}
