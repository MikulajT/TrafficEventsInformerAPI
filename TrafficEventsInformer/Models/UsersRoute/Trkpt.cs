using System.Xml.Serialization;

namespace TrafficEventsInformer.Models.UsersRoute
{
    [XmlRoot(ElementName = "trkpt")]
    public class Trkpt
    {

        [XmlElement(ElementName = "ele")]
        public double Ele { get; set; }

        [XmlAttribute(AttributeName = "lat")]
        public double Lat { get; set; }

        [XmlAttribute(AttributeName = "lon")]
        public double Lon { get; set; }
    }
}
