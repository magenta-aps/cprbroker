using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CprServices
{
    /// <summary>
    /// Provides a one constructor time calculation of the search plan
    /// </summary>
    public class SearchPlan
    {
        public bool IsSatisfactory { get; private set; }

        public List<SearchMethodCall> PlannedCalls = new List<SearchMethodCall>();

        public SearchPlan(SearchRequest request, params SearchMethod[] availableMethods)
        {
            var inputFields = request.CriteriaFields.Where(f => !string.IsNullOrEmpty(f.Value)).ToList();
            var planMethods = new List<SearchMethod>();
            var usedFieldNames = new List<string>();
            foreach (var method in availableMethods)
            {
                if (method.CanBeUsedFor(request))
                {
                    planMethods.Add(method);
                    var newUsedFieldNames =
                        from inp in inputFields
                        from mf in method.InputFields
                        where inp.Key == mf.Name
                        select inp.Key;

                    usedFieldNames.AddRange(newUsedFieldNames.Except(usedFieldNames));
                    if (usedFieldNames.Count == inputFields.Count)// No need to use the other search methods
                        break;
                }
            }
            if (usedFieldNames.Count < inputFields.Count)
            {
                IsSatisfactory = false;
            }
            else
            {
                IsSatisfactory = true;
                PlannedCalls.AddRange(planMethods.Select(pm => new SearchMethodCall(pm, request)));
            }
        }
    }
}
