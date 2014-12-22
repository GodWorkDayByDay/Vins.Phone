using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Hdc.Mv.Inspection;

namespace Hdc.Mv
{
    public static class RelativeCoordinateFactory
    {
        public static IRelativeCoordinate CreateCoordinate(IList<CircleSearchingResult> circleDefinitions)
        {
            List<Vector> actualVectors = circleDefinitions.Select(x => x.Circle.GetCenterVector()).ToList();

            var expectVectors = new List<Vector>(
                circleDefinitions.Select(x => new Vector(
                x.Definition.BaselineX * 1000.0 / 16.0,
                x.Definition.BaselineY * 1000.0 / 16.0))) { };

//            var expectVectors = expertVectors.Select(x => new Vector(
//                x.X*1000.0/16.0,
//                x.Y*1000.0/16.0)).ToList();

            var coordinate = CreateCoordinate(actualVectors, expectVectors);
            return coordinate;
        }

        public static IRelativeCoordinate CreateCoordinate(IList<Vector> actualVectors,
                                                           IList<Vector> expectVectors)
        {
//            var actualVectors = actualVectors2.ToList();
//            var expectVectors = expectVectors2.Select(x => new Vector(
//                x.X * 1000.0 / 16.0,
//                x.Y * 1000.0 / 16.0)).ToList();

            // expect
            var expertAvgX = expectVectors.Average(x => x.X);
            var expertAvgY = expectVectors.Average(x => x.Y);
            var expertAvgVector = new Vector(expertAvgX, expertAvgY);
            var expertAvgVectorAngle = new Vector(100, 0).GetAngleTo(expertAvgVector);

            var expertMidToEachCircles = expectVectors.Select(x => expertAvgVector - x).ToList();

            // actual
            Vector actualAvgVectorOK; // origin OK
            double actualAvgVectorOKAngle;

            var actualAvgX = actualVectors.Average(x => x.X);
            var actualAvgY = actualVectors.Average(x => x.Y);
            actualAvgVectorOK = new Vector(actualAvgX, actualAvgY);

            var actualMidToCs = actualVectors.Select(x => actualAvgVectorOK - x).ToList();

            List<double> angleDiffCs = new List<double>();
            for (int i = 0; i < actualMidToCs.Count; i++)
            {
                var actualMidToC = actualMidToCs[i];
                var expertMidToC = expertMidToEachCircles[i];
                var angleDiff = actualMidToC.GetAngleTo(expertMidToC);
                angleDiffCs.Add(angleDiff);
            }

            var angleDiffAvg = angleDiffCs.Average();

            //
            var coord = new RelativeCoordinate(actualAvgVectorOK, -angleDiffAvg)
                        {
                            OriginOffset = -expertAvgVector
                        };

//            var origin = coord.GetRelativeVector(actualVectors.First());
//            coord.OriginOffset = origin;

            return coord;
        }
    }
}