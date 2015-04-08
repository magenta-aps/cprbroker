using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.CprServices
{
    public class SearchCriteriaFactory
    {
        public static SoegInputType1 Create()
        {
            var inp = new SoegInputType1()
            {
                SoegObjekt = new SoegObjektType()
                {
                    BrugervendtNoegleTekst = null,
                    SoegAttributListe = new SoegAttributListeType()
                    {
                        LokalUdvidelse = null,
                        SoegEgenskab = new SoegEgenskabType[]{
                            new SoegEgenskabType(){
                                NavnStruktur = new NavnStrukturType(){
                                    PersonNameStructure = new PersonNameStructureType(){
                                        //PersonGivenName = "Morten"
                                    }
                                }
                            }
                        },
                        SoegRegisterOplysning = new RegisterOplysningType[]{
                            new RegisterOplysningType(){
                                Item = new CprBorgerType(){
                                    FolkeregisterAdresse = new AdresseType(){
                                        Item = new DanskAdresseType(){
                                            AddressComplete = new AddressCompleteType(){
                                                AddressAccess = new AddressAccessType(){
                                                    StreetBuildingIdentifier = "1",
                                                    MunicipalityCode = null,
                                                    StreetCode = null
                                                },
                                                AddressPostal = new AddressPostalType(){
                                                    StreetBuildingIdentifier = "1",
                                                    StreetName = "Industrivej",
                                                    PostCodeIdentifier = "2600",
                                                    DistrictName = "Glostrup"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    SoegRegistrering = null,
                    SoegRelationListe = null,
                    SoegTilstandListe = null,
                    SoegVirkning = null,
                    UUID = null
                },
                FoersteResultatReference = "0",
                MaksimalAntalKvantitet = "100"
            };
            return inp;
        }
    }
}
