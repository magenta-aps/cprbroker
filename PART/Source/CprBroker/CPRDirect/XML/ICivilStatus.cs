using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;
using CprBroker.Schemas.Util;

namespace CprBroker.Providers.CPRDirect
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

    }

    public partial class CurrentCivilStatusType : ICivilStatus
    {
        DateTime? ICivilStatus.CivilStatusEndDate
        {
            get { return null; }
        }

        char ICivilStatus.CivilStatusEndDateUncertainty
        {
            get { return '*'; }
        }
    }

    public partial class HistoricalCivilStatusType : ICivilStatus
    {

    }
}
