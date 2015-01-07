using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    public interface IGeneralInspector : IDisposable
    {
        void Init();

//        void SetImageInfo(ImageInfo imageInfo);

        void SetImageInfo(HImage imageInfo);

        InspectionResult Inspect(InspectionSchema inspectionSchema);

        InspectionResult Inspect(HImage image, InspectionSchema inspectionSchema);

//        InspectionResult Inspect(ImageInfo imageInfo, InspectionSchema inspectionSchema);

        CircleSearchingResultCollection SearchCircles(IList<CircleSearchingDefinition> circleSearchingDefinitions);

        CircleSearchingResultCollection SearchCircles(HImage imageInfo, IList<CircleSearchingDefinition> circleSearchingDefinitions);

        EdgeSearchingResultCollection SearchEdges(HImage imageInfo, IList<EdgeSearchingDefinition> edgeSearchingDefinitions);

        DefectResultCollection SearchDefects(HImage imageInfo);

        DefectResultCollection SearchDefects(HImage imageInfo, HImage mask);

        HImage FindRegions(HImage imageInfo);

        HImage FindRegions();
    }
}