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
using CPRBroker.DAL;
using CPRBroker.Engine;

namespace CPRService.Pages
{
    public partial class DataProviders : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

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
                CPRBroker.Engine.DataProviderManager.InitializeDataProviders();
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
                CPRBroker.Engine.DataProviderManager.InitializeDataProviders();
            }
        }

        protected void dataProvidersLinqDataSource_Deleted(object sender, LinqDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                CPRBroker.Engine.DataProviderManager.InitializeDataProviders();
            }
        }

        protected void dataProvidersLinqDataSource_Updated(object sender, LinqDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                CPRBroker.Engine.DataProviderManager.InitializeDataProviders();
            }
        }

        protected void dataProvidersGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ping")
            {
                using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
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

    }
}
