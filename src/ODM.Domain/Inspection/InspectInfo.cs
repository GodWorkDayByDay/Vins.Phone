using System.Collections.Generic;

namespace ODM.Domain.Inspection
{
    public class InspectInfo
    {
        public int Index { get; set; }
        public bool HasError { get; set; }
        public int SurfaceTypeIndex { get; set; }
        public List<DefectInfo> DefectInfos { get; set; }
        public List<MeasurementInfo> MeasurementInfos { get; set; }
    };
}