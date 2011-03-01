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
