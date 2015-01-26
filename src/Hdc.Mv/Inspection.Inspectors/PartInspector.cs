using System.Collections.Generic;
using HalconDotNet;
using Hdc.Mv.Halcon;
using Hdc.Reflection;
using Hdc.Windows.Media.Imaging;

namespace Hdc.Mv.Inspection
{
    public class PartInspector : IPartInspector
    {
        private string _cacheImageDir = typeof (Mv.Ex).Assembly.GetAssemblyDirectoryPath() + "\\CacheImages";

        public IList<PartSearchingResult> SearchParts(HImage image, IList<PartSearchingDefinition> definitions)
        {
            IList<PartSearchingResult> results = new List<PartSearchingResult>();

            foreach (var def in definitions)
            {
                int offsetX = 0;
                int offsetY = 0;
                HRegion domain;

                var result = new PartSearchingResult()
                             {
                                 Definition = def.DeepClone(),
                             };

                var reducedImage = HDevelopExport.Singletone.ChangeDomainForRectangle(
                    image,
                    line: def.RoiLine,
                    hv_RoiWidthLen: def.RoiHalfWidth,
                    margin: 0.5);

                if (def.Domain_SaveCacheImageEnabled)
                    reducedImage.CropDomain()
                        .ToBitmapSource()
                        .SaveToJpeg(_cacheImageDir + "\\PartInspector_" + def.Name + "_1_Domain.jpg");

                domain = reducedImage.GetDomain();

                offsetX = domain.GetColumn1();
                offsetY = domain.GetRow1();
                var croppedImage = reducedImage.CropDomain();


                var region = def.PartExtractor.Process(croppedImage);

                var countObj = region.CountObj();
                if (countObj == 0)
                {
                }
                else
                {
                    var movedRegion = region.MoveRegion(offsetY, offsetX);

                    var rect2 = movedRegion.GetSmallestHRectangle2();
                    var line = rect2.GetRoiLineFromRectangle2Phi();
                    result.PartLine = line;
                    result.PartHalfWidth = rect2.Length2;
                }
                results.Add(result);
            }
            return results;
        }
    }
}