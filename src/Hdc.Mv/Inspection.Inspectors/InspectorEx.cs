using System.Collections.Generic;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    public static class InspectorEx
    {
        public static IList<CircleSearchingResult> SearchCircles(this ICircleInspector inspector, HImage image,
                                                                 IList<CircleSearchingDefinition>
                                                                     definitions)
        {
            var csrs = new CircleSearchingResultCollection();

            int index = 0;
            foreach (var definition in definitions)
            {
                var csr = inspector.SearchCircle(image, definition);
                csr.Index = index;
                csrs.Add(csr);
                index++;
            }
            return csrs;
        }

        public static IList<EdgeSearchingResult> SearchEdges(this IEdgeInspector inspector, HImage image,
                                                             IList<EdgeSearchingDefinition> definitions)
        {
            var csrs = new EdgeSearchingResultCollection();

            int index = 0;
            foreach (var definition in definitions)
            {
                var csr = inspector.SearchEdge(image, definition);
                csr.Index = index;
                csrs.Add(csr);
                index++;
            }
            return csrs;
        }


        public static IList<SurfaceResult> SearchSurfaces(this ISurfaceInspector inspector,
                                                          HImage image, IList<SurfaceDefinition> definitions)
        {
            var csrs = new SurfaceResultCollection();

            int index = 0;
            foreach (var definition in definitions)
            {
                var csr = inspector.SearchSurface(image, definition);
                csr.Index = index;
                csrs.Add(csr);
                index++;
            }
            return csrs;
        }

        public static IList<RegionTargetResult> SearchRegionTargets(this IRegionTargetInspector inspector,
                                                          HImage image, IList<RegionTargetDefinition> definitions)
        {
            var csrs = new List<RegionTargetResult>();

            int index = 0;
            foreach (var definition in definitions)
            {
                var csr = inspector.SearchRegionTarget(image, definition);
                csr.Index = index;
                csrs.Add(csr);
                index++;
            }
            return csrs;
        }
    }
}