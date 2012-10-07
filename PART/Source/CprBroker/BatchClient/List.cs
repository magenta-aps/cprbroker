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
    class List : RefreshData
    {
        public override string[] LoadCprNumbers()
        {
            return LoadCprNumbersBatch();
        }

        int Succeeded = 0;
        int Failed = 0;


        public override void ProcessPerson(string joinedPnrBatch)
        {
            var partService = new BatchClient.Part.Part();
            partService.Url = this.PartServiceUrl;
            partService.ApplicationHeaderValue = new BatchClient.Part.ApplicationHeader() { ApplicationToken = this.ApplicationToken, UserToken = this.UserToken };

            string[] pnrs = joinedPnrBatch.Split(',');
            var uuids = new List<string>();
            foreach (var pnr in pnrs)
            {
                var getUuidResult = partService.GetUuid(pnr);
                ValidateResult(pnr, "GetUuid", getUuidResult.StandardRetur);
                uuids.Add(getUuidResult.UUID);
            }

            var request = new BatchClient.Part.ListInputType()
            {
                UUID = uuids.ToArray()
            };
            var listResult = partService.List(request);
            ValidateResult(joinedPnrBatch, "List", listResult.StandardRetur);

            Succeeded += listResult.LaesResultat.Where(res => res != null).Count();
            Failed += listResult.LaesResultat.Where(res => res == null).Count();
            Console.WriteLine(string.Format("Batch finished: Size={0}, Succeeded So far={1}; Failed So far={2}", pnrs.Length, Succeeded, Failed));
        }
    }
}
