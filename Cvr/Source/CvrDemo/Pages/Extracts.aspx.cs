using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CvrDemo.Pages
{
    public partial class Extracts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void RefreshButton_Click(object sender, EventArgs e) {
            //TODO: do update stuff here...
            Label textField = (Label)frmUpdateDatabase.FindControl("StatusText");
            textField.Text = "Testing";
        }
    }
}