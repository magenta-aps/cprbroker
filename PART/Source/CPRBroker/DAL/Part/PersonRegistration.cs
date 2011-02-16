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
                var ret = new CprBroker.Schemas.Part.RegistreringType1()
                {
                    AktoerRef = ActorRef.ToXmlType(db.ActorRef),
                    Tidspunkt = TidspunktType.Create(db.RegistrationDate),
                    AttributListe = PersonAttributes.ToXmlType(db.PersonAttributes),
                    TilstandListe = PersonState.ToXmlType(db.PersonState),
                    RelationListe = PersonRelationship.ToXmlType(db.PersonRelationships.ToArray().AsQueryable()),
                    CommentText = db.CommentText,
                    LivscyklusKode = LifecycleStatus.GetEnum(db.LifecycleStatusId),
                    Virkning = null,
                };
                ret.CalculateVirkning();
                return ret;
            }
            return null;
        }

        public static void SetChildLoadOptions(PartDataContext dataContext)
        {
            // TODO: LoadOptions actually slows down performance
            return;

            DataLoadOptions loadOptions = new DataLoadOptions();
            SetChildLoadOptions(loadOptions);
            dataContext.LoadOptions = loadOptions;
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<PersonRegistration>(pr => pr.PersonAttributes);
            loadOptions.LoadWith<PersonRegistration>(pr => pr.PersonRelationships);
            loadOptions.LoadWith<PersonRegistration>(pr => pr.PersonState);
            loadOptions.LoadWith<PersonRegistration>(pr => pr.ActorRef);
            loadOptions.LoadWith<PersonRegistration>(pr => pr.LifecycleStatus);


            Effect.SetChildLoadOptions(loadOptions);
            CountryRef.SetChildLoadOptions(loadOptions);

            PersonAttributes.SetChildLoadOptions(loadOptions);
            PersonRelationship.SetChildLoadOptions(loadOptions);
            PersonState.SetChildLoadOptions(loadOptions);
        }

        public static PersonRegistration FromXmlType(CprBroker.Schemas.Part.RegistreringType1 partRegistration)
        {
            PersonRegistration ret = new PersonRegistration()
            {
                PersonRegistrationId = Guid.NewGuid(),

                ActorRef = ActorRef.FromXmlType(partRegistration.AktoerRef),
                CommentText = partRegistration.CommentText,
                LifecycleStatusId = LifecycleStatus.GetCode(partRegistration.LivscyklusKode),
                RegistrationDate = partRegistration.Tidspunkt.ToDateTime().Value,

                PersonAttributes = PersonAttributes.FromXmlType(partRegistration.AttributListe),
                PersonState = PersonState.FromXmlType(partRegistration.TilstandListe)
            };
            if (partRegistration.RelationListe != null)
            {
                ret.PersonRelationships.AddRange(PersonRelationship.FromXmlType(partRegistration.RelationListe));
            }
            return ret;
        }
    }
}
