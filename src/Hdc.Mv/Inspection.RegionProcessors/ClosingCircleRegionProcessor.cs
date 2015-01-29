using HalconDotNet;

namespace Hdc.Mv.Inspection
{
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