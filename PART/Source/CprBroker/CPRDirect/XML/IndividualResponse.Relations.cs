using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class IndividualResponseType
    {
        public RelationListeType ToRelationListeType()
        {
            return new RelationListeType()
            {
                Aegtefaelle = ToSpouses(),
                Boern = ToChildren(),
                Bopaelssamling = ToBopaelssamling(),
                ErstatningAf = ToErstatningAf(),
                ErstatningFor = ToErstatningFor(),
                Fader = ToFather(),
                Foraeldremyndighedsboern = ToForaeldremyndighedsboern(),
                Foraeldremyndighedsindehaver = ToForaeldremyndighedsindehaver(),
                LokalUdvidelse = ToLokalUdvidelseType(),
                Moder = ToMother(),
                RegistreretPartner = ToRegisteredPartners(),
                RetligHandleevneVaergeForPersonen = ToRetligHandleevneVaergeForPersonen(),
                RetligHandleevneVaergemaalsindehaver = ToRetligHandleevneVaergemaalsindehaver()
            };
        }

        private PersonRelationType[] ToRegisteredPartners()
        {
            throw new NotImplementedException();
        }

        private PersonFlerRelationType[] ToChildren()
        {
            throw new NotImplementedException();
        }

        private PersonRelationType[] ToSpouses()
        {
            throw new NotImplementedException();
        }

        private PersonRelationType[] ToMother()
        {
            throw new NotImplementedException();
        }

        private PersonRelationType[] ToFather()
        {
            throw new NotImplementedException();
        }

        private PersonFlerRelationType[] ToRetligHandleevneVaergemaalsindehaver()
        {
            throw new NotImplementedException();
        }

        private PersonRelationType[] ToRetligHandleevneVaergeForPersonen()
        {
            throw new NotImplementedException();
        }

        private PersonRelationType[] ToForaeldremyndighedsindehaver()
        {
            throw new NotImplementedException();
        }

        private PersonFlerRelationType[] ToForaeldremyndighedsboern()
        {
            throw new NotImplementedException();
        }

        private PersonFlerRelationType[] ToBopaelssamling()
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

    }
}
