namespace Hdc.Mv.Inspection
{
    public interface IImageCalibrator
    {
//        void Init(string cammerParamsFileName,
//                  string cameraPoseFileName,
//                  string calibImageFileName,
//                  string calibImageDirName);

        ImageInfo CalibrateImage(ImageInfo originalImageInfo);
    }
}