using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class IndividualResponseType
    {
        public RegistreringType1 ToRegistreringType1(Func<string, Guid> cpr2uuidFunc, DateTime effectDate)
        {
            var ret = new RegistreringType1()
            {
                AktoerRef = ToAktoerRefType(),
                AttributListe = ToAttributListeType(effectDate),
                CommentText = ToCommentText(),
                LivscyklusKode = ToLivscyklusKodeType(),
                RelationListe = ToRelationListeType(cpr2uuidFunc),
                Tidspunkt = ToTidspunktType(),
                TilstandListe = ToTilstandListeType(),
                Virkning = null
            };
            ret.CalculateVirkning();
            return ret;
        }

        public UnikIdType ToAktoerRefType()
        {
            return UnikIdType.Create(Constants.ActorId);
        }

        public string ToCommentText()
        {
            return Constants.CommentText;
        }

    }
}

