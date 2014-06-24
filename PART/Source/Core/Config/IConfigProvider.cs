using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Config
{
    public interface IConfigProvider
    {
        Properties.Settings Settings { get; }
    }
}
