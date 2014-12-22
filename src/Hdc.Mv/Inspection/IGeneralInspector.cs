using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Hdc.Mv.Inspection
{
    public interface IGeneralInspector : IDisposable
    {
        void Init();

        InspectionResult Inspect(ImageInfo imageInfo, InspectionSchema inspectionSchema);

        CircleSearchingResultCollection SearchCircles(ImageInfo imageInfo, IList<CircleSearchingDefinition> circleSearchingDefinitions);

        EdgeSearchingResultCollection SearchEdges(ImageInfo imageInfo, IList<EdgeSearchingDefinition> edgeSearchingDefinitions);

        DefectResultCollection SearchDefects(ImageInfo imageInfo);

        DefectResultCollection SearchDefects(ImageInfo imageInfo, ImageInfo mask);

        ImageInfo FindRegions(ImageInfo imageInfo);

        ImageInfo FindRegions();
    }
}