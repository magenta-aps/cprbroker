using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class IndividualResponseType
    {
        public RegistreringType1 ToRegistreringType1(Func<string, Guid> cpr2uuidFunc)
        {
            return new RegistreringType1()
            {
                AktoerRef = ToAktoerRefType(),
                AttributListe = ToAttributListeType(),
                CommentText = ToCommentText(),
                LivscyklusKode = ToLivscyklusKodeType(),
                RelationListe = ToRelationListeType(cpr2uuidFunc),
                Tidspunkt = ToTidspunktType(),
                TilstandListe = ToTilstandListeType(),
                Virkning = ToVirkningType()
            };
        }

        public UnikIdType ToAktoerRefType()
        {
            return UnikIdType.Create(Constants.ActorId);
        }

        public string ToCommentText()
        {
            return Constants.CommentText;
        }

        public static IList<IndividualResponseType> ParseBatch(string batchFileText)
        {
            var rd = new System.IO.StringReader(batchFileText);
            var lines = rd.ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return ParseBatch(lines, Constants.DataObjectMap);
        }

        public static new IList<IndividualResponseType> ParseBatch(string[] dataLines, Dictionary<string, Type> typeMap)
        {
            var allLineWrappers = Wrapper.ParseBatch(dataLines, typeMap);

            var startRecord = allLineWrappers.Where(w => w is StartRecordType).FirstOrDefault() as StartRecordType;
            var endRecord = allLineWrappers.Where(w => w is EndRecordType).FirstOrDefault() as EndRecordType;

            var relevantLines = allLineWrappers
                .Where(lineWrapper => !new Wrapper[] { startRecord, endRecord }.Contains(lineWrapper));

            var groupedWrapers = relevantLines
                .GroupBy(w => new LineWrapper(w.Contents).PNR);

            var ret = groupedWrapers
                .Select(individualWrappersGrouping =>
                    {
                        var individualLines = individualWrappersGrouping.ToList();
                        var individual = new IndividualResponseType();
                        individual.FillFrom(individualLines, startRecord, endRecord);
                        return individual;
                    }
            );
            return ret.ToList();
        }

    }
}

