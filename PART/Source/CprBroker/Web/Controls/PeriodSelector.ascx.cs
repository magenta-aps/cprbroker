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