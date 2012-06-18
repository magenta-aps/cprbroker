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
                Foraeldremyndighedsindehaver = ToForaeldremyndighedsindehaver(cpr2uuidFunc),
                LokalUdvidelse = ToLokalUdvidelseType(),
                Moder = ToMother(cpr2uuidFunc),
                RegistreretPartner = ToRegisteredPartners(cpr2uuidFunc),
                RetligHandleevneVaergeForPersonen = ToRetligHandleevneVaergeForPersonen(),
                RetligHandleevneVaergemaalsindehaver = ToRetligHandleevneVaergemaalsindehaver(cpr2uuidFunc)
            };
        }

        public PersonRelationType[] ToFather(Func<string, Guid> cpr2uuidFunc)
        {
            return this.ParentsInformation.ToFather(cpr2uuidFunc);
        }

        public PersonRelationType[] ToMother(Func<string, Guid> cpr2uuidFunc)
        {
            return this.ParentsInformation.ToMother(cpr2uuidFunc);
        }

        public PersonFlerRelationType[] ToChildren(Func<string, Guid> cpr2uuidFunc)
        {
            return ChildType.ToPersonFlerRelationType(this.Child, cpr2uuidFunc);
        }

        public PersonRelationType[] ToRegisteredPartners(Func<string, Guid> cpr2uuidFunc)
        {
            return this.CurrentCivilStatus.ToRegisteredPartners(cpr2uuidFunc);
        }

        public PersonRelationType[] ToSpouses(Func<string, Guid> cpr2uuidFunc)
        {
            return this.CurrentCivilStatus.ToSpouses(cpr2uuidFunc);
        }

        public PersonFlerRelationType[] ToRetligHandleevneVaergemaalsindehaver(Func<string, Guid> cpr2uuidFunc)
        {
            // Persons who have legal authority on current person
            return DisempowermentType.ToPersonRelationType(this.Disempowerment, cpr2uuidFunc);
        }

        public PersonRelationType[] ToForaeldremyndighedsindehaver(Func<string, Guid> cpr2uuidFunc)
        {
            // Parental authority owner
            return ParentalAuthorityType.ToPersonRelationType(this.ParentalAuthority, cpr2uuidFunc);
        }





    }
}
