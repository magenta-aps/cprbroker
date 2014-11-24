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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CprBroker.Web.Controls
{
    public partial class PeriodSelector : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.DataBind();

                txtFrom.Text = Request.Params["From"];
                txtTo.Text = Request.Params["To"];
                lnkGoDate.HRef = this.Request.Url.ToString();
            }
        }

        public override void DataBind()
        {
            base.DataBind();
            lvPeriod.DataBind();
        }

        protected void lvPeriod_DataBinding(object sender, EventArgs e)
        {
            lvPeriod.DataSource = Enum.GetNames(typeof(LogPeriod));
        }

        public LogPeriod CurrentPeriod
        {
            get
            {
                LogPeriod period = LogPeriod.Day;
                try
                {
                    period = (LogPeriod)Enum.Parse(typeof(LogPeriod), Page.Request.Params["period"]);
                }
                catch { }
                return period;
            }
        }

        public DateTime? CurrentFromDate
        {
            get
            {
                DateTime ret;
                if (DateTime.TryParse(txtFrom.Text, out ret))
                    return ret.Date;
                else
                    return null;
            }
        }

        public DateTime? CurrentToDate
        {
            get
            {
                DateTime ret;
                if (DateTime.TryParse(txtTo.Text, out ret))
                    return ret.Date;
                else
                    return null;
            }
        }

        public DateTime EffectiveFromDate
        {
            get
            {
                if (this.IsInPeriodMode)
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
                else
                {
                    return CurrentFromDate.Value;
                }
            }
        }

        public DateTime? EffectiveToDate
        {
            get
            {
                if (IsInPeriodMode)
                {
                    return null;
                }
                else
                {
                    return CurrentToDate;
                }
            }
        }

        public bool IsInPeriodMode
        {
            get
            {
                return !(CurrentFromDate.HasValue && CurrentToDate.HasValue);
            }
        }

        protected string CreatePeriodLink(object period)
        {
            return WebUtils.CreateLink("Period", period.ToString(), Request.Url, "From", "To");
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
                lnkGoDate.HRef = WebUtils.CreateLink(name, txt.Text, new Uri(lnkGoDate.HRef), "Period");
            }
            else if (string.IsNullOrEmpty(txt.Text.Trim()))
            {
                lnkGoDate.HRef = WebUtils.CreateLink(name, "", new Uri(lnkGoDate.HRef), "Period");
            }
            if (!this.IsInPeriodMode)
            {
                Response.Redirect(lnkGoDate.HRef);
            }
        }
    }
}