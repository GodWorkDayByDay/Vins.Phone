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
using Hdc.Mv.Halcon;
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

        //        private object locker = new object();
        //
        //        public void SetImageInfo(ImageInfo imageInfo)
        //        {
        //            if (_hDevelopExportHelper != null)
        //                _hDevelopExportHelper.Dispose();
        //            if (_hImage != null)
        //                _hImage.Dispose();
        //
        //            _hImage = imageInfo.To8BppHImage();
        //
        //            lock (locker)
        //            {
        //                _hDevelopExportHelper = new HDevelopExportHelper(_hImage);
        //            }
        //        }

        public void SetImageInfo(HImage image)
        {
            if (_hDevelopExportHelper != null)
                _hDevelopExportHelper.Dispose();
            if (_hImage != null)
                _hImage.Dispose();

            _hImage = image;

            //            lock (locker)
            //            {
            _hDevelopExportHelper = new HDevelopExportHelper(_hImage);
            //            }
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
                inspectionResult.Circles = circles;
            }


            if (inspectionSchema.SurfaceDefinitions.Any())
            {
                SurfaceResultCollection regionResults = null;

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


            var finalEdges = SearchEdges(inspectionSchema.EdgeSearchingDefinitions);
            inspectionResult.Edges = finalEdges;

            if (inspectionSchema.EdgeSearching_EnhanceEdgeArea_Enable &&
                inspectionSchema.EdgeSearching_EnhanceEdgeArea_SaveCacheImageEnable)
            {
                //                var ii = _hDevelopExportHelper.HImageCache.ToImageInfo();
                //                var bitmapSource = ii.ToBitmapSource();
                //                if (!Directory.Exists(@"Cache")) Directory.CreateDirectory("Cache");
                //                bitmapSource.SaveToTiff(@"Cache\EdgeSearchingCacheImage.tif");
            }

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
                        _hDevelopExportHelper.HImage.SaveCacheImagesForRegion(blob.Boundary("inner"), blob, fileName);
                    }
                }
            }

            return drs;
        }

        private SurfaceResultCollection SearchSurfaces(IList<SurfaceDefinition> surfaceDefinitions)
        {
            var cs = new SurfaceResultCollection();

            foreach (var def in surfaceDefinitions)
            {
                var surfaceResult = new SurfaceResult()
                                    {
                                        Definition = def.DeepClone(),
                                    };

                var unionIncludeRegion = new HRegion();
                var unionIncludeDomain = new HRegion();
                unionIncludeRegion.GenEmptyRegion();
                unionIncludeDomain.GenEmptyRegion();

                var unionExcludeRegion = new HRegion();
                var unionExcludeDomain = new HRegion();
                unionExcludeRegion.GenEmptyRegion();
                unionExcludeDomain.GenEmptyRegion();

                foreach (var excludeRegion in def.ExcludeRegions)
                {
                    HRegion region;
                    using (new NotifyStopwatch("excludeRegion.Process: " + excludeRegion.Name))
                        region = excludeRegion.Process(_hImage);
                    var domain = excludeRegion.GetRegion();

                    unionExcludeRegion = unionExcludeRegion.Union2(region);
                    unionExcludeDomain = unionExcludeDomain.Union2(domain);

                    if (excludeRegion.SaveCacheImageEnabled)
                    {
                        var fileName = "SurfaceDefinition_" + def.Name + "_Exclude_" + excludeRegion.Name;
                        _hDevelopExportHelper.HImage.SaveCacheImagesForRegion(domain, region, fileName);
                    }

                    surfaceResult.ExcludeRegionResults.Add(new RegionResult()
                                                           {
                                                               SurfaceGroupName = def.GroupName,
                                                               SurfaceName = def.Name,
                                                               RegionName = excludeRegion.Name,
                                                               Domain = domain,
                                                               Region = region,
                                                           });

                    //                    region.Dispose();
                    //                    domain.Dispose();
                }

                foreach (var includeRegion in def.IncludeRegions)
                {
                    var domain = includeRegion.GetRegion();
                    unionIncludeDomain = unionIncludeDomain.Union2(domain);

                    var remainDomain = domain.Difference(unionExcludeRegion);

                    HRegion region;
                    using (new NotifyStopwatch("includeRegion.Process: " + includeRegion.Name))
                        region = includeRegion.Process(_hImage, remainDomain);
                    var remainRegion = region.Difference(unionExcludeRegion);
                    unionIncludeRegion = unionIncludeRegion.Union2(remainRegion);

                    if (includeRegion.SaveCacheImageEnabled)
                    {
                        var fileName = "SurfaceDefinition_" + def.Name + "_Include_" + includeRegion.Name;
                        //                        _hDevelopExportHelper.HImage.SaveCacheImagesForRegion(domain, remainRegion, fileName);
                        _hDevelopExportHelper.HImage.SaveCacheImagesForRegion(domain, remainRegion, unionExcludeRegion,
                            fileName);
                    }

                    surfaceResult.IncludeRegionResults.Add(new RegionResult()
                                                           {
                                                               SurfaceGroupName = def.GroupName,
                                                               SurfaceName = def.Name,
                                                               RegionName = includeRegion.Name,
                                                               Domain = domain,
                                                               Region = remainRegion,
                                                           });
                }

                if (def.SaveCacheImageEnabled && def.IncludeRegions.Any())
                {
                    var fileName = "SurfaceDefinition_" + def.Name + "_Include";
                    _hDevelopExportHelper.HImage.SaveCacheImagesForRegion(unionIncludeDomain, unionIncludeRegion,
                        unionExcludeRegion, fileName);
                }

                if (def.SaveCacheImageEnabled && def.ExcludeRegions.Any())
                {
                    var fileName = "SurfaceDefinition_" + def.Name + "_Exclude";
                    _hDevelopExportHelper.HImage.SaveCacheImagesForRegion(unionExcludeDomain, unionExcludeRegion,
                        fileName);
                }

                surfaceResult.ExcludeRegion = unionExcludeRegion;
                surfaceResult.IncludeRegion = unionIncludeRegion;
                cs.Add(surfaceResult);
            }

            return cs;
        }


        public CircleSearchingResultCollection SearchCircles(
            IList<CircleSearchingDefinition> circleSearchingDefinitions)
        {
            var result = new CircleSearchingResultCollection();

            // ExtractCircle
            int index = 0;
            foreach (var circleDefinition in circleSearchingDefinitions)
            {
                var circleSearchingResult = new CircleSearchingResult
                                            {
                                                Definition = circleDefinition.DeepClone(),
                                                Name = circleDefinition.Name,
                                                Index = index
                                            };
                int offsetX = 0;
                int offsetY = 0;
                HImage enhImage = null;

                if (circleDefinition.ImageFilter_Enabled == false)
                    circleDefinition.ImageFilter = null;
                if (circleDefinition.RegionExtractor_Enabled == false)
                    circleDefinition.RegionExtractor = null;

                var reducedImage = _hDevelopExportHelper.HImage.ReduceDomainForRing(
                    circleDefinition.CenterX,
                    circleDefinition.CenterY,
                    circleDefinition.InnerRadius,
                    circleDefinition.OuterRadius);

                if (circleDefinition.Domain_SaveCacheImageEnabled)
                    reducedImage.CropDomain()
                        .ToBitmapSource()
                        .SaveToJpeg("_EnhanceEdgeArea_" + circleDefinition.Name + "_Domain.jpg");

                HRegion domain;
                if (circleDefinition.RegionExtractor != null)
                {
                    var oldDomain = reducedImage.GetDomain();
                    domain = circleDefinition.RegionExtractor.Process(reducedImage, oldDomain);
                    oldDomain.Dispose();

                    if (circleDefinition.ImageFilter_SaveCacheImageEnabled)
                        reducedImage
                            .ReduceDomain(domain)
                            .CropDomain()
                            .ToBitmapSource()
                            .SaveToJpeg("_RegionExtractor_" + circleDefinition.Name + "_ROIRegion.jpg");
                }
                else
                {
                    domain = reducedImage.GetDomain();
                }

                offsetX = domain.GetColumn1();
                offsetY = domain.GetRow1();

                var croppedImage = reducedImage.CropDomain();

                if (circleDefinition.ImageFilter != null)
                {
                    enhImage = circleDefinition.ImageFilter.Process(croppedImage);
                }
                else
                {
                    enhImage = croppedImage;
                }

                if (circleDefinition.ImageFilter_SaveCacheImageEnabled)
                    enhImage.CropDomain()
                        .ToBitmapSource()
                        .SaveToJpeg("_ImageFilter_" + circleDefinition.Name + "_Mod.jpg");


                if (circleDefinition.ImageFilter_SaveCacheImageEnabled)
                {
                    var savingImage = _hDevelopExportHelper.HImage.Clone();
                    savingImage.OverpaintGray(enhImage);

                    savingImage.ToImageInfo()
                        .ToBitmapSource()
                        .SaveToJpeg("_EnhanceEdgeArea_" + circleDefinition.Name + "_OverpaintGray.jpg");
                }

                var offsetCenterX = circleDefinition.CenterX - offsetX;
                var offsetCenterY = circleDefinition.CenterY - offsetY;

                var circle = circleDefinition.CircleExtractor.FindCircle(enhImage,
                    offsetCenterX, offsetCenterY, circleDefinition.InnerRadius, circleDefinition.OuterRadius);


                if (circle.IsEmpty)
                {
                    circleSearchingResult.HasError = true;
                    circleSearchingResult.IsNotFound = true;
//                    circleSearchingResult.Circle = new Circle(circleDefinition.CenterX, circleDefinition.CenterY);
                }
                else
                {
                    var newCircle = new Circle(circle.CenterX + offsetX, circle.CenterY + offsetY, circle.Radius);
                    circleSearchingResult.Circle = newCircle;
                }

                result.Add(circleSearchingResult);

                index++;
            }

            return result;
        }

        public EdgeSearchingResultCollection SearchEdges(EdgeSearchingDefinition edgeSearchingDefinition)
        {
            return SearchEdges(new[] {edgeSearchingDefinition});
        }

        public EdgeSearchingResultCollection SearchEdges(IList<EdgeSearchingDefinition> edgeSearchingDefinitions)
        {
            var sr = new EdgeSearchingResultCollection();

            HImage mergedImage = null;

            foreach (var esd in edgeSearchingDefinitions)
            {
                var esr = new EdgeSearchingResult();
                esr.Definition = esd.DeepClone();
                esr.Name = esd.Name;
                int offsetX = 0;
                int offsetY = 0;
                HImage enhImage = null;

                //                HImage image;

                if (!esr.Definition.ImageFilter_Enabled)
                    esr.Definition.ImageFilter = null;
                if (esr.Definition.RegionExtractor_Enabled == false)
                    esr.Definition.RegionExtractor = null;



                var reducedImage = HDevelopExport.Singletone.ReduceDomainForRectangle(
                    _hDevelopExportHelper.HImage,
                    line: esd.Line,
                    hv_RoiWidthLen: esd.ROIWidth/2.0,
                    margin: 0.5);

                if (esd.Domain_SaveCacheImageEnabled)
                    reducedImage.CropDomain()
                        .ToBitmapSource()
                        .SaveToJpeg("_EnhanceEdgeArea_" + esd.Name + "_Domain.jpg");

                HRegion domain;
                if (esr.Definition.RegionExtractor != null)
                {
                    var oldDomain = reducedImage.GetDomain();
                    domain = esr.Definition.RegionExtractor.Process(reducedImage, oldDomain);
                    oldDomain.Dispose();

                    if (esd.ImageFilter_SaveCacheImageEnabled)
                        reducedImage
                            .ReduceDomain(domain)
                            .CropDomain()
                            .ToBitmapSource()
                            .SaveToJpeg("_EnhanceEdgeArea_" + esd.Name + "_ROIRegion.jpg");
                }
                else
                {
                    domain = reducedImage.GetDomain();
                }

                offsetX = domain.GetColumn1();
                offsetY = domain.GetRow1();
                var croppedImage = reducedImage.CropDomain();
                //                   croppedImage.WriteImage("tiff", 0, @"_croppedImage.tif");

                if (esr.Definition.ImageFilter != null)
                {
                    var sw = new NotifyStopwatch("ImageFilter");
                    sw.Start();
                    enhImage = esr.Definition.ImageFilter.Process(croppedImage);
                    sw.Stop();
                    sw.Dispose();
                }
                else
                {
                    enhImage = croppedImage;
                }

                //                    enhImage.WriteImage("tiff", 0, @"_enhance.tif");
                //image.OverpaintGray();

                if (esd.ImageFilter_SaveCacheImageEnabled)
                    enhImage.CropDomain()
                        .ToBitmapSource()
                        .SaveToJpeg("_EnhanceEdgeArea_" + esd.Name + "_Mod.jpg");


                using (var sw3 = new NotifyStopwatch("ImageFilter_SaveCacheImageEnabled"))
                {
                    if (esd.ImageFilter_SaveCacheImageEnabled)
                        //                    if (true)
                    {
                        //                        _hDevelopExportHelper.HImage.ToImageInfo().ToBitmapSource().SaveToJpeg("temp_ori.jpg");
                        //                        var fullDomain = enhImage.FullDomain();
                        var savingImage = _hDevelopExportHelper.HImage.Clone();
                        savingImage.OverpaintGray(enhImage);

                        //                            var i = savingImage.PaintRegion(foundRegion, 255.0, "margin");

                        savingImage.ToImageInfo()
                            .ToBitmapSource()
                            .SaveToJpeg("_EnhanceEdgeArea_" + esd.Name + ".jpg");
                        //                            reducedImage.CropDomain().ToImageInfo().ToBitmapSource().SaveToJpeg("_EnhanceEdgeArea_" + esd.Name + "_Ori.jpg");
                        //                            enhImage.CropDomain().ToImageInfo().ToBitmapSource().SaveToJpeg("_EnhanceEdgeArea_" + esd.Name + "_Mod.jpg");
                    }
                }

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

                Line offsetLine = new Line(esd.Line.X1 - offsetX,
                    esd.Line.Y1 - offsetY, esd.Line.X2 - offsetX,
                    esd.Line.Y2 - offsetY);

                var line = esd.LineExtractor.FindLine(enhImage, offsetLine);

                var translatedLine = new Line(line.X1 + offsetX,
                    line.Y1 + offsetY, line.X2 + offsetX,
                    line.Y2 + offsetY);

                esr.EdgeLine = translatedLine;

                if (Math.Abs(line.X1) < 0.0000001 || Math.Abs(line.X2) < 0.0000001 ||
                    Math.Abs(line.Y1) < 0.0000001 || Math.Abs(line.Y2) < 0.0000001)
                {
                    esr.IsNotFound = true;
                    Debug.WriteLine("Edge not found: " + esr.Name);
                }

                sr.Add(esr);
            }

            _hDevelopExportHelper.HImageCache = mergedImage;

            return sr;
        }

        public DefectResultCollection SearchDefects(HImage imageInfo)
        {
            throw new NotImplementedException();
        }

        public DefectResultCollection SearchDefects(HImage imageInfo, HImage mask)
        {
            throw new NotImplementedException();
        }

        public HImage FindRegions(HImage imageInfo)
        {
            var ii = _hDevelopExportHelper.FindS1404SurfaceA();
            return ii;
        }

        public HImage FindRegions()
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
    }
}