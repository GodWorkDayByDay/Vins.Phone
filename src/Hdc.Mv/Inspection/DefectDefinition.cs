using System;
using System.Collections.ObjectModel;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class DefectDefinition
    {
        public string Name { get; set; }

        public Collection<RegionReference> RegionReferences { get; set; }

        public Collection<SurfaceDefinition> Surfaces { get; set; }

        public IDefectExtractor Extractor { get; set; }

        public IRegionSelector Selector { get; set; }
    }
}