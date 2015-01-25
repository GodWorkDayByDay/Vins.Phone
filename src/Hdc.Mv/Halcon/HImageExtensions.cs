using System.Windows;
using HalconDotNet;
using Hdc.Windows;
using Hdc.Windows.Media.Imaging;

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

        public static HImage ReduceDomainForRing(this HImage hImage, double centerX, double centerY, double innerRadius,
                                          double outerRadius)
        {
            var innerCircle = new HRegion();
            innerCircle.GenCircle(centerY, centerX, innerRadius);

            var outerCircle = new HRegion();
            outerCircle.GenCircle(centerY, centerX, outerRadius);

            var ring = outerCircle.Difference(innerCircle);
            var reducedImage = hImage.ChangeDomain(ring);

            innerCircle.Dispose();
            outerCircle.Dispose();
            ring.Dispose();

//            reducedImage.CropDomain()
//                      .ToBitmapSource()
//                      .SaveToJpeg("_EnhanceEdgeArea_Domain.jpg");

            return reducedImage;
        }

        public static HImage PaintGrayOffset(this HImage imageSource, HImage imageDestination,
                                    int offsetRow, int offsetColumn)
        {
            HObject image;
            HDevelopExport.Singletone.PaintGrayOffset(imageSource, imageDestination, out image, offsetRow, offsetColumn);
            return new HImage(image);
        }
    }
}