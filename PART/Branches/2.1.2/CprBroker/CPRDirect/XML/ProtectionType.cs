/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{

    public partial class ProtectionType : ITimedType
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
                && Utilities.Dates.DateRangeIncludes(this.ToStartTS(), this.ToEndTS(), effectDate, true);
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

        public DataTypeTags Tag
        {
            get
            {
                switch (this.ProtectionCategoryCode)
                {
                    case ProtectionCategoryCodes.LocalDirectory:
                        return DataTypeTags.LocalDirectoryProtection;
                    case ProtectionCategoryCodes.Marketing:
                        return DataTypeTags.MarketingProtection;
                    case ProtectionCategoryCodes.NameAndAddress:
                        return DataTypeTags.NameAndAddressProtection;
                    case ProtectionCategoryCodes.Research:
                        return DataTypeTags.ResearchProtection;
                    default:
                        return DataTypeTags.None;
                }
            }
        }

        public DateTime? ToStartTS()
        {
            return this.StartDate;
        }

        public DateTime? ToEndTS()
        {
            return this.EndDate;
        }


    }
}
