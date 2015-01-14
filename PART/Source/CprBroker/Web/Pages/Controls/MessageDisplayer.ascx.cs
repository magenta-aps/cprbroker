using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CprBroker.Web.Pages.Controls
{
    public partial class MessageDisplayer : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PreRender += MessageDisplayer_PreRender;
        }

        public readonly List<string> AlertMessages = new List<string>();

        void MessageDisplayer_PreRender(object sender, EventArgs e)
        {
            // Render script elements
            this.divMessages.InnerHtml = string
                .Join("<br><br>", this.AlertMessages.ToArray())
                .Replace("\r\n","<br>");
            if (AlertMessages.Count > 0)
            {
                string val = "<script language=\"javascript\">"
                    + "openDialog('dialog-box')"
                    + "</script>";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "alerts", val);
            }
        }
    }
}