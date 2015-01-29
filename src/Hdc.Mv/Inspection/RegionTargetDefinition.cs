using System;
// ReSharper disable InconsistentNaming



namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class RegionTargetDefinition
    {
        public string Name { get; set; }
        public Line RoiActualLine { get; set; }
        public Line RoiRelativeLine { get; set; }
        public double RoiHalfWidth { get; set; }
        public bool Domain_SaveCacheImageEnabled { get; set; }

        public IImageFilter AroundImageFilter { get; set; }
        public bool AroundImageFilter_SaveCacheImageEnabled { get; set; }

        public IRegionExtractor AroundRegionExtractor { get; set; }
        public bool AroundRegionExtractor_SaveCacheImageEnabled { get; set; }

        public IRegionProcessor AroundRegionProcessor { get; set; }
        public bool AroundRegionProcessor_SaveCacheImageEnabled { get; set; }

        public IImageFilter TargetImageFilter { get; set; }
        public bool TargetImageFilter_SaveCacheImageEnabled { get; set; }

        public IRegionExtractor TargetRegionExtractor { get; set; }
        public bool TargetRegionExtractor_SaveCacheImageEnabled { get; set; }

        public IRegionProcessor TargetRegionProcessor { get; set; }
        public bool TargetRegionProcessor_SaveCacheImageEnabled { get; set; }
    }
}

// ReSharper restore InconsistentNaming