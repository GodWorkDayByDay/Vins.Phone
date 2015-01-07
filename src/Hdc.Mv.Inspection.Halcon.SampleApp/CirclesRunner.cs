using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Hdc.Collections.Generic;
using Hdc.Mv.Inspection.Halcon.BatchInspector;

namespace Hdc.Mv.Inspection.Halcon.SampleApp
{
    public class CirclesRunner : Runner
    {
        public override void Run(ImageInfo imageInfo, InspectionSchema schema)
        {
            InspectionController
                .StartInspect()
              .SetInspectionSchema()
              .SetImageInfo(imageInfo.To8BppHImage())
              .CreateCoordinate()
              .Inspect()
              ;

            MainWindow.Show_CircleSearchingDefinitions(InspectionController.InspectionResult.GetCoordinateCircleSearchingDefinitions());
            MainWindow.Show_CircleSearchingResults(InspectionController.InspectionResult.CoordinateCircles);

            MainWindow.Show_CircleSearchingDefinitions(InspectionController.InspectionResult.GetCircleSearchingDefinitions(), Brushes.DodgerBlue);
            MainWindow.Show_CircleSearchingResults(InspectionController.InspectionResult.Circles, Brushes.DodgerBlue);


            var csv = new CsvWriterContainer("CirclesData" + DateTime.Now.ToString("_yyyy-MM-dd_HH.mm.ss_") + ".csv");
            // 
            Debug.WriteLine("Coordinate Circles Result \n");
            foreach (var def in InspectionController.InspectionResult.CoordinateCircles)
            {
                //def.UpdateRelativeCircle(InspectionController.Coordinate);

                Debug.WriteLine("Coordinate Circle " + def.Index);
                Debug.WriteLine("EXP.X = \t" + def.Definition.BaselineX);
                Debug.WriteLine("EXP.Y = \t" + def.Definition.BaselineY);
                Debug.WriteLine("ACT.X = \t" + def.Circle.CenterX);
                Debug.WriteLine("ACT.Y = \t" + def.Circle.CenterY);
                Debug.WriteLine("REL.X = \t" + def.RelativeCircle.CenterX * 16 / 1000);
                Debug.WriteLine("REL.Y = \t" + def.RelativeCircle.CenterY * 16 / 1000);
                Debug.WriteLine("\n");

                csv.CsvWriter.WriteField("Coordinate Circle ");
                csv.CsvWriter.WriteField(def.Index);
                csv.CsvWriter.NextRecord();
                csv.CsvWriter.WriteField("EXP.X = \t");
                csv.CsvWriter.WriteField(def.Definition.BaselineX);
                csv.CsvWriter.NextRecord();
                csv.CsvWriter.WriteField("EXP.Y = \t");
                csv.CsvWriter.WriteField(def.Definition.BaselineY);
                csv.CsvWriter.NextRecord();
                csv.CsvWriter.WriteField("ACT.X = \t");
                csv.CsvWriter.WriteField(def.Circle.CenterX);
                csv.CsvWriter.NextRecord();
                csv.CsvWriter.WriteField("ACT.Y = \t");
                csv.CsvWriter.WriteField(def.Circle.CenterY);
                csv.CsvWriter.NextRecord();
                csv.CsvWriter.WriteField("REL.X = \t");
                csv.CsvWriter.WriteField(def.RelativeCircle.CenterX * 16 / 1000);
                csv.CsvWriter.NextRecord();
                csv.CsvWriter.WriteField("REL.Y = \t");
                csv.CsvWriter.WriteField(def.RelativeCircle.CenterY * 16 / 1000);
                csv.CsvWriter.NextRecord();
                csv.CsvWriter.WriteField("\n");
                csv.CsvWriter.NextRecord();
            }

            // 
            Debug.WriteLine("Objective Circles Result \n");
            foreach (var def in InspectionController.InspectionResult.Circles)
            {
                //def.UpdateRelativeCircle(inspectionController.Coordinate);

                //                csv.CsvWriter.WriteField<string>("Objective Circle " + def.Index);
                //                csv.CsvWriter.WriteField<string>("EXP.X = \t" + def.Definition.BaselineX);
                //                csv.CsvWriter.WriteField<string>("EXP.Y = \t" + def.Definition.BaselineY);
                //                csv.CsvWriter.WriteField<string>("ACT.X = \t" + def.Circle.CenterX);
                //                csv.CsvWriter.WriteField<string>("ACT.Y = \t" + def.Circle.CenterY);
                //                csv.CsvWriter.WriteField<string>("REL.X = \t" + def.RelativeCircle.CenterX * 16 / 1000);
                //                csv.CsvWriter.WriteField<string>("REL.Y = \t" + def.RelativeCircle.CenterY * 16 / 1000);
                //                csv.CsvWriter.WriteField<string>("\n");

                csv.CsvWriter.WriteField("Objective Circle ");
                csv.CsvWriter.WriteField(def.Index);
                csv.CsvWriter.NextRecord();
                csv.CsvWriter.WriteField("EXP.X = \t");
                csv.CsvWriter.WriteField(def.Definition.BaselineX);
                csv.CsvWriter.NextRecord();
                csv.CsvWriter.WriteField("EXP.Y = \t");
                csv.CsvWriter.WriteField(def.Definition.BaselineY);
                csv.CsvWriter.NextRecord();
                csv.CsvWriter.WriteField("ACT.X = \t");
                csv.CsvWriter.WriteField(def.Circle.CenterX);
                csv.CsvWriter.NextRecord();
                csv.CsvWriter.WriteField("ACT.Y = \t");
                csv.CsvWriter.WriteField(def.Circle.CenterY);
                csv.CsvWriter.NextRecord();
                csv.CsvWriter.WriteField("REL.X = \t");
                csv.CsvWriter.WriteField(def.RelativeCircle.CenterX * 16 / 1000);
                csv.CsvWriter.NextRecord();
                csv.CsvWriter.WriteField("REL.Y = \t");
                csv.CsvWriter.WriteField(def.RelativeCircle.CenterY * 16 / 1000);
                csv.CsvWriter.NextRecord();
                csv.CsvWriter.WriteField("\n");
                csv.CsvWriter.NextRecord();
            }

            csv.Dispose();
        }
    }
}