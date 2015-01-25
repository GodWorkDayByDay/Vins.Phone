using System;
using System.Windows.Markup;

namespace Hdc.Mv.Inspection
{
    // ReSharper disable InconsistentNaming
    [Serializable]
    public class CircleSearchingDefinition
    {
        public string Name { get; set; }

        // General
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double InnerRadius { get; set; }
        public double OuterRadius { get; set; }
        public bool Domain_SaveCacheImageEnabled { get; set; }

        public double BaselineAngle { get; set; }
        public double BaselineX { get; set; }
        public double BaselineY { get; set; }

        public IRegionExtractor RegionExtractor { get; set; }
        public bool RegionExtractor_Disabled { get; set; }
        public bool RegionExtractor_SaveCacheImageEnabled { get; set; }

        public IImageFilter ImageFilter { get; set; }
        public bool ImageFilter_Disabled { get; set; }
        public bool ImageFilter_SaveCacheImageEnabled { get; set; }

        public ICircleExtractor CircleExtractor { get; set; }
    }

    // ReSharper restore InconsistentNaming
}