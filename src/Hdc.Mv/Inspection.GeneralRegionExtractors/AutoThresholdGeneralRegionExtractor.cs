using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class AutoThresholdGeneralRegionExtractor : IGeneralRegionExtractor
    {
        public HRegion Extract(HImage image)
        {
            var region = image.AutoThreshold(Sigma);
            return region;
        }

        public double Sigma { get; set; }
    }
}