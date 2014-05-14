using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CprBroker.Web.Controls
{
    public partial class ConfigPropertyGridEditor : System.Web.UI.UserControl
    {
        public event Action<ConfigPropertyGridEditor, Dictionary<string, string>> InsertCommand;

        public event Action<ConfigPropertyGridEditor, Dictionary<string, string>> OnInsertCommand
        {
            add { InsertCommand += value; }
            remove { InsertCommand -= value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void grd_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Insert" && InsertCommand != null)
            {
                var props = new Dictionary<string, string>();
                foreach (GridViewRow item in this.grd.Rows)
                {
                    SmartTextBox smartTextBox = item.FindControl("SmartTextBox") as SmartTextBox;
                    string propName = grd.DataKeys[item.RowIndex].Value.ToString();
                    props[propName] = smartTextBox.Text;
                }
                InsertCommand(this, props);
            }
        }

        public object DataSource
        {
            get { return this.grd.DataSource; }
            set { this.grd.DataSource = value; }
        }
    }
}