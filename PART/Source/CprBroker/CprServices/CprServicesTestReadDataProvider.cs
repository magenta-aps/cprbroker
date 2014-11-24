using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.PartInterface;
using CprBroker.Engine;
using CprBroker.Engine.Part;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;


namespace CprBroker.Providers.CprServices
{
    public class CprServicesTestReadDataProvider : CprServicesDataProvider, IPartReadDataProvider, IExternalDataProvider, IPerCallDataProvider
    {
        public override string[] OperationKeys
        {
            get
            {
                var ret = base.OperationKeys.ToList();
                ret.Add(Constants.OperationKeys.Unfinished.ADRESSE3);
                return ret.ToArray();
            }
        }

        public RegistreringType1 Read(Schemas.PersonIdentifier uuid, LaesInputType input, Func<string, Guid> cpr2uuidFunc, out Schemas.QualityLevel? ql)
        {
            ql = Schemas.QualityLevel.Cpr;

            var addressMethod = new SearchMethod(Properties.Resources.ADRESSE3);
            var addressRequest = new SearchRequest(uuid.CprNumber);
            var addressCall = new SearchMethodCall(addressMethod, addressRequest);

            var xml = addressCall.ToRequestXml(Properties.Resources.SearchTemplate);

            var xmlOut = "";
            string token = this.SignonAndGetToken();

            // Get address
            var kvit = Send(addressCall.Name, xml, ref token, out xmlOut);
            if (kvit.OK)
            {
                var addressPerson = addressCall.ParseResponse(xmlOut, false).First();
                if (string.IsNullOrEmpty(addressPerson.PNR))
                    addressPerson.PNR = uuid.CprNumber;

                var cache = new UuidCache();
                cache.FillCache(new string[] { addressPerson.PNR });
                var ret = addressPerson.ToRegistreringType1(cache.GetUuid);

                // Now get the name
                var nameMethod = new SearchMethod(Properties.Resources.NAVNE3);
                var nameRequest = new SearchRequest(uuid.CprNumber);
                var nameCall = new SearchMethodCall(nameMethod, nameRequest);
                var nameXml = nameCall.ToRequestXml(Properties.Resources.SearchTemplate);

                kvit = Send(nameCall.Name, nameXml, ref token, out xmlOut);
                if (kvit.OK)
                {
                    var namePersons = nameCall.ParseResponse(xmlOut, false);
                    namePersons[0].PNR = uuid.CprNumber;
                    var nameRet = namePersons[0].ToRegistreringType1(cache.GetUuid);
                    ret.AttributListe.Egenskab[0].NavnStruktur = nameRet.AttributListe.Egenskab[0].NavnStruktur;
                    return ret;
                }

            }
            return null;
        }
    }
}
