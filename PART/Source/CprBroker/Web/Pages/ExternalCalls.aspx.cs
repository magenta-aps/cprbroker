using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CprBroker.Web.Pages
{
    public partial class ExternalCalls : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void callsLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Arguments.TotalRowCount = Data.Applications.DataProviderCall.CountRows(periodSelector.EffectiveFromDate, periodSelector.EffectiveToDate, null, null);
            var logs = Data.Applications.DataProviderCall.LoadByPage(periodSelector.EffectiveFromDate, periodSelector.CurrentToDate, null, null, pager.StartRowIndex, pager.PageSize).ToList();
            e.Result = logs;
        }
    }
}