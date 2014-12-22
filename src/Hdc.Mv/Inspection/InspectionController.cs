﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows;
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

        ImageInfo ImageInfo { get; }

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
        ICreateCoordinate SetImageInfo(ImageInfo imageInfo);
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
        private readonly ConcurrentDictionary<string, IGeneralInspector> _inspectors = new ConcurrentDictionary<string, IGeneralInspector>();

        private Func<string, IGeneralInspector> _inspectorFactory;



        //        private IGeneralInspector _inspector;
        private IRelativeCoordinate _coordinate;
        private IObservable<InspectionResult> _coordinateCreatedEvent = new Subject<InspectionResult>();
        private ImageInfo _imageInfo;
        private InspectionSchema _inspectionSchema;
        private InspectionResult _inspectionResult;

        public ICreateCoordinate SetImageInfo(ImageInfo imageInfo)
        {
            _inspectionResult = new InspectionResult();
            _imageInfo = imageInfo;
            return this;
        }

        //        public InspectionController SetInspector(IGeneralInspector inspector)
        //        {
        //            _inspector = inspector;
        //            return this;
        //        }

        //        public InspectionController SetInspector(Func<InspectionSchema, IGeneralInspector> getInspector)
        //        {
        //            _inspector = getInspector(_inspectionSchema);
        //            return this;
        //        }

        public static InspectionSchema GetInspectionSchema()
        {
            var dir = typeof(InspectionController).Assembly.GetAssemblyDirectoryPath();
            var fileName = Path.Combine(dir, "InspectionSchema.xaml");
            if (!File.Exists(fileName))
            {
                var ds = InspectionSchemaFactory.CreateDefaultSchema();
                ds.SerializeToXamlFile(fileName);
            }
            var schema = fileName.DeserializeFromXamlFile<InspectionSchema>();
            return schema;
        }

        public ISetImageInfo SetInspectionSchema(string fileName = null)
        {
            _inspectionSchema = string.IsNullOrEmpty(fileName)
                ? GetInspectionSchema()
                : fileName.DeserializeFromXamlFile<InspectionSchema>();
            return this;
        }

        ISetImageInfo ISetInspectionSchema.SetInspectionSchema(InspectionSchema inspectionSchema)
        {
            _inspectionSchema = inspectionSchema;
            return this;
        }

        //        public InspectionController SetInspectionSchema(InspectionSchema inspectionSchema)
        //        {
        //            _inspectionSchema = inspectionSchema;
        //            return this;
        //        }

        public InspectionController CreateCoordinate()
        {
            var inspector = GetOrAddInspector(_inspectionSchema.InspectorNameForCircles);

            var searchCoordinateCircles = inspector.SearchCoordinateCircles(_imageInfo, _inspectionSchema);
            _inspectionResult.CoordinateCircles = searchCoordinateCircles;

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
                    _coordinate = RelativeCoordinateFactory.CreateCoordinate(_inspectionResult.CoordinateCircles);
                    break;
                case CoordinateType.NearOrigin:
                    throw new NotSupportedException("CoordinateType does not implement!");
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

            // print
            /*            {
                            List<Vector> actualVectors = _inspectionResult.CoordinateCircles.Select(x => x.Circle.GetCenterVector()).ToList();
                            var relativeCs = actualVectors.Select(x => _coordinate.GetRelativePoint(x.ToPoint())).ToList();

                            Debug.WriteLine("--------------------------------");
                            for (int index = 1; index < relativeCs.Count; index++)
                            {
                                var distance = relativeCs[index];
                                Debug.WriteLine("C.X = \t" + distance.X * 16);
                                Debug.WriteLine("C.Y = \t" + distance.Y * 16);
                            }
                        }
                     */
            // 
            Debug.WriteLine("\n");
            foreach (var cir in _inspectionResult.CoordinateCircles)
            {
                //                var relativeVector = new Vector(def.BaselineX * 1000.0 / 16.0, def.BaselineY * 1000.0 / 16.0);
                //                var originalVector = coord.GetOriginalVector(relativeVector);
                //                def.CenterX = originalVector.X;
                //                def.CenterY = originalVector.Y;

                Debug.WriteLine("EXP.X = \t" + cir.Definition.BaselineX);
                Debug.WriteLine("EXP.Y = \t" + cir.Definition.BaselineY);
                Debug.WriteLine("REL.X = \t" + cir.RelativeCircle.CenterX);
                Debug.WriteLine("REL.Y = \t" + cir.RelativeCircle.CenterY);
                Debug.WriteLine("ACT.X = \t" + cir.Circle.CenterX);
                Debug.WriteLine("ACT.Y = \t" + cir.Circle.CenterY);
                Debug.WriteLine("\n");
            }

            return this;
        }

        public IInspectionController Inspect()
        {
            // circles
            var circlesInspector = GetOrAddInspector(_inspectionSchema.InspectorNameForCircles);
            var circlesResult = circlesInspector.Inspect(_imageInfo, _inspectionSchema);
            _inspectionResult.Circles = circlesResult.Circles;

            // edges
            var edgesInspector = GetOrAddInspector(_inspectionSchema.InspectorNameForEdges);
            var edgesResult = edgesInspector.Inspect(_imageInfo, _inspectionSchema);
            _inspectionResult.Edges = edgesResult.Edges;
            _inspectionResult.DistanceBetweenPointsResults = edgesResult.DistanceBetweenPointsResults;

            // defects
            var halInspector = GetOrAddInspector("Hal");
            var maskImageInfo = halInspector.FindRegions();

            var inspector = GetOrAddInspector(_inspectionSchema.InspectorNameForDefects);
            var defects = inspector.SearchDefects(_imageInfo, maskImageInfo);
            _inspectionResult.DefectResults = defects;

            return this;
        }

        public IGeneralInspector GetOrAddInspector(string inspectorName)
        {
            if (!_inspectors.ContainsKey(inspectorName))
            {
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

        public ImageInfo ImageInfo
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
            return this;
        }


        IInspect ICreateCoordinate.CreateCoordinate()
        {
            return CreateCoordinate();
        }
    }
}