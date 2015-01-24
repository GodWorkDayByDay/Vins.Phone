﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    public class SimGeneralInspector : IGeneralInspector
    {
        private HImage _image;

        public void Dispose()
        {
        }

        public void Init()
        {
        }

        public void SetImageInfo(HImage image)
        {
            _image = image;
        }

        public InspectionResult Inspect(InspectionSchema inspectionSchema)
        {
            var ir = new InspectionResult();
            var circles = SearchCircles(inspectionSchema.CircleSearchingDefinitions);
            var edges = SearchEdges(inspectionSchema.EdgeSearchingDefinitions);
            ir.Circles = circles;
            ir.Edges = edges;
            return ir;
        }

        public CircleSearchingResultCollection SearchCircles(IList<CircleSearchingDefinition> circleSearchingDefinitions)
        {
            var searchingResult = new CircleSearchingResultCollection();
            Random random = new Random();

            for (int i = 0; i < circleSearchingDefinitions.Count; i++)
            {
                var definition = circleSearchingDefinitions[i];
                int randomFactor = 0;
                var csr = new CircleSearchingResult()
                {
                    Index = i,
                    Name = definition.Name,
                    Circle =
                        new Circle(definition.CenterX + random.NextDouble() * randomFactor,
                        definition.CenterY + random.NextDouble() * randomFactor,
                        (definition.InnerRadius + definition.OuterRadius) / 2.0 + random.NextDouble() * randomFactor),
                    Definition = definition.DeepClone(),
                };
                //                if (i == 2)
                //                    csr.IsNotFound = true;

                searchingResult.Add(csr);
            }

            return searchingResult;
        }

//        public CircleSearchingResultCollection SearchCircles(HImage imageInfo, IList<CircleSearchingDefinition> circleSearchingDefinitions)
//        {
//            return SearchCircles(circleSearchingDefinitions);
//        }

        public EdgeSearchingResultCollection SearchEdges(IList<EdgeSearchingDefinition> edgeSearchingDefinitions)
        {
            var sr = new EdgeSearchingResultCollection();

            int i = 0;

            foreach (var esd in edgeSearchingDefinitions)
            {
                var esr = new EdgeSearchingResult(i, esd)
                {
                };

                var startVector = new Vector(esd.StartX, esd.StartY);
                var endVector = new Vector(esd.EndX, esd.EndY);

                var linkVector = startVector - endVector;
                var centerVector = new Vector((startVector.X + endVector.X) / 2.0, (startVector.Y + endVector.Y) / 2.0);
                var newLengthRatio = (esd.ROIWidth * 1.0) / linkVector.Length;

                var matrix = new Matrix();
                matrix.Rotate(90);
                var verticalVector = linkVector * matrix;
                verticalVector *= newLengthRatio;
                var newStartVector = centerVector + verticalVector;

                var matrix2 = new Matrix();
                matrix2.Rotate(-90);
                var verticalVector2 = linkVector * matrix2;
                verticalVector2 *= newLengthRatio;
                var newEndVector = centerVector + verticalVector2;

                esr.EdgeLine = new Line(newStartVector.X, newStartVector.Y, newEndVector.X, newEndVector.Y);

                sr.Add(esr);

                i++;
            }

            return sr;
        }

        public DefectResultCollection SearchDefects(HImage imageInfo)
        {
            var drc = new DefectResultCollection();
            for (int i = 0; i < 10; i++)
            {
                var dr = new DefectResult()
                         {
                             X = 200 * i,
                             Y = 200 * i,
                             Width = 150 + i * 100,
                             Height = 300 + i * 50,
                         };
                dr.Size = dr.Width*dr.Height;
                drc.Add(dr);
            }

            return drc;
        }

        public DefectResultCollection SearchDefects(HImage imageInfo, HImage mask)
        {
            return new DefectResultCollection();
        }

        public HImage FindRegions(HImage imageInfo)
        {
            throw new NotImplementedException();
        }

        public HImage FindRegions()
        {
            throw new NotImplementedException();
        }
    }
}