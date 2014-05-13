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

        protected void grdDbr_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            var valuesDataList = grdDbr.Rows[e.RowIndex].Cells[0].FindControl("configEditor") as ConfigPropertyEditor;
            var id = (Guid)this.grdDbr.DataKeys[e.RowIndex].Value;
            QueueBase.UpdateAttributesById(id, valuesDataList.ToDictionary());
            
            grdDbr.EditIndex = -1;
            grdDbr.DataBind();
        }

        protected void grdDbr_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            grdDbr.EditIndex = -1;
            grdDbr.DataBind();
        }

        protected void grdDbr_DataBinding(object sender, EventArgs e)
        {
            grdDbr.DataSource = CprBroker.Engine.Queues.QueueBase.GetQueues<DbrQueue>();
        }

        protected void grdDbr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ping")
            {
                var dbr = QueueBase.GetById<DbrQueue>(new Guid(e.CommandArgument.ToString()));
                if (dbr.IsAlive())
                {
                    Master.AlertMessages.Add("Ping succeeded");
                }
                else
                {
                    Master.AlertMessages.Add("Ping failed");
                }
            }
        }

        protected void grdDbr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            grdDbr.EditIndex = e.NewEditIndex;
            grdDbr.DataBind();
        }

        protected void grdDbr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var id = (Guid)this.grdDbr.DataKeys[e.RowIndex].Value;
            QueueBase.DeleteById(id);
            grdDbr.DataBind();
        }

        protected void newDbr_DataBinding(object sender, EventArgs e)
        {
            newDbr.DataSource = new DbrQueue().ToAllPropertyInfo();
        }

        protected void newDataProviderGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Insert")
            {
                var props = new Dictionary<string, string>();
                foreach (GridViewRow item in this.newDbr.Rows)
                {
                    SmartTextBox smartTextBox = item.FindControl("SmartTextBox") as SmartTextBox;
                    string propName = newDbr.DataKeys[item.RowIndex].Value.ToString();
                    props[propName] = smartTextBox.Text;
                }
                var queue = QueueBase.AddQueue<DbrQueue>(CprBroker.Providers.CPRDirect.DbrBaseQueue.TargetQueueTypeId, props, 10, 10);
                this.grdDbr.DataBind();
                this.newDbr.DataBind();
            }
        }
    }
}