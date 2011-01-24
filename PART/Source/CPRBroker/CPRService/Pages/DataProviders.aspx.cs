using System;
using System.Collections;
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
using CprBroker.DAL.DataProviders;
using CprBroker.Engine;
using CprBroker.Web.Controls;

namespace CprBroker.Web.Pages
{
    public partial class DataProviders : System.Web.UI.Page
    {

        #region Load & Data bind

        protected void Page_Load(object sender, EventArgs e)
        {
            BrokerContext.Initialize(CprBroker.DAL.Applications.Application.BaseApplicationToken.ToString(), CprBroker.Engine.Constants.UserToken, false, false, true);
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
                DataProvider.SetChildLoadOptions(dataContext);
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
            Type t = Type.GetType(newDataProviderDropDownList.SelectedItem.Value);
            IExternalDataProvider dp = t.InvokeMember(null, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, null, null) as IExternalDataProvider;
            newDataProviderGridView.DataSource = from ck in dp.ConfigurationKeys select new { Value = ck };
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
            var id = (int)this.dataProvidersGridView.DataKeys[e.RowIndex].Value;

            using (var dataContext = new CprBroker.DAL.DataProviders.DataProvidersDataContext())
            {
                DataProvider dbPrrov = (from dp in dataContext.DataProviders where dp.DataProviderId == id select dp).Single();
                foreach (DataListItem item in valuesDataList.Items)
                {
                    SmartTextBox smartTextBox = item.FindControl("SmartTextBox") as SmartTextBox;
                    dbPrrov[valuesDataList.DataKeys[item.ItemIndex].ToString()] = smartTextBox.Text;
                }
                dataContext.SubmitChanges();
                dataProvidersGridView.EditIndex = -1;
                dataProvidersGridView.DataBind();
                CprBroker.Engine.DataProviderManager.InitializeDataProviders();
            }
        }

        #endregion

        #region Delete

        protected void dataProvidersGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            using (var dataContext = new CprBroker.DAL.DataProviders.DataProvidersDataContext())
            {
                var id = (int)this.dataProvidersGridView.DataKeys[e.RowIndex].Value;
                var dbProv = dataContext.DataProviders.Where(dp => dp.DataProviderId == id).FirstOrDefault();
                dataContext.DataProviders.DeleteOnSubmit(dbProv);
                dataContext.SubmitChanges();
                dataProvidersGridView.DataBind();
                CprBroker.Engine.DataProviderManager.InitializeDataProviders();
            }
        }

        #endregion

        #region Ping

        protected void dataProvidersGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var id = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName == "Ping")
            {
                using (var dataContext = new DataProvidersDataContext())
                {
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
                    DataProvider dbProv = dataContext.DataProviders.Where(p => p.DataProviderId == id).SingleOrDefault();
                    dbProv.IsEnabled = !(dbProv.IsEnabled);
                    dataContext.SubmitChanges();
                    dataProvidersGridView.DataBind();
                    DataProviderManager.InitializeDataProviders();
                }
            }
            else if (e.CommandName == "Up" || e.CommandName == "Down")
            {
                using (var dataContext = new DataProvidersDataContext())
                {
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
                        DataProviderManager.InitializeDataProviders();
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
                    using (var dataContext = new CprBroker.DAL.DataProviders.DataProvidersDataContext())
                    {
                        DataProvider dbPrrov = new DataProvider()
                        {
                            DataProviderTypeId = 1,
                            IsExternal = true,
                            TypeName = newDataProviderDropDownList.SelectedValue,
                            Ordinal = dataContext.DataProviders.Select(dp => dp.Ordinal).Max() + 1,
                            IsEnabled = true
                        };
                        dataContext.DataProviders.InsertOnSubmit(dbPrrov);

                        foreach (GridViewRow item in newDataProviderGridView.Rows)
                        {
                            SmartTextBox smartTextBox = item.FindControl("SmartTextBox") as SmartTextBox;
                            string propName = newDataProviderGridView.DataKeys[item.RowIndex].Value.ToString();
                            dbPrrov[propName] = smartTextBox.Text;
                        }
                        dataContext.SubmitChanges();
                        dataProvidersGridView.DataBind();
                        newDataProviderGridView.DataBind();
                        CprBroker.Engine.DataProviderManager.InitializeDataProviders();
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

        protected DataProviderProperty[] SortDataProviderProperties(object props)
        {
            return (props as System.Data.Linq.EntitySet<DataProviderProperty>).OrderBy(p => p.Ordinal).ToArray();
        }

        protected DataProvider[] LoadDataProviders(DataProvidersDataContext dataContext)
        {
            return dataContext.DataProviders.Where(dp => dp.IsExternal).OrderBy(dp => dp.Ordinal).ToArray();
        }
        #endregion
    }
}
