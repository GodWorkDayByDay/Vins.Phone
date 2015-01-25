using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using HalconDotNet;
using Hdc.Diagnostics;
using Hdc.Mv.Halcon;
using Hdc.Reflection;
using Hdc.Windows.Media.Imaging;

namespace Hdc.Mv.Inspection
{
    class EdgeInspector : IEdgeInspector
    {
        private string _cacheImageDir = typeof(Mv.Ex).Assembly.GetAssemblyDirectoryPath() + "\\CacheImages";

//        public EdgeInspector()
//        {
//            var dir = typeof(Mv.Ex).Assembly.GetAssemblyDirectoryPath();
//            _cacheImageDir = Path.Combine(dir, "CacheImages");
//
//            if (!Directory.Exists(_cacheImageDir))
//            {
//                Directory.CreateDirectory(_cacheImageDir);
//            }
//        }

        public IList<EdgeSearchingResult> SearchEdges(HImage image, IList<EdgeSearchingDefinition> edgeSearchingDefinitions)
        {
            var sr = new EdgeSearchingResultCollection();

            foreach (var esd in edgeSearchingDefinitions)
            {
                var esr = new EdgeSearchingResult();
                esr.Definition = esd.DeepClone();
                esr.Name = esd.Name;
                int offsetX = 0;
                int offsetY = 0;
                HImage enhImage = null;

                if (esr.Definition.ImageFilter_Disabled)
                    esr.Definition.ImageFilter = null;
                if (esr.Definition.RegionExtractor_Disabled)
                    esr.Definition.RegionExtractor = null;


                var reducedImage = HDevelopExport.Singletone.ReduceDomainForRectangle(
                    image,
                    line: esd.Line,
                    hv_RoiWidthLen: esd.ROIWidth / 2.0,
                    margin: 0.5);

                if (esd.Domain_SaveCacheImageEnabled)
                    reducedImage.CropDomain()
                        .ToBitmapSource()
                        .SaveToJpeg(_cacheImageDir + "\\SearchEdges_" + esd.Name + "_1_Domain.jpg");

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
                            .SaveToJpeg(_cacheImageDir + "\\SearchEdges_" + esd.Name + "_2_ROI.jpg");
                }
                else
                {
                    domain = reducedImage.GetDomain();
                }

                offsetX = domain.GetColumn1();
                offsetY = domain.GetRow1();
                var croppedImage = reducedImage.CropDomain();

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

                if (esd.ImageFilter_SaveCacheImageEnabled)
                {
                    enhImage.CropDomain()
                        .ToBitmapSource()
                        .SaveToJpeg(_cacheImageDir + "\\SearchEdges_" + esd.Name + "_3_ImageFilter.jpg");

                    var paintedImage = enhImage.PaintGrayOffset(image, offsetY, offsetX);
                    paintedImage
                        .ToBitmapSource()
                        .SaveToJpeg(_cacheImageDir + "\\SearchEdges_" + esd.Name + "_3_ImageFilter_PaintGrayOffset.jpg");
                    paintedImage.Dispose();
                }


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

            return sr;
        }
    }
}