using System.Xml.Serialization;

namespace TilesDavis.Core
{
    public enum ShowName
    {
        [XmlEnum(Name = "on")]
        On,
        [XmlEnum(Name = "off")]
        Off
    }
}