using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using HalconDotNet;
using Hdc.Collections.Generic;
using Hdc.Diagnostics;
using Hdc.Mv.Inspection.Halcon;
using Hdc.Windows.Media.Imaging;

namespace Hdc.Mv.Inspection
{
    public class HalconGeneralInspector : IGeneralInspector
    {
        private HDevelopExportHelper _hDevelopExportHelper;
        private SimGeneralInspector _simGeneralInspector = new SimGeneralInspector();
        private HImage _hImage;

        public void Dispose()
        {
        }

        public void Init()
        {
        }

        public void SetImageInfo(ImageInfo imageInfo)
        {
            if (_hDevelopExportHelper != null)
                _hDevelopExportHelper.Dispose();
            if (_hImage != null)
                _hImage.Dispose();

            _hImage = imageInfo.To8BppHImage();
            _hDevelopExportHelper = new HDevelopExportHelper(_hImage);
        }

        public InspectionResult Inspect(InspectionSchema inspectionSchema)
        {
            Debug.WriteLine("HalconGeneralInspector.Inspect in");

            var inspectionResult = new InspectionResult();

            var circles = SearchCircles(inspectionSchema.CircleSearchingDefinitions);

            inspectionResult.Circles = circles;

            {
                //                var esds = new List<EdgeSearchingDefinition>();
                //                foreach (var esd in inspectionSchema.EdgeSearchingDefinitions)
                //                {
                //                    var esd2 = esd.DeepClone();
                //                    esd2.Hal_SelectionMode = SelectionMode.First;
                //                    esd2.Hal_Transition = Transition.Negative;
                //                    esd2.Name = esd.Name + "b";
                //                    esd2.Hal_Threshold = 6;
                //                    esds.Add(esd);
                //                    esds.Add(esd2);
                //                }


                //                    var x1 = esr.EdgeLine.X1 + (esr2.EdgeLine.X1 - esr.EdgeLine.X1) / 3.0;
                //                    var y1 = esr.EdgeLine.Y1 + (esr2.EdgeLine.Y1 - esr.EdgeLine.Y1) / 3.0;
                //                    var x2 = esr.EdgeLine.X2 + (esr2.EdgeLine.X2 - esr.EdgeLine.X2) / 3.0;
                //                    var y2 = esr.EdgeLine.Y2 + (esr2.EdgeLine.Y2 - esr.EdgeLine.Y2) / 3.0;

                //                var edges = SearchEdges(inspectionSchema.EdgeSearchingDefinitions);
                //                var finalEdges = new EdgeSearchingResultCollection();
                //                for (int i = 0; i < edges.Count; i += 2)
                //                {
                //                    var esr = edges[i];
                //                    var esr2 = edges[i + 1];
                //
                //                    var offset = esr.EdgeLine.GetCenterPoint() - esr2.EdgeLine.GetCenterPoint();
                //                    var offset2 = esr.Definition.GetLine().GetCenterPoint() - esr2.EdgeLine.GetCenterPoint();
                //
                //                    if (offset.Length < 8 && offset2.Length < 8)
                //                    {
                //                        var x1 = (esr.EdgeLine.X1 + esr2.EdgeLine.X1) / 2.0;
                //                        var x2 = (esr.EdgeLine.X2 + esr2.EdgeLine.X2) / 2.0;
                //                        var y1 = (esr.EdgeLine.Y1 + esr2.EdgeLine.Y1) / 2.0;
                //                        var y2 = (esr.EdgeLine.Y2 + esr2.EdgeLine.Y2) / 2.0;
                //                        esr.EdgeLine = new Line(x1, y1, x2, y2);
                //                        finalEdges.Add(esr);
                //                    }
                //                    else
                //                    {
                //                        finalEdges.Add(esr);
                //                    }
                //                }
                //
                //                inspectionResult.Edges = finalEdges;


                var finalEdges = SearchEdges(inspectionSchema.EdgeSearchingDefinitions,
                    inspectionSchema.EdgeSearching_EnhanceEdgeArea_Enable);
                inspectionResult.Edges = finalEdges;

                if (inspectionSchema.EdgeSearching_EnhanceEdgeArea_Enable &&
                    inspectionSchema.EdgeSearching_EnhanceEdgeArea_SaveCacheImageEnable)
                {
                    var ii = _hDevelopExportHelper.HImageCache.ToImageInfo();
                    var bitmapSource = ii.ToBitmapSource();
                    if (!Directory.Exists(@"Cache")) Directory.CreateDirectory("Cache");
                    bitmapSource.SaveToTiff(@"Cache\EdgeSearchingCacheImage.tif");
                }

                var results = GetDistanceBetweenPointsResults2(finalEdges);
                inspectionResult.DistanceBetweenPointsResults.AddRange(results);
            }

            return inspectionResult;
        }

