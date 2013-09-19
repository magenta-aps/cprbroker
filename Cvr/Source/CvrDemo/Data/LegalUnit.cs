using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CvrDemo.Data
{
    public partial class LegalUnit
    {
        public ProductionUnit[] ActualProductionUnits
        {
            get
            {
                return this.ProductionUnits.Where(pu => pu != null).ToArray();
            }
        }
    }
}