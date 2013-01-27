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
        public class Base
        {
            public static int ProviderStubCreatedCount = 0;

            [SetUp]
            public void ClearCreatedCount()
            {
                ProviderStubCreatedCount = 0;
            }

            public static DataProvidersConfigurationSection CreateConfigSection()
            {
                var section = new DataProvidersConfigurationSection();
                section.Types.Add(new TypeElement() { TypeName = typeof(LocalDataProviderStub).AssemblyQualifiedName });
                section.Types.Add(new TypeElement() { TypeName = typeof(CustomExternalDataProviderStub).AssemblyQualifiedName });
                return section;
            }

            public DataProvider[] CreateDatabaseDataProviders()
            {
                return new Type[] { typeof(LocalDataProviderStub), typeof(CustomExternalDataProviderStub) }
                    .Select(t => new DataProvider() { IsEnabled = true, Ordinal = 1, TypeName = t.AssemblyQualifiedName })
                    .ToArray();
            }

            public DataProvider[] CreateDatabaseObjects(int randomCount, params Type[] types)
            {
                return CreateDatabaseObjects(randomCount, 1, types);
            }

            public DataProvider[] CreateDatabaseObjects(int randomCount, int repeatTypes, params Type[] types)
            {
                types = types.Join(new int[repeatTypes], t => 1, i => 1, (t, i) => t).ToArray();
                return types
                    .Select(t => t.AssemblyQualifiedName)
                    .Concat(Utilities.RandomGuidStrings(randomCount))
                    .Select(n => new DataProvider() { TypeName = n })
                    .ToArray()
                    ;
            }

        }

        interface IDataProviderStub : IDataProvider
        { }

        class LocalDataProviderStub : IDataProviderStub
        {
            public LocalDataProviderStub()
            {
                Base.ProviderStubCreatedCount++;
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
                Base.ProviderStubCreatedCount++;
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

        [TestFixture]
        public class GetAvailableDataProviderTypes : Base
        {
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
        }

        [TestFixture]
        public class GetDataProviderList : Base
        {

            [Test]
            public void GetDataProviderList_LocalOnly_ReturnsLocal()
            {
                var result = DataProviderManager.GetDataProviderList(CreateConfigSection(), CreateDatabaseDataProviders(), typeof(IDataProviderStub), Schemas.SourceUsageOrder.LocalOnly);
                Assert.AreEqual(1, result.Count());
                Assert.IsNotInstanceOf<IExternalDataProvider>(result.First());
            }

            [Test]
            public void GetDataProviderList_ExternalOnly_ReturnsExternal()
            {
                var result = DataProviderManager.GetDataProviderList(CreateConfigSection(), CreateDatabaseDataProviders(), typeof(IDataProviderStub), Schemas.SourceUsageOrder.ExternalOnly);
                Assert.AreEqual(1, result.Count());
                Assert.IsInstanceOf<IExternalDataProvider>(result.First());
            }

            [Test]
            public void GetDataProviderList_LocalThenExternal_ReturnsBoth()
            {
                var result = DataProviderManager.GetDataProviderList(CreateConfigSection(), CreateDatabaseDataProviders(), typeof(IDataProviderStub), Schemas.SourceUsageOrder.LocalThenExternal);
                Assert.AreEqual(2, result.Count());

                Assert.AreEqual(1, result.Where(p => p is IExternalDataProvider).Count());
                Assert.AreEqual(1, result.Where(p => !(p is IExternalDataProvider)).Count());
            }
        }

        
        [TestFixture]
        public class LoadLocalDataProviders : Base
        {
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
        }

        [TestFixture]
        public class LoadExternalDataProviders : Base
        {
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
}
