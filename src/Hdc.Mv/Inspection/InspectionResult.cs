using System.Collections.ObjectModel;

namespace Hdc.Mv.Inspection
{
    public class InspectionResult
    {
        public int Index { get; set; }
        public string Comment { get; set; }

        public InspectionResult()
        {
            CoordinateCircles = new CircleSearchingResultCollection();
            Circles = new CircleSearchingResultCollection();
            Edges = new EdgeSearchingResultCollection();
//            DistanceBetweenLinesResults = new DistanceBetweenLinesResultCollection();
            DistanceBetweenPointsResults = new DistanceBetweenPointsResultCollection();
            DefectResults = new DefectResultCollection();
        }

        public CircleSearchingResultCollection CoordinateCircles { get; set; }
        public CircleSearchingResultCollection Circles { get; set; }
        public EdgeSearchingResultCollection Edges { get; set; }
//        public DistanceBetweenLinesResultCollection DistanceBetweenLinesResults { get; set; }
        public DistanceBetweenPointsResultCollection DistanceBetweenPointsResults { get; set; }
        public DefectResultCollection DefectResults { get; set; }
    }
}