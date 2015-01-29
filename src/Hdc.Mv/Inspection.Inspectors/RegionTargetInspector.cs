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

            HRegion aroundProcessedRegion;
            if (definition.AroundRegionProcessor != null)
            {
                aroundProcessedRegion = definition.AroundRegionProcessor.Process(aroundRegion);
            }
            else
            {
                aroundProcessedRegion = aroundRegion;
            }

            HImage aroundImage = roiImage.ChangeDomain(aroundProcessedRegion);
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