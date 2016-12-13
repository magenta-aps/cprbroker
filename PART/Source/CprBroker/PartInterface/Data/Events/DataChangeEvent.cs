using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Data.Events
{
    partial class DataChangeEvent
    {
        public static void DeletePerson(Guid personUuid)
        {
            using (var dataContext = new DataChangeEventDataContext())
            {
                var dataChangeEventsToDelete = dataContext.DataChangeEvents
                    .Where(dce => dce.PersonUuid == personUuid);
                dataContext.DataChangeEvents.DeleteAllOnSubmit(dataChangeEventsToDelete);

                dataContext.SubmitChanges();
            }
        }
    }
}
