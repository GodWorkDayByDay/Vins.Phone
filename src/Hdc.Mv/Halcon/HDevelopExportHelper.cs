using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using HalconDotNet;
using Hdc.Diagnostics;
using Hdc.Mv.Halcon;
using Hdc.Windows.Media.Imaging;

namespace Hdc.Mv.Inspection
{
    public class HDevelopExportHelper : IDisposable
    {
        private readonly HImage _hImage;
        private readonly HImage _equHistoImage;
        private readonly HImage _horizontalMeanImage;
        private readonly HImage _verticalMeanImage;
        private readonly HImage _emphasizeHorizontalMeanImage;
        private readonly HImage _emphasizeVerticalMeanImage;
        private HImage _hImageCache;

        private readonly HDevelopExport HDevelopExport = new HDevelopExport();
        private HImage _borderImage;

        public bool SaveCacheImages { get; set; }

        public HDevelopExportHelper(HImage hImage)
        {
            SaveCacheImages = false;

            var outerRegion = new HRegion(812.0, 612, 812.0 + 11200, 612 + 6050);

            var innerRegion = new HRegion(1748.0, 1436, 1748.0 + 9000, 1436 + 4400);

            var border = outerRegion.Difference(innerRegion);

            _hImage = hImage.ReduceDomain(outerRegion);
            //            _borderImage = hImage.ReduceDomain(border);

            using (var sw = new NotifyStopwatch("EquHistoImage"))
            {
                _equHistoImage = _hImage.EquHistoImage();
            }

            using (var sw = new NotifyStopwatch("MeanImage 6,2"))
            {
                //_horizontalMeanImage = _hImage.MeanImage(6, 1);
                //                _horizontalMeanImage = _borderImage.MeanSp(6, 3, 100, 200);
                //                _emphasizeHorizontalMeanImage = _horizontalMeanImage.Emphasize(3, 7, 5);
            }

            using (var sw = new NotifyStopwatch("MeanImage 2,6"))
            {
                //_verticalMeanImage = _hImage.MeanImage(1, 6);
                //                _verticalMeanImage = _borderImage.MeanSp(3, 6, 120, 200);
                //                _emphasizeVerticalMeanImage = _verticalMeanImage.Emphasize(7, 3, 5);
            }

            if (SaveCacheImages)
                _emphasizeHorizontalMeanImage.ToImageInfo()
                    .ToBitmapSource()
                    .SaveToJpeg("_EmphasizeHorizontalMeanImage.jpg");

            if (SaveCacheImages)
                _emphasizeVerticalMeanImage.ToImageInfo().ToBitmapSource().SaveToJpeg("_EmphasizeVerticalMeanImage.jpg");

            //            _horizontalMeanImage.Dispose();
            //            _verticalMeanImage.Dispose();
            //            _emphasizeHorizontalMeanImage.Dispose();
            //            _emphasizeVerticalMeanImage.Dispose();
            //            _borderImage.Dispose();
        }

        public HDevelopExportHelper(ImageInfo imageInfo)
            : this((HImage)imageInfo.To8BppHImage())
        {
        }

        public HImage HImage
        {
            get { return _hImage; }
        }

        public HImage HorizontalMeanImage
        {
            get { return _horizontalMeanImage; }
        }

        public HImage VerticalMeanImage
        {
            get { return _verticalMeanImage; }
        }

        public HImage EmphasizeHorizontalMeanImage
        {
            get { return _emphasizeHorizontalMeanImage; }
        }

        public HImage EmphasizeVerticalMeanImage
        {
            get { return _emphasizeVerticalMeanImage; }
        }

        public HImage HImageCache
        {
            get { return _hImageCache; }
            set
            {
                if (_hImageCache != null)
                    _hImageCache.Dispose();
                _hImageCache = value;
            }
        }


        public void DistanceOfLineToLine(Line line1, Line line2, out Line distanceLine, out Point root, out double angle)
        {
            HTuple distance, distanceBeginX, distanceBeginY, distanceEndX, distanceEndY, rootX, rootY, hAngle;

            HDevelopExport.DistanceOfLineToLine(
                line1.Y1, line1.X1, line1.Y2, line1.X2,
                line2.Y1, line2.X1, line2.Y2, line2.X2,
                out distance, out distanceBeginY, out distanceBeginX, out distanceEndY, out distanceEndX, out rootY,
                out rootX, out hAngle);

            distanceLine = new Line(distanceBeginX, distanceBeginY, distanceEndX, distanceEndY);
            root = new Point(rootX, rootY);
            angle = hAngle.ToDArr()[0];
        }


