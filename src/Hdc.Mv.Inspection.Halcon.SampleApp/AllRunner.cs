using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using Hdc.Diagnostics;
using Hdc.Mv.Inspection.Halcon.BatchInspector;

namespace Hdc.Mv.Inspection.Halcon.SampleApp
{
    public class AllRunner : Runner
    {
        public override void Run(ImageInfo imageInfo, InspectionSchema schema)
        {
            using (var sw = new NotifyStopwatch("AllRunner.InspectionController.Inspect()"))
            {
                InspectionController
                    .StartInspect()
                    .SetInspectionSchema()
                    .SetImageInfo(imageInfo)
                    .CreateCoordinate()
                    .Inspect()
                    ;
            }

            MainWindow.Show_CircleSearchingDefinitions(
                InspectionController.InspectionResult.GetCoordinateCircleSearchingDefinitions());
            MainWindow.Show_CircleSearchingResults(InspectionController.InspectionResult.CoordinateCircles);

            MainWindow.Show_CircleSearchingDefinitions(
                InspectionController.InspectionResult.GetCircleSearchingDefinitions(), Brushes.DodgerBlue);
            MainWindow.Show_CircleSearchingResults(InspectionController.InspectionResult.Circles, Brushes.DodgerBlue);

            MainWindow.Show_EdgeSearchingDefinitions(InspectionController.InspectionResult.GetEdgeSearchingDefinitions());
            MainWindow.Show_EdgeSearchingResults(InspectionController.InspectionResult.Edges);
            MainWindow.Show_DistanceBetweenPointsResults(InspectionController.InspectionResult.DistanceBetweenPointsResults);
            MainWindow.Show_DefectResults(InspectionController.InspectionResult.DefectResults);
        }
    }
}