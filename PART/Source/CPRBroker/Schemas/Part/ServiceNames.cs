using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part
{
    public static class ServiceNames
    {
        public class Part
        {
            public const string Service = "Part";

            public static class Methods
            {
                public const string Read = "Read";
                public const string List = "List";
                public const string Search = "Search";
            }
        }
    }
}
