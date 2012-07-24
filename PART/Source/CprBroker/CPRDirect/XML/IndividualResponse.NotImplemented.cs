using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class IndividualResponseType
    {
        private object ToSourceObject()
        {
            // TODO: Implement source objects in CPR Direct
            return null;
        }

        private LivscyklusKodeType ToLivscyklusKodeType()
        {
            // TODO: Implemet further lifecycle status codes in CPR Direct (from history change extractes)
            return LivscyklusKodeType.Rettet;
        }

        public LokalUdvidelseType ToLokalUdvidelseType()
        {
            return null;
        }

        public PersonRelationType[] ToErstatningAf()
        {
            // TODO: Implement replaced by
            return null;
        }

        public PersonFlerRelationType[] ToErstatningFor()
        {
            // TOTO: Implemenet replacement for
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
            // TODO: Implement persons who live at the same address
            return null;
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
            // TODO: Fill Supplementary address after seeing real data (CurrentAddressInformation.SupplementaryAddressLine 1 - 5
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
