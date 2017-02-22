using CprBroker.Data.Applications;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CprBroker.Web.Controllers
{
    public class LogDisplayParameters
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int PageSize { get; set; } = 20;
        public int PageNumber { get; set; } = 0;

        public DateTime EffectiveFrom { get { return From ?? CprBroker.Utilities.Constants.MinSqlDate; } }
        public DateTime EffectiveTo { get { return To ?? CprBroker.Utilities.Constants.MaxSqlDate; } }
    }

    [RoutePrefix("Pages/LogDisplay")]
    public class LogDisplayController : Controller
    {
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("ActivityList")]
        public ActionResult ActivityList(LogDisplayParameters pars)
        {
            using (var dc = new ApplicationDataContext())
            {
                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<Activity>(ac => ac.Application);
                dc.LoadOptions = loadOptions;
                var acts = dc.Activities.Where(a => a.StartTS >= pars.EffectiveFrom && a.StartTS <= pars.EffectiveTo)
                    .OrderByDescending(a => a.StartTS)
                    .Skip(pars.PageSize * pars.PageNumber)
                    .Take(pars.PageSize)
                    .ToArray();

                return PartialView(acts);
            }
        }

        [Route("Details/{activityId}")]
        public ActionResult Activity(Guid activityId)
        {
            using (var dc = new ApplicationDataContext())
            {
                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<Activity>(a => a.LogEntries);
                loadOptions.LoadWith<Activity>(a => a.Operations);
                loadOptions.LoadWith<Activity>(a => a.DataProviderCalls);

                loadOptions.LoadWith<LogEntry>(a => a.LogType);
                loadOptions.LoadWith<Operation>(a => a.OperationType);
                dc.LoadOptions = loadOptions;
                var act = dc.Activities.SingleOrDefault(a => a.ActivityId == activityId);
                return PartialView("Activity", act);
            }
        }

        public ActionResult Operations(LogDisplayParameters pars)
        {
            using (var dc = new ApplicationDataContext())
            {
                var acts = dc.Operations.Where(a => a.Activity.StartTS >= pars.EffectiveFrom && a.Activity.StartTS <= pars.EffectiveTo)
                    .OrderByDescending(a => a.Activity.StartTS)
                    .Skip(pars.PageSize * pars.PageNumber)
                    .Take(pars.PageNumber)
                    .ToArray();

                return View(acts);
            }
        }

        public ActionResult ExternalCalls(LogDisplayParameters pars)
        {
            using (var dc = new ApplicationDataContext())
            {
                var calls = dc.DataProviderCalls.Where(a => a.CallTime >= pars.EffectiveFrom && a.CallTime <= pars.EffectiveTo)
                    .OrderByDescending(a => a.CallTime)
                    .Skip(pars.PageSize * pars.PageNumber)
                    .Take(pars.PageNumber)
                    .ToArray();

                return View(calls);
            }
        }

        public ActionResult Logs(LogDisplayParameters pars, object type)
        {
            return View();
        }


    }
}