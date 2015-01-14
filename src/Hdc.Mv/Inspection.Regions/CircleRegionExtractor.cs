using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class CircleRegionExtractor : RegionExtractor
    {
        protected override HRegion GetRegion(HImage image)
        {
            var rect = new HRegion();
            rect.GenCircle(Y, X, Radius);
            return rect;
        }

        public double Radius { get; set; }
    }
}