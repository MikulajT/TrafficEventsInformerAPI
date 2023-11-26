using System.Xml.Serialization;

namespace TrafficEventsInformer.Models.UsersRoute
{
    [XmlRoot(ElementName = "trk")]
    public class Trk
    {

        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "trkseg")]
        public Trkseg Trkseg { get; set; }
    }
}