        public void Init(int width, int height)
        {
        }

        public InspectionResult Inspect(ImageInfo imageInfo, InspectionSchema inspectionSchema)
        {
            SetImageInfo(imageInfo);
            return Inspect(inspectionSchema);
        }


        private static List<DistanceBetweenPointsResult> GetDistanceBetweenPointsResults2(
            EdgeSearchingResultCollection edges)
        {
            Debug.WriteLine("HalconGeneralInspector.Inspect out");


            var t1Result = edges.Single(x => x.Name == "T1");
            var t1 = t1Result.EdgeLine.GetCenterPoint();
            var t2Result = edges.Single(x => x.Name == "T2");
            var t2 = t2Result.EdgeLine.GetCenterPoint();
            var b1R = edges.Single(x => x.Name == "B1");
            var b1 = b1R.EdgeLine.GetCenterPoint();
            var b2R = edges.Single(x => x.Name == "B2");
            var b2 = b2R.EdgeLine.GetCenterPoint();
            var l1R = edges.Single(x => x.Name == "L1");
            var l1 = l1R.EdgeLine.GetCenterPoint();
            var l2R = edges.Single(x => x.Name == "L2");
            var l2 = l2R.EdgeLine.GetCenterPoint();
            var l3R = edges.Single(x => x.Name == "L3");
            var l3 = l3R.EdgeLine.GetCenterPoint();
            var r1R = edges.Single(x => x.Name == "R1");
            var r1 = r1R.EdgeLine.GetCenterPoint();
            var r2R = edges.Single(x => x.Name == "R2");
            var r2 = r2R.EdgeLine.GetCenterPoint();
            var r3R = edges.Single(x => x.Name == "R3");
            var r3 = r3R.EdgeLine.GetCenterPoint();

            if (t1.X == 0 ||
                t2.X == 0 ||
                b1.X == 0 ||
                b2.X == 0 ||
                l1.X == 0 ||
                l2.X == 0 ||
                l3.X == 0 ||
                r1.X == 0 ||
                r2.X == 0 ||
                r3.X == 0
                )
            {
                Debug.WriteLine("T, B, L, R == 0");
            }

            //var distance = t1.ToVector() - t2.ToVector();
            var distanceT1B1 = (t1 - b1).Length;
            var distanceT2B2 = (t2 - b2).Length;

            var distanceL1R1 = (l1 - r1).Length;
            var distanceL2R2 = (l2 - r2).Length;
            var distanceL3R3 = (l3 - r3).Length;

            Debug.WriteLine("T1-B1: \t" + distanceT1B1);
            Debug.WriteLine("T2-B2: \t" + distanceT2B2);

            Debug.WriteLine("L1-R1: \t" + distanceL1R1);
            Debug.WriteLine("L2-R2: \t" + distanceL2R2);
            Debug.WriteLine("L3-R3: \t" + distanceL3R3);

            var distanceT1B1Result = new DistanceBetweenPointsResult()
                                     {
                                         Index = 0,
                                         Name = "T1B1",
                                         Point1 = t1,
                                         Point2 = b1,
                                         DistanceInPixel = (t1 - b1).Length,
                                     };

            var distanceT2B2Result = new DistanceBetweenPointsResult()
                                     {
                                         Index = 1,
                                         Name = "T2B2",
                                         Point1 = t2,
                                         Point2 = b2,
                                         DistanceInPixel = (t2 - b2).Length,
                                     };

            var distanceL1R1Result = new DistanceBetweenPointsResult()
                                     {
                                         Index = 3,
                                         Name = "L1R1",
                                         Point1 = l1,
                                         Point2 = r1,
                                         DistanceInPixel = (l1 - r1).Length,
                                     };

            var distanceL2R2Result = new DistanceBetweenPointsResult()
                                     {
                                         Index = 4,
                                         Name = "L2R2",
                                         Point1 = l2,
                                         Point2 = r2,
                                         DistanceInPixel = (l2 - r2).Length,
                                     };

            var distanceL3R3Result = new DistanceBetweenPointsResult()
                                     {
                                         Index = 5,
                                         Name = "L3R3",
                                         Point1 = l3,
                                         Point2 = r3,
                                         DistanceInPixel = (l3 - r3).Length,
                                     };

            List<DistanceBetweenPointsResult> results = new List<DistanceBetweenPointsResult>
                                                        {
                                                            distanceL1R1Result,
                                                            distanceL2R2Result,
                                                            distanceL3R3Result,
                                                            distanceT1B1Result,
                                                            distanceT2B2Result
                                                        };
            return results;
        }

