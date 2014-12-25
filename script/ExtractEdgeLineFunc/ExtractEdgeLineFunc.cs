//
//  File generated by HDevelop for HALCON/DOTNET (C#) Version 11.0
//

using HalconDotNet;

public partial class HDevelopExport
{
#if !(NO_EXPORT_MAIN || NO_EXPORT_APP_MAIN)
#endif

  // Procedures 
  public void ExtractEdgeLine (HObject ho_InputImage, HTuple hv_LineStartPoint_Row, 
      HTuple hv_LineStartPoint_Column, HTuple hv_LineEndPoint_Row, HTuple hv_LineEndPoint_Column, 
      HTuple hv_RoiWidthLen, HTuple hv_LowThreshold, HTuple hv_HighThreshold, HTuple hv_FilterAlpha, 
      out HTuple hv_EdgeLineStartPoint_Row, out HTuple hv_EdgeLineStartPoint_Column, 
      out HTuple hv_EdgeLineEndPoint_Row, out HTuple hv_EdgeLineEndPoint_Column)
  {



    // Local iconic variables 

    HObject ho_Rectangle, ho_ImageReduced, ho_Edges;
    HObject ho_UnionContours, ho_UnionContours1, ho_LineSegments;
    HObject ho_firstline;


    // Local control variables 

    HTuple hv_TmpCtrl_Row = null, hv_TmpCtrl_Column = null;
    HTuple hv_TmpCtrl_Dr = null, hv_TmpCtrl_Dc = null, hv_TmpCtrl_Phi = null;
    HTuple hv_TmpCtrl_Len1 = null, hv_TmpCtrl_Len2 = null;
    HTuple hv_Nr = null, hv_Nc = null, hv_Dist = null;

    // Initialize local and output iconic variables 
    HOperatorSet.GenEmptyObj(out ho_Rectangle);
    HOperatorSet.GenEmptyObj(out ho_ImageReduced);
    HOperatorSet.GenEmptyObj(out ho_Edges);
    HOperatorSet.GenEmptyObj(out ho_UnionContours);
    HOperatorSet.GenEmptyObj(out ho_UnionContours1);
    HOperatorSet.GenEmptyObj(out ho_LineSegments);
    HOperatorSet.GenEmptyObj(out ho_firstline);

    //
    //init
    //FilterAlpha := 6
    //Measure 01: Convert coordinates to rectangle2 type
    hv_TmpCtrl_Row = 0.5*(hv_LineStartPoint_Row+hv_LineEndPoint_Row);
    hv_TmpCtrl_Column = 0.5*(hv_LineStartPoint_Column+hv_LineEndPoint_Column);
    hv_TmpCtrl_Dr = hv_LineStartPoint_Row-hv_LineEndPoint_Row;
    hv_TmpCtrl_Dc = hv_LineEndPoint_Column-hv_LineStartPoint_Column;
    hv_TmpCtrl_Phi = hv_TmpCtrl_Dr.TupleAtan2(hv_TmpCtrl_Dc);
    hv_TmpCtrl_Len1 = 0.5*((((hv_TmpCtrl_Dr*hv_TmpCtrl_Dr)+(hv_TmpCtrl_Dc*hv_TmpCtrl_Dc))).TupleSqrt()
        );
    hv_TmpCtrl_Len2 = hv_RoiWidthLen.Clone();
    ho_Rectangle.Dispose();
    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_TmpCtrl_Row, hv_TmpCtrl_Column, 
        hv_TmpCtrl_Phi, hv_TmpCtrl_Len1, hv_TmpCtrl_Len2);
    ho_ImageReduced.Dispose();
    HOperatorSet.ReduceDomain(ho_InputImage, ho_Rectangle, out ho_ImageReduced);
    ho_Edges.Dispose();
    HOperatorSet.EdgesSubPix(ho_ImageReduced, out ho_Edges, "canny", hv_FilterAlpha, 
        hv_LowThreshold, hv_HighThreshold);
    if (HDevWindowStack.IsOpen())
    {
      //dev_clear_window ()
    }
    if (HDevWindowStack.IsOpen())
    {
      //dev_display (ImageReduced)
    }
    //????
    ho_UnionContours.Dispose();
    HOperatorSet.UnionCollinearContoursXld(ho_Edges, out ho_UnionContours, 33, 1, 
        2, 0.1, "attr_keep");
    ho_UnionContours1.Dispose();
    HOperatorSet.UnionAdjacentContoursXld(ho_UnionContours, out ho_UnionContours1, 
        33, 8, "attr_keep");
    ho_LineSegments.Dispose();
    HOperatorSet.SegmentContoursXld(ho_UnionContours1, out ho_LineSegments, "lines", 
        9, 22, 22);
    //???????
    ho_firstline.Dispose();
    HOperatorSet.SelectContoursXld(ho_LineSegments, out ho_firstline, "contour_length", 
        hv_RoiWidthLen/2, 99999, hv_RoiWidthLen/2, 99999);
    //???????
    HOperatorSet.FitLineContourXld(ho_firstline, "tukey", -1, 1, 5, 3, out hv_EdgeLineStartPoint_Row, 
        out hv_EdgeLineStartPoint_Column, out hv_EdgeLineEndPoint_Row, out hv_EdgeLineEndPoint_Column, 
        out hv_Nr, out hv_Nc, out hv_Dist);
    ho_firstline.Dispose();
    HOperatorSet.GenRegionLine(out ho_firstline, hv_EdgeLineStartPoint_Row, hv_EdgeLineStartPoint_Column, 
        hv_EdgeLineEndPoint_Row, hv_EdgeLineEndPoint_Column);
    //
    ho_Rectangle.Dispose();
    ho_ImageReduced.Dispose();
    ho_Edges.Dispose();
    ho_UnionContours.Dispose();
    ho_UnionContours1.Dispose();
    ho_LineSegments.Dispose();
    ho_firstline.Dispose();

    return;
  }


}
#if !(NO_EXPORT_MAIN || NO_EXPORT_APP_MAIN)
public class HDevelopExportApp
{
  static void Main(string[] args)
  {
    new HDevelopExport();
  }
}
#endif

