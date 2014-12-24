using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Hdc.Collections.Generic;
using Hdc.Mv.Inspection.Halcon;

namespace Hdc.Mv.Inspection
{
    public class HalconGeneralInspector : IGeneralInspector
    {
        private HDevelopExportHelper _hDevelopExportHelper;
        private SimGeneralInspector _simGeneralInspector = new SimGeneralInspector();


        public void Dispose()
        {
        }

        public void Init()
        {
        }

        public void Init(int width, int height)
        {
        }

        public InspectionResult Inspect(ImageInfo imageInfo, InspectionSchema inspectionSchema)
        {
            Debug.WriteLine("HalconGeneralInspector.Inspect in");

            var inspectionResult = new InspectionResult();

            _hDevelopExportHelper = new HDevelopExportHelper(imageInfo);

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

                var edges = SearchEdges(inspectionSchema.EdgeSearchingDefinitions);
                var finalEdges = new EdgeSearchingResultCollection();
                for (int i = 0; i < edges.Count; i += 2)
                {
                    var esr = edges[i];
                    var esr2 = edges[i + 1];

                    var offset = esr.EdgeLine.GetCenterPoint() - esr2.EdgeLine.GetCenterPoint();
                    var offset2 = esr.Definition.GetLine().GetCenterPoint() - esr2.EdgeLine.GetCenterPoint();

                    if (offset.Length < 8 && offset2.Length < 8)
                    {
                        var x1 = (esr.EdgeLine.X1 + esr2.EdgeLine.X1) / 2.0;
                        var x2 = (esr.EdgeLine.X2 + esr2.EdgeLine.X2) / 2.0;
                        var y1 = (esr.EdgeLine.Y1 + esr2.EdgeLine.Y1) / 2.0;
                        var y2 = (esr.EdgeLine.Y2 + esr2.EdgeLine.Y2) / 2.0;
                        esr.EdgeLine = new Line(x1, y1, x2, y2);
                        finalEdges.Add(esr);
                    }
                    else
                    {
                        finalEdges.Add(esr);
                    }
                }

                inspectionResult.Edges = finalEdges;



//                                var finalEdges = SearchEdges(inspectionSchema.EdgeSearchingDefinitions);
//                                inspectionResult.Edges = finalEdges;



                var results = GetDistanceBetweenPointsResults2(finalEdges);
                inspectionResult.DistanceBetweenPointsResults.AddRange(results);
            }

            return inspectionResult;
        }