        private CircleSearchingResultCollection SearchCircles(
            IList<CircleSearchingDefinition> circleSearchingDefinitions)
        {
            var result = new CircleSearchingResultCollection();

            // ExtractCircle
            int index = 0;
            foreach (var circleDefinition in circleSearchingDefinitions)
            {
                //                if (!inspectionSchema.CircleSearchingEnable) continue;

                var circleSearchingResult = new CircleSearchingResult
                                            {
                                                Definition = circleDefinition.DeepClone(),
                                                Name = circleDefinition.Name,
                                                Index = index
                                            };

                Circle foundCircle;
                double roundless;

                //                var isOK = _hDevelopExportHelper.ExtractCircle(circleDefinition.CenterX, circleDefinition.CenterY,
                //                    circleDefinition.InnerRadius, circleDefinition.OuterRadius, out foundCircle, out roundless);
                var isOK = _hDevelopExportHelper.ExtractCircle(
                    circleDefinition.CenterX,
                    circleDefinition.CenterY,
                    circleDefinition.InnerRadius,
                    circleDefinition.OuterRadius,
                    out foundCircle, out roundless,
                    circleDefinition.Hal_RegionsCount,
                    //                    circleDefinition.Hal_RegionHeight,
                    circleDefinition.Hal_RegionWidth,
                    circleDefinition.Hal_Sigma,
                    circleDefinition.Hal_Threshold,
                    circleDefinition.Hal_SelectionMode,
                    circleDefinition.Hal_Transition,
                    circleDefinition.Hal_Direct
                    );

                if (isOK)
                {
                    circleSearchingResult.Circle = new Circle(
                        foundCircle.CenterX,
                        foundCircle.CenterY,
                        foundCircle.Radius);

                    circleSearchingResult.Roundness = roundless;
                }
                else
                {
                    circleSearchingResult.HasError = true;
                    circleSearchingResult.IsNotFound = true;
                }

                result.Add(circleSearchingResult);

                index++;
            }

            return result;
        }

        public CircleSearchingResultCollection SearchCircles(ImageInfo imageInfo,
                                                             IList<CircleSearchingDefinition> circleSearchingDefinitions)
        {
            _hDevelopExportHelper = new HDevelopExportHelper(imageInfo);

            return SearchCircles(circleSearchingDefinitions);
        }

        public EdgeSearchingResultCollection SearchEdges(ImageInfo imageInfo,
                                                         IList<EdgeSearchingDefinition> edgeSearchingDefinitions)
        {
            throw new NotImplementedException();
        }

        public EdgeSearchingResultCollection SearchEdges(EdgeSearchingDefinition edgeSearchingDefinition,
                                                         bool enhanceEdgeAreaEnabled)
        {
            return SearchEdges(new[] {edgeSearchingDefinition}, enhanceEdgeAreaEnabled);
        }

