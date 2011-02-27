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
using CprBroker.DAL;
using CprBroker.Utilities;

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
            CprBroker.DAL.Applications.Application newApp = e.NewObject as CprBroker.DAL.Applications.Application;
            CprBroker.DAL.Applications.Application orgApp = e.OriginalObject as CprBroker.DAL.Applications.Application;

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
            CprBroker.DAL.Applications.Application newApp = e.NewObject as CprBroker.DAL.Applications.Application;
            newApp.ApplicationId = Guid.NewGuid();
            newApp.RegistrationDate = DateTime.Now;
            if (newApp.IsApproved)
            {
                newApp.ApprovedDate = DateTime.Now;
            }
        }

        protected void newApplicationDetailsView_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            e.KeepInInsertMode = true;

            if (e.Exception != null)
            {
                Master.AppendError(e.Exception.Message);
                e.ExceptionHandled = true;
            }
            else
            {
                applicationsGridView.DataBind();
                newApplicationDetailsView.DataBind();
            }
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
