using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part
{
    public class PersonRelations
    {
        public Effect<PersonRelation> [] Parents { get; set; }
        public Effect<PersonRelation> [] Children { get; set; }
        public Effect<PersonRelation>[] Spouses { get; set; }
        public Effect<PersonRelation>[] SubstituteFor { get; set; }
        public Effect<PersonRelation> ReplacedBy { get; set; }
    }
}
