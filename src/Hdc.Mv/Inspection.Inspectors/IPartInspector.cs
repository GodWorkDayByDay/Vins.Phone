using System.Collections;
using System.Collections.Generic;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    public interface IPartInspector
    {
        IList<PartSearchingResult> SearchParts(HImage image, IList<PartSearchingDefinition> definitions);
    }
}