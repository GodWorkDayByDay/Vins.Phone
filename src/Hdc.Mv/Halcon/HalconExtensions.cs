using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HalconDotNet;

namespace Hdc.Mv.Halcon
{
    public static class HalconExtensions
    {
        public static HImage To8BppHImage(this ImageInfo imageInfo)
        {
            var hImage = new HImage("byte", imageInfo.PixelWidth, imageInfo.PixelHeight, imageInfo.BufferPtr);
            return hImage;
        }

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

        public static void HobjectToHimage(this HObject hobject, ref HImage image)
        {
            HTuple pointer, type, width, height;

            HOperatorSet.GetImagePointer1(hobject, out pointer, out type, out width, out height);
            image.GenImage1(type, width, height, pointer);
        }
    }
}