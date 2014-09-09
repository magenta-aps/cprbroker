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

            if (currentAddress.CurrentAddressInformation.RelocationDate.HasValue)
                pa.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.RelocationDate.Value, 12);
            pa.MunicipalityCode = currentAddress.CurrentAddressInformation.MunicipalityCode;
            pa.StreetCode = currentAddress.CurrentAddressInformation.StreetCode;
            pa.HouseNumber = currentAddress.CurrentAddressInformation.HouseNumber;

            if (!string.IsNullOrEmpty(currentAddress.CurrentAddressInformation.Floor))
                pa.Floor = currentAddress.CurrentAddressInformation.Floor;
            else
                pa.Floor = null;

            if (!string.IsNullOrEmpty(currentAddress.CurrentAddressInformation.Door))
            {
                if (currentAddress.CurrentAddressInformation.Door.Equals("th") || currentAddress.CurrentAddressInformation.Door.Equals("tv"))
                    pa.DoorNumber = "  " + currentAddress.CurrentAddressInformation.Door;
                else
                    pa.DoorNumber = currentAddress.CurrentAddressInformation.Door;
            }
            else
                pa.DoorNumber = null;
            if (!string.IsNullOrEmpty(currentAddress.ClearWrittenAddress.BuildingNumber))
                pa.GreenlandConstructionNumber = currentAddress.ClearWrittenAddress.BuildingNumber;
            else
                pa.GreenlandConstructionNumber = null;

            pa.PostCode = currentAddress.ClearWrittenAddress.PostCode;
            pa.MunicipalityName = CprBroker.Providers.CPRDirect.Authority.GetAuthorityNameByCode(pa.MunicipalityCode.ToString());

            if (!string.IsNullOrEmpty(currentAddress.ClearWrittenAddress.StreetAddressingName))
                pa.StreetAddressingName = currentAddress.ClearWrittenAddress.StreetAddressingName;
            else
                pa.StreetAddressingName = null;

            if (currentAddress.CurrentAddressInformation.RelocationDate.Value != null)
            {
                pa.AddressStartDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.RelocationDate.Value, 12);
            }
            else
            {
                pa.AddressStartDate = 0;
            }
            if (!char.IsWhiteSpace(currentAddress.CurrentAddressInformation.RelocationDateUncertainty))
                pa.AddressStartDateMarker = currentAddress.CurrentAddressInformation.RelocationDateUncertainty;
            pa.AddressEndDate = null; // This is the current date
            if (currentAddress.CurrentAddressInformation.LeavingMunicipalityCode > 0)
                pa.LeavingFromMunicipalityCode = currentAddress.CurrentAddressInformation.LeavingMunicipalityCode;
            else
                pa.LeavingFromMunicipalityCode = null;
            if (currentAddress.CurrentAddressInformation.LeavingMunicipalityDepartureDate != null)
            {
                pa.LeavingFromMunicipalityDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.LeavingMunicipalityDepartureDate.Value, 12);
            }
            else
            {
                pa.LeavingFromMunicipalityDate = null;
            }
            if (currentAddress.CurrentAddressInformation.MunicipalityArrivalDate != null)
            {
                pa.MunicipalityArrivalDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.MunicipalityArrivalDate.Value, 12);
            }
            else
            {
                pa.MunicipalityArrivalDate = null;
            }
            pa.AlwaysNull1 = null;
            pa.AlwaysNull2 = null;
            pa.AlwaysNull3 = null;
            pa.AlwaysNull4 = null;
            pa.AlwaysNull5 = null;
            if (currentAddress.CurrentAddressInformation.StartDate != null)
            {
                pa.AdditionalAddressDate = CprBroker.Utilities.Dates.DateToDecimal(currentAddress.CurrentAddressInformation.StartDate.Value, 12);
            }
            else
            {
                pa.AdditionalAddressDate = null;
            }
            pa.CorrectionMarker = null; //This is the current status
            if (!string.IsNullOrEmpty(currentAddress.CurrentAddressInformation.CareOfName))
                pa.CareOfName = currentAddress.CurrentAddressInformation.CareOfName;
            else
                pa.CareOfName = null;
            if (!string.IsNullOrEmpty(currentAddress.ClearWrittenAddress.CityName))
                pa.Town = currentAddress.ClearWrittenAddress.CityName;
            else
                pa.Town = null;
            if (!string.IsNullOrEmpty(currentAddress.ClearWrittenAddress.Location))
                pa.Location = currentAddress.ClearWrittenAddress.Location;
            else
                pa.Location = null;
            if (!string.IsNullOrEmpty(currentAddress.CurrentAddressInformation.SupplementaryAddress1))
                pa.AdditionalAddressLine1 = currentAddress.CurrentAddressInformation.SupplementaryAddress1;
            else
                pa.AdditionalAddressLine1 = null;
            if (!string.IsNullOrEmpty(currentAddress.CurrentAddressInformation.SupplementaryAddress2))
                pa.AdditionalAddressLine2 = currentAddress.CurrentAddressInformation.SupplementaryAddress2;
            else
                pa.AdditionalAddressLine2 = null;
            if (!string.IsNullOrEmpty(currentAddress.CurrentAddressInformation.SupplementaryAddress3))
                pa.AdditionalAddressLine3 = currentAddress.CurrentAddressInformation.SupplementaryAddress3;
            else
                pa.AdditionalAddressLine3 = null;
            if (!string.IsNullOrEmpty(currentAddress.CurrentAddressInformation.SupplementaryAddress4))
                pa.AdditionalAddressLine4 = currentAddress.CurrentAddressInformation.SupplementaryAddress4;
            else
                pa.AdditionalAddressLine4 = null;
            if (!string.IsNullOrEmpty(currentAddress.CurrentAddressInformation.SupplementaryAddress5))
                pa.AdditionalAddressLine5 = currentAddress.CurrentAddressInformation.SupplementaryAddress5;
            else
                pa.AdditionalAddressLine5 = null;
            return pa;
        }

        public static PersonAddress ToDpr(this HistoricalAddressType historicalAddress)
        {
            PersonAddress pa = new PersonAddress();
            pa.PNR = Decimal.Parse(historicalAddress.PNR);
            if (historicalAddress.RelocationDate.HasValue)
                pa.CprUpdateDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.RelocationDate.Value, 12);
            pa.MunicipalityCode = historicalAddress.MunicipalityCode;
            pa.StreetCode = historicalAddress.StreetCode;
            pa.HouseNumber = historicalAddress.HouseNumber;
            if (!string.IsNullOrEmpty(historicalAddress.Floor))
                pa.Floor = historicalAddress.Floor;
            else
                pa.Floor = null;
            if (!string.IsNullOrEmpty(historicalAddress.Door))
            {
                if (historicalAddress.Door.Equals("th") || historicalAddress.Door.Equals("tv"))
                    pa.DoorNumber = "  " + historicalAddress.Door;
                else
                    pa.DoorNumber = historicalAddress.Door;
            }
            else
                pa.DoorNumber = null;
            if (!string.IsNullOrEmpty(historicalAddress.BuildingNumber))
                pa.GreenlandConstructionNumber = historicalAddress.BuildingNumber;
            else
                pa.GreenlandConstructionNumber = null;
            pa.PostCode = 0; //Find in GeoLookup, based on streetCode and houseNumber

            pa.MunicipalityName = CprBroker.Providers.CPRDirect.Authority.GetAuthorityNameByCode(pa.MunicipalityCode.ToString());
            pa.StreetAddressingName = null; //TODO: Can be fetched in CPR Services, vejadrnvn

            // TODO: Shall we use length 12 or 13?
            if (historicalAddress.RelocationDate.HasValue)
                pa.AddressStartDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.RelocationDate.Value, 12);
            if (!char.IsWhiteSpace(historicalAddress.RelocationDateUncertainty))
                pa.AddressStartDateMarker = historicalAddress.RelocationDateUncertainty;
            if (historicalAddress.LeavingDate.HasValue)
                pa.AddressEndDate = CprBroker.Utilities.Dates.DateToDecimal(historicalAddress.LeavingDate.Value, 12);
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
            if (!string.IsNullOrEmpty(historicalAddress.CareOfName))
                pa.CareOfName = historicalAddress.CareOfName;
            else
                pa.CareOfName = null;
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
