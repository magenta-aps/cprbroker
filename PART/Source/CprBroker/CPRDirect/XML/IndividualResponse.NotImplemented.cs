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

        private VirkningType[] ToVirkningType()
        {
            throw new NotImplementedException();
        }

        private LokalUdvidelseType ToLokalUdvidelseType()
        {
            throw new NotImplementedException();
        }

        private PersonRelationType[] ToErstatningAf()
        {
            throw new NotImplementedException();
        }

        private PersonFlerRelationType[] ToErstatningFor()
        {
            throw new NotImplementedException();
        }

        private PersonFlerRelationType[] ToForaeldremyndighedsboern()
        {
            // Parental authority children
            throw new NotImplementedException();
        }

        private PersonRelationType[] ToRetligHandleevneVaergeForPersonen()
        {
            // Persons for whom this person is a guardian
            throw new NotImplementedException();
        }

        private PersonFlerRelationType[] ToBopaelssamling()
        {
            // residence collection ???
            throw new NotImplementedException();
        }

        private SundhedOplysningType[] ToSundhedOplysningType()
        {
            throw new NotImplementedException();
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
            // TODO: See if can be found
            return null;
        }

    }
}
