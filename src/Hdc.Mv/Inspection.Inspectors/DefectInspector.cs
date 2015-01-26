using System.Collections.Generic;
using HalconDotNet;
using Hdc.Mv.Halcon;

namespace Hdc.Mv.Inspection
{
    class DefectInspector : IDefectInspector
    {
        public IList<DefectResult> SearchDefects(HImage image, IList<DefectDefinition> defectDefinitions, IList<SurfaceResult> surfaceResults)
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

                    var blob = defectDefinition.Extractor.GetDefectRegion(image, regionResult.Region);
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
                        image.SaveCacheImagesForRegion(blob.Boundary("inner"), blob, fileName);
                    }
                }
            }

            return drs;
        }
    }
}