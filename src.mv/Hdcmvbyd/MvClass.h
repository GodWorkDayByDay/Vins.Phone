#pragma once
#include "defstruct.h"
#include <mil.h>


class CircleClass;
class EdgeClass;
class DefectClass;

struct DefectPara //20141218 gtc
{
	int		m_HistLowThreshold;
	int		m_HistHighThreshold;
	int		m_HistOpenValue;
	int		m_HistCloseValue;
	int		m_SelectOpenValue;
	int		m_BinaryValue;
	int		m_BlobLeastArea;
	int		m_BlobLeastMeanValue;
	int		m_BlobLeastWidth;
	int		m_BlobLeastHeight;
	int		m_BlobMaxArea;
};

class AppClass
{
public:
	AppClass();
	virtual ~AppClass();
public:
	MIL_ID m_App;
	MIL_ID m_Sys;
	MIL_ID m_Calibration;
	MIL_ID m_OriginImage;
	MIL_ID m_CalImage;
	MIL_ID m_OriginChild;
	MIL_ID m_CalChild;
	long   m_ImageWidth;
	long   m_ImageHeight;
	long   m_ImageByteLine;
	unsigned char* m_pOrigin;
	unsigned char* m_pCal;
	char   m_ModelPath[MAX_PATH];
	BOOL   m_bGetModelPath;
	BOOL   m_bAllocBuffer;
	CircleClass* pCircleClass;
	int	   m_CircleClassNumber;
	int    m_CurCircleIndex;
	EdgeClass* pEdgeClass;
	int    m_EdgeClassNumber;
	int    m_CurEdgeIndex;
	DefectClass* pDefectClass;
	int    m_DefectClassNumber;
	int    m_CurDefectIndex;
	Point  m_ZeroPoint;
	double m_RelativeAngle;
	int    m_bDebugSaveBmp;
	int    m_CircleSizeRedundance;
	int    m_ShenFilterSmoothness;
	char  m_bExcuteBlobCalculate[MAX_PATH];
	DefectPara m_DefectInitPara;//20141218 gtc
public:
	int GetDllPath();
	int LoadParameter();
	int LoadCalibration();
	int InitMil(int width, int height);
	int UinitMil();
	int CalibrateOriginImage(ImageInfo iSour, ImageInfo* iCalImage);
	int ToRelativeCoord(Line baseline, double angle);
	int InitDefinitions();
	int FreeDefinitions();
	Point FromRelativeToOrigin(Point pt);
	Point FromOriginToRelative(Point pt);
	int GetDistanceBetweenLines(Line line1,Line line2,double* distanceInPixel,double* distanceInWorld,double* angle);
	int GetDistanceBetweenPoints(Point point1,Point point2,double* distanceInPixel,double* distanceInWorld,double* angle);
	int GetDistancePointToLine(Point point,Line  line,double* distanceInPixel,double* distanceInWorld,Point* pt);
	void SaveCalBmp();
};



class CircleClass
{
public:
	CircleClass();
	virtual ~CircleClass();
public:
	int m_Center_x;
	int m_Center_y;
	int m_OuterRad;
	int m_InnerRad;
	int m_LowThreshold;
	int m_HighThreshold;
	double m_fPosDif;
	double m_fRadDif;
	double m_fRatio;
	int m_ImageWidth;
	int m_ImageHeight;
	unsigned char* pSour;
	Houghcircleobj obj;
	AppClass* pApp;
	int m_processtype;
	int m_bExcuteBlobAnalysis;
public:
	int InitCircle(AppClass* pA, int width, int height, unsigned char* ptrSour, double Center_x, double Center_y, double InnRad, double OutRad, int LowThre, int HighTre, double PosDif, double RadDif, double ratio, int processtype);
	int ModifyCircle(double Center_x, double Center_y, double InnRad, double OutRad, int LowThre, int HighTre, double PosDif, double RadDif, double ratio);
	int FindCircle(int iCircleIndex);
	int UinitCircle();
	int GetContour(unsigned char* pSr, int offx, int offy, int nWd, int nHt);
};

class EdgeClass
{
public:
	EdgeClass();
	virtual ~EdgeClass();
public:
	AppClass* m_pApp;
	double m_startPointX;
	double m_startPointY;
	double m_endPointX;
	double m_endPointY;
	double m_roiWidth;
	int m_polarity;
	int m_orientation;
	SearchRect m_rect1;
	SearchRect m_rect2;
	MIL_ID m_Mark1;
	MIL_ID m_Mark2;
	Point m_point1;
	Point m_point2;
	Point m_InterPoint;
public:
	int InitEdge(AppClass* pA,double startPointX,double startPointY,double endPointX,double endPointY,double roiWidth,int polarity,int orientation);
	int ModifyEdge();
	int FindEdge(int iEdgeIndex);
	int UinitEdge();
	void CreateBox(double startPointX,double startPointY,double endPointX,double endPointY,double roiWidth);
};

class DefectClass//20141218 gtc
{
public:
	DefectClass();
	virtual ~DefectClass();
public:
	AppClass* m_pApp;
	MIL_ID m_MilBlob;
	MIL_ID m_MilBlobResult;
	MIL_ID m_MilImage;
	MIL_ID m_MilChildImage;
	int    m_Width;
	int    m_Height;
	int    m_Byteline;
	unsigned char* m_pImage;
	unsigned char* m_pGrayImage;
	unsigned char* m_pAnalysisImage;
	MIL_ID m_AnalysisImage;
	MIL_ID m_GrayImage;
	MIL_ID m_ChildAnalysis;
	MIL_INT m_blobnum;
	double* m_pBlobArea;
	double* m_pBlobMeanPixel;
	double* m_pBlobPosX;
	double* m_pBlobPosY;
	double* m_pBlobLeft;
	double* m_pBlobRight;
	double* m_pBlobTop;
	double* m_pBlobBottom;
	double* m_pBlobWidth;
	double* m_pBlobHeight;//
	double* m_pBlobArea_cur;
	double* m_pBlobMeanPixel_cur;
	double* m_pBlobPosX_cur;
	double* m_pBlobPosY_cur;
	double* m_pBlobLeft_cur;
	double* m_pBlobRight_cur;
	double* m_pBlobTop_cur;
	double* m_pBlobBottom_cur;
	double* m_pBlobWidth_cur;
	double* m_pBlobHeight_cur;
	int		m_HistLowThreshold;
	int		m_HistHighThreshold;
	int		m_HistOpenValue;
	int		m_HistCloseValue;
	int		m_SelectOpenValue;
	int		m_BinaryValue;
	int		m_BlobLeastArea;
	int		m_BlobLeastMeanValue;
	int		m_BlobLeastWidth;
	int		m_BlobLeastHeight;
	int		m_BlobMaxArea;
	BOOL    m_bInitDefect;
public:
	int InitDefect(AppClass* pA, int nWidth, int nHeight);
	int CalculateDefect(ImageInfo imageinfo, ImageInfo imagemask);
	int UinitDefect(); 
	int ModifyFrame(unsigned char* pSour, int nWidth, int nHeight, int nMaxGap);
};