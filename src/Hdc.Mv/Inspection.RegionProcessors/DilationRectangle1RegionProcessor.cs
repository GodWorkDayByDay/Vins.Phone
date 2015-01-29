using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    public class DilationRectangle1RegionProcessor : IRegionProcessor
    {
        public HRegion Process(HRegion region)
        {
            var dilation = region.DilationRectangle1(Width, Height);
            return dilation;
        }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}