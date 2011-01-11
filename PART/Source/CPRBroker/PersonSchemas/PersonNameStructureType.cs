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
        /// Creates a person name from an array containing one or more of first, middle and last names
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
        /// Initializes the object from an array of names (no commas)
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
