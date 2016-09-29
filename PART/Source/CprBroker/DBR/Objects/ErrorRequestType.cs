using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CprBroker.DBR
{
    public class ErrorRequestType : NewRquestType
    {
        public ErrorRequestType(string contents)
        {

        }

        public override DiversionResponseType Process(string dprConnectionString)
        {
            DiversionResponseType ret = null;

            if (Contents.Length < 12)
            {
                ret = new ClassicResponseType() { Contents = "00999999999999Fejl i kaldet - Færre end 12 tegn." };
            }
            else if (!Regex.Match(Contents, @"\A[0-9]{12}").Success)
            {
                ret = new ClassicResponseType() { Contents = "Fejl i kaldet - første 12 tegn ikke numeriske." };
            }
            else if (!PartInterface.Strings.IsValidPersonNumber(this.PNR))
            {
                ret = new ClassicResponseType()
                {
                    Type = this.Type,
                    LargeData = this.LargeData,
                    ErrorNumber = "40",
                    PNR = this.PNR,
                    Data = " Personnummer forkert opbygget. Kald afvist."
                };

            }
            else if (!Regex.Match(this.Contents, @"\A[013]").Success)
            {
                ret = new ClassicResponseType()
                {
                    Type = this.Type,
                    LargeData = this.LargeData,
                    ErrorNumber = "10",
                    PNR = this.PNR,
                    Data = " ABON_TYPE ukendt"
                };
            }
            else if (!Regex.Match(this.Contents, @"\A[013][01]").Success)
            {
                ret = new ClassicResponseType() { Contents = "Tom Codepath i sSvarGammelType. Stordata = " + (int)this.LargeData };
            }
            else
            {
                // Other input validation errors 
            }

            return ret;
        }
    }
}
