using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CPRBroker.Schemas;

namespace CPRBroker.Providers.DPR
{
    public partial class Child
    {
        internal static readonly Expression<Func<decimal,decimal, DPRDataContext, IQueryable< PersonTotal>>> PersonChildrenExpression = (effectTime,pnr, dataContext) =>
            from child in dataContext.Childs
            join personTotal in dataContext.PersonTotals on child.ChildPNR equals personTotal.PNR
            where child.ParentPNR ==pnr &&  personTotal.DateOfBirth >= effectTime
            select personTotal;

        internal static readonly Expression<Func<decimal, DPRDataContext, IQueryable<PersonTotal>>> PersonParentsExpression = (pnr, dataContext) =>
            from child in dataContext.Childs
            join personTotal in dataContext.PersonTotals on child.ChildPNR equals personTotal.PNR
            where child.ChildPNR == pnr 
            select personTotal;

        public ChildRelationshipType ToChildRelationship(PersonName personName)
        {
            return new ChildRelationshipType()
            {
                SimpleCPRPerson = personName.ToSimpleCprPerson()
            };
        }

    }
}
