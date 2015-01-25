using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    [ContentProperty("Items")]
    public class CompositeFilter: Collection<IImageFilter>, IImageFilter
    {
        public HImage Process(HImage image)
        {
            var processImage = image;

            int index = 0;
            foreach (var imageFilter in Items)
            {
                processImage = imageFilter.Process(processImage);
//                processImage.WriteImage("tiff", 0, @"B:\Test_Composite_" + index);
                index++;
            }

            return processImage;
        }
    }
}