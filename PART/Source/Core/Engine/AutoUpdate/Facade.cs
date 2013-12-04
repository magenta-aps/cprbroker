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
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Engine
{
    public abstract class Facade
    {
        public abstract Type AutoUpdateType { get; }
        public abstract Array GetChanges(IDataProvider prov, int c);
        public abstract Array GetObjects(IDataProvider prov, Array keys);
        public abstract void UpdateLocal(Array keys, Array values);
        public abstract void DeleteChanges(IDataProvider prov, Array keys);

        public static Type[] AllTypes
        {
            get { return new Type[] { typeof(CprFacade) }; }
        }
    }

    public abstract class Facade<TKey, TObject, TContext> : Facade
    {
        public override Type AutoUpdateType
        {
            get { return typeof(IAutoUpdateDataProvider<TKey, TObject, TContext>); }
        }
        public override Array GetChanges(IDataProvider prov, int c)
        {
            return GetChanges(prov as IAutoUpdateDataProvider<TKey, TObject, TContext>, c);
        }

        public TKey[] GetChanges(IAutoUpdateDataProvider<TKey, TObject, TContext> dataProvider, int c)
        {
            return dataProvider.GetChanges(c);
        }

        public override Array GetObjects(IDataProvider prov, Array keys)
        {
            return GetObjects(prov as IAutoUpdateDataProvider<TKey, TObject, TContext>, keys as TKey[]);
        }

        public Array GetObjects(IAutoUpdateDataProvider<TKey, TObject, TContext> prov, TKey[] keys)
        {
            TContext context = default(TContext);
            return prov.GetValues(keys, context);
        }

        public override void DeleteChanges(IDataProvider prov, Array keys)
        {
            DeleteChanges(prov as IAutoUpdateDataProvider<TKey, TObject, TContext>, keys);
        }

        public void DeleteChanges(IAutoUpdateDataProvider<TKey, TObject, TContext> prov, Array keys)
        {
            prov.DeleteChanges(keys as TKey[]);
        }

        public override void UpdateLocal(Array keys, Array values)
        {
            var keys2 = keys.OfType<TKey>().ToArray();
            var values2 = values.OfType<TObject>().ToArray();
            UpdateLocal(keys2, values2);
        }

        public virtual void UpdateLocal(TKey[] keys, TObject[] values)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                UpdateLocal(keys[i], values[i]);
            }
        }

        public abstract void UpdateLocal(TKey key, TObject value);
    }

    public class CprFacade : Facade<PersonIdentifier, RegistreringType1, object>
    {
        public override void UpdateLocal(PersonIdentifier key, RegistreringType1 value)
        {
            Local.UpdateDatabase.UpdatePersonRegistration(key, value);
        }
    }
}
