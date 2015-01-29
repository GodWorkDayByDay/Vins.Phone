using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    public class ErosionCircleRegionProcessor : IRegionProcessor
    {
        public HRegion Process(HRegion region)
        {
            var dilation = region.ErosionCircle(Radius);
            return dilation;
        }

        public double Radius { get; set; }
    }
}