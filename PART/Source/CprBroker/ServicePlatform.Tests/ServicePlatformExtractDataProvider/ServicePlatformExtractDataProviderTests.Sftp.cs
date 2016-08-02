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
using CprBroker.Providers.ServicePlatform;
using CprBroker.Providers.CprServices;
using CprBroker.Providers.ServicePlatform.Responses;
using CprBroker.Providers.CprServices.Responses;
using NUnit.Framework;
using System.IO;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.ServicePlatform
{
    namespace ServicePlatformExtractDataProviderTests
    {
        [TestFixture]
        public class Sftp
        {
            protected static readonly string fileName1 = "SftpUploadFileTest.txt";
            protected static readonly string fileName2 = "SftpUploadFileTest2.txt";
            protected static readonly string pathToWorkDir = @".\Resources\SFTP-testfiles\";

            [SetUp]
            public void SetUp()
            {
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "NUnit");
                var prov = ServicePlatformDataProviderFactory.CreateExtractDataProvider();
                prov.UploadFile(pathToWorkDir, fileName1);
                prov.UploadFile(pathToWorkDir, fileName1 + CprBroker.Providers.ServicePlatform.Constants.MetaDataFilePostfix);
                prov.UploadFile(pathToWorkDir, fileName2);
                prov.UploadFile(pathToWorkDir, fileName2 + CprBroker.Providers.ServicePlatform.Constants.MetaDataFilePostfix);
            }

            [Test]
            public void ListFtpContents_SampleFilesAdded_ContainsSampleFiles()
            {
                var prov = ServicePlatformDataProviderFactory.CreateExtractDataProvider();
                
                // The sample files should have been added in the InitContext() startup method
                String[] ftpContent = prov.ListFtpContents();

                Assert.AreEqual(2, ftpContent.Length);
                Assert.True((ftpContent[0].Equals(fileName1) || ftpContent[1].Equals(fileName1))
                    && (ftpContent[0].Equals(fileName2) || ftpContent[1].Equals(fileName2)));
            }

            [Test]
            public void DownloadFile_Normal_ExistsInExtractsFolder()
            {
                var prov = ServicePlatformDataProviderFactory.CreateExtractDataProvider();
                String fileName2LocalPath = Utilities.Strings.EnsureEndString(prov.ExtractsFolder, "\\", true) + fileName2;
                Assert.False(File.Exists(fileName2LocalPath));

                prov.DownloadFile(fileName2, fileName2LocalPath, 0);

                Assert.True(File.Exists(fileName2LocalPath));
                Assert.True(File.Exists(fileName2LocalPath + CprBroker.Providers.ServicePlatform.Constants.MetaDataFilePostfix));

                File.Delete(fileName2LocalPath);
                File.Delete(fileName2LocalPath + CprBroker.Providers.ServicePlatform.Constants.MetaDataFilePostfix);
            }

            [Test]
            [ExpectedException(ExpectedMessage = "already exists", MatchType = MessageMatch.Contains)]
            public void DownloadFile_CompanionExistsLocally_Exception()
            {
                var prov = ServicePlatformDataProviderFactory.CreateExtractDataProvider();
                String fileName2LocalPath = Utilities.Strings.EnsureEndString(prov.ExtractsFolder, "\\", true) + fileName2;
                var companionFile2Path = ExtractPaths.CompanionFilePath(prov, fileName2LocalPath);
                Assert.False(File.Exists(fileName2LocalPath));
                File.WriteAllText(companionFile2Path, "ABC");

                prov.DownloadFile(fileName2, fileName2LocalPath, 0);
            }

            [TearDown]
            public void TearDown()
            {
                var prov = ServicePlatformDataProviderFactory.CreateExtractDataProvider();
                prov.DeleteFile(fileName1);
                prov.DeleteFile(fileName2);
            }
        }
    }
}
