using System;
using ODM.Inspectors.Halcon.SampleApp;

namespace Hdc.Mv.Inspection.Halcon.SampleApp
{
    public interface IRunner:IDisposable
    {
        IRunner Init(Func<string, IGeneralInspector> inspectorFactory, MainWindow mainWindow);

        void Run(ImageInfo imageInfo, InspectionSchema schema);
    }
}