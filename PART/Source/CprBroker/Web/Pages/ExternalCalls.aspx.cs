using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CprBroker.Data.Applications;

namespace CprBroker.Web.Pages
{
    public partial class ExternalCalls : System.Web.UI.Page
    {
        protected decimal Cost { get; set; }
        protected int RowCount { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.grdCalls.DataBound += new EventHandler(grdCalls_DataBound);
        }

        void grdCalls_DataBound(object sender, EventArgs e)
        {
            summaryTable.DataBind();
        }

        protected void callsLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            RowCount = DataProviderCall.CountRows(periodSelector.EffectiveFromDate, periodSelector.EffectiveToDate, null, null);
            Cost = DataProviderCall.SumCost(periodSelector.EffectiveFromDate, periodSelector.EffectiveToDate, null, null);

            e.Arguments.TotalRowCount = RowCount;
            var logs = DataProviderCall.LoadByPage(periodSelector.EffectiveFromDate, periodSelector.CurrentToDate, null, null, pager.StartRowIndex, pager.PageSize).ToList();
            e.Result = logs;
        }
    }
}