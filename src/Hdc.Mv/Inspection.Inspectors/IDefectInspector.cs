using System.Collections.Generic;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    public interface IDefectInspector
    {
        IList<DefectResult> SearchDefects(HImage image, IList<DefectDefinition> defectDefinitions,
                                          IList<SurfaceResult> surfaceResults);
    }
}