using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class ClosingRectangle1RegionProcessor : IRegionProcessor
    {
        public HRegion Process(HRegion region)
        {
            var closingRectangle1 = region.ClosingRectangle1(Width, Height);
            return closingRectangle1;
        }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}