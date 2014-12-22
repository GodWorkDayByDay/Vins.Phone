using System;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class DistanceBetweenLinesDefinition
    {
        public string Name { get; set; }
        public string Line1Name { get; set; }
        public string Line2Name { get; set; }
    }
}