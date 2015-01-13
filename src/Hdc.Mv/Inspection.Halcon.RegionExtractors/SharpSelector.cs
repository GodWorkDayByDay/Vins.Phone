using System;
using System.Collections.ObjectModel;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class SharpSelector : Collection<SharpSelectorEntry>
    {
        
    }

    public class SharpSelectorEntry
    {
        public SharpFeature SharpFeature { get; set; }
    }
}