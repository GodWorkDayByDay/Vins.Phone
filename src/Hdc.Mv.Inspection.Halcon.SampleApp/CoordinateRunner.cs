using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Hdc.Collections.Generic;

namespace Hdc.Mv.Inspection.Halcon.SampleApp
{
    class CoordinateRunner : Runner
    {
        public override void Run(ImageInfo imageInfo, InspectionSchema schema)
        {
            InspectionController
              .StartInspect()
              .SetInspectionSchema()
              .SetImageInfo(imageInfo)
              .CreateCoordinate()
//              .Inspect()
              ;

            MainWindow.Show_CircleSearchingDefinitions(InspectionController.InspectionResult.GetCoordinateCircleSearchingDefinitions());
            MainWindow.Show_CircleSearchingResults(InspectionController.InspectionResult.CoordinateCircles);

            // print
//            List<Vector> actualVectors = inspectionController.InspectionResult.Circles.Select(x => x.Circle.GetCenterVector()).ToList();
//            var relativeCs = actualVectors.Select(x => coord.GetRelativePoint(x.ToPoint())).ToList();

            Debug.WriteLine("--------------------------------");
            for (int index = 1; index < InspectionController.InspectionResult.CoordinateCircles.Count; index++)
            {
                var csr = InspectionController.InspectionResult.CoordinateCircles[index];
                Debug.WriteLine("C.X = \t" + csr.RelativeCircle.CenterX.ToMicrometerFromPixel(16));
                Debug.WriteLine("C.Y = \t" + csr.RelativeCircle.CenterY.ToMicrometerFromPixel(16));
            }
        }
    }
}