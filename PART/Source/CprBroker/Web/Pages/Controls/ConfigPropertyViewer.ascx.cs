using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CprBroker.Web.Controls
{
   public partial class ConfigPropertyViewer : System.Web.UI.UserControl, IControlWithDataSource
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public object DataSource
        {
            get { return this.DataList1.DataSource; }
            set { this.DataList1.DataSource = value; }
        }
    }
}