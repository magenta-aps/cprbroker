using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using CprBroker.Schemas.Part;
using CprBroker.Engine;
using CprBroker.Utilities;

namespace CprBroker.Providers.Local.Search
{
    public partial class LocalSearchDataProvider : IPartSearchDataProvider
    {

        private static Expression<Func<PersonSearchCache, bool>> AddNamePredicates(Expression<Func<PersonSearchCache, bool>> pred, NavnStrukturType prop)
        {
            if (!string.IsNullOrEmpty(prop.PersonNameForAddressingName))
            {
                pred = pred.And((pt) => pt.AddressingName == prop.PersonNameForAddressingName);
            }
            if (!string.IsNullOrEmpty(prop.KaldenavnTekst))
            {
                pred = pred.And((pt) => pt.NickName == prop.KaldenavnTekst);
            }
            if (!string.IsNullOrEmpty(prop.NoteTekst))
            {
                pred = pred.And((pt) => pt.Note == prop.NoteTekst);
            }
            if (prop.PersonNameStructure != null)
            {
                // Search by name
                var name = prop.PersonNameStructure;
                if (!name.IsEmpty)
                {
                    if (!string.IsNullOrEmpty(name.PersonGivenName))
                    {
                        pred = pred.And((pt) => pt.PersonGivenName == name.PersonGivenName);
                    }
                    if (!string.IsNullOrEmpty(name.PersonMiddleName))
                    {
                        pred = pred.And((pt) => pt.PersonMiddleName == name.PersonMiddleName);
                    }
                    if (!string.IsNullOrEmpty(name.PersonSurnameName))
                    {
                        pred = pred.And((pt) => pt.PersonSurnameName == name.PersonSurnameName);
                    }
                }
            }
            return pred;
        }
    }
}