using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Engine;
using CPRBroker.Schemas;
using CPRBroker.Schemas.Part;
using CPRBroker.DAL;
using CPRBroker;

namespace CPRBroker.Engine.Local
{
    public partial class UpdateDatabase
    {
        public static void UpdatePersonRegistration(PersonRegistration personRegistraion)
        {
            
        }

        public static Guid[] AssignGuids(params PersonIdentifier[] personPnrAndBirthdates)
        {
            return Array.ConvertAll<PersonIdentifier, Guid>(personPnrAndBirthdates, (t) => Guid.NewGuid());
        }

        public static TResult[] AssignGuids<TSource,TResult>(TSource[] objects, Converter<TSource,TResult> converter, Converter<TSource,PersonIdentifier> personIdentifierGetter, Action<TResult, Guid> idSetter)
        {
            var ret = Array.ConvertAll<TSource, TResult>(objects,converter);
            var identifiers  = Array.ConvertAll<TSource, PersonIdentifier>(objects, personIdentifierGetter);
            
            Guid[] ids = AssignGuids(identifiers);
            for (int i = 0; i < ids.Length; i++)
            {
                idSetter(ret[i], ids[i]);
            }
            return ret;
        }
    }
}
