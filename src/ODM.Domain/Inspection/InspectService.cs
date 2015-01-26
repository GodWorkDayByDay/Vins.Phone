using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using HalconDotNet;
using Hdc.Diagnostics;
using Hdc.Mercury;
using Hdc.Mv;
using Hdc.Mv.Calibration;
using Hdc.Mv.Halcon;
using Hdc.Mv.ImageAcquisition;
using Hdc.Mv.Inspection;
using Hdc.Mv.Inspection.Mil.Interop;
using Hdc.Patterns;
using Hdc.Reactive;
using Microsoft.Practices.Unity;
using MvInspection;
using MvInspection.ImageAcquisition;
using ODM.Domain.Configs;

namespace ODM.Domain.Inspection
{
    internal class InspectService : IInspectService
    {
        private readonly ISubject<int> _acquisitionStartedEvent = new Subject<int>();
        private readonly ISubject<int> _inspectionStartedEvent = new Subject<int>();
        private readonly ISubject<ImageInfo> _acquisitionCompletedEvent = new Subject<ImageInfo>();
        private readonly ISubject<SurfaceInspectInfo> _inspectionCompletedEvent = new Subject<SurfaceInspectInfo>();
        private readonly ISubject<int> _calibrationStartedEvent = new Subject<int>();
        private readonly ISubject<ImageInfo> _calibrationCompletedEvent = new Subject<ImageInfo>();

        private int _currentIndex;

        private ICamera _camera;
        private IInspector _inspector;

        [Dependency]
        public IMachineProvider MachineProvider { get; set; }

        [Dependency]
        public IMachineConfigProvider MachineConfigProvider { get; set; }

        public IMachine Machine
        {
            get { return MachineProvider.Machine; }
        }

        [Dependency]
        public IInspectionDomainService InspectionDomainService { get; set; }

        [InjectionMethod]
        public void Init()
        {
            InitCameraAndInspector();

            Machine.Inspection_SurfaceFront_GrabReadyPlcEventDevice
                .Subscribe(async x =>
                                 {
                                     if (!x) return;

                                     _currentIndex++;
                                     Machine.Inspection_SurfaceFront_GrabReadyPlcEventDevice.WriteFalse();
                                     await AcquisitionAndInspectAsync(0);
                                 });

            Machine.Inspection_SurfaceBack_GrabReadyPlcEventDevice
                .Subscribe(async x =>
                                 {
                                     if (!x) return;

                                     _currentIndex++;
                                     Machine.Inspection_SurfaceBack_GrabReadyPlcEventDevice.WriteFalse();
                                     await AcquisitionAndInspectAsync(1);
                                 });

            Machine.Inspection_Forward_MotionStartedPlcEventDevice
                .WhereTrue()
                .Subscribe(x => { Machine.Inspection_Forward_MotionStartedPlcEventDevice.WhereFalse(); });
            Machine.Inspection_Forward_MotionFinishedPlcEventDevice
                .WhereTrue()
                .Subscribe(x => { Machine.Inspection_Forward_MotionFinishedPlcEventDevice.WhereFalse(); });
            Machine.Inspection_Backward_MotionStartedPlcEventDevice
                .WhereTrue()
                .Subscribe(x => { Machine.Inspection_Backward_MotionStartedPlcEventDevice.WhereFalse(); });
            Machine.Inspection_Backward_MotionFinishedPlcEventDevice
                .WhereTrue()
                .Subscribe(x => { Machine.Inspection_Backward_MotionFinishedPlcEventDevice.WhereFalse(); });
        }

        private async void InitCameraAndInspector()
        {
            if (MachineConfigProvider.MachineConfig.MV_SimulationAcquisitionEnabled)
            {
                _camera = new SimCamera(MachineConfigProvider.MachineConfig.MV_SimulationImageFileNames);
                //                _camera = new SimMatroxCamera();
            }
            else
                _camera = new E2VMatroxCamera();

            if (MachineConfigProvider.MachineConfig.MV_SimulationInspectorEnabled)
            {
                _inspector = new SimInspector();
            }
            else
            {
                var adaptedInspector = new AdaptedInspector2
                                       {
                                           MachineConfig = MachineConfigProvider.MachineConfig
                                       };
                _inspector = adaptedInspector;
            }

            bool isSuccessful;

            isSuccessful = await Task.Run(() => _camera.Init());

            if (!isSuccessful)
                throw new Exception("Camera cannot init");
        }

        private int _inspectCounter = 0;

