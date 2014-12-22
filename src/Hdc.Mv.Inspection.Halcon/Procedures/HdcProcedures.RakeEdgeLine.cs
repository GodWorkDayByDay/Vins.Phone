/*using System;
using System.IO;
using HalconDotNet;
using Hdc.Reflection;

namespace Hdc.Mv.Inspection.Halcon
{
    public static partial class HdcProcedures
    {
        // Local procedures 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ho_Image"></param>
        /// <param name="hv_Elements"></param>
        /// <param name="hv_DetectHeight"></param>
        /// <param name="hv_DetectWidth"></param>
        /// <param name="hv_Sigma"></param>
        /// <param name="hv_Threshold"></param>
        /// <param name="hv_Transition"></param>
        /// <param name="hv_Select"></param>
        /// <param name="hv_Row1"></param>
        /// <param name="hv_Column1"></param>
        /// <param name="hv_Row2"></param>
        /// <param name="hv_Column2"></param>
        /// <param name="hv_BeginRow"></param>
        /// <param name="hv_BeginCol"></param>
        /// <param name="hv_EndRow"></param>
        /// <param name="hv_EndCol"></param>
        public static void RakeEdgeLine(HObject ho_Image, HTuple hv_Elements, HTuple hv_DetectHeight,
            HTuple hv_DetectWidth, HTuple hv_Sigma, HTuple hv_Threshold, HTuple hv_Transition,
            HTuple hv_Select, HTuple hv_Row1, HTuple hv_Column1, HTuple hv_Row2, HTuple hv_Column2,
            out HTuple hv_BeginRow, out HTuple hv_BeginCol, out HTuple hv_EndRow, out HTuple hv_EndCol)
        {
            //            string ProcedurePath = @"C:\Program Files\MVTec\HALCON-11.0\procedures\general";
            //            String ProgramPathString = @"C:\Program Files\MVTec\HALCON-11.0\procedures\general\RakeEdgeLine.hdev";
            //            String ProgramPathString = @"C:\Program Files\MVTec\HALCON-11.0\procedures\general\New Folder\RakeEdgeLine.hdev";

            // Local iconic variables 

            HObject ho_Regions, ho_Line;

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_Line);

            //
            ho_Regions.Dispose();

            var assDir = typeof(HdcProcedures).Assembly.GetAssemblyDirectoryPath();
            String ProgramPathString = Path.Combine(assDir, @"RakeEdgeLine.hdev");

            HDevProgram Program = new HDevProgram(ProgramPathString);
            HDevProcedure rakeProc = new HDevProcedure(Program, "rake");
            HDevProcedure ptsProc = new HDevProcedure(Program, "pts_to_best_line");
            HDevProcedureCall rakeProcCall;
            HDevProcedureCall ptsProcCall;


            HOperatorSet.EquHistoImage(ho_Image, out ho_Image);
            //            HOperatorSet.WriteImage(ho_Image, "tiff", 0, "Histo.tif");

            rakeProcCall = new HDevProcedureCall(rakeProc);
            rakeProcCall.SetInputIconicParamObject("Image", ho_Image);
            rakeProcCall.SetInputCtrlParamTuple("Elements", hv_Elements);
            rakeProcCall.SetInputCtrlParamTuple("DetectHeight", hv_DetectHeight);
            rakeProcCall.SetInputCtrlParamTuple("DetectWidth", hv_DetectWidth);
            rakeProcCall.SetInputCtrlParamTuple("Sigma", hv_Sigma);
            rakeProcCall.SetInputCtrlParamTuple("Threshold", hv_Threshold);
            rakeProcCall.SetInputCtrlParamTuple("Transition", hv_Transition);
            rakeProcCall.SetInputCtrlParamTuple("Select", hv_Select);
            rakeProcCall.SetInputCtrlParamTuple("Row1", hv_Row1);
            rakeProcCall.SetInputCtrlParamTuple("Column1", hv_Column1);
            rakeProcCall.SetInputCtrlParamTuple("Row2", hv_Row2);
            rakeProcCall.SetInputCtrlParamTuple("Column2", hv_Column2);
            rakeProcCall.Execute();
            // get output parameters from procedure call

            ho_Regions = rakeProcCall.GetOutputIconicParamXld("Regions");
            hv_BeginRow = rakeProcCall.GetOutputCtrlParamTuple("ResultRow");
            hv_BeginCol = rakeProcCall.GetOutputCtrlParamTuple("ResultColumn");


            ho_Line.Dispose();

            ptsProcCall = new HDevProcedureCall(ptsProc);
            ptsProcCall.SetInputCtrlParamTuple("Rows", hv_BeginRow);
            ptsProcCall.SetInputCtrlParamTuple("Cols", hv_BeginCol);
            ptsProcCall.SetInputCtrlParamTuple("ActiveNum", 2);
            ptsProcCall.Execute();
            // get output parameters from procedure call

            ho_Line = ptsProcCall.GetOutputIconicParamObject("Line");
            hv_BeginRow = ptsProcCall.GetOutputCtrlParamTuple("Row1");
            hv_BeginCol = ptsProcCall.GetOutputCtrlParamTuple("Col1");
            hv_EndRow = ptsProcCall.GetOutputCtrlParamTuple("Row2");
            hv_EndCol = ptsProcCall.GetOutputCtrlParamTuple("Col2");

            ho_Regions.Dispose();
            ho_Line.Dispose();

            //return;
        }


    }
}*/