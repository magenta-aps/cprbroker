/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 *
 * The Initial Developer of the Original Code is
 * IT- og Telestyrelsen / Danish National IT and Telecom Agency.
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
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
using System.Reflection;
using System.Text;
using System.Threading;
using System.Data.Linq;
using System.IO;
using CprBroker.Data.DataProviders;
using CprBroker.Utilities;

namespace CprBroker.Engine
{
    /// <summary>
    /// Manages the loading and selection of data providers
    /// </summary>
    public static class DataProviderManager
    {
        #region Creators
        /// <summary>
        /// Converts the current DataProvider (database object) to the appropriate IDataProvider object based on its type
        /// </summary>
        /// <param name="dbDataProvider">The database object that represents the data provider</param>
        /// <returns>The newly created IDataProvider</returns>
        public static IExternalDataProvider CreateDataProvider(CprBroker.Data.DataProviders.DataProvider dbDataProvider)
        {
            IExternalDataProvider dataProvider = Utilities.Reflection.CreateInstance<IExternalDataProvider>(dbDataProvider.TypeName);
            if (dataProvider is IExternalDataProvider)
            {
                try
                {
                    IExternalDataProvider externalProvider = dataProvider as IExternalDataProvider;
                    externalProvider.ConfigurationProperties = dbDataProvider.ToPropertiesDictionary(externalProvider.ConfigurationKeys.Select(p => p.Name).ToArray());
                }
                catch (Exception ex)
                {
                    Local.Admin.LogException(ex);
                }
            }
            return dataProvider;
        }

        #endregion

        #region Loaders

        public static Type[] GetAvailableDataProviderTypes(bool isExternal)
        {
            return GetAvailableDataProviderTypes(null, isExternal);
        }

        public static Type[] GetAvailableDataProviderTypes(Type interfaceType, bool isExternal)
        {
            if (interfaceType == null)
            {
                interfaceType = typeof(IDataProvider);
            }

            List<Type> neededTypes = new List<Type>();
            try
            {
                DataProvidersConfigurationSection section = DataProvidersConfigurationSection.GetCurrent();
                if (section != null)
                {
                    for (int i = 0; i < section.Types.Count; i++)
                    {
                        try
                        {
                            string typeName = section.Types[i].TypeName;
                            Type t = Type.GetType(typeName);
                            if (interfaceType.IsAssignableFrom(t))
                            {
                                if (t.IsClass && !t.IsAbstract && typeof(IDataProvider).IsAssignableFrom(t))
                                {
                                    if (isExternal && typeof(IExternalDataProvider).IsAssignableFrom(t))
                                    {
                                        neededTypes.Add(t);
                                    }
                                    else if (!isExternal && !typeof(IExternalDataProvider).IsAssignableFrom(t))
                                    {
                                        neededTypes.Add(t);
                                    }
                                }
                            }
                        }
                        catch (System.Reflection.ReflectionTypeLoadException ex)
                        {
                            Local.Admin.LogException(ex, ex.LoaderExceptions);
                        }
                        catch (Exception ex)
                        {
                            Local.Admin.LogException(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Local.Admin.LogException(ex);
            }
            return neededTypes.ToArray();
        }

        private static IExternalDataProvider[] LoadExternalDataProviders(Type interfaceType)
        {
            List<IExternalDataProvider> providers = new List<IExternalDataProvider>();
            try
            {
                using (var dataContext = new CprBroker.Data.DataProviders.DataProvidersDataContext())
                {
                    var dbProviders = (from prov in dataContext.DataProviders
                                       where prov.IsEnabled
                                       orderby prov.Ordinal
                                       select prov).ToArray();

                    // Append external clearData providers
                    foreach (var dbProv in dbProviders)
                    {
                        try
                        {
                            Type providerType = Type.GetType(dbProv.TypeName);
                            if (providerType != null && interfaceType.IsAssignableFrom(providerType))
                            {
                                IExternalDataProvider dataProvider = CreateDataProvider(dbProv) as IExternalDataProvider;
                                if (dataProvider != null)
                                {
                                    providers.Add(dataProvider);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Local.Admin.LogException(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Local.Admin.LogException(ex);
            }
            return providers.ToArray();
        }

        private static IDataProvider[] LoadLocalDataProviders(Type interfaceType)
        {
            return Array.ConvertAll<Type, IDataProvider>(GetAvailableDataProviderTypes(interfaceType, false), t => Reflection.CreateInstance<IDataProvider>(t.AssemblyQualifiedName));
        }

        #endregion

        #region Filtration by type


        internal static List<IDataProvider> GetDataProviderList(Type interfaceType, LocalDataProviderUsageOption localOption)
        {

            // First copy to local defined list to avoid threading issues
            List<IDataProvider> availableProviders = new List<IDataProvider>();

            // External
            availableProviders.AddRange(LoadExternalDataProviders(interfaceType));

            // Now add the local clearData providers if needed
            if (localOption != LocalDataProviderUsageOption.Forbidden)
            {
                var availableLocalProviders = LoadLocalDataProviders(interfaceType);

                if (localOption == LocalDataProviderUsageOption.UseFirst)
                {
                    availableProviders.InsertRange(0, availableLocalProviders);
                }
                else
                {
                    availableProviders.AddRange(availableLocalProviders);
                }
            }
            return availableProviders;
        }

        #endregion
    }
}
