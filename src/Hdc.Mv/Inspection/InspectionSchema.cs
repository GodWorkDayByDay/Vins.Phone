using System;
using System.Collections.ObjectModel;
using System.Windows;

// ReSharper disable InconsistentNaming
namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class InspectionSchema
    {
        public InspectionSchema()
        {
            CoordinateCircles = new Collection<CircleSearchingDefinition>();
            CircleSearchingDefinitions = new Collection<CircleSearchingDefinition>();
            EdgeSearchingDefinitions = new Collection<EdgeSearchingDefinition>();
            DistanceBetweenLinesDefinitions = new Collection<DistanceBetweenLinesDefinition>();
            DistanceBetweenIntersectionPointsDefinitions = new Collection<DistanceBetweenLinesDefinition>();
        }

//        public string InspectorName { get; set; }
        public string InspectorNameForCoordinate { get; set; }
        public string InspectorNameForCircles { get; set; }
        public string InspectorNameForEdges { get; set; }
        public string InspectorNameForDefects { get; set; }

        public int ImagePixelWidth { get; set; }
        public int ImagePixelHeight { get; set; }
        public string TestImageFilePath { get; set; }
        public string TestImagesDirectory { get; set; }

        public Int32Rect CropRect { get; set; }
        public bool CropRectEnable { get; set; }

        public bool EdgeSearchingEnable { get; set; }
        public bool EdgeSearching_EnhanceEdgeArea_Enable { get; set; }
        public bool EdgeSearching_EnhanceEdgeArea_SaveCacheImageEnable { get; set; }
        public bool EdgeSearching_EnhanceEdgeArea_SaveAllCacheImageEnable { get; set; }
        public bool CircleSearchingEnable { get; set; }

        public Collection<CircleSearchingDefinition> CoordinateCircles { get; set; }
        public Collection<EdgeSearchingDefinition> EdgeSearchingDefinitions { get; set; }
        public Collection<CircleSearchingDefinition> CircleSearchingDefinitions { get; set; }
        public Collection<DistanceBetweenLinesDefinition> DistanceBetweenLinesDefinitions { get; set; }
        public Collection<DistanceBetweenLinesDefinition> DistanceBetweenIntersectionPointsDefinitions { get; set; }

        public CoordinateType CoordinateType { get; set; }

        public bool CoordinateOriginOffsetEnable { get; set; }
        public double CoordinateOriginOffsetX { get; set; }
        public double CoordinateOriginOffsetY { get; set; }

        
    }
}
// ReSharper restore InconsistentNaming