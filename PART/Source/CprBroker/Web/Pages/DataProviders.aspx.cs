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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using CprBroker.Data.DataProviders;
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Web.Controls;
using System.Web.Configuration;
using CprBroker.Utilities;

namespace CprBroker.Web.Pages
{
    public partial class DataProviders : System.Web.UI.Page
    {

        #region Load & Data bind

        protected void Page_Load(object sender, EventArgs e)
        {
            BrokerContext.Initialize(Constants.BaseApplicationToken.ToString(), Constants.UserToken);
            if (!IsPostBack)
            {
                dataProviderTypesGridView.DataBind();
                dataProvidersGridView.DataBind();
                newDataProviderDropDownList.DataBind();
                newDataProviderGridView.DataBind();
            }
        }

        protected void dataProviderTypesGridView_DataBinding(object sender, EventArgs e)
        {
            dataProviderTypesGridView.DataSource = DataProviderManager.GetAvailableDataProviderTypes(true);
        }

        protected void dataProvidersGridView_DataBinding(object sender, EventArgs e)
        {
            using (var dataContext = new DataProvidersDataContext())
            {
                dataProvidersGridView.DataSource = LoadDataProviders(dataContext);
            }
        }

        protected void dataProvidersGridView_DataBound(object sender, EventArgs e)
        {
            if (dataProvidersGridView.Rows.Count > 0)
            {
                var up = dataProvidersGridView.Rows[0].FindControl("UpButton");
                if (up != null)
                {
                    up.Visible = false;
                }
                var down = dataProvidersGridView.Rows[dataProvidersGridView.Rows.Count - 1].FindControl("DownButton");
                if (down != null)
                {
                    down.Visible = false;
                }
            }
        }

        protected void newDataProviderDropDownList_DataBinding(object sender, EventArgs e)
        {
            newDataProviderDropDownList.DataSource = DataProviderManager.GetAvailableDataProviderTypes(true);
        }

        protected void newDataProviderDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            newDataProviderGridView.DataBind();
        }

        protected void newDataProviderGridView_DataBinding(object sender, EventArgs e)
        {
            if (newDataProviderDropDownList.Items.Count > 0)
            {
                Type t = Type.GetType(newDataProviderDropDownList.SelectedItem.Value);
                var dp = t.InvokeMember(null, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, null, null) as IExternalDataProvider;
                newDataProviderGridView.DataSource = dp.ToAllPropertyInfo();
            }
        }

        #endregion

        #region Update

