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
using Hdc.Linq;
using Hdc.Mv.Halcon;
using Hdc.Reflection;
using Hdc.Windows.Media.Imaging;

namespace Hdc.Mv.Inspection
{
    public class HalconGeneralInspector// : IGeneralInspector
    {
        private static string _cacheImageDir = typeof(Mv.Ex).Assembly.GetAssemblyDirectoryPath() + "\\CacheImages";
        private HImage _hImage;

        IEdgeInspector _edgeInspector = new EdgeInspector();
        ICircleInspector _circleInspector = new CircleInspector();
        ISurfaceInspector _surfaceInspector = new SurfaceInspector();

        public HalconGeneralInspector()
        {
            if (!Directory.Exists(_cacheImageDir))
            {
                Directory.CreateDirectory(_cacheImageDir);
            }
        }

        public void Dispose()
        {
        }

        public void Init()
        {
        }

        public void SetImageInfo(HImage image)
        {
            if (_hImage != null)
                _hImage.Dispose();

            _hImage = image;
        }

        public InspectionResult Inspect(InspectionSchema inspectionSchema)
        {
            Debug.WriteLine("HalconGeneralInspector.Inspect in");

            var inspectionResult = new InspectionResult();

            if (inspectionSchema.CircleSearchingDefinitions.Any())
            {
                var sw = new NotifyStopwatch("SearchCircles()");
                var circles = SearchCircles(inspectionSchema.CircleSearchingDefinitions);
                sw.Dispose();
                inspectionResult.Circles = new CircleSearchingResultCollection(circles);
            }

            if (inspectionSchema.SurfaceDefinitions.Any())
            {
                IList<SurfaceResult> regionResults = null;

                using (new NotifyStopwatch("SearchSurfaces()"))
                    regionResults = SearchSurfaces(inspectionSchema.SurfaceDefinitions);

                DefectResultCollection defectResultCollection = null;
                if (inspectionSchema.DefectDefinitions.Any())
                {
                    defectResultCollection = SearchDefects2(inspectionSchema.DefectDefinitions, regionResults);
                    inspectionResult.DefectResults = defectResultCollection;
                    //                inspectionResult.ClosedRegionResults = regionResults;
                }
            }

            if (inspectionSchema.EdgeSearching_SaveCacheImage_Disabled)
            {
                foreach (var def in inspectionSchema.EdgeSearchingDefinitions)
                {
                    def.Domain_SaveCacheImageEnabled = false;
                    def.RegionExtractor_Disabled = false;
                    def.ImageFilter_SaveCacheImageEnabled = false;
                }
            }

            var finalEdges = SearchEdges(inspectionSchema.EdgeSearchingDefinitions);
            inspectionResult.Edges = new EdgeSearchingResultCollection(finalEdges);

            int i = 0;
            foreach (var def in inspectionSchema.DistanceBetweenIntersectionPointsDefinitions)
            {
                var line1 = finalEdges.Single(x => x.Name == def.Line1Name);
                var line2 = finalEdges.Single(x => x.Name == def.Line2Name);

                var line1Center = line1.Definition.Line.GetCenterPoint();
                var line2Center = line2.Definition.Line.GetCenterPoint();

                var linkLine = new Line(line1Center, line2Center);

                var intersection1 = line1.EdgeLine.IntersectionWith(linkLine);
                var intersection2 = line2.EdgeLine.IntersectionWith(linkLine);

                if (Math.Abs(intersection1.X) < 0.00001 ||
                    Math.Abs(intersection2.X) < 0.00001)
                {
                    Debug.WriteLine(@"DistanceBetweenIntersectionPointsDefinitions failed: {0}", def.Name);
                }

                //var distance = t1.ToVector() - t2.ToVector();
                var distance = (intersection1 - intersection2).Length;

                Debug.WriteLine("Distance {0}: {1}\t", def.Name, distance);

                var distanceBetweenPointsResult = new DistanceBetweenPointsResult()
                                                  {
                                                      Definition = def.DeepClone(),
                                                      Index = i,
                                                      Name = def.Name,
                                                      Point1 = intersection1,
                                                      Point2 = intersection2,
                                                      DistanceInPixel = (intersection1 - intersection2).Length,
                                                      DistanceInWorld =
                                                          (intersection1 - intersection2).Length
                                                          .ToMillimeterFromPixel(16),
                                                  };

                inspectionResult.DistanceBetweenPointsResults.Add(distanceBetweenPointsResult);

                i++;
            }

            //                var results = GetDistanceBetweenPointsResults2(finalEdges);
            //                inspectionResult.DistanceBetweenPointsResults.AddRange(results);


            return inspectionResult;
        }

        private DefectResultCollection SearchDefects2(IList<DefectDefinition> defectDefinitions,
                                                      IList<SurfaceResult> surfaceResults)
        {
            var drs = new DefectResultCollection();

            foreach (var defectDefinition in defectDefinitions)
            {
                foreach (var refer in defectDefinition.RegionReferences)
                {
                    //var surface = surfaceResults.Single(x => x.Definition.Name == regionReference.SurfaceName);
                    //var region = surface.IncludeRegionResults.Single(x => x.RegionName == regionReference.RegionName);

                    var regionResult = surfaceResults.GetRegionResult(refer.SurfaceName, refer.RegionName);

                    //                    _hImage
                    //                        .ReduceDomain(regionResult.Region)
                    //                        .CropDomain()
                    //                        .ToBitmapSource()
                    //                        .SaveToJpeg("_regionResult_" + refer.RegionName + "_ExtractedRegion.jpg");

                    var blob = defectDefinition.Extractor.GetDefectRegion(_hImage, regionResult.Region);
                    //var selectedBlob = defectDefinition.
                    var blobs = blob.ToList();

                    int index = 0;
                    foreach (var hRegion in blobs)
                    {
                        var dr = new DefectResult
                                 {
                                     Index = index,
                                     X = hRegion.GetColumn(),
                                     Y = hRegion.GetRow(),
                                     Width = hRegion.GetWidth(),
                                     Height = hRegion.GetHeight(),
                                     Name = defectDefinition.Name,
                                 };
                        drs.Add(dr);
                        index++;
                    }

                    if (defectDefinition.SaveCacheImageEnabled)
                    {
                        continue;
                        var fileName = "SurfaceDefinition_Defects_" + defectDefinition.Name;
                        _hImage.SaveCacheImagesForRegion(blob.Boundary("inner"), blob, fileName);
                    }
                }
            }

            return drs;
        }

        private IList<SurfaceResult> SearchSurfaces(IList<SurfaceDefinition> surfaceDefinitions)
        {
            var results = _surfaceInspector.SearchSurfaces(_hImage, surfaceDefinitions);
            return results;
        }

        public IList<CircleSearchingResult> SearchCircles(
            IList<CircleSearchingDefinition> circleSearchingDefinitions)
        {
            var results = _circleInspector.SearchCircles(_hImage, circleSearchingDefinitions);
            return results;
        }

        public IList<EdgeSearchingResult> SearchEdges(IList<EdgeSearchingDefinition> edgeSearchingDefinitions)
        {
            var results = _edgeInspector.SearchEdges(_hImage, edgeSearchingDefinitions);
            return results;
        }
    }
}