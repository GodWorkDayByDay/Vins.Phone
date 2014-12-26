//
//  File generated by HDevelop for HALCON/DOTNET (C#) Version 11.0
//

using HalconDotNet;

public partial class HDevelopExport
{
#if !(NO_EXPORT_MAIN || NO_EXPORT_APP_MAIN)
#endif

  // Procedures 
  // Chapter: File
  // Short Description: Get all image files under the given path 
  public void list_image_files (HTuple hv_ImageDirectory, HTuple hv_Extensions, HTuple hv_Options, 
      out HTuple hv_ImageFiles)
  {


    // Local control variables 

    HTuple hv_HalconImages = null, hv_OS = null;
    HTuple hv_Directories = null, hv_Index = null, hv_FileExists = new HTuple();
    HTuple hv_AllFiles = new HTuple(), hv_i = new HTuple();
    HTuple hv_Selection = new HTuple();

    HTuple   hv_Extensions_COPY_INP_TMP = hv_Extensions.Clone();
    HTuple   hv_ImageDirectory_COPY_INP_TMP = hv_ImageDirectory.Clone();

    // Initialize local and output iconic variables 

    //This procedure returns all files in a given directory
    //with one of the suffixes specified in Extensions.
    //
    //input parameters:
    //ImageDirectory: as the name says
    //   If a tuple of directories is given, only the images in the first
    //   existing directory are returned.
    //   If a local directory is not found, the directory is searched
    //   under %HALCONIMAGES%/ImageDirectory. If %HALCONIMAGES% is not set,
    //   %HALCONROOT%/images is used instead.
    //Extensions: A string tuple containing the extensions to be found
    //   e.g. ['png','tif',jpg'] or others
    //If Extensions is set to 'default' or the empty string '',
    //   all image suffixes supported by HALCON are used.
    //Options: as in the operator list_files, except that the 'files'
    //   option is always used. Note that the 'directories' option
    //   has no effect but increases runtime, because only files are
    //   returned.
    //
    //output parameter:
    //ImageFiles: A tuple of all found image file names
    //
    if ((int)((new HTuple((new HTuple(hv_Extensions_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
        new HTuple(hv_Extensions_COPY_INP_TMP.TupleEqual(""))))).TupleOr(new HTuple(hv_Extensions_COPY_INP_TMP.TupleEqual(
        "default")))) != 0)
    {
      hv_Extensions_COPY_INP_TMP = new HTuple();
      hv_Extensions_COPY_INP_TMP[0] = "ima";
      hv_Extensions_COPY_INP_TMP[1] = "tif";
      hv_Extensions_COPY_INP_TMP[2] = "tiff";
      hv_Extensions_COPY_INP_TMP[3] = "gif";
      hv_Extensions_COPY_INP_TMP[4] = "bmp";
      hv_Extensions_COPY_INP_TMP[5] = "jpg";
      hv_Extensions_COPY_INP_TMP[6] = "jpeg";
      hv_Extensions_COPY_INP_TMP[7] = "jp2";
      hv_Extensions_COPY_INP_TMP[8] = "jxr";
      hv_Extensions_COPY_INP_TMP[9] = "png";
      hv_Extensions_COPY_INP_TMP[10] = "pcx";
      hv_Extensions_COPY_INP_TMP[11] = "ras";
      hv_Extensions_COPY_INP_TMP[12] = "xwd";
      hv_Extensions_COPY_INP_TMP[13] = "pbm";
      hv_Extensions_COPY_INP_TMP[14] = "pnm";
      hv_Extensions_COPY_INP_TMP[15] = "pgm";
      hv_Extensions_COPY_INP_TMP[16] = "ppm";

    }
    if ((int)(new HTuple(hv_ImageDirectory_COPY_INP_TMP.TupleEqual(""))) != 0)
    {
      hv_ImageDirectory_COPY_INP_TMP = ".";
    }
    HOperatorSet.GetSystem("image_dir", out hv_HalconImages);
    HOperatorSet.GetSystem("operating_system", out hv_OS);
    if ((int)(new HTuple(((hv_OS.TupleSubstr(0,2))).TupleEqual("Win"))) != 0)
    {
      hv_HalconImages = hv_HalconImages.TupleSplit(";");
    }
    else
    {
      hv_HalconImages = hv_HalconImages.TupleSplit(":");
    }
    hv_Directories = hv_ImageDirectory_COPY_INP_TMP.Clone();
    for (hv_Index=0; (int)hv_Index<=(int)((new HTuple(hv_HalconImages.TupleLength()
        ))-1); hv_Index = (int)hv_Index + 1)
    {
      hv_Directories = hv_Directories.TupleConcat(((hv_HalconImages.TupleSelect(hv_Index))+"/")+hv_ImageDirectory_COPY_INP_TMP);
    }
    hv_ImageFiles = new HTuple();
    for (hv_Index=0; (int)hv_Index<=(int)((new HTuple(hv_Directories.TupleLength()
        ))-1); hv_Index = (int)hv_Index + 1)
    {
      HOperatorSet.FileExists(hv_Directories.TupleSelect(hv_Index), out hv_FileExists);
      if ((int)(hv_FileExists) != 0)
      {
        HOperatorSet.ListFiles(hv_Directories.TupleSelect(hv_Index), (new HTuple("files")).TupleConcat(
            hv_Options), out hv_AllFiles);
        hv_ImageFiles = new HTuple();
        for (hv_i=0; (int)hv_i<=(int)((new HTuple(hv_Extensions_COPY_INP_TMP.TupleLength()
            ))-1); hv_i = (int)hv_i + 1)
        {
          HOperatorSet.TupleRegexpSelect(hv_AllFiles, (((".*"+(hv_Extensions_COPY_INP_TMP.TupleSelect(
              hv_i)))+"$")).TupleConcat("ignore_case"), out hv_Selection);
          hv_ImageFiles = hv_ImageFiles.TupleConcat(hv_Selection);
        }
        HOperatorSet.TupleRegexpReplace(hv_ImageFiles, (new HTuple("\\\\")).TupleConcat(
            "replace_all"), "/", out hv_ImageFiles);
        HOperatorSet.TupleRegexpReplace(hv_ImageFiles, (new HTuple("//")).TupleConcat(
            "replace_all"), "/", out hv_ImageFiles);

        return;
      }
    }

    return;
  }

  public void GetCalibrationParameters (HTuple hv_DescrFile, HTuple hv_ImageDirectory, 
      HTuple hv_Focus, HTuple hv_Sx, HTuple hv_Sy, HTuple hv_Width, HTuple hv_Height, 
      HTuple hv_CameraType, out HTuple hv_interCamera, out HTuple hv_PoseNewOrigin, 
      out HTuple hv_distance)
  {


    // Local iconic variables 

    HObject ho_Image=null, ho_CaltabRegion=null;


    // Local control variables 

    HTuple hv_CalibDataID = null, hv_startParam = new HTuple();
    HTuple hv_ImageFiles = null, hv_n = null, hv_RCoord = new HTuple();
    HTuple hv_CCoord = new HTuple(), hv_StartPose = new HTuple();
    HTuple hv_Error = null, hv_Pose = null, hv_cut = null;
    HTuple hv_YX = null, hv_YY = null, hv_X = null, hv_Y = null;
    HTuple hv_X1 = null, hv_Y1 = null, hv_X2 = null, hv_Y2 = null;
    HTuple hv_Distance1 = null, hv_Distance2 = null, hv_OffsetX = null;
    HTuple hv_OffsetY = null, hv_PoseR = null;

    // Initialize local and output iconic variables 
    HOperatorSet.GenEmptyObj(out ho_Image);
    HOperatorSet.GenEmptyObj(out ho_CaltabRegion);

    HOperatorSet.CreateCalibData("calibration_object", 1, 1, out hv_CalibDataID);

    if ((int)((new HTuple(hv_Width.TupleEqual("area_scan_telecentric_division"))).TupleOr(
        new HTuple(hv_Width.TupleEqual("area_scan_telecentric_polynomial")))) != 0)
    {

      hv_startParam = new HTuple();
      hv_startParam[0] = 0;
      hv_startParam = hv_startParam.TupleConcat(hv_Sx);
      hv_startParam = hv_startParam.TupleConcat(hv_Sy);
      hv_startParam = hv_startParam.TupleConcat(hv_Width/2);
      hv_startParam = hv_startParam.TupleConcat(hv_Height/2);
      hv_startParam = hv_startParam.TupleConcat(hv_Width);
      hv_startParam = hv_startParam.TupleConcat(hv_Height);

      HOperatorSet.SetCalibDataCamParam(hv_CalibDataID, 0, hv_CameraType, hv_startParam);

    }
    else
    {

      hv_startParam = new HTuple();
      hv_startParam = hv_startParam.TupleConcat(hv_Focus);
      hv_startParam = hv_startParam.TupleConcat(0);
      hv_startParam = hv_startParam.TupleConcat(hv_Sx);
      hv_startParam = hv_startParam.TupleConcat(hv_Sy);
      hv_startParam = hv_startParam.TupleConcat(hv_Width/2);
      hv_startParam = hv_startParam.TupleConcat(hv_Height/2);
      hv_startParam = hv_startParam.TupleConcat(hv_Width);
      hv_startParam = hv_startParam.TupleConcat(hv_Height);

      HOperatorSet.SetCalibDataCamParam(hv_CalibDataID, 0, hv_CameraType, hv_startParam);

    }

    HOperatorSet.SetCalibDataCalibObject(hv_CalibDataID, 0, hv_DescrFile);

    list_image_files(hv_ImageDirectory, "default", new HTuple(), out hv_ImageFiles);


    for (hv_n=0; (int)hv_n<=(int)((new HTuple(hv_ImageFiles.TupleLength()))-1); hv_n = (int)hv_n + 1)
    {

      ho_Image.Dispose();
      HOperatorSet.ReadImage(out ho_Image, hv_ImageFiles.TupleSelect(hv_n));

      ho_CaltabRegion.Dispose();
      HOperatorSet.FindCaltab(ho_Image, out ho_CaltabRegion, hv_DescrFile, 3, 112, 
          5);

      HOperatorSet.FindMarksAndPose(ho_Image, ho_CaltabRegion, hv_DescrFile, hv_startParam, 
          128, 10, 18, 0.9, 15, 100, out hv_RCoord, out hv_CCoord, out hv_StartPose);

      HOperatorSet.SetCalibDataObservPoints(hv_CalibDataID, 0, 0, 1, hv_RCoord, hv_CCoord, 
          "all", hv_StartPose);

    }


    HOperatorSet.CalibrateCameras(hv_CalibDataID, out hv_Error);

    HOperatorSet.GetCalibData(hv_CalibDataID, "camera", 0, "params", out hv_interCamera);

    HOperatorSet.GetCalibData(hv_CalibDataID, "calib_obj_pose", (new HTuple(0)).TupleConcat(
        1), "pose", out hv_Pose);

    hv_cut = ((hv_Pose.TupleSelect(5))).TupleInt();

    hv_YX = 100;
    hv_YY = 100;

    HOperatorSet.ImagePointsToWorldPlane(hv_interCamera, hv_Pose, hv_YY, hv_YX, 1, 
        out hv_X, out hv_Y);

    HOperatorSet.ImagePointsToWorldPlane(hv_interCamera, hv_Pose, hv_YY+1, hv_YX, 
        1, out hv_X1, out hv_Y1);

    HOperatorSet.ImagePointsToWorldPlane(hv_interCamera, hv_Pose, hv_YY, hv_YX+1, 
        1, out hv_X2, out hv_Y2);

    HOperatorSet.DistancePp(hv_Y, hv_X, hv_Y1, hv_X1, out hv_Distance1);

    HOperatorSet.DistancePp(hv_Y, hv_X, hv_Y2, hv_X2, out hv_Distance2);

    hv_distance = (hv_Distance1+hv_Distance2)/2;

    hv_OffsetX = (hv_CCoord.TupleSelect(24))*hv_distance;
    hv_OffsetY = (hv_RCoord.TupleSelect(24))*hv_distance;

    hv_PoseR = hv_Pose.Clone();
    if (hv_PoseR == null)
      hv_PoseR = new HTuple();
    hv_PoseR[5] = (hv_Pose.TupleSelect(5))-hv_cut;

    HOperatorSet.SetOriginPose(hv_PoseR, -hv_OffsetX, -hv_OffsetY, 0, out hv_PoseNewOrigin);

    ho_Image.Dispose();
    ho_CaltabRegion.Dispose();

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
