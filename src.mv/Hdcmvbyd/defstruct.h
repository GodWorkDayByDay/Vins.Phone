#pragma once

typedef int MeasHandle;
typedef unsigned long	MEAS_ID;

typedef enum _G_DLL_PARA_TYPE
{
	G_MEAS_NUMBER	=	0,
	G_MEAS_POLARITY,
	G_MEAS_ORIENTATION,
	G_MEAS_FILTERTYPE,
	G_MEAS_EDGETHRESHOLD,
	G_MEAS_FILTERSMOOTHNESS,
	G_MEAS_BOXANGLEMODE,
	G_MEAS_CONTRAST,
	G_MEAS_EDGESTRENGTH,
	G_MEAS_BOXANGLE,
	G_MEAS_MARKEROPPOSITE,
}MEAS_PARA;
typedef enum _G_FILTER_TYPE
{
	G_FILTER_EULER	=	0,
	G_FILTER_PREWITT,
	G_FILTER_SHEN,
}FILTER_TYPE;
typedef enum _G_POLARITY_TYPE
{
	G_POLARITY_NEGATIVE	=	0,
	G_POLARITY_POSITIVE,
	G_POLARITY_ANY,
};
typedef enum _G_ORIENTATION_TYPE
{
	G_ORIENTATION_HORIZONTAL	=	0,
	G_ORIENTATION_VERTICAL,
	G_ORIENTATION_ANY,
};
typedef enum _G_ABLE_TYPE
{
	G_DISABLE	=	0,
	G_ENABLE,
	G_ANY,
};

typedef struct tagMeasRect
{
	double nStPointX;//start point
	double nStPointY;
	double nEdPointX;//end point
	double nEdPointY;
	double nLineWidth;//width of connected line 
}MeasRect;

typedef struct tagSearchRect
{
	int  nOffSetX;
	int  nOffSetY;
	int  nWidth;
	int  nHeight;
}SearchRect;

typedef struct tagMeasResult
{
	double st_fPosX;//start point
	double st_fPosY;
	double ed_fPosX;//end point
	double ed_fPosY;
	double pp_fPosX;//perpendicular point
	double pp_fPosY;
	double fDistanceofPixel;//pixel distance
	double fDistanceofReal;//real world distance
	double fLineDegree;//degree
}MeasResult;
/*replace WINDOWS Size*/
typedef struct tagImSize
{
	long cx;
	long cy;
}ImSize;

/*inherited struct definition begin*/
struct ImageInfo
{
	int Index;
	int SurfaceTypeIndex;
	int Width;
	int Height;
	int BitsPerPixel;
	unsigned char* Buffer;
};

struct InspectInfo
{
	int Index;
	int SurfaceTypeIndex;
	int HasError;
	int DefectsCount;
	int MeasurementsCount;
};

struct DefectInfo
{
	int Index;
	int TypeCode;
	double X;
	double Y;
	double Width;
	double Height;
	double Size;
	double X_Real;
	double Y_Real;
	double Width_Real;
	double Height_Real;
	double Size_Real;
};

struct MeasurementInfo
{
	int Index;
	int TypeCode;
	double StartPointX;
	double StartPointY;
	double EndPointX;
	double EndPointY;
	double Value;
	int GroupIndex;
	double StartPointX_Real;
	double StartPointY_Real;
	double EndPointX_Real;
	double EndPointY_Real;
	double Value_Real;
};
/*inherited struct definition end*/

/**************************************/
typedef struct tagCalibrationPoint
{
	double x;
	double y;
}CalPoint;

typedef struct tagCalibrationStruct
{
	CalPoint src1;
	CalPoint src2;
	CalPoint src3;
	CalPoint src4;
	CalPoint dst1;
	CalPoint dst2;
	CalPoint dst3;
	CalPoint dst4;
}CalStruct;

///////////////////////used to define new interface
typedef struct tagMeasurement
{
	MeasRect rect_st;
	MeasRect rect_ed;
	int		 polarity_st;
	int      polarity_ed;
	int      orientation;
}Measurement;

///////////////////////////////////////////
typedef struct tagPoint
{
	double X;
	double Y;
}Point;

typedef struct tagLine
{
	double X1;
	double X2;
	double Y1;
	double Y2;
}Line;

typedef struct tagCircle
{
	double CenterX;
	double CenterY;
	double Radius;
}Circle;

////////////////////////////////////////////////////////////////////////
typedef struct taghoughline
{
	double line_x; //degree
	double line_y; //slope
	int  lineCount;
}Houghlineobj;

typedef struct taghoughcircle
{
	double center_x;
	double center_y;
	double circle_rad;
	int     circleCount;
}Houghcircleobj;

typedef struct taghoughSize
{
	int off_x;
	int off_y;
	int width;
	int height;
}HoughSize;