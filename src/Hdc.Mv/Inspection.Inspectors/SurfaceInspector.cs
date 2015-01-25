using System.Collections.Generic;
using System.Linq;
using HalconDotNet;
using Hdc.Diagnostics;

namespace Hdc.Mv.Inspection
{
    class SurfaceInspector : ISurfaceInspector
    {
        public IList<SurfaceResult> SearchSurfaces(HImage image, IList<SurfaceDefinition> surfaceDefinitions)
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
                        region = excludeRegion.Process(image);
                    var domain = excludeRegion.GetRegion();

                    unionExcludeRegion = unionExcludeRegion.Union2(region);
                    unionExcludeDomain = unionExcludeDomain.Union2(domain);

                    if (excludeRegion.SaveCacheImageEnabled)
                    {
                        var fileName = "SurfaceDefinition_" + def.Name + "_Exclude_" + excludeRegion.Name;
                        image.SaveCacheImagesForRegion(domain, region, fileName);
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
                        region = includeRegion.Process(image, remainDomain);
                    var remainRegion = region.Difference(unionExcludeRegion);
                    unionIncludeRegion = unionIncludeRegion.Union2(remainRegion);

                    if (includeRegion.SaveCacheImageEnabled)
                    {
                        var fileName = "SurfaceDefinition_" + def.Name + "_Include_" + includeRegion.Name;
                        //                        _hDevelopExportHelper.HImage.SaveCacheImagesForRegion(domain, remainRegion, fileName);
                        image.SaveCacheImagesForRegion(domain, remainRegion, unionExcludeRegion,
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
                    image.SaveCacheImagesForRegion(unionIncludeDomain, unionIncludeRegion,
                        unionExcludeRegion, fileName);
                }

                if (def.SaveCacheImageEnabled && def.ExcludeRegions.Any())
                {
                    var fileName = "SurfaceDefinition_" + def.Name + "_Exclude";
                    image.SaveCacheImagesForRegion(unionExcludeDomain, unionExcludeRegion,
                        fileName);
                }

                surfaceResult.ExcludeRegion = unionExcludeRegion;
                surfaceResult.IncludeRegion = unionIncludeRegion;
                cs.Add(surfaceResult);
            }

            return cs;
        }
    }
}