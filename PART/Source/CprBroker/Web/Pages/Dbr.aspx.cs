using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CprBroker.DBR;
using CprBroker.Engine;
using CprBroker.Engine.Queues;
using CprBroker.Data;
using CprBroker.Data.Queues;
using CprBroker.Web.Controls;
using CprBroker.Utilities;

namespace CprBroker.Web.Pages
{
    public partial class Dbr : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BrokerContext.Initialize(Constants.BaseApplicationToken.ToString(), Constants.UserToken);
            if (!IsPostBack)
            {
                this.grdDbr.DataBind();
                this.newDbr.DataBind();
            }
        }

        protected void grdDbr_DataBinding(object sender, EventArgs e)
        {
            grdDbr.DataSource = CprBroker.Engine.Queues.QueueBase.GetQueues<DbrQueue>();
        }

        protected void newDbr_DataBinding(object sender, EventArgs e)
        {
            newDbr.DataSource = new DbrQueue().ToAllPropertyInfo();
        }

        protected void newDataProviderGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Insert")
            {
                var queue = QueueBase.AddQueue<DbrQueue>(CprBroker.Providers.CPRDirect.DbrBaseQueue.TargetQueueTypeId, 10, 10,
                    q =>
                    {
                        var props = new Dictionary<string, string>();
                        foreach (GridViewRow item in this.newDbr.Rows)
                        {
                            SmartTextBox smartTextBox = item.FindControl("SmartTextBox") as SmartTextBox;
                            string propName = newDbr.DataKeys[item.RowIndex].Value.ToString();
                            props[propName] = smartTextBox.Text;
                        }                        
                        q.ConfigurationProperties = props;
                        q.CopyToEncryptedStorage(q.Impl);
                    });
                this.grdDbr.DataBind();
                this.newDbr.DataBind();
            }
        }
    }
}