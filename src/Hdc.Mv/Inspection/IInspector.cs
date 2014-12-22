using System;
using Hdc.Mv.Inspection;

namespace Hdc.Mv.Inspection
{
    public interface IInspector : IDisposable
    {
        bool Init(); // return true = successful, false = failed
        bool LoadParameters(); // return true = successful, false = failed
        //void FreeObject();

        InspectionInfo Inspect(ImageInfo imageInfo);
    }
}