using System;
using System.Windows;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class EdgeSearchingDefinition
    {
        public string Name { get; set; }

        // General
        public double StartX { get; set; }
        public double StartY { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
        public double ROIWidth { get; set; }

        // MIL
        public int Polarity { get; set; }
        public int Orientation { get; set; }


        // Reserved
        public int LowThreshold { get; set; }
        public int HighThreshold { get; set; }
        public double FilterAlpha { get; set; }

        // 
        public int Hal_RegionsCount { get; set; }
        public int Hal_RegionHeight { get; set; }
        public int Hal_RegionWidth { get; set; }
        public double Hal_Sigma { get; set; }
        public double Hal_Threshold { get; set; }
        public SelectionMode Hal_SelectionMode { get; set; }
        public Transition Hal_Transition { get; set; }

        public bool Hal_EnhanceEdgeArea_Enabled { get; set; }
        public Orientation Hal_EnhanceEdgeArea_Orientation { get; set; }
        public int Hal_EnhanceEdgeArea_EmpMaskWidth { get; set; }
        public int Hal_EnhanceEdgeArea_EmpMaskHeight { get; set; }
        public double Hal_EnhanceEdgeArea_EmpMaskFactor { get; set; }
        public int Hal_EnhanceEdgeArea_MeanMaskWidth { get; set; }
        public int Hal_EnhanceEdgeArea_MeanMaskHeight { get; set; }

        public int Hal_EnhanceEdgeArea_IterationCount { get; set; }
        public double Hal_EnhanceEdgeArea_ScaleMult { get; set; }
        public double Hal_EnhanceEdgeArea_ScaleAdd { get; set; }

        public bool Hal_EnhanceEdgeArea_SaveCacheImageEnabled { get; set; }

        public EdgeSearchingDefinition()
        {
        }

        public EdgeSearchingDefinition(Line line, double roiWidth = 0)
            : this(line.X1, line.Y1, line.X2, line.Y2, roiWidth)
        {
        }

        public EdgeSearchingDefinition(double startX, double startY, double endX, double endY, double roiWidth = 0)
        {
            StartX = startX;
            StartY = startY;
            EndX = endX;
            EndY = endY;
            ROIWidth = roiWidth;
        }

        public EdgeSearchingDefinition(Point p1, Point p2, double roiWidth = 0) :
            this(p1.X, p1.Y, p2.X, p2.Y, roiWidth)
        {
        }


        public Line Line
        {
            get { return new Line(StartX, StartY, EndX, EndY); }
        }

        //
        public Line RelativeLine { get; set; }
    }
}