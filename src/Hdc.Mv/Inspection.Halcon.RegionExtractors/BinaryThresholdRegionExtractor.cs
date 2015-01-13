using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class BinaryThresholdRegionExtractor : RegionExtractor
    {
        protected override HRegion GetRegion(HImage image)
        {
            if(!(LightDark==LightDark.Light || LightDark==LightDark.Dark))
                throw new InvalidOperationException("BinaryThresholdRegionExtractor.LightDark must be Light or Dark. Now is " + LightDark);

            HObject foundRegionObject;

            HDevelopExport.Singletone.GetRegionByBinaryThreshold(image,
                out foundRegionObject,
                MedianRadius,
                EmpWidth,
                EmpHeight,
                EmpFactor,
                LightDark.ToHalconString(),
                AreaMin,
                AreaMax,
                ClosingRadius,
                DilationRadius);

            var hRegion = new HRegion(foundRegionObject);
            return hRegion;
        }

        public int MedianRadius { get; set; }
        public int EmpWidth { get; set; }
        public int EmpHeight { get; set; }
        public double EmpFactor { get; set; }
        public LightDark LightDark { get; set; }
        public double AreaMin { get; set; }
        public double AreaMax { get; set; }
        public double ClosingRadius { get; set; }
        public double DilationRadius { get; set; }
    }
}