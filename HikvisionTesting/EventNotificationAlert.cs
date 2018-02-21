using System;
using System.Collections.Generic;
using System.Text;

namespace HikvisionTesting
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.hikvision.com/ver20/XMLSchema", IsNullable = false)]
    public class EventNotificationAlert
    {
        public string ipAddress { get; set; }

        public int portNo { get; set; }

        public string protocol { get; set; }

        public string macAddress { get; set; }

        public int channelID { get; set; }

        public string dateTime { get; set; }

        public int activePostCount { get; set; }

        //public string eventType { get; set; }
        public EventType eventType { get; set; }

        public string eventState { get; set; }

        public string eventDescription { get; set; }
    }
}