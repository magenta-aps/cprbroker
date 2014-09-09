using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using CprBroker.Engine;
using CprBroker.Data.DataProviders;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;
using CprBroker.Utilities.Config;

namespace CprBroker.DBR
{
    public partial class ClassicRequestType
    {
        public override DiversionResponse Process(string dprConnectionString)
        {
            if (this.LargeData == Providers.DPR.DetailType.ExtendedData && this.Type == Providers.DPR.InquiryType.DataUpdatedAutomaticallyFromCpr)
            {
                var providers = LoadDataProviders();

                var response = providers
                    .Select(p => p.GetPerson(this.PNR))
                    .First(p => p != null);

                if (response != null)
                {
                    using (var conn = new SqlConnection(dprConnectionString))
                    {
                        using (var dataContext = new DPRDataContext(conn))
                        {
                            CprConverter.AppendPerson(response, dataContext);

                            conn.Open();
                            using (var trans = conn.BeginTransaction())
                            {
                                dataContext.Transaction = trans;
                                CprConverter.DeletePersonRecords(this.PNR, dataContext);
                                conn.BulkInsertChanges(dataContext.GetChangeSet().Inserts, trans);
                                trans.Commit();
                            }
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

        public virtual IEnumerable<ICprDirectPersonDataProvider> LoadDataProviders()
        {
            DataProvidersConfigurationSection section = ConfigManager.Current.DataProvidersSection;
            DataProvider[] dbProviders = DataProviderManager.ReadDatabaseDataProviders();

            var providers = DataProviderManager
                .GetDataProviderList(section, dbProviders, typeof(ICprDirectPersonDataProvider), Schemas.SourceUsageOrder.LocalThenExternal)
                .Select(p => p as ICprDirectPersonDataProvider);
            return providers;
        }
    }
}
