using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas
{
    /// <summary>
    /// Levels of data quality
    /// </summary>
    public enum QualityLevel
    {
        Cpr,
        DataProvider,
        LocalCache
    }

    /// <summary>
    /// Used to pass the data quality level to web service clients
    /// </summary>
    public class QualityHeader : System.Web.Services.Protocols.SoapHeader
    {
        public QualityLevel? QualityLevel;
    }
}
