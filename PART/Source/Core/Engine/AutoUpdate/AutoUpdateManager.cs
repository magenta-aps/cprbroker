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
using System.Reflection;

namespace CprBroker.Engine
{
    public class AutoUpdateManager
    {
        public int BatchSize
        {
            get { return 1; }
        }

        public void Run()
        {
            foreach (var facadeType in DataComponentFacade.AllTypes)
            {
                Run(Utilities.Reflection.CreateInstance<DataComponentFacade>(facadeType));
            }
        }

        public void Run(DataComponentFacade facade)
        {
            var dbProv = DataProviderManager.ReadDatabaseDataProviders();
            var providers = DataProviderManager.LoadExternalDataProviders(dbProv, facade.InterfaceType);
            Run(providers);
        }

        public void Run(DataComponentFacade facade, IEnumerable<IDataProvider> providers)
        {
            foreach (var prov in providers)
            {
                var keys = facade.GetChanges(prov, BatchSize);
                while (keys.Length > 0)
                {
                    var values = facade.GetObjects(prov, keys);
                    facade.UpdateLocal(keys, values);
                    facade.DeleteChanges(prov, keys);
                    keys = facade.GetChanges(prov, BatchSize);
                }
            }
        }

        public void Run(IEnumerable<IDataProvider> providers)
        {
            foreach (var prov in providers)
            {
                var tProv = prov.GetType();
                var getChangesMethod = tProv.GetMethod("GetChanges");
                var keys = getChangesMethod.Invoke(prov, new object[] { BatchSize }) as Array;
                while (keys.Length > 0)
                {
                    var getValuesMethod = tProv.GetMethod("GetValues");
                    var values = getValuesMethod.Invoke(prov, new object[] { keys, null }) as Array;
                    // TODO: add Update logic here
                    var deleteChangesMethod = tProv.GetMethod("DeleteChanges");
                    deleteChangesMethod.Invoke(prov, new object[] { keys });
                    keys = getChangesMethod.Invoke(prov, new object[] { BatchSize }) as Array;
                }
            }
        }


    }
}
