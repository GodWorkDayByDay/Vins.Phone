using System.Collections.Generic;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    public interface ISurfaceInspector
    {
        IList<SurfaceResult> SearchSurfaces(HImage image, IList<SurfaceDefinition> surfaceDefinitions);
    }
}