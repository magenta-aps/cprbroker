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
using System.Data.Linq;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Data.Part
{
    /// <summary>
    /// Represents the ContactChannel table
    /// </summary>
    public partial class ContactChannel
    {
        public static KontaktKanalType ToXmlType(ContactChannel db)
        {
            if (db != null)
            {
                KontaktKanalType ret = new KontaktKanalType()
                {
                    BegraensetAnvendelseTekst = db.UsageLimits,
                    NoteTekst = db.Note,
                    Item = null
                };

                switch ((Part.ContactChannelType.ChannelTypes)db.ContactChannelTypeId)
                {
                    case ContactChannelType.ChannelTypes.Email:
                        ret.Item = db.Value;
                        break;
                    case ContactChannelType.ChannelTypes.Telephone:
                        ret.Item = new TelefonType()
                        {
                            KanBrugesTilSmsIndikator = db.CanSendSms.HasValue && db.CanSendSms.Value,
                            TelephoneNumberIdentifier = db.Value,
                        };
                        break;
                    case ContactChannelType.ChannelTypes.Other:
                        ret.Item = new AndenKontaktKanalType()
                        {
                            KontaktKanalTekst = db.Value,
                            NoteTekst = db.OtherNote,
                        };
                        break;
                }
                return ret;
            }
            return null;
        }

        public static ContactChannel FromXmlType(KontaktKanalType oio)
        {
            if (oio != null)
            {
                var ret = new ContactChannel()
                {
                    UsageLimits = oio.BegraensetAnvendelseTekst,
                    Note = oio.NoteTekst,
                };
                if (oio.Item is string)
                {
                    ret.Value = oio.Item as string;
                }
                else if (oio.Item is TelefonType)
                {
                    var tel = oio.Item as TelefonType;
                    ret.CanSendSms = tel.KanBrugesTilSmsIndikator;
                    ret.Value = tel.TelephoneNumberIdentifier;
                }
                else if (oio.Item is AndenKontaktKanalType)
                {
                    var other = oio.Item as AndenKontaktKanalType;
                    ret.Value = other.KontaktKanalTekst;
                    ret.OtherNote = other.NoteTekst;
                }
                return ret;
            }
            return null;
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<ContactChannel>(cc => cc.ContactChannelType);
        }
    }
}
