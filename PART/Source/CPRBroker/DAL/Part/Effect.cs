using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class Effect
    {
        public static VirkningType ToVirkningType(Effect db)
        {
            return new VirkningType()
            {
                AktoerRef = ActorRef.ToXmlType(db.ActorRef),

                CommentText = db.CommentText,
                FraTidspunkt = TidspunktType.Create(db.FromDate),
                TilTidspunkt = TidspunktType.Create(db.ToDate)
            };
        }

        public static TilstandVirkningType ToTilstandVirkningType(Effect db)
        {
            return new TilstandVirkningType()
            {
                AktoerRef = ActorRef.ToXmlType(db.ActorRef),
                CommentText = db.CommentText,
                FraTidspunkt = TidspunktType.Create(db.FromDate),
            };
        }

        public static Effect FromVirkningType(VirkningType virkning)
        {
            if (virkning != null)
            {
                return new Effect()
                {
                    ActorRef = ActorRef.FromXmlType(virkning.AktoerRef),

                    CommentText = virkning.CommentText,
                    FromDate = virkning.FraTidspunkt.ToDateTime(),
                    ToDate = virkning.TilTidspunkt.ToDateTime()
                };
            }
            return null;
        }

        public static Effect FromTilstandVirkningType(TilstandVirkningType virkning)
        {
            if (virkning != null)
            {
                return new Effect()
                {
                    ActorRef = ActorRef.FromXmlType(virkning.AktoerRef),
                    CommentText = virkning.CommentText,
                    FromDate = virkning.FraTidspunkt.ToDateTime(),
                    ToDate = null
                };
            }
            return null;
        }

    }
}
