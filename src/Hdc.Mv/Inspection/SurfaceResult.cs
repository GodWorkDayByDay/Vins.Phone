using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    public class SurfaceResult
    {
        public SurfaceDefinition Definition { get; set; }

        public HRegion IncludeRegion { get; set; }

        public HRegion ExcludeRegion { get; set; }
    }
}