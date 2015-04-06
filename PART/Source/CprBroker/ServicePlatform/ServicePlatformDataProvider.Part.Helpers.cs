using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.PartInterface;
using CprBroker.Schemas.Part;
using CprBroker.Providers.CprServices;
using CprBroker.Engine.Local;
using CprBroker.Engine.Part;
using CprBroker.Engine;
using System.Xml;
using CprBroker.Providers.ServicePlatform.Responses;

namespace CprBroker.Providers.ServicePlatform
{
    public partial class ServicePlatformDataProvider
    {
        

        public AttributListeType ToAttributListeType(string stamPlusResponse)
        {
            return new Schemas.Part.AttributListeType()
            {
                Egenskab = new EgenskabType[]
                {
                    new EgenskabType()
                    {
                        NavnStruktur = null,
                        AndreAdresser = null,
                        BirthDate = default(DateTime),
                        FoedestedNavn = null,
                        FoedselsregistreringMyndighedNavn = null,
                        KontaktKanal = null,
                        NaermestePaaroerende = null,
                        PersonGenderCode = default(PersonGenderCodeType),
                        Virkning = null
                    }
                },
                RegisterOplysning = new RegisterOplysningType[] 
                { 
                    new RegisterOplysningType()
                    {
                        Item = new CprBorgerType()
                        {
                            AdresseNoteTekst = null,
                            PersonNummerGyldighedStatusIndikator = default(bool),
                            FolkekirkeMedlemIndikator = default(bool),
                            FolkeregisterAdresse = new AdresseType()
                            {
                                Item = null,
                            },
                            ForskerBeskyttelseIndikator = default(bool),
                            NavneAdresseBeskyttelseIndikator = default(bool),
                            PersonCivilRegistrationIdentifier = null,
                            PersonNationalityCode = null,
                            TelefonNummerBeskyttelseIndikator = default(bool)
                        },
                        Virkning = null,
                    }
                }
            };
        }
    }
}
