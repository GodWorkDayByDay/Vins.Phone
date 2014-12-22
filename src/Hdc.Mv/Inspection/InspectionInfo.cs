using System.Collections.Generic;
using Hdc.Mv.Inspection;

namespace Hdc.Mv.Inspection
{
    public class InspectionInfo
    {
        public int Index { get; set; }
        public int SurfaceTypeIndex { get; set; }
        public bool HasError { get; set; }
        public List<DefectInfo> DefectInfos { get; set; }
        public List<MeasurementInfo> MeasurementInfos { get; set; }

        public InspectionInfo()
        {
            DefectInfos = new List<DefectInfo>();

            MeasurementInfos = new List<MeasurementInfo>();
        }
    }
}