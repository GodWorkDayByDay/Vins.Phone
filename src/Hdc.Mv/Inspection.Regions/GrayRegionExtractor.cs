using System;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class GrayRegionExtractor : RegionExtractor
    {
        protected override HRegion GetRegion(HImage image)
        {
            HObject foundRegionObject;

            HDevelopExport.Singletone.GetRegionByGray(image,
                out foundRegionObject,
                MeanMaskWidth,
                MeanMaskHeight,
                EmpMaskWidth,
                EmpMaskHeight,
                EmpFactor,
                ScaleMult,
                ScaleAdd,
                ThresholdMinGray,
                ThresholdMaxGray,
                ErosionRadius,
                ClosingCircleRadius,
                ClosingRectWidth,
                ClosingRectHeight,
                DilationRadius,
                AreaMin,
                AreaMax);

            return new HRegion(foundRegionObject);
        }

        public HTuple MeanMaskWidth { get; set; }
        public HTuple MeanMaskHeight { get; set; }
        public HTuple EmpMaskWidth { get; set; }
        public HTuple EmpMaskHeight { get; set; }
        public HTuple EmpFactor { get; set; }
        public HTuple ScaleMult { get; set; }
        public HTuple ScaleAdd { get; set; }
        public HTuple ThresholdMinGray { get; set; }
        public HTuple ThresholdMaxGray { get; set; }
        public HTuple ErosionRadius { get; set; }
        public HTuple ClosingCircleRadius { get; set; }
        public HTuple ClosingRectWidth { get; set; }
        public HTuple ClosingRectHeight { get; set; }
        public HTuple DilationRadius { get; set; }
        public HTuple AreaMin { get; set; }
        public HTuple AreaMax { get; set; }
    }
}