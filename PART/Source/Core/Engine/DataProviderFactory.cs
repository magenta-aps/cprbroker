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
        public DataProviderFactory()
        {
            this.AvailableTypesSource = CprBroker.Utilities.Config.ConfigManager.Current.DataProvidersSection;
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
        public IEnumerable<string> AvailableTypesSource { get; set; }

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
            using (var dataContext = new CprBroker.Data.DataProviders.DataProvidersDataContext())
            {
                var dbProviders = (from prov in dataContext.DataProviders
                                   where prov.IsEnabled == true
                                   orderby prov.Ordinal
                                   select prov).ToArray();
                return dbProviders;
            }
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


        public IEnumerable<IDataProvider> GetDataProviderList(IEnumerable<string> knownTypeNames, DataProvider[] dbProviders, Type interfaceType, SourceUsageOrder localOption)
        {
            switch (localOption)
            {
                case SourceUsageOrder.ExternalOnly:
                    return LoadExternalDataProviders(dbProviders, interfaceType);

                case SourceUsageOrder.LocalOnly:
                    return LoadLocalDataProviders(knownTypeNames, interfaceType);

                case SourceUsageOrder.LocalThenExternal:
                    return LoadLocalDataProviders(knownTypeNames, interfaceType)
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
