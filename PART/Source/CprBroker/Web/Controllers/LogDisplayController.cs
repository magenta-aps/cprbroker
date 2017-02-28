using CprBroker.Data.Applications;
using CprBroker.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CprBroker.Web.Controllers
{
    public class LogDisplayParameters
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Contains { get; set; }
        public Guid? ApplicationId { get; set; }

        public int PageSize { get; set; } = 20;
        public int PageNumber { get; set; } = 0;

        public DateTime EffectiveFrom { get { return From ?? CprBroker.Utilities.Constants.MinSqlDate; } }
        public DateTime EffectiveTo { get { return To ?? CprBroker.Utilities.Constants.MaxSqlDate; } }
        public ActivityContentTypes? EffectiveContains
        {
            get
            {
                ActivityContentTypes ret;
                if (Enum.TryParse<ActivityContentTypes>(Contains, out ret) && ret != ActivityContentTypes.None)
                    return ret;
                else
                    return null;
            }
        }

    }

    [Flags]
    public enum ActivityContentTypes
    {
        None = 0,
        Errors = 1,
        Information = 2,
        Warnings = 4,
        ExternalCalls = 8,
        Operations = 16,
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
                var pred = PredicateBuilder.True<Activity>();
                pred = pred.And(a => a.StartTS >= pars.EffectiveFrom.Date && a.StartTS < pars.EffectiveTo.Date.AddDays(1));
                if (pars.ApplicationId.HasValue)
                    pred = pred.And(a => a.ApplicationId == pars.ApplicationId);
                if (pars.EffectiveContains.HasValue && pars.EffectiveContains != ActivityContentTypes.None)
                {
                    var containsPred = PredicateBuilder.False<Activity>();

                    if ((pars.EffectiveContains.Value & ActivityContentTypes.Errors) != 0)
                    {
                        containsPred = containsPred.Or(a => a.HasErrors.Value == true);
                    }
                    if ((pars.EffectiveContains.Value & ActivityContentTypes.Information) != 0)
                    {
                        containsPred = containsPred.Or(a => a.HasInformation.Value == true);
                    }
                    if ((pars.EffectiveContains.Value & ActivityContentTypes.Warnings) != 0)
                    {
                        containsPred = containsPred.Or(a => a.HasWarnings.Value == true);
                    }
                    if ((pars.EffectiveContains.Value & ActivityContentTypes.ExternalCalls) != 0)
                    {
                        containsPred = containsPred.Or(a => a.HasDataProviderCalls.Value == true);
                    }
                    if ((pars.EffectiveContains.Value & ActivityContentTypes.Operations) != 0)
                    {
                        containsPred = containsPred.Or(a => a.HasOperations.Value == true);
                    }

                    pred = pred.And(containsPred);
                }

                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<Activity>(ac => ac.Application);
                dc.LoadOptions = loadOptions;

                var acts = dc.Activities.Where(pred)
                    .OrderByDescending(a => a.StartTS)
                    .Skip(pars.PageSize * pars.PageNumber)
                    .Take(pars.PageSize)
                    .ToArray();

                var totalItemCount = dc.Activities.Where(pred).Count();

                return PartialView(new PagedList.StaticPagedList<Activity>(acts, pars.PageNumber + 1, pars.PageSize, totalItemCount));
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