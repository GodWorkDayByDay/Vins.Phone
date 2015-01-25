using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Hdc.Mv.Inspection
{
    public static class Ex
    {
        public static Line GetLine(this EdgeSearchingDefinition edgeSearchingDefinition)
        {
            return new Line(edgeSearchingDefinition.StartX,
                edgeSearchingDefinition.StartY,
                edgeSearchingDefinition.EndX,
                edgeSearchingDefinition.EndY);
        }

        public static EdgeSearchingDefinition GetEdgeSearchingDefinition(this Line line, double roiWidth = 0)
        {
            return new EdgeSearchingDefinition(line, roiWidth);
        }

        public static Vector GetBaselineVector(this CircleSearchingDefinition circleSearchingDefinition)
        {
            return new Vector(circleSearchingDefinition.BaselineX, circleSearchingDefinition.BaselineY);
        }

        public static Vector GetBaselineVectorInPixel(this CircleSearchingDefinition circleSearchingDefinition)
        {
            var v = circleSearchingDefinition.GetBaselineVector();
            return new Vector(v.X*1000/16.0, v.Y*1000/16.0);
        }

        public static void UpdateRelativeCircle(this CircleSearchingResult circleResult,
                                                IRelativeCoordinate relativeCoordinate)
        {
            var actualCenterPoint = circleResult.Circle.GetCenterPoint();
            var relativePoint = relativeCoordinate.GetRelativePoint(actualCenterPoint);
            circleResult.RelativeCircle = new Circle(relativePoint, circleResult.Circle.Radius);
        }

        public static void UpdateRelativeCircles(this IEnumerable<CircleSearchingResult> circleResults,
                                                IRelativeCoordinate relativeCoordinate)
        {
            foreach (var circleResult in circleResults)
            {
                circleResult.UpdateRelativeCircle(relativeCoordinate);
            }
        }

        public static void UpdateObjectiveCircle(this CircleSearchingDefinition csd,
                                                IRelativeCoordinate coordinate)
        {
            var relativeVector = new Vector(csd.BaselineX * 1000.0 / 16.0, csd.BaselineY * 1000.0 / 16.0);
            var originalVector = coordinate.GetOriginalVector(relativeVector);
            csd.CenterX = originalVector.X;
            csd.CenterY = originalVector.Y;
        }

        public static void UpdateObjectiveCircles(this IEnumerable<CircleSearchingDefinition> circleResults,
                                                IRelativeCoordinate relativeCoordinate)
        {
            foreach (var circleResult in circleResults)
            {
                circleResult.UpdateObjectiveCircle(relativeCoordinate);
            }
        }

        public static void UpdateFromRelativeLine(this EdgeSearchingDefinition def,
                                                IRelativeCoordinate relativeCoordinate)
        {
            if (Math.Abs(def.RelativeLine.X1) < 0.000001 ||
                  Math.Abs(def.RelativeLine.Y1) < 0.000001 ||
                  Math.Abs(def.RelativeLine.X2) < 0.000001 ||
                  Math.Abs(def.RelativeLine.Y2) < 0.000001) 
            { return; }

            var p1 = def.RelativeLine.GetPoint1();
            var p2 = def.RelativeLine.GetPoint2();

            var actualP1 = relativeCoordinate.GetOriginalPoint(p1);
            var actualP2 = relativeCoordinate.GetOriginalPoint(p2);

            def.StartX = actualP1.X;
            def.StartY = actualP1.Y;

            def.EndX = actualP2.X;
            def.EndY = actualP2.Y;
        }

        public static void UpdateFromRelativeLines(this IEnumerable<EdgeSearchingDefinition> definitions,
                                                  IRelativeCoordinate relativeCoordinate)
        {
            foreach (var esd in definitions)
            {
                esd.UpdateFromRelativeLine(relativeCoordinate);
            }
        }

        public static IEnumerable<EdgeSearchingDefinition> GetEdgeSearchingDefinitions(this InspectionResult inspectionResult)
        {
            return inspectionResult.Edges.Select(x => x.Definition);
        }


        public static IEnumerable<EdgeSearchingDefinition> GetCoordinateEdges(this InspectionResult inspectionResult)
        {
            return inspectionResult.CoordinateEdges.Select(x => x.Definition);
        }

        public static IEnumerable<CircleSearchingDefinition> GetCircleSearchingDefinitions(this InspectionResult inspectionResult)
        {
            return inspectionResult.Circles.Select(x => x.Definition);
        }

        public static IEnumerable<CircleSearchingDefinition> GetCoordinateCircleSearchingDefinitions(this InspectionResult inspectionResult)
        {
            return inspectionResult.CoordinateCircles.Select(x => x.Definition);
        }

        public static RegionResult GetRegionResult(this SurfaceResult surfaceResult, string regionName)
        {
            var region = surfaceResult.IncludeRegionResults.SingleOrDefault(x => x.RegionName == regionName);
            return region;
        }

        public static RegionResult GetRegionResult(this IEnumerable<SurfaceResult> surfaceResults, string surfaceName, string regionName)
        {
            var surface = surfaceResults.SingleOrDefault(x => x.Definition.Name == surfaceName);
            if (surface == null) return null;
            return surface.GetRegionResult(regionName);
        }
    }
}