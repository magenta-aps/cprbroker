using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class PersonRegistration
    {
        public Schemas.Part.RegistreringType1 ToXmlType()
        {
            Schemas.Part.RegistreringType1 ret = new CprBroker.Schemas.Part.RegistreringType1()
            {
                AktoerTekst = this.ActorText,                
                TidspunktDatoTid = TidspunktType.Create(this.RegistrationDate),
                AttributListe = this.PersonAttribute.ToXmlType(),
                TilstandListe = PersonState.ToXmlType(),
                //ToArray() is called to avoid querying the database again
                RelationListe = PersonRelationship.ToXmlType(this.PersonRelationships.ToArray().AsQueryable()),
                CommentText = this.CommentText,                
                LivscyklusKode = LifecycleStatus.GetEnum(this.LifecycleStatusId),                
                Virkning = this.Effect.ToXmlType()

            };
            return ret;
        }

        public static PersonRegistration FromXmlType(CprBroker.Schemas.Part.RegistreringType1 partRegistration)
        {
            PersonRegistration ret = new PersonRegistration()
            {
                ActorText = partRegistration.AktoerTekst,
                RegistrationDate = partRegistration.TidspunktDatoTid.ToDateTime().Value,

                PersonRegistrationId = Guid.NewGuid(),

                PersonAttribute = PersonAttribute.FromXmlType(partRegistration.AttributListe),
                PersonState = PersonState.FromXmlType(partRegistration.TilstandListe)
            };
            // TODO: Add relations
            //ret.PersonRelationships.AddRange(PersonRelationship.FromXmlType(partRegistration.RelationListe));
            return ret;
        }
    }
}
