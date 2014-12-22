using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Hdc.Mercury;
using Hdc.Mv.ImageAcquisition;
using Hdc.Mv.Inspection;
using Hdc.Mv.Inspection.Halcon;
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

        private int _currentIndex;

        private ICamera _camera;
        private IInspector _inspector;
        private IImageLoader _imageLoader;

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
                .Subscribe(x =>
                {
                    Machine.Inspection_Forward_MotionStartedPlcEventDevice.WhereFalse();
                });
            Machine.Inspection_Forward_MotionFinishedPlcEventDevice
                .WhereTrue()
                .Subscribe(x =>
                {
                    Machine.Inspection_Forward_MotionFinishedPlcEventDevice.WhereFalse();
                });
            Machine.Inspection_Backward_MotionStartedPlcEventDevice
                .WhereTrue()
                .Subscribe(x =>
                {
                    Machine.Inspection_Backward_MotionStartedPlcEventDevice.WhereFalse();
                });
            Machine.Inspection_Backward_MotionFinishedPlcEventDevice
                .WhereTrue()
                .Subscribe(x =>
                {
                    Machine.Inspection_Backward_MotionFinishedPlcEventDevice.WhereFalse();
                });
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

            _imageLoader = new ImageLoader();

            if (MachineConfigProvider.MachineConfig.MV_SimulationInspectorEnabled)
            {
                _inspector = new SimInspector();
            }
            else
            {
                //_inspector = new MilInspector();

//                await Task.Run(() =>
//                {
                    var adaptedInspector = new AdaptedInspector2
                    {
                        MachineConfig = MachineConfigProvider.MachineConfig
                    };
                    _inspector = adaptedInspector;
//                    _inspector.Init();
//                });
                
            }

            bool isSuccessful;

            isSuccessful = await Task.Run(() => _camera.Init());

            if (!isSuccessful)
                throw new Exception("Camera cannot init");

            await Task.Run(() => { _inspector.Init(); });

            isSuccessful = _inspector.LoadParameters();

            if (!isSuccessful)
                throw new Exception("_inspector.Init cannot init");
        }

        private int _inspectCounter = 0;

        public async Task<int> AcquisitionAndInspectAsync(int surfaceTypeIndex)
        {
            // Acquisition
            Task.Run(() => _acquisitionStartedEvent.OnNext(surfaceTypeIndex));
            var imageInfo = await _camera.AcquisitionAsync();

                        IImageCalibrator calib = new HalconImageCalibrator();
//            IImageCalibrator calib = new MilImageCalibrator();
            //            IImageCalibrator calib = new SimImageCalibrator();

            var calibImage = calib.CalibrateImage(imageInfo);

            calibImage.SurfaceTypeIndex = surfaceTypeIndex;
            switch (surfaceTypeIndex)
            {
                case 0:
                    Machine.Inspection_SurfaceFront_GrabFinishedPcEventDevice.WriteTrue();
                    break;
                case 1:
                    Machine.Inspection_SurfaceBack_GrabFinishedPcEventDevice.WriteTrue();
                    break;
            }
            Debug.WriteLine("SurfaceType " + calibImage.SurfaceTypeIndex + ": StartGrabAsync() Finished");
            Task.Run(() => _acquisitionCompletedEvent.OnNext(calibImage.ToEntity()));

            // Inspect
            //Thread.Sleep(2000);

            Task.Run(() => { _inspectionStartedEvent.OnNext(surfaceTypeIndex); });

//            var inspectionInfo = await _inspector.InspectAsync(calibImage);
            var inspectionInfo = _inspector.Inspect(calibImage);

//            Debug.WriteLine(DateTime.Now + ": InspectAsync finished.");
//            Debug.WriteLine("\t - inspectionInfo.Index" + inspectionInfo.Index);
//            Debug.WriteLine("\t - inspectionInfo.SurfaceTypeIndex" + inspectionInfo.SurfaceTypeIndex);
//            Debug.WriteLine("\t - inspectionInfo.HasError" + inspectionInfo.HasError);
//            Debug.WriteLine("\t - inspectionInfo.MeasurementInfos.Count" + inspectionInfo.MeasurementInfos.Count);
//            Debug.WriteLine("\t - inspectionInfo.DefectInfos.Count" + inspectionInfo.DefectInfos.Count);

            InspectInfo inspectInfoEntity = inspectionInfo.ToEntity();
            ImageInfo imageInfoEntity = calibImage.ToEntity();

            var surfaceInspectInfo = new SurfaceInspectInfo()
                                     {
                                         WorkpieceIndex =
                                             _inspectCounter /
                                             MachineConfigProvider.MachineConfig.MV_AcquisitionCountPerWorkpiece,
                                         SurfaceTypeIndex = surfaceTypeIndex,
                                         ImageInfo = imageInfoEntity,
                                         InspectInfo = inspectInfoEntity,
                                     };
            Task.Run(() => _inspectionCompletedEvent.OnNext(surfaceInspectInfo));

            HandleInspect(surfaceInspectInfo);

            _inspectCounter++;

            return imageInfo.Index;
        }

        private void HandleInspect(SurfaceInspectInfo inspectInfo)
        {
            Debug.WriteLine("Inspect Finished. SurfaceTypeIndex: " + inspectInfo.SurfaceTypeIndex);

            if (inspectInfo.InspectInfo.DefectInfos.Count <= 0)
            {
                InspectionDomainService.HandleSurfaceInspectionInfo(inspectInfo);

                switch (inspectInfo.SurfaceTypeIndex)
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
                InspectionDomainService.HandleSurfaceInspectionInfo(inspectInfo);

                switch (inspectInfo.SurfaceTypeIndex)
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
        }

        public IObservable<int> AcquisitionStartedEvent
        {
            get { return _acquisitionStartedEvent; }
        }

        public IObservable<ImageInfo> AcquisitionCompletedEvent
        {
            get { return _acquisitionCompletedEvent; }
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
            Task.Run(() => _acquisitionStartedEvent.OnNext(surfaceTypeIndex));
            var imageInfo = await _imageLoader.LoadFromFileAsync(fileName);
            imageInfo.Index = 0;
            imageInfo.SurfaceTypeIndex = surfaceTypeIndex;
            ImageInfo imageInfoEntity = imageInfo.ToEntity();
            Task.Run(() => _acquisitionCompletedEvent.OnNext(imageInfoEntity));
            //
            await Task.Run(() => Thread.Sleep(500));
            //
            Task.Run(() => _inspectionStartedEvent.OnNext(surfaceTypeIndex));
            var inspectionInfo = await _inspector.InspectAsync(imageInfo);
            var inspectInfoEntity = inspectionInfo.ToEntity();
            var surfaceInspectInfo = new SurfaceInspectInfo()
            {
                WorkpieceIndex =
                    _inspectCounter /
                    MachineConfigProvider.MachineConfig.MV_AcquisitionCountPerWorkpiece,
                SurfaceTypeIndex = surfaceTypeIndex,
                ImageInfo = imageInfoEntity,
                InspectInfo = inspectInfoEntity,
            };
            Task.Run(() => _inspectionCompletedEvent.OnNext(surfaceInspectInfo));
        }
    }
}