﻿using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class ThresholdGeneralRegionExtractor : IGeneralRegionExtractor
    {
        public HRegion Extract(HImage image)
        {
            var region = image.Threshold(MinGray, MaxGray);
            return region;
        }

        public double MinGray { get; set; }

        public double MaxGray { get; set; }
    }
}