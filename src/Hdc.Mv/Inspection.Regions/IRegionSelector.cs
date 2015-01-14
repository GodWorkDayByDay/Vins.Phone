using System.Collections;
using System.Collections.Generic;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    public interface IRegionSelector
    {
        HRegion SelectRegion(HRegion region);
    }


}