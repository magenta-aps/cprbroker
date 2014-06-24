using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Config
{
    public class ConfigManager
    {
        private static IConfigProvider _Current;

        public static IConfigProvider Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new StandardConfigProvider();
                }
                return _Current;
            }
            set
            {
                _Current = value;
            }
        }

    }
}
