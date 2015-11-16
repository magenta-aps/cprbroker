using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.ServicePlatform;
using CprBroker.Providers.CprServices;
using CprBroker.Providers.ServicePlatform.Responses;
using CprBroker.Providers.CprServices.Responses;
using NUnit.Framework;
using System.IO;

namespace CprBroker.Tests.ServicePlatform
{
    namespace ServicePlatformDataProviderTests
    {
        [TestFixture]
        public class SearchList
        {
            [SetUp]
            public void InitContext()
            {
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "NUnit");
            }


            [Test]
            public void SearchList_NotNull()
            {
                var prov = ServicePlatformDataProviderFactory.Create();
                var input = SearchCriteriaFactory.Create();
                var cache = UuidCacheFactory.Create();

                var ret = prov.SearchList(input, cache);
                Assert.NotNull(ret);
            }

            [Test]
            public void SearchList_DataFound()
            {
                var prov = ServicePlatformDataProviderFactory.Create();
                var input = SearchCriteriaFactory.Create();
                var cache = UuidCacheFactory.Create();
                var ret = prov.SearchList(input, cache);
                Assert.Greater(ret.Length, 0);
            }

            [Test]
            public void SearchList_WrongName_NoDataFound()
            {
                var prov = ServicePlatformDataProviderFactory.Create();
                var input = SearchCriteriaFactory.Create();
                input.SoegObjekt.SoegAttributListe.SoegEgenskab[0].NavnStruktur.PersonNameStructure.PersonGivenName = Guid.NewGuid().ToString();
                var cache = UuidCacheFactory.Create();
                var ret = prov.SearchList(input, cache);
                Assert.AreEqual(0, ret.Length);
            }

            [Test]
            public void SearchList_LifeStatus()
            {
                var prov = ServicePlatformDataProviderFactory.Create();
                var input = SearchCriteriaFactory.Create();
                var cache = UuidCacheFactory.Create();
                var ret = prov.SearchList(input, cache).First();
                var oo = ret.Item as CprBroker.Schemas.Part.FiltreretOejebliksbilledeType;
                Assert.NotNull(oo.TilstandListe);
                Assert.NotNull(oo.TilstandListe.LivStatus.TilstandVirkning.FraTidspunkt.ToDateTime());
            }
        }

        [TestFixture]
        public class Sftp
        {
            protected static readonly string fileName1 = "SftpUploadFileTest.txt";
            protected static readonly string fileName2 = "SftpUploadFileTest2.txt";
            protected static readonly string pathToWorkDir = @".\Resources\SFTP-testfiles\";




            [SetUp]
            public void InitContext()
            {
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "NUnit");
                var prov = ServicePlatformDataProviderFactory.Create();
                prov.UploadFile(pathToWorkDir, fileName1);
                prov.UploadFile(pathToWorkDir, fileName1 + CprBroker.Providers.ServicePlatform.Constants.MetaDataFilePostfix);
                prov.UploadFile(pathToWorkDir, fileName2);
                prov.UploadFile(pathToWorkDir, fileName2 + CprBroker.Providers.ServicePlatform.Constants.MetaDataFilePostfix);

            }

            [Test]
            public void Sftp_TestListSFtpContents()
            {

                var prov = ServicePlatformDataProviderFactory.Create();
                String[] ftpContent = prov.ListFtpContents();
                Assert.AreEqual(2, ftpContent.Length);
                Assert.True((ftpContent[0].Equals(fileName1) || ftpContent[1].Equals(fileName1))
                    && (ftpContent[0].Equals(fileName2) || ftpContent[1].Equals(fileName2)));
            }

            [Test]
            public void Sftp_TestDownloadSftpFileToExtractsFolder()
            {
                var prov = ServicePlatformDataProviderFactory.Create();
                String fileName2LocalPath = Utilities.Strings.EnsureEndString(prov.ExtractsFolder, "\\", true) + fileName2;
                Assert.False(File.Exists(fileName2LocalPath));
                prov.DownloadFile(fileName2, 0);
                Assert.True(File.Exists(fileName2LocalPath));
                Assert.True(File.Exists(fileName2LocalPath + CprBroker.Providers.ServicePlatform.Constants.MetaDataFilePostfix));
                File.Delete(fileName2LocalPath);
                Assert.False(File.Exists(fileName2LocalPath));
                File.Delete(fileName2LocalPath + CprBroker.Providers.ServicePlatform.Constants.MetaDataFilePostfix);
                Assert.False(File.Exists(fileName2LocalPath + CprBroker.Providers.ServicePlatform.Constants.MetaDataFilePostfix));
            }

            [TearDown]
            public void tearDown()
            {
                var prov = ServicePlatformDataProviderFactory.Create();
                prov.DeleteFile(fileName1);
                prov.DeleteFile(fileName2);
            }
        }


    }
}
