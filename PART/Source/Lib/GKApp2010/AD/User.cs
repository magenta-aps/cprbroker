//------------------------------------------------------------------------------
// Gentofte Kommune application framework 2011.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;

namespace GKApp2010.AD
{
    // ================================================================================
    /// <summary>
    /// Implements a number of methods for querying and manipulation of user objects in AD (Active Directory).
    /// All methods will be executed againt AD in the security context of the current thread identity.
    /// Also it's assumed, that methods will be executed againt the current domain.
    /// </summary>
    public class User
    {
        // -----------------------------------------------------------------------------
        public static bool IsAccountEnabled(string userId)
        {
            bool? enabled = false;

            PrincipalContext _principalContext = new PrincipalContext(ContextType.Domain, Domain.GetCurrentDomain().Name);
            UserPrincipal userP = UserPrincipal.FindByIdentity(_principalContext, IdentityType.SamAccountName, userId.ToUpper());

            if (userP != null)
            {
                enabled = userP.Enabled;
            }

            return (enabled.HasValue) ? enabled.Value : false;
        }
    }
}
