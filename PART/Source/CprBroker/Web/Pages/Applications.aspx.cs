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
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using CprBroker.Data;
using CprBroker.Utilities;
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Web.Pages
{
    public partial class Applications : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void baseApplicationLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.WhereParameters["ApplicationId"] = Constants.BaseApplicationId;
        }

        protected void applicationsLinqDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.WhereParameters["ApplicationId"] = Constants.BaseApplicationId;
        }

        protected void applicationsLinqDataSource_Updating(object sender, LinqDataSourceUpdateEventArgs e)
        {
            CprBroker.Data.Applications.Application newApp = e.NewObject as CprBroker.Data.Applications.Application;
            CprBroker.Data.Applications.Application orgApp = e.OriginalObject as CprBroker.Data.Applications.Application;

            // Approved date
            if (newApp.IsApproved && !orgApp.IsApproved)
            {
                newApp.ApprovedDate = DateTime.Now;
            }
            if (!newApp.IsApproved)
            {
                newApp.ApprovedDate = null;
            }
        }

        protected void applicationsGridView_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            if (e.Exception != null)
            {
                Master.AppendError(e.Exception.Message);

                e.ExceptionHandled = true;
                e.KeepInEditMode = true;
            }
        }

        protected void baseApplicationDetailsView_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            if (e.Exception != null)
            {
                Master.AppendError(e.Exception.Message);

                e.ExceptionHandled = true;
                e.KeepInEditMode = true;
            }
        }

        protected void applicationsLinqDataSource_Inserting(object sender, LinqDataSourceInsertEventArgs e)
        {
            e.Cancel=true;
            
            CprBroker.Data.Applications.Application newApp = e.NewObject as CprBroker.Data.Applications.Application;
            var result = Manager.Admin.RequestAppRegistration(Constants.UserToken, Constants.BaseApplicationToken.ToString(), newApp.Name);
            Master.AppendErrorIfPossible(result);
            if (StandardReturType.IsSucceeded(result.StandardRetur))
            {
                if (newApp.IsApproved)
                {
                    var approveResult = Manager.Admin.ApproveAppRegistration(Constants.UserToken, Constants.BaseApplicationToken.ToString(), result.Item.Token);
                    Master.AppendErrorIfPossible(approveResult);
                }
            }
            applicationsGridView.DataBind();
            newApplicationDetailsView.DataBind();            
        }

        protected void applicationsGridView_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            if (e.Exception != null)
            {
                Master.AppendError(e.Exception.Message);

                e.ExceptionHandled = true;
            }
        }


    }
}
