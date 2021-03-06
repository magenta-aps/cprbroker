﻿/* ***** BEGIN LICENSE BLOCK *****
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

using CprBroker.PartInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Providers.CPRDirect
{
    public static class ExtractPaths
    {
        public static string TempDownloadFolder(IExtractDataProvider prov)
        {
            return Utilities.Strings.EnsureDirectoryEndSlash(prov.ExtractsFolder) + "tmp\\";
        }

        public static string ProcessedFolder(IExtractDataProvider prov)
        {
            return Utilities.Strings.EnsureDirectoryEndSlash(prov.ExtractsFolder) + "Processed\\";
        }

        public static string LocalName(string ftpFileName)
        {
            return ftpFileName.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
        }

        public static string TempDownloadFilePath(IExtractDataProvider prov, string ftpFileName, bool createDirIfNeeded)
        {
            var localName = LocalName(ftpFileName);
            var folder = TempDownloadFolder(prov);
            var postfix = prov.CompanionFilePostfix;
            return UniqueFileName(folder, localName, postfix, createDirIfNeeded);
        }

        public static string ProcessedFilePath(IExtractDataProvider prov, string ftpFileName, bool createDirIfNeeded)
        {
            var localName = LocalName(ftpFileName);
            var folder = ProcessedFolder(prov);
            var postFix = prov.CompanionFilePostfix;
            return UniqueFileName(folder, localName, postFix, createDirIfNeeded);
        }

        public static string ExtractFilePath(IExtractDataProvider prov, string ftpFileName)
        {
            var folder = Utilities.Strings.EnsureDirectoryEndSlash(prov.ExtractsFolder);
            var localName = LocalName(ftpFileName);
            return folder + localName;
        }

        public static string CompanionFilePath(IExtractDataProvider prov, string path)
        {
            return CompanionFilePath(path, prov.CompanionFilePostfix);
        }

        public static string CompanionFilePath(string path, string postFix)
        {
            return string.Format("{0}{1}", path, postFix);
        }

        public static string UniqueFileName(string targetFolder, string localName, string companionFilePostfix, bool createDirIfNeeded = false)
        {
            targetFolder = CprBroker.Utilities.Strings.EnsureDirectoryEndSlash(targetFolder, true);
            var targetDirInfo = new DirectoryInfo(targetFolder);

            var targetFile = targetDirInfo.FullName + localName;
            var targetFileInfo = new FileInfo(localName);

            while (File.Exists(targetFile) || File.Exists(CompanionFilePath(targetFile, companionFilePostfix)))
            {
                targetFile = string.Format("{0}{1}\\{2}",
                      targetDirInfo.FullName,
                      CprBroker.Utilities.Strings.NewRandomString(5),
                      targetFileInfo.Name
                      );
            }

            if (createDirIfNeeded && !targetDirInfo.Exists)
            {
                Directory.CreateDirectory(targetFolder);
            }

            return targetFile;
        }

    }
}
