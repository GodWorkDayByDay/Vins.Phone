using System;
using System.Windows.Markup;

namespace Hdc.Mv.Inspection
{
    // ReSharper disable InconsistentNaming
    [Serializable]
    [ContentProperty("ProcessMethod")]
    public class CircleSearchingDefinition
    {
        public string Name { get; set; }

        // General
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double InnerRadius { get; set; }
        public double OuterRadius { get; set; }

        public double BaselineAngle { get; set; }
        public double BaselineX { get; set; }
        public double BaselineY { get; set; }

        // Mil
        public int Mil_LowThreshold { get; set; }
        public int Mil_HighThreshold { get; set; }
        public double Mil_CenterDiffer { get; set; }
        public double Mil_RadiusDiffer { get; set; }
        public double Mil_Ratio { get; set; }
        public CircleSearchingMethod Mil_CircleSearchingMethod { get; set; }
//        public bool BaselineOriginEnable { get; set; }
//        public bool BaselineReferenceEnable { get; set; }

        // 
        public int Hal_RegionsCount { get; set; }
//        public int Hal_RegionHeight { get; set; }
        public int Hal_RegionWidth { get; set; }
        public double Hal_Sigma { get; set; }
        public double Hal_Threshold { get; set; }
        public SelectionMode Hal_SelectionMode { get; set; }
        public Transition Hal_Transition { get; set; }
        public CircleDirect Hal_Direct { get; set; }

        public IProcessMethod ProcessMethod { get; set; }
    }

    // ReSharper restore InconsistentNaming

    public interface IProcessMethod
    {
//        string Name { get; }
    }

    [Serializable]
    public class HoughCircleDetectProcessMethod : IProcessMethod
    {
//        public string Name { get { return "HoughCircleDetect"; } }

        public double Sigma { get; set; }
        public int MinGray { get; set; }
        public int MaxGray { get; set; }
        public int ExpectRadius { get; set; }
        public int Percent { get; set; }
    }
}