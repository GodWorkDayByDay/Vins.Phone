using System;
using System.Collections.ObjectModel;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class SharpSelectorSeries : Collection<SharpSelector>, IRegionSelector
    {
        public HRegion SelectRegion(HRegion region)
        {
            HRegion selectedRegion = region;
            foreach (var item in Items)
            {
                selectedRegion = item.SelectRegion(selectedRegion);
            }
            return selectedRegion;
        }
    }
}