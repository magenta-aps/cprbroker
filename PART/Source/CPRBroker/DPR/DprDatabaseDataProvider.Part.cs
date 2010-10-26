using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Engine;
using CPRBroker.Schemas;
using System.Linq.Expressions;

namespace CPRBroker.Providers.DPR
{
    public partial class DprDatabaseDataProvider : BaseProvider, IPartSearchDataProvider
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

        public Guid[] Search(CPRBroker.Schemas.Part.PersonSearchCriteria searchCriteria, out CPRBroker.Schemas.QualityLevel? ql)
        {
            using (DPRDataContext dataContext = new DPRDataContext(DatabaseObject.ConnectionString))
            {
                DateTime? effectDate = DateTime.Today;
                DateTime? registrationDate = DateTime.Today;

                // Gets a person's civil status
                Expression<Func<decimal, CivilStatus>> civilStatusExpression =
                    (pnr) =>
                        (
                            from civilStatus in dataContext.CivilStatus
                            where civilStatus.PNR == pnr
                            && civilStatus.SpousePNR.HasValue
                            && civilStatus.SpousePNR.Value > 0
                            select civilStatus
                        )
                        .OrderByDescending((cs) => cs.MaritalStatusDate)
                        .FirstOrDefault();

                var personInfo = (
                    from innerPersonInfo in PersonInfo.PersonInfoExpression.Compile()(effectDate.Value, dataContext)
                    where 1 == 1 //innerPersonInfo.PersonName.PNR == cprNum
                    select new
                    {
                        PersonInfo = innerPersonInfo,
                        NumberOfChildren = (from child in dataContext.Childs where child.ParentPNR == innerPersonInfo.PersonName.PNR select child).Count(),
                        CivilStatus = civilStatusExpression.Compile()(innerPersonInfo.PersonName.PNR)
                    }
                    ).FirstOrDefault();
            }
            ql = QualityLevel.DataProvider;
            return null;
        }

        #endregion
    }
}
