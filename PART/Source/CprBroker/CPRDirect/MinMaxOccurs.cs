using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MinMaxOccurs : Attribute
    {
        public MinMaxOccurs()
            : this(1, 1)
        { }
        public MinMaxOccurs(int min, int max)
        {
            if (min > max)
            {
                throw new ArgumentException(string.Format("Parameter min <{0}> must be less than or equal to max <{1}>"));
            }
            MinOccurs = min;
            MaxOccurs = max;
        }

        public int MinOccurs { get; set; }
        public int MaxOccurs { get; set; }

        public void ValidateSingleObject(Wrapper w)
        {
            var actualOccurs = (w == null) ? 0 : 1;
            ValidateCount(actualOccurs);
        }

        public void ValidateList(IList<Wrapper> w)
        {
            if (w.Where(a => a == null).Count() > 0)
            {
                throw new ArgumentNullException();
            }
            var actualOccurs = w.Count;
            ValidateCount(actualOccurs);
        }

        public void ValidateCount(int actualCount)
        {
            if (actualCount < MinOccurs || actualCount > MaxOccurs)
            {
                throw new ArgumentOutOfRangeException(
                    "Occurences",
                    actualCount,
                    string.Format("Should be between <{0}> and <{1}>", MinOccurs, MaxOccurs)
                    );
            }
        }
    }
}
