using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part
{
    public class Effect<T>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public T Value { get; set; }

        public Effect()
        {}

        public Effect(DateTime? startDate, DateTime? endDate, T value)
        {
            StartDate = startDate;
            EndDate = endDate;
            Value = value;
        }
    }
}
