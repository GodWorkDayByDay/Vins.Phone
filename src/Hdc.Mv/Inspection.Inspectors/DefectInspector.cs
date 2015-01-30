using System.Collections.Generic;
using HalconDotNet;
using Hdc.Mv.Halcon;

namespace Hdc.Mv.Inspection
{
    public  class DefectInspector : IDefectInspector
    {
        public IList<RegionDefectResult> SearchDefects(HImage image, DefectDefinition defectDefinition,
                                                 IList<SurfaceResult> surfaceResults)
        {
            var rdrs = new List<RegionDefectResult>();

            foreach (var refer in defectDefinition.RegionReferences)
            {
                var regionResult = surfaceResults.GetRegionResult(refer.SurfaceName, refer.RegionName);

                var regionDefectResult = new RegionDefectResult {RegionResult = regionResult};

                var blob = defectDefinition.Extractor.GetDefectRegion(image, regionResult.Region);
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
                    regionDefectResult.DefectResults.Add(dr);
                    index++;
                }

                if (defectDefinition.SaveCacheImageEnabled)
                {
//                    continue;
//                    var fileName = "SurfaceDefinition_Defects_" + defectDefinition.Name;
//                    image.SaveCacheImagesForRegion(blob.Boundary("inner"), blob, fileName);
                }

                rdrs.Add(regionDefectResult);
            }

            return rdrs;
        }
    }
}