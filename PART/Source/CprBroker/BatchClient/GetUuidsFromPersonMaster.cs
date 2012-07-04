using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Utilities.ConsoleApps;

namespace BatchClient
{
    class GetUuidsFromPersonMaster : GetUuids
    {
        public override string[] LoadCprNumbers()
        {
            var baseRet = base.LoadCprNumbers();
            int batchSize = 500;
            var ret = new List<string>();
            var myRet = new List<string>(batchSize);
            for (int i = 0; i < baseRet.Length; i++)
            {
                myRet.Add(baseRet[i]);
                if (myRet.Count == batchSize || i == baseRet.Length - 1)
                {
                    ret.Add(string.Join(",", myRet.ToArray()));
                    myRet.Clear();
                }
            }
            return ret.ToArray();
        }

        int Succeeded = 0;
        int Failed = 0;


        public override void ProcessPerson(string joinedPnrBatch)
        {
            WSHttpBinding binding = new WSHttpBinding();

            var identity = new SpnEndpointIdentity(PersonMasterSpnName);
            EndpointAddress endPointAddress = new EndpointAddress(new Uri(PersonMasterUrl + "/PersonMasterService12"), identity);
            PersonMaster.BasicOpClient client = new PersonMaster.BasicOpClient(binding, endPointAddress);
            string[] pnrs = joinedPnrBatch.Split(',');
            string aux = "";
            var result = client.GetObjectIDsFromCprArray("BacthClient", pnrs, ref aux);
            Succeeded += result.Where(res => res.HasValue).Count();
            Failed += result.Where(res => !res.HasValue).Count();
            Console.WriteLine(string.Format("Batch finished: Size={0}, Succeeded So far={1}; Failed So far={2}", pnrs.Length, Succeeded, Failed));
        }
    }
}
