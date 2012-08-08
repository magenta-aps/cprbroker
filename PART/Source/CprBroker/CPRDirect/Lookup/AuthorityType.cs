using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    /// <summary>
    /// Line wrapper for authority - P02680-001
    /// </summary>
    public partial class AuthorityType
    {
        /// <summary>
        /// Converts to the database object
        /// </summary>
        /// <returns></returns>
        public Authority ToAuthority()
        {
            return new Authority()
            {
                Address = this.Adressee,
                AddressLine1 = this.Address1,
                AddressLine2 = this.Address2,
                AddressLine3 = this.Address3,
                AddressLine4 = this.Address4,
                Alpha2CountryCode = this.Alpha2CountryCode,
                Alpha3CountryCode = this.Alpha3CountryCode,
                AuthorityCode = this.AuthorityCode,
                AuthorityGroup = this.AuthorityGroupCode.ToString(),
                AuthorityName = this.AuthorityName,
                AuthorityPhone = this.AuthorityPhone,
                AuthorityType = this.AuthorityTypeCode,
                Email = this.Email,
                EndDate = this.AuthorityEndDate,
                StartDate = this.AuthorityStartDate.Value,
                FullName = this.FullName,
                NumericCountryCode = this.NumericCountryCode,
                Telefax = this.Telefax,
                UpdateTime = this.UpdateTime.Value
            };
        }
    }
}