        public EdgeSearchingResultCollection SearchEdges(IList<EdgeSearchingDefinition> edgeSearchingDefinitions,
                                                         bool enhanceEdgeAreaEnabled)
        {
            var sr = new EdgeSearchingResultCollection();

            HImage mergedImage = null;
            //            foreach (var esd in edgeSearchingDefinitions)
            //            {
            //                if (enhanceEdgeAreaEnabled && !esd.Hal_EnhanceEdgeArea_Enabled)
            //                    continue;
            //
            //                var image = _hDevelopExportHelper.EnhanceEdgeArea(
            //                    _hDevelopExportHelper.HImage,
            //                    line: esd.Line,
            //                    hv_RoiWidthLen: esd.ROIWidth,
            //                    hv_EmpMaskWidth: esd.Hal_EnhanceEdgeArea_EmpMaskWidth,
            //                    hv_EmpMaskHeight: esd.Hal_EnhanceEdgeArea_EmpMaskHeight,
            //                    hv_EmpMaskFactor: esd.Hal_EnhanceEdgeArea_EmpMaskFactor,
            //                    hv_MeanMaskWidth: esd.Hal_EnhanceEdgeArea_MeanMaskWidth,
            //                    hv_MeanMaskHeight: esd.Hal_EnhanceEdgeArea_MeanMaskHeight,
            //                    hv_IterationCount: esd.Hal_EnhanceEdgeArea_IterationCount
            //                    );
            //                if (hImage == null) hImage = image;
            //                else
            //                {
            //                    hImage = _hDevelopExportHelper.AddImagesWithFullDomain(hImage, image);
            //                }
            //            }

            foreach (var esd in edgeSearchingDefinitions)
            {
                var esr = new EdgeSearchingResult();
                esr.Definition = esd.DeepClone();
                esr.Name = esd.Name;

                HImage image;

                if (esr.Definition.Hal_EnhanceEdgeArea_Enabled && enhanceEdgeAreaEnabled)
                {
                    var sw = new NotifyStopwatch("EnhanceEdgeArea");
                    sw.Start();

                    var enhImage = _hDevelopExportHelper.EnhanceEdgeArea(
                        _hDevelopExportHelper.HImage,
                        line: esd.Line,
                        hv_RoiWidthLen: esd.ROIWidth,
                        hv_EmpMaskWidth: esd.Hal_EnhanceEdgeArea_EmpMaskWidth,
                        hv_EmpMaskHeight: esd.Hal_EnhanceEdgeArea_EmpMaskHeight,
                        hv_EmpMaskFactor: esd.Hal_EnhanceEdgeArea_EmpMaskFactor,
                        hv_MeanMaskWidth: esd.Hal_EnhanceEdgeArea_MeanMaskWidth,
                        hv_MeanMaskHeight: esd.Hal_EnhanceEdgeArea_MeanMaskHeight,
                        hv_IterationCount: esd.Hal_EnhanceEdgeArea_IterationCount
                        );

                    sw.Stop();
                    sw.Dispose();
                    

                    image = enhImage;
//                    image = enhImage.Clone();
//
//                    if (mergedImage == null) mergedImage = image;
//                    else
//                    {
//                        var merge = _hDevelopExportHelper.AddImagesWithFullDomain(mergedImage, image);
//                        mergedImage.Dispose();
//                        image.Dispose();
//                        mergedImage = merge;
//                    }
//
//                    enhImage.Dispose();
                }
                else
                {
                    image = _hDevelopExportHelper.HImage;
                }
                var sw2 = new NotifyStopwatch("RakeEdgeLine");
                sw2.Start();

                var lines = _hDevelopExportHelper.RakeEdgeLine(image,
                    line: esd.Line,
                    regionsCount: esd.Hal_RegionsCount,
                    regionHeight: esd.Hal_RegionHeight,
                    regionWidth: esd.Hal_RegionWidth,
                    sigma: esd.Hal_Sigma,
                    threshold: esd.Hal_Threshold,
                    transition: esd.Hal_Transition,
                    selectionMode: esd.Hal_SelectionMode);

                sw2.Stop();
                sw2.Dispose();

                var line = lines[0];
                esr.EdgeLine = line;

                if (line.X1 == 0 || line.X2 == 0 || line.Y1 == 0 || line.Y2 == 0)
                {
                    esr.IsNotFound = true;
                    Debug.WriteLine("Edge not found: " + esr.Name);
                }

                sr.Add(esr);
            }

            _hDevelopExportHelper.HImageCache = mergedImage;

            return sr;
        }

        public EdgeSearchingResultCollection SearchEdges(ImageInfo imageInfo,
                                                         IList<EdgeSearchingDefinition> edgeSearchingDefinitions,
                                                         bool enhanceEdgeAreaEnabled)
        {
            _hDevelopExportHelper = new HDevelopExportHelper(imageInfo);

            return SearchEdges(edgeSearchingDefinitions, enhanceEdgeAreaEnabled);
        }

        public DefectResultCollection SearchDefects(ImageInfo imageInfo)
        {
            throw new NotImplementedException();
        }

        public DefectResultCollection SearchDefects(ImageInfo imageInfo, ImageInfo mask)
        {
            throw new NotImplementedException();
        }

        public ImageInfo FindRegions(ImageInfo imageInfo)
        {
            var ii = _hDevelopExportHelper.FindS1404SurfaceA();
            return ii;
        }

        public ImageInfo FindRegions()
        {
            var ii = _hDevelopExportHelper.FindS1404SurfaceA();
            return ii;
        }

        public DistanceBetweenLinesResult GetDistanceBetweenLines(Line line1, Line line2)
        {
            throw new System.NotImplementedException();
        }

        public DistanceBetweenPointsResult GetDistanceBetweenPoints(Point point1, Point point2)
        {
            throw new System.NotImplementedException();
        }

        //        public InspectionResult SearchEdges(ImageInfo imageInfo, InspectionSchema inspectionSchema)
        //        {
        //        }
    }
}