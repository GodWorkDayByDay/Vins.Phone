﻿using System;
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
            SurfaceDefinitions = new Collection<SurfaceDefinition>();
            DefectDefinitions = new Collection<DefectDefinition>();
        }

        public int ImagePixelWidth { get; set; }
        public int ImagePixelHeight { get; set; }
        public string TestImageFilePath { get; set; }
        public string TestImagesDirectory { get; set; }

        public string EdgeSearching_InspectorName { get; set; }
        public bool EdgeSearching_Disabled { get; set; }
        public bool EdgeSearching_SaveCacheImage_Disabled { get; set; }

        public string CircleSearching_InspectorName { get; set; }
        public bool CircleSearching_Disabled { get; set; }
        public bool CircleSearching_SaveCacheImage_Disabled { get; set; }

        public string Defects_InspectorName { get; set; }
        public bool Defects_Disabled { get; set; }
        public bool Defects_SaveCacheImage_Disabled { get; set; }

        public Collection<CircleSearchingDefinition> CoordinateCircles { get; set; }
        public Collection<EdgeSearchingDefinition> CoordinateEdges { get; set; }
        public Collection<EdgeSearchingDefinition> EdgeSearchingDefinitions { get; set; }
        public Collection<CircleSearchingDefinition> CircleSearchingDefinitions { get; set; }
        public Collection<DistanceBetweenLinesDefinition> DistanceBetweenLinesDefinitions { get; set; }
        public Collection<DistanceBetweenLinesDefinition> DistanceBetweenIntersectionPointsDefinitions { get; set; }
        public Collection<SurfaceDefinition> SurfaceDefinitions { get; set; }
        public Collection<DefectDefinition> DefectDefinitions { get; set; }

        public CoordinateType CoordinateType { get; set; }

        public bool CoordinateOriginOffsetEnable { get; set; }
        public double CoordinateOriginOffsetX { get; set; }
        public double CoordinateOriginOffsetY { get; set; }

        public bool Disabled { get; set; }
        public string Comment { get; set; }
    }
}
// ReSharper restore InconsistentNaming