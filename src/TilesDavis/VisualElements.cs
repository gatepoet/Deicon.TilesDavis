using System.Xml;
using System.Xml.Serialization;

namespace TilesDavis.Core
{
    public class VisualElements
    {
        [XmlAttribute]
        public string BackgroundColor { get; set; } = IconBackgroundColor.Transparent;

        [XmlAttribute]
        public ShowName ShowNameOnSquare150x150Logo { get; set; } = ShowName.On;

        [XmlAttribute]
        public ForegroundText ForegroundText { get; set; } = ForegroundText.Light;

        [XmlAttribute]
        public string Square150x150Logo { get; set; }

        [XmlAttribute]
        public string Square70x70Logo { get; set; }
    }
}