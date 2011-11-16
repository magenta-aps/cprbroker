using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.E_M
{
    public partial class Citizen
    {
        public virtual RelationListeType ToRelationListeType(Func<string, Guid> cpr2uuidFunc)
        {
            var ret = new RelationListeType()
            {
                Aegtefaelle = this.ToSpouses(cpr2uuidFunc),
                Boern = this.ToChildren(cpr2uuidFunc),
                Bopaelssamling = null,
                ErstatningAf = null,
                ErstatningFor = null,
                Fader = this.ToFather(cpr2uuidFunc),
                Foraeldremyndighedsboern = null,
                Foraeldremyndighedsindehaver = null,
                LokalUdvidelse = null,
                Moder = this.ToMother(cpr2uuidFunc),
                RegistreretPartner = this.ToRegisteredPartners(cpr2uuidFunc),
                RetligHandleevneVaergeForPersonen = null,
                RetligHandleevneVaergemaalsindehaver = null
            };

            return ret;
        }

        public string ToSpousePNR()
        {
            return Converters.ToCprNumber(this.SpousePNR);
        }

        public PersonRelationType[] ToSpouses(Func<string, Guid> cpr2uuidFunc)
        {
            if (cpr2uuidFunc != null)
            {
                switch (Converters.ToCivilStatusKodeType(this.MaritalStatus))
                {
                    case CivilStatusKodeType.Gift:
                        return PersonRelationType.CreateList(cpr2uuidFunc(this.ToSpousePNR()), this.ToMaritalStatusDate(), null);
                    case CivilStatusKodeType.Separeret:
                        return PersonRelationType.CreateList(cpr2uuidFunc(this.ToSpousePNR()), null, this.ToMaritalStatusDate());
                    case CivilStatusKodeType.Skilt:
                        return PersonRelationType.CreateList(cpr2uuidFunc(this.ToSpousePNR()), null, this.ToMaritalStatusDate());
                    case CivilStatusKodeType.Enke:
                        return PersonRelationType.CreateList(cpr2uuidFunc(this.ToSpousePNR()), null, this.ToMaritalStatusDate());
                }
                return new PersonRelationType[0];
            }
            else
            {
                throw new ArgumentNullException("cpr2uuidFunc");
            }
        }

        public PersonFlerRelationType[] ToChildren(Func<string, Guid> cpr2uuidFunc)
        {
            if (cpr2uuidFunc != null)
            {
                var gender = Converters.ToPersonGenderCodeType(this.Gender);
                Func<System.Data.Linq.EntitySet<Child>, PersonFlerRelationType[]> converter =
                    (children) =>
                        children.Select(child => child.ToPersonFlerRelationType(cpr2uuidFunc)).ToArray();
                switch (gender)
                {
                    case PersonGenderCodeType.male:
                        return converter(this.ChildrenAsFather);
                    case PersonGenderCodeType.female:
                        return converter(this.ChildrenAsMother);
                }
                return new PersonFlerRelationType[0];
            }
            else
            {
                throw new ArgumentNullException("cpr2uuidFunc");
            }
        }

        public PersonRelationType[] ToRegisteredPartners(Func<string, Guid> cpr2uuidFunc)
        {
            if (cpr2uuidFunc != null)
            {
                switch (Converters.ToCivilStatusKodeType(this.MaritalStatus))
                {
                    case CivilStatusKodeType.RegistreretPartner:
                        return PersonRelationType.CreateList(cpr2uuidFunc(this.ToSpousePNR()), this.ToMaritalStatusDate(), null);
                    case CivilStatusKodeType.OphaevetPartnerskab:
                        return PersonRelationType.CreateList(cpr2uuidFunc(this.ToSpousePNR()), null, this.ToMaritalStatusDate());
                    case CivilStatusKodeType.Laengstlevende:
                        return PersonRelationType.CreateList(cpr2uuidFunc(this.ToSpousePNR()), null, this.ToMaritalStatusDate());
                }
                return new PersonRelationType[0];
            }
            else
            {
                throw new ArgumentNullException("cpr2uuidFunc");
            }
        }

        public PersonRelationType[] ToFather(Func<string, Guid> cpr2uuidFunc)
        {
            if (Converters.IsValidCprNumber(this.FatherPNR))
            {
                if (cpr2uuidFunc != null)
                {
                    return PersonRelationType.CreateList(cpr2uuidFunc(Converters.ToCprNumber(this.FatherPNR)));
                }
                else
                {
                    throw new ArgumentNullException("cpr2uuidFunc");
                }
            }
            else
            {
                throw new ArgumentException("Invalid FatherPNR", "citizen.FatherPNR");
            }
        }

        public PersonRelationType[] ToMother(Func<string, Guid> cpr2uuidFunc)
        {
            if (Converters.IsValidCprNumber(this.MotherPNR))
            {
                if (cpr2uuidFunc != null)
                {
                    return PersonRelationType.CreateList(cpr2uuidFunc(Converters.ToCprNumber(this.MotherPNR)));
                }
                else
                {
                    throw new ArgumentNullException("cpr2uuidFunc");
                }
            }
            else
            {
                throw new ArgumentException("Invalid MotherPNR", "citizen.MotherPNR");
            }
        }

    }
}