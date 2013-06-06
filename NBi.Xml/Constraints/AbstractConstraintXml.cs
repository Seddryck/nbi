﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using NBi.Xml.Items;
using NBi.Xml.Settings;

namespace NBi.Xml.Constraints
{
    public abstract class AbstractConstraintXml
    {
        public AbstractConstraintXml()
        {
            Default = new DefaultXml();
            Settings = new SettingsXml();
            Logs = new List<LogXml>();
        }

        private DefaultXml _default;
        [XmlIgnore()]
        public virtual DefaultXml Default
        {
            get { return _default; }
            set
            {
                _default = value;
                if (BaseItem != null)
                    BaseItem.Default = value;
            }
        }
        private SettingsXml settings;
        [XmlIgnore()]
        public virtual SettingsXml Settings
        {
            get { return settings; }
            set
            {
                settings = value;
                if (BaseItem != null)
                    BaseItem.Settings = value;
            }
        }

        [XmlElement("log")]
        public List<LogXml> Logs { get; set; }
        
        [XmlIgnore]
        public virtual BaseItem BaseItem 
        { 
            get {return null;} 
        }

        [XmlAttribute("not")]
        [DefaultValue(false)]
        public bool Not { get; set; }
    }
}
