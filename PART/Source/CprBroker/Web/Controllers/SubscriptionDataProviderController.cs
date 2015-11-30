using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CprBroker.Engine;
using System.Data.Linq;

namespace CprBroker.Web.Controllers
{
    public class SubscriptionDataProviderController : Controller
    {
        // GET: SubscriptionDataProvider
        public ActionResult ViewDetails(Guid dataProviderId)
        {
            var factory = new DataProviderFactory();
            var dataProvider = factory.GetDataProvider<ISubscriptionManagerDataProvider>(dataProviderId);

            return View("DataProviderView", dataProvider);
        }
    }
}