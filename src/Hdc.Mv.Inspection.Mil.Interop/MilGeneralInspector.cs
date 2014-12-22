using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using Hdc.Collections.Generic;
using Hdc.Mv.Inspection.Mil.Interop;
using Omu.ValueInjecter;

namespace Hdc.Mv.Inspection.Mil
{
    public class MilGeneralInspector : IGeneralInspector
    {
        public InspectionResult Inspect(ImageInfo imageInfo, InspectionSchema inspectionSchema)
        {
            var inspectionResult = new InspectionResult();

            Debug.WriteLine("MilGeneralInspector.Inspect in");

            int errorCode;

            InteropApi.CleanDefinitions();

            AddCircleDefinitions(inspectionSchema);

            // AddEdgeDefinitions(inspectionSchema);

            Debug.WriteLine("InteropApi.Calculate begin");
            errorCode = InteropApi.Calculate(imageInfo);
            Debug.WriteLine("InteropApi.Calculate end");

            if (errorCode != 0)
            {
                Debug.WriteLine("InteropApi.Calculate error");
                throw new MilInteropException("Calculate", errorCode);
            }

            inspectionResult.Circles.AddRange(GetCircleResults(inspectionSchema));

            // inspectionResult.Edges.AddRange(GetEdgeResults(inspectionSchema));

            Debug.WriteLine("MilGeneralInspector.Inspect out");

            return inspectionResult;
        }

        public CircleSearchingResultCollection SearchCircles(ImageInfo imageInfo, IList<CircleSearchingDefinition> circleSearchingDefinitions)
        {
            var inspectionResult = new CircleSearchingResultCollection();

            Debug.WriteLine("MilGeneralInspector.Inspect in");

            int errorCode;

            InteropApi.CleanDefinitions();

            AddCirclesDefinitions(circleSearchingDefinitions);

            Debug.WriteLine("InteropApi.Calculate begin");
            errorCode = InteropApi.Calculate(imageInfo);
            Debug.WriteLine("InteropApi.Calculate end");

            if (errorCode != 0)
            {
                Debug.WriteLine("InteropApi.Calculate error");
                throw new MilInteropException("Calculate", errorCode);
            }

            inspectionResult.AddRange(GetCircleSearchingResults(circleSearchingDefinitions));

            Debug.WriteLine("MilGeneralInspector.Inspect out");

            return inspectionResult;
        }

        public EdgeSearchingResultCollection SearchEdges(ImageInfo imageInfo, IList<EdgeSearchingDefinition> edgeSearchingDefinitions)
        {
            throw new NotImplementedException();
        }

        public DefectResultCollection SearchDefects(ImageInfo imageInfo)
        {
            var drs = new DefectResultCollection();
//            return drs;

            InspectInfo inspectInfo;

            int errorCode = 0;

            errorCode = InteropApi.InspectCalculate(imageInfo, imageInfo, out inspectInfo);

            if (errorCode != 0)
                throw new MilInteropException("InteropApi.InspectCalculate", errorCode);

            for (int i = 0; i < inspectInfo.DefectsCount; i++)
            {
                DefectInfo defectInfo;
                errorCode = InteropApi.GetInspectDefect(i, out defectInfo);
                if (errorCode != 0)
                    throw new MilInteropException("InteropApi.GetInspectDefect", errorCode);

                var defectResult = new DefectResult();
                defectResult.InjectFrom(defectInfo);
                drs.Add(defectResult);
            }

            return drs;
        }

        public DefectResultCollection SearchDefects(ImageInfo imageInfo, ImageInfo mask)
        {
            var drs = new DefectResultCollection();

            InspectInfo inspectInfo;

            int errorCode = 0;

            errorCode = InteropApi.InspectCalculate(imageInfo, mask,  out inspectInfo);

            if (errorCode != 0)
                throw new MilInteropException("InteropApi.InspectCalculate", errorCode);

            for (int i = 0; i < inspectInfo.DefectsCount; i++)
            {
                DefectInfo defectInfo;
                errorCode = InteropApi.GetInspectDefect(i, out defectInfo);
                if (errorCode != 0)
                    throw new MilInteropException("InteropApi.GetInspectDefect", errorCode);

                var defectResult = new DefectResult();
                defectResult.InjectFrom(defectInfo);
                drs.Add(defectResult);
            }

            return drs;
        }

        public ImageInfo FindRegions(ImageInfo imageInfo)
        {
            throw new NotImplementedException();
        }

        public ImageInfo FindRegions()
        {
            throw new NotImplementedException();
        }

        private static IList<CircleSearchingResult> GetCircleResults(InspectionSchema inspectionSchema)
        {

            if (!inspectionSchema.CircleSearchingEnable) return new List<CircleSearchingResult>();


            IList<CircleSearchingDefinition> circleSearchingDefinitions = inspectionSchema.CircleSearchingDefinitions;

            var circleSearchingResults = GetCircleSearchingResults(circleSearchingDefinitions);

            return circleSearchingResults;
        }

