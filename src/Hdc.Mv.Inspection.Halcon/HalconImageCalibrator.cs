using System;
using System.Runtime.InteropServices;
using HalconDotNet;
using Hdc.Windows.Media.Imaging;

namespace Hdc.Mv.Inspection.Halcon
{
    public class HalconImageCalibrator : IImageCalibrator
    {
//        private HTuple cameraParams;
//        private HTuple pose;
//        private double distance;
//        private string _cammerParamsFileName;
//        private string _cameraPoseFileName;
//
//        public void Init(string cammerParamsFileName, string cameraPoseFileName, string calibImageFileName, string calibImageDirName)
//        {
//            _cameraPoseFileName = cameraPoseFileName;
//            _cammerParamsFileName = cammerParamsFileName;
//
//            if (cameraParams == null)
//                throw new HalconInspectorException("GetCalibrationParameters.cameraParams error");
//
//            if (pose == null)
//                throw new HalconInspectorException("GetCalibrationParameters.pose error");
//        }

        public ImageInfo CalibrateImage(ImageInfo originalImageInfo)
        {
            var cameraParamsFileName = "camera_params.cal";
            var cameraPoseFileName = "camera_pose.dat";

            HImage hImage;
            hImage = HDevelopExportHelper.Calibrate(originalImageInfo, cameraParamsFileName, cameraPoseFileName);

            var bsi = hImage.ToImageInfo();
            return bsi;
        }

    }
}