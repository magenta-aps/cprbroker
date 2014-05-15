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

        #region Select/Edit/Cancel
        protected void dsDbr_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            dsDbr.SelectParameters["@TypeId"].DefaultValue = CprBroker.Providers.CPRDirect.DbrBaseQueue.TargetQueueTypeId.ToString();
        }

        protected IHasConfigurationProperties grdDbrPropertiesField_ObjectCreating(IHasEncryptedAttributes arg)
        {
            return QueueBase.ToQueue(arg as CprBroker.Data.Queues.Queue);
        }
        #endregion

        #region Ping
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
        #endregion

        #region Insert
        protected void newDbr_DataBinding(object sender, EventArgs e)
        {
            newDbr.DataSource = new DbrQueue().ToAllPropertyInfo();
        }

        protected void newDbr_InsertCommand(object sender, Dictionary<string, string> props)
        {
            var queue = QueueBase.AddQueue<DbrQueue>(CprBroker.Providers.CPRDirect.DbrBaseQueue.TargetQueueTypeId, props, 10, 10);
            this.grdDbr.DataBind();
            this.newDbr.DataBind();
        }
        #endregion
    }
}