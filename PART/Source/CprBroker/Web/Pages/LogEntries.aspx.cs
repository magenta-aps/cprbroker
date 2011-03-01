using System;
using System.Collections;
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

namespace CprBroker.Web.Pages
{
    public partial class LogEntries : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void logEntriesLinqDataSource_Selected(object sender, LinqDataSourceStatusEventArgs e)
        {
            var list = e.Result as List<CprBroker.Data.Applications.LogEntry>;
            foreach (var logEntry in list)
            {
                if (!string.IsNullOrEmpty(logEntry.Text))
                {
                    logEntry.Text = logEntry.Text.Replace(Environment.NewLine, "<br/>");
                }
                if (!string.IsNullOrEmpty(logEntry.DataObjectXml))
                {
                    logEntry.DataObjectXml = logEntry.DataObjectXml.Replace(Environment.NewLine, "<br/>");
                }
            }
        }
    }
}
