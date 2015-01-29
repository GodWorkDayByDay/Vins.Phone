using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class OpeningCircleRegionProcessor : IRegionProcessor
    {
        public HRegion Process(HRegion region)
        {
            var openingCircle = region.OpeningCircle(Radius);
            return openingCircle;
        }

        public double Radius { get; set; }
    }
}