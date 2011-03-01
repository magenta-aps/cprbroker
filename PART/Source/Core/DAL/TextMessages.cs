using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Data
{
    public static class TextMessages
    {
        public static readonly string NameOrTokenAlreadyExists = "Application name or token already exists";
        public static readonly string CannotDeleteApplicationBecauseItHasSubscriptions = "Cannot delete application because it has subscriptions";
    }
}
