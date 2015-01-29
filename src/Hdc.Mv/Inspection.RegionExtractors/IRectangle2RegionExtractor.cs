using System.Security.Cryptography.X509Certificates;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    public interface IRectangle2RegionExtractor : IGeneralRegionExtractor, IRectangle2Def
    {
//        HRegion Process(HImage image);

        HRegion Extract(HImage image, HRegion domain);

        string Name { get; set; }

        bool SaveCacheImageEnabled { get; set; }

//        double RelativeCenterX { get; set; }
//        double RelativeCenterY { get; set; }
//        double RelativeAngle { get; set; }

        IRectangle2Def RelativeRect { get; set; }
//        Rectangle2Def Domain { get; set; }
//        HRegion Process(HImage image, HRegion domain);
    }

//    public interface RegionExtractor
//    {
//        HRegion Process(HImage image);
//        IRegionExtractor Extractor { get; set; }
//    }
}