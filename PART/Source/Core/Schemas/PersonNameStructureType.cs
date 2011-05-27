/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    /// <summary>
    /// Extents PersonNameStructureType by adding some utility functions
    /// </summary>
    public partial class PersonNameStructureType
    {
        public PersonNameStructureType()
        {

        }

        /// <summary>
        /// Creates a person name fromDate an array containing one or more of first, middle and last names
        /// </summary>
        /// <param name="namesArray">String array containing the names, possibly containing commas</param>
        public PersonNameStructureType(params string[] namesArray)
        {
            string fullName = string.Join(" ", namesArray);
            int commaIndex = fullName.IndexOf(',');
            if (commaIndex != -1)
            {
                fullName = fullName.Substring(commaIndex + 1) + " " + fullName.Substring(0, commaIndex);
            }
            namesArray = fullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Initialize(namesArray);
        }

        /// <summary>
        /// Initializes the object fromDate an array of names (no commas)
        /// </summary>
        /// <param name="namesArray">Array of names</param>
        private void Initialize(params string[] namesArray)
        {
            List<string> names = new List<string>(namesArray);
            if (names.Count >= 1)
            {
                this.PersonGivenName = names[0];
            }
            if (names.Count >= 2)
            {
                this.PersonSurnameName = names[names.Count - 1];
            }
            if (names.Count >= 3)
            {
                names.RemoveAt(0);
                names.RemoveAt(names.Count - 1);
                this.PersonMiddleName = string.Join(" ", names.ToArray());
            }
        }

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(this.PersonGivenName)
                    && string.IsNullOrEmpty(this.PersonMiddleName)
                    && string.IsNullOrEmpty(this.PersonSurnameName)
                    ;
            }
        }

        /// <summary>
        /// Converts the object to a full name string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string[] arr = new string[] 
            { 
                PersonGivenName, 
                PersonMiddleName, 
                PersonSurnameName 
            };

            arr = arr
                .AsQueryable()
                .Where(
                    (s) =>
                        !string.IsNullOrEmpty(s)
                    )
                .ToArray();

            return string.Join(" ", arr);
        }

        public string ToMiddleAndLastNameString()
        {
            string[] arr = new string[] { PersonMiddleName, PersonSurnameName };
            arr = arr
            .AsQueryable()
            .Where(
                (s) =>
                    !string.IsNullOrEmpty(s)
                )
            .ToArray();
            return string.Join(" ", arr);
        }

    }
}
