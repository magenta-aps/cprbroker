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
            //var logonInp = File.ReadAllText(@"..\..\OrdinaryLogon.xml", Constants.XmlEncoding);
            //var adr3Inp = File.ReadAllText(@"..\..\ADRSOG1.xml", Constants.XmlEncoding);
            string token = "Sfm70b7s";

            var prov = new CprServicesDataProvider() { Address = "https://gctp-demo.cpr.dk/cpr-online-gctp/gctp", UserId = "WUS00082", Password = "Mag2Cpr#" };
            //token = prov.SignonAndGetToken();
            //var ss = prov.ChangePassword("Mag2Cpr#");

            prov.CallADRESSE3(token, "1312814435");
            return;
            prov.CallADRSOG1(token, new SoegAttributListeType()
            {
                SoegEgenskab = new SoegEgenskabType[]{ 
                    new SoegEgenskabType(){
                        NavnStruktur =new NavnStrukturType(){
                            PersonNameForAddressingName = "Beemen"
                        }}}
            });
        }
    }
}
