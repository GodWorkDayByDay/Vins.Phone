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
    internal class EdgeInspector : IEdgeInspector
    {
        private string _cacheImageDir = typeof (Mv.Ex).Assembly.GetAssemblyDirectoryPath() + "\\CacheImages";

        public EdgeSearchingResult SearchEdge(HImage image, EdgeSearchingDefinition definition)
        {
            var esr = new EdgeSearchingResult
                      {
                          Definition = definition.DeepClone(),
                          Name = definition.Name
                      };

            if (esr.Definition.ImageFilter_Disabled)
                esr.Definition.ImageFilter = null;

            if (esr.Definition.RegionExtractor_Disabled)
                esr.Definition.RegionExtractor = null;

            var rectImage = HDevelopExport.Singletone.ChangeDomainForRectangle(
                image,
                definition.Line,
                definition.ROIWidth/2.0);

            if (definition.Domain_SaveCacheImageEnabled)
            {
                rectImage.WriteImageOfTiffLzwOfCropDomain(
                    _cacheImageDir + "\\SearchEdges_" + definition.Name + "_1_Domain_Cropped.tif");
            }

            // RegionExtractor
            HImage roiImage = null;

            if (esr.Definition.RegionExtractor != null)
            {
                var rectDomain = rectImage.GetDomain();
                HRegion roiDomain;
                if (!esr.Definition.RegionExtractor_CropDomainEnabled)
                {
                    roiDomain = esr.Definition.RegionExtractor.Extract(rectImage);

                    if (definition.RegionExtractor_SaveCacheImageEnabled)
                    {
                        rectImage.WriteImageOfTiffLzwOfCropDomain(roiDomain,
                            _cacheImageDir + "\\SearchEdges_" + definition.Name + "_2_ROI.tif");
                    }
                    roiImage = rectImage.ReduceDomain(roiDomain);
                    rectImage.Dispose();
                }
                else
                {
                    throw new NotImplementedException();
                    var domainOffsetRow1 = rectDomain.GetRow1();
                    var domainOffsetColumn1 = rectDomain.GetColumn1();

                    var croppedRectImage = rectImage.CropDomain();
                    var croppedRoiDomain = esr.Definition.RegionExtractor.Extract(croppedRectImage);
                    roiDomain = croppedRoiDomain.MoveRegion(domainOffsetRow1, domainOffsetColumn1);

                    throw new NotImplementedException();
                }
            }
            else
            {
                roiImage = rectImage;
            }

            // ImageFilter
            HImage filterImage = null;

            if (esr.Definition.ImageFilter != null)
            {
                if (!esr.Definition.ImageFilter_CropDomainEnabled)
                {
                    var sw = new NotifyStopwatch("ImageFilter");
                    filterImage = esr.Definition.ImageFilter.Process(roiImage);
                    sw.Dispose();

                    roiImage.Dispose();

                    if (definition.ImageFilter_SaveCacheImageEnabled)
                    {
                        var cropDomain = filterImage.CropDomain();
                        cropDomain.WriteImageOfTiffLzw(_cacheImageDir + "\\SearchEdges_" + definition.Name +
                                                       "_3_ImageFilter_Cropped.tif");
                        cropDomain.Dispose();

                        var paintedImage = cropDomain.PaintGrayOffset(image, 0, 0);
                        paintedImage.WriteImageOfJpeg(_cacheImageDir + "\\SearchEdges_" + definition.Name +
                                                      "_3_ImageFilter_PaintGrayOffset.jpg");
                        paintedImage.Dispose();
                    }
                }
                else
                {
                    throw new NotImplementedException();
                    var roiDomain = roiImage.GetDomain();
                    int offsetX = 0;
                    int offsetY = 0;
                    offsetX = roiDomain.GetColumn1();
                    offsetY = roiDomain.GetRow1();
                    var croppedImage = roiImage.CropDomain();

                    var sw = new NotifyStopwatch("ImageFilter");
                    filterImage = esr.Definition.ImageFilter.Process(croppedImage);
                    sw.Dispose();

                    if (definition.ImageFilter_SaveCacheImageEnabled)
                    {
                        var cropDomain = filterImage.CropDomain();
                        cropDomain.WriteImageOfTiffLzw(_cacheImageDir + "\\SearchEdges_" + definition.Name +
                                                       "_3_ImageFilter_Cropped.tif");
                        cropDomain.Dispose();

                        var paintedImage = filterImage.PaintGrayOffset(image, offsetY, offsetX);
                        paintedImage.WriteImageOfJpeg(_cacheImageDir + "\\SearchEdges_" + definition.Name +
                                                      "_3_ImageFilter_PaintGrayOffset.jpg");
                        paintedImage.Dispose();
                    }


                    /*                        Line offsetLine = new Line(esd.Line.X1 - offsetX,
                                                esd.Line.Y1 - offsetY, esd.Line.X2 - offsetX,
                                                esd.Line.Y2 - offsetY);

                                            var line = esd.LineExtractor.FindLine(filterImage, offsetLine);

                                            var translatedLine = new Line(line.X1 + offsetX,
                                                line.Y1 + offsetY, line.X2 + offsetX,
                                                line.Y2 + offsetY);

                                            esr.EdgeLine = translatedLine;

                                            if (line.IsEmpty)
                                            {
                                                esr.IsNotFound = true;
                                                Debug.WriteLine("Edge not found: " + esr.Name);
                                            }*/

                    throw new NotImplementedException();
                }
            }
            else
            {
                filterImage = roiImage;
            }

            var line = definition.LineExtractor.FindLine(filterImage, definition.Line);

            if (line.IsEmpty)
            {
                esr.IsNotFound = true;
                Debug.WriteLine("Edge not found: " + esr.Name);
            }

            esr.EdgeLine = line;
            return esr;
        }
    }
}