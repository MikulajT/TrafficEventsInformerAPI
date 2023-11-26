using System.Xml.Serialization;

namespace TrafficEventsInformer.Models.UsersRoute
{
    [XmlRoot(ElementName = "trkseg")]
    public class Trkseg
    {

        [XmlElement(ElementName = "trkpt")]
        public List<Trkpt> Trkpt { get; set; }
    }
}
