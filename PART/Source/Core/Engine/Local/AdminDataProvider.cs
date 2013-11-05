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
using System.Diagnostics;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Engine;
using CprBroker.Data;
using CprBroker.Data.DataProviders;
using CprBroker.Data.Applications;
using CprBroker.Utilities;

namespace CprBroker.Engine.Local
{
    /// <summary>
    /// Implementation of administration web methods
    /// </summary>
    public partial class AdminDataProvider : IDataProviderManager, IVersionManager, ILoggingDataProvider
    {
        #region IDataProvider Members

        public bool IsAlive()
        {
            return true;
        }

        public Version Version
        {
            get { return new Version(Constants.Versioning.Major, Constants.Versioning.Minor); }
        }
        #endregion

        #region Provider list

        public Schemas.DataProviderType[] GetDataProviderList()
        {
            // GetPropertyValuesOfType data providers fromDate database and convert to the appropriate XML type
            using (var context = new DataProvidersDataContext())
            {
                List<Schemas.DataProviderType> dataProviders = new List<CprBroker.Schemas.DataProviderType>();
                var ret = context.DataProviders
                    .Where(prov => prov.IsExternal)
                    .Select(prov => prov.ToXmlType())
                    .ToArray();

                // Now clear any confidential data
                foreach (var oioProv in ret)
                {
                    var prov = Reflection.CreateInstance<IExternalDataProvider>(oioProv.TypeName);
                    if (prov != null)
                    {
                        // TODO: Include IPerCallDataProvider keys here
                        foreach (var configKey in prov.ConfigurationKeys)
                        {
                            if (configKey.Confidential)
                            {
                                var oioAttr = oioProv.Attributes.Where(a => a.Name == configKey.Name).FirstOrDefault();
                                if (oioAttr != null)
                                {
                                    oioAttr.Value = "";
                                }
                            }
                        }
                    }
                    else
                    {
                        Array.ForEach<AttributeType>(oioProv.Attributes, a => a.Value = "");
                    }
                }
                return ret;
            }
        }

        public bool SetDataProviderList(Schemas.DataProviderType[] dataProviders)
        {
            using (var context = new DataProvidersDataContext())
            {
                #region Delete existing data
                var currentDataProviders =
                    from dataProvider in context.DataProviders
                    //where dataProvider.DataProviderType.IsExternal == true
                    select dataProvider;

                if (currentDataProviders.Count() > 0)
                {
                    context.DataProviders.DeleteAllOnSubmit(currentDataProviders);
                }
                #endregion
                // Add new data providers
                for (int iProv = 0; iProv < dataProviders.Length; iProv++)
                {
                    DataProviderType oio = dataProviders[iProv];
                    var provObj = Reflection.CreateInstance<IExternalDataProvider>(oio.TypeName);
                    // TODO: Also pass IPerDataProvider properties here
                    var dbProv = DataProvider.FromXmlType(oio, iProv, provObj.ConfigurationKeys.Select(p => p.Name).ToArray());
                    context.DataProviders.InsertOnSubmit(dbProv);
                }
                context.SubmitChanges();
                return true;
            }
        }
        #endregion

        #region Versioning managment

        private IQueryable<string> GetMethodNames()
        {
            // Use reflection to get a list of all methods fromDate MethodNames classes
            List<System.Reflection.FieldInfo> fields = new List<System.Reflection.FieldInfo>();
            fields.AddRange(typeof(ServiceNames.Part.Methods).GetFields());
            fields.AddRange(typeof(ServiceNames.Admin.MethodNames).GetFields());
            return
                from f in fields.AsQueryable()
                select f.Name;
        }
        public ServiceVersionType[] GetCapabilities()
        {
            ServiceVersionType cprVersion = new ServiceVersionType();

            cprVersion.Version = string.Format("{0}.{1}", Constants.Versioning.Major, Constants.Versioning.Minor);
            cprVersion.Functions.AddRange(GetMethodNames());

            return new ServiceVersionType[] { cprVersion };
        }

        public bool IsImplementing(string methodName, string version)
        {
            IQueryable<string> fields = GetMethodNames();
            foreach (var field in fields)
            {
                if (string.Equals(field.ToLower(), methodName.ToLower()) &&
                    string.Equals(version.ToLower(), string.Format("{0}.{1}", Constants.Versioning.Major, Constants.Versioning.Minor).ToLower()))
                    return true;
            }
            return false;
        }
        #endregion

        #region ILoggingDataProvider Members
        public bool Log(string text)
        {
            CprBroker.Engine.Local.Admin.AddNewLog(System.Diagnostics.TraceEventType.Information, "LogFunctions", text, null, null);
            return true;
        }

        #endregion
    }
}
