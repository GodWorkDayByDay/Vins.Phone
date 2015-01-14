using System;
using System.Collections.Generic;
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

    [Serializable]
    public class SharpSelector : Collection<SharpSelectorEntry>, IRegionSelector
    {
        public HRegion SelectRegion(HRegion region)
        {
            HTuple features = new HTuple();
            HTuple min = new HTuple();
            HTuple max = new HTuple();

            string operation = Operation.ToHalconString();

            HRegion selectedRegion = region;

            foreach (var entry in Items)
            {
                var feature = entry.Feature.ToHalconString();
                features.Append(feature);

                min.Append(entry.Min);
                max.Append(entry.Max);
            }

            region = selectedRegion.SelectShape(features, operation, min, max);
            return region;
        }

        public LogicOperation Operation { get; set; }
    }

    [Serializable]
    public class SharpSelectorEntry
    {
        public SharpSelectorEntry()
        {
        }

        public SharpSelectorEntry(SharpFeature feature, double min, double max)
        {
            Feature = feature;
            Min = min;
            Max = max;
        }

        public SharpFeature Feature { get; set; }

        public double Min { get; set; }
        public double Max { get; set; }
    }
}