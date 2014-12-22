using System.Runtime.InteropServices;

namespace Hdc.Mv.Inspection
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InspectInfo
    {
        public int Index;

        public int SurfaceTypeIndex;

        public int HasError;

        public int DefectsCount;

        public int MeasurementsCount;
    };
}