using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;
using System.Data.Linq;

namespace CprBroker.Data.Part
{
    public partial class PersonState
    {
        public static CprBroker.Schemas.Part.TilstandListeType ToXmlType(PersonState db)
        {
            if (db != null)
            {
                return new TilstandListeType()
                {
                    CivilStatus = PersonCivilState.ToXmlType(db.PersonCivilState),
                    LivStatus = PersonLifeState.ToXmlType(db.PersonLifeState),
                    LokalUdvidelse = null
                };
            }
            return null;
        }

        public static PersonState FromXmlType(Schemas.Part.TilstandListeType partState)
        {
            if (partState != null)
            {
                return new PersonState()
                {
                    PersonCivilState = PersonCivilState.FromXmlType(partState.CivilStatus),
                    PersonLifeState = PersonLifeState.FromXmlType(partState.LivStatus)
                };
            }
            return null;
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<PersonState>(ps => ps.PersonCivilState);
            loadOptions.LoadWith<PersonState>(ps => ps.PersonLifeState);
        }
    }
}
