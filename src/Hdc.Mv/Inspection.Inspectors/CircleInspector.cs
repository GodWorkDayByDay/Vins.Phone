using System.Collections.Generic;
using System.IO;
using HalconDotNet;
using Hdc.Mv.Halcon;
using Hdc.Reflection;
using Hdc.Windows.Media.Imaging;

namespace Hdc.Mv.Inspection
{
    public class CircleInspector : ICircleInspector
    {
        private string _cacheImageDir = typeof(Mv.Ex).Assembly.GetAssemblyDirectoryPath() + "\\CacheImages";

//        public CircleInspector()
//        {
//            var dir = typeof (Mv.Ex).Assembly.GetAssemblyDirectoryPath();
//            _cacheImageDir = Path.Combine(dir, "CacheImages");
//
//            if (!Directory.Exists(_cacheImageDir))
//            {
//                Directory.CreateDirectory(_cacheImageDir);
//            }
//        }

        public IList<CircleSearchingResult> SearchCircles(HImage image,
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

                if (circleDefinition.ImageFilter_Disabled)
                    circleDefinition.ImageFilter = null;
                if (circleDefinition.RegionExtractor_Disabled)
                    circleDefinition.RegionExtractor = null;


                var topLeftX = circleDefinition.CenterX - circleDefinition.OuterRadius;
                var topLeftY = circleDefinition.CenterY - circleDefinition.OuterRadius;
                var bottomRightX = circleDefinition.CenterX + circleDefinition.OuterRadius;
                var bottomRightY = circleDefinition.CenterY + circleDefinition.OuterRadius;

                var reg = new HRegion();
                reg.GenRectangle1(topLeftY, topLeftX, bottomRightY, bottomRightX);
                var reducedImage = image.ReduceDomain(reg);
                reg.Dispose();

                //                var reducedImage = _hDevelopExportHelper.HImage.ReduceDomainForRing(
                //                    circleDefinition.CenterX,
                //                    circleDefinition.CenterY,
                //                    circleDefinition.InnerRadius,
                //                    circleDefinition.OuterRadius);

                if (circleDefinition.Domain_SaveCacheImageEnabled)
                    reducedImage.CropDomain()
                        .ToBitmapSource()
                        .SaveToJpeg(_cacheImageDir + "\\SearchCircles_" + circleDefinition.Name + "_1_Domain.jpg");

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
                            .SaveToJpeg(_cacheImageDir + "\\SearchCircles_" + circleDefinition.Name + "_2_ROI.jpg");
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

                    if (circleDefinition.ImageFilter_SaveCacheImageEnabled)
                    {
                        var cropDomain = enhImage.CropDomain();
                        cropDomain
                            .ToBitmapSource()
                            .SaveToJpeg(_cacheImageDir + "\\SearchCircles_" + circleDefinition.Name +
                                        "_3_ImageFilter.jpg");
                        cropDomain.Dispose();

                        //
                        var paintedImage = enhImage.PaintGrayOffset(image, offsetY, offsetX);
                        paintedImage
                            .ToBitmapSource()
                            .SaveToJpeg(_cacheImageDir + "\\SearchCircles_" + circleDefinition.Name +
                                        "_3_ImageFilter_PaintGrayOffset.jpg");
                        paintedImage.Dispose();
                    }
                }
                else
                {
                    enhImage = croppedImage;
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
    }
}