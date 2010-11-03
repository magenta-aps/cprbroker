using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas;
using CPRBroker.Schemas.Util;

namespace CPRBroker.DAL.Part
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
                var foundPersons = from personReg in dataContext.PesronMappings select personReg;
                var pred = PredicateBuilder.False<DAL.Part.PesronMapping>();
                foreach (var personIdentifier in personIdentifiers)
                {
                    pred.Or((d) => d.BirthDate == personIdentifier.Birthdate && d.CprNumber == personIdentifier.CprNumber);
                }
                foundPersons = foundPersons.Where(pred);

                var foundPersonsArray = foundPersons.ToArray();

                for (int iPerson = 0; iPerson < personIdentifiers.Length; iPerson++)
                {
                    var personIdentifier = personIdentifiers[iPerson];
                    var personMapping = (from d in foundPersons where d.BirthDate == personIdentifier.Birthdate && d.CprNumber == personIdentifier.CprNumber select d).FirstOrDefault();

                    if (personMapping == null)
                    {
                        personMapping = new PesronMapping();
                        personMapping.UUID = Guid.NewGuid();
                        personMapping.BirthDate = personIdentifier.Birthdate;
                        personMapping.CprNumber = personIdentifier.CprNumber;
                        dataContext.PesronMappings.InsertOnSubmit(personMapping);
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
                    from pm in dataContext.PesronMappings
                    where pm.UUID == uuid
                    select new PersonIdentifier()
                    {
                        CprNumber = pm.CprNumber,
                        Birthdate = pm.BirthDate,
                        UUID = uuid
                    }
                ).FirstOrDefault();
            }
            return ret;
        }
    }
}
