using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Utilities;

namespace CprBroker.DAL.Part
{
    public partial class PersonMapping
    {

        /// <summary>
        /// Maps CPR Numbers to UUIDs
        /// </summary>
        /// <param name="cprNumbers"></param>
        /// <returns></returns>
        public static Guid?[] AssignGuids(params string[] cprNumbers)
        {
            Guid?[] ret = new Guid?[cprNumbers.Length];

            using (PartDataContext dataContext = new PartDataContext())
            {
                var foundPersons = dataContext.PersonMappings.AsQueryable();
                var pred = PredicateBuilder.False<PersonMapping>();

                foreach (var cprNumber in cprNumbers)
                {
                    pred = pred.Or((d) => d.CprNumber == cprNumber);
                }

                foundPersons = foundPersons.Where(pred);

                var foundPersonsArray = foundPersons.ToArray();

                for (int iPerson = 0; iPerson < cprNumbers.Length; iPerson++)
                {
                    var cprNumber = cprNumbers[iPerson];
                    var personMapping = (from d in foundPersonsArray where d.CprNumber == cprNumber select d).FirstOrDefault();

                    if (personMapping != null)
                    {
                        ret[iPerson] = personMapping.UUID;
                    }

                }
                dataContext.SubmitChanges();
            }
            return ret;
        }

        /// <summary>
        /// Maps a person UUID to a PersonIdentifier object.
        /// Returns null if no match is found.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public static PersonIdentifier GetPersonIdentifier(Guid uuid)
        {
            PersonIdentifier ret = null;
            using (var dataContext = new PartDataContext())
            {
                ret =
                (
                    from pm in dataContext.PersonMappings
                    where pm.UUID == uuid
                    select new PersonIdentifier()
                    {
                        CprNumber = pm.CprNumber,
                        UUID = uuid
                    }
                ).FirstOrDefault();
            }
            return ret;
        }
    }
}
