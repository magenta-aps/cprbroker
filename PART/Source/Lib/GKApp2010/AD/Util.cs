//------------------------------------------------------------------------------
// Gentofte Kommune application framework 2011.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

using System;
using System.Linq;

namespace GKApp2010.AD
{
    // ================================================================================
    public static class Util
    {
        // -----------------------------------------------------------------------------
        /// <summary>
        /// Convert a AD OU path in canonical form to it's DN representation.
        /// 
        /// Example:
        /// local.acme.com/Users/Sales/Domestic ==> OU=Domestic,OU=Sales,OU=Users,DC=local,DC=acme,DC=com
        /// 
        /// </summary>
        /// <param name="canonicalPath">The OU represented in canonical form</param>
        /// <returns>The distinquished name (DN) of the OU</returns>
        public static string GetDistinguishedOUNameFromCanonical(string canonicalPath)
        {
            string dnOUPath = "";

            // local.acme.com/Users/Sales/Domestic - strip domain name (assume a slash separates DC and OU parts)
            int i = canonicalPath.IndexOf("/");

            string domName = canonicalPath.Substring(0, i);
            canonicalPath = canonicalPath.Substring(i);

            char[] delimDot = { '.' };
            string[] DCparts = domName.Split(delimDot, StringSplitOptions.RemoveEmptyEntries);

            char[] delimSlash = { '/' };
            string[] OUparts = canonicalPath.Split(delimSlash, StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in OUparts.Reverse())
                dnOUPath += "OU=" + s + ",";

            foreach (string s in DCparts)
                dnOUPath += "DC=" + s + ",";

            // Remove last comma
            dnOUPath = dnOUPath.TrimEnd(new char[] { ',' });
            //dnOUPath = dnOUPath.Substring(0, dnOUPath.Length - 1);

            return dnOUPath;
        }

    }
}
