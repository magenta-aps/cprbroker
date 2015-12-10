/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

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
            e.WhereParameters["TypeId"] = CprBroker.Providers.CPRDirect.DbrBaseQueue.TargetQueueTypeId;
        }

        protected IHasConfigurationProperties grdDbrPropertiesField_ObjectCreating(IHasEncryptedAttributes arg)
        {
            return Queue.ToQueue(arg as CprBroker.Data.Queues.DbQueue);
        }
        #endregion

        #region Ping
        protected void grdDbr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ping")
            {
                var dbr = Queue.GetById<DbrQueue>(new Guid(e.CommandArgument.ToString()));
                if (dbr != null && dbr.IsAlive())
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
            var queue = Queue.AddQueue<DbrQueue>(CprBroker.Providers.CPRDirect.DbrBaseQueue.TargetQueueTypeId, props, 500, 10);
            this.grdDbr.DataBind();
            this.newDbr.DataBind();
        }
        #endregion
    }
}