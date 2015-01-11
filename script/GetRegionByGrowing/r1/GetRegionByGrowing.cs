//
//  File generated by HDevelop for HALCON/DOTNET (C#) Version 12.0
//

using HalconDotNet;

public partial class HDevelopExport
{
#if !(NO_EXPORT_MAIN || NO_EXPORT_APP_MAIN)
#endif

  // Procedures 
  public void GetRegionByGrowing (HObject ho_Image, HObject ho_Domain, out HObject ho_FoundRegion, 
      HTuple hv_MedianRadius, HTuple hv_GrowingRow, HTuple hv_GrowingColumn, HTuple hv_GrowingTolerance, 
      HTuple hv_GrowingMinSize, HTuple hv_ClosingCircleRadius, HTuple hv_ClosingRectangleWidth, 
      HTuple hv_ClosingRectangleHeight, HTuple hv_AreaMin, HTuple hv_AreaMax, HTuple hv_RowMin, 
      HTuple hv_RowMax, HTuple hv_ColumnMin, HTuple hv_ColumnMax)
  {




    // Local iconic variables 

    HObject ho_MaskImage, ho_ImageMedian, ho_Regions;
    HObject ho_RegionFillUp, ho_RegionClosing, ho_RegionClosing2;
    HObject ho_RegionsByArea, ho_RegionsByRow, ho_RegionsByColumn;
    // Initialize local and output iconic variables 
    HOperatorSet.GenEmptyObj(out ho_FoundRegion);
    HOperatorSet.GenEmptyObj(out ho_MaskImage);
    HOperatorSet.GenEmptyObj(out ho_ImageMedian);
    HOperatorSet.GenEmptyObj(out ho_Regions);
    HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
    HOperatorSet.GenEmptyObj(out ho_RegionClosing);
    HOperatorSet.GenEmptyObj(out ho_RegionClosing2);
    HOperatorSet.GenEmptyObj(out ho_RegionsByArea);
    HOperatorSet.GenEmptyObj(out ho_RegionsByRow);
    HOperatorSet.GenEmptyObj(out ho_RegionsByColumn);
    ho_MaskImage.Dispose();
    HOperatorSet.ReduceDomain(ho_Image, ho_Domain, out ho_MaskImage);

    ho_ImageMedian.Dispose();
    HOperatorSet.MedianImage(ho_MaskImage, out ho_ImageMedian, "circle", hv_MedianRadius, 
        "continued");
    ho_Regions.Dispose();
    HOperatorSet.Regiongrowing(ho_ImageMedian, out ho_Regions, hv_GrowingRow, hv_GrowingColumn, 
        hv_GrowingTolerance, hv_GrowingMinSize);
    ho_RegionFillUp.Dispose();
    HOperatorSet.FillUp(ho_Regions, out ho_RegionFillUp);
    ho_RegionClosing.Dispose();
    HOperatorSet.ClosingCircle(ho_RegionFillUp, out ho_RegionClosing, hv_ClosingCircleRadius);
    ho_RegionClosing2.Dispose();
    HOperatorSet.ClosingRectangle1(ho_RegionClosing, out ho_RegionClosing2, hv_ClosingRectangleWidth, 
        hv_ClosingRectangleHeight);

    ho_RegionsByArea.Dispose();
    HOperatorSet.SelectShape(ho_RegionClosing2, out ho_RegionsByArea, "area", "and", 
        hv_AreaMin, hv_AreaMax);
    ho_RegionsByRow.Dispose();
    HOperatorSet.SelectShape(ho_RegionsByArea, out ho_RegionsByRow, "row", "and", 
        hv_RowMin, hv_RowMax);
    ho_RegionsByColumn.Dispose();
    HOperatorSet.SelectShape(ho_RegionsByRow, out ho_RegionsByColumn, "column", "and", 
        hv_ColumnMin, hv_ColumnMax);

    ho_FoundRegion.Dispose();
    HOperatorSet.MoveRegion(ho_RegionsByColumn, out ho_FoundRegion, 0, 0);

    ho_MaskImage.Dispose();
    ho_ImageMedian.Dispose();
    ho_Regions.Dispose();
    ho_RegionFillUp.Dispose();
    ho_RegionClosing.Dispose();
    ho_RegionClosing2.Dispose();
    ho_RegionsByArea.Dispose();
    ho_RegionsByRow.Dispose();
    ho_RegionsByColumn.Dispose();

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
