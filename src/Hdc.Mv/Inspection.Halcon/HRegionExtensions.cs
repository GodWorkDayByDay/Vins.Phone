﻿using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection.Halcon
{
    public static class HRegionExtensions
    {
        public static int GetWidth(this HRegion region)
        {
            var value = region.RegionFeatures("width");
            var int32 = Convert.ToInt32(value);
            return int32;
        }

        public static int GetHeight(this HRegion region)
        {
            var value = region.RegionFeatures("height");
            var int32Value = Convert.ToInt32(value);
            return int32Value;
        }

        public static int GetRow1(this HRegion region)
        {
            var value = region.RegionFeatures("row1");
            var int32Value = Convert.ToInt32(value);
            return int32Value;
        }

        public static int GetColumn1(this HRegion region)
        {
            var value = region.RegionFeatures("column1");
            var int32Value = Convert.ToInt32(value);
            return int32Value;
        }

        public static int GetRow(this HRegion region)
        {
            var value = region.RegionFeatures("row");
            var int32Value = Convert.ToInt32(value);
            return int32Value;
        }

        public static int GetColumn(this HRegion region)
        {
            var value = region.RegionFeatures("column");
            var int32Value = Convert.ToInt32(value);
            return int32Value;
        }

        public static int GetRow2(this HRegion region)
        {
            var value = region.RegionFeatures("row2");
            var int32Value = Convert.ToInt32(value);
            return int32Value;
        }

        public static int GetColumn2(this HRegion region)
        {
            var value = region.RegionFeatures("column2");
            var int32Value = Convert.ToInt32(value);
            return int32Value;
        }
    }
}