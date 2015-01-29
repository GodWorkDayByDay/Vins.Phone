using System;
using HalconDotNet;
using Hdc.Mv.Halcon;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public abstract class Rectangle2RegionExtractor : IRectangle2RegionExtractor
    {
        public string Name { get; set; }
        public bool SaveCacheImageEnabled { get; set; }
        public IRectangle2Def RelativeRect { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }
        public double HalfWidth { get; set; }
        public double HalfHeight { get; set; }

        public virtual HRegion Extract(HImage image, HRegion domain)
        {
            //            var row1 = Convert.ToInt32(domain.RegionFeatures("row1"));
            //            var column1 = Convert.ToInt32(domain.RegionFeatures("column1"));
            var row1 = domain.GetRow1();
            var column1 = domain.GetColumn1();
            var row2 = domain.GetRow2();
            var column2 = domain.GetColumn2();

            var reducedImage = image.ChangeDomain(domain);
            var croppedImage = reducedImage.CropDomain();

            var foundRegion = GetRegion(croppedImage);
            var offsetFoundRegion = foundRegion.MoveRegion(row1, column1);

            reducedImage.Dispose();
            croppedImage.Dispose();
            foundRegion.Dispose();

            return offsetFoundRegion;
        }

        public HRegion Extract(HImage image)
        {
            var processRegion = new HRegion();
            processRegion.GenRectangle2(Y, X, Angle, HalfWidth, HalfHeight);

            return Extract(image, processRegion);
        }

        protected abstract HRegion GetRegion(HImage image);
    }
}