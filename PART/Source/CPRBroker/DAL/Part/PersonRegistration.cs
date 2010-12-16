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
                ActorId = this.ActorId,
                RegistrationDate = this.RegistrationDate,

                Attributes = this.PersonAttribute.ToXmlType(),
                States = PersonState.ToXmlType(),
                Relations = PersonRelationship.ToXmlType(this.PersonRelationships.ToArray().AsQueryable())
            };
            return ret;
        }

        public static PersonRegistration FromXmlType(CPRBroker.Schemas.Part.PersonRegistration partRegistration)
        {
            PersonRegistration ret = new PersonRegistration()
            {
                ActorId = partRegistration.ActorId,
                RegistrationDate = partRegistration.RegistrationDate,

                PersonRegistrationId = Guid.NewGuid(),

                PersonAttribute = PersonAttribute.FromXmlType(partRegistration.Attributes),
                PersonState = PersonState.FromXmlType(partRegistration.States)
            };
            ret.PersonRelationships.AddRange(PersonRelationship.FromXmlType(partRegistration.Relations));
            return ret;
        }
    }
}
