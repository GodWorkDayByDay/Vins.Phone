using System;
using System.Windows.Markup;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    [ContentProperty("ThresholdImageFilter")]
    public class DynThresholdRegionExtractor : IRegionExtractor
    {
        public HRegion Extract(HImage image)
        {
//            image.WriteImage("tiff", 0, @"B:\Test_Ori");

            HImage preprocessImage;
            if (PreprocessFilter != null)
                preprocessImage = PreprocessFilter.Process(image);
            else
            {
                preprocessImage = image;
            }
//            preprocessImage.WriteImage("tiff", 0, @"B:\Test_preprocessImage");

            var thresholdImage = ThresholdImageFilter.Process(preprocessImage);
//            thresholdImage.WriteImage("tiff", 0, @"B:\Test_thresholdImage");

            HRegion region = preprocessImage.DynThreshold(
                thresholdImage,
                Offset,
                LightDark.ToHalconString());

            preprocessImage.Dispose();
            thresholdImage.Dispose();

            return region;
        }

        public string Name { get; set; }

        public IImageFilter PreprocessFilter { get; set; }
        public IImageFilter ThresholdImageFilter { get; set; }
        public double Offset { get; set; }
        public LightDark LightDark { get; set; }
    }
}