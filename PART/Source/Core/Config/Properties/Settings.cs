using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Config.Properties 
{
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
    
#if Mono
        static Settings()
        {            
            defaultInstance = new Settings();
            defaultInstance.Reload();                
        }
#endif

    }
}
