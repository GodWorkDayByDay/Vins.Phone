using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    public interface IGeneralRegionExtractor
    {
        HRegion Extract(HImage image);
    }
}