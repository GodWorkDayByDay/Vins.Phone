using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using HalconDotNet;
using Hdc.Mv.Inspection.Halcon;

namespace Hdc.Mv.Inspection.Halcon
{
    public class HDevelopExportHelper: IDisposable
    {
        private readonly HImage _hImage;
        private readonly HImage _equHistoImage;
        private HImage _hImageCache;

        private static readonly HDevelopExport HDevelopExport = new HDevelopExport();

        public HDevelopExportHelper(HImage hImage)
        {
            _hImage = hImage;

            HObject imageEquHisto;
            HOperatorSet.EquHistoImage(_hImage, out imageEquHisto);
            _equHistoImage = new HImage(imageEquHisto);
        }

        public HDevelopExportHelper(ImageInfo imageInfo)
            : this((HImage) imageInfo.To8BppHImage())
        {
        }

        public HImage HImage
        {
            get { return _hImage; }
        }

        public HImage HImageCache
        {
            get { return _hImageCache; }
            set
            {
                if (_hImageCache!=null)
                    _hImageCache.Dispose();
                _hImageCache = value;
            }
        }

        public Line ExtracEdgeLine(Line line, double roiWidth, double low, double high, double filterAlpha)
        {
            return ExtracEdgeLine(line.X1, line.Y1, line.X2, line.Y2, roiWidth, low, high, filterAlpha);
        }

