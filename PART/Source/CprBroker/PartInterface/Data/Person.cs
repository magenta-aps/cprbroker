using CprBroker.Data.Part;
using CprBroker.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Data.Part
{
    public partial class Person
    {
        public static void Delete(PersonIdentifier personIdentifier)
        {
            using (var dataContext = new PartDataContext())
            {
                var registrationsToDelete = dataContext.PersonRegistrations.Where(pr => pr.UUID == personIdentifier.UUID);
                dataContext.PersonRegistrations.DeleteAllOnSubmit(registrationsToDelete);

                var actorsToDelete = registrationsToDelete.Select(r => r.ActorRef);
                dataContext.ActorRefs.DeleteAllOnSubmit(actorsToDelete);

                var personsToDelete = dataContext.Persons.Where(p => p.UUID == personIdentifier.UUID);
                dataContext.Persons.DeleteAllOnSubmit(personsToDelete);

                dataContext.SubmitChanges();
            }
        }
    }
}
