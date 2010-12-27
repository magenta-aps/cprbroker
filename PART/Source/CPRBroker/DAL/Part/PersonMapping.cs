using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Util;

namespace CprBroker.DAL.Part
{
    public partial class PersonMapping
    {
        /// <summary>
        /// Maps the passed PersonIdentifier objects to UUIDs
        /// Any unfound persons are assigned new UUID's and saved to the mapping table
        /// </summary>
        /// <param name="personIdentifiers"></param>
        /// <returns></returns>
        public static Guid[] AssignGuids(params PersonIdentifier[] personIdentifiers)
        {
            Guid[] ret = new Guid[personIdentifiers.Length];

            using (PartDataContext dataContext = new PartDataContext())
            {
                var foundPersons = dataContext.PersonMappings.AsQueryable();
                var pred = PredicateBuilder.False<PersonMapping>();

                foreach (var personIdentifier in personIdentifiers)
                {
                    string cprNumber = personIdentifier.CprNumber;
                    pred = pred.Or((d) => d.CprNumber == cprNumber);
                }

                foundPersons = foundPersons.Where(pred);

                var foundPersonsArray = foundPersons.ToArray();

                for (int iPerson = 0; iPerson < personIdentifiers.Length; iPerson++)
                {
                    var personIdentifier = personIdentifiers[iPerson];
                    var personMapping = (from d in foundPersonsArray where d.CprNumber == personIdentifier.CprNumber select d).FirstOrDefault();

                    if (personMapping == null)
                    {
                        personMapping = new PersonMapping();
                        //TODO: Replace this with a call to Gentofte UUID service
                        personMapping.UUID = Guid.NewGuid();
                        personMapping.CprNumber = personIdentifier.CprNumber;
                        dataContext.PersonMappings.InsertOnSubmit(personMapping);
                    }
                    ret[iPerson] = personMapping.UUID;
                    personIdentifiers[iPerson].UUID = personMapping.UUID;
                }
                dataContext.SubmitChanges();
            }
            return ret;
        }

        /// <summary>
        /// Maps the given objects to other objects that contain a UUID
        /// Used primarily to directly convert database relation objects (logical) to PersonRelation objects (with physical UUID)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="objects"></param>
        /// <param name="converter"></param>
        /// <param name="personIdentifierGetter"></param>
        /// <param name="idSetter"></param>
        /// <returns></returns>
        public static TResult[] AssignGuids<TSource, TResult>(TSource[] objects, Converter<TSource, TResult> converter, Converter<TSource, PersonIdentifier> personIdentifierGetter, Action<TResult, Guid> idSetter)
        {
            var ret = Array.ConvertAll<TSource, TResult>(objects, converter);
            var identifiers = Array.ConvertAll<TSource, PersonIdentifier>(objects, personIdentifierGetter);

            Guid[] ids = AssignGuids(identifiers);
            for (int i = 0; i < ids.Length; i++)
            {
                idSetter(ret[i], ids[i]);
            }
            return ret;
        }

        // TODO: Move the main logic for UUID assignment to this function bacause it is the most simple
        public static Guid[] AssignGuids(params string[] cprNumbers)
        {
            var pIds = Array.ConvertAll<string, PersonIdentifier>(cprNumbers, (cpr) => new PersonIdentifier() { CprNumber = cpr });
            return AssignGuids(pIds);
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
