using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class AddressPointStatus
    {
        public static AddressPointStatusStructureType ToXmlType(AddressPointStatus db)
        {
            if (db != null)
            {
                new AddressPointStatusStructureType()
                {
                    AddressCoordinateQualityClassCode = db.AddressCoordinateQualityTypeCode.HasValue ? (AddressCoordinateQualityClassCodeType)Enum.Parse(typeof(AddressCoordinateQualityClassCodeType), db.AddressCoordinateQualityTypeCode.Value.ToString()) : AddressCoordinateQualityClassCodeType.A,
                    AddressCoordinateQualityClassCodeSpecified = db.AddressCoordinateQualityTypeCode.HasValue,
                    AddressPointRevisionDateTime = db.RevisionDate,
                    AddressPointValidEndDateTime = db.ValidEndDate.HasValue ? db.ValidEndDate.Value : new DateTime(),
                    AddressPointValidEndDateTimeSpecified = db.ValidEndDate.HasValue,
                    AddressPointValidStartDateTime = db.ValidStartDate.HasValue ? db.ValidStartDate.Value : new DateTime(),
                    AddressPointValidStartDateTimeSpecified = db.ValidStartDate.HasValue,
                };
            }
            return null;
        }

        public static AddressPointStatus FromXmlType(AddressPointStatusStructureType oio)
        {
            if (oio != null)
            {
                return new AddressPointStatus()
                {
                    AddressCoordinateQualityTypeCode = oio.AddressCoordinateQualityClassCodeSpecified ? oio.AddressCoordinateQualityClassCode.ToString()[0] : (char?)null,
                    RevisionDate = oio.AddressPointRevisionDateTime,
                    ValidEndDate = oio.AddressPointValidEndDateTimeSpecified ? oio.AddressPointValidEndDateTime : (DateTime?)null,
                    ValidStartDate = oio.AddressPointValidStartDateTimeSpecified ? oio.AddressPointValidStartDateTime : (DateTime?)null,
                };
            }
            return null;
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<AddressPointStatus>(aps => aps.AddressCoordinateQualityType);
        }
    }
}
