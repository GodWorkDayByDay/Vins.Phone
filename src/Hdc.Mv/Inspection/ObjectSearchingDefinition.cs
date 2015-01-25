using System;
using System.Windows;

namespace Hdc.Mv.Inspection
{
    // ReSharper disable InconsistentNaming
    [Serializable]
    public class ObjectSearchingDefinition
    {
        public string Name { get; set; }
        public bool Domain_SaveCacheImageEnabled { get; set; }

        public Line RoiRelativeLine { get; set; }
        public double RoiRelativeLineWidth { get; set; }

        public Line AreaRelativeLine { get; set; }
        public double AreaRelativeLineWidth { get; set; }

        public IRegionExtractor RegionExtractor { get; set; }
        public bool RegionExtractor_Disabled { get; set; }
        public bool RegionExtractor_SaveCacheImageEnabled { get; set; }

        public IImageFilter ImageFilter { get; set; }
        public bool ImageFilter_Disabled { get; set; }
        public bool ImageFilter_SaveCacheImageEnabled { get; set; }

        public IRegionExtractor AreaExtractor { get; set; }
        public IRegionExtractor ObjectExtractor { get; set; }
    }
}

// ReSharper restore InconsistentNaming