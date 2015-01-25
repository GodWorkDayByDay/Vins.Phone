/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using HalconDotNet;
using Hdc.Diagnostics;
using Hdc.Mv.Halcon;
using Hdc.Windows.Media.Imaging;

namespace Hdc.Mv.Inspection
{
    public class HDevelopExportHelper : IDisposable
    {
        private readonly HImage _hImage;

        private readonly HDevelopExport HDevelopExport = new HDevelopExport();

        public HDevelopExportHelper(HImage hImage)
        {
            _hImage = hImage;
        }

        public HDevelopExportHelper(ImageInfo imageInfo)
            : this((HImage)imageInfo.To8BppHImage())
        {
        }

        public HImage HImage
        {
            get { return _hImage; }
        }



        public void Dispose()
        {
            if (_hImage != null) _hImage.Dispose();
        }
     

     
    }
}*/