using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CprBroker.Schemas;

namespace CprBroker.Providers.DPR
{
    public partial class PersonName
    {
        internal static Expression<Func<DPRDataContext, IQueryable<PersonName>>> ActivePersonsExpression =
            (context) =>
                from pn in context.PersonNames
                where pn.NameTerminationDate == null
                select pn;

        public SimpleCPRPersonType ToSimpleCprPerson()
        {
            SimpleCPRPersonType simpleCPRPerson = new SimpleCPRPersonType();
            simpleCPRPerson.PersonCivilRegistrationIdentifier = Convert.ToInt64(PNR).ToString("D10");
            simpleCPRPerson.PersonNameStructure = new PersonNameStructureType(FirstName, LastName);
            return simpleCPRPerson;
        }
    }
}
