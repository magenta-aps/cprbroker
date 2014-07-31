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
using System.IO;
using System.Data.SqlClient;
using CprBroker.Utilities;

namespace CprBroker.Providers.CPRDirect
{
    /// <summary>
    /// Database object for authority
    /// </summary>
    public partial class Authority
    {

        public static void ImportText(string text)
        {
            using (var conn = new SqlConnection(CprBroker.Config.ConfigManager.Current.Settings.CprBrokerConnectionString))
            {
                conn.Open();
                ImportText(text, conn);
            }
        }

        public static void ImportText(string text, SqlConnection conn)
        {
            var authorities = LineWrapper.ParseBatch(text)
                    .Select(line => line.ToWrapper(Constants.DataObjectMap_P02680))
                    .Where(w => w != null)
                    .Select(w => (w as AuthorityType).ToAuthority());

            using (var trans = conn.BeginTransaction())
            {
                conn.DeleteAll<Authority>(trans);
                conn.BulkInsertAll<Authority>(authorities, trans);
                trans.Commit();
            }
        }

        private static Dictionary<string, string> _AuthorityMap;

        static void FillAuthorityMap()
        {
            try
            {
                Constants.AuthorityLock.EnterUpgradeableReadLock();
                if (_AuthorityMap == null)
                {
                    try
                    {
                        Constants.AuthorityLock.EnterWriteLock();
                        if (_AuthorityMap == null)
                        {
                            using (var dataContext = new LookupDataContext())
                            {
                                _AuthorityMap = dataContext
                                    .Authorities
                                    .ToDictionary(au => au.AuthorityCode, au => au.FullName);
                            }
                        }
                    }
                    finally
                    {
                        Constants.AuthorityLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                Constants.AuthorityLock.ExitUpgradeableReadLock();
            }
        }

        public static string GetNameByCode(string code)
        {
            FillAuthorityMap();
            if (!string.IsNullOrEmpty(code) && _AuthorityMap.ContainsKey(code))
            {
                return _AuthorityMap[code];
            }
            else
            {
                return null;
            }
        }

    }
}
