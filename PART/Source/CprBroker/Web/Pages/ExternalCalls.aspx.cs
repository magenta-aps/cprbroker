using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CprBroker.Data.Applications;
using CprBroker.Engine;

namespace CprBroker.Web.Pages
{
    public partial class ExternalCalls : System.Web.UI.Page
    {
        protected decimal Cost { get; set; }
        protected int RowCount { get; set; }

        public string CurrentProviderType
        {
            get
            {
                return Page.Request.Params["Prov"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                providerListView.DataBind();
            }
            this.grdCalls.DataBound += new EventHandler(grdCalls_DataBound);
        }

        protected void providerListView_DataBinding(object sender, EventArgs e)
        {
            var s = DataProviderManager.GetAvailableDataProviderTypes(typeof(IPerCallDataProvider), true).Select(t => t.Name).ToArray();
            providerListView.DataSource = s;
        }

        protected string CreateTypeLink(object type)
        {
            return WebUtils.CreateLink("Prov", type.ToString(), Request.Url);
        }

        void grdCalls_DataBound(object sender, EventArgs e)
        {
            summaryTable.DataBind();
        }


        protected void callsLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            RowCount = DataProviderCall.CountRows(periodSelector.EffectiveFromDate, periodSelector.EffectiveToDate, CurrentProviderType, null);
            Cost = DataProviderCall.SumCost(periodSelector.EffectiveFromDate, periodSelector.EffectiveToDate, CurrentProviderType, null);

            e.Arguments.TotalRowCount = RowCount;
            var logs = DataProviderCall.LoadByPage(periodSelector.EffectiveFromDate, periodSelector.CurrentToDate, CurrentProviderType, null, pager.StartRowIndex, pager.PageSize).ToList();
            e.Result = logs;
        }
    }
}