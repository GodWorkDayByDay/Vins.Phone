using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HalconDotNet;

namespace Hdc.Mv.Inspection.Halcon
{
    public static class HalconExtensions
    {
//        public static Point IntersectionWith(this Line sourceLine, Line targetLine)
//        {
//            return HDevelopExportHelper.IntersectionLines(sourceLine, targetLine);
//        }


        public static ImageInfo ToImageInfo(this HImage hImage)
        {
            int pixelHeight;
            int pixelWidth;
            string type;
            IntPtr intPtr = hImage.GetImagePointer1(out type, out pixelWidth, out pixelHeight);


            var stride = pixelWidth;
            int bufferSize = stride * pixelHeight;
            IntPtr bufferPtr = Marshal.AllocHGlobal(bufferSize);

            //            Marshal.Copy(new IntPtr[] { intPtr }, 0, bufferPtr, 0);
            var buffer = new byte[bufferSize];
            Marshal.Copy(intPtr, buffer, 0, bufferSize);
            Marshal.Copy(buffer, 0, bufferPtr, bufferSize);

            var bsi = new ImageInfo()
            {
                BitsPerPixel = 8,
                BufferPtr = bufferPtr,
                PixelHeight = pixelHeight,
                PixelWidth = pixelWidth
            };
            return bsi;
        }

        public static BitmapSource ToBitmapSource(this HImage hImage)
        {
            int pixelHeight;
            int pixelWidth;
            string type;
            IntPtr intPtr = hImage.GetImagePointer1(out type, out pixelWidth, out pixelHeight);

            var stride = pixelWidth;
            var size = stride * pixelHeight;

/*            int bufferSize = stride * pixelHeight;
            IntPtr bufferPtr = Marshal.AllocHGlobal(bufferSize);

            var buffer = new byte[bufferSize];
            Marshal.Copy(intPtr, buffer, 0, bufferSize);
            Marshal.Copy(buffer, 0, bufferPtr, bufferSize);

            var bs = BitmapSource.Create(
            pixelWidth, pixelHeight,
            96, 96,
            PixelFormats.Gray8,
            BitmapPalettes.Gray256,
            bufferPtr,
            size,
            stride);*/

            var bs = BitmapSource.Create(
            pixelWidth, pixelHeight,
            96, 96,
            PixelFormats.Gray8,
            BitmapPalettes.Gray256,
            intPtr,
            size,
            stride);

            return bs;
        }

        public static HImage EnhanceEdgeArea4(this HImage image,
              int meanMaskWidth, int meanMaskHeight, int firstMinGray, int firstMaxGray,
            Order order,
              int empMaskWidth, int empMaskHeight, double empMaskFactor, int lastMinGray,
              int lastMaxGray)
        {
            HObject enhancedImage = null;
            HObject region = null;

            string orderString = null;
            if (order == Order.Increase) orderString = "true";
            if (order == Order.Decrease) orderString = "false";

            HDevelopExport.Singletone.EnhanceEdgeArea4(
                image,
                out enhancedImage,
                out region,
                meanMaskWidth,
                meanMaskHeight,
                firstMinGray,
                firstMaxGray,
                orderString,
                empMaskWidth,
                empMaskHeight,
                empMaskFactor,
                lastMinGray,
                lastMaxGray
                );

            region.Dispose();

            return new HImage(enhancedImage);
        }

        public static HRegion GetRegionByGrayAndArea(this HImage image, 
                                           int medianRadius,
                                           int empWidth, int empHeight, double empFactor,
                                           int thresholdMinGray, int thresholdMaxGray,
                                           int areaMin, int areaMax,
                                           double closingRadius, double dilationRadius)
        {
            HObject foundRegionObject;

            HDevelopExport.Singletone.GetRegionByGrayAndArea(image, out foundRegionObject, medianRadius,
                empWidth, empHeight, empFactor, thresholdMinGray, thresholdMaxGray, areaMin,
                areaMax,
                closingRadius, dilationRadius);

            return new HRegion(foundRegionObject);
        }
    }
}