using System;
using HalconDotNet;
using Hdc.Mv.Halcon;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public abstract class RegionExtractor : IRegionExtractor
    {
        public string Name { get; set; }
        public bool SaveCacheImageEnabled { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }
        public double HalfWidth { get; set; }
        public double HalfHeight { get; set; }

        public HRegion Process(HImage image, HRegion domain)
        {
            //            var row1 = Convert.ToInt32(domain.RegionFeatures("row1"));
            //            var column1 = Convert.ToInt32(domain.RegionFeatures("column1"));
            var row1 = domain.GetRow1();
            var column1 = domain.GetColumn1();

            var reducedImage = image.ReduceDomain(domain);
            var croppedImage = reducedImage.CropDomain();

            var foundRegion = GetRegion(croppedImage);
            var offsetFoundRegion = foundRegion.MoveRegion(row1, column1);

            reducedImage.Dispose();
            croppedImage.Dispose();
            foundRegion.Dispose();

            return offsetFoundRegion;
        }

        public HRegion Process(HImage image)
        {
            var processRegion = new HRegion();
            processRegion.GenRectangle2(Y, X, Angle, HalfWidth, HalfHeight);

            return Process(image, processRegion);
        }

        protected abstract HRegion GetRegion(HImage image);
    }
}