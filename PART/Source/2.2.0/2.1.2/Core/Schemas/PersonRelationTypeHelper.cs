using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public static class PersonRelationTypeHelper
    {
        public static TRelation Create<TRelation>(Guid targetUuid, DateTime? fromDate, DateTime? toDate) where TRelation : IPersonRelationType, new()
        {
            if (targetUuid != Guid.Empty)
            {
                return new TRelation()
                {
                    //TODO: Add comment text
                    CommentText = null,
                    CprNumber = null,
                    ReferenceID = UnikIdType.Create(targetUuid),
                    // TODO: Fill virkning object from parameters
                    Virkning = VirkningType.Create(fromDate, toDate)
                };
            }
            else
            {
                throw new ArgumentNullException("targetUuid");
            }
        }

        public static TRelation Create<TRelation>(string cprNumber, DateTime? fromDate, DateTime? toDate) where TRelation : IPersonRelationType, new()
        {
            if (!string.IsNullOrEmpty(cprNumber))
            {
                return new TRelation()
                {
                    //TODO: Add comment text
                    CommentText = null,
                    CprNumber = cprNumber,
                    ReferenceID = null,
                    // TODO: Fill virkning object from parameters
                    Virkning = VirkningType.Create(fromDate, toDate)
                };
            }
            else
            {
                throw new ArgumentNullException("cprNumber");
            }
        }

        //TODO: add parameters for from and to dates
        public static TRelation[] CreateList<TRelation>(params Guid[] targetUuids) where TRelation : IPersonRelationType, new()
        {
            return Array.ConvertAll<Guid, TRelation>
            (
                targetUuids,
                (uuid) => Create<TRelation>(uuid, null, null)
            );
        }

        public static TRelation[] CreateList<TRelation>(params string[] cprNumbers) where TRelation : IPersonRelationType, new()
        {
            return Array.ConvertAll<string, TRelation>
            (
                cprNumbers,
                (cprNumber) => Create<TRelation>(cprNumber, null, null)
            );
        }
    }
}
