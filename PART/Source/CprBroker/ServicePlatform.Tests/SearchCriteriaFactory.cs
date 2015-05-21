using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.ServicePlatform
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
                                        PersonGivenName = "uffe",
                                        //PersonSurnameName = "Ulling"
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
                                                    // This field (HUSNR) is repeated later, but should pass the tests
                                                    StreetBuildingIdentifier = "91",
                                                    MunicipalityCode = null,
                                                    StreetCode = null
                                                },
                                                //Industrivænget 91 1 tv 9000 Aalborg
                                                AddressPostal = new AddressPostalType(){
                                                    StreetBuildingIdentifier = "0091",
                                                    StreetName = "Industrivænget",
                                                    PostCodeIdentifier = "9000",
                                                    DistrictName = "Aalborg",
                                                    //DistrictSubdivisionIdentifier = "1",
                                                    //FloorIdentifier = "1",
                                                    //SuiteIdentifier = "tv"
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
