using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class ClosingCircleRegionProcessor : IRegionProcessor
    {
        public HRegion Process(HRegion region)
        {
            var closingCircle = region.ClosingCircle(Radius);
            return closingCircle;
        }

        public double Radius { get; set; }
    }
}