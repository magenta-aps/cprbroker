using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Data.Applications
{
    partial class Operation
    {
        public static KeyValuePair<string, Operation[]>[] Get(string[] keys, OperationType.Types[] types, DateTime? fromDate, DateTime? toDate)
        {
            // Input corrections
            if (fromDate == null)
                fromDate = CprBroker.Utilities.Constants.MinSqlDate;

            if (toDate == null)
                toDate = DateTime.Now;
            if (toDate.Value.Equals(toDate.Value.Date))
                toDate = toDate.Value.Date.AddDays(1).AddMilliseconds(-1);

            var typeIds = types.Select(t => (int)t).ToArray();

            using (var context = new ApplicationDataContext())
            {
                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith((Operation o) => o.Activity);
                context.LoadOptions = loadOptions;

                var ops = context.Operations
                    .Where(o =>
                        keys.Contains(o.OperationKey)
                        && typeIds.Contains(o.OperationTypeId)
                        && o.Activity.StartTS >= fromDate
                        && o.Activity.StartTS <= toDate
                    )
                    .GroupBy(o => o.OperationKey)
                    .ToArray();

                return Enumerable.Range(0, keys.Length)
                    .Select(i =>
                    {
                        var grp = ops.SingleOrDefault(op => keys[i].Equals(op.Key));
                        var keyObjects = (grp == null) ?
                            new Operation[] { } :
                            grp.ToArray();
                        return new KeyValuePair<string, Operation[]>(keys[i], keyObjects);
                    })
                    .ToArray();
            }
        }
    }
}