        public async Task<int> AcquisitionAndInspectAsync(int surfaceTypeIndex)
        {
            // Acquisition
            Task.Run(() => _acquisitionStartedEvent.OnNext(surfaceTypeIndex));
            var imageInfo = await _camera.AcquisitionAsync();

            switch (surfaceTypeIndex)
            {
                case 0:
                    Machine.Inspection_SurfaceFront_GrabFinishedPcEventDevice.WriteTrue();
                    break;
                case 1:
                    Machine.Inspection_SurfaceBack_GrabFinishedPcEventDevice.WriteTrue();
                    break;
            }

            Task.Run(() => _acquisitionCompletedEvent.OnNext(imageInfo.ToEntity()));

            // Calibration
            Task.Run(() => _calibrationStartedEvent.OnNext(surfaceTypeIndex));
            HalconImageCalibrator calib = new HalconImageCalibrator();

            var calibImage = calib.CalibrateImage(imageInfo);
            var calibImageInfo = calibImage.ToImageInfo();
            calibImageInfo.SurfaceTypeIndex = surfaceTypeIndex;

            Debug.WriteLine("SurfaceType " + surfaceTypeIndex + ": StartGrabAsync() Finished");
            Task.Run(() => _calibrationCompletedEvent.OnNext(calibImageInfo.ToEntity()));

            Task.Run(() => _inspectionStartedEvent.OnNext(surfaceTypeIndex));

            Task.Run(() =>
                     {
                         var sw3 = new NotifyStopwatch("_inspector.Inspect()");
                         var inspectionInfo = _inspector.Inspect(calibImage);
                         sw3.Dispose();

                         var surfaceInspectInfo = new SurfaceInspectInfo()
                         {
                             SurfaceTypeIndex = surfaceTypeIndex,
                             ImageInfo = imageInfo.ToEntity(),
                             InspectInfo = inspectionInfo.ToEntity(),
                         };

                         HandleInspect(surfaceInspectInfo);
                     });

            return imageInfo.Index;
        }

/*        private void HandleInspect(int surfaceTypeIndex, InspectionInfo inspectionInfo, Hdc.Mv.ImageInfo imageInfo)
        {
            var surfaceInspectInfo = new SurfaceInspectInfo()
            {
//                WorkpieceIndex =
//                    _inspectCounter /
//                    MachineConfigProvider.MachineConfig.MV_AcquisitionCountPerWorkpiece,
                SurfaceTypeIndex = surfaceTypeIndex,
                ImageInfo = imageInfo.ToEntity(),
                InspectInfo = inspectionInfo.ToEntity(),
            };

            HandleInspect(surfaceInspectInfo);
        }

        private void HandleInspect(int surfaceTypeIndex, InspectionInfo inspectionInfo, BitmapSource bitmapSource)
        {
            var surfaceInspectInfo = new SurfaceInspectInfo()
            {
                //                WorkpieceIndex =
                //                    _inspectCounter /
                //                    MachineConfigProvider.MachineConfig.MV_AcquisitionCountPerWorkpiece,
                SurfaceTypeIndex = surfaceTypeIndex,
                BitmapSource = bitmapSource,
                InspectInfo = inspectionInfo.ToEntity(),
            }; 
            
            HandleInspect( surfaceInspectInfo);
        }*/

        private void HandleInspect(SurfaceInspectInfo surfaceInspectInfo)
        {
//            InspectInfo inspectInfoEntity = inspectionInfo.ToEntity();
//            ImageInfo imageInfoEntity = imageInfo.ToEntity();
//
//            var surfaceInspectInfo = new SurfaceInspectInfo()
//                                     {
//                                         WorkpieceIndex =
//                                             _inspectCounter/
//                                             MachineConfigProvider.MachineConfig.MV_AcquisitionCountPerWorkpiece,
//                                         SurfaceTypeIndex = surfaceTypeIndex,
//                                         ImageInfo = imageInfoEntity,
//                                         InspectInfo = inspectInfoEntity,
//                                     };

            surfaceInspectInfo.WorkpieceIndex =
                _inspectCounter/
                MachineConfigProvider.MachineConfig.MV_AcquisitionCountPerWorkpiece;

            Task.Run(() => _inspectionCompletedEvent.OnNext(surfaceInspectInfo));

            Debug.WriteLine("Inspect Finished. SurfaceTypeIndex: " + surfaceInspectInfo.SurfaceTypeIndex);

            if (surfaceInspectInfo.InspectInfo.DefectInfos.Count <= 0)
            {
                InspectionDomainService.HandleSurfaceInspectionInfo(surfaceInspectInfo);

                switch (surfaceInspectInfo.SurfaceTypeIndex)
                {
                    case 0:
                        Machine.Inspection_SurfaceFront_InspectionFinishedWithAcceptedPcEventDevice.WriteTrue();
                        Debug.WriteLine(
                            "Inspect Finished. Inspection_SurfaceFront_InspectionFinishedWithAcceptedPcEventDevice.WriteTrue()");
                        break;
                    case 1:
                        Machine.Inspection_SurfaceBack_InspectionFinishedWithAcceptedPcEventDevice.WriteTrue();
                        Debug.WriteLine(
                            "Inspect Finished. Inspection_SurfaceBack_InspectionFinishedWithAcceptedPcEventDevice.WriteTrue()");
                        break;
                }

                if (MachineConfigProvider.MachineConfig.MV_SimulationInspectorEnabled)
                {
                    Machine.Production_TotalCountDevice.Value++;
                }
            }
            else
            {
                InspectionDomainService.HandleSurfaceInspectionInfo(surfaceInspectInfo);

                switch (surfaceInspectInfo.SurfaceTypeIndex)
                {
                    case 0:
                        Machine.Inspection_SurfaceFront_InspectionFinishedWithRejectedPcEventDevice.WriteTrue();
                        Debug.WriteLine(
                            "Inspect Finished. Inspection_SurfaceFront_InspectionFinishedWithRejectedPcEventDevice.WriteTrue()");
                        break;
                    case 1:
                        Machine.Inspection_SurfaceBack_InspectionFinishedWithRejectedPcEventDevice.WriteTrue();
                        Debug.WriteLine(
                            "Inspect Finished. Inspection_SurfaceBack_InspectionFinishedWithRejectedPcEventDevice.WriteTrue()");
                        break;
                }

                if (MachineConfigProvider.MachineConfig.MV_SimulationInspectorEnabled)
                {
                    Machine.Production_TotalCountDevice.Value++;
                    Machine.Production_TotalRejectCountDevice.Value++;
                }
            }

            _inspectCounter++;
        }

