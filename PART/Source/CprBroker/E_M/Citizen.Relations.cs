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
        private RelationListeType ToRelationListeType(Func<string, Guid> cpr2uuidFunc)
        {
            var ret = new RelationListeType()
            {
                Aegtefaelle = ToSpouses(cpr2uuidFunc),
                Boern = ToChildren(cpr2uuidFunc),
                Bopaelssamling = null,
                ErstatningAf = null,
                ErstatningFor = null,
                Fader = ToFather(cpr2uuidFunc),
                Foraeldremyndighedsboern = null,
                Foraeldremyndighedsindehaver = null,
                LokalUdvidelse = null,
                Moder = ToMother(cpr2uuidFunc),
                RegistreretPartner = ToRegisteredPartners(cpr2uuidFunc),
                RetligHandleevneVaergeForPersonen = null,
                RetligHandleevneVaergemaalsindehaver = null
            };

            return ret;
        }

        private PersonRelationType[] ToSpouses(Func<string, Guid> cpr2uuidFunc)
        {
            //TODO: Revise the logic for start and end dates
            PersonRelationType ret = null;
            switch (Converters.ToCivilStatusKodeType(MaritalStatus))
            {
                case CivilStatusKodeType.Gift:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(SpousePNR)), Converters.ToDateTime(MaritalStatusTimestamp, MaritalStatusTimestampUncertainty), null);
                    break;
                case CivilStatusKodeType.Separeret:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(SpousePNR)), Converters.ToDateTime(MaritalStatusTimestamp, MaritalStatusTimestampUncertainty), Converters.ToDateTime(MaritalStatusTerminationTimestamp, MaritalStatusTerminationTimestampUncertainty));
                    break;
                case CivilStatusKodeType.Skilt:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(SpousePNR)), Converters.ToDateTime(MaritalStatusTimestamp, MaritalStatusTimestampUncertainty), Converters.ToDateTime(MaritalStatusTerminationTimestamp, MaritalStatusTerminationTimestampUncertainty));
                    break;
                case CivilStatusKodeType.Enke:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(SpousePNR)), Converters.ToDateTime(MaritalStatusTimestamp, MaritalStatusTimestampUncertainty), Converters.ToDateTime(MaritalStatusTerminationTimestamp, MaritalStatusTerminationTimestampUncertainty));
                    break;
                default:
                    return null;
            }
            return new PersonRelationType[] { ret };
        }

        private PersonFlerRelationType[] ToChildren(Func<string, Guid> cpr2uuidFunc)
        {
            var children = Converters.ToPersonGenderCodeType(Gender) == PersonGenderCodeType.male ? ChildrenAsFather : ChildrenAsMother;
            return PersonFlerRelationType.CreateList(children.Select(child => cpr2uuidFunc(Converters.ToCprNumber(child.PNR))).ToArray());
        }

        private PersonRelationType[] ToRegisteredPartners(Func<string, Guid> cpr2uuidFunc)
        {
            //TODO: Revise the logic for start and end dates
            PersonRelationType ret = null;
            switch (Converters.ToCivilStatusKodeType(MaritalStatus))
            {
                case CivilStatusKodeType.RegistreretPartner:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(SpousePNR)), Converters.ToDateTime(MaritalStatusTimestamp, MaritalStatusTimestampUncertainty), null);
                    break;
                case CivilStatusKodeType.OphaevetPartnerskab:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(SpousePNR)), Converters.ToDateTime(MaritalStatusTimestamp, MaritalStatusTimestampUncertainty), Converters.ToDateTime(MaritalStatusTerminationTimestamp, MaritalStatusTerminationTimestampUncertainty));
                    break;
                case CivilStatusKodeType.Laengstlevende:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(SpousePNR)), Converters.ToDateTime(MaritalStatusTimestamp, MaritalStatusTimestampUncertainty), Converters.ToDateTime(MaritalStatusTerminationTimestamp, MaritalStatusTerminationTimestampUncertainty));
                    break;
                default:
                    return null;
            }
            return new PersonRelationType[] { ret };
        }

        private PersonRelationType[] ToFather(Func<string, Guid> cpr2uuidFunc)
        {
            return PersonRelationType.CreateList(cpr2uuidFunc(Converters.ToCprNumber(FatherPNR)));
        }

        private PersonRelationType[] ToMother(Func<string, Guid> cpr2uuidFunc)
        {
            return PersonRelationType.CreateList(cpr2uuidFunc(Converters.ToCprNumber(MotherPNR)));
        }

    }
}