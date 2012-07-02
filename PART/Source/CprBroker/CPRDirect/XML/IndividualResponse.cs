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

        public static IList<IndividualResponseType> ParseBatch(string[] dataLines, Dictionary<string, Type> typeMap)
        {
            var allLineWrappers = dataLines
                .Where(line => line.Length >= Constants.DataObjectCodeLength)
                .Select(dataLine => new LineWrapper(dataLine));

            var startRecord = allLineWrappers.Where(w => w.IntCode == Constants.StartRecordCode).First();
            var endRecord = allLineWrappers.Where(w => w.IntCode == Constants.EndRecordCode).First();

            var relevantLines = allLineWrappers
                .Where(lineWrapper => lineWrapper.IntCode != 0 && lineWrapper.IntCode != 999);

            var ret = relevantLines
                .GroupBy(lineWrapper => lineWrapper.PNR)
                .Select(
                individualLineGrouping =>
                {
                    var individualLines = individualLineGrouping.ToList();
                    individualLines.Insert(0, startRecord);
                    individualLines.Add(endRecord);
                    var individualWrappers = new List<Wrapper>();
                    foreach (var lineWrapper in individualLines)
                    {
                        if (typeMap.ContainsKey(lineWrapper.Code))
                        {
                            Type type = typeMap[lineWrapper.Code];
                            var wrapper = Utilities.Reflection.CreateInstance(type) as Wrapper;
                            lineWrapper.Contents = lineWrapper.Contents.PadRight(wrapper.Length);
                            wrapper.Contents = lineWrapper.Contents;
                            individualWrappers.Add(wrapper);
                        }
                    }

                    var individual = new IndividualResponseType();
                    individual.FillFrom(individualWrappers);
                    return individual;
                }
                );
            return ret.ToList();
        }

    }
}

