using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using System.Linq;

namespace TilesDavis.Wpf.Util
{
    public static class IconExtensions
    {
        public static BitmapSource ToBitmapSource(this Icon icon)
        {
            if (icon == null)
                return null;

            using (var stream = new MemoryStream())
            {
                using (icon) icon.Save(stream);
                var ico = new IconBitmapDecoder(
                    stream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);

                return ico.Frames.First();
            }
        }
    }
}