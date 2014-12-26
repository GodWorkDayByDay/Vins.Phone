using HalconDotNet;

namespace Hdc.Mv.Inspection.Halcon
{
    public static class ImageInfoExtensions
    {
        public static HImage To8BppHImage(this ImageInfo imageInfo)
        {
            var hImage = new HImage("byte", imageInfo.PixelWidth, imageInfo.PixelHeight, imageInfo.BufferPtr);
            return hImage;
        }
    }
}