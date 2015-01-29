using HalconDotNet;

namespace Hdc.Mv.Inspection
{
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