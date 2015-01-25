/*using System;
using System.Windows.Media;
using Hdc.Diagnostics;
using Hdc.Mv.Halcon;
using ODM.Inspectors.Halcon.SampleApp;

namespace Hdc.Mv.Inspection.Halcon.SampleApp
{
    public class Runner : IRunner
    {
        protected MainWindow MainWindow;

        protected Func<string,IGeneralInspector> InspectorFactory;

        protected IInspectionController InspectionController;

        public IRunner Init(Func<string, IGeneralInspector> inspectorFactory, MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            InspectionController = new InspectionController();
            InspectionController.SetInspectorFactory(inspectorFactory);
            return this;
        }

        public void Run(ImageInfo imageInfo, InspectionSchema schema)
        {
            using (var sw = new NotifyStopwatch("AllRunner.InspectionController.Inspect()"))
            {
                InspectionController
                    .StartInspect()
                    .SetInspectionSchema()
                    .SetImageInfo(imageInfo.To8BppHImage())
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

        public void Dispose()
        {
            InspectionController.Dispose();
        }
    }
}*/