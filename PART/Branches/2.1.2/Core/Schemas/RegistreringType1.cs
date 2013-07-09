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

namespace CprBroker.Schemas.Part
{
    public partial class RegistreringType1
    {
        [System.Xml.Serialization.XmlIgnore]
        public string SourceObjectsXml { get; set; }

        public void CalculateVirkning()
        {
            this.Virkning = null;
            var partialVirkning = GetPropertyValuesOfType<VirkningType>(this);
            var partialVirkning2 = GetPropertyValuesOfType<TilstandVirkningType>(this)
                .Select(tv => tv.ToVirkningType());
            var allVirknings = partialVirkning.Concat(partialVirkning2)
                .Where(v => !VirkningType.IsDoubleOpen(v));
            // TODO: Check this
            this.Virkning = allVirknings.ToArray();
        }

        public static T[] GetPropertyValuesOfType<T>(object root) where T : class
        {
            List<T> ret = new List<T>();
            List<object> scannedObjects = new List<object>();
            Type targetType = typeof(T);

            Action<object> method = null;

            method = (subRoot) =>
            {
                if (subRoot == null || scannedObjects.Contains(subRoot))
                    return;

                scannedObjects.Add(subRoot);
                Type subRootType = subRoot.GetType();

                var props = subRootType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (var prop in props)
                {
                    if (prop.PropertyType.IsClass)
                    {
                        if (targetType.IsAssignableFrom(prop.PropertyType))
                        {
                            var ff = prop.GetValue(subRoot, null);
                            ret.Add(ff as T);
                        }
                        else
                        {
                            object value = prop.GetValue(subRoot, null);
                            if (prop.PropertyType.IsArray)
                            {
                                Array arr = value as Array;
                                if (arr != null)
                                {
                                    for (int i = 0; i < arr.Length; i++)
                                    {
                                        method(arr.GetValue(i));
                                    }
                                }
                            }
                            else
                            {
                                method(value);
                            }
                        }
                    }
                }
            };

            method(root);
            return ret.ToArray();
        }
    }
}
