using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class ContactChannel
    {
        public KontaktKanalType ToXmlType()
        {
            KontaktKanalType ret = new KontaktKanalType()
            {
                BegraensetAnvendelseTekst = UsageLimits,
                NoteTekst = Note,
                Item = null
            };

            switch ((Part.ContactChannelType.ChannelTypes)ContactChannelTypeId)
            {
                case ContactChannelType.ChannelTypes.Email:
                    ret.Item = Value;
                    break;
                case ContactChannelType.ChannelTypes.Telephone:
                    ret.Item = new TelefonType()
                    {
                        KanBrugesTilSmsIndikator = CanSendSms.HasValue && CanSendSms.Value,
                        TelephoneNumberIdentifier = Value,
                    };
                    break;
                case ContactChannelType.ChannelTypes.Other:
                    ret.Item = new AndenKontaktKanalType()
                    {
                        KontaktKanalTekst = Value,
                        NoteTekst = OtherNote,
                    };
                    break;
            }
            return ret;
        }

        public static ContactChannel FromXmlType(KontaktKanalType oio)
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
    }
}