        protected void dataProvidersGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            dataProvidersGridView.EditIndex = e.NewEditIndex;
            dataProvidersGridView.DataBind();
            e.Cancel = true;
        }

        protected void dataProvidersGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            dataProvidersGridView.EditIndex = -1;
            dataProvidersGridView.DataBind();
        }

        protected void dataProvidersGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            var valuesDataList = dataProvidersGridView.Rows[e.RowIndex].Cells[0].FindControl("EditDataList") as DataList;
            var id = (Guid)this.dataProvidersGridView.DataKeys[e.RowIndex].Value;

            using (var dataContext = new CprBroker.Data.DataProviders.DataProvidersDataContext())
            {
                DataProvider dbPrrov = (from dp in dataContext.DataProviders where dp.DataProviderId == id select dp).Single();
                foreach (DataListItem item in valuesDataList.Items)
                {
                    SmartTextBox smartTextBox = item.FindControl("SmartTextBox") as SmartTextBox;
                    if (smartTextBox.Confidential)
                    {
                        // Only update the password if something is entered to avoid erasing the password by mistake
                        if (!string.IsNullOrEmpty(smartTextBox.Text))
                        {
                            dbPrrov[valuesDataList.DataKeys[item.ItemIndex].ToString()] = smartTextBox.Text;
                        }
                    }
                    else
                    {
                        dbPrrov[valuesDataList.DataKeys[item.ItemIndex].ToString()] = smartTextBox.Text;
                    }
                }
                dataContext.SubmitChanges();
                dataProvidersGridView.EditIndex = -1;
                dataProvidersGridView.DataBind();
            }
        }

        #endregion

        #region Delete

        protected void dataProvidersGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            using (var dataContext = new CprBroker.Data.DataProviders.DataProvidersDataContext())
            {
                var id = (Guid)this.dataProvidersGridView.DataKeys[e.RowIndex].Value;
                var dbProv = dataContext.DataProviders.Where(dp => dp.DataProviderId == id).FirstOrDefault();
                dataContext.DataProviders.DeleteOnSubmit(dbProv);
                dataContext.SubmitChanges();
                dataProvidersGridView.DataBind();
            }
        }

        #endregion

        #region Ping

        protected void dataProvidersGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ping")
            {
                using (var dataContext = new DataProvidersDataContext())
                {
                    var id = new Guid(e.CommandArgument.ToString());
                    DataProvider dbProv = dataContext.DataProviders.Where(p => p.DataProviderId == id).OrderBy(dp => dp.Ordinal).SingleOrDefault();
                    IDataProvider prov = DataProviderManager.CreateDataProvider(dbProv);

                    if (prov.IsAlive())
                    {
                        Master.AlertMessages.Add("Ping succeeded");
                    }
                    else
                    {
                        Master.AlertMessages.Add("Ping failed");
                    }
                }
            }
            else if (e.CommandName == "Enable")
            {
                using (var dataContext = new DataProvidersDataContext())
                {
                    var id = new Guid(e.CommandArgument.ToString());
                    DataProvider dbProv = dataContext.DataProviders.Where(p => p.DataProviderId == id).SingleOrDefault();
                    dbProv.IsEnabled = !(dbProv.IsEnabled);
                    dataContext.SubmitChanges();
                    dataProvidersGridView.DataBind();
                }
            }
            else if (e.CommandName == "Up" || e.CommandName == "Down")
            {
                using (var dataContext = new DataProvidersDataContext())
                {
                    var id = new Guid(e.CommandArgument.ToString());
                    var dataProviders = LoadDataProviders(dataContext);
                    DataProvider dbProv = dataProviders.First(p => p.DataProviderId == id);

                    for (int index = 0; index < dataProviders.Length; index++)
                    {
                        dataProviders[index].Ordinal = index;
                    }

                    int providerIndex = Array.IndexOf<DataProvider>(dataProviders, dbProv);
                    if (providerIndex != -1)
                    {
                        if (e.CommandName == "Up" && providerIndex > 0)
                        {
                            dataProviders[providerIndex].Ordinal = providerIndex - 1;
                            dataProviders[providerIndex - 1].Ordinal = providerIndex;

                        }
                        else if (e.CommandName == "Down" && providerIndex < dataProviders.Length - 1)
                        {
                            dataProviders[providerIndex].Ordinal = providerIndex + 1;
                            dataProviders[providerIndex + 1].Ordinal = providerIndex;
                        }
                        dataContext.SubmitChanges();
                        dataProvidersGridView.DataBind();
                    }
                }
            }
        }

        #endregion

        #region Insert


        protected void newDataProviderGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Insert")
            {
                try
                {
                    using (var dataContext = new CprBroker.Data.DataProviders.DataProvidersDataContext())
                    {
                        DataProvider dbProv = new DataProvider()
                        {
                            DataProviderId = Guid.NewGuid(),
                            IsExternal = true,
                            TypeName = newDataProviderDropDownList.SelectedValue,
                            Ordinal = dataContext.DataProviders.OrderByDescending(dp => dp.Ordinal).Select(p => p.Ordinal).FirstOrDefault() + 1,
                            IsEnabled = true
                        };

                        foreach (GridViewRow item in newDataProviderGridView.Rows)
                        {
                            SmartTextBox smartTextBox = item.FindControl("SmartTextBox") as SmartTextBox;
                            string propName = newDataProviderGridView.DataKeys[item.RowIndex].Value.ToString();
                            dbProv[propName] = smartTextBox.Text;
                        }

                        dataContext.DataProviders.InsertOnSubmit(dbProv);
                        dataContext.SubmitChanges();
                        dataProvidersGridView.DataBind();
                        newDataProviderGridView.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    Master.AppendError(ex.Message);
                }
            }
        }

        #endregion

        #region Utility methods

        protected string GetShortTypeName(string assebblyQualifiedName)
        {
            try
            {
                Type t = Type.GetType(assebblyQualifiedName);
                if (t != null)
                {
                    return t.Name;
                }
            }
            catch (Exception ex)
            {
                Master.AppendError(ex.Message);
            }
            return assebblyQualifiedName;
        }

        protected Array GetAttributes(object dbProvider)
        {
            // TODO: Include IPerCallData providers here

            var dbProv = dbProvider as Data.DataProviders.DataProvider;
            var paidProv = DataProviderManager.CreatePaidDataProvider(dbProv);

            var prov = DataProviderManager.CreateDataProvider(dbProv);
            var properties = dbProv.GetProperties();
            var configKeys = prov.ToAllPropertyInfo();

            return (from pInfo in configKeys
                    join pp in properties
                    on pInfo.Name equals pp.Name into joined
                    from pVal in joined.DefaultIfEmpty()
                    select new
                    {
                        Name = pInfo.Name,
                        Value = (pVal == null || pInfo.Confidential) ? null : pVal.Value,
                        Confidential = pInfo.Confidential,
                        Required = pInfo.Required,
                        Type = pInfo.Type
                    }).ToArray();
        }

        protected DataProvider[] LoadDataProviders(DataProvidersDataContext dataContext)
        {
            return dataContext.DataProviders.Where(dp => dp.IsExternal == true).OrderBy(dp => dp.Ordinal).ToArray();
        }
        #endregion
    }
}
