using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Hdc.Collections.Generic;
using Hdc.Linq;
using Hdc.Mv.Halcon;
using ODM.Inspectors.Halcon.SampleApp;

namespace Hdc.Mv.Inspection.Halcon.SampleApp
{
    public class EdgesRunner : Runner
    {
        public override void Run(ImageInfo imageInfo, InspectionSchema schema)
        {
            InspectionController
              .StartInspect()
              .SetInspectionSchema()
              .SetImageInfo(imageInfo.To8BppHImage())
              .CreateCoordinate()
              .Inspect()
              ;

            MainWindow.Show_CircleSearchingDefinitions(InspectionController.InspectionResult.GetCoordinateCircleSearchingDefinitions());
            MainWindow.Show_CircleSearchingResults(InspectionController.InspectionResult.CoordinateCircles);

            MainWindow.Show_EdgeSearchingDefinitions(schema.EdgeSearchingDefinitions);
            MainWindow.Show_EdgeSearchingResults(InspectionController.InspectionResult.Edges);
        }
    }
}