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
    }
}
