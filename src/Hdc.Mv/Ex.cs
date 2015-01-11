﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using HalconDotNet;
using Hdc.IO;
using Hdc.Linq;
using Hdc.Mv.Inspection.Halcon;
using Hdc.Reflection;
using Hdc.Windows.Media.Imaging;

namespace Hdc.Mv
{
    public static class Ex
    {
        static Ex()
        {
        }

        public static Point ToPoint(this Vector vector)
        {
            return new Point(vector.X, vector.Y);
        }

        public static Vector ToVector(this Point point)
        {
            return new Vector(point.X, point.Y);
        }

        public static Point GetCenterPoint(this Circle circle)
        {
            return new Point(circle.CenterX, circle.CenterY);
        }

        public static Vector GetCenterVector(this Circle circle)
        {
            return new Vector(circle.CenterX, circle.CenterY);
        }

        public static double GetDistanceToOrigin(this Circle circle)
        {
            return circle.GetCenterVector().Length;
        }

        public static Vector GetVectorTo(this Circle circle1, Circle circle2)
        {
            var v1 = circle1.GetCenterVector();
            var v2 = circle2.GetCenterVector();
            var v1to2 = v1 - v2;
            return v1to2;
        }

        public static Vector GetVectorTo(this Point p1, Point p2)
        {
            var v1 = p1.ToVector();
            var v2 = p2.ToVector();
            var v = v1 - v2;
            return v;
        }

        public static Vector GetVectorTo(this Vector p1, Vector p2)
        {
            var v = p1 - p2;
            return v;
        }

        public static double GetDistanceTo(this Circle circle1, Circle circle2)
        {
            return circle1.GetVectorTo(circle2).Length;
        }

        public static Point GetPoint1(this Line line)
        {
            return new Point(line.X1, line.Y1);
        }

        public static Point GetPoint2(this Line line)
        {
            return new Point(line.X2, line.Y2);
        }

        public static Vector GetVector1(this Line line)
        {
            return new Vector(line.X1, line.Y1);
        }

        public static Vector GetVector2(this Line line)
        {
            return new Vector(line.X2, line.Y2);
        }

        public static Vector GetVectorFrom1To2(this Line line)
        {
            var v1 = line.GetVector1();
            var v2 = line.GetVector2();
            var v = v1 - v2;
            return v;
        }

        public static Vector GetVectorFrom2To1(this Line line)
        {
            var v1 = line.GetVector1();
            var v2 = line.GetVector2();
            var v = v2 - v1;
            return v;
        }

        public static double GetLength(this Line line)
        {
            return line.GetVectorFrom1To2().Length;
        }

        public static Vector Rotate(this Vector vector, double angle)
        {
            var matrix = new Matrix();
            matrix.Rotate(angle);
            var rotatedVector = matrix.Transform(vector);
            return rotatedVector;
        }

        public static double GetAngleTo(this Vector fromVector, Vector toVector)
        {
            return Vector.AngleBetween(fromVector, toVector);
        }

        public static Point GetRelativePoint(this Point point, Line baseLine, double angle)
        {
            var vFromOriginToTarget = point.GetVectorTo(baseLine.GetPoint1());
            var vFromOriginToRight = baseLine.GetPoint2().GetVectorTo(baseLine.GetPoint1());
            var coordinateVector = vFromOriginToRight.Rotate(angle);
                // 0 degree mains: the line is X, direct to right. x>0, follow the clock.

            var angleBetweenTargetAndRight = vFromOriginToTarget.GetAngleTo(coordinateVector);

            var vectorNoAngle = new Vector(vFromOriginToTarget.Length, 0);
            var vectorIncludeAngle = vectorNoAngle.Rotate(0 - angleBetweenTargetAndRight);
            return vectorIncludeAngle.ToPoint();
        }

        public static Point CreateRelativeC(this Point point, Line baseLine, double angle)
        {
            var vFromOriginToTarget = point.GetVectorTo(baseLine.GetPoint1());
            var vFromOriginToRight = baseLine.GetPoint2().GetVectorTo(baseLine.GetPoint1());
            var coordinateVector = vFromOriginToRight.Rotate(angle);
                // 0 degree mains: the line is X, direct to right. x>0, follow the clock.

            var angleBetweenTargetAndRight = vFromOriginToTarget.GetAngleTo(coordinateVector);

            var vectorNoAngle = new Vector(vFromOriginToTarget.Length, 0);
            var vectorIncludeAngle = vectorNoAngle.Rotate(0 - angleBetweenTargetAndRight);
            return vectorIncludeAngle.ToPoint();
        }

