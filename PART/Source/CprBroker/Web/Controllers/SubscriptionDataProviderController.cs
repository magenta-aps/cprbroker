using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CprBroker.Engine;
using System.Data.Linq;

namespace CprBroker.Web.Controllers
{
    [RoutePrefix("mvc/pages/dataproviders")]
    public class SubscriptionDataProviderController : Controller
    {
        [Route("{dataProviderId:Guid}")]
        public ActionResult ViewDetails(Guid dataProviderId)
        {
            CprBroker.Engine.BrokerContext.Initialize(
                CprBroker.Utilities.Constants.BaseApplicationToken.ToString(),
                this.HttpContext.User.Identity.Name);

            var factory = new DataProviderFactory();
            var dataProvider = factory.GetDataProvider<ISubscriptionManagerDataProvider>(dataProviderId);

            return View("DataProviderView", new Tuple<ISubscriptionManagerDataProvider, Guid>(dataProvider, dataProviderId));
        }

        [Route("{dataProviderId:Guid}/putsubscription/{field}/{value}")]
        public ActionResult PutSubscription(Guid dataProviderId, string field, string value)
        {
            CprBroker.Engine.BrokerContext.Initialize(
                CprBroker.Utilities.Constants.BaseApplicationToken.ToString(),
                this.HttpContext.User.Identity.Name);

            var factory = new DataProviderFactory();
            var dataProvider = factory.GetDataProvider<ISubscriptionManagerDataProvider>(dataProviderId);

            try
            {
                var ret = dataProvider.PutSubscription(field, value);
                if (ret)
                    return new HttpStatusCodeResult(200);
                else
                    return new HttpStatusCodeResult(500, "PutSubscription failed");
            }
            catch (Exception ex)
            {
                CprBroker.Engine.Local.Admin.LogException(ex);
                return new HttpStatusCodeResult(500, ex.ToString());
            }
        }

        [Route("{dataProviderId:Guid}/removesubscription/{field}/{value}")]
        public ActionResult RemoveSubscription(Guid dataProviderId, string field, string value)
        {
            CprBroker.Engine.BrokerContext.Initialize(
                CprBroker.Utilities.Constants.BaseApplicationToken.ToString(),
                this.HttpContext.User.Identity.Name);

            var factory = new DataProviderFactory();
            var dataProvider = factory.GetDataProvider<ISubscriptionManagerDataProvider>(dataProviderId);

            try
            {
                var ret = dataProvider.RemoveSubscription(field, value);
                if (ret)
                    return new HttpStatusCodeResult(200);
                else
                    return new HttpStatusCodeResult(500, "RemoveSubscription failed");
            }
            catch (Exception ex)
            {
                CprBroker.Engine.Local.Admin.LogException(ex);
                return new HttpStatusCodeResult(500, ex.ToString());
            }
        }
    }
}