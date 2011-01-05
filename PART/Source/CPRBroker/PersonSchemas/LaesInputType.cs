using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public partial class LaesInputType
    {
        public static LaesInputType Create(string uuid, ListInputType listInput)
        {
            return new LaesInputType()
            {
                RegistreringFraFilter = listInput.RegistreringFraFilter,
                RegistreringTilFilter = listInput.RegistreringTilFilter,
                UUID = uuid,
                VirkningFraFilter = listInput.VirkningFraFilter,
                VirkningTilFilter = listInput.VirkningTilFilter
            };
        }
    }
}
