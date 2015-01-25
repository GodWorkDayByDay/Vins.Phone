using System;
using HalconDotNet;
using Hdc.Mv.Halcon;

namespace Hdc.Mv.Halcon
{
    public static class HDevelopExportExtensions
    {
        public static HImage Calibrate(this ImageInfo originalImageInfo,
                                       string hv_CameraParams,
                                       string hv_CameraPose
            //            out double hv_LengthPerPixelX,
            //            out double hv_LengthPerPixelY
            )
        {
            try
            {
                var orignalHImage = originalImageInfo.To8BppHImage();

                HObject hCalibImage;
                HTuple lengthPerPixelX, lengthPerPixelY;
                HDevelopExport.Singletone.Calibrate(orignalHImage, out hCalibImage, hv_CameraParams, hv_CameraPose,
                    out lengthPerPixelX, out lengthPerPixelY);

                var hi = new HImage();
                hCalibImage.HobjectToHimage(ref hi);
                return hi;
            }
            catch (Exception e)
            {
                throw;
            }
        }


        [Obsolete]
        public static ImageInfo FindS1404SurfaceA(ImageInfo imageInfo)
        {
            var orignalHImage = imageInfo.To8BppHImage();

            HObject binImageObject;

            HDevelopExport.Singletone.FindS1404SurfaceA(orignalHImage, out binImageObject, 1);

            var hi = new HImage();
            binImageObject.HobjectToHimage(ref hi);

            var binImageInfo = hi.ToImageInfo();
            return binImageInfo;
        }

        [Obsolete]
        public static HImage FindS1404SurfaceA(this HImage image)
        {
            HObject binImageObject;

            HDevelopExport.Singletone.FindS1404SurfaceA(image, out binImageObject, 1);

            var hi = new HImage();
            binImageObject.HobjectToHimage(ref hi);

            //            var binImageInfo = hi.ToImageInfo();
            //            return binImageInfo;
            return hi;
        }

    }
}