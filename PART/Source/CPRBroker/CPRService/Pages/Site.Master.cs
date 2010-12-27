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
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

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
            Page.ClientScript.RegisterStartupScript(this.GetType(),"alerts", val);
        }

        public void AppendError(string text)
        {
            errorLabel.Text += text;
        }

        public readonly List<string> AlertMessages = new List<string>();
    }
}