        public IObservable<int> AcquisitionStartedEvent
        {
            get { return _acquisitionStartedEvent; }
        }

        public IObservable<ImageInfo> AcquisitionCompletedEvent
        {
            get { return _acquisitionCompletedEvent; }
        }

        public IObservable<int> CalibrationStartedEvent
        {
            get { return _calibrationStartedEvent; }
        }

        public IObservable<ImageInfo> CalibrationCompletedEvent
        {
            get { return _calibrationCompletedEvent; }
        }

        public IObservable<int> InspectionStartedEvent
        {
            get { return _inspectionStartedEvent; }
        }

        public IObservable<SurfaceInspectInfo> InspectionCompletedEvent
        {
            get { return _inspectionCompletedEvent; }
        }

        public void Start()
        {
            _currentIndex = 0;

            MachineProvider.Machine.Production_StartCommandDevice.WriteTrue();
        }

        public void Stop()
        {
            MachineProvider.Machine.Production_StopCommandDevice.WriteTrue();
        }

        public void Reset()
        {
            MachineProvider.Machine.Production_ResetCommandDevice.WriteTrue();
        }

        public async void InspectImageFile(int surfaceTypeIndex, string fileName)
        {
            // 
//            Task.Run(() => _acquisitionStartedEvent.OnNext(surfaceTypeIndex));
//            _acquisitionStartedEvent.OnNext(surfaceTypeIndex);
//            Thread.Sleep(500);

            //            Task.Run(() => _acquisitionCompletedEvent.OnNext(imageInfoEntity));
            //            _acquisitionCompletedEvent.OnNext(imageInfoEntity);
            //            Thread.Sleep(500);


            //            _calibrationStartedEvent.OnNext(surfaceTypeIndex);
            //            Thread.Sleep(500);

            Task.Run(() => _calibrationStartedEvent.OnNext(surfaceTypeIndex));

            var sw = new NotifyStopwatch("InspectImageFile: new BitmapImage(fileName)");
            BitmapImage bi = new BitmapImage(new Uri(fileName, UriKind.RelativeOrAbsolute));
            sw.Dispose();

            var sw1 = new NotifyStopwatch("InspectImageFile: BitmapImage.ToImageInfoWith8Bpp()");
            Hdc.Mv.ImageInfo imageInfo = bi.ToImageInfoWith8Bpp();
            sw1.Dispose();

            imageInfo.Index = 0;
            imageInfo.SurfaceTypeIndex = surfaceTypeIndex;
            ImageInfo imageInfoEntity = imageInfo.ToEntity();

            await Task.Run(() => _calibrationCompletedEvent.OnNext(imageInfoEntity));
            //            _calibrationCompletedEvent.OnNext(imageInfoEntity);
            //            Thread.Sleep(500);
            //
            Task.Run(() => _inspectionStartedEvent.OnNext(surfaceTypeIndex));

            await Task.Run(() =>
                           {
                               var sw4 = new NotifyStopwatch("imageInfo.To8BppHImage()");
                               var to8BppHImage = imageInfo.To8BppHImage();
                               sw4.Dispose();

                               var sw3 = new NotifyStopwatch("inspector.Inspect()");
                               var inspectionInfo = _inspector.Inspect(to8BppHImage);
                               sw3.Dispose();

                               var surfaceInspectInfo = new SurfaceInspectInfo()
                               {
                                   SurfaceTypeIndex = surfaceTypeIndex,
                                   BitmapSource = bi,
                                   InspectInfo = inspectionInfo.ToEntity(),
                               };

                               HandleInspect(surfaceInspectInfo);
                           });
        }
    }
}