using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class IndividualResponseType
    {
        private TidspunktType ToTidspunktType()
        {
            throw new NotImplementedException();
        }

        private object ToSourceObject()
        {
            throw new NotImplementedException();
        }

        private LivscyklusKodeType ToLivscyklusKodeType()
        {
            throw new NotImplementedException();
        }

        public LokalUdvidelseType ToLokalUdvidelseType()
        {
            return null;
        }

        public PersonRelationType[] ToErstatningAf()
        {
            return null;
        }

        public PersonFlerRelationType[] ToErstatningFor()
        {
            return null;
        }

        private PersonFlerRelationType[] ToForaeldremyndighedsboern()
        {
            // Parental authority children
            // TODO: Can we perform a reverse parental authority lookup here?
            return null;
        }

        private PersonRelationType[] ToRetligHandleevneVaergeForPersonen()
        {
            // Persons for whom this person is a guardian
            // TODO: Can we perform a reverse guardian/disempoerment lookup here?
            return null;
        }

        private PersonFlerRelationType[] ToBopaelssamling()
        {
            // residence collection ???
            throw new NotImplementedException();
        }

        private SundhedOplysningType[] ToSundhedOplysningType()
        {
            // Not implemented
            return null;
        }

        public KontaktKanalType ToKontaktKanalType()
        {
            return null;
        }

        public KontaktKanalType ToNaermestePaaroerende()
        {
            return null;
        }

        public string ToFoedestedNavn()
        {
            // Birth name not implemented
            // TODO: See if can be found in historical names
            return null;
        }

        public AdresseType ToAndreAdresser()
        {
            // TODO: See if it is possible to get another address in CPR Direct
            return null;
        }

        public bool ToTelefonNummerBeskyttelseIndikator()
        {
            // No phone protection
            // TODO: Is phone protection the same as directory protection? If yes, fix this and other data providers too
            return false;
        }

    }
}
