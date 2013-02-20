using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatchClient
{
    public class Read : PartClient
    {
        public override string[] LoadCprNumbers()
        {
            return base.LoadCprNumbersOneByOne();
        }
        public override void ProcessPerson(string cprNumberOrUuid)
        {
            var partService = new BatchClient.Part.Part();
            partService.Url = this.PartServiceUrl;
            partService.ApplicationHeaderValue = new BatchClient.Part.ApplicationHeader() { ApplicationToken = this.ApplicationToken, UserToken = this.UserToken };

            string uuid;
            if (CprBroker.Utilities.Strings.IsGuid(cprNumberOrUuid))
            {
                uuid = cprNumberOrUuid;
            }
            else
            {
                var getUuidResult = partService.GetUuid(cprNumberOrUuid);
                ValidateResult(cprNumberOrUuid, "GetUuid", getUuidResult.StandardRetur);
                uuid = getUuidResult.UUID;
            }
            var request = new BatchClient.Part.LaesInputType()
            {
                UUID = uuid
            };
            var readResult = partService.Read(request);
            ValidateResult(cprNumberOrUuid, "Read", readResult.StandardRetur);
            Console.WriteLine(partService.QualityHeaderValue.QualityLevel);
        }
    }
}
