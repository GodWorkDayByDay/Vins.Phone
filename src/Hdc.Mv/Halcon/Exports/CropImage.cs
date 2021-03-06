//
//  File generated by HDevelop for HALCON/DOTNET (C#) Version 11.0
//

using HalconDotNet;

public partial class HDevelopExport
{
#if !(NO_EXPORT_MAIN || NO_EXPORT_APP_MAIN)
#endif

  // Procedures 
  public void CropImage (HObject ho_OriginalImage, out HObject ho_CroppedImage, HTuple hv_X, 
      HTuple hv_Y, HTuple hv_Width, HTuple hv_Height)
  {



    // Local iconic variables 

    HObject ho_Rectangle, ho_ImagePart=null;


    // Local control variables 

    HTuple hv_ImageWidth = null, hv_ImageHeight = null;
    HTuple hv_FinalPointRow = null, hv_FinalPointColumn = null;

    // Initialize local and output iconic variables 
    HOperatorSet.GenEmptyObj(out ho_CroppedImage);
    HOperatorSet.GenEmptyObj(out ho_Rectangle);
    HOperatorSet.GenEmptyObj(out ho_ImagePart);

    HOperatorSet.GetImageSize(ho_OriginalImage, out hv_ImageWidth, out hv_ImageHeight);

    hv_FinalPointRow = hv_Y+hv_Height;
    hv_FinalPointColumn = hv_X+hv_Width;

    ho_Rectangle.Dispose();
    HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Y, hv_X, hv_FinalPointRow, hv_FinalPointColumn);


    if ((int)((new HTuple(hv_FinalPointRow.TupleLess(hv_ImageWidth))).TupleAnd(new HTuple(hv_FinalPointColumn.TupleLess(
        hv_ImageHeight)))) != 0)
    {

      ho_CroppedImage.Dispose();
      HOperatorSet.ReduceDomain(ho_OriginalImage, ho_Rectangle, out ho_CroppedImage
          );
      ho_ImagePart.Dispose();
      HOperatorSet.CropDomain(ho_CroppedImage, out ho_ImagePart);

    }
    else
    {



    }


    ho_Rectangle.Dispose();
    ho_ImagePart.Dispose();

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

