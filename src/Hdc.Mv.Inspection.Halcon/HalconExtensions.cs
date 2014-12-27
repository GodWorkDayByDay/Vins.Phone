using System;
using System.Runtime.InteropServices;
using System.Windows;
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
    }
}