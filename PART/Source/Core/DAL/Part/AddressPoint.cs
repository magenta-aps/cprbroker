using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class AddressPoint
    {
        public static AddressPointType ToXmlType(AddressPoint db)
        {
            if (db != null)
            {
                return new AddressPointType()
                {
                    AddressPointIdentifier = db.Identifier,
                    AddressPointStatusStructure = AddressPointStatus.ToXmlType(db.AddressPointStatus),
                    GeographicPointLocation = GeographicPointLocation.ToXmlType(db.GeographicPointLocation)
                };
            }
            return null;
        }

        public static AddressPoint FromXmlType(AddressPointType oio)
        {
            if (oio != null)
            {
                return new AddressPoint
               {
                   AddressPointStatus = AddressPointStatus.FromXmlType(oio.AddressPointStatusStructure),
                   GeographicPointLocation = GeographicPointLocation.FromXmlType(oio.GeographicPointLocation),
                   Identifier = oio.AddressPointIdentifier,
               };
            }
            return null;
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<AddressPoint>(ap => ap.AddressPointStatus);
            loadOptions.LoadWith<AddressPoint>(ap => ap.GeographicPointLocation);

            AddressPointStatus.SetChildLoadOptions(loadOptions);
            GeographicPointLocation.SetChildLoadOptions(loadOptions);
        }
    }
}
