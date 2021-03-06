//
//  File generated by HDevelop for HALCON/DOTNET (C#) Version 12.0
//

using HalconDotNet;

public partial class HDevelopExport
{
#if !(NO_EXPORT_MAIN || NO_EXPORT_APP_MAIN)
#endif

  // Procedures 
  public void GetRegionByBinaryThresholdRect (HObject ho_Image, out HObject ho_FoundRegion, 
      HTuple hv_MeanMaskWidth, HTuple hv_MeanMaskHeight, HTuple hv_ScaleAdd, HTuple hv_LightDark, 
      HTuple hv_AreaMin, HTuple hv_AreaMax, HTuple hv_OpeningWidth, HTuple hv_OpeningHeight, 
      HTuple hv_ClosingWidth, HTuple hv_ClosingHeight, HTuple hv_DilationRadius)
  {




    // Stack for temporary objects 
    HObject[] OTemp = new HObject[20];

    // Local iconic variables 

    HObject ho_Domain, ho_ImageMean, ho_ImageScaled=null;
    HObject ho_Regions, ho_ConnectedRegions, ho_SelectedRegions;
    HObject ho_RegionFillUp, ho_RegionUnion;

    // Local control variables 

    HTuple hv_Value = null, hv_UsedThreshold = null;
    // Initialize local and output iconic variables 
    HOperatorSet.GenEmptyObj(out ho_FoundRegion);
    HOperatorSet.GenEmptyObj(out ho_Domain);
    HOperatorSet.GenEmptyObj(out ho_ImageMean);
    HOperatorSet.GenEmptyObj(out ho_ImageScaled);
    HOperatorSet.GenEmptyObj(out ho_Regions);
    HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
    HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
    HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
    HOperatorSet.GenEmptyObj(out ho_RegionUnion);
    ho_Domain.Dispose();
    HOperatorSet.GetDomain(ho_Image, out ho_Domain);

    ho_ImageMean.Dispose();
    HOperatorSet.MeanImage(ho_Image, out ho_ImageMean, hv_MeanMaskWidth, hv_MeanMaskHeight);

    HOperatorSet.GrayFeatures(ho_Domain, ho_ImageMean, "mean", out hv_Value);

    if ((int)(new HTuple(hv_LightDark.TupleEqual("light"))) != 0)
    {
      ho_ImageScaled.Dispose();
      HOperatorSet.ScaleImage(ho_ImageMean, out ho_ImageScaled, 1, (255-hv_Value)+hv_ScaleAdd);
    }
    else if ((int)(new HTuple(hv_LightDark.TupleEqual("dark"))) != 0)
    {
      ho_ImageScaled.Dispose();
      HOperatorSet.ScaleImage(ho_ImageMean, out ho_ImageScaled, 1, (-hv_Value)+hv_ScaleAdd);
    }

    ho_Regions.Dispose();
    HOperatorSet.BinaryThreshold(ho_ImageScaled, out ho_Regions, "max_separability", 
        hv_LightDark, out hv_UsedThreshold);
    ho_ConnectedRegions.Dispose();
    HOperatorSet.Connection(ho_Regions, out ho_ConnectedRegions);
    ho_SelectedRegions.Dispose();
    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area", 
        "and", hv_AreaMin, hv_AreaMax);
    ho_RegionFillUp.Dispose();
    HOperatorSet.FillUp(ho_SelectedRegions, out ho_RegionFillUp);

    if ((int)((new HTuple(hv_OpeningWidth.TupleGreater(0))).TupleAnd(new HTuple(hv_OpeningHeight.TupleGreater(
        0)))) != 0)
    {
      {
      HObject ExpTmpOutVar_0;
      HOperatorSet.OpeningRectangle1(ho_RegionFillUp, out ExpTmpOutVar_0, hv_OpeningWidth, 
          hv_OpeningHeight);
      ho_RegionFillUp.Dispose();
      ho_RegionFillUp = ExpTmpOutVar_0;
      }
    }

    if ((int)((new HTuple(hv_ClosingWidth.TupleGreater(0))).TupleAnd(new HTuple(hv_ClosingHeight.TupleGreater(
        0)))) != 0)
    {
      {
      HObject ExpTmpOutVar_0;
      HOperatorSet.ClosingRectangle1(ho_RegionFillUp, out ExpTmpOutVar_0, hv_ClosingWidth, 
          hv_ClosingHeight);
      ho_RegionFillUp.Dispose();
      ho_RegionFillUp = ExpTmpOutVar_0;
      }
    }

    if ((int)(new HTuple(hv_DilationRadius.TupleGreater(0))) != 0)
    {
      {
      HObject ExpTmpOutVar_0;
      HOperatorSet.DilationCircle(ho_RegionFillUp, out ExpTmpOutVar_0, hv_DilationRadius);
      ho_RegionFillUp.Dispose();
      ho_RegionFillUp = ExpTmpOutVar_0;
      }
    }

    ho_RegionUnion.Dispose();
    HOperatorSet.Union1(ho_RegionFillUp, out ho_RegionUnion);
    ho_FoundRegion.Dispose();
    HOperatorSet.MoveRegion(ho_RegionUnion, out ho_FoundRegion, 0, 0);

    ho_Domain.Dispose();
    ho_ImageMean.Dispose();
    ho_ImageScaled.Dispose();
    ho_Regions.Dispose();
    ho_ConnectedRegions.Dispose();
    ho_SelectedRegions.Dispose();
    ho_RegionFillUp.Dispose();
    ho_RegionUnion.Dispose();

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

