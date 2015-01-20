using System;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class SurfaceDefinition
    {
        public SurfaceDefinition()
        {
            IncludeRegions = new RegionExtractorCollection();

            ExcludeRegions = new RegionExtractorCollection();
        }

        public string Name { get; set; }

        public string GroupName { get; set; }

        public bool SaveCacheImageEnabled { get; set; }

        public bool SaveAllCacheImageEnabled { get; set; }

        public RegionExtractorCollection ExcludeRegions { get; set; }

        public RegionExtractorCollection IncludeRegions { get; set; }
    }
}