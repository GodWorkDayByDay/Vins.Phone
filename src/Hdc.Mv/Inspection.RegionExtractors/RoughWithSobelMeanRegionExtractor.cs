﻿using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class RoughWithSobelMeanRegionExtractor : IRectangle2RegionExtractor
    {
        public double ThresholdMinGray { get; set; }
        public double ThresholdMaxGray { get; set; }
        public double MinMaxGrayPercent { get; set; }
        public int HatWidth { get; set; }
        public int HatHeight { get; set; }
        public double SelectAreaMin { get; set; }
        public double SelectAreaMax { get; set; }
        public int RoughAreaClosingWidth { get; set; }
        public int RoughAreaClosingHeight { get; set; }
        public int RoughAreaOpeningWidth { get; set; }
        public int RoughAreaOpeningHeight { get; set; }
        public int SobelAmpSize { get; set; }
        public double SobelAmpMeanMin { get; set; }
        public double SobelAmpThresholdMinGray { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }
        public double HalfWidth { get; set; }
        public double HalfHeight { get; set; }


        public HRegion Extract(HImage image)
        {
            HObject regionHObject;

            HDevelopExport.Singletone.GetRegionOfRoughWithSobelMean(image, out regionHObject,
                ThresholdMinGray, 
                ThresholdMaxGray, 
                MinMaxGrayPercent,
                HatWidth, HatHeight, 
                SelectAreaMin, 
                SelectAreaMax,
                RoughAreaClosingWidth, 
                RoughAreaClosingHeight, 
                RoughAreaOpeningWidth,
                RoughAreaOpeningHeight, 
                SobelAmpSize, 
                SobelAmpMeanMin,
                SobelAmpThresholdMinGray);

            return new HRegion(regionHObject);
        }

        public HRegion Extract(HImage image, HRegion domain)
        {
            throw new NotImplementedException();
        }

        public string Name { get; set; }
        public bool SaveCacheImageEnabled { get; set; }
        public IRectangle2Def RelativeRect { get; set; }
    }
}