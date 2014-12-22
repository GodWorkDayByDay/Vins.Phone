using Hdc.Mvvm;
using ODM.Domain.Inspection;

namespace ODM.Presentation.ViewModels
{
    public class DefectInfoViewModel : ViewModel
    {
        private long _id;

        public long Id
        {
            get { return _id; }
            set
            {
                if (Equals(_id, value)) return;
                _id = value;
                RaisePropertyChanged(() => Id);
            }
        }

        private int _index;

        public int Index
        {
            get { return _index; }
            set
            {
                if (Equals(_index, value)) return;
                _index = value;
                RaisePropertyChanged(() => Index);
                RaisePropertyChanged(() => DisplayIndex);
            }
        }

        public int DisplayIndex
        {
            get
            {
                return Index + 1;
            }
        }

        private bool _isReject;

        public bool IsReject
        {
            get { return _isReject; }
            set
            {
                if (Equals(_isReject, value)) return;
                _isReject = value;
                RaisePropertyChanged(() => IsReject);
            }
        }

        private int _x;

        public int X
        {
            get { return _x; }
            set
            {
                if (Equals(_x, value)) return;
                _x = value;
                RaisePropertyChanged(() => X);
            }
        }


        private int _y;

        public int Y
        {
            get { return _y; }
            set
            {
                if (Equals(_y, value)) return;
                _y = value;
                RaisePropertyChanged(() => Y);
            }
        }

        private int _width;

        public int Width
        {
            get { return _width; }
            set
            {
                if (Equals(_width, value)) return;
                _width = value;
                RaisePropertyChanged(() => Width);
            }
        }

        private int _height;

        public int Height
        {
            get { return _height; }
            set
            {
                if (Equals(_height, value)) return;
                _height = value;
                RaisePropertyChanged(() => Height);
            }
        }

        private DefectType _type;

        public DefectType Type
        {
            get { return _type; }
            set
            {
                if (Equals(_type, value)) return;
                _type = value;
                RaisePropertyChanged(() => Type);
                RaisePropertyChanged(() => TypeDescription);
            }
        }

        private int _typeCode;

        public int TypeCode
        {
            get { return _typeCode; }
            set
            {
                if (Equals(_typeCode, value)) return;
                _typeCode = value;
                RaisePropertyChanged(() => TypeCode);
            }
        }

        private string _surfaceName;

        public string SurfaceName
        {
            get { return _surfaceName; }
            set
            {
                if (Equals(_surfaceName, value)) return;
                _surfaceName = value;
                RaisePropertyChanged(() => SurfaceName);
            }
        }

        public string TypeDescription
        {
            get
            {
                return Type.ToString().Replace(
                    "DefectType","DT-");
                switch (Type)
                {
                    case DefectType.Undefined:
                        return "未定义";
                    case DefectType.ForeignSpot:
                        return "异物";
                    case DefectType.DefectType04:
                        return "黑点";
                    case DefectType.DirtyPoint:
                        return "污点";
                    case DefectType.DefectType05:
                        return "黑斑";
                    case DefectType.DefectType02:
                        return "亮斑";
                    case DefectType.NonuniformSpot:
                        return "不匀斑块";
                    case DefectType.HorizontalNonuniform:
                        return "横向不匀";
                    case DefectType.VerticalNonuniform:
                        return "纵向不匀";

                    case DefectType.DefectType00:
                        return "亮边";
                    case DefectType.DefectType01:
                        return "亮点";
                    case DefectType.DefectType06:
                        return "亮块6";
                    case DefectType.DefectType07:
                        return "亮块7";
                    case DefectType.DefectType10:
                        return "亮块10";
                    case DefectType.DefectType12:
                        return "亮块12";
                    case DefectType.DefectType15:
                        return "亮块15";

                    case DefectType.DefectType08:
                        return "黑块8";
                    case DefectType.DefectType09:
                        return "黑块9";
                    case DefectType.DefectType11:
                        return "黑块11";
                    case DefectType.DefectType13:
                        return "黑块13";
                    case DefectType.DefectType16:
                        return "黑块16";

                    case DefectType.DefectType14:
                        return "蓝坑";

                    default:
                        return "未定义";
                }
            }
        }

        private int _size;

        public int Size
        {
            get { return _size; }
            set
            {
                if (Equals(_size, value)) return;
                _size = value;
                RaisePropertyChanged(() => Size);
            }
        }

        private int _surfaceTypeIndex;

        public int SurfaceTypeIndex
        {
            get { return _surfaceTypeIndex; }
            set
            {
                if (Equals(_surfaceTypeIndex, value)) return;
                _surfaceTypeIndex = value;
                RaisePropertyChanged(() => SurfaceTypeIndex);                
                RaisePropertyChanged(() => SurfaceTypeDisplayName);
            }
        }

        public string SurfaceTypeDisplayName
        {
            get
            {
                switch (SurfaceTypeIndex)
                {
                    case 0:
                        return "正面";
                    case 1:
                        return "背面";
                    default:
                        return "默认";
                }
            }
        }


        public string IndexDisplayValue
        {
            get
            {
                return SurfaceTypeDisplayName + "-" +
                    (Index + 1).ToString("D2");
            }
        }


        private double _xActualValue;

        public double XActualValue
        {
            get { return _xActualValue; }
            set
            {
                if (Equals(_xActualValue, value)) return;
                _xActualValue = value;
                RaisePropertyChanged(() => XActualValue);
            }
        }

        private double _yActualValue;

        public double YActualValue
        {
            get { return _yActualValue; }
            set
            {
                if (Equals(_yActualValue, value)) return;
                _yActualValue = value;
                RaisePropertyChanged(() => YActualValue);
            }
        }

        private double _widthActualValue;

        public double WidthActualValue
        {
            get { return _widthActualValue; }
            set
            {
                if (Equals(_widthActualValue, value)) return;
                _widthActualValue = value;
                RaisePropertyChanged(() => WidthActualValue);
            }
        }

        private double _heightActualValue;

        public double HeightActualValue
        {
            get { return _heightActualValue; }
            set
            {
                if (Equals(_heightActualValue, value)) return;
                _heightActualValue = value;
                RaisePropertyChanged(() => HeightActualValue);
            }
        }

        private double _sizeActualValue;

        public double SizeActualValue
        {
            get { return _sizeActualValue; }
            set
            {
                if (Equals(_sizeActualValue, value)) return;
                _sizeActualValue = value;
                RaisePropertyChanged(() => SizeActualValue);
            }
        }

//        public string WidthDisplayValue { get { return (Width / 100.0).ToString("F2"); } }
//        public string HeightDisplayValue { get { return (Height / 125.0).ToString("F2"); } }
//        public string XDisplayValue { get { return (X / 100.0).ToString("F2"); } }
//        public string YDisplayValue { get { return (Y / 125.0).ToString("F2"); } }


        public string WidthDisplayValue { get { return WidthActualValue.ToString("000.000"); } }
        public string HeightDisplayValue { get { return HeightActualValue.ToString("000.000"); } }
        public string XDisplayValue { get { return XActualValue.ToString("000.000"); } }
        public string YDisplayValue { get { return YActualValue.ToString("000.000"); } }
        public string SizeDisplayValue { get { return SizeActualValue.ToString("000.000"); } }
    }
}