        private static IList<CircleSearchingResult> GetCircleSearchingResults(IList<CircleSearchingDefinition> circleSearchingDefinitions)
        {
            int errorCode;

            IList<CircleSearchingResult> circleSearchingResults = new List<CircleSearchingResult>();

            for (int i = 0; i < circleSearchingDefinitions.Count; i++)
            {
                var cd = circleSearchingDefinitions[i];

                Circle foundCircle;
                int isNotFound;
                errorCode = InteropApi.GetCircleResult(i, out foundCircle, out isNotFound);

                Debug.WriteLine("GetCircleResult " + i + ", errorCode: " + errorCode);

                if (errorCode == -1)
                {
                    var errorResult = new CircleSearchingResult(i, cd)
                    {
                        HasError = true,
                        IsNotFound = isNotFound == 1
                    };
                    circleSearchingResults.Add(errorResult);
                    continue;
                }

                if (errorCode != 0)
                    throw new MilInteropException("InteropApi.GetCircleResult", errorCode);

                var result = new CircleSearchingResult(index: i, definition: cd.DeepClone(), circle: foundCircle)
                {
                    IsNotFound = isNotFound != 0
                };
                circleSearchingResults.Add(result);
            }
            return circleSearchingResults;
        }

        private static void AddCircleDefinitions(InspectionSchema inspectionSchema)
        {
            Collection<CircleSearchingDefinition> circleSearchingDefinitions = inspectionSchema.CircleSearchingDefinitions;

            if (!inspectionSchema.CircleSearchingEnable) return;

            AddCirclesDefinitions(circleSearchingDefinitions);
        }

        private static void AddCirclesDefinitions(IList<CircleSearchingDefinition> circleSearchingDefinitions)
        {
            int index = 0;
            int errorCode;

            foreach (var circleDef in circleSearchingDefinitions)
            {
                int iFilter = (int)circleDef.Mil_CircleSearchingMethod;

                errorCode = InteropApi.AddCircleDefinition(circleDef.CenterX, circleDef.CenterY,
                    circleDef.InnerRadius,
                    circleDef.OuterRadius, circleDef.Mil_LowThreshold, circleDef.Mil_HighThreshold,
                    circleDef.Mil_CenterDiffer, circleDef.Mil_RadiusDiffer, circleDef.Mil_Ratio, iFilter);

                Debug.WriteLine("AddCircleDefinition " + index);
                Debug.WriteLine("AddCircleDefinition CenterX" + circleDef.CenterX);
                Debug.WriteLine("AddCircleDefinition CenterY" + circleDef.CenterY);
                index++;

                if (errorCode != 0)
                {
                    Debug.WriteLine("InteropApi.AddCircleDefinition error");
                    throw new MilInteropException("InteropApi.AddCircleDefinition", errorCode);
                }
            }
        }

        public void Dispose()
        {
            InteropApi.FreeApp();
        }

        public void Init()
        {
            InteropApi.InitApp(8192, 12500);
        }

        public void Init(int width, int height)
        {
            InteropApi.InitApp(8192, 12500);
        }

        public InspectionResult SearchCircles(ImageInfo imageInfo, InspectionSchema inspectionSchema)
        {
            throw new NotImplementedException();
        }

        public InspectionResult SearchEdges(ImageInfo imageInfo, InspectionSchema inspectionSchema)
        {
            throw new NotImplementedException();
        }

        public DistanceBetweenLinesResult GetDistanceBetweenLines(Line line1, Line line2)
        {
            double distInPixel;
            double distInWorld;
            double angle;
            Point footPoint1;
            Point footPoint2;

            var errorCode = InteropApi.GetDistanceBetweenLines(
                line1,
                line2,
                out distInPixel,
                out distInWorld,
                out angle,
                out footPoint1,
                out footPoint2);

            if (errorCode != 0)
                throw new MilInteropException("GetDistanceBetweenLines", errorCode);

            return new DistanceBetweenLinesResult()
                   {
                       DistanceInPixel = distInPixel,
                       DistanceInWorld = distInWorld,
                       Angle = angle,
                       FootPoint1 = footPoint1,
                       FootPoint2 = footPoint2,
                   };
        }

        public DistanceBetweenPointsResult GetDistanceBetweenPoints(Point point1, Point point2)
        {
            double distInPixel2;
            double distInWorld2;
            double angle2;

            var errorCode = InteropApi.GetDistanceBetweenPoints(
                point1,
                point2,
                out distInPixel2,
                out distInWorld2,
                out angle2);

            if (errorCode != 0)
                throw new MilInteropException("GetDistanceBetweenPoints", errorCode);

            return new DistanceBetweenPointsResult()
                   {
                       DistanceInPixel = distInPixel2,
                       DistanceInWorld = distInWorld2,
                       Angle = angle2,
                       Point1 = point1,
                       Point2 = point2,
                   };
        }
    }
}