using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities.ConsoleApps;
using System.Data.Linq;

namespace BatchClient
{
    [Obsolete("only used for testing")]
    class CallSearch : PartClient
    {
        public override string[] LoadCprNumbers()
        {
            return Utilities.LoadCprNumbersOneByOne(this.SourceFile);
        }

        public override void ProcessPerson(string pnr)
        {
            using (var dataContext = new DataContext(BrokerConnectionString))
            {
                var partService = new BatchClient.Part.Part();
                partService.Url = this.PartServiceUrl;
                partService.ApplicationHeaderValue = new BatchClient.Part.ApplicationHeader() { ApplicationToken = this.ApplicationToken, UserToken = this.UserToken };
                partService.Credentials = System.Net.CredentialCache.DefaultCredentials;

                var name = dataContext.ExecuteQuery<string>("SELECT PersonGivenName FROM PersonSearchCache WHERE UUID={0}", pnr).Single();
                
                var ret = partService.Search(new Part.SoegInputType1()
                {
                    SoegObjekt = new Part.SoegObjektType()
                    {
                        SoegAttributListe = new Part.SoegAttributListeType()
                        {
                            SoegEgenskab = new Part.SoegEgenskabType[]
                {
                    new Part.SoegEgenskabType(){ NavnStruktur = new Part.NavnStrukturType(){ PersonNameStructure = new Part.PersonNameStructureType(){ PersonGivenName = name}
                    }}}
                        }
                    }
                });
                ValidateResult(pnr, "Search", ret.StandardRetur);

                if (ret.Idliste.Length != 1)
                {
                    throw new Exception(string.Format("Unexpected length for result, expected <{0}>, found <{1}>", 1, ret.Idliste.Length));
                }
                if (new Guid(ret.Idliste[0]) != new Guid(pnr))
                {
                    throw new Exception(string.Format("Unexpected UUID in result, expected <{0}>, found <{1}>", pnr, ret.Idliste[0]));
                }
            }
        }
    }
}
