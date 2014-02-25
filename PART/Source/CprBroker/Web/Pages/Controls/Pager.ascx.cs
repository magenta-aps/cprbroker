using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CprBroker.Web.Controls
{
    public partial class Pager : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string PagedControlID
        {
            get { return pager.PagedControlID; }
            set { pager.PagedControlID = value; }
        }

        public int StartRowIndex
        {
            get { return pager.StartRowIndex; }
        }

        public int PageSize
        {
            get { return pager.PageSize; }
        }
    }
}