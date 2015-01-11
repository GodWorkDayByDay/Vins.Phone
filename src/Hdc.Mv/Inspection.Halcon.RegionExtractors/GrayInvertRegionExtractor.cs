using System;
using HalconDotNet;
using Hdc.Mv.Inspection.Halcon;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class GrayInvertRegionExtractor : RegionExtractor
    {
        protected override HRegion GetRegion(HImage image)
        {
/*            var foundRegion = image.GetRegionByGrayInvert(
                MeanMaskWidth,
                MeanMaskHeight,
                EmpMaskWidth,
                EmpMaskHeight,
                EmpFactor1,
                EmpFactor2,
                ThresholdMinGray,
                ThresholdMaxGray,
                AreaMin,
                AreaMax);
            return foundRegion;*/

            HObject foundRegionObject;

            HDevelopExport.Singletone.GetRegionByGrayInvert(image,
                out foundRegionObject,
                MeanMaskWidth,
                MeanMaskHeight,
                EmpMaskWidth,
                EmpMaskHeight,
                EmpFactor1,
                EmpFactor2,
                ThresholdMinGray,
                ThresholdMaxGray,
                AreaMin,
                AreaMax);

            var hRegion = new HRegion(foundRegionObject);

/*            image.Clone().CropDomain().WriteImage("tiff", 100, @"B:\Temp_0_ori_crop.tif");
            
            var meanImage = image.MeanImage(MeanMaskWidth, MeanMaskHeight);
//            meanImage.Clone().WriteImage("tiff", 100, @"B:\Temp_1_meanImage.tif");
            meanImage.Clone().CropDomain().WriteImage("tiff", 100, @"B:\Temp_1_meanImage_crop.tif");

            var empImage = meanImage.Emphasize(EmpMaskWidth, EmpMaskHeight, EmpFactor1);
//            empImage.Clone().WriteImage("tiff", 100, @"B:\Temp_2_empImage.tif");
            empImage.Clone().CropDomain().WriteImage("tiff", 100, @"B:\Temp_2_empImage_crop.tif");

            var invertEmpImage = empImage.InvertImage();
//            invertEmpImage.Clone().WriteImage("tiff", 100, @"B:\Temp_3_invertEmpImage.tif");
            invertEmpImage.Clone().CropDomain().WriteImage("tiff", 100, @"B:\Temp_3_invertEmpImage_crop.tif");

            var absDiffImage = empImage.AbsDiffImage(invertEmpImage, 1.0);
//            absDiffImage.Clone().WriteImage("tiff", 100, @"B:\Temp_4_absDiffImage.tif");
            absDiffImage.Clone().CropDomain().WriteImage("tiff", 100, @"B:\Temp_4_absDiffImage_crop.tif");*/

//            var hRegion = new HRegion();
//            var x =image.Clone();
//            x.OverpaintRegion(hRegion, 100.0, "fill");
//            x.CropDomain().WriteImage("tiff", 100, @"B:\Temp2.tiff");
            return hRegion;
        }

        public int MeanMaskWidth { get; set; }
        public int MeanMaskHeight { get; set; }
        public int EmpMaskWidth { get; set; }
        public int EmpMaskHeight { get; set; }
        public double EmpFactor1 { get; set; }
        public double EmpFactor2 { get; set; }
        public double ThresholdMinGray { get; set; }
        public double ThresholdMaxGray { get; set; }
        public double AreaMin { get; set; }
        public double AreaMax { get; set; }
    }
}