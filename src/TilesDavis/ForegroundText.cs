using System;
using System.Xml.Serialization;

namespace TilesDavis.Core
{
    public enum ForegroundText
    {
        [XmlEnum(Name = "light")]
        Light,
        [XmlEnum(Name = "dark")]
        Dark
    }
}