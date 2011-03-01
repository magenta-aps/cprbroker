using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Utilities;

namespace CprBroker.Data.Applications
{
    /// <summary>
    /// Represets the Application table
    /// </summary>
    public partial class Application
    {
        /// <summary>
        /// Converts the object to an OIO object
        /// </summary>
        /// <returns></returns>
        public ApplicationType ToXmlType()
        {
            ApplicationType newApp = new ApplicationType();
            newApp.ApplicationId = ApplicationId.ToString();
            newApp.Name = Name;
            newApp.Token = Token.ToString();
            newApp.RegistrationDate = RegistrationDate;
            newApp.IsApproved = IsApproved;
            if (ApprovedDate.HasValue)
                newApp.ApprovedDate = ApprovedDate.Value;
            return newApp;
        }

        partial void OnValidate(System.Data.Linq.ChangeAction action)
        {
            if (action == System.Data.Linq.ChangeAction.None)
            {
                return;
            }

            using (ApplicationDataContext context = new ApplicationDataContext())
            {
                if (action == System.Data.Linq.ChangeAction.Insert || action == System.Data.Linq.ChangeAction.Update)
                {
                    var exists = (from app in context.Applications
                                  where app.ApplicationId != ApplicationId
                                  && (app.Token.ToLower() == Token.ToLower() || app.Name.ToLower() == Name.ToLower())
                                  select app).Count() > 0;
                    if (exists)
                    {
                        throw new Exception(Constants.TextMessages.NameOrTokenAlreadyExists);
                    }
                }
                else if (action == System.Data.Linq.ChangeAction.Delete)
                {
                    // TODO: Handle this case
                    /*var hasSubscriptions = (fromDate sub in context.Subscriptions
                                            where sub.ApplicationId == this.ApplicationId
                                            select sub).Count() > 0;
                    if (hasSubscriptions)
                    {
                        throw new Exception(TextMessages.CannotDeleteApplicationBecauseItHasSubscriptions);
                    }*/
                }
            }
        }
    }
}
