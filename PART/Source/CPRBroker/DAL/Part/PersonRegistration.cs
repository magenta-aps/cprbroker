using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas.Part;

namespace CPRBroker.DAL.Part
{
    public partial class PersonRegistration
    {
        public Schemas.Part.PersonRegistration ToXmlType()
        {
            Schemas.Part.PersonRegistration ret = new CPRBroker.Schemas.Part.PersonRegistration()
            {
                Attributes = this.PersonAttribute.ToXmlType(),
                RegistrationDate = this.RegistrationDate,
                States = PersonState.ToXmlType(),
                Relations = PersonRelationship.GetPersonRelations(this.PersonRelationships.ToArray().AsQueryable())
            };

            return ret;
        }

        public static PersonRegistration FromXmlType(Person person, CPRBroker.Schemas.Part.PersonRegistration partRegistration)
        {
            PersonRegistration ret = new PersonRegistration()
            {
                ActorId = partRegistration.ActorId,
                RegistrationDate = partRegistration.RegistrationDate,
                PersonRegistrationId = Guid.NewGuid(),
                Person = person,
                //TODO : Fill person attributes
                PersonAttribute = null,
                //TODO : Fill person relations
                PersonRelationships = null,
                //TODO : Fill person state
                PersonState = null
            };
            return ret;
        }
    }
}
