using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    [ContentProperty("Extractor")]
    public class DefectDefinition
    {
        public DefectDefinition()
        {
            RegionReferences = new Collection<RegionReference>();
        }

        public string Name { get; set; }

        public bool SaveCacheImageEnabled { get; set; }

        public Collection<RegionReference> RegionReferences { get; set; }

//        public Collection<SurfaceDefinition> Surfaces { get; set; }

        public IDefectExtractor Extractor { get; set; }

//        public IRegionSelector Selector { get; set; }
    }
}