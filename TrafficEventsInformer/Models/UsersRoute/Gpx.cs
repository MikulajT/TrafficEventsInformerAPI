using System.Xml.Serialization;

namespace TrafficEventsInformer.Models.UsersRoute
{
    [XmlRoot(ElementName = "gpx", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Gpx
    {

        [XmlElement(ElementName = "trk")]
        public Trk Trk { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        [XmlAttribute(AttributeName = "creator")]
        public string Creator { get; set; }
    }
}