        private void GetValue(Collection<EdgeSearchingDefinition> edgeSearchingDefinitions, InspectionResult inspectionResult)
        {
            var r2Def = edgeSearchingDefinitions.Single(x => x.Name == "R2").DeepClone();
            var r2ExpCenter = r2Def.GetLine().GetCenterPoint();
            var r2 = SearchEdges(r2Def).First();
            var r2ActualCenter = r2.EdgeLine.GetCenterPoint();
            inspectionResult.Edges.Add(r2);


            var r1Def = edgeSearchingDefinitions.Single(x => x.Name == "R1").DeepClone();
            var r1ExpCenter = r1Def.GetLine().GetCenterPoint();
            r1Def.StartX = r2ActualCenter.X;
            r1Def.EndX = r2ActualCenter.X;
            var r1 = SearchEdges(r1Def).First();
            var r1ActualCenter = r1.EdgeLine.GetCenterPoint();
            inspectionResult.Edges.Add(r1);


            var r0Def = edgeSearchingDefinitions.Single(x => x.Name == "R0").DeepClone();
            var r0ExpCenter = r0Def.GetLine().GetCenterPoint();
            r0Def.StartX = r1ActualCenter.X;
            r0Def.EndX = r1ActualCenter.X;
            var r0 = SearchEdges(r0Def).First();
            var r0ActualCenter = r0.EdgeLine.GetCenterPoint();
            inspectionResult.Edges.Add(r0);


            var r3Def = edgeSearchingDefinitions.Single(x => x.Name == "R3").DeepClone();
            var r3ExpCenter = r3Def.GetLine().GetCenterPoint();
            r3Def.StartX = r2ActualCenter.X;
            r3Def.EndX = r2ActualCenter.X;
            var r3 = SearchEdges(r3Def).First();
            var r3ActualCenter = r3.EdgeLine.GetCenterPoint();
            inspectionResult.Edges.Add(r3);

            // Right
            var l2Def = edgeSearchingDefinitions.Single(x => x.Name == "L2").DeepClone();
            var l2ExpCenter = l2Def.GetLine().GetCenterPoint();
            var l2 = SearchEdges(l2Def).First();
            var l2ActualCenter = l2.EdgeLine.GetCenterPoint();
            inspectionResult.Edges.Add(l2);


            var l1Def = edgeSearchingDefinitions.Single(x => x.Name == "L1").DeepClone();
            var l1ExpCenter = l1Def.GetLine().GetCenterPoint();
            l1Def.StartX = l2ActualCenter.X;
            l1Def.EndX = l2ActualCenter.X;
            var l1 = SearchEdges(l1Def).First();
            var l1ActualCenter = l1.EdgeLine.GetCenterPoint();
            inspectionResult.Edges.Add(l1);


            var l0Def = edgeSearchingDefinitions.Single(x => x.Name == "L0").DeepClone();
            var l0ExpCenter = l0Def.GetLine().GetCenterPoint();
            l0Def.StartX = l1ActualCenter.X;
            l0Def.EndX = l1ActualCenter.X;
            var l0 = SearchEdges(l0Def).First();
            var l0ActualCenter = l0.EdgeLine.GetCenterPoint();
            inspectionResult.Edges.Add(l0);


            var l3Def = edgeSearchingDefinitions.Single(x => x.Name == "L3").DeepClone();
            var l3ExpCenter = l3Def.GetLine().GetCenterPoint();
            l3Def.StartX = l2ActualCenter.X;
            l3Def.EndX = l2ActualCenter.X;
            var l3 = SearchEdges(l3Def).First();
            var l3ActualCenter = l3.EdgeLine.GetCenterPoint();
            inspectionResult.Edges.Add(l3);


            var distanceL0R0Result = new DistanceBetweenPointsResult()
            {
                Index = 2,
                Name = "L0R0",
                Point1 = l0ActualCenter,
                Point2 = r0ActualCenter,
                DistanceInPixel = (l0ActualCenter - r0ActualCenter).Length,
            };

            var distanceL1R1Result = new DistanceBetweenPointsResult()
            {
                Index = 3,
                Name = "L1R1",
                Point1 = l1ActualCenter,
                Point2 = r1ActualCenter,
                DistanceInPixel = (l1ActualCenter - r1ActualCenter).Length,
            };

            var distanceL2R2Result = new DistanceBetweenPointsResult()
            {
                Index = 4,
                Name = "L2R2",
                Point1 = l2ActualCenter,
                Point2 = r2ActualCenter,
                DistanceInPixel = (l2ActualCenter - r2ActualCenter).Length,
            };

            var distanceL3R3Result = new DistanceBetweenPointsResult()
            {
                Index = 5,
                Name = "L3R3",
                Point1 = l3ActualCenter,
                Point2 = r3ActualCenter,
                DistanceInPixel = (l3ActualCenter - r3ActualCenter).Length,
            };

            List<DistanceBetweenPointsResult> results = new List<DistanceBetweenPointsResult>
            {
                distanceL0R0Result,
                distanceL1R1Result,
                distanceL2R2Result,
                distanceL3R3Result,
                //                distanceT1B1Result,
                //                distanceT2B2Result
            };

            inspectionResult.DistanceBetweenPointsResults.AddRange(results);
        }

