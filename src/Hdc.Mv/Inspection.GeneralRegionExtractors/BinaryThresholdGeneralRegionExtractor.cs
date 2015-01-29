using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class BinaryThresholdGeneralRegionExtractor : IGeneralRegionExtractor
    {
        public HRegion Extract(HImage image)
        {
            int usedThreshold;
            var region = image.BinaryThreshold("max_separability", LightDark.ToHalconString(), out usedThreshold);
            return region;
        }

        public LightDark LightDark { get; set; }
    }
}