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

        public PersonRelationType[] ToSpouses(CivilStatusKodeType existingStatusCode, CivilStatusKodeType[] terminatedStatusCodes, bool sameGenderSpouseForDead, Func<string, Guid> cpr2uuidFunc)
        {
            if (cpr2uuidFunc != null)
            {
                var status = Converters.ToCivilStatusKodeType(this.MaritalStatus);
                if (status == CivilStatusKodeType.Ugift
                    && this.Spouse != null
                    && sameGenderSpouseForDead == (this.Gender == this.Spouse.Gender)
                    )
                {
                    return PersonRelationType.CreateList(cpr2uuidFunc(this.ToSpousePNR()), null, this.ToMaritalStatusDate());
                }
                else if (status == existingStatusCode)
                {
                    return PersonRelationType.CreateList(cpr2uuidFunc(this.ToSpousePNR()), this.ToMaritalStatusDate(), null);
                }
                else if (terminatedStatusCodes.Contains(status))
                {
                    return PersonRelationType.CreateList(cpr2uuidFunc(this.ToSpousePNR()), null, this.ToMaritalStatusDate());
                }
                else
                {
                    return new PersonRelationType[0];
                }
            }
            else
            {
                throw new ArgumentNullException("cpr2uuidFunc");
            }
        }

        public PersonRelationType[] ToSpouses(Func<string, Guid> cpr2uuidFunc)
        {
            return ToSpouses(
                CivilStatusKodeType.Gift,
                new CivilStatusKodeType[]{
                    CivilStatusKodeType.Separeret,
                    CivilStatusKodeType.Skilt,
                    CivilStatusKodeType.Enke},
                false,
                cpr2uuidFunc
            );
        }

        public PersonRelationType[] ToRegisteredPartners(Func<string, Guid> cpr2uuidFunc)
        {
            return ToSpouses(
               CivilStatusKodeType.RegistreretPartner,
               new CivilStatusKodeType[]{
                    CivilStatusKodeType.OphaevetPartnerskab,
                    CivilStatusKodeType.Laengstlevende},
               true,
               cpr2uuidFunc
           );
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

        public PersonRelationType[] ToFather(Func<string, Guid> cpr2uuidFunc)
        {
            if (this.FatherPNR > 0)
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
            return new PersonRelationType[0];
        }

        public PersonRelationType[] ToMother(Func<string, Guid> cpr2uuidFunc)
        {
            if (this.MotherPNR > 0)
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
            return new PersonRelationType[0];
        }

    }
}