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
using System.Reflection;
using System.Text;
using System.Threading;
using System.Data.Linq;
using System.IO;
using CprBroker.Data.DataProviders;
using CprBroker.Utilities;
using CprBroker.Schemas;

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
                    dataProvider.ConfigurationProperties = dbDataProvider.ToPropertiesDictionary(dataProvider.ConfigurationKeys.Select(p => p.Name).ToArray());
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

        public static IEnumerable<Type> GetAvailableDataProviderTypes(bool isExternal)
        {
            return GetAvailableDataProviderTypes(null, isExternal);
        }

        public static IEnumerable<Type> GetAvailableDataProviderTypes(Type interfaceType, bool isExternal)
        {
            return GetAvailableDataProviderTypes(DataProvidersConfigurationSection.GetCurrent(), interfaceType, isExternal);

        }

        public static IEnumerable<Type> GetAvailableDataProviderTypes(DataProvidersConfigurationSection section, Type interfaceType, bool isExternal)
        {
            if (interfaceType == null)
            {
                interfaceType = typeof(IDataProvider);
            }

            if (section != null)
            {
                try
                {
                    var typesConfigurationElements = new TypeElement[section.Types.Count];
                    section.Types.CopyTo(typesConfigurationElements, 0);

                    return typesConfigurationElements
                        .Select(typeConfigElement =>
                        {
                            try
                            {
                                string typeName = typeConfigElement.TypeName;
                                Type t = Type.GetType(typeName);
                                if (interfaceType.IsAssignableFrom(t))
                                {
                                    if (t.IsClass && !t.IsAbstract && typeof(IDataProvider).IsAssignableFrom(t))
                                    {
                                        if (isExternal && typeof(IExternalDataProvider).IsAssignableFrom(t))
                                        {
                                            return t;
                                        }
                                        else if (!isExternal && !typeof(IExternalDataProvider).IsAssignableFrom(t))
                                        {
                                            return t;
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
                            return null;
                        })
                        .Where(t => t != null);

                }
                catch (Exception ex)
                {
                    Local.Admin.LogException(ex);
                }
            }
            return new Type[0];
        }

        public static DataProvider[] ReadDatabaseDataProviders()
        {
            using (var dataContext = new CprBroker.Data.DataProviders.DataProvidersDataContext())
            {
                var dbProviders = (from prov in dataContext.DataProviders
                                   where prov.IsEnabled == true
                                   orderby prov.Ordinal
                                   select prov).ToArray();
                return dbProviders;
            }
        }

        public static IEnumerable<IDataProvider> LoadExternalDataProviders(DataProvider[] dbProviders, Type interfaceType)
        {
            return dbProviders
                .AsEnumerable()
                .Select(dbProv =>
                {
                    IDataProvider dataProvider = null;
                    try
                    {
                        Type providerType = Type.GetType(dbProv.TypeName);
                        if (providerType != null && interfaceType.IsAssignableFrom(providerType) && typeof(IExternalDataProvider).IsAssignableFrom(providerType))
                        {
                            dataProvider = CreateDataProvider(dbProv) as IDataProvider;
                        }
                    }
                    catch (Exception ex)
                    {
                        Local.Admin.LogException(ex);
                    }
                    return dataProvider;
                })
                .Where(prov => prov != null);
        }

        public static IEnumerable<IDataProvider> LoadLocalDataProviders(DataProvidersConfigurationSection section, Type interfaceType)
        {
            return GetAvailableDataProviderTypes(section, interfaceType, false)
                .Select(
                    t => Reflection.CreateInstance<IDataProvider>(t)
                );
        }

        #endregion

        #region Filtration by type


        public static IEnumerable<IDataProvider> GetDataProviderList(DataProvidersConfigurationSection section, DataProvider[] dbProviders, Type interfaceType, SourceUsageOrder localOption)
        {
            switch (localOption)
            {
                case SourceUsageOrder.ExternalOnly:
                    return LoadExternalDataProviders(dbProviders, interfaceType);

                case SourceUsageOrder.LocalOnly:
                    return LoadLocalDataProviders(section, interfaceType);

                case SourceUsageOrder.LocalThenExternal:
                    return LoadLocalDataProviders(section, interfaceType)
                        .Concat(
                            LoadExternalDataProviders(dbProviders, interfaceType)
                        );

                default:
                    return new IDataProvider[0];
            }
        }

        #endregion


    }
}
