using System.Windows;
using HalconDotNet;

namespace Hdc.Mv.Halcon
{
    public static class HOperatorSetExtensions
    {
        public static Point IntersectionWith(this Line line1, Line line2)
        {
            if (line1.GetCenterPoint() == new Point())
                return new Point();

            if (line2.GetCenterPoint() == new Point())
                return new Point();

            HTuple pX, pY, pp;
            HOperatorSet.IntersectionLines(line1.Y1, line1.X1, line1.Y2, line1.X2,
                line2.Y1, line2.X1, line2.Y2, line2.X2,
                out pY, out pX, out pp);

            if (pp != 0)
            {
//                throw new HalconInspectorException();
                return new Point();
            }

            var p = new Point(pX, pY);
            return p;
        } 
    }
}