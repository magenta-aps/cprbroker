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
using CprBroker.Engine;
using CprBroker.Utilities;

namespace CprBroker.Web.Pages
{
    public partial class Site : System.Web.UI.MasterPage
    {
        public Site()
        {
            BrokerContext.Initialize(Constants.BaseApplicationToken.ToString(), Constants.UserToken);
            if (HttpContext.Current != null && HttpContext.Current.Handler is Page)
            {
                (HttpContext.Current.Handler as Page).Error += new EventHandler(Page_Error);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        void Page_Error(object sender, EventArgs e)
        {            
            Exception ex = Server.GetLastError();
            AppendError(ex.Message);
            Response.Write(ex.Message);
            CprBroker.Engine.Local.Admin.LogException(ex);
            Server.ClearError();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Render script elements
            string val = "<script language=\"javascript\">";
            val += string.Join("",
                (
                    from text in AlertMessages
                    select "alert('" + text.Replace("'", "''") + "');"
                ).ToArray()
                );
            val += "</script>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "alerts", val);
        }

        public void AppendError(string text)
        {
            errorLabel.Text += text;
        }

        public readonly List<string> AlertMessages = new List<string>();
    }
}
