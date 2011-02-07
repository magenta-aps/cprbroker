using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class PersonRegistration
    {
        public static Schemas.Part.RegistreringType1 ToXmlType(PersonRegistration db)
        {
            if (db != null)
            {
                return new CprBroker.Schemas.Part.RegistreringType1()
                {
                    AktoerRef = ActorRef.ToXmlType(db.ActorRef),
                    Tidspunkt = TidspunktType.Create(db.RegistrationDate),
                    AttributListe = PersonAttributes.ToXmlType(db.PersonAttributes),
                    TilstandListe = PersonState.ToXmlType(db.PersonState),
                    RelationListe = PersonRelationship.ToXmlType(db.PersonRelationships.ToArray().AsQueryable()),
                    CommentText = db.CommentText,
                    LivscyklusKode = LifecycleStatus.GetEnum(db.LifecycleStatusId),
                    Virkning = Utilities.AsArray<VirkningType>(Effect.ToVirkningType(db.Effect)),
                };
            }
            return null;
        }

        public static void SetChildLoadOptions(PartDataContext dataContext)
        {
            DataLoadOptions loadOptions = new DataLoadOptions();
            SetChildLoadOptions(loadOptions);
            dataContext.LoadOptions = loadOptions;
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<PersonRegistration>(pr => pr.Effect);
            loadOptions.LoadWith<PersonRegistration>(pr => pr.PersonAttributes);
            loadOptions.LoadWith<PersonRegistration>(pr => pr.PersonRelationships);
            loadOptions.LoadWith<PersonRegistration>(pr => pr.PersonState);

            PersonAttributes.SetChildLoadOptions(loadOptions);
        }

        public static PersonRegistration FromXmlType(CprBroker.Schemas.Part.RegistreringType1 partRegistration)
        {
            PersonRegistration ret = new PersonRegistration()
            {
                PersonRegistrationId = Guid.NewGuid(),

                ActorRef = partRegistration.AktoerRef != null ? ActorRef.FromXmlType(partRegistration.AktoerRef) : null,
                CommentText = partRegistration.CommentText,
                Effect = partRegistration.Virkning != null && partRegistration.Virkning.Length > 0 && partRegistration.Virkning[0] != null ? DAL.Part.Effect.FromVirkningType(partRegistration.Virkning[0]) : null,
                LifecycleStatusId = LifecycleStatus.GetCode(partRegistration.LivscyklusKode),
                RegistrationDate = partRegistration.Tidspunkt.ToDateTime().Value,

                PersonAttributes = partRegistration.AttributListe != null ? PersonAttributes.FromXmlType(partRegistration.AttributListe) : null,
                PersonState = partRegistration.TilstandListe != null ? PersonState.FromXmlType(partRegistration.TilstandListe) : null
            };
            if (partRegistration.RelationListe != null)
            {
                ret.PersonRelationships.AddRange(PersonRelationship.FromXmlType(partRegistration.RelationListe));
            }
            return ret;
        }
    }
}
