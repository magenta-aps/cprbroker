using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Providers.Local.Search
{
    partial class PersonSearchCache
    {
        public static T GetValue<T>(Guid uuid, Func<PersonSearchCache, T> getter)
        {
            using (var dataContext = new PartSearchDataContext())
            {
                var personCache = dataContext.PersonSearchCaches.SingleOrDefault(psc => psc.UUID == uuid);
                return personCache == null ?
                    default(T) :
                    getter(personCache);
            }
        }
    }
}
