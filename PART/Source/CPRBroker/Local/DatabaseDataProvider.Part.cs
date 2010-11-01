using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using CPRBroker.Engine;
using CPRBroker.DAL;
using CPRBroker.Schemas;
using CPRBroker.Schemas.Util;
using CPRBroker.Engine.Local;

namespace CPRBroker.Providers.Local
{
    /// <summary>
    /// Handles implementation of data provider using the system's local database
    /// </summary>
    public partial class DatabaseDataProvider : IPartReadDataProvider, IPartSearchDataProvider
    {

        #region IPartSearchDataProvider Members

        public PersonIdentifier[] Search(CPRBroker.Schemas.Part.PersonSearchCriteria searchCriteria,DateTime? effectDate, out QualityLevel? ql)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IPartReadDataProvider Members

        public CPRBroker.Schemas.Part.PersonRegistration Read(PersonIdentifier uuid, DateTime? effectDate, out QualityLevel? ql)
        {
            throw new NotImplementedException();
        }

        public CPRBroker.Schemas.Part.PersonRegistration[] List(PersonIdentifier[] uuids, DateTime? effectDate, out QualityLevel? ql)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
