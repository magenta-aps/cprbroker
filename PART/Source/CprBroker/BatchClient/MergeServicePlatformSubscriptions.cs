using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.ServicePlatform;
using CprBroker.Engine;
using CprBroker.Utilities.ConsoleApps;
using System.IO;
using System.Text.RegularExpressions;

namespace BatchClient
{
    public class MergeServicePlatformSubscriptions : ConsoleEnvironment
    {
        ServicePlatformDataProvider prov;
        Dictionary<string, string[]> existing;
        string[] pnrFiles;

        public override void Initialize()
        {
            base.Initialize();

            if (!string.IsNullOrEmpty(this.BrokerConnectionString))
                Utilities.UpdateConnectionString(this.BrokerConnectionString);
            var token = string.IsNullOrEmpty(this.ApplicationToken) ? CprBroker.Utilities.Constants.BaseApplicationToken.ToString() : this.ApplicationToken;

            CprBroker.Engine.BrokerContext.Initialize(token, "");

            var sourceFiles = this.SourceFile.Split(",;".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            prov = CprBroker.Tests.ServicePlatform.ServicePlatformDataProviderFactory.Create(sourceFiles.First());
            pnrFiles = sourceFiles.Skip(1).ToArray();

            existing = prov.SubscriptionFields
                .Select(f => new { f, values = prov.GetSubscriptions(f) })
                .ToDictionary(f => f.f, f => f.values);
        }

        public override string[] LoadCprNumbers()
        {
            return pnrFiles
                .SelectMany(f => File.ReadAllLines(f))
                .Where(v => !string.IsNullOrEmpty(v))
                .Select(v => v.Length <= 3 ? v.PadLeft(4,'0') :
                    Regex.IsMatch(v, @"\A\d{9,10}\Z") ? v.PadLeft(10, '0') :
                    v
                ).ToArray();
        }

        public override void ProcessPerson(string pnr)
        {
            var field = pnr.Length == 4 ? Constants.SubscriptionFields.MunicipalityCode :
                pnr.Length == 10 ? Constants.SubscriptionFields.PNR :
                Constants.SubscriptionFields.ChangeCode;

            if (existing[field].Contains(pnr))
            {
                Log("Subscription exists, skipping");
            }
            else
            {
                Log("Subscriping");
                ServicePlatformDataProvider.ReturnCodePNR retCode;
                if (prov.PutSubscription(field, pnr, out retCode))
                { }
                else
                {
                    throw new Exception(string.Format("Failed, reason = {0}", retCode));
                }
            }
        }
    }
}
