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

        public static DateTime? ToMaritalStatusDate(Citizen citizen)
        {
            if (citizen != null)
            {
                return Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty);
            }
            else
            {
                throw new ArgumentNullException("citizen");
            }
        }

        public static DateTime? ToMaritalStatusTerminationDate(Citizen citizen)
        {
            if (citizen != null)
            {
                return Converters.ToDateTime(citizen.MaritalStatusTerminationTimestamp, citizen.MaritalStatusTerminationTimestampUncertainty);
            }
            else
            {
                throw new ArgumentNullException("citizen");
            }
        }

        public static PersonRelationType[] ToSpouses(Citizen citizen, Func<string, Guid> cpr2uuidFunc)
        {
            if (citizen != null)
            {
                if (cpr2uuidFunc != null)
                {
                    //TODO: Revise the logic for start and end dates
                    List<PersonRelationType> ret = new List<PersonRelationType>();
                    switch (Converters.ToCivilStatusKodeType(citizen.MaritalStatus))
                    {
                        case CivilStatusKodeType.Gift:
                            ret.Add(PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty), null));
                            break;
                        case CivilStatusKodeType.Separeret:
                            ret.Add(PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty), Converters.ToDateTime(citizen.MaritalStatusTerminationTimestamp, citizen.MaritalStatusTerminationTimestampUncertainty)));
                            break;
                        case CivilStatusKodeType.Skilt:
                            ret.Add(PersonRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), Converters.ToDateTime(citizen.MaritalStatusTimestamp, citizen.MaritalStatusTimestampUncertainty), Converters.ToDateTime(citizen.MaritalStatusTerminationTimestamp, citizen.MaritalStatusTerminationTimestampUncertainty)));
                            break;
                        case CivilStatusKodeType.Enke:
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
                    switch (Converters.ToCivilStatusKodeType(citizen.MaritalStatus))
                    {
                        case CivilStatusKodeType.RegistreretPartner:
                            return PersonRelationType.CreateList(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), ToMaritalStatusDate(citizen), null);
                            break;
                        case CivilStatusKodeType.OphaevetPartnerskab:
                            return PersonRelationType.CreateList(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), ToMaritalStatusDate(citizen), ToMaritalStatusTerminationDate(citizen));
                            break;
                        case CivilStatusKodeType.Laengstlevende:
                            return PersonRelationType.CreateList(cpr2uuidFunc(Converters.ToCprNumber(citizen.SpousePNR)), ToMaritalStatusDate(citizen), ToMaritalStatusTerminationDate(citizen));
                            break;
                    }
                    return new PersonRelationType[0];
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