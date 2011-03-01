using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Data.Part
{
    /// <summary>
    /// Represents the GeographicCoorditaneTuple table
    /// </summary>
    public partial class GeographicCoorditaneTuple
    {
        public static GeographicCoordinateTupleType ToXmlType(GeographicCoorditaneTuple db)
        {
            if (db != null)
            {
                return new GeographicCoordinateTupleType()
                {
                    GeographicEastingMeasure = db.Easting,
                    GeographicHeightMeasure = db.Height.HasValue ? db.Height.Value : 0,
                    GeographicHeightMeasureSpecified = db.Height.HasValue,
                    GeographicNorthingMeasure = db.Northing,
                };
            }
            return null;
        }

        public static GeographicCoorditaneTuple FromXmlType(GeographicCoordinateTupleType oio)
        {
            if (oio != null)
            {
                return new GeographicCoorditaneTuple()
                {
                    Easting = oio.GeographicEastingMeasure,
                    Northing = oio.GeographicNorthingMeasure,
                    Height = oio.GeographicHeightMeasureSpecified ? oio.GeographicHeightMeasure : (decimal?)null
                };
            }
            return null;
        }
    }
}