        public static Point GetCenterPoint(this Line line)
        {
            return new Point((line.X1 + line.X2)/2.0, (line.Y1 + line.Y2)/2.0);
        }

        public static string ToNumbericString(this double value, int intCount = 4)
        {
            switch (intCount)
            {
                case 0:
                    throw new NotSupportedException();
                case 1:
                    return value.ToString("+0.000;-0.000");
                case 2:
                    return value.ToString("+00.000;-00.000");
                case 3:
                    return value.ToString("+000.000;-000.000");
                case 4:
                    return value.ToString("+0000.000;-0000.000");
                case 5:
                    return value.ToString("+00000.000;-00000.000");
                case 6:
                    return value.ToString("+000000.000;-000000.000");
            }
            return null;
        }

        public static double ToMillimeterFromPixel(this double value, double factor)
        {
            return value*factor/1000.0;
        }

        public static double ToMicrometerFromPixel(this double value, double factor)
        {
            return value*factor;
        }

        public static string ToNumbericStringInMillimeterFromPixel(this double value, double factor, int intCount = 4)
        {
            return value.ToMillimeterFromPixel(factor).ToNumbericString(intCount); //+" mm";
        }

        public static string ToNumbericStringInMicrometerFromPixel(this double value, double factor, int intCount = 4)
        {
            return value.ToMicrometerFromPixel(factor).ToNumbericString(intCount); // + " um";
        }

        public static string ToHalconString(this Polarity polarity)
        {
            string polarityString = null;

            switch (polarity)
            {
                case Polarity.All:
                    polarityString = "all";
                    break;
                case Polarity.Negative:
                    polarityString = "negative";
                    break;
                case Polarity.Positive:
                    polarityString = "positive";
                    break;
            }

            return polarityString;
        }


        public static string ToHalconString(this Transition transition)
        {
            string polarityString = null;

            switch (transition)
            {
                case Transition.All:
                    polarityString = "all";
                    break;
                case Transition.Negative:
                    polarityString = "negative";
                    break;
                case Transition.Positive:
                    polarityString = "positive";
                    break;
            }

            return polarityString;
        }


        public static string ToHalconString(this SelectionMode selectionMode)
        {
            string selectionModeString = null;

            switch (selectionMode)
            {
                case SelectionMode.Max:
                    selectionModeString = "max";
                    break;
                case SelectionMode.First:
                    selectionModeString = "first";
                    break;
                case SelectionMode.Last:
                    selectionModeString = "last";
                    break;
            }

            return selectionModeString;
        }


        public static string ToHalconString(this CircleDirect selectionMode)
        {
            string selectionModeString = null;

            switch (selectionMode)
            {
                case CircleDirect.Inner:
                    selectionModeString = "inner";
                    break;
                case CircleDirect.Outer:
                    selectionModeString = "outer";
                    break;
            }

            return selectionModeString;
        }


        public static HRegion GetRegion(this IRectangle2Def rect)
        {
            var processRegion = new HRegion();
            processRegion.GenRectangle2(rect.Y, rect.X, rect.Angle, rect.HalfWidth, rect.HalfHeight);
            return processRegion;
        }

        public static void SaveCacheImagesForRegion(this HImage image, HRegion domain, HRegion region, string fileName)
        {
            var dir = typeof(Ex).Assembly.GetAssemblyDirectoryPath();
            var cacheDir = Path.Combine(dir, "CacheImages");

            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
            }

            var reducedImage = image.ReduceDomain(domain);
            var croppedImage = reducedImage.CropDomain();
            croppedImage.ToBitmapSource().SaveToTiff(cacheDir.CombilePath(fileName) + ".Ori.tif");

            var paintRegion = reducedImage.PaintRegion(region, 255.0, "margin");
            var croppedImage2 = paintRegion.CropDomain();
            croppedImage2.ToBitmapSource().SaveToTiff(cacheDir.CombilePath(fileName) + ".Paint.Margin.tif");

