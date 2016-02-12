using System;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace TilesDavis.Wpf.Util
{
    public static class ColorExtensions
    {
        private const string HexFormat = "X2";
        public static string ToHexString(this Color color) => string.Format("#{0}{1}{2}", color.R.ToString(HexFormat), color.G.ToString(HexFormat), color.B.ToString(HexFormat));

        public static Color Get(this ImmersiveColors immersiveColor)
        {
            IntPtr pElementName = Marshal.StringToHGlobalUni(immersiveColor.ToString());
            var colourset = StarScreenColorsHelper.GetImmersiveUserColorSetPreference(false, false);
            uint type = StarScreenColorsHelper.GetImmersiveColorTypeFromName(pElementName);
            Marshal.FreeCoTaskMem(pElementName);
            uint colourdword = StarScreenColorsHelper.GetImmersiveColorFromColorSetEx((uint)colourset, type, false, 0);
            byte[] colourbytes = new byte[4];
            colourbytes[0] = (byte)((0xFF000000 & colourdword) >> 24); // A
            colourbytes[1] = (byte)((0x00FF0000 & colourdword) >> 16); // B
            colourbytes[2] = (byte)((0x0000FF00 & colourdword) >> 8); // G
            colourbytes[3] = (byte)(0x000000FF & colourdword); // R
            Color color = Color.FromArgb(colourbytes[0], colourbytes[3], colourbytes[2], colourbytes[1]);

            return color;
        }

    }
    public static class StringExtensions
    {
        public static string Truncate(this string value, int length)
        {
            return value?.Substring(0, Math.Min(value.Length, length));
        }
    }
}