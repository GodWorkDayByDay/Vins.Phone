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
    public interface IInspectionController :
        IStartInspect,
        ISetImageInfo,
        IDisposable
    {
        InspectionResult InspectionResult { get; }

        IRelativeCoordinate Coordinate { get; }

        HImage Image { get; }

        InspectionSchema InspectionSchema { get; }
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
        IStartInspect,
        ISetInspectionSchema,
        ISetImageInfo,
        ICreateCoordinate,
        IInspect
    {
        private IRelativeCoordinate _coordinate;
        private IObservable<InspectionResult> _coordinateCreatedEvent = new Subject<InspectionResult>();
        private HImage _image;
        private InspectionSchema _inspectionSchema;
        private InspectionResult _inspectionResult;

        private HalconGeneralInspector _inspector;

        public ICreateCoordinate SetImageInfo(HImage imageInfo)
        {
            _inspectionResult = new InspectionResult();
            _image = imageInfo;

            return this;
        }

        public static InspectionSchema GetInspectionSchema()
        {
            var dir = typeof (InspectionController).Assembly.GetAssemblyDirectoryPath();

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

            ((ISetInspectionSchema) this).SetInspectionSchema(inspectionSchema);

            return this;
        }

        ISetImageInfo ISetInspectionSchema.SetInspectionSchema(InspectionSchema inspectionSchema)
        {
            _inspectionSchema = inspectionSchema;

            if (_inspector == null)
            {
                _inspector = new HalconGeneralInspector();
            }

            return this;
        }

        public InspectionController CreateCoordinate()
        {
            var sw = new NotifyStopwatch("InspectionController.CreateCoordinate.Inspect()");

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
                    var inspector =
                        InspectorFactory.CreateCircleInspector(_inspectionSchema.CircleSearching_InspectorName);
                    var searchCoordinateCircles = inspector.SearchCircles(_image, _inspectionSchema.CoordinateCircles);
                    _inspectionResult.CoordinateCircles = new CircleSearchingResultCollection(searchCoordinateCircles);
                    _coordinate = RelativeCoordinateFactory.CreateCoordinate(_inspectionResult.CoordinateCircles);
                    break;
                case CoordinateType.NearOrigin:
                    throw new NotSupportedException("CoordinateType does not implement!");
                    break;
                case CoordinateType.Boarder:
                    var inspector2 = InspectorFactory.CreateEdgeInspector(_inspectionSchema.EdgeSearching_InspectorName);
                    var searchCoordinateEdges = inspector2.SearchEdges(_image, _inspectionSchema.CoordinateEdges);
                    _inspectionResult.CoordinateEdges = new EdgeSearchingResultCollection(searchCoordinateEdges);
                    _coordinate =
                        RelativeCoordinateFactory.CreateCoordinateUsingBorder(_inspectionResult.CoordinateEdges);
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
            _inspectionSchema.CoordinateCircles.UpdateObjectiveCircles(_coordinate);
            _inspectionSchema.CoordinateEdges.UpdateFromRelativeLines(_coordinate);
            _inspectionSchema.CircleSearchingDefinitions.UpdateObjectiveCircles(_coordinate);
            _inspectionSchema.EdgeSearchingDefinitions.UpdateFromRelativeLines(_coordinate);

            sw.Dispose();

            return this;
        }

        public IInspectionController Inspect()
        {
            var sw2 = new NotifyStopwatch("Inspector.Inspect()");
            var inspectionResult = _inspector.Inspect(_inspectionSchema);
            sw2.Dispose();

            _inspectionResult.Circles = inspectionResult.Circles;
            _inspectionResult.Edges = inspectionResult.Edges;
            _inspectionResult.DistanceBetweenPointsResults = inspectionResult.DistanceBetweenPointsResults;
            _inspectionResult.DefectResults = inspectionResult.DefectResults;

            return this;
        }

        public IRelativeCoordinate Coordinate
        {
            get { return _coordinate; }
        }

        public HImage Image
        {
            get { return _image; }
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
            _inspector.Dispose();
        }

        public ISetInspectionSchema StartInspect()
        {
            var dir = typeof (Ex).Assembly.GetAssemblyDirectoryPath();
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