using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
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
    }
}
