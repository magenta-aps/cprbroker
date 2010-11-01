using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Engine;
using CPRBroker.Schemas;
using System.Linq.Expressions;


namespace CPRBroker.Providers.DPR
{
    public partial class DprDatabaseDataProvider : ClientDataProvider, IPartSearchDataProvider, IPartReadDataProvider
    {
        [Obsolete]
        protected override string TestResponse
        {
            get
            {
                return "11471926741444Rkzmqvyjtyt                             Jpgdu                                                                               Sgk S?wuscdsxnbg                        579D485671    3744Btjaoxuwbsoxr       2005197001";
            }
        }

        #region IPartSearchDataProvider Members

        public PersonIdentifier[] Search(CPRBroker.Schemas.Part.PersonSearchCriteria searchCriteria, DateTime? effectDate, out CPRBroker.Schemas.QualityLevel? ql)
        {
            // TODO: Add DPR search implementation
            throw new NotImplementedException();
        }

        #endregion

        #region IPartReadDataProvider Members

        public CPRBroker.Schemas.Part.PersonRegistration Read(PersonIdentifier uuid, DateTime? effectDate, out QualityLevel? ql)
        {
            CPRBroker.Schemas.Part.PersonRegistration ret = null;
            EnsurePersonDataExists(uuid.CprNumber);
            using (var dataContext = new DPRDataContext(this.DatabaseObject.ConnectionString))
            {
                var db =
                (
                    from personInfo in PersonInfo.PersonInfoExpression.Compile()(effectDate.Value, dataContext)
                    where personInfo.PersonName.PNR == Decimal.Parse(uuid.CprNumber) && personInfo.PersonTotal.DateOfBirth == Utilities.DecimalFromDate(uuid.Birthdate)
                    select personInfo
                ).FirstOrDefault();
                if (db != null)
                {
                    ret = db.ToPersonRegistration(effectDate, dataContext);
                }
            }
            ql = QualityLevel.DataProvider;
            return ret;
        }

        public CPRBroker.Schemas.Part.PersonRegistration[] List(PersonIdentifier[] uuids, DateTime? effectDate, out QualityLevel? ql)
        {
            // TODO: Add DPR List implementation after Read implementation is OK
            throw new NotImplementedException();
        }

        #endregion
    }
}
