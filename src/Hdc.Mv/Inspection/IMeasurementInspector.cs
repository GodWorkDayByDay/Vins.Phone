namespace Hdc.Mv.Inspection
{
    public interface IMeasurementInspector
    {
        /// <summary>
        /// 提取边界线
        /// </summary>
        /// <param name="directionLine">扫描方向线</param>
        /// <param name="roiWidth">ROI宽度，数值为单边的宽度，单位为像素</param>
        /// <param name="threshold">阈值</param>
        /// <param name="edgeLine">边界线</param>
        /// <returns>是否正常</returns>
        bool ExtractEdgeLine(Line directionLine, double roiWidth, double threshold, out Line edgeLine);


    }
}