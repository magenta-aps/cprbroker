using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;
using System.Xml.Linq;
using System.IO;
using CprBroker.Utilities;

namespace CprBroker.Data.Part
{
    public partial class PersonRegistration
    {
        public static Schemas.Part.RegistreringType1 ToXmlType(PersonRegistration db)
        {
            if (db != null)
            {
                var xml = db.Contents.ToString();
                return Strings.Deserialize<RegistreringType1>(xml);
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
            PersonRegistration ret = null;
            if (partRegistration != null)
            {
                ret = new PersonRegistration()
                {
                    PersonRegistrationId = Guid.NewGuid(),

                    ActorRef = ActorRef.FromXmlType(partRegistration.AktoerRef),
                    CommentText = partRegistration.CommentText,
                    LifecycleStatusId = LifecycleStatus.GetCode(partRegistration.LivscyklusKode),
                    RegistrationDate = partRegistration.Tidspunkt.ToDateTime().Value,

                    PersonAttributes = PersonAttributes.FromXmlType(partRegistration.AttributListe),
                    PersonState = PersonState.FromXmlType(partRegistration.TilstandListe),
                };
                if (partRegistration.RelationListe != null)
                {
                    ret.PersonRelationships.AddRange(PersonRelationship.FromXmlType(partRegistration.RelationListe));
                }

                var xml = Strings.SerializeObject(partRegistration);
                ret.Contents = System.Xml.Linq.XElement.Load(new StringReader(xml));
            }
            return ret;
        }

        public bool Equals(RegistreringType1 oio)
        {
            var xml = Strings.SerializeObject(oio);
            // Repeat serialization to avoid empty text
            oio = Strings.Deserialize<RegistreringType1>(xml);
            xml = Strings.SerializeObject(oio);

            var thisOio = ToXmlType(this);
            var thisXml = Strings.SerializeObject(thisOio);
            return string.Equals(xml, thisXml);
        }
    }
}
