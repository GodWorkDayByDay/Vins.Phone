using System.Runtime.InteropServices;

namespace Hdc.Mv.Inspection
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MeasurementInfo
    {
        public int Index { get; set; }
        public int TypeCode { get; set; }
        public double StartPointX { get; set; }
        public double StartPointY { get; set; }
        public double EndPointX { get; set; }
        public double EndPointY { get; set; }
        public double Value { get; set; }
        public int GroupIndex { get; set; }
        public double StartPointXActualValue { get; set; }
        public double StartPointYActualValue { get; set; }
        public double EndPointXActualValue { get; set; }
        public double EndPointYActualValue { get; set; }
        public double ValueActualValue { get; set; }
        public bool HasError { get; set; }
        public string TypeName { get; set; }
        public string TypeDescription { get; set; }
        public string GroupName { get; set; }
    };
}