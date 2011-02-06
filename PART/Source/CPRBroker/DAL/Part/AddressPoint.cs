using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class AddressPoint
    {
        public AddressPointType ToXmlType()
        {
            return new AddressPointType()
            {
                AddressPointIdentifier = Identifier,
                AddressPointStatusStructure = new AddressPointStatusStructureType()
                {
                    AddressCoordinateQualityClassCode = this.CoordinateQualityTypeCode.HasValue ? (AddressCoordinateQualityClassCodeType)Enum.Parse(typeof(AddressCoordinateQualityClassCodeType), CoordinateQualityTypeCode.Value.ToString()) : AddressCoordinateQualityClassCodeType.A,
                    AddressCoordinateQualityClassCodeSpecified = CoordinateQualityTypeCode.HasValue,
                    AddressPointRevisionDateTime = RevisionDate,
                    AddressPointValidEndDateTime = ValidEndDate.HasValue ? ValidEndDate.Value : new DateTime(),
                    AddressPointValidEndDateTimeSpecified = ValidEndDate.HasValue,
                    AddressPointValidStartDateTime = ValidStartDate.HasValue ? ValidStartDate.Value : new DateTime(),
                    AddressPointValidStartDateTimeSpecified = ValidStartDate.HasValue,
                },
                GeographicPointLocation = new GeographicPointLocationType()
                {
                    crsIdentifier = CrsIdentifier,
                    GeographicCoordinateTuple = new GeographicCoordinateTupleType()
                    {
                        GeographicEastingMeasure = Easting,
                        GeographicHeightMeasure = Height.HasValue ? Height.Value : 0,
                        GeographicHeightMeasureSpecified = Height.HasValue,
                        GeographicNorthingMeasure = Northing
                    }
                }
            };
        }

        public static AddressPoint FromXmlType(AddressPointType oio)
        {
            return new AddressPoint
           {
               CoordinateQualityTypeCode = oio.AddressPointStatusStructure.AddressCoordinateQualityClassCodeSpecified ? oio.AddressPointStatusStructure.AddressCoordinateQualityClassCode.ToString()[0] : (char?)null,
               Easting = oio.GeographicPointLocation.GeographicCoordinateTuple.GeographicEastingMeasure,
               Height = oio.GeographicPointLocation.GeographicCoordinateTuple.GeographicHeightMeasure,
               CrsIdentifier = oio.GeographicPointLocation.crsIdentifier,
               Identifier = oio.AddressPointIdentifier,
               Northing = oio.GeographicPointLocation.GeographicCoordinateTuple.GeographicNorthingMeasure,
               RevisionDate = oio.AddressPointStatusStructure.AddressPointRevisionDateTime,
               ValidEndDate = oio.AddressPointStatusStructure.AddressPointValidEndDateTimeSpecified ? oio.AddressPointStatusStructure.AddressPointValidEndDateTime : (DateTime?)null,
               ValidStartDate = oio.AddressPointStatusStructure.AddressPointValidStartDateTimeSpecified ? oio.AddressPointStatusStructure.AddressPointValidStartDateTime : (DateTime?)null,
           };
        }
    }
}
