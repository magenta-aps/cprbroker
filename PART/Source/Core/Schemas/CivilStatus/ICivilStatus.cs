using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Util;

namespace CprBroker.Schemas.Part
{
    /// <summary>
    /// Common interface for CurrentCivilStatus and HistoricalCivilStatus
    /// </summary>
    public interface ICivilStatus : ITimedType
    {
        string PNR { get; }
        char CivilStatusCode { get; }

        string ToSpousePnr();

        bool IsValid();
    }

    public interface ISeparation
    {
        CivilStatusType ToCivilStatusType();
    }


}
