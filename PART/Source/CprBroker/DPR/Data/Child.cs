using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CprBroker.Schemas;

namespace CprBroker.Providers.DPR
{
    /// <summary>
    /// Represents the DTBOERN table
    /// </summary>
    public partial class Child
    {
        internal static readonly Expression<Func<decimal, decimal, DPRDataContext, IQueryable<PersonTotal>>> PersonChildrenExpression = (effectTime, pnr, dataContext) =>
            from child in dataContext.Childs
            join personTotal in dataContext.PersonTotals on child.ChildPNR equals personTotal.PNR
            where child.ParentPNR == pnr && personTotal.DateOfBirth >= effectTime
            select personTotal;
    }
}
