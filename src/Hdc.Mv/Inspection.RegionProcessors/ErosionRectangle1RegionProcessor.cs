using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    public class ErosionRectangle1RegionProcessor : IRegionProcessor
    {
        public HRegion Process(HRegion region)
        {
            var dilation = region.ErosionRectangle1(Width, Height);
            return dilation;
        }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}