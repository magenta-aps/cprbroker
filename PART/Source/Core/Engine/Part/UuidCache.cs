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
using CprBroker.Engine;
using CprBroker.Engine.Part;
using CprBroker.Utilities;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine.Part
{
    /// <summary>
    /// Pre-caches a group of CPR/UUID mappings
    /// Helps to reduce the number of individual calls to database and/or person master by grouping such calls in batches and offering a quick in-memory cache
    /// </summary>
    public class UuidCache
    {
        Dictionary<string, Guid> _Cache = new Dictionary<string, Guid>();

        /// <summary>
        /// Assigns UUIDs to <paramref name="pnrs"/> from local database and/or person master
        /// </summary>
        /// <param name="pnrs">Array of CPR numbers</param>
        public void FillCache(string[] pnrs)
        {
            if (pnrs.Length > 0)
            {
                var result = Manager.Part.GetUuidArray(BrokerContext.Current.UserToken, BrokerContext.Current.ApplicationToken, pnrs);
                if (StandardReturType.IsSucceeded(result.StandardRetur))
                {

                    for (int i = 0; i < pnrs.Length; i++)
                    {
                        if (Strings.IsGuid(result.UUID[i]))
                        {
                            _Cache[pnrs[i]] = new Guid(result.UUID[i]);
                        }
                    }
                }
                else
                {
                    throw new Exception("Could not get UUID array");
                }
            }
        }

        public void PreLoadExistingMappings()
        {
            using (var dataContext = new CprBroker.Data.Part.PartDataContext())
            {
                var all = dataContext.PersonMappings.Select(pm => new KeyValuePair<string, Guid>(pm.CprNumber, pm.UUID)).ToArray();                
                int count = all.Length;                
                Console.WriteLine("Found <{0}> mappings", count);
                for (int i = 0; i < count; i++)
                {
                    _Cache[all[i].Key] = all[i].Value;
                    if (i % 10000 == 9999)
                    {
                        Console.WriteLine("UUID Cache : <{0}> rows loaded", _Cache.Count);
                    }

                }
            }
        }

        public Guid GetUuid(string pnr)
        {
            if (this._Cache.ContainsKey(pnr))
            {
                return this._Cache[pnr];
            }
            else
            {
                return ReadSubMethodInfo.CprToUuid(pnr);
            }
        }

    }
}
