using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Data.DataProviders;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.DBR
{
    public partial class ClassicRequestType
    {
        public override DiversionResponse Process()
        {
            if (this.LargeData == Providers.DPR.DetailType.ExtendedData && this.Type == Providers.DPR.InquiryType.DataUpdatedAutomaticallyFromCpr)
            {
                DataProvidersConfigurationSection section = DataProvidersConfigurationSection.GetCurrent();
                DataProvider[] dbProviders = DataProviderManager.ReadDatabaseDataProviders();

                var providers = DataProviderManager
                    .GetDataProviderList(section, dbProviders, typeof(ICprDirectPersonDataProvider), Schemas.SourceUsageOrder.LocalThenExternal)
                    .Select(p => p as ICprDirectPersonDataProvider);

                var response = providers
                    .First(p => p.GetPerson(this.PNR) != null);

                if (response != null)
                {
                    var ret = new ClassicResponseType()
                    {
                        Type = this.Type,
                        LargeData = this.LargeData,
                        PNR = this.PNR,
                        ErrorNumber = "00",
                        Data = "Basen er opdateret"
                    };
                    return ret;
                }
                else
                {
                    // TODO: person not found, return error
                    return base.Process();
                }
            }
            else
            {
                // TODO: unimplemented mode of operation
                return base.Process();
            }
        }
    }
}
