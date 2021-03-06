﻿using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class DilationCircleRegionProcessor : IRegionProcessor
    {
        public HRegion Process(HRegion region)
        {
            if (Math.Abs(Radius) < 0.0000001)
                return region.MoveRegion(0, 0);

            var dilation = region.DilationCircle(Radius);
            return dilation;
        }

        public double Radius { get; set; }
    }
}