using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class IndividualResponseType
    {
        public RelationListeType ToRelationListeType(Func<string, Guid> cpr2uuidFunc)
        {
            return new RelationListeType()
            {
                Aegtefaelle = ToSpouses(cpr2uuidFunc),
                Boern = ToChildren(cpr2uuidFunc),
                Bopaelssamling = ToBopaelssamling(),
                ErstatningAf = ToErstatningAf(),
                ErstatningFor = ToErstatningFor(),
                Fader = ToFather(cpr2uuidFunc),
                Foraeldremyndighedsboern = ToForaeldremyndighedsboern(),
                Foraeldremyndighedsindehaver = ToForaeldremyndighedsindehaver(),
                LokalUdvidelse = ToLokalUdvidelseType(),
                Moder = ToMother(cpr2uuidFunc),
                RegistreretPartner = ToRegisteredPartners(cpr2uuidFunc),
                RetligHandleevneVaergeForPersonen = ToRetligHandleevneVaergeForPersonen(),
                RetligHandleevneVaergemaalsindehaver = ToRetligHandleevneVaergemaalsindehaver()
            };
        }

        public PersonRelationType[] ToFather(Func<string, Guid> cpr2uuidFunc)
        {
            return this.ParentsInformation.ToFather(cpr2uuidFunc);
        }

        public PersonRelationType[] ToMother(Func<string, Guid> cpr2uuidFunc)
        {
            return this.ParentsInformation.ToMather(cpr2uuidFunc);
        }

        public PersonFlerRelationType[] ToChildren(Func<string, Guid> cpr2uuidFunc)
        {
            return ChildType.ToPersonFlerRelationType(this.Child, cpr2uuidFunc);
        }

        private PersonRelationType[] ToRegisteredPartners(Func<string, Guid> cpr2uuidFunc)
        {
            return this.CurrentCivilStatus.ToRegisteredPartners(cpr2uuidFunc);
        }

        private PersonRelationType[] ToSpouses(Func<string, Guid> cpr2uuidFunc)
        {
            return this.CurrentCivilStatus.ToSpouses(cpr2uuidFunc);
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
