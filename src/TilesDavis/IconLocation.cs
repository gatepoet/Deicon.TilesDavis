using System;

namespace TilesDavis.Core
{
    public class IconLocation
    {
        public IconLocation(string filename, int index = 0)
        {
            Filename = Environment.ExpandEnvironmentVariables(filename);
            Index = index;
        }
        public string Filename { get; private set; }
        public int Index { get; private set; }

        public string ToComString()
        {
            return $"{Filename},{Index}";
        }

        public static IconLocation ParseComString(string comString)
        {
            var values = comString.Split(',');
            return new IconLocation(values[0], int.Parse(values[1]));
        }
    }
}