        public static bool GetCalibrationParameters(string descriptionFileName, string dirName, double focus, double sx,
                                                    double sy,
                                                    double width, double height, string cameraType,
                                                    out HTuple interCamera, out HTuple PoseNewOrigin,
                                                    out double distance)
        {
            try
            {
                HTuple hv_interCamera, hv_PoseNewOrigin, hv_distance;
                HDevelopExport.Singletone.GetCalibrationParameters(descriptionFileName, dirName, focus, sx, sy,
                    width, height, cameraType, out hv_interCamera, out hv_PoseNewOrigin, out hv_distance);

                interCamera = hv_interCamera;
                PoseNewOrigin = hv_PoseNewOrigin;
                distance = hv_distance;

                return true;
            }
            catch (HOperatorException e)
            {
                interCamera = null;
                PoseNewOrigin = null;
                distance = 0;
                return false;
            }
        }

        public static bool GetCalibrationParameters(ImageInfo image, HTuple interCamera, HTuple PoseNewOrigin,
                                                    double distance, out HImage calibImage)
        {
            try
            {
                var orignalImage = image.To8BppHImage();
                //                ImageInfo calibImage;

                HObject hCalibImage;
                HDevelopExport.Singletone.GetCalibratedImage(orignalImage, out hCalibImage, interCamera, PoseNewOrigin, distance);

                var hi = new HImage();
                HobjectToHimage(hCalibImage, ref hi);
                calibImage = hi;
                return true;
            }
            catch (HOperatorException e)
            {
                calibImage = null;
                return false;
            }
        }

        private static void HobjectToHimage(HObject hobject, ref HImage image)
        {
            HTuple pointer, type, width, height;

            HOperatorSet.GetImagePointer1(hobject, out pointer, out type, out width, out height);
            image.GenImage1(type, width, height, pointer);
        }


        public static HImage Calibrate(
            ImageInfo originalImageInfo,
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
                HobjectToHimage(hCalibImage, ref hi);
                return hi;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public ImageInfo FindS1404SurfaceA(ImageInfo imageInfo)
        {
            var orignalHImage = imageInfo.To8BppHImage();

            HObject binImageObject;

            HDevelopExport.FindS1404SurfaceA(orignalHImage, out binImageObject, 1);

            var hi = new HImage();
            HobjectToHimage(binImageObject, ref hi);

            var binImageInfo = hi.ToImageInfo();
            return binImageInfo;
        }

        public HImage FindS1404SurfaceA()
        {
            HObject binImageObject;

            HDevelopExport.FindS1404SurfaceA(_hImage, out binImageObject, 1);

            var hi = new HImage();
            HobjectToHimage(binImageObject, ref hi);

//            var binImageInfo = hi.ToImageInfo();
//            return binImageInfo;
            return hi;
        }


        public HImage AddImagesWithFullDomain(HImage image1, HImage image2)
        {
            var imageFull1 = image1.FullDomain();
            var imageFull2 = image2.FullDomain();
            return imageFull1.AddImage(imageFull2, new HTuple(1), new HTuple(0));
        }

        public void Dispose()
        {
            if (_hImage != null) _hImage.Dispose();
            if (_hImageCache != null) _hImageCache.Dispose();
            if (_horizontalMeanImage != null) _horizontalMeanImage.Dispose();
            if (_verticalMeanImage != null) _verticalMeanImage.Dispose();
            if (_emphasizeHorizontalMeanImage != null) _emphasizeHorizontalMeanImage.Dispose();
            if (_emphasizeVerticalMeanImage != null) _emphasizeVerticalMeanImage.Dispose();
            if (_borderImage != null) _borderImage.Dispose();
        }

     

        public HRegion GetRegionByGrayAndArea(HImage image,
                                              int medianRadius,
                                              int empWidth, int empHeight, double empFactor,
                                              int thresholdMinGray, int thresholdMaxGray,
                                              int areaMin, int areaMax,
                                              double closingRadius, double dilationRadius)
        {
            HObject foundRegionObject;

            HDevelopExport.GetRegionByGrayAndArea(image, out foundRegionObject, medianRadius,
                empWidth, empHeight, empFactor, thresholdMinGray, thresholdMaxGray, areaMin,
                areaMax,
                closingRadius, dilationRadius);

            return new HRegion(foundRegionObject);
        }
    }
}