using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Hdc.Collections.Generic;
using Hdc.Mv;
using Hdc.Mv.Inspection;
using Hdc.Mv.Inspection.Mil;
using Hdc.Mvvm.Dialogs;
using Hdc.Reflection;
using Hdc.Serialization;
using ODM.Domain.Configs;
using Omu.ValueInjecter;

namespace ODM.Domain.Inspection
{

    public class AdaptedInspector2 : IInspector
    {
        //public IGeneralInspector _Inspector;

        public MachineConfig MachineConfig { get; set; }

        public void Dispose()
        {
            _inspectionController.Dispose();
        }

        private readonly IInspectionController _inspectionController = new InspectionController();

        public bool Init()
        {
            //var schema = InspectionController.GetInspectionSchema();

            var inspectorFactory = new Func<string, IGeneralInspector>(
                name =>
                {
                    switch (name)
                    {
                        case "Sim":
                            {
                                var sim = new SimGeneralInspector();
                                return sim;
                            }
                            break;
                        case "Mil":
                            {
                                var mi = new MilGeneralInspector();
                                int imageWidth = MachineConfig.MV_LineScanFrameWidth;
                                int imageHeight = MachineConfig.MV_LineScanFrameHeight * MachineConfig.MV_LineScanMosaicCount;

                                mi.Init(imageWidth, imageHeight);

                                return mi;
                            }
                            break;
                        case "Hal":
                            {
                                var hi = new HalconGeneralInspector();
                                return hi;
                            }
                            break;
                        default:
                            throw new NotSupportedException("InspectionSchema.InspectorName not be set!");
                    }
                });

            _inspectionController.SetInspectorFactory(inspectorFactory);

            return true;
        }

        public bool LoadParameters()
        {
            return true;
        }

        public InspectionInfo Inspect(Hdc.Mv.ImageInfo imageInfo)
        {
            _inspectionController
                .StartInspect()
                .SetInspectionSchema()
                .SetImageInfo(imageInfo)
                .CreateCoordinate()
                .Inspect()
                ;

            var inspectionInfo = new InspectionInfo();
            var originAResult = _inspectionController.InspectionResult.CoordinateCircles[0];

            var coord = _inspectionController.Coordinate;

            for (int i = 1; i < _inspectionController.InspectionResult.Circles.Count; i++)
            {
                var circleSearchingResult = _inspectionController.InspectionResult.Circles[i];
                if (!circleSearchingResult.HasError)
                {
                    var measurement = new Hdc.Mv.Inspection.MeasurementInfo()
                                      {
                                          StartPointX = _inspectionController.Coordinate.GetOriginalVector(new Vector(0, 0)).X,
                                          StartPointY = _inspectionController.Coordinate.GetOriginalVector(new Vector(0, 0)).Y,
                                          EndPointX = circleSearchingResult.RelativeCircle.CenterX,
                                          EndPointY = circleSearchingResult.RelativeCircle.CenterY,
                                      };

                    var relativeVector = circleSearchingResult.RelativeCircle.GetCenterVector();

                    measurement.Index = i;
                    measurement.GroupIndex = i;
                    measurement.Value = relativeVector.Length.ToMillimeterFromPixel(16);
                    measurement.StartPointXActualValue = relativeVector.X.ToMillimeterFromPixel(16); // Relative X
                    measurement.StartPointYActualValue = relativeVector.Y.ToMillimeterFromPixel(16); // Relative Y
                    //                                measurement.EndPointXActualValue = relativePoint.X * 16 / 1000.0;
                    //                                measurement.EndPointYActualValue = relativePoint.Y * 16 / 1000.0;
                    measurement.ValueActualValue = relativeVector.Length.ToMillimeterFromPixel(16);
                    measurement.ValueActualValue = circleSearchingResult.RelativeCircle.Radius.ToMillimeterFromPixel(16);

                    measurement.EndPointXActualValue = measurement.StartPointXActualValue - circleSearchingResult.Definition.BaselineX; // Relative X Diff
                    measurement.EndPointYActualValue = measurement.StartPointYActualValue - circleSearchingResult.Definition.BaselineY; // Relative Y Diff

                    inspectionInfo.MeasurementInfos.Add(measurement);
                }
            }

            for (int i = 0; i < _inspectionController.InspectionResult.DistanceBetweenPointsResults.Count; i++)
            {
                var distanceBetweenPointsResult = _inspectionController.InspectionResult.DistanceBetweenPointsResults[i];
                if (!distanceBetweenPointsResult.HasError)
                {
                    var measurement = new Hdc.Mv.Inspection.MeasurementInfo()
                                      {
                                          Index = i,
                                          TypeCode = 100 + i,
                                          StartPointX = distanceBetweenPointsResult.Point1.X,
                                          StartPointY = distanceBetweenPointsResult.Point1.Y,
                                          EndPointX = distanceBetweenPointsResult.Point2.X,
                                          EndPointY = distanceBetweenPointsResult.Point2.Y,
                                      };

                    measurement.Index = i;
                    measurement.GroupIndex = i;
                    measurement.Value = distanceBetweenPointsResult.DistanceInPixel;
                    measurement.ValueActualValue = distanceBetweenPointsResult.DistanceInPixel.ToMillimeterFromPixel(16);

                    Vector relativeP1 = coord.GetRelativeVector(distanceBetweenPointsResult.Point1.ToVector());
                    Vector relativeP2 = coord.GetRelativeVector(distanceBetweenPointsResult.Point2.ToVector());

                    measurement.StartPointXActualValue = relativeP1.X.ToMillimeterFromPixel(16);
                    measurement.StartPointYActualValue = relativeP1.Y.ToMillimeterFromPixel(16);
                    measurement.EndPointXActualValue = relativeP2.X.ToMillimeterFromPixel(16);
                    measurement.EndPointYActualValue = relativeP2.Y.ToMillimeterFromPixel(16);

                    inspectionInfo.MeasurementInfos.Add(measurement);
                }
            }

            for (int i = 0; i < _inspectionController.InspectionResult.DefectResults.Count; i++)
            {
                var dr = _inspectionController.InspectionResult.DefectResults[i];

                var relPoint = coord.GetRelativeVector(new Vector(dr.X - dr.Width / 2.0, dr.Y - dr.Height / 2.0));

                var di = new Hdc.Mv.Inspection.DefectInfo
                {
                    Index = i,
                    X = dr.X - dr.Width/2.0,
                    Y = dr.Y - dr.Height/2.0,
                    Width = dr.Width,
                    Height = dr.Height,
                    TypeCode = dr.TypeCode,
                    Size = dr.Size * 256.0 / 1000000.0,
                    X_Real = relPoint.X.ToMillimeterFromPixel(16),
                    Y_Real = relPoint.Y.ToMillimeterFromPixel(16),
                    Width_Real = dr.Width.ToMillimeterFromPixel(16),
                    Height_Real = dr.Height.ToMillimeterFromPixel(16),
                    Size_Real = dr.Size * 256.0 / 1000000.0
                };

                inspectionInfo.DefectInfos.Add(di);
            }

            return inspectionInfo;
        }


    }
}