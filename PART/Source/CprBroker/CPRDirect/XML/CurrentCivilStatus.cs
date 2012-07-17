using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class CurrentCivilStatusType : ICivilStatus
    {
        string ICivilStatus.ToSpousePnr()
        {
            return Converters.ToPnrStringOrNull(this.SpousePNR);
        }

        DateTime? ICivilStatus.ToCivilStatusStartDate()
        {
            return Converters.ToDateTime(this.CivilStatusStartDate, this.CivilStatusStartDateUncertainty);
        }

        DateTime? ICivilStatus.ToCivilStatusEndDate()
        {
            return null;
        }

        bool ICivilStatus.IsValid()
        {
            return !string.IsNullOrEmpty((this as ICivilStatus).ToSpousePnr());
        }
    }

    public partial class HistoricalCivilStatusType : ICivilStatus
    {
        bool ICivilStatus.IsValid()
        {
            return Converters.IsValidCorrectionMarker(this.CorrectionMarker);
        }

        string ICivilStatus.ToSpousePnr()
        {
            return Converters.ToPnrStringOrNull(this.SpousePNR);
        }

        DateTime? ICivilStatus.ToCivilStatusStartDate()
        {
            return Converters.ToDateTime(this.CivilStatusStartDate, this.CivilStatusStartDateUncertainty);
        }

        DateTime? ICivilStatus.ToCivilStatusEndDate()
        {
            return Converters.ToDateTime(this.CivilStatusEndDate, this.CivilStatusEndDateUncertainty);
        }
    }

    public partial class CurrentSeparationType : ISeparation
    {

    }
}
