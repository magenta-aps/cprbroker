using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;


namespace CprBroker.Engine
{
    interface IReversePersonMappingDataProvider : IBatchDataProvider<string, PersonIdentifier, object>
    { }

    interface IReadDataProvider : ISingleDataProvider<PersonIdentifier, RegistreringType1, ApplicationHeader>
    { }

    interface INon : ISingleDataProvider<RegistreringType1, RegistreringType1, object>
    { }

    interface IGeoLocator : ISingleDataProvider<AdresseType, GeographicCoordinateTupleType, object>
    { }

    public class ListMethodPlan
    {
        public RegistreringType1[] Run(IDataProvider[] dataProviders, string[] input)
        {
            //ConnectinPoint<string,int> conn = (s)=> 0;

            var uuidCaller = new ProviderMethod<string, PersonIdentifier, Element<string, PersonIdentifier>, object, IReversePersonMappingDataProvider>();
            var readCaller = new ProviderMethod<PersonIdentifier, RegistreringType1, Element<PersonIdentifier, RegistreringType1>, ApplicationHeader, IReadDataProvider>();
            var nonCaller = new ProviderMethod<RegistreringType1, RegistreringType1, Element<RegistreringType1, RegistreringType1>, object, INon>();
            var geoCaller = new ProviderMethod<AdresseType, GeographicCoordinateTupleType, Element<AdresseType, GeographicCoordinateTupleType>, object, IGeoLocator>();


            var ret = uuidCaller
                .Cascade<RegistreringType1>(input, dataProviders, readCaller);

            var ret2 = uuidCaller
                .MethodExpression(dataProviders, input)
                .Cascade<string,  PersonIdentifier,   PersonIdentifier,   RegistreringType1>(dataProviders, readCaller, s => s)            
                .Cascade<string,  RegistreringType1,  RegistreringType1,  RegistreringType1>(dataProviders, nonCaller, s => s);

            return null;

        }
    }
}
