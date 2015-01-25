using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows;
using HalconDotNet;
using Hdc.Diagnostics;
using Hdc.Reflection;
using Hdc.Serialization;

namespace Hdc.Mv.Inspection
{
    public interface IInspectionController : ISetInspectorFactory,
        IStartInspect,
        ISetImageInfo,
        IDisposable
    {
        InspectionResult InspectionResult { get; }

        IRelativeCoordinate Coordinate { get; }

        HImage Image { get; }

        InspectionSchema InspectionSchema { get; }
    }

    public interface ISetInspectorFactory
    {
        InspectionController SetInspectorFactory(Func<string, IGeneralInspector> inspectorFactory);
    }

    public interface IStartInspect
    {
        ISetInspectionSchema StartInspect();
    }

    public interface ISetInspectionSchema
    {
        ISetImageInfo SetInspectionSchema(string fileName = null);
        ISetImageInfo SetInspectionSchema(InspectionSchema inspectionSchema);
    }

    public interface ICreateCoordinate
    {
        IInspect CreateCoordinate();
    }

    public interface IInspect
    {
        IInspectionController Inspect();
    }

    public interface ISetImageInfo
    {
        ICreateCoordinate SetImageInfo(HImage imageInfo);
    }

    public class InspectionController :
        IInspectionController,
        ISetInspectorFactory,
        IStartInspect,
        ISetInspectionSchema,
        ISetImageInfo,
        ICreateCoordinate,
        IInspect
    {
        private readonly ConcurrentDictionary<string, IGeneralInspector> _inspectors =
            new ConcurrentDictionary<string, IGeneralInspector>();

        private Func<string, IGeneralInspector> _inspectorFactory;

        private IRelativeCoordinate _coordinate;
        private IObservable<InspectionResult> _coordinateCreatedEvent = new Subject<InspectionResult>();
        private HImage _imageInfo;
        private InspectionSchema _inspectionSchema;
        private InspectionResult _inspectionResult;

        public InspectionController()
        {
        }

        public ICreateCoordinate SetImageInfo(HImage imageInfo)
        {
            _inspectionResult = new InspectionResult();
            _imageInfo = imageInfo;

            foreach (var inspector in _inspectors)
            {
                inspector.Value.SetImageInfo(_imageInfo);
            }

            return this;
        }

        public static InspectionSchema GetInspectionSchema()
        {
            var dir = typeof(InspectionController).Assembly.GetAssemblyDirectoryPath();

            var inspectionSchemaDirPath = dir + "\\InspectionSchema";
            var inspectionSchemaFilePath = dir + "\\InspectionSchema\\InspectionSchema.xaml";
            //            var fileName = Path.Combine(inspectionSchemaDirPath, "InspectionSchema.xaml");

            if (!Directory.Exists(inspectionSchemaDirPath))
            {
                Directory.CreateDirectory(inspectionSchemaDirPath);
            }

            if (!File.Exists(inspectionSchemaFilePath))
            {
                var ds = InspectionSchemaFactory.CreateDefaultSchema();
                ds.SerializeToXamlFile(inspectionSchemaFilePath);
            }

            var schema = inspectionSchemaFilePath.DeserializeFromXamlFile<InspectionSchema>();


            var files = Directory.GetFiles(inspectionSchemaDirPath);
            foreach (var file in files)
            {
                if (file == inspectionSchemaFilePath)
                    continue;

                var slaveSchema = file.DeserializeFromXamlFile<InspectionSchema>();
                if (!slaveSchema.Disabled)
                    schema.Merge(slaveSchema);
            }

            return schema;
        }

        public ISetImageInfo SetInspectionSchema(string fileName = null)
        {
            var inspectionSchema = string.IsNullOrEmpty(fileName)
                ? GetInspectionSchema()
                : fileName.DeserializeFromXamlFile<InspectionSchema>();

            ((ISetInspectionSchema)this).SetInspectionSchema(inspectionSchema);

            return this;
        }

        ISetImageInfo ISetInspectionSchema.SetInspectionSchema(InspectionSchema inspectionSchema)
        {
            _inspectionSchema = inspectionSchema;


            GetOrAddInspector(_inspectionSchema.CircleSearching_InspectorName);
            GetOrAddInspector(_inspectionSchema.EdgeSearching_InspectorName);
            GetOrAddInspector(_inspectionSchema.Defects_InspectorName);

            return this;
        }

        public InspectionController CreateCoordinate()
        {
            var sw = new NotifyStopwatch("InspectionController.CreateCoordinate.Inspect()");

            var inspector = GetOrAddInspector(_inspectionSchema.CircleSearching_InspectorName);
            //            if(inspector.)

            switch (_inspectionSchema.CoordinateType)
            {
                case CoordinateType.Baseline:
                    var origin = _inspectionResult.CoordinateCircles[0];
                    var refCircle = _inspectionResult.CoordinateCircles[1];
                    _coordinate = new RelativeCoordinate(
                        origin.Circle.GetCenterPoint(),
                        refCircle.Circle.GetCenterPoint(),
                        refCircle.Definition.BaselineAngle);
                    break;
                case CoordinateType.VectorsCenter:
                    var searchCoordinateCircles = inspector.SearchCircles(_inspectionSchema.CoordinateCircles);
                    _inspectionResult.CoordinateCircles = searchCoordinateCircles;
                    _coordinate = RelativeCoordinateFactory.CreateCoordinate(_inspectionResult.CoordinateCircles);
                    break;
                case CoordinateType.NearOrigin:
                    throw new NotSupportedException("CoordinateType does not implement!");
                    break;
                case CoordinateType.Boarder:
                      var searchCoordinateEdges = inspector.SearchEdges(_inspectionSchema.CoordinateEdges);
                      _inspectionResult.CoordinateEdges = searchCoordinateEdges;
                      _coordinate = RelativeCoordinateFactory.CreateCoordinateUsingBorder(_inspectionResult.CoordinateEdges);
                    break;
                default:
                    throw new NotSupportedException("CoordinateType does not support!");
            }

            if (_inspectionSchema.CoordinateOriginOffsetEnable)
            {
                _coordinate.OriginOffset = new Vector(_inspectionSchema.CoordinateOriginOffsetX,
                    _inspectionSchema.CoordinateOriginOffsetY);
            }

            _inspectionResult.CoordinateCircles.UpdateRelativeCircles(_coordinate);

            // 
            _inspectionSchema.CircleSearchingDefinitions.UpdateObjectiveCircles(_coordinate);
            _inspectionSchema.CoordinateCircles.UpdateObjectiveCircles(_coordinate);
            _inspectionSchema.EdgeSearchingDefinitions.UpdateFromRelativeLines(_coordinate);
            //            _inspectionSchema.CoordinateEdges.UpdateObjectiveCircles(_coordinate);

            sw.Dispose();

            return this;
        }

        public IInspectionController Inspect()
        {
            var inspector = GetOrAddInspector(_inspectionSchema.EdgeSearching_InspectorName);

            var sw2 = new NotifyStopwatch("Inspector.Inspect()");
            var inspectionResult = inspector.Inspect(_inspectionSchema);
            sw2.Dispose();

            _inspectionResult.Circles = inspectionResult.Circles;
            _inspectionResult.Edges = inspectionResult.Edges;
            _inspectionResult.DistanceBetweenPointsResults = inspectionResult.DistanceBetweenPointsResults;
            _inspectionResult.DefectResults = inspectionResult.DefectResults;

            return this;
        }

        public IGeneralInspector GetOrAddInspector(string inspectorName)
        {
            if (!_inspectors.ContainsKey(inspectorName))
            {
                Debug.WriteLine("_inspectorFactory create: " + inspectorName);
                var newInspector = _inspectorFactory(inspectorName);
                var isAdded = _inspectors.TryAdd(inspectorName, newInspector);
                if (!isAdded)
                    throw new Exception("inspectors.ContainsKey, TryAdd failed");
            }

            var inspector = _inspectors[inspectorName];
            return inspector;
        }

        public IRelativeCoordinate Coordinate
        {
            get { return _coordinate; }
        }

        public HImage Image
        {
            get { return _imageInfo; }
        }

        public InspectionSchema InspectionSchema
        {
            get { return _inspectionSchema; }
        }

        public InspectionResult InspectionResult
        {
            get { return _inspectionResult; }
        }

        public IObservable<InspectionResult> CoordinateCreatedEvent
        {
            get { return _coordinateCreatedEvent; }
            set { _coordinateCreatedEvent = value; }
        }

        public void Dispose()
        {
            foreach (var inspector in _inspectors)
            {
                inspector.Value.Dispose();
            }
        }

        public InspectionController SetInspectorFactory(Func<string, IGeneralInspector> inspectorFactory)
        {
            _inspectorFactory = inspectorFactory;

            return this;
        }

        public ISetInspectionSchema StartInspect()
        {
            var dir = typeof(Ex).Assembly.GetAssemblyDirectoryPath();
            var cacheDir = Path.Combine(dir, "CacheImages");
            if (Directory.Exists(cacheDir))
            {
                foreach (var file in Directory.GetFiles(cacheDir))
                {
                    File.Delete(file);
                }
            }

            return this;
        }

        IInspect ICreateCoordinate.CreateCoordinate()
        {
            return CreateCoordinate();
        }
    }
}