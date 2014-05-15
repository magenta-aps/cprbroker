using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace CprBroker.Web.Controls
{
    public interface IControlWithDataSource
    {
        object DataSource { get; set; }
        Control NamingContainer { get; }
    }
    
}