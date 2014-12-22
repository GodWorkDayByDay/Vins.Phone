using HalconDotNet;

namespace Hdc.Mv.Inspection.Halcon
{
    public static class ImageInfoExtensions
    {
        public static HImage To8BppHImage(this ImageInfo imageInfo)
        {
/*            // Local iconic variables 

            HObject ho_Image, ho_Region, ho_ConnectedRegions;
            HObject ho_SelectedRegions;


            // Local control variables 

            HTuple hv_WindowID = new HTuple(), hv_Rows = null;
            HTuple hv_Columns = null, hv_RowLine1 = null, hv_ColLine1 = null;
            HTuple hv_RowLine2 = null, hv_ColLine2 = null, hv_NumberTuple = null;
            HTuple hv_i = null, hv_Distance = new HTuple();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);

            HOperatorSet.GenImage1(out ho_Image, new HTuple("byte"), 
                new HTuple(bitmapSourceInfo.PixelWidth), 
                new HTuple(bitmapSourceInfo.PixelHeight), 
                bitmapSourceInfo.BufferPtr);

            return ho_Image;*/


            var hImage = new HImage("byte", imageInfo.PixelWidth, imageInfo.PixelHeight, imageInfo.BufferPtr);
            return hImage;
        }
    }
}