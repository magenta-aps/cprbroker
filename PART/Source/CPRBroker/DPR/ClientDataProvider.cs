using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Schemas;
using CprBroker.Schemas.Util;
using System.Linq.Expressions;

namespace CprBroker.Providers.DPR
{
    /// <summary>
    /// Implemenst a DPR data provider that uses TCP to get very basic data and DPR database for more detailed data
    /// </summary>
    public partial class ClientDataProvider : BaseProvider, IPersonNameAndAddressDataProvider, IPersonBasicDataProvider, IPersonFullDataProvider, IPersonChildrenDataProvider, IPersonRelationsDataProvider
    {
        #region Private methods
        /// <summary>
        /// Ensures that the DPR database contains the given person
        /// </summary>
        /// <param name="cprNumber"></param>
        protected void EnsurePersonDataExists(string cprNumber)
        {
            // TODO: include BirthDate in the search
            decimal cprNum = Convert.ToDecimal(cprNumber);
            using (DPRDataContext dataContext = new DPRDataContext(DatabaseObject.ConnectionString))
            {
                var exists = (from personName in dataContext.PersonNames
                              select personName.PNR).Contains(cprNum);

                if (!exists)
                {
                    GetPersonData(InquiryType.DataUpdatedAutomaticallyFromCpr, DetailType.ExtendedData, cprNumber);
                    // TODO: make sure that deleting the subscription is a good decision
                    GetPersonData(InquiryType.DeleteAutomaticDataUpdateFromCpr, DetailType.ExtendedData, cprNumber);
                }
            }
        }

        /// <summary>
        /// Retrieves a list of persons that match the given criteria
        /// </summary>
        /// <typeparam name="TDb">Type of object to query against</typeparam>
        /// <typeparam name="TResult">Type of result object</typeparam>
        /// <param name="context">Data context</param>
        /// <param name="listGetter">Method to be used to get the source list</param>
        /// <param name="isDbOjectNotReadyFunc">Used to filter the source list</param>
        /// <param name="cprNumberGetter">How to know the CPR number from a <paramref name="TDb"/></param>
        /// <param name="objectConverter">Method to be used to convert TDb into TResult</param>
        /// <returns></returns>
        private IQueryable<TResult> GetPersonList<TDb, TResult>(
            DPRDataContext context,
            Expression<Func<DPRDataContext, IQueryable<TDb>>> listGetter,
            Expression<Func<TDb, bool>> isDbOjectNotReadyFunc,
            Expression<Func<TDb, string>> cprNumberGetter,
            Expression<Func<TDb, TResult>> objectConverter)
        {
            var missingCprNumbers =
                listGetter.Compile()(context)
                .Where(isDbOjectNotReadyFunc)
                .Select(cprNumberGetter)
                .ToArray();

            foreach (string cprNumber in missingCprNumbers)
            {
                EnsurePersonDataExists(cprNumber);
            }

            var ret = listGetter.Compile()(context)
                      .Select(objectConverter);
            return ret;
        }

        /// <summary>
        /// Retrieves a list of a person's relations
        /// </summary>
        /// <typeparam name="TDbRelation">Type of database relation object</typeparam>
        /// <typeparam name="TOioRelation">Type of OIO relation object</typeparam>
        /// <param name="cprNumber">Person CPR number</param>
        /// <param name="dataContext">Data context</param>
        /// <param name="today">Today's date</param>
        /// <param name="relationGetter">Method that retrieves the relations</param>
        /// <param name="personCprNumberGetter">How to get CPR number from the main database person object</param>
        /// <param name="relatedCprNumberGetter">How to get the CPR number from the related database person object</param>
        /// <returns></returns>
        private IQueryable<TOioRelation> GetPersonRelations<TDbRelation, TOioRelation>(
            decimal cprNumber,
            DPRDataContext dataContext,
            DateTime today,
            Func<DPRDataContext, IQueryable<TDbRelation>> relationGetter,
            Func<TDbRelation, decimal> personCprNumberGetter,
            Func<TDbRelation, decimal> relatedCprNumberGetter
            ) where TOioRelation : BaseRelationshipType, new()
        {

            return GetPersonList<RelationInfo<TDbRelation>, TOioRelation>(
                dataContext,
                (context) =>
                        from dbRel in relationGetter(dataContext)
                        join personInfo in PersonInfo.PersonInfoExpression.Compile()(today, dataContext) on relatedCprNumberGetter(dbRel) equals personInfo.PersonName.PNR into pInfo
                        where personCprNumberGetter(dbRel) == cprNumber
                        select new RelationInfo<TDbRelation>()
                        {
                            RelationObject = dbRel,
                            RelatedPersonInfo = pInfo.SingleOrDefault()
                        },
                    (rel) => rel.RelatedPersonInfo == null,
                    (rel) => relatedCprNumberGetter(rel.RelationObject).ToString(),
                    (rel) =>
                        new TOioRelation()
                        {
                            //RelationStartDate=,
                            //RelationEndDate=,
                            SimpleCPRPerson = rel.RelatedPersonInfo.PersonName.ToSimpleCprPerson()
                        }
            );
        }

