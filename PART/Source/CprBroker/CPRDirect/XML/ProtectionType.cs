using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{

    public partial class ProtectionType
    {
        public enum ProtectionCategoryCodes
        {
            NameAndAddress = 1,
            LocalDirectory = 2,
            Marketing = 3,
            Research = 4
        }

        public ProtectionCategoryCodes ProtectionCategoryCode
        {
            get { return (ProtectionCategoryCodes)this.ProtectionType_; }
            set { this.ProtectionType_ = (decimal)value; }
        }

        public bool HasProtection(DateTime effectDate, params ProtectionCategoryCodes[] categories)
        {
            // Remove time part because start and end dates contain only date parts
            effectDate = effectDate.Date;
            return
                categories.Contains(this.ProtectionCategoryCode)
                && Utilities.Dates.DateRangeIncludes(this.StartDate, this.EndDate, effectDate, true);
        }

        public VirkningType ToVirkningType()
        {
            return VirkningType.Create(this.StartDate, this.EndDate);
        }

        public static bool HasProtection(IList<ProtectionType> protectionRecords, DateTime effectDate, ProtectionCategoryCodes category)
        {
            return protectionRecords
                .Where(p => p.HasProtection(effectDate, category))
                .FirstOrDefault()
                != null;
        }

        public static VirkningType[] ToVirkningTypeArray(IList<ProtectionType> protectionRecords, DateTime effectDate, params ProtectionCategoryCodes[] categories)
        {
            return protectionRecords
                .Where(p => p.HasProtection(effectDate, categories))
                .Select(p => p.ToVirkningType())
                .ToArray();
        }
    }
}
