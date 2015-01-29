using HalconDotNet;
using Hdc.Mv.Halcon;
using Hdc.Reflection;

namespace Hdc.Mv.Inspection
{
    public class RegionTargetInspector : IRegionTargetInspector
    {
        private string _cacheImageDir = typeof(Mv.Ex).Assembly.GetAssemblyDirectoryPath() + "\\CacheImages";

        public RegionTargetResult SearchRegionTarget(HImage image, RegionTargetDefinition definition)
        {
            var result = new RegionTargetResult
                         {
                             Definition = definition.DeepClone(),
                         };

            var roiImage = HDevelopExport.Singletone.ChangeDomainForRectangle(
                image,
                definition.RoiActualLine,
                definition.RoiHalfWidth);

            if (definition.Domain_SaveCacheImageEnabled)
            {
                roiImage.WriteImageOfTiffLzwOfCropDomain(
                    _cacheImageDir + "\\SearchRegionTarget_" + definition.Name + "_1_Domain_Cropped.tif");
            }

            // Around
            HImage aroundFilterImage;
            if (definition.AroundImageFilter != null)
            {
                aroundFilterImage = definition.AroundImageFilter.Process(roiImage);
            }
            else
            {
                aroundFilterImage = roiImage;
            }

            HRegion aroundRegion;
            if (definition.AroundRegionExtractor != null)
            {
                aroundRegion = definition.AroundRegionExtractor.Extract(aroundFilterImage);
            }
            else
            {
                aroundRegion = aroundFilterImage.GetDomain();
            } 
            
            if (definition.AroundRegionExtractor_SaveCacheImageEnabled)
            {
//                var reducedImage = image.ChangeDomain(aroundRegion.Union1());
//                reducedImage.WriteImageOfTiffLzwOfCropDomain(
//                    _cacheImageDir + "\\SearchRegionTarget_" + definition.Name + "_1_AroundRegionExtractor.tif");
//                reducedImage.Dispose();
                image.WriteImageOfTiffLzwOfCropDomain(aroundRegion,
                    _cacheImageDir + "\\SearchRegionTarget_" + definition.Name + "_1_AroundRegionExtractor.tif");
            }

            HRegion aroundProcessedRegion;
            if (definition.AroundRegionProcessor != null)
            {
                aroundProcessedRegion = definition.AroundRegionProcessor.Process(aroundRegion);
            }
            else
            {
                aroundProcessedRegion = aroundRegion;
            }

            HImage aroundImage = image.ChangeDomain(aroundProcessedRegion);

            if (definition.AroundRegionProcessor_SaveCacheImageEnabled)
            {
                aroundImage.WriteImageOfTiffLzwOfCropDomain(
                    _cacheImageDir + "\\SearchRegionTarget_" + definition.Name + "_1_Around_Cropped.tif");
            }

            aroundProcessedRegion.Dispose();
            aroundFilterImage.Dispose();
            aroundRegion.Dispose();

            // Target

            HImage targetFilterImage;
            if (definition.TargetImageFilter != null)
            {
                targetFilterImage = definition.TargetImageFilter.Process(aroundImage);
            }
            else
            {
                targetFilterImage = aroundImage;
            }

            if (definition.TargetImageFilter_SaveCacheImageEnabled)
            {
                targetFilterImage.WriteImageOfTiffLzwOfCropDomain(
                    _cacheImageDir + "\\SearchRegionTarget_" + definition.Name + "_1_TargetImageFilter_Cropped.tif");
            }

            HRegion targetRegion;
            if (definition.TargetRegionExtractor != null)
            {
                targetRegion = definition.TargetRegionExtractor.Extract(targetFilterImage);
            }
            else
            {
                targetRegion = targetFilterImage.GetDomain();
            }

            HRegion targetProcessedRegion;
            if (definition.TargetRegionProcessor != null)
            {
                targetProcessedRegion = definition.TargetRegionProcessor.Process(targetRegion);
            }
            else
            {
                targetProcessedRegion = targetRegion;
            }

            targetFilterImage.Dispose();
            roiImage.Dispose();

            result.TargetRegion = targetProcessedRegion;

            if (targetRegion.GetArea() < 1)
                result.HasError = true;

            return result;
        }
    }
}