        /// <summary>
        /// Gets a list of parent/child relationships for a person
        /// </summary>
        /// <typeparam name="TResult">OIO relation type</typeparam>
        /// <param name="cprNum">Person CPR number</param>
        /// <param name="dataContext">Data context</param>
        /// <param name="today">Today's date</param>
        /// <param name="getChildren">True to get children, false to get parents</param>
        /// <returns></returns>
        private IQueryable<TResult> GetParentOrChild<TResult>(decimal cprNum, DPRDataContext dataContext, DateTime today, bool getChildren) where TResult : BaseRelationshipType, new()
        {
            return GetPersonRelations<Child, TResult>(
                cprNum,
                dataContext,
                today,
                (context) => context.Childs,
                (child) => getChildren ? child.ParentPNR : child.ChildPNR.Value,
                (child) => getChildren ? child.ChildPNR.Value : child.ParentPNR
            );
        }

        /// <summary>
        /// Creates an OIO relation object from a given PersonName
        /// </summary>
        /// <typeparam name="TOioRelation">Type of OIO relationship</typeparam>
        /// <param name="relatedPersonName">Name of related person</param>
        /// <returns></returns>
        private TOioRelation CreateRelationship<TOioRelation>(PersonName relatedPersonName) where TOioRelation : BaseRelationshipType, new()
        {
            if (relatedPersonName != null)
            {
                var ret = new TOioRelation()
                {
                    SimpleCPRPerson = relatedPersonName.ToSimpleCprPerson()
                };
                return ret;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves a list containing person children
        /// </summary>
        /// <param name="context">Data context</param>
        /// <param name="cprNumber">Person CPR number</param>
        /// <returns></returns>
        private ChildRelationshipType[] GetPersonChildren(DPRDataContext context, decimal cprNumber)
        {
            // Get list of children
            var all = (from child in context.Childs
                       join pn in PersonName.ActivePersonsExpression.Compile()(context) on child.ChildPNR equals pn.PNR into childPersonName
                       where child.ParentPNR == cprNumber
                       select new Relation<Child, ChildRelationshipType>()
                       {
                           DbRelation = child,
                           OioRelation = CreateRelationship<ChildRelationshipType>(childPersonName.SingleOrDefault())
                       }
                      ).ToArray();

            // Force DPR to load data of missing children
            var missing = from ch in all
                          where ch.OioRelation == null
                          select ch;

            if (missing.Count() > 0)
            {
                foreach (var ch in missing)
                {
                    EnsurePersonDataExists(ch.DbRelation.ChildPNR.ToString());
                    var personName = from pn in context.PersonNames
                                     where pn.PNR == ch.DbRelation.ChildPNR
                                     select pn;
                    ch.OioRelation = CreateRelationship<ChildRelationshipType>(personName.SingleOrDefault());
                }
            }

            // return
            return (from ch in all
                    select ch.OioRelation
                    ).ToArray();
        }

        /// <summary>
        /// Retrieves a list of a person's parents
        /// </summary>
        /// <param name="context">Data context</param>
        /// <param name="cprNumber">Person CPR number</param>
        /// <returns></returns>
        private ParentRelationshipType[] GetPersonParents(DPRDataContext context, decimal cprNumber)
        {
            // Get list of parents
            var all = (from child in context.Childs
                       join pn in PersonName.ActivePersonsExpression.Compile()(context) on child.ParentPNR equals pn.PNR into childPersonName
                       where child.ChildPNR == cprNumber
                       select new Relation<Child, ParentRelationshipType>()
                       {
                           DbRelation = child,
                           OioRelation = CreateRelationship<ParentRelationshipType>(childPersonName.SingleOrDefault())
                       }
                      ).ToArray();

            // Force DPR to load data of missing parents
            var missing = from ch in all
                          where ch.OioRelation == null
                          select ch;

            if (missing.Count() > 0)
            {
                foreach (var ch in missing)
                {
                    EnsurePersonDataExists(ch.DbRelation.ChildPNR.ToString());
                    var personName = from pn in context.PersonNames
                                     where pn.PNR == ch.DbRelation.ChildPNR
                                     select pn;
                    ch.OioRelation = CreateRelationship<ParentRelationshipType>(personName.SingleOrDefault());
                }
            }

            // return
            return (from ch in all
                    select ch.OioRelation
                    ).ToArray();
        }

        /// <summary>
        /// Retrievs a list containing a person's Spouse(s)
        /// </summary>
        /// <param name="context">Data context</param>
        /// <param name="cprNumber">Person CPR number</param>
        /// <returns></returns>
        private MaritalRelationshipType[] GetPersonSpouses(DPRDataContext context, decimal cprNumber)
        {
            // Get list of spouses
            var all = (from civil in context.CivilStatus
                       join pn in PersonName.ActivePersonsExpression.Compile()(context) on civil.SpousePNR equals pn.PNR into spousePersonName
                       where civil.PNR == cprNumber
                       && civil.SpousePNR != null
                           // Do not include married row if there is a divorce row that started the same time of this end time
                       && (
                            civil.MaritalStatus.Value != Constants.MaritalStatus.Married
                            ||
                            (from civ2 in context.CivilStatus where civ2.PNR == cprNumber && civ2.MaritalStatusDate == civil.MaritalEndDate select civ2).Count() == 0
                        )
                       orderby civil.MaritalStatusDate
                       select new Relation<CivilStatus, MaritalRelationshipType>()
                       {
                           DbRelation = civil,
                           OioRelation = CreateRelationship<MaritalRelationshipType>(spousePersonName.SingleOrDefault())
                       }
                      ).ToArray();

            // Force DPR to load missing spouse persons
            var missing = from ch in all
                          where ch.OioRelation == null
                          select ch;

            if (missing.Count() > 0)
            {
                foreach (var ch in missing)
                {
                    EnsurePersonDataExists(ch.DbRelation.SpousePNR.ToString());
                    var personName = from pn in context.PersonNames
                                     where pn.PNR == ch.DbRelation.SpousePNR
                                     select pn;
                    ch.OioRelation = CreateRelationship<MaritalRelationshipType>(personName.SingleOrDefault());
                }
            }

            // Fill dates
            foreach (var rel in all)
            {
                rel.OioRelation.RelationStartDate = Utilities.DateFromDecimal(rel.DbRelation.MaritalStatusDate);
                rel.OioRelation.RelationEndDate = Utilities.DateFromDecimal(rel.DbRelation.MaritalEndDate);
            }

            // return
            return (from sp in all
                    select sp.OioRelation
                    ).ToArray();
        }

        #endregion

        #region IPersonNameAndAddressDataProvider Members

        public PersonNameAndAddressStructureType GetCitizenNameAndAddress(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
        {
            // Basically use TCP to get person's data and then parse the response to create a return object
            EnsurePersonDataExists(cprNumber);

            string response = GetPersonData(InquiryType.DataNotUpdatedAutomatically, DetailType.MasterData, cprNumber);
            StreamStringReader rd = new StreamStringReader(response);

            rd.ReadNext(4);

            PersonNameAndAddressStructureType ret = new PersonNameAndAddressStructureType();

            ret.SimpleCPRPerson = new SimpleCPRPersonType();
            ret.SimpleCPRPerson.PersonCivilRegistrationIdentifier = rd.ReadNext(10);


            var lastName = rd.ReadNext(40).Trim();
            var foreName = rd.ReadNext(50).Trim();
            ret.SimpleCPRPerson.PersonNameStructure = new PersonNameStructureType(foreName, lastName);

            Schemas.Util.Address address = new CprBroker.Schemas.Util.Address();

            address[AddressField.CareOfName] = rd.ReadNext(34).Trim();
            address[AddressField.StreetName] = rd.ReadNext(40).Trim();
            address[AddressField.HouseNumber] = rd.ReadNext(4).Trim();
            address[AddressField.Floor] = rd.ReadNext(2).Trim();
            address[AddressField.Door] = rd.ReadNext(4).Trim();
            address[AddressField.Building] = rd.ReadNext(4).Trim();
            address[AddressField.PostCode] = rd.ReadNext(4).Trim();
            address[AddressField.PostDistrictName] = rd.ReadNext(20).Trim();

            string strProtectionDate = rd.ReadNext(8).Trim();
            if (!string.IsNullOrEmpty(strProtectionDate) && !strProtectionDate.Equals(new string('0', strProtectionDate.Length)))
            {
                StreamStringReader rd2 = new StreamStringReader(strProtectionDate);
                // This parse operation is ddMMyyyy rather than yyyyMMdd, so Utilities.DecimalToDateTime is not used
                address.ProtectionDate = DateTime.ParseExact(strProtectionDate, "ddMMyyyy", null);
            }

            string civilStatus = rd.ReadNext(2);
            ret.Item = address.ToOioAddress(Schemas.Util.Enums.ToCivilRegistrationStatus(civilStatus));
            qualityLevel = QualityLevel.DataProvider;
            return ret;
        }

        #endregion

        #region IPersonBasicDataProvider Members
        public PersonBasicStructureType GetCitizenBasic(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
        {
            this.EnsurePersonDataExists(cprNumber);
            using (DPRDataContext dataContext = new DPRDataContext(DatabaseObject.ConnectionString))
            {
                DateTime today = DateTime.Today;
                decimal cprNum = Convert.ToDecimal(cprNumber);

                // Load from database
                PersonInfo personInfo = (
                    from innerPersonInfo in PersonInfo.PersonInfoExpression.Compile()(today, dataContext)
                    where innerPersonInfo.PersonName.PNR == cprNum
                    select innerPersonInfo
                    ).SingleOrDefault();

                // Fill return object
                PersonBasicStructureType ret = null;
                if (personInfo != null)
                {
                    ret = new PersonBasicStructureType();

                    // Regular CPR Person
                    ret.RegularCPRPerson = personInfo.ToRegularCprPerson();

                    // Marital status
                    ret.MaritalStatusCode = personInfo.PersonTotal.MaritalStatusCodeType;

                    // Nationality
                    ret.PersonNationalityCode = CprBroker.DAL.Country.GetCountryAlpha2CodeByDanishName(personInfo.PersonTotal.Nationality);

                    // Data from GetCitizenNameAndAddress

                    // Address
                    ret.Item = personInfo.PersonTotal.ToOioAddress(ret.RegularCPRPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusCode, personInfo.Street, personInfo.ContactAddress);
                    //ret.AddressIdentifierCode = personNameAndAddress.AddressIdentifierCode;
                    ret.AddressIdentifierCodeSpecified = false;

                    // Death date 
                    if (ret.RegularCPRPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusCode == PersonCivilRegistrationStatusCodeType.Item90)
                    {
                        ret.PersonDeathDateStructure = new PersonDeathDateStructureType()
                        {
                            PersonDeathDate = ret.RegularCPRPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusStartDate,
                            PersonDeathDateUncertaintyIndicator = false,
                        };
                    }
                }
                qualityLevel = QualityLevel.DataProvider;
                return ret;
            }
        }
        #endregion

        #region IPersonFullDataProvider Members
        public PersonFullStructureType GetCitizenFull(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
        {
            // Ensure person's data then read database
            this.EnsurePersonDataExists(cprNumber);
            using (DPRDataContext dataContext = new DPRDataContext(DatabaseObject.ConnectionString))
            {
                DateTime today = DateTime.Today;
                decimal cprNum = Convert.ToDecimal(cprNumber);

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
                    from innerPersonInfo in PersonInfo.PersonInfoExpression.Compile()(today, dataContext)
                    where innerPersonInfo.PersonName.PNR == cprNum
                    select new
                    {
                        PersonInfo = innerPersonInfo,
                        NumberOfChildren = (from child in dataContext.Childs where child.ParentPNR == innerPersonInfo.PersonName.PNR select child).Count(),
                        CivilStatus = civilStatusExpression.Compile()(innerPersonInfo.PersonName.PNR)
                    }
                    ).FirstOrDefault();

                PersonFullStructureType ret = null;

                if (personInfo != null)
                {
                    // Fill return object
                    ret = new PersonFullStructureType();

                    ret.NumberOfChildren = personInfo.NumberOfChildren;
                    ret.PersonNationalityCode = DAL.Country.GetCountryAlpha2CodeByDanishName(personInfo.PersonInfo.PersonTotal.Nationality);

                    ret.RegularCPRPerson = personInfo.PersonInfo.ToRegularCprPerson();

                    // Marital status and spouse name
                    if (personInfo.CivilStatus == null)
                    {
                        ret.MaritalStatusCode = MaritalStatusCodeType.unmarried;
                    }
                    else
                    {
                        ret.MaritalStatusCode = personInfo.CivilStatus.CurrentMaritalStatusCode;

                        if (!string.IsNullOrEmpty(personInfo.CivilStatus.SpouseName))
                        {
                            ret.SpouseName = personInfo.CivilStatus.SpouseName;
                        }
                        else
                        {
                            EnsurePersonDataExists(personInfo.CivilStatus.SpousePNR.ToString());
                            ret.SpouseName = (
                                from personName in dataContext.PersonNames
                                where personName.PNR == personInfo.CivilStatus.SpousePNR
                                select personName.ToSimpleCprPerson().PersonNameStructure.ToString()
                                ).FirstOrDefault();
                        }

                    }
                    // Data from GetCitizenNameAndAddress

                    // Address
                    ret.Item = personInfo.PersonInfo.PersonTotal.ToOioAddress(ret.RegularCPRPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusCode, personInfo.PersonInfo.Street, personInfo.PersonInfo.ContactAddress);
                    //ret.AddressIdentifierCode = personNameAndAddress.AddressIdentifierCode;
                    ret.AddressIdentifierCodeSpecified = false;

                    // Death date 
                    if (ret.RegularCPRPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusCode == PersonCivilRegistrationStatusCodeType.Item90)
                    {
                        ret.PersonDeathDateStructure = new PersonDeathDateStructureType()
                        {
                            PersonDeathDate = ret.RegularCPRPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusStartDate,
                            PersonDeathDateUncertaintyIndicator = false,
                        };
                    }

                }
                qualityLevel = QualityLevel.DataProvider;
                return ret;
            }
        }

        #endregion

        #region IPersonChildrenDataProvider Members

        public SimpleCPRPersonType[] GetCitizenChildren(string userToken, string appToken, string cprNumber, bool includeCustodies, out QualityLevel? qualityLevel)
        {
            EnsurePersonDataExists(cprNumber);

            using (DPRDataContext dataContext = new DPRDataContext(DatabaseObject.ConnectionString))
            {
                DateTime today = DateTime.Today;
                decimal cprNum = decimal.Parse(cprNumber);

                var retArr =
                    from rel in GetPersonChildren(dataContext, cprNum)
                    select rel.SimpleCPRPerson;

                qualityLevel = QualityLevel.DataProvider;
                return retArr
                    .OrderBy((p) => DAL.Person.PersonNumberToDate(p.PersonCivilRegistrationIdentifier))
                    .ToArray();
            }
        }

        #endregion

        #region IPersonRelationsDataProvider Members

        public PersonRelationsType GetCitizenRelations(string userToken, string appToken, string cprNumber, out QualityLevel? qualityLevel)
        {
            PersonRelationsType ret = new PersonRelationsType();
            EnsurePersonDataExists(cprNumber);

            using (DPRDataContext dataContext = new DPRDataContext(DatabaseObject.ConnectionString))
            {
                // Create return object and fill members
                DateTime today = DateTime.Today;
                decimal cprNum = decimal.Parse(cprNumber);

                ret.SimpleCPRPerson = (from personInfo in PersonInfo.PersonInfoExpression.Compile()(today, dataContext)
                                       where personInfo.PersonName.PNR == cprNum
                                       select personInfo.PersonName.ToSimpleCprPerson()
                                       ).SingleOrDefault();

                ret.Parents.AddRange(GetPersonParents(dataContext, cprNum).OrderBy((p) => DAL.Person.PersonNumberToDate(p.SimpleCPRPerson.PersonCivilRegistrationIdentifier)));
                ret.Children.AddRange(GetPersonChildren(dataContext, cprNum).OrderBy((p) => DAL.Person.PersonNumberToDate(p.SimpleCPRPerson.PersonCivilRegistrationIdentifier)));

                ret.Spouses.AddRange(GetPersonSpouses(dataContext, cprNum));

                qualityLevel = QualityLevel.DataProvider;
                return ret;
            }
        }
        #endregion
    }
}

