using System.Windows;
using HalconDotNet;
using Hdc.Windows;

namespace Hdc.Mv.Halcon
{
    public static class HImageExtensions
    {
        public static int GetWidth(this HImage image)
        {
            int w, h;
            image.GetImageSize(out w, out h);
            return w;
        }

        public static int GetHeight(this HImage image)
        {
            int w, h;
            image.GetImageSize(out w, out h);
            return h;
        }

        public static Int32Size GetSize(this HImage image)
        {
            int w, h;
            image.GetImageSize(out w, out h);
            return new Int32Size(w, h);
        }
    }
}