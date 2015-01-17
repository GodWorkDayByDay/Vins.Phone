using System;

namespace Hdc.Mv.Inspection
{
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