using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class PersonName
    {
        public static NavnStruktur ToXmlType(PersonName db)
        {
            if (db != null)
            {
                return new NavnStruktur(db.FirstName, db.MiddleName, db.LastName);
            }
            return null;
        }

        public static PersonName FromXmlType(NavnStruktur oio)
        {
            if (oio != null)
            {
                return new PersonName()
                {
                    FirstName = oio.PersonGivenName,
                    MiddleName = oio.PersonMiddleName,
                    LastName = oio.PersonSurnameName
                };
            }
            return null;
        }
    }
}
