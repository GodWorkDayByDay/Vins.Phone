using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    public class DilationCircleRegionProcessor : IRegionProcessor
    {
        public HRegion Process(HRegion region)
        {
            var dilation = region.DilationCircle(Radius);
            return dilation;
        }

        public double Radius { get; set; }
    }
}