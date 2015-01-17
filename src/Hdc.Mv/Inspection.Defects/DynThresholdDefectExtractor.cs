using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class DynThresholdDefectExtractor : DefectExtractor
    {
        public override HRegion GetBlobsInner(HImage image, HRegion domain)
        {
            HObject foundRegionObject;

            HDevelopExport.Singletone.GetBlobsByDynThreshold(image,
                out foundRegionObject,
                MedianRadius,
                MeanSpMaskWidth, // Halcon 12, mean_sp has a bug that height and width are invert
                MeanSpMaskHeight, // 
                MeanSpMinThreshold,
                MeanSpMaxThreshold,
                DynOffset,
                DynLightDark.ToHalconString());

            var hRegion = new HRegion(foundRegionObject);
            return hRegion;
        }

        public int MedianRadius { get; set; }
        public int MeanSpMaskWidth { get; set; }
        public int MeanSpMaskHeight { get; set; }
        public int MeanSpMinThreshold { get; set; }
        public int MeanSpMaxThreshold { get; set; }
        public double DynOffset { get; set; }
        public LightDark DynLightDark { get; set; }
    }
}