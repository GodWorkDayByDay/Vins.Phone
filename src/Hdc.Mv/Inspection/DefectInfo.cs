using System.Runtime.InteropServices;

namespace Hdc.Mv.Inspection
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DefectInfo
    {
        public int Index { get; set; }
        public int TypeCode { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Size { get; set; }
        public double X_Real { get; set; }
        public double Y_Real { get; set; }
        public double Width_Real { get; set; }
        public double Height_Real { get; set; }
        public double Size_Real { get; set; }
    };
}