using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class GeographicPointLocation
    {
        public static GeographicPointLocationType ToXmlType(GeographicPointLocation db)
        {
            if (db != null)
            {
                return new GeographicPointLocationType()
                {
                    crsIdentifier = db.CrsIdentifier,
                    GeographicCoordinateTuple = GeographicCoorditaneTuple.ToXmlType(db.GeographicCoorditaneTuple),
                };
            }
            return null;
        }

        public static GeographicPointLocation FromXmlType(GeographicPointLocationType oio)
        {
            if (oio != null)
            {
                return new GeographicPointLocation()
                {
                    CrsIdentifier = oio.crsIdentifier,
                    GeographicCoorditaneTuple = GeographicCoorditaneTuple.FromXmlType(oio.GeographicCoordinateTuple),
                };
            }
            return null;
        }

    }
}
