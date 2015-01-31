using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class ProcessedRegionExtractor : IRegionExtractor
    {
        public HRegion Extract(HImage image)
        {
            var region = RegionExtractor.Extract(image);

            HRegion processedRegion;
            if (RegionProcessor != null)
            {
                processedRegion = RegionProcessor.Process(region);
                region.Dispose();
            }
            else
            {
                processedRegion = region;
            }
            return processedRegion;
        }

        public IRegionExtractor RegionExtractor { get; set; }
        public IRegionProcessor RegionProcessor { get; set; }
    }
}