        public Line ExtracEdgeLine(double beginPointX, double beginPointY, double endPointX, double endPointY,
                                   double roiWidth, double low, double high, double filterAlpha)
        {
            HTuple lineBeginX, lineBeginY, lineEndX, lineEndY;
            HDevelopExport.ExtractEdgeLine(_hImage, beginPointY, beginPointX, endPointY, endPointX, roiWidth,
                low, high, filterAlpha,
                out lineBeginY, out lineBeginX, out lineEndY, out lineEndX);

            try
            {
                double[] lineX1s = lineBeginX;
                double[] lineX2s = lineEndX;
                double[] lineY1s = lineBeginY;
                double[] lineY2s = lineEndY;
                return new Line(lineX1s[0], lineY1s[0], lineX2s[0], lineY2s[0]);
            }
            catch (Exception)
            {
                try
                {
                    double lineX1 = lineBeginX;
                    double lineX2 = lineEndX;
                    double lineY1 = lineBeginY;
                    double lineY2 = lineEndY;
                    return new Line(lineX1, lineY1, lineX2, lineY2);
                }
                catch (Exception)
                {
                    throw;
                    //return new Line(0, 0, 0, 0);
                }
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


        public bool ExtractCircle(Point center, double innerCircleRadius, double outerCircleRadius,
                                  out Circle foundCircle, out double roundness)
        {
            try
            {
                HTuple centerX, centerY, radius, roundness2;
                HDevelopExport.ExtractCircle(_equHistoImage, center.Y, center.X, outerCircleRadius, innerCircleRadius,
                    out centerY, out centerX, out radius, out roundness2);

                foundCircle = new Circle(centerX, centerY, radius);
                roundness = roundness2;
                return true;
            }
            catch (HOperatorException)
            {
                foundCircle = new Circle();
                roundness = 0;
                return false;
            }
        }

        public bool ExtractCircle(double centerX, double centerY, double innerCircleRadius, double outerCircleRadius,
                                  out Circle foundCircle, out double roundness)
        {
            try
            {
                HTuple centerX2, centerY2, radius, roundness2;
                HDevelopExport.ExtractCircle(_equHistoImage, centerY, centerX, outerCircleRadius, innerCircleRadius,
                    out centerX2, out centerY2, out radius, out roundness2);

                foundCircle = new Circle(centerX, centerY, radius);
                roundness = 0.0; // TODO roundness2;
                return true;
            }
            catch (HOperatorException e)
            {
                foundCircle = new Circle();
                roundness = 0;
                return false;
            }
        }

        public bool ExtractCircle(double centerX, double centerY, double innerCircleRadius, double outerCircleRadius,
                                  out Circle foundCircle, out double roundness,
                                  int regionsCount,
//                                  int regionHeight,
                                  int regionWidth,
                                  double sigma, double threshold, SelectionMode selectionMode, Transition transition,
                                  CircleDirect direct)
        {
            try
            {
                HTuple centerX2, centerY2, radius, roundness2;
                HDevelopExport.SpokeCircle(_equHistoImage, centerY, centerX, outerCircleRadius, innerCircleRadius, regionsCount, regionWidth,
                    sigma, threshold,
                    transition.ToHalconString(),
                    selectionMode.ToHalconString(),
                    direct.ToHalconString(),
                    out centerY2, out centerX2, out radius, out roundness2);

                foundCircle = new Circle(centerX2, centerY2, radius);
                roundness = roundness2;
                return true;
            }
            catch (HOperatorException e)
            {
                foundCircle = new Circle();
                roundness = 0;
                return false;
            }
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
                HDevelopExport.GetCalibrationParameters(descriptionFileName, dirName, focus, sx, sy,
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
                HDevelopExport.GetCalibratedImage(orignalImage, out hCalibImage, interCamera, PoseNewOrigin, distance);

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
                HDevelopExport.Calibrate(orignalHImage, out hCalibImage, hv_CameraParams, hv_CameraPose,
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

        public ImageInfo FindS1404SurfaceA()
        {
            HObject binImageObject;

            HDevelopExport.FindS1404SurfaceA(_hImage, out binImageObject, 1);

            var hi = new HImage();
            HobjectToHimage(binImageObject, ref hi);

            var binImageInfo = hi.ToImageInfo();
            return binImageInfo;
        }

        public static Point IntersectionLines(Line line1, Line line2)
        {
            HTuple pX, pY, pp;
            HOperatorSet.IntersectionLines(line1.Y1, line1.X1, line1.Y2, line1.X2,
                line2.Y1, line2.X1, line2.Y2, line2.X2,
                out pY, out pX, out pp);

            if (pp != 0)
            {
                throw new HalconInspectorException();
            }

            var p = new Point(pX, pY);
            return p;
        }


        public IList<Line> RakeEdgeLine(HImage hImage, Line line,
                                        int regionsCount, int regionHeight, int regionWidth,
                                        double sigma, double threshold, Transition transition,
                                        SelectionMode selectionMode)
        {
            return RakeEdgeLine(hImage, line.X1, line.Y1, line.X2, line.Y2, regionsCount, regionHeight, regionWidth,
                sigma, threshold, transition, selectionMode);
        }

        public IList<Line> RakeEdgeLine(HImage hImage, double startX, double startY, double endX, double endY,
                                        int regionsCount, int regionHeight, int regionWidth,
                                        double sigma, double threshold, Transition transition, SelectionMode selectionMode)
        {
            // Local iconic variables 

            HObject ho_Image, ho_Regions;

            // Local control variables 

            HTuple hv_Row1 = null, hv_Column1 = null, hv_Row2 = null;
            HTuple hv_Column2 = null, hv_BeginRow = null, hv_BeginCol = null;
            HTuple hv_EndRow = null, hv_EndCol = null;

            // Initialize local and output iconic variables 

            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_Regions);

            try
            {
                //                ho_Image.Dispose();
                //                HOperatorSet.ReadImage(out ho_Image, @"B:\ConsoleApplication1\Untitled2.tif");
                ho_Regions.Dispose();

                hv_Row1 = new HTuple(startY);
                hv_Column1 = new HTuple(startX);
                hv_Row2 = new HTuple(endY);
                hv_Column2 = new HTuple(endX);

                HDevelopExport.RakeEdgeLine(hImage, regionsCount, regionHeight, regionWidth,
                    sigma, threshold, transition.ToHalconString(), selectionMode.ToHalconString(),
                    hv_Row1, hv_Column1, hv_Row2, hv_Column2,
                    out hv_BeginRow, out hv_BeginCol, out hv_EndRow, out hv_EndCol);

                double[] BeginRow = hv_BeginRow;
                double[] BeginColumn = hv_BeginCol;
                double[] EndRow = hv_EndRow;
                double[] EndColumn = hv_EndCol;

                IList<Line> lines = new List<Line>();

                for (int i = 0; i < BeginRow.Length; i++)
                {
                    lines.Add(new Line(BeginColumn[i], BeginRow[i], EndColumn[i], EndRow[i]));
                }

                return lines;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image.Dispose();
                ho_Regions.Dispose();

                throw HDevExpDefaultException;
            }
            ho_Image.Dispose();
            ho_Regions.Dispose();
        }

        public HImage EnhanceEdgeArea(HImage hImage, Line line, double hv_RoiWidthLen, int hv_EmpMaskWidth,
                                    int hv_EmpMaskHeight, double hv_EmpMaskFactor, int hv_MeanMaskWidth,
                                    int hv_MeanMaskHeight,
                                    int hv_IterationCount)
        {
            return EnhanceEdgeArea(hImage, line.Y1, line.X1, line.Y2, line.X2, hv_RoiWidthLen, hv_EmpMaskWidth, hv_EmpMaskHeight,
                hv_EmpMaskFactor, hv_MeanMaskWidth, hv_MeanMaskHeight, hv_IterationCount);
        }

        public HImage EnhanceEdgeArea(HObject ho_InputImage,
                                    double hv_LineStartPoint_Row, double hv_LineStartPoint_Column,
                                    double hv_LineEndPoint_Row,
                                    double hv_LineEndPoint_Column, double hv_RoiWidthLen, int hv_EmpMaskWidth,
                                    int hv_EmpMaskHeight, double hv_EmpMaskFactor, int hv_MeanMaskWidth,
                                    int hv_MeanMaskHeight,
                                    int hv_IterationCount)
        {
            HObject ho_EnhancedImage = null;
            HDevelopExport.EnhanceEdgeArea(
              ho_InputImage, out  ho_EnhancedImage,
                                    hv_LineStartPoint_Row,  hv_LineStartPoint_Column,
                                    hv_LineEndPoint_Row,
                                    hv_LineEndPoint_Column,  hv_RoiWidthLen,  hv_EmpMaskWidth,
                                    hv_EmpMaskHeight,  hv_EmpMaskFactor,  hv_MeanMaskWidth,
                                    hv_MeanMaskHeight,
                                    hv_IterationCount);
            return new HImage(ho_EnhancedImage);
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
        }
    }
}