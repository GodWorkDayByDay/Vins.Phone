using System;
using System.Windows.Markup;
using HalconDotNet;
using Hdc.Mv.Inspection.Halcon;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    [ContentProperty("Rect")]
    public class GrayAndAreaRegionExtractor : IRegionExtractor
    {
        public GrayAndAreaRegionExtractor()
        {
        }

        public string Name { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }
        public double HalfWidth { get; set; }
        public double HalfHeight { get; set; } 

        public int MedianRadius { get; set; }
        public int EmpWidth { get; set; }
        public int EmpHeight { get; set; }
        public double EmpFactor { get; set; }
        public int ThresholdMinGray { get; set; }
        public int ThresholdMaxGray { get; set; }
        public int AreaMin { get; set; }
        public int AreaMax { get; set; }
        public double ClosingRadius { get; set; }
        public double DilationRadius { get; set; }
        public bool SaveCacheImageEnabled { get; set; }

        public HRegion Process(HImage image)
        {
            var processRegion = new HRegion();
            processRegion.GenRectangle2(Y, X, Angle, HalfWidth, HalfHeight);
            var reducedImage = image.ReduceDomain(processRegion);

            var foundRegion = reducedImage.GetRegionByGrayAndArea(MedianRadius, EmpWidth,
                EmpHeight, EmpFactor,
                ThresholdMinGray, ThresholdMaxGray, AreaMin, AreaMax,
                ClosingRadius, DilationRadius);

            reducedImage.Dispose();

            return foundRegion;
        }
    }
}