using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CprBroker.Config.Properties
{
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
    {

#if Mono
        static Settings()
        {            
            defaultInstance = new Settings();
            defaultInstance.Reload();                
        }
#endif

        public bool CustomImplementation { get; set; }

        private Dictionary<string, object> _Values;
        public Dictionary<string, object> Values
        {
            get
            {
                if (_Values == null)
                {
                    _Values = new Dictionary<string, object>();
                    var def = Settings.defaultInstance;
                    SettingsProperty[] props = new SettingsProperty[def.Properties.Count];
                    Settings.Default.Properties.CopyTo(props, 0);
                    _Values = props.ToDictionary(p => p.Name, p => def[p.Name]);
                }
                return _Values;
            }
        }

        public override object this[string propertyName]
        {
            get
            {
                if (CustomImplementation)
                {
                    return _Values[propertyName];
                }
                else
                {
                    return base[propertyName];
                }
            }
            set
            {
                if (CustomImplementation)
                {
                    base[propertyName] = value;
                }
                else
                {
                    base[propertyName] = value;
                }
            }
        }

    }
}
