//
//  File generated by HDevelop for HALCON/DOTNET (C#) Version 11.0
//

using HalconDotNet;

public partial class HDevelopExport
{
#if !(NO_EXPORT_MAIN || NO_EXPORT_APP_MAIN)
#endif

  // Procedures 
  public void EnhanceEdgeArea3 (HObject ho_InputImage, out HObject ho_EnhancedImage, 
      HTuple hv_EmpMaskWidth, HTuple hv_EmpMaskHeight, HTuple hv_EmpMaskFactor, HTuple hv_MeanMaskWidth, 
      HTuple hv_MeanMaskHeight, HTuple hv_IterationCount, HTuple hv_ScaleMult, HTuple hv_ScaleAdd)
  {



    // Stack for temporary objects 
    HObject[] OTemp = new HObject[20];

    // Local iconic variables 

    HObject ho_ImageScaled=null, ho_ImageOpening=null;


    // Local control variables 

    HTuple hv_Index = null;

    // Initialize local and output iconic variables 
    HOperatorSet.GenEmptyObj(out ho_EnhancedImage);
    HOperatorSet.GenEmptyObj(out ho_ImageScaled);
    HOperatorSet.GenEmptyObj(out ho_ImageOpening);


    ho_EnhancedImage.Dispose();
    HOperatorSet.MeanImage(ho_InputImage, out ho_EnhancedImage, 1, 1);


    HTuple end_val4 = hv_IterationCount;
    HTuple step_val4 = 1;
    for (hv_Index=1; hv_Index.Continue(end_val4, step_val4); hv_Index = hv_Index.TupleAdd(step_val4))
    {
      HOperatorSet.MeanImage(ho_EnhancedImage, out OTemp[0], hv_MeanMaskWidth, hv_MeanMaskHeight);
      ho_EnhancedImage.Dispose();
      ho_EnhancedImage = OTemp[0];
      ho_ImageScaled.Dispose();
      HOperatorSet.ScaleImage(ho_EnhancedImage, out ho_ImageScaled, hv_ScaleMult, 
          hv_ScaleAdd);
      ho_EnhancedImage.Dispose();
      HOperatorSet.Emphasize(ho_ImageScaled, out ho_EnhancedImage, hv_EmpMaskWidth, 
          hv_EmpMaskHeight, hv_EmpMaskFactor);

    }

    if ((int)(new HTuple(hv_MeanMaskWidth.TupleGreater(hv_MeanMaskHeight))) != 0)
    {
      ho_ImageOpening.Dispose();
      HOperatorSet.GrayOpeningRect(ho_EnhancedImage, out ho_ImageOpening, 1, hv_MeanMaskWidth*2);
      ho_EnhancedImage.Dispose();
      HOperatorSet.GrayClosingRect(ho_ImageOpening, out ho_EnhancedImage, 1, hv_MeanMaskWidth*2);
    }
    else
    {
      ho_ImageOpening.Dispose();
      HOperatorSet.GrayOpeningRect(ho_EnhancedImage, out ho_ImageOpening, hv_MeanMaskHeight*2, 
          1);
      ho_EnhancedImage.Dispose();
      HOperatorSet.GrayClosingRect(ho_ImageOpening, out ho_EnhancedImage, hv_MeanMaskHeight*2, 
          1);
    }



    //mean_image (ImageEmp, EnhancedImage, MeanMaskWidth, MeanMaskHeight)

    ho_ImageScaled.Dispose();
    ho_ImageOpening.Dispose();

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