            var paintRegion2 = reducedImage.PaintRegion(region, 255.0, "fill");
            var croppedImage2b = paintRegion2.CropDomain();
            croppedImage2b.ToBitmapSource().SaveToTiff(cacheDir.CombilePath(fileName) + ".Paint.Fill.tif");

            var reducedImage3 = image.ReduceDomain(region);
            var croppedImage3 = reducedImage3.CropDomain();
            croppedImage3.ToBitmapSource().SaveToTiff(cacheDir.CombilePath(fileName) + ".Crop.tif");

            domain.Dispose();
            reducedImage.Dispose();
            reducedImage3.Dispose();
            croppedImage.Dispose();
            croppedImage2.Dispose();
            croppedImage2b.Dispose();
            croppedImage3.Dispose();
            paintRegion.Dispose();
        }

        public static void SaveCacheImagesForRegion(this HImage image, HRegion domain, HRegion includeRegion, HRegion excludeRegion, string fileName)
        {
            var dir = typeof(Ex).Assembly.GetAssemblyDirectoryPath();
            var cacheDir = Path.Combine(dir, "CacheImages");

            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
            }

            var imageWidth = image.GetWidth();
            var imageHeight = image.GetHeight();

            // Domain.Ori
            var reducedImage = image.ReduceDomain(domain);
            var croppedImage = reducedImage.CropDomain();
            croppedImage.ToBitmapSource().SaveToTiff(cacheDir.CombilePath(fileName) + ".Domain.Ori.tif");
            reducedImage.Dispose();
            croppedImage.Dispose();

            // Domain.PaintMargin
            var reducedImage4 = image.ReduceDomain(domain);
            var paintRegionImage = reducedImage4.PaintRegion(includeRegion, 250.0, "margin");
            var paintRegion2Image = paintRegionImage.PaintRegion(excludeRegion, 5.0, "margin");
            var croppedImage2 = paintRegion2Image.CropDomain();
            croppedImage2.ToBitmapSource().SaveToTiff(cacheDir.CombilePath(fileName) + ".Domain.PaintMargin.tif");
            reducedImage4.Dispose();
            croppedImage2.Dispose();
            paintRegionImage.Dispose();
            paintRegion2Image.Dispose();

            // PaintFill
//            var paintRegion3Image = reducedImage.PaintRegion(includeRegion, 250.0, "fill");
//            var croppedImage2bImage = paintRegion3Image.CropDomain();
//            croppedImage2bImage.ToBitmapSource().SaveToTiff(cacheDir.CombilePath(fileName) + ".Domain.PaintFill.tif");
//            croppedImage2bImage.Dispose();

            // Domain.Crop
            var reducedImage3 = image.ReduceDomain(includeRegion);
            var croppedImage3 = reducedImage3.CropDomain();
            croppedImage3.ToBitmapSource().SaveToTiff(cacheDir.CombilePath(fileName) + ".Domain.Crop.tif");
            reducedImage3.Dispose();
            croppedImage3.Dispose();

            // bin image in domain
            var row1 = domain.GetRow1();
            var column1 = domain.GetColumn1();
            var movedRegion= includeRegion.MoveRegion(-row1, -column1);

            var w = domain.GetWidth();
            var h = domain.GetHeight();
            var binImage = movedRegion.RegionToBin(255, 0, w, h);
            binImage.ToBitmapSource().SaveToTiff(cacheDir.CombilePath(fileName) + ".Domain.Bin.tif");
            binImage.Dispose();
            movedRegion.Dispose();

            // Full.Bin, 
            var binImage2 = includeRegion.RegionToBin(255, 0, imageWidth, imageHeight);
            binImage2.ToBitmapSource().SaveToJpeg(cacheDir.CombilePath(fileName) + ".Full.Bin.jpg");
            binImage2.Dispose();

            // Full.BinOnlyDomain
            var binImage3 = includeRegion.RegionToBin(255, 0, imageWidth, imageHeight);
            var reducedImage5= binImage3.ReduceDomain(domain);
            var binOnlyDomainImage = image.Clone();
            binOnlyDomainImage.OverpaintGray(reducedImage5);
            binOnlyDomainImage.ToBitmapSource().SaveToJpeg(cacheDir.CombilePath(fileName) + ".Full.BinOnlyDomain.jpg");

            binImage3.Dispose();
            reducedImage5.Dispose();
            binOnlyDomainImage.Dispose();
        }
    }
}