        private static List<DistanceBetweenPointsResult> GetDistanceBetweenPointsResults(InspectionResult edges)
        {
            Debug.WriteLine("HalconGeneralInspector.Inspect out");


            var t1Result = edges.Edges.Single(x => x.Name == "T1");
            var t1 = t1Result.EdgeLine.GetCenterPoint();
            var t2Result = edges.Edges.Single(x => x.Name == "T2");
            var t2 = t2Result.EdgeLine.GetCenterPoint();
            var b1R = edges.Edges.Single(x => x.Name == "B1");
            var b1 = b1R.EdgeLine.GetCenterPoint();
            var b2R = edges.Edges.Single(x => x.Name == "B2");
            var b2 = b2R.EdgeLine.GetCenterPoint();
            var l0R = edges.Edges.Single(x => x.Name == "L0");
            var l0 = l0R.EdgeLine.GetCenterPoint();
            var l1R = edges.Edges.Single(x => x.Name == "L1");
            var l1 = l1R.EdgeLine.GetCenterPoint();
            var l2R = edges.Edges.Single(x => x.Name == "L2");
            var l2 = l2R.EdgeLine.GetCenterPoint();
            var l3R = edges.Edges.Single(x => x.Name == "L3");
            var l3 = l3R.EdgeLine.GetCenterPoint();
            var r0R = edges.Edges.Single(x => x.Name == "R0");
            var r0 = r0R.EdgeLine.GetCenterPoint();
            var r1R = edges.Edges.Single(x => x.Name == "R1");
            var r1 = r1R.EdgeLine.GetCenterPoint();
            var r2R = edges.Edges.Single(x => x.Name == "R2");
            var r2 = r2R.EdgeLine.GetCenterPoint();
            var r3R = edges.Edges.Single(x => x.Name == "R3");
            var r3 = r3R.EdgeLine.GetCenterPoint();

            if (t1.X == 0 ||
                t2.X == 0 ||
                b1.X == 0 ||
                b2.X == 0 ||
                l0.X == 0 ||
                l1.X == 0 ||
                l2.X == 0 ||
                l3.X == 0 ||
                l0.X == 0 ||
                r0.X == 0 ||
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

            var distanceL0R0 = (l0 - r0).Length;
            var distanceL1R1 = (l1 - r1).Length;
            var distanceL2R2 = (l2 - r2).Length;
            var distanceL3R3 = (l3 - r3).Length;

            Debug.WriteLine("T1-B1: \t" + distanceT1B1);
            Debug.WriteLine("T2-B2: \t" + distanceT2B2);

            Debug.WriteLine("L0-R0: \t" + distanceL0R0);
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

            var distanceL0R0Result = new DistanceBetweenPointsResult()
            {
                Index = 2,
                Name = "L0R0",
                Point1 = l0,
                Point2 = r0,
                DistanceInPixel = (l0 - r0).Length,
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
                distanceL0R0Result,
                distanceL1R1Result,
                distanceL2R2Result,
                distanceL3R3Result,
                distanceT1B1Result,
                distanceT2B2Result
            };
            return results;
        }
        private static List<DistanceBetweenPointsResult> GetDistanceBetweenPointsResults2(EdgeSearchingResultCollection edges)
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

        private CircleSearchingResultCollection SearchCircles(IList<CircleSearchingDefinition> circleSearchingDefinitions)
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

        public CircleSearchingResultCollection SearchCircles(ImageInfo imageInfo, IList<CircleSearchingDefinition> circleSearchingDefinitions)
        {
            _hDevelopExportHelper = new HDevelopExportHelper(imageInfo);

            return SearchCircles(circleSearchingDefinitions);
        }

        public EdgeSearchingResultCollection SearchEdges(EdgeSearchingDefinition edgeSearchingDefinition)
        {
            return SearchEdges(new[] { edgeSearchingDefinition });
        }
        public EdgeSearchingResultCollection SearchEdges(IList<EdgeSearchingDefinition> edgeSearchingDefinitions)
        {
            var sr = new EdgeSearchingResultCollection();

            foreach (var esd in edgeSearchingDefinitions)
            {
                var esr = new EdgeSearchingResult();
                esr.Definition = esd.DeepClone();
                esr.Name = esd.Name;

                var lines = _hDevelopExportHelper.RakeEdgeLine(
                   line: esd.Line,
                   regionsCount: esd.Hal_RegionsCount,
                   regionHeight: esd.Hal_RegionHeight,
                   regionWidth: esd.Hal_RegionWidth,
                   sigma: esd.Hal_Sigma,
                   threshold: esd.Hal_Threshold,
                   transition: esd.Hal_Transition,
                   selectionMode: esd.Hal_SelectionMode);

                var line = lines[0];
                esr.EdgeLine = line;

                if (line.X1 == 0 || line.X2 == 0 || line.Y1 == 0 || line.Y2 == 0)
                {
                    esr.IsNotFound = true;
                    Debug.WriteLine("Edge not found: " + esr.Name);
                }

                sr.Add(esr);
            }

            return sr;
        }

        public EdgeSearchingResultCollection SearchEdges(ImageInfo imageInfo, IList<EdgeSearchingDefinition> edgeSearchingDefinitions)
        {
            _hDevelopExportHelper = new HDevelopExportHelper(imageInfo);

            return SearchEdges(edgeSearchingDefinitions);
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