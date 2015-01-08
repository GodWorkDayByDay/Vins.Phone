using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    public interface IRegionExtractor : IRectangle2Def
    {
        HRegion Process(HImage image);

        string Name { get; set; }

        bool SaveCacheImageEnabled { get; set; }

//        Rectangle2Def Domain { get; set; }
//        HRegion Process(HImage image, HRegion domain);
    }

//    public interface RegionExtractor
//    {
//        HRegion Process(HImage image);
//        IRegionExtractor Extractor { get; set; }
//    }
}