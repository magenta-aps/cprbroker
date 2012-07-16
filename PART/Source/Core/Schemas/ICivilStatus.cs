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
    public interface ICivilStatus
    {
        string PNR { get; }
        string SpousePNR { get; }
        char CivilStatus { get; }

        char CivilStatusStartDateUncertainty { get; }
        DateTime? CivilStatusStartDate { get; }
        char CivilStatusEndDateUncertainty { get; }
        DateTime? CivilStatusEndDate { get; }

        DateTime? ToCivilStatusDate();
        string ToSpousePnr();

        bool IsValid();
    }

    public interface ISeparation
    {
        CivilStatusType ToCivilStatusType();
    }

    
}
