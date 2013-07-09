// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Default.aspx.cs" company="Gentofte Kommune">
//   Copyright 2010 Gentofte Kommune
// </copyright>
// <summary>
//   Defines the Default type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace CPRNotificationServiceDemo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Default class
    /// </summary>
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void SubscribeButton_OnClick(object sender, EventArgs e)
        {
            var notificationUrl = CalcNotificationUrl();

            var cprAdapter = new CprAdministrationAdapter();
            cprAdapter.Subscribe(CPRTextBox.Text, notificationUrl);
        }

        protected void ShowNotificationsButton_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("ShowNotifications.aspx", true);
        }

        private string CalcNotificationUrl()
        {
            var context = HttpContext.Current;
            var t = this.Request.Url.AbsoluteUri;
            t = t.Substring(0, t.LastIndexOf('/') + 1);
            t += "Notification.asmx";
            return t;
        }

    }
}