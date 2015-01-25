using System;
using HalconDotNet;
using Hdc.Mv.Halcon;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class MeanFilter : IImageFilter
    {
        public HImage Process(HImage image)
        {
            var domain = image.GetDomain();
            var domainWidth = domain.GetWidth();
            var domainHeight = domain.GetHeight();

            var maskWidth = MaskWidth == 0 ? domainWidth : MaskWidth;
            var maskHeight = MaskHeight == 0 ? domainHeight : MaskHeight;

            HImage enhancedImage = image.MeanImage(maskWidth, maskHeight);

            return enhancedImage;
        }

        public int MaskWidth { get; set; }
        public int MaskHeight { get; set; }
    }
}