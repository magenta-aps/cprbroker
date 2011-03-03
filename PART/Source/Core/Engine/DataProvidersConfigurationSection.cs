using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CprBroker.Engine
{
    public class DataProvidersConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("knownTypes", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(TypeCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public TypeCollection Types
        {
            get
            {
                return (TypeCollection)this["knownTypes"];
            }
            set
            {
                this["knownTypes"] = value;
            }
        }

        public const string SectionName = "dataProviders";

        public static DataProvidersConfigurationSection GetCurrent()
        {
            var configFile = Utilities.Config.GetConfigFile();
            var group = configFile.SectionGroups[Utilities.Constants.DataProvidersSectionGroupName];
            if (group != null)
            {
                return group.Sections[SectionName] as DataProvidersConfigurationSection;
            }
            return null;
        }

        
    }

    public class TypeCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new TypeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as TypeElement).TypeName;
        }

        public TypeElement this[int index]
        {
            get
            {
                return (TypeElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public TypeElement this[string Name]
        {
            get
            {
                return (TypeElement)BaseGet(Name);
            }
        }

        public int IndexOf(TypeElement typeName)
        {
            return BaseIndexOf(typeName);
        }

        public void Add(TypeElement typeElement)
        {
            BaseAdd(typeElement);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(TypeElement typeElement)
        {
            if (BaseIndexOf(typeElement) >= 0)
                BaseRemove(typeElement.TypeName);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }

    }

    public class TypeElement : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true)]
        public string TypeName
        {
            get
            {
                return (string)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }
    }
}
