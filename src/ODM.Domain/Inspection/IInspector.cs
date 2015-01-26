using System;
using HalconDotNet;
using Hdc.Mv.Inspection;

namespace ODM.Domain.Inspection
{
    public interface IInspector : IDisposable
    {
        InspectionInfo Inspect(HImage imageInfo);
    }
}