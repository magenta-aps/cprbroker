using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Engine;
using CPRBroker.Schemas;
using CPRBroker.Schemas.Part;
using CPRBroker.DAL;
using CPRBroker.DAL.Part;
using CPRBroker;

namespace CPRBroker.Engine.Local
{

    public partial class UpdateDatabase
    {
        public static void UpdatePersonRegistration(PersonRegistration personRegistraion)
        {

        }

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
                }
                dataContext.SubmitChanges();
            }
            return ret;
        }

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
    }
}
