using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class OpeningRectangle1RegionProcessor : IRegionProcessor
    {
        public HRegion Process(HRegion region)
        {
            var openingRectangle1 = region.OpeningRectangle1(Width, Height);
            return openingRectangle1;
        }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}