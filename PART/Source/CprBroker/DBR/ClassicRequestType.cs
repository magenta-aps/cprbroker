using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Data.DataProviders;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;

namespace CprBroker.DBR
{
    public partial class ClassicRequestType
    {
        public override DiversionResponse Process(string dprConnectionString)
        {
            if (this.LargeData == Providers.DPR.DetailType.ExtendedData && this.Type == Providers.DPR.InquiryType.DataUpdatedAutomaticallyFromCpr)
            {
                DataProvidersConfigurationSection section = Config.ConfigManager.Current.DataProvidersSection;
                DataProvider[] dbProviders = DataProviderManager.ReadDatabaseDataProviders();

                var providers = DataProviderManager
                    .GetDataProviderList(section, dbProviders, typeof(ICprDirectPersonDataProvider), Schemas.SourceUsageOrder.LocalThenExternal)
                    .Select(p => p as ICprDirectPersonDataProvider);

                var response = providers
                    .Select(p => p.GetPerson(this.PNR))
                    .First(p => p != null);

                if (response != null)
                {
                    using (var dataContext = new DPRDataContext(dprConnectionString))
                    {
                        CprConverter.DeletePersonRecords(this.PNR, dataContext);
                        CprConverter.AppendPerson(response, dataContext);
                        dataContext.SubmitChanges();
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
                }
                else
                {
                    // TODO: person not found, return error
                    throw new NotImplementedException();
                }
            }
            else
            {
                // TODO: unimplemented mode of operation
                throw new NotImplementedException();
            }
        }
    }
}
