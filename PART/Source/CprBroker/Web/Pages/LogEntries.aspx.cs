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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
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
            if (!IsPostBack)
            {
                lvPeriod.DataBind();
                lvType.DataBind();
                lnkGoDate.HRef = this.Request.Url.ToString();
                txtFrom.Text = Request.Params["From"];
                txtTo.Text = Request.Params["To"];
            }
            this.PreRender += new EventHandler(LogEntries_PreRender);
        }

        void LogEntries_PreRender(object sender, EventArgs e)
        {
            DateTime dt;
            lnkGoDate.Disabled = !DateTime.TryParse(txtFrom.Text, out dt) || !DateTime.TryParse(txtTo.Text, out dt); 
            return;
            txtFrom.Attributes["onchange"] = string.Format("SetDateUrl('{0}','From','{1}')", txtFrom.ClientID, lnkGoDate.ClientID);
            txtTo.Attributes["onchange"] = string.Format("SetDateUrl('{0}','To','{1}')", txtTo.ClientID, lnkGoDate.ClientID);
            lnkGoDate.HRef = CreatePeriodLink("");
            lnkGoDate.Attributes["url"] = CreatePeriodLink("");
            lnkGoDate.Attributes["base"] = UrlWithoutQuery(Request.Url.ToString());
        }

        protected void txtFrom_TextChanged(object sender, EventArgs e)
        {
            CreateDateLink(txtFrom, "From");
        }

        protected void txtTo_TextChanged(object sender, EventArgs e)
        {
            CreateDateLink(txtTo, "To");
        }

        protected void CreateDateLink(TextBox txt, string name)
        {
            DateTime date = DateTime.MinValue;
            if (DateTime.TryParse(txt.Text, out date))
            {
                lnkGoDate.HRef = CreateLink(name, txt.Text, new Uri(lnkGoDate.HRef), "Period");
            }
        }

        protected void lvPeriod_DataBinding(object sender, EventArgs e)
        {
            lvPeriod.DataSource = Enum.GetNames(typeof(LogPeriod));
        }

        protected void lvType_DataBinding(object sender, EventArgs e)
        {
            lvType.DataSource = Enum.GetNames(typeof(LogType));
        }

        protected string CreatePeriodLink(object period)
        {
            return CreateLink("Period", period.ToString(), Request.Url, "From", "To");
        }

        protected string CreateTypeLink(object type)
        {
            return CreateLink("Type", type.ToString(), Request.Url);
        }

        protected string CreateAppLink(object appName)
        {
            return CreateLink("App", appName.ToString(), Request.Url);
        }

        protected string UrlWithoutQuery(string url)
        {
            var full = url;
            var ind = full.IndexOf('?');
            var urlWithoutQuery = ind >= 0 ? full.Substring(0, ind) : full;
            return urlWithoutQuery;
        }

        protected string CreateLink(string name, string value, Uri baseUrl, params string[] namesToRemove)
        {
            var parameters = HttpUtility.ParseQueryString(baseUrl.Query);

            if (string.IsNullOrEmpty(value))
                parameters.Remove(name);
            else
                parameters[name] = value;

            foreach (var nameToRemove in namesToRemove)
                parameters.Remove(nameToRemove);

            var urlWithoutQuery = UrlWithoutQuery(baseUrl.ToString());
            if (parameters.Count > 0)
                return urlWithoutQuery + "?" + parameters.ToString();
            else
                return urlWithoutQuery;
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

        protected LogPeriod CurrentPeriod
        {
            get
            {
                LogPeriod period = LogPeriod.Day;
                try
                {
                    period = (LogPeriod)Enum.Parse(typeof(LogPeriod), Request.Params["period"]);
                }
                catch { }
                return period;
            }
        }

        DateTime CurrentStartDate
        {
            get
            {
                switch (CurrentPeriod)
                {
                    case LogPeriod.Hour:
                        return DateTime.Now.AddHours(-1);

                    case LogPeriod.Day:
                        return DateTime.Now.AddDays(-1);

                    case LogPeriod.Week:
                        return DateTime.Now.AddDays(-7);

                    case LogPeriod.Month:
                        return DateTime.Now.AddMonths(-1);
                }
                return DateTime.Now.AddDays(-1);
            }
        }

        TraceEventType? CurrentType
        {
            get
            {
                TraceEventType? type = null;
                try
                {
                    type = (TraceEventType)Enum.Parse(typeof(TraceEventType), Request.Params["Type"]);
                }
                catch { }
                return type;
            }
        }

        string CurrentAppName
        {
            get
            {
                return Request.Params["App"];
            }
        }


        protected void logEntriesLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Arguments.TotalRowCount = (int)Data.Statistics.stat.CountRowsByStatistics<Data.Applications.LogEntry>(Config.Properties.Settings.Default.CprBrokerConnectionString, TimeSpan.FromMinutes(15));
            var logs = Data.Applications.LogEntry.LoadByPage(CurrentStartDate, CurrentType, CurrentAppName, pager.StartRowIndex, pager.PageSize).ToList();
            foreach (var log in logs)
            {
                log.Text = log.Text.Replace("<", "&lt;").Replace(">", "&gt;");
            }
            e.Result = logs;
        }
    }
}
