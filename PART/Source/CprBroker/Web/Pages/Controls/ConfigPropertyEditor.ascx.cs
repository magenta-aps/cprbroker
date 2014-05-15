using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CprBroker.Web.Controls
{
    public partial class ConfigPropertyEditor : System.Web.UI.UserControl, IControlWithDataSource
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public Dictionary<string, string> ToDictionary()
        {
            var props = new Dictionary<string, string>();
            foreach (DataListItem item in this.EditDataList.Items)
            {
                SmartTextBox smartTextBox = item.FindControl("SmartTextBox") as SmartTextBox;
                string propName = EditDataList.DataKeys[item.ItemIndex].ToString();
                props[propName] = smartTextBox.Text;
            }
            return props;
        }

        public object DataSource
        {
            get { return this.EditDataList.DataSource; }
            set { this.EditDataList.DataSource = value; }
        }

        public int RepeatColumns
        {
            get { return this.EditDataList.RepeatColumns; }
            set { this.EditDataList.RepeatColumns = value; }
        }
    }
}