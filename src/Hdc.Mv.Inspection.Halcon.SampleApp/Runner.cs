using System;
using ODM.Inspectors.Halcon.SampleApp;

namespace Hdc.Mv.Inspection.Halcon.SampleApp
{
    public abstract class Runner : IRunner
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

        public abstract void Run(ImageInfo imageInfo, InspectionSchema schema);

        public void Dispose()
        {
            InspectionController.Dispose();
        }
    }
}