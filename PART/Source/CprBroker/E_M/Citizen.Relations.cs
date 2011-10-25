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
        public static RelationListeType ToRelationListeType(Citizen citizen, Func<string, Guid> cpr2uuidFunc)
        {
            var ret = new RelationListeType()
            {
                Aegtefaelle = ToSpouses(citizen, cpr2uuidFunc),
                Boern = ToChildren(citizen, cpr2uuidFunc),
                Bopaelssamling = null,
                ErstatningAf = null,
                ErstatningFor = null,
                Fader = ToFather(citizen, cpr2uuidFunc),
                Foraeldremyndighedsboern = null,
                Foraeldremyndighedsindehaver = null,
                LokalUdvidelse = null,
                Moder = ToMother(citizen, cpr2uuidFunc),
                RegistreretPartner = ToRegisteredPartners(citizen, cpr2uuidFunc),
                RetligHandleevneVaergeForPersonen = null,
                RetligHandleevneVaergemaalsindehaver = null
            };

            return ret;
        }

        public static PersonRelationType[] ToSpouses(Citizen citizen, Func<string, Guid> cpr2uuidFunc)
        {
            //TODO: Revise the logic for start and end dates
            PersonRelationType ret = null;
            switch (Converters.ToCivilStatusKodeType(citizen.MaritalStatus))
            {
                case CivilStatusKodeType.Gift:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty), null);
                    break;
                case CivilStatusKodeType.Separeret:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty), Converters.ToDateTime(citizen.MaritalStatusTerminationTimestamp, citizen.MaritalStatusTerminationTimestampUncertainty));
                    break;
                case CivilStatusKodeType.Skilt:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty), Converters.ToDateTime(citizen.MaritalStatusTerminationTimestamp, citizen.MaritalStatusTerminationTimestampUncertainty));
                    break;
                case CivilStatusKodeType.Enke:
                    ret = PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty), Converters.ToDateTime(citizen.MaritalStatusTerminationTimestamp, citizen.MaritalStatusTerminationTimestampUncertainty));
                    break;
                default:
                    return null;
            }
            return new PersonRelationType[] { ret };
        }

        public static PersonFlerRelationType[] ToChildren(Citizen citizen, Func<string, Guid> cpr2uuidFunc)
        {
            var children = Converters.ToPersonGenderCodeType(citizen.Gender) == PersonGenderCodeType.male ? citizen.ChildrenAsFather : citizen.ChildrenAsMother;
            return children.Select(child => Child.ToPersonFlerRelationType(child, cpr2uuidFunc)).ToArray();
        }

        public static PersonRelationType[] ToRegisteredPartners(Citizen citizen, Func<string, Guid> cpr2uuidFunc)
        {
            if (citizen != null)
            {
                if (cpr2uuidFunc != null)
                {
                    //TODO: Revise the logic for start and end dates
                    List<PersonRelationType> ret = new List<PersonRelationType>();
                    switch (Converters.ToCivilStatusKodeType(citizen.MaritalStatus))
                    {
                        case CivilStatusKodeType.RegistreretPartner:
                            if (Converters.ToDateTime(citizen.MaritalStatusTerminationTimestamp, citizen.MaritalStatusTerminationTimestampUncertainty).HasValue)
                            {
                                throw new ArgumentException("Termination date should be empty for existing registered partnership");
                            }
                            ret.Add(PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty), null));
                            break;
                        case CivilStatusKodeType.OphaevetPartnerskab:
                            ret.Add(PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty), Converters.ToDateTime(citizen.MaritalStatusTerminationTimestamp, citizen.MaritalStatusTerminationTimestampUncertainty)));
                            break;
                        case CivilStatusKodeType.Laengstlevende:
                            ret.Add(PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty), Converters.ToDateTime(citizen.MaritalStatusTerminationTimestamp, citizen.MaritalStatusTerminationTimestampUncertainty)));
                            break;
                    }
                    return ret.ToArray();
                }
                else
                {
                    throw new ArgumentNullException("cpr2uuidFunc");
                }
            }
            else
            {
                throw new ArgumentNullException("citizen");
            }
        }

        public static PersonRelationType[] ToFather(Citizen citizen, Func<string, Guid> cpr2uuidFunc)
        {
            if (citizen != null)
            {
                if (Converters.IsValidCprNumber(citizen.FatherPNR))
                {
                    if (cpr2uuidFunc != null)
                    {
                        return PersonRelationType.CreateList(cpr2uuidFunc(Converters.ToCprNumber(citizen.FatherPNR)));
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
            else
            {
                throw new ArgumentNullException("citizen");
            }
        }

        public static PersonRelationType[] ToMother(Citizen citizen, Func<string, Guid> cpr2uuidFunc)
        {
            if (citizen != null)
            {
                if (Converters.IsValidCprNumber(citizen.MotherPNR))
                {
                    if (cpr2uuidFunc != null)
                    {
                        return PersonRelationType.CreateList(cpr2uuidFunc(Converters.ToCprNumber(citizen.MotherPNR)));
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
            else
            {
                throw new ArgumentNullException("citizen");
            }
        }

    }
}