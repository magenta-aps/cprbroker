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
using System.Reflection;
using System.Text;
using System.Threading;
using System.Data.Linq;
using System.IO;
using CprBroker.Data;
using CprBroker.Data.DataProviders;
using CprBroker.Utilities;
using CprBroker.Schemas;
using CprBroker.Utilities.Config;

namespace CprBroker.Engine
{
    /// <summary>
    /// Manages the loading and selection of data providers
    /// </summary>
    public class DataProviderFactory
    {
        public IEnumerable<string> AvailableTypesSource { get; set; }
        public IEnumerable<DataProvider> ConfiguredProvidersSource { get; set; }

        public DataProviderFactory()
        {
            this.AvailableTypesSource = CprBroker.Utilities.Config.ConfigManager.Current.DataProvidersSection;
            this.ConfiguredProvidersSource = new DataProviderDatabaseReader();
        }

        #region Creators
        /// <summary>
        /// Converts the current DataProvider (database object) to the appropriate IDataProvider object based on its type
        /// </summary>
        /// <param name="dbDataProvider">The database object that represents the data provider</param>
        /// <returns>The newly created IDataProvider</returns>
        public IExternalDataProvider CreateDataProvider(CprBroker.Data.DataProviders.DataProvider dbDataProvider)
        {
            IExternalDataProvider dataProvider = Utilities.Reflection.CreateInstance<IExternalDataProvider>(dbDataProvider.TypeName);
            if (dataProvider is IExternalDataProvider)
            {
                try
                {
                    dataProvider.FillFromEncryptedStorage(dbDataProvider);
                }
                catch (Exception ex)
                {
                    Local.Admin.LogException(ex);
                }
            }
            return dataProvider;
        }

        public IPerCallDataProvider CreatePaidDataProvider(CprBroker.Data.DataProviders.DataProvider dbDataProvider)
        {
            IPerCallDataProvider dataProvider = Utilities.Reflection.CreateInstance<IPerCallDataProvider>(dbDataProvider.TypeName);
            if (dataProvider is IPerCallDataProvider)
            {
                try
                {
                    dataProvider.FillFromEncryptedStorage(dbDataProvider);
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

        public TInterface GetDataProvider<TInterface>(Guid dataProviderId) where TInterface : class, IExternalDataProvider
        {
            using (var dataContext = new DataProvidersDataContext())
            {
                var dbProv = dataContext.DataProviders.Where(p => p.DataProviderId == dataProviderId).SingleOrDefault();
                if (dbProv != null)
                    return CreateDataProvider(dbProv) as TInterface;
            }
            return null;
        }

        public IEnumerable<Type> GetAvailableDataProviderTypes(bool isExternal)
        {
            return GetAvailableDataProviderTypes(null, isExternal);
        }

        public IEnumerable<Type> GetAvailableDataProviderTypes(Type interfaceType, bool isExternal)
        {
            return GetAvailableDataProviderTypes(AvailableTypesSource, interfaceType, isExternal);

        }

        public IEnumerable<Type> GetAvailableDataProviderTypes(IEnumerable<string> typeNames, Type interfaceType, bool isExternal)
        {
            if (interfaceType == null)
            {
                interfaceType = typeof(IDataProvider);
            }

            if (typeNames != null)
            {
                try
                {
                    return typeNames
                        .Select(typeName =>
                        {
                            try
                            {
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

        public DataProvider[] ReadDatabaseDataProviders()
        {
            return this.ConfiguredProvidersSource.ToArray();
        }

        public IEnumerable<IDataProvider> LoadExternalDataProviders(DataProvider[] dbProviders, Type interfaceType)
        {
            return dbProviders
                .AsEnumerable()
                .Select(dbProv =>
                {
                    IDataProvider dataProvider = null;
                    try
                    {
                        Type providerType = Type.GetType(dbProv.TypeName);
                        if (providerType != null && typeof(IExternalDataProvider).IsAssignableFrom(providerType))
                        {
                            if (interfaceType.IsAssignableFrom(providerType)
                                || providerType.GetInterface(interfaceType.Name) != null
                                )
                            {
                                dataProvider = CreateDataProvider(dbProv) as IDataProvider;
                            }
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

        public IEnumerable<IDataProvider> LoadLocalDataProviders(IEnumerable<string> typeNames, Type interfaceType)
        {
            return GetAvailableDataProviderTypes(typeNames, interfaceType, false)
                .Select(
                    t => Reflection.CreateInstance<IDataProvider>(t)
                );
        }

        #endregion

        #region Filtration by type

        private int GetClassOrder(IDataProvider p)
        {
            /*
             * PreLocal = 1
             * Local = 2
             * PostLocal = 3
             * PureExternal = 4
             */
            if (p is IExternalDataProvider)
            {
                if (p is ILocalProxyDataProvider)
                {
                    switch ((p as ILocalProxyDataProvider).LocalProxyUsage)
                    {
                        case LocalProxyUsageOptions.BeforeLocal:
                            return 1;
                        case LocalProxyUsageOptions.AfterLocal:
                            return 3;
                        default: //LocalProxyUsageOptions.Default
                            return 4;
                    }
                }
                else
                {
                    return 4;
                }
            }
            else
            {
                return 2; // Local
            }
        }

        public IEnumerable<IDataProvider> GetDataProviderList(IEnumerable<string> knownTypeNames, DataProvider[] dbProviders, Type interfaceType, SourceUsageOrder localOption)
        {
            var realLocals = LoadLocalDataProviders(knownTypeNames, interfaceType);
            var allExternals = LoadExternalDataProviders(dbProviders, interfaceType);

            int i = 0;
            var all = realLocals.Concat(allExternals)
                .Select(p => new
                {
                    Prov = p,
                    Index = i++,
                    External = (p is IExternalDataProvider),
                    Class = GetClassOrder(p)
                });

            switch (localOption)
            {
                case SourceUsageOrder.ExternalOnly:
                    // All externals in the defined order
                    all = all
                        .Where(p => p.External)
                        .OrderBy(p => p.Index);
                    break;

                case SourceUsageOrder.LocalOnly:
                    // Pre local -> local -> post local
                    all = all
                        .Where(p => new int[] { 1, 2, 3 }.Contains(p.Class))
                        .OrderBy(p => p.Class)
                        .ThenBy(p => p.Index);
                    break;

                case SourceUsageOrder.LocalThenExternal:
                    all = all.OrderBy(p => p.Class);
                    break;

                default:
                    all = all.Take(0);
                    break;
            }

            return all.Select(p => p.Prov).AsEnumerable();
        }

        #endregion

    }
}
