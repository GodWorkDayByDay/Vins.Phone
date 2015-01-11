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
                var circles = SearchCircles(inspectionSchema.CircleSearchingDefinitions);

                inspectionResult.Circles = circles;
            }


            if (inspectionSchema.SurfaceDefinitions.Any())
            {
                var regionResults = SearchClosedRegions(inspectionSchema.SurfaceDefinitions);

//                inspectionResult.ClosedRegionResults = regionResults;
            }

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

        private SurfaceResultCollection SearchClosedRegions(IList<SurfaceDefinition> surfaceDefinitions)
        {
            var cs = new SurfaceResultCollection();

            foreach (var def in surfaceDefinitions)
            {
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
                    var region = excludeRegion.Process(_hImage);
                    var domain = excludeRegion.GetRegion();

                    unionExcludeRegion = unionExcludeRegion.Union2(region);
                    unionExcludeDomain = unionExcludeDomain.Union2(domain);

                    if (excludeRegion.SaveCacheImageEnabled)
                    {
                        var fileName = "SurfaceDefinition_" + def.Name + "_Exclude_" + excludeRegion.Name;
                        _hDevelopExportHelper.HImage.SaveCacheImagesForRegion(domain, region, fileName);
                    }

                    region.Dispose();
                    domain.Dispose();
                }

                foreach (var includeRegion in def.IncludeRegions)
                {
                    var domain = includeRegion.GetRegion();
                    unionIncludeDomain = unionIncludeDomain.Union2(domain);

                    var remainDomain = domain.Difference(unionExcludeRegion);
                    var region = includeRegion.Process(_hImage, remainDomain);
                    var remainRegion = region.Difference(unionExcludeRegion);
                    unionIncludeRegion = unionIncludeRegion.Union2(remainRegion);

                    if (includeRegion.SaveCacheImageEnabled)
                    {
                        var fileName = "SurfaceDefinition_" + def.Name + "_Include_" + includeRegion.Name;
//                        _hDevelopExportHelper.HImage.SaveCacheImagesForRegion(domain, remainRegion, fileName);
                        _hDevelopExportHelper.HImage.SaveCacheImagesForRegion(domain, remainRegion, unionExcludeRegion, fileName);
                    }
                }

                if (def.SaveCacheImageEnabled && def.IncludeRegions.Any())
                {
                    var fileName = "SurfaceDefinition_" + def.Name + "_Include";
                    _hDevelopExportHelper.HImage.SaveCacheImagesForRegion(unionIncludeDomain, unionIncludeRegion, unionExcludeRegion, fileName);
                }

                if (def.SaveCacheImageEnabled && def.ExcludeRegions.Any())
                {
                    var fileName = "SurfaceDefinition_" + def.Name + "_Exclude";
                    _hDevelopExportHelper.HImage.SaveCacheImagesForRegion(unionExcludeDomain, unionExcludeRegion, fileName);
                }

                cs.Add(new SurfaceResult()
                       {
                           Definition = def.DeepClone(),
                           IncludeRegion = unionIncludeRegion,
                           ExcludeRegion = unionExcludeRegion,
                       });
            }

            return cs;
        }

        public void Init(int width, int height)
        {
        }

        public InspectionResult Inspect(HImage imageInfo, InspectionSchema inspectionSchema)
        {
            SetImageInfo(imageInfo);
            return Inspect(inspectionSchema);
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

                Circle foundCircle;
                double roundless;

                if (circleDefinition.ProcessMethod != null)
                {
                    var hcd = circleDefinition.ProcessMethod as HoughCircleDetectProcessMethod;
                    if (hcd == null) continue;

                    double cX, cY;

                    var isOK2 = _hDevelopExportHelper.FindCircleCenterUseHough(
                        _hDevelopExportHelper.HImage,
                        circleDefinition.CenterY,
                        circleDefinition.CenterX,
                        circleDefinition.InnerRadius,
                        circleDefinition.OuterRadius,
                        hcd.Sigma,
                        hcd.MinGray,
                        hcd.MaxGray,
                        hcd.ExpectRadius,
                        hcd.Percent,
                        out cY,
                        out cX
                        );

                    if (isOK2)
                    {
                        circleSearchingResult.Circle = new Circle(
                            cX,
                            cY,
                            hcd.ExpectRadius);
                    }
                    else
                    {
                        circleSearchingResult.HasError = true;
                        circleSearchingResult.IsNotFound = true;
                    }

                    result.Add(circleSearchingResult);

                    index++;


                    continue;
                }


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

        public CircleSearchingResultCollection SearchCircles(HImage imageInfo,
                                                             IList<CircleSearchingDefinition> circleSearchingDefinitions)
        {
            _hDevelopExportHelper = new HDevelopExportHelper(imageInfo);

            return SearchCircles(circleSearchingDefinitions);
        }

        public EdgeSearchingResultCollection SearchEdges(HImage imageInfo,
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

            foreach (var esd in edgeSearchingDefinitions)
            {
                var esr = new EdgeSearchingResult();
                esr.Definition = esd.DeepClone();
                esr.Name = esd.Name;

                HImage image;

                if (esr.Definition.ImageFilter_Enabled && enhanceEdgeAreaEnabled)
                {
                    var sw = new NotifyStopwatch("ImageFilter");
                    sw.Start();

                    var reducedImage = _hDevelopExportHelper.ReduceDomainForRectangle(
                        _hDevelopExportHelper.HImage,
                        line: esd.Line,
                        hv_RoiWidthLen: esd.ROIWidth/2.0,
                        margin: 0.5);

                    if (esd.ImageFilter_SaveCacheImageEnabled)
                        reducedImage.CropDomain()
                            .ToImageInfo()
                            .ToBitmapSource()
                            .SaveToJpeg("_EnhanceEdgeArea_" + esd.Name + "_Ori.jpg");

                    var enhImage = esr.Definition.ImageFilter.Process(reducedImage);

                    if (esd.ImageFilter_SaveCacheImageEnabled)
                        enhImage.CropDomain()
                            .ToImageInfo()
                            .ToBitmapSource()
                            .SaveToJpeg("_EnhanceEdgeArea_" + esd.Name + "_Mod.jpg");

                    //                    var enhImage = _hDevelopExportHelper.EnhanceEdgeArea3(
                    //                        reducedImage,
                    //                        hv_EmpMaskWidth: esd.Hal_EnhanceEdgeArea_EmpMaskWidth,
                    //                        hv_EmpMaskHeight: esd.Hal_EnhanceEdgeArea_EmpMaskHeight,
                    //                        hv_EmpMaskFactor: esd.Hal_EnhanceEdgeArea_EmpMaskFactor,
                    //                        hv_MeanMaskWidth: esd.Hal_EnhanceEdgeArea_MeanMaskWidth,
                    //                        hv_MeanMaskHeight: esd.Hal_EnhanceEdgeArea_MeanMaskHeight,
                    //                        hv_IterationCount: esd.Hal_EnhanceEdgeArea_IterationCount,
                    //                        hv_ScaleAdd: esd.Hal_EnhanceEdgeArea_ScaleAdd,
                    //                        hv_ScaleMult: esd.Hal_EnhanceEdgeArea_ScaleMult
                    //                        );

                    image = enhImage;

                    sw.Stop();
                    sw.Dispose();

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
                }
                else
                {
                    image = _hDevelopExportHelper.HImage;
                }
                //                var sw2 = new NotifyStopwatch("RakeEdgeLine");
                //                sw2.Start();

                var lines = _hDevelopExportHelper.RakeEdgeLine(image,
                    line: esd.Line,
                    regionsCount: esd.Hal_RegionsCount,
                    regionHeight: esd.Hal_RegionHeight,
                    regionWidth: esd.Hal_RegionWidth,
                    sigma: esd.Hal_Sigma,
                    threshold: esd.Hal_Threshold,
                    transition: esd.Hal_Transition,
                    selectionMode: esd.Hal_SelectionMode);
                //
                //                sw2.Stop();
                //                sw2.Dispose();

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

        //        public InspectionResult SearchEdges(ImageInfo imageInfo, InspectionSchema inspectionSchema)
        //        {
        //        }
    }
}