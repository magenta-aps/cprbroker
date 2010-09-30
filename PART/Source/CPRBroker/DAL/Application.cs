using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas;

namespace CPRBroker.DAL
{
    public partial class Application
    {
        /// <summary>
        /// Token of preapproved base application that comes with a first installation
        /// </summary>
        public static readonly Guid BaseApplicationId = new Guid("3E9890FF-0038-42A4-987A-99B63E8BC865");
        public static readonly Guid BaseApplicationToken = new Guid("07059250-E448-4040-B695-9C03F9E59E38");

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

            using (CPRBrokerDALDataContext context = new CPRBrokerDALDataContext())
            {
                if (action == System.Data.Linq.ChangeAction.Insert || action == System.Data.Linq.ChangeAction.Update)
                {
                    var exists = (from app in context.Applications
                                  where app.ApplicationId != ApplicationId
                                  && (app.Token.ToLower() == Token.ToLower() || app.Name.ToLower() == Name.ToLower())
                                  select app).Count() > 0;
                    if (exists)
                    {
                        throw new Exception(TextMessages.NameOrTokenAlreadyExists);
                    }
                }
                else if (action == System.Data.Linq.ChangeAction.Delete)
                {
                    var hasSubscriptions = (from sub in context.Subscriptions
                                            where sub.ApplicationId == this.ApplicationId
                                            select sub).Count() > 0;
                    if (hasSubscriptions)
                    {
                        throw new Exception(TextMessages.CannotDeleteApplicationBecauseItHasSubscriptions);
                    }
                }
            }
        }
    }
}
