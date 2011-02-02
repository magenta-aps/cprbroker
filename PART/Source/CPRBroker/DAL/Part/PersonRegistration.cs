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
        public Schemas.Part.RegistreringType1 ToXmlType()
        {
            Schemas.Part.RegistreringType1 ret = new CprBroker.Schemas.Part.RegistreringType1()
            {
                //AktoerTekst = this.ActorText,
                //TidspunktDatoTid = TidspunktType.Create(this.RegistrationDate),
                AttributListe = this.PersonAttribute.ToXmlType(),
                TilstandListe = PersonState.ToXmlType(),
                //ToArray() is called to avoid querying the database again
                RelationListe = PersonRelationship.ToXmlType(this.PersonRelationships.ToArray().AsQueryable()),
                CommentText = this.CommentText,
                LivscyklusKode = LifecycleStatus.GetEnum(this.LifecycleStatusId),
                //Virkning = this.Effect.ToXmlType()

            };
            return ret;
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
            loadOptions.LoadWith<PersonRegistration>(pr => pr.PersonAttribute);
            loadOptions.LoadWith<PersonRegistration>(pr => pr.PersonRelationships);
            loadOptions.LoadWith<PersonRegistration>(pr => pr.PersonState);

            PersonAttribute.SetChildLoadOptions(loadOptions);
        }

        public static PersonRegistration FromXmlType(CprBroker.Schemas.Part.RegistreringType1 partRegistration)
        {
            PersonRegistration ret = new PersonRegistration()
            {
                PersonRegistrationId = Guid.NewGuid(),

                //ActorText = partRegistration.AktoerTekst,
                CommentText = partRegistration.CommentText,
                //Effect = DAL.Part.Effect.FromXmlType(partRegistration.Virkning),
                LifecycleStatusId = LifecycleStatus.GetCode(partRegistration.LivscyklusKode),
                //RegistrationDate = partRegistration.TidspunktDatoTid.ToDateTime().Value,

                PersonAttribute = PersonAttribute.FromXmlType(partRegistration.AttributListe),
                PersonState = PersonState.FromXmlType(partRegistration.TilstandListe)
            };

            ret.PersonRelationships.AddRange(PersonRelationship.FromXmlType(partRegistration.RelationListe));
            return ret;
        }
    }
}
