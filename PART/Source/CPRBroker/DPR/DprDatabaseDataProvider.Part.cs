using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Engine;
using CPRBroker.Schemas;
using CPRBroker.Schemas.Part;
using CPRBroker.Schemas.Util;
using System.Linq.Expressions;


namespace CPRBroker.Providers.DPR
{
    public partial class DprDatabaseDataProvider : ClientDataProvider, IPartSearchDataProvider, IPartReadDataProvider
    {
        public static readonly Guid ActorId = new Guid("{4A953CF9-B4C1-4ce9-BF09-2BF655DC61C7}");

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
            using (var dataContext = new DPRDataContext(this.DatabaseObject.ConnectionString))
            {
                var expr = from pi in PersonInfo.PersonInfoExpression.Compile()(effectDate.Value, dataContext)
                           select new PersonIdentifier()
                           {
                               CprNumber = pi.PersonTotal.PNR.ToString("D2"),
                           };

                var pred = PredicateBuilder.False<PersonInfo>();
                if (searchCriteria.BirthDate.HasValue)
                {
                    pred = pred.And((pt) => pt.PersonTotal.DateOfBirth == Utilities.DecimalFromDate(searchCriteria.BirthDate.Value));
                }
                if (!string.IsNullOrEmpty(searchCriteria.CprNumber))
                {
                    this.EnsurePersonDataExists(searchCriteria.CprNumber);
                    pred = pred.And((pt) => pt.PersonTotal.PNR == Decimal.Parse(searchCriteria.CprNumber));
                }
                if (searchCriteria.Gender.HasValue)
                {
                    pred = pred.And((pt) => pt.PersonTotal.Sex == Utilities.CharFromGender(searchCriteria.Gender));
                }
                if (!searchCriteria.Name.IsEmpty)
                {
                    if (!string.IsNullOrEmpty(searchCriteria.Name.PersonGivenName))
                    {
                        pred = pred.And((pt) => pt.PersonName.FirstName == searchCriteria.Name.PersonGivenName);
                    }

                    //TODO: ensure that middle name is included with last name in the database (rather than with first name)
                    if (!string.IsNullOrEmpty(searchCriteria.Name.ToMiddleAndLastNameString()))
                    {
                        pred = pred.And((pt) => pt.PersonName.LastName == searchCriteria.Name.ToMiddleAndLastNameString());
                    }

                };
                if (!string.IsNullOrEmpty(searchCriteria.NationalityCountryCode))
                {
                    pred = pred.And((pt) => CPRBroker.DAL.Country.GetCountryEnglishAndDanishNamesByAlpha2Code(searchCriteria.NationalityCountryCode).Contains(pt.PersonTotal.Nationality));
                }

                var ids = expr.ToArray();
                CPRBroker.DAL.Part.PersonMapping.AssignGuids(ids);
                ql = QualityLevel.DataProvider;
                return ids;
            }
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
                    where personInfo.PersonName.PNR == Decimal.Parse(uuid.CprNumber)
                    select personInfo
                ).FirstOrDefault();
                if (db != null)
                {
                    ret = db.ToPersonRegistration(effectDate, dataContext);
                    ret.ActorId = ActorId;
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
