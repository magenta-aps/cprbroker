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
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region Insert

        protected void newDataProviderDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            newDataProviderDetailsView.DataBind();
        }

        protected void newDataProviderDetailsView_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            e.KeepInInsertMode = true;
            if (e.Exception != null)
            {
                Master.AppendError(e.Exception.Message);
                e.ExceptionHandled = true;
            }
            else
            {
                dataProvidersGridView.DataBind();
                newDataProviderDetailsView.DataBind();
                CprBroker.Engine.DataProviderManager.InitializeDataProviders();
            }
        }

        protected void dataProvidersLinqDataSource_Inserting(object sender, LinqDataSourceInsertEventArgs e)
        {
            DataProvider prov = e.NewObject as DataProvider;
            prov.DataProviderTypeId = int.Parse(newDataProviderDropDownList.SelectedValue);
        }

        protected void dataProvidersLinqDataSource_Inserted(object sender, LinqDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                CprBroker.Engine.DataProviderManager.InitializeDataProviders();
            }
        }

        #endregion

        #region Delete

        protected void dataProvidersLinqDataSource_Deleted(object sender, LinqDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                CprBroker.Engine.DataProviderManager.InitializeDataProviders();
            }
        }

        #endregion

        #region Ping

        protected void dataProvidersGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ping")
            {
                using (var context = new DataProvidersDataContext())
                {

                    DataProvider dbProv = (from p in context.DataProviders where p.DataProviderId == Convert.ToInt32(e.CommandArgument) select p).SingleOrDefault();
                    IDataProvider prov = dbProv.ToIDataProvider();

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
        }

        #endregion

        #region Update

        System.Collections.Generic.Dictionary<string, string> dataProviderProps;

        protected void dataProvidersGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            var valuesDataList = dataProvidersGridView.Rows[e.RowIndex].Cells[0].FindControl("EditDataList") as DataList;
            dataProviderProps = new System.Collections.Generic.Dictionary<string, string>();
            foreach (DataListItem item in valuesDataList.Items)
            {
                SmartTextBox smartTextBox = item.FindControl("SmartTextBox") as SmartTextBox;
                dataProviderProps[valuesDataList.DataKeys[item.ItemIndex].ToString()] = smartTextBox.Text;
            }
        }

        protected void dataProvidersLinqDataSource_Updating(object sender, LinqDataSourceUpdateEventArgs e)
        {
            using (var dataContext = new CprBroker.DAL.DataProviders.DataProvidersDataContext())
            {
                var id = (e.NewObject as DataProvider).DataProviderId;
                DataProvider dbPrrov = (from dp in dataContext.DataProviders where dp.DataProviderId == id select dp).Single();
                foreach (var kvp in dataProviderProps)
                {
                    dbPrrov[kvp.Key] = kvp.Value;
                }
                dataContext.SubmitChanges();

                CprBroker.Engine.DataProviderManager.InitializeDataProviders();
                e.Cancel = true;
            }
        }

        protected void dataProvidersLinqDataSource_Updated(object sender, LinqDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {

            }
        }

        protected void dataProvidersGridView_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            object o = "";
        }

        #endregion


    }
}
