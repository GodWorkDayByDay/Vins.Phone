/*using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    public class SimGeneralInspector : IGeneralInspector
    {
        private HImage _image;

        public void Dispose()
        {
        }

        public void Init()
        {
        }

        public void SetImageInfo(HImage image)
        {
            _image = image;
        }

        public InspectionResult Inspect(InspectionSchema inspectionSchema)
        {
            var ir = new InspectionResult();
            var circles = SearchCircles(inspectionSchema.CircleSearchingDefinitions);
            var edges = SearchEdges(inspectionSchema.EdgeSearchingDefinitions);
            ir.Circles = new CircleSearchingResultCollection(circles);
            ir.Edges = new EdgeSearchingResultCollection(edges);
            return ir;
        }

        public IList<CircleSearchingResult> SearchCircles(IList<CircleSearchingDefinition> circleSearchingDefinitions)
        {
            var results = new SimCircleInspector().SearchCircles(_image, circleSearchingDefinitions);
            return results;
        }

        public IList<EdgeSearchingResult> SearchEdges(IList<EdgeSearchingDefinition> edgeSearchingDefinitions)
        {
            return new SimEdgeInspector().SearchEdges(_image, edgeSearchingDefinitions);
        }
//
//        public DefectResultCollection SearchDefects(HImage imageInfo)
//        {
//            var drc = new DefectResultCollection();
//            for (int i = 0; i < 10; i++)
//            {
//                var dr = new DefectResult()
//                         {
//                             X = 200 * i,
//                             Y = 200 * i,
//                             Width = 150 + i * 100,
//                             Height = 300 + i * 50,
//                         };
//                dr.Size = dr.Width * dr.Height;
//                drc.Add(dr);
//            }
//
//            return drc;
//        }
//
//        public DefectResultCollection SearchDefects(HImage imageInfo, HImage mask)
//        {
//            return new DefectResultCollection();
//        }
    }
}*/