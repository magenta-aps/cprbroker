using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CprServices
{
    public class Program
    {
        public static void Main()
        {
            var p = new Program();
            p.Run();
        }

        public void Run()
        {
            var prov = new CprServicesDataProvider() { Address = "https://gctp-demo.cpr.dk/cpr-online-gctp/gctp", UserId = "WUS00082", Password = "Mag2Cpr#" };

            //var ss = prov.ChangePassword("Mag2Cpr#");

            var res = prov.SearchList(new SoegInputType1()
            {
                SoegObjekt = new SoegObjektType()
                {
                    SoegAttributListe = new
                        SoegAttributListeType()
                        {
                            SoegEgenskab = new SoegEgenskabType[]{ 
                    new SoegEgenskabType(){
                        NavnStruktur =new NavnStrukturType(){
                            PersonNameForAddressingName = "Morten Hansen"
                        }}},
                            SoegRegisterOplysning = new RegisterOplysningType[] { 
                    new RegisterOplysningType(){
                        Item = new CprBorgerType(){
                            FolkeregisterAdresse = new AdresseType(){
                                Item = new DanskAdresseType(){
                                    AddressComplete = new AddressCompleteType(){
                                        AddressAccess = new AddressAccessType(){
                                            //MunicipalityCode = "0851",
                                            //StreetCode="8512"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                        }
                }
            });
            object o = res;
        }
    }
}
