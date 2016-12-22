using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.DBR.Extensions
{
    partial class CprConverterExtensions
    {
        public static IndividualResponseType ToChangedPnrAdjustedIndividualResponse(this IndividualResponseType person)
        {
            var wrappers = person.GetChildrenAsType<Wrapper>();
            var ret = new IndividualResponseType();
            ret.FillPropertiesFromWrappers(wrappers);

            var currentPnr = decimal.Parse(
                string.Format("0{0}", ret.PersonInformation.CurrentCprNumber)
                );

            if (currentPnr > 0m)
            {
                // Clear historical records
                ret.HistoricalAddress.Clear();
                ret.HistoricalChurchInformation.Clear();
                ret.HistoricalCitizenship.Clear();
                ret.HistoricalCivilStatus.Clear();
                ret.HistoricalDeparture.Clear();
                ret.HistoricalDisappearance.Clear();
                ret.HistoricalName.Clear();
                ret.HistoricalSeparation.Clear();

                // This one stays the same
                //ret.HistoricalPNR.Clear();
            }
            return ret;
        }
    }
}
