using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;

namespace CprBroker.DBR.Extensions
{
    public static partial class CprConverterExtensions
    {
        public static PersonAddress ToDpr(this CurrentAddressWrapper currentAddress)
        {
            PersonAddress pa = new PersonAddress();
            pa.PNR = Decimal.Parse(currentAddress.CurrentAddressInformation.PNR);
            pa.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.Registration.RegistrationDate, 12);
            pa.MunicipalityCode = currentAddress.CurrentAddressInformation.MunicipalityCode;
            pa.StreetCode = currentAddress.CurrentAddressInformation.StreetCode;
            pa.HouseNumber = currentAddress.CurrentAddressInformation.HouseNumber;
            pa.Floor = currentAddress.CurrentAddressInformation.Floor;
            pa.DoorNumber = currentAddress.CurrentAddressInformation.Door;
            pa.GreenlandConstructionNumber = currentAddress.ClearWrittenAddress.BuildingNumber;
            pa.PostCode = currentAddress.ClearWrittenAddress.PostCode;
            pa.MunicipalityName = null; // CprBroker.Providers.CPRDirect.Authority.GetNameByCode(pa.MunicipalityCode.ToString());
            pa.StreetAddressingName = currentAddress.ClearWrittenAddress.StreetAddressingName;
            if (currentAddress.CurrentAddressInformation.StartDate != null)
            {
                pa.AddressStartDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.StartDate.Value, 8);
            }
            else
            {
                Console.WriteLine("currentAddress.CurrentAddressInformation.StartDate was NULL");
                pa.AddressStartDate = 0;
            }
            pa.AddressStartDateMarker = ' '; //Defines whether the address is trustable (updated from CPR) - default is ' ', which means that the address is being updated
            if (currentAddress.CurrentAddressInformation.EndDate != null)
            {
                pa.AddressEndDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.EndDate.Value, 8);
            }
            else
            {
                Console.WriteLine("currentAddress.CurrentAddressInformation.EndDate was NULL");
                pa.AddressEndDate = null;
            }
            pa.LeavingFromMunicipalityCode = currentAddress.CurrentAddressInformation.LeavingMunicipalityCode;
            if (currentAddress.CurrentAddressInformation.LeavingMunicipalityDepartureDate != null)
            {
                pa.LeavingFromMunicipalityDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.LeavingMunicipalityDepartureDate.Value, 12);
            }
            else
            {
                Console.WriteLine("currentAddress.CurrentAddressInformation.LeavingMunicipalityDepartureDate was NULL");
                pa.LeavingFromMunicipalityDate = null;
            }
            if (currentAddress.CurrentAddressInformation.MunicipalityArrivalDate != null)
            {
                pa.MunicipalityArrivalDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.MunicipalityArrivalDate.Value, 12);
            }
            else
            {
                Console.WriteLine("currentAddress.CurrentAddressInformation.MunicipalityArrivalDate was NULL");
                pa.MunicipalityArrivalDate = null;
            }
            pa.AlwaysNull1 = null;
            pa.AlwaysNull2 = null;
            pa.AlwaysNull3 = null;
            pa.AlwaysNull4 = null;
            pa.AlwaysNull5 = null;
            if (currentAddress.CurrentAddressInformation.StartDate != null)
            {
                pa.AdditionalAddressDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.StartDate.Value, 8);
            }
            else
            {
                Console.WriteLine("currentAddress.CurrentAddressInformation.StartDate was NULL");
                pa.AdditionalAddressDate = null;
            }

            pa.CorrectionMarker = null; //This is the current status
            pa.CareOfName = currentAddress.CurrentAddressInformation.CareOfName;
            pa.Town = currentAddress.ClearWrittenAddress.CityName;
            pa.Location = currentAddress.ClearWrittenAddress.Location;
            pa.AdditionalAddressLine1 = currentAddress.CurrentAddressInformation.SupplementaryAddress1;
            pa.AdditionalAddressLine2 = currentAddress.CurrentAddressInformation.SupplementaryAddress2;
            pa.AdditionalAddressLine3 = currentAddress.CurrentAddressInformation.SupplementaryAddress3;
            pa.AdditionalAddressLine4 = currentAddress.CurrentAddressInformation.SupplementaryAddress4;
            pa.AdditionalAddressLine5 = currentAddress.CurrentAddressInformation.SupplementaryAddress5;
            return pa;
        }

        public static PersonAddress ToDpr(this HistoricalAddressType historicalAddress)
        {
            PersonAddress pa = new PersonAddress();
            pa.PNR = Decimal.Parse(historicalAddress.PNR);
            pa.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.Registration.RegistrationDate, 12);
            pa.MunicipalityCode = historicalAddress.MunicipalityCode;
            pa.StreetCode = historicalAddress.StreetCode;
            pa.HouseNumber = historicalAddress.HouseNumber;
            pa.Floor = historicalAddress.Floor;
            pa.DoorNumber = historicalAddress.Door;
            pa.GreenlandConstructionNumber = historicalAddress.BuildingNumber;
            pa.PostCode = 0; //Find in GeoLookup, based on streetCode and houseNumber
            pa.MunicipalityName = CprBroker.Providers.CPRDirect.Authority.GetNameByCode(pa.MunicipalityCode.ToString());
            pa.StreetAddressingName = null; //TODO: Can be fetched in CPR Services, vejadrnvn

            // TODO: Shall we use length 12 or 13?
            if (historicalAddress.RelocationDate.HasValue)
                pa.AddressStartDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.RelocationDate.Value, 12);

            pa.AddressStartDateMarker = historicalAddress.RelocationDateUncertainty;

            if (historicalAddress.LeavingDate.HasValue)
                pa.AddressEndDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.LeavingDate.Value, 8);

            pa.LeavingFromMunicipalityCode = null; // Seems not available in historical records....
            pa.LeavingFromMunicipalityDate = null; // Seems not available in historical records....
            pa.MunicipalityArrivalDate = null;  //Seems only available for current address
            pa.AlwaysNull1 = null;
            pa.AlwaysNull2 = null;
            pa.AlwaysNull3 = null;
            pa.AlwaysNull4 = null;
            pa.AlwaysNull5 = null;
            pa.AdditionalAddressDate = null; //TODO: Can be fetched in CPR Services, supladrhaenstart
            pa.CorrectionMarker = historicalAddress.CorrectionMarker;
            pa.CareOfName = historicalAddress.CareOfName;
            pa.Town = null; //Find in GoeLookup, based on street code and house number
            pa.Location = null; //Find in GoeLookup, based on street code and house number
            pa.AdditionalAddressLine1 = null; // Seems not available in historical records....
            pa.AdditionalAddressLine2 = null; // Seems not available in historical records....
            pa.AdditionalAddressLine3 = null; // Seems not available in historical records....
            pa.AdditionalAddressLine4 = null; // Seems not available in historical records....
            pa.AdditionalAddressLine5 = null; // Seems not available in historical records....
            return pa;
        }

    }
}
