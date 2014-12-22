#include "stdafx.h"
#include "MvClass.h"
#include <math.h>
#include <malloc.h>
#include "HoughDll.h"
#pragma comment(lib,"HoughDll.lib");

extern HMODULE g_hINSTANCE;

AppClass::AppClass()
{
	m_App					= M_NULL;
	m_Sys					= M_NULL;
	m_Calibration			= M_NULL;
	m_OriginImage			= M_NULL;
	m_CalImage				= M_NULL;
	m_ImageWidth			= 0;
	m_ImageHeight			= 0;
	m_ImageByteLine			= 0;
	m_pOrigin				= NULL;
	m_pCal					= NULL;
	memset(m_ModelPath, 0 , sizeof(MAX_PATH));
	m_bGetModelPath			= FALSE;
	m_bAllocBuffer			= FALSE;
	pCircleClass			= NULL;
	m_CircleClassNumber		= 0;
	m_CurCircleIndex		= 0;
	pEdgeClass				= NULL;
	m_EdgeClassNumber		= 0;
	m_CurEdgeIndex			= 0;
	pDefectClass			= NULL;
	m_DefectClassNumber		= 0;
	m_CurDefectIndex		= 0;
	m_ZeroPoint.X			= 0;
	m_ZeroPoint.Y			= 0;
	m_RelativeAngle			= 0;
	m_CircleSizeRedundance	= 0;
	m_bDebugSaveBmp			= 0;
	m_ShenFilterSmoothness	= 0;
	//m_bExcuteBlobCalculate	= NULL;
	OutputDebugStringA("****************AppClass Constructor*****************");
}
AppClass::~AppClass()
{
	UinitMil();
	OutputDebugStringA("****************AppClass Destructor*****************");
}
int AppClass::GetDllPath()
{
	char exeFullPath[MAX_PATH];
	DWORD retval;
	if(g_hINSTANCE)
	{
		OutputDebugStringA("Return dll full path"); 
		retval = GetModuleFileNameA(g_hINSTANCE,exeFullPath,MAX_PATH); 
		DWORD erro = GetLastError();
		char str[MAX_PATH];
		sprintf(str,"Error Code:%d",erro);
		OutputDebugStringA(str);
		LPVOID lpMsgBuf;
		FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
			NULL,erro,MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPTSTR) &lpMsgBuf,0,NULL );
		OutputDebugStringW((TCHAR*)lpMsgBuf);
		OutputDebugStringA(exeFullPath);
		if(retval == 0)
		{
			return -1;
		}
	}
	else
	{
		OutputDebugStringA("Return exe full path"); 
		retval = GetModuleFileNameA(NULL,exeFullPath,MAX_PATH); 
		DWORD erro = GetLastError();
		char str[MAX_PATH];
		sprintf(str,"Error Code:%d",erro);
		OutputDebugStringA(str);
		LPVOID lpMsgBuf;
		FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
			NULL,erro,MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPTSTR) &lpMsgBuf,0,NULL );
		OutputDebugStringW((TCHAR*)lpMsgBuf);
		OutputDebugStringA(exeFullPath);
		if(retval == 0)
		{
			return -1;
		}
	}
	size_t length = strlen(exeFullPath);
	size_t pos = 0;
	for(size_t i=length-1;i>=0;i--)
	{
		if(exeFullPath[i] == '\\')
		{
			pos = i;
			break;
		}
	}
	size_t k=0;
	for(k=0;k<pos;k++)
	{
		m_ModelPath[k] = exeFullPath[k];
	}
	m_ModelPath[k] = '\0';
	OutputDebugStringA(m_ModelPath);
	m_bGetModelPath = TRUE;
	return 0;
}
int AppClass::LoadParameter()
{
	char strPath[MAX_PATH];
	strcpy(strPath,m_ModelPath);
	strcat(strPath,"\\inspectpara.ini");
	OutputDebugStringA(strPath);
	int retval;
	retval = GetPrivateProfileIntA("PARAMETER", "m_BinaryValue", 0, strPath);
	m_DefectInitPara.m_BinaryValue = retval;
	retval = GetPrivateProfileIntA("PARAMETER", "m_BlobLeastArea", 0, strPath);
	m_DefectInitPara.m_BlobLeastArea = retval;
	retval = GetPrivateProfileIntA("PARAMETER", "m_BlobLeastMeanValue", 0, strPath);
	m_DefectInitPara.m_BlobLeastMeanValue = retval;
	retval = GetPrivateProfileIntA("PARAMETER", "m_HistCloseValue", 0, strPath);
	m_DefectInitPara.m_HistCloseValue = retval;
	retval = GetPrivateProfileIntA("PARAMETER", "m_HistOpenValue", 0, strPath);
	m_DefectInitPara.m_HistOpenValue = retval;
	retval = GetPrivateProfileIntA("PARAMETER", "m_HistLowThreshold", 0, strPath);
	m_DefectInitPara.m_HistLowThreshold = retval;
	retval = GetPrivateProfileIntA("PARAMETER", "m_HistHighThreshold", 0, strPath);
	m_DefectInitPara.m_HistHighThreshold = retval;
	retval = GetPrivateProfileIntA("PARAMETER", "m_BlobLeastWidth", 0, strPath);
	m_DefectInitPara.m_BlobLeastWidth = retval;
	retval = GetPrivateProfileIntA("PARAMETER", "m_BlobLeastHeight", 0, strPath);
	m_DefectInitPara.m_BlobLeastHeight = retval;
	retval = GetPrivateProfileIntA("PARAMETER", "m_BlobMaxArea", 0, strPath);
	m_DefectInitPara.m_BlobMaxArea = retval;
	retval = GetPrivateProfileIntA("PARAMETER", "m_SelectOpenValue", 0, strPath);
	m_DefectInitPara.m_SelectOpenValue = retval;
	//////////////////////////////////////////
	retval = GetPrivateProfileIntA("PARAMETER","m_ShenFilterSmoothness",0,strPath);
	m_ShenFilterSmoothness = static_cast<long>(retval);
	retval = GetPrivateProfileIntA("PARAMETER","m_bDebugSaveBmp",0,strPath);
	m_bDebugSaveBmp = static_cast<long>(retval);
	retval = GetPrivateProfileIntA("PARAMETER","m_CircleSizeRedundance",0,strPath);
	m_CircleSizeRedundance = static_cast<long>(retval);
	retval = GetPrivateProfileIntA("PARAMETER","m_CircleClassNumber",0,strPath);
	m_CircleClassNumber = static_cast<long>(retval);
	retval = GetPrivateProfileIntA("PARAMETER","m_EdgeClassNumber",0,strPath);
	m_EdgeClassNumber = static_cast<long>(retval);
	retval = GetPrivateProfileIntA("PARAMETER","m_DefectClassNumber",0,strPath);
	m_DefectClassNumber = static_cast<long>(retval);
	GetPrivateProfileStringA("PARAMETER", "ExcuteBlobCalculate", "000", m_bExcuteBlobCalculate, MAX_PATH, strPath);
	m_bExcuteBlobCalculate[MAX_PATH - 1] = '\0';
	OutputDebugStringA("\r\n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
	OutputDebugStringA(m_bExcuteBlobCalculate);
	OutputDebugStringA("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n");
	sprintf(strPath,"Width = %d(pixels), Height = %d(pixels),m_CircleClassNumber = %d, m_EdgeClassNumber = %d, m_DefectClassNumber = %d, m_ShenFilterSmoothness = %d\n",
		m_ImageWidth,m_ImageHeight,m_CircleClassNumber,m_EdgeClassNumber,m_DefectClassNumber,m_ShenFilterSmoothness);
	OutputDebugStringA(strPath);
	if(m_ImageWidth == 0 || m_ImageHeight == 0 || m_CircleClassNumber == 0 || m_EdgeClassNumber == 0 || m_DefectClassNumber == 0) return -1;
	//m_bExcuteBlobCalculate = (char*)malloc((m_CircleClassNumber + 1) * sizeof(char));
	return 0;
}
int AppClass::LoadCalibration()
{
	char strPath[MAX_PATH];
	strcpy(strPath,m_ModelPath);
	strcat(strPath,"\\byd.cal");
	OutputDebugStringA(strPath);
	if(m_Calibration)
	{
		McalFree(m_Calibration);
		m_Calibration = M_NULL;
	}
	if(M_NULL == McalRestoreA(strPath,m_Sys,M_DEFAULT,&m_Calibration))
		return -1;
	return 0;
}
int AppClass::InitMil(int width, int height)
{
	m_ImageWidth = width;
	m_ImageHeight = height;
	m_ImageByteLine = (m_ImageWidth + 3) / 4 * 4; 
	if(GetDllPath() != 0)
	{
		OutputDebugStringA("**********GetDllPath Fail************\n");
		return -1;
	}
	if(LoadParameter() != 0)
	{
		OutputDebugStringA("**********LoadParameter Fail************\n");
		return -1;
	}
	MappAlloc(M_DEFAULT,&m_App);
	MsysAlloc(M_SYSTEM_HOST,M_DEFAULT,M_DEFAULT,&m_Sys);
	if(LoadCalibration() != 0)
	{
		OutputDebugStringA("**********LoadCalibration Fail************\n");
		return -1;
	}
	if(m_ImageWidth <= 0 || m_ImageHeight <= 0) 
	{
		OutputDebugStringA("**********Wrong Image Size************\n");
		return -1;
	}
	if(m_pOrigin) 
	{
		free(m_pOrigin);
		m_pOrigin = NULL;
	}
	if(m_pCal)
	{
		free(m_pCal);
		m_pCal = NULL;
	}
	if(m_OriginImage)
	{
		MbufFree(m_OriginImage);
		m_OriginImage = M_NULL;
	}
	if(m_CalImage)
	{
		MbufFree(m_CalImage);
		m_CalImage = M_NULL;
	}
	m_ImageByteLine = (m_ImageWidth + 3) / 4 * 4;
	m_pOrigin = (unsigned char*)malloc(m_ImageByteLine * m_ImageHeight);
	memset(m_pOrigin, 0, m_ImageByteLine * m_ImageHeight);
	m_pCal = (unsigned char*)malloc(m_ImageByteLine * m_ImageHeight);
	memset(m_pCal, 0, m_ImageByteLine * m_ImageHeight);
	MbufCreate2d(m_Sys,m_ImageWidth,m_ImageHeight,M_UNSIGNED+8,M_PROC+M_IMAGE,M_HOST_ADDRESS+M_PITCH,m_ImageByteLine,(void*)m_pOrigin,&m_OriginImage);
	MbufCreate2d(m_Sys,m_ImageWidth,m_ImageHeight,M_UNSIGNED+8,M_PROC+M_IMAGE,M_HOST_ADDRESS+M_PITCH,m_ImageByteLine,(void*)m_pCal,&m_CalImage);
	MbufChild2d(m_OriginImage,1000,1000,6000,9500,&m_OriginChild);
	MbufChild2d(m_CalImage,1000,1000,6000,9500,&m_CalChild);
	MbufClear(m_OriginImage,0);
	MbufClear(m_CalImage,0);
	m_bAllocBuffer = TRUE;
	if(InitDefinitions() != 0)
		return -1;
	pDefectClass = new DefectClass; //20141218 gtc add
	pDefectClass->InitDefect(this, m_ImageWidth, m_ImageHeight); //20141218 gtc add
	return 0;
}
int AppClass::UinitMil()
{
	if(m_pOrigin) 
	{
		free(m_pOrigin);
		m_pOrigin = NULL;
	}
	if(m_pCal)
	{
		free(m_pCal);
		m_pCal = NULL;
	}
	if(m_CalChild)
	{
		MbufFree(m_CalChild);
		m_CalChild = M_NULL;
	}
	if(m_OriginChild)
	{
		MbufFree(m_OriginChild);
		m_OriginChild = M_NULL;
	}
	if(m_OriginImage)
	{
		MbufFree(m_OriginImage);
		m_OriginImage = M_NULL;
	}
	if(m_CalImage)
	{
		MbufFree(m_CalImage);
		m_CalImage = M_NULL;
	}
	if(m_Calibration)
	{
		McalFree(m_Calibration);
		m_Calibration = M_NULL;
	}
	FreeDefinitions();
	if (pDefectClass) //20141218 gtc add
	{
		delete pDefectClass;
		pDefectClass = NULL;
	}
	if(m_Sys)
	{
		MsysFree(m_Sys);
		m_Sys = M_NULL;
	}
	if(m_App)
	{
		MappFree(m_App);
		m_App = M_NULL;
	}
	return 0;
}
int AppClass::CalibrateOriginImage(ImageInfo iSour, ImageInfo* iCalImage)
{
	if(iSour.Buffer == NULL || iSour.Height != m_ImageHeight || iSour.Width != m_ImageWidth || iSour.BitsPerPixel != 8)		
	{
		OutputDebugStringA("*******CalibrateOriginImage Input Wrong ImageInfo struct********");
		return -1;
	}
	memcpy_s(m_pOrigin, m_ImageByteLine * m_ImageHeight, iSour.Buffer, m_ImageByteLine * m_ImageHeight);
	McalTransformImage(m_OriginImage,m_CalImage,m_Calibration,M_BILINEAR+M_OVERSCAN_CLEAR,M_FULL_CORRECTION,M_DEFAULT);
	//McalTransformImage(m_OriginChild,m_CalChild,m_Calibration,M_BILINEAR+M_OVERSCAN_CLEAR,M_FULL_CORRECTION,M_DEFAULT);
	//MbufClear(m_OriginImage,0);
	//MbufCopy(m_CalChild,m_OriginChild);
	//MbufExportA("D:\\2014-09_比亚迪手机\\2014-12-07_0\\Vins.Phone_3.x(git)\\bin\\Release\\1.bmp", M_BMP, m_CalImage);
	iCalImage->BitsPerPixel = 8;
	iCalImage->Buffer = m_pCal;
	iCalImage->Height = m_ImageHeight;
	iCalImage->Index = 0;
	iCalImage->SurfaceTypeIndex = 0;
	iCalImage->Width = m_ImageWidth;
	OutputDebugStringA("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^CalibrateOriginImage&&&&^^^^^^^^^^^^^^^");
	return 0;
}
void AppClass::SaveCalBmp()
{
	static int index = 0;
	if (m_bDebugSaveBmp != 0)
	{
		char text_s[MAX_PATH];
		char extension[100];
		sprintf(extension, "\\CalImage_%04d.bmp", index++);
		strcpy(text_s,m_ModelPath);
		strcat(text_s,extension);
		MbufExportA(text_s,M_BMP,m_CalImage);
	}
}
int AppClass::ToRelativeCoord(Line baseline, double angle)
{
	double k,b,originangle;
	if(baseline.X1 == baseline.X2)
	{
		originangle = 90 - angle;
		m_RelativeAngle = originangle;
		m_ZeroPoint.X = baseline.X1;
		m_ZeroPoint.Y = baseline.Y1;
	}
	else
	{
		k = (baseline.Y1 - baseline.Y2) / (baseline.X1 - baseline.X2);
		b = baseline.Y1 - k * baseline.X1;
		originangle = atan(k) / 3.1415926 * 180.0;
		originangle -= angle;
		m_RelativeAngle = originangle + 90.0;
		m_ZeroPoint.X = baseline.X1;
		m_ZeroPoint.Y = baseline.Y1;
	}
	return 0;
}
int AppClass::InitDefinitions()
{
	if(pCircleClass || pEdgeClass/* || pDefectClass*/) //20141218 gtc delete
		return -1;
	pCircleClass = new CircleClass[m_CircleClassNumber];
	m_CurCircleIndex = 0;
	pEdgeClass = new EdgeClass[m_EdgeClassNumber];
	m_CurEdgeIndex = 0;
	/*pDefectClass = new DefectClass[m_DefectClassNumber];//20141218 gtc delete
	m_CurDefectIndex = 0;*///20141218 gtc delete
	return 0;
}
int AppClass::FreeDefinitions()
{
	int retval=0;
	if(pCircleClass)
	{
		for(int i=0; i<m_CircleClassNumber; ++i)
		{
			retval += pCircleClass[i].UinitCircle();
			//retval = pCircleClass[i].UinitCircle();
		}
		delete []pCircleClass;
		pCircleClass = NULL;
		m_CurCircleIndex = 0;
	}
	if(pEdgeClass)
	{
		for(int i=0; i<m_EdgeClassNumber; ++i)
		{
			retval += pEdgeClass[i].UinitEdge();
		}
		delete []pEdgeClass;
		pEdgeClass = NULL;
		m_CurEdgeIndex = 0;
	}
	/*if(pDefectClass) //20141218 gtc delete
	{
		for(int i=0; i<m_DefectClassNumber; ++i)
		{
			retval += pDefectClass[i].UinitDefect();
		}
		delete []pDefectClass;
		pDefectClass = NULL;
		m_CurDefectIndex = 0;
	}*/
	return retval;
}

Point AppClass::FromRelativeToOrigin(Point pt)
{
	// Xm = cos * (Xs - deltax) + sin * (Ys - deltay)
	// Ym = cos * (Ys - deltay) - sin * (Xs - deltax)
	Point tempPoint;
	double fcos = cos(m_RelativeAngle / 180.0 * 3.1415926);
	double fsin = sin(m_RelativeAngle / 180.0 * 3.1415926);
	tempPoint.X = pt.X * fcos - pt.Y * fsin + m_ZeroPoint.X;
	tempPoint.Y = pt.X * fsin + pt.Y * fcos + m_ZeroPoint.Y;
	return tempPoint;
}

Point AppClass::FromOriginToRelative(Point pt)
{
	// Xs = Xm * cos - Ym * sin + deltax
	// Ys = Xm * sin + Ym * cos + deltay
	Point tempPoint;
	double fcos = cos(m_RelativeAngle / 180.0 * 3.1415926);
	double fsin = sin(m_RelativeAngle / 180.0 * 3.1415926);
	tempPoint.X = fcos * (pt.X - m_ZeroPoint.X) + fsin * (pt.Y - m_ZeroPoint.Y);
	tempPoint.Y = fcos * (pt.Y - m_ZeroPoint.Y) - fsin * (pt.X - m_ZeroPoint.X);
	/*FILE* fw = fopen("C:\\Users\\qianminhua\\Desktop\\log.txt","wb");
	fprintf(fw, "m_ZeroPoint Point(%.3f,%.3f), m_RelativeAngle = %.3f\r\n", m_ZeroPoint.X, m_ZeroPoint.Y, m_RelativeAngle);
	fprintf(fw,"Origin Point(%.3f,%.3f) to Relative Point (%.3f,%.3f)\r\n",pt.X,pt.Y,tempPoint.X,tempPoint.Y);
	fclose(fw);*/
	return tempPoint;
}
int AppClass::GetDistanceBetweenLines(Line line1,Line line2,double* distanceInPixel,double* distanceInWorld,double* angle)
{
	Point line1pt1,line1pt2;
	Point line2pt1,line2pt2;
	Point interpt;
	double k1,k2,k3;
	double b1,b2,b3;
	double tempangle;
	line1pt1.X = line1.X1;
	line1pt1.Y = line1.Y1;
	line1pt2.X = line1.X2;
	line1pt2.Y = line1.Y2;
	line2pt1.X = line2.X1;
	line2pt1.Y = line2.Y1;
	line2pt2.X = line2.X2;
	line2pt2.Y = line2.Y2;
	line1pt1 = FromOriginToRelative(line1pt1);
	line1pt2 = FromOriginToRelative(line1pt2);
	line2pt1 = FromOriginToRelative(line2pt1);
	line2pt2 = FromOriginToRelative(line2pt2);
	if((line1pt1.X == line1pt2.X && line1pt1.Y == line1pt2.Y) || (line2pt1.X == line2pt2.X && line2pt1.Y == line2pt2.Y))
	{
		OutputDebugStringA("****GetDistanceBetweenLines Input Wrong Line Parameter****");
		return -1;
	}
	if(line1pt1.X == line1pt2.X && line1pt1.Y != line1pt2.Y)
	{
		if(line2pt1.X == line2pt2.X && line2pt1.Y != line2pt2.Y)
		{
			*distanceInPixel = abs(line2pt1.X - line1pt1.X);
			*distanceInWorld = *distanceInPixel;
			*angle = 90;
		}
		else if(line2pt1.X != line2pt2.X && line2pt1.Y == line2pt2.Y)
		{
			*distanceInPixel = 0;
			*distanceInWorld = 0;
			*angle = 0;
		}
		else
		{
			k2 = (line2pt2.Y - line2pt1.Y) / (line2pt2.X  - line2pt1.X );
			*angle = atan(k2) / 3.1415926 * 180.0;
			*angle *= 0.5;
			k3 = tan((*angle) / 180.0 * 3.1415926);
			if(k3 == 0)
			{
				*distanceInPixel = abs((line2pt2.Y + line2pt1.Y) *0.5 - (line1pt2.Y + line1pt1.Y) *0.5);
				*distanceInWorld = *distanceInPixel;
			}
			else
			{
				k3 = -1.0 / k3;
				b3 = (line2pt2.Y + line2pt1.Y) *0.5 - k3 * (line2pt2.X + line2pt1.X) *0.5;
				interpt.X = line1pt1.X;
				interpt.Y = interpt.X * k3 + b3;
				*distanceInPixel = sqrt((interpt.Y - (line2pt2.Y + line2pt1.Y) *0.5) * (interpt.Y - (line2pt2.Y + line2pt1.Y) *0.5) + 
					(interpt.X - (line2pt2.X + line2pt1.X) *0.5) * (interpt.X - (line2pt2.X + line2pt1.X) *0.5));
				*distanceInWorld = *distanceInPixel;
			}
		}
	}
	else if(line1pt1.X != line1pt2.X && line1pt1.Y == line1pt2.Y)
	{
		if(line2pt1.X == line2pt2.X && line2pt1.Y != line2pt2.Y)
		{
			*distanceInPixel = 0;
			*distanceInWorld = 0;
			*angle = 0;
		}
		else if(line2pt1.X != line2pt2.X && line2pt1.Y == line2pt2.Y)
		{
			*distanceInPixel = abs(line2pt1.Y - line1pt1.Y);
			*distanceInWorld = *distanceInPixel;
			*angle = 90;
		}
		else
		{
			k2 = (line2pt2.Y - line2pt1.Y) / (line2pt2.X  - line2pt1.X);
			*angle = atan(k2) / 3.1415926 * 180.0;
			*angle *= 0.5;
			k3 = tan((*angle) / 180.0 * 3.1415926);
			if(k3 == 0)
			{
				*distanceInPixel = abs((line2pt2.Y + line2pt1.Y) *0.5 - (line1pt2.Y + line1pt1.Y) *0.5);
				*distanceInWorld = *distanceInPixel;
			}
			else
			{
				k3 = -1.0 / k3;
				b3 = (line2pt2.Y + line2pt1.Y) *0.5 - k3 * (line2pt2.X + line2pt1.X) *0.5;
				interpt.Y = line1pt1.Y;
				interpt.X = (interpt.Y - b3) / k3;
				*distanceInPixel = sqrt((interpt.Y - (line2pt2.Y + line2pt1.Y) *0.5) * (interpt.Y - (line2pt2.Y + line2pt1.Y) *0.5) + 
					(interpt.X - (line2pt2.X + line2pt1.X) *0.5) * (interpt.X - (line2pt2.X + line2pt1.X) *0.5));
				*distanceInWorld = *distanceInPixel;
			}
		}
	}
	else
	{
		k1 = (line1pt2.Y - line1pt1.Y) / (line1pt2.X  - line1pt1.X);
		tempangle = atan(k1) / 3.1415926 * 180.0;
		b1 = line1pt1.Y - k1 * line1pt1.X;
		if(line2pt1.X == line2pt2.X && line2pt1.Y != line2pt2.Y)
		{
			tempangle = tempangle * 0.5;
			k3 = tan(tempangle / 180.0 * 3.1415926);
			b3 = (line1pt2.Y + line1pt1.Y) *0.5 - k3 * (line1pt2.X + line1pt1.X) *0.5;
			interpt.X = line2pt1.X;
			interpt.Y = k3 * interpt.X + b3;
			*distanceInPixel = sqrt((interpt.Y - (line1pt2.Y + line1pt1.Y) *0.5) * (interpt.Y - (line1pt2.Y + line1pt1.Y) *0.5) + 
				(interpt.X - (line1pt2.X + line1pt1.X) *0.5) * (interpt.X - (line1pt2.X + line1pt1.X) *0.5));
			*distanceInWorld = *distanceInPixel;
			*angle = tempangle;
		}
		else if(line2pt1.X != line2pt2.X && line2pt1.Y == line2pt2.Y)
		{
			tempangle = tempangle * 0.5;
			k3 = tan(tempangle / 180.0 * 3.1415926);
			b3 = (line1pt2.Y + line1pt1.Y) *0.5 - k3 * (line1pt2.X + line1pt1.X) *0.5;
			interpt.Y = line2pt1.Y;
			interpt.X = (interpt.Y - b3) / k3;
			*distanceInPixel = sqrt((interpt.Y - (line1pt2.Y + line1pt1.Y) *0.5) * (interpt.Y - (line1pt2.Y + line1pt1.Y) *0.5) + 
				(interpt.X - (line1pt2.X + line1pt1.X) *0.5) * (interpt.X - (line1pt2.X + line1pt1.X) *0.5));
			*distanceInWorld = *distanceInPixel;
			*angle = tempangle;
		}
		else
		{
			k2 = (line2pt2.Y - line2pt1.Y) / (line2pt2.X  - line2pt1.X);
			b2 = line2pt2.Y - k2 * line2pt2.X;
			tempangle += atan(k2) / 3.1415926 * 180.0;
			tempangle *= 0.5;
			k3 = tan(tempangle / 180.0 * 3.1415926);
			b3 = (line1pt2.Y + line1pt1.Y) *0.5 - k3 * (line1pt2.X + line1pt1.X) *0.5;
			interpt.Y = (k3*b2 - k2*b3) / (k3-k2);
			interpt.X = (b2-b3) / (k3-k2);
			*distanceInPixel = sqrt((interpt.Y - (line1pt2.Y + line1pt1.Y) *0.5) * (interpt.Y - (line1pt2.Y + line1pt1.Y) *0.5) + 
				(interpt.X - (line1pt2.X + line1pt1.X) *0.5) * (interpt.X - (line1pt2.X + line1pt1.X) *0.5));
			*distanceInWorld = *distanceInPixel;
			*angle = tempangle;
		}
	}
	return 0;
}
int AppClass::GetDistanceBetweenPoints(Point point1,Point point2,double* distanceInPixel,double* distanceInWorld,double* angle)
{
	Point relativept1,relativept2;
	double k;
	relativept1 = FromOriginToRelative(point1);
	relativept2 = FromOriginToRelative(point2);
	*distanceInPixel = sqrt((relativept1.Y - relativept2.Y)*(relativept1.Y - relativept2.Y) + (relativept1.X - relativept2.X)*(relativept1.X - relativept2.X));
	*distanceInWorld = *distanceInPixel;
	if(relativept1.X == relativept2.X && relativept1.Y == relativept2.Y)
	{
		*angle = 0; 
	}
	else if(relativept1.X == relativept2.X && relativept1.Y != relativept2.Y)
	{
		*angle = 90;
	}
	else if(relativept1.X != relativept2.X && relativept1.Y == relativept2.Y)
	{
		*angle = 0;
	}
	else
	{
		k = (relativept2.Y - relativept1.Y) / (relativept2.X - relativept1.X);
		*angle = atan(k) / 3.1415926 * 180.0;
	}
	return 0;
}
int AppClass::GetDistancePointToLine(Point point,Line line,double* distanceInPixel,double* distanceInWorld,Point* pt)
{
	point = FromOriginToRelative(point);
	Point pt1,pt2;
	pt1.X = line.X1;
	pt1.Y = line.Y1;
	pt2.X = line.X2;
	pt2.Y = line.Y2;
	pt1 = FromOriginToRelative(pt1);
	pt2 = FromOriginToRelative(pt2);
	double k,b;
	if(pt1.X == pt2.X && pt1.Y == pt2.Y)
	{
		OutputDebugStringA("****GetDistancePointToLine Function Input Wrong Line****");
		return -1;
	}
	if(pt1.X != pt2.X && pt1.Y == pt2.Y)
	{
		*distanceInPixel = abs(point.Y - pt1.Y);
		pt->X = point.X;
		pt->Y = pt1.Y;
	}
	else if(pt1.X == pt2.X && pt1.Y != pt2.Y)
	{
		*distanceInPixel = abs(point.X - pt1.X);
		pt->Y= point.Y;
		pt->X = pt1.X;
	}
	else
	{
		k = (pt2.Y - pt1.Y) / (pt2.X - pt1.X);
		b = pt2.Y - pt2.X * k;
		*distanceInPixel = abs(point.X * k + b - point.Y) / sqrt(k*k + 1);
		*distanceInWorld = *distanceInPixel;
		double b1 = point.Y + point.X / k;
		pt->Y = (k * b1 + b / k ) / (k + 1.0 / k);
		pt->X = (b1 - b) / (k + 1.0 / k);
	}
	return 0;
}








///////////////////////////////////////////////////////////////////////////////////////////////////
CircleClass::CircleClass()
{
	m_Center_x		= 0;
	m_Center_y		= 0;
	m_OuterRad		= 0;
	m_InnerRad		= 0;
	m_LowThreshold	= 0;
	m_HighThreshold	= 0;
	m_fPosDif		= 0;
	m_fRadDif		= 0;
	m_fRatio		= 0;
	m_ImageWidth	= 0;
	m_ImageHeight	= 0;
	pSour			= NULL;
	obj.center_x	= 0;
	obj.center_y	= 0;
	obj.circle_rad	= 0;
	obj.circleCount = 0;
	pApp = NULL;
	m_processtype	= 0;
	m_bExcuteBlobAnalysis = 0;
	OutputDebugStringA("********************CircleClass Constructor******************");
}
CircleClass::~CircleClass()
{
	/*if (obj.center_x)
	{
		free(obj.center_x);
		obj.center_x = NULL;
	}
	if (obj.center_y)
	{
		free(obj.center_y);
		obj.center_y = NULL;
	}
	if (obj.circle_rad)
	{
		free(obj.circle_rad);
		obj.circle_rad = NULL;
	}*/
	OutputDebugStringA("********************CircleClass Destructor******************");
}
int CircleClass::FindCircle(int iCircleIndex)
{
	//ImageInfo iSour;
	//MIL_ID childbuffer;
	//MIL_ID MaskBuffer;
	//char s_s[MAX_PATH];
	//char text_s[MAX_PATH];
	//iSour.BitsPerPixel = 8;
	//iSour.Buffer = pSour;
	//iSour.Height = m_ImageHeight;
	//iSour.Width = m_ImageWidth;
	//iSour.Index = 0;
	//iSour.SurfaceTypeIndex = 0;
	//HoughSize iSize;
	//int redundance = pApp->m_CircleSizeRedundance;
	//iSize.off_x = (m_Center_x - m_OuterRad - redundance) < 0 ? 0 : (m_Center_x - m_OuterRad - redundance);
	//iSize.off_y = (m_Center_y - m_OuterRad - redundance) < 0 ? 0 : (m_Center_y - m_OuterRad - redundance);
	//iSize.width = (iSize.off_x + (m_OuterRad + redundance) * 2) > m_ImageWidth ? m_ImageWidth - iSize.off_x -1 : (m_OuterRad + redundance) * 2;
	//iSize.height = (m_Center_y + m_OuterRad + redundance) > m_ImageHeight ? m_ImageHeight- iSize.off_y - 1 : (m_OuterRad + redundance) * 2;
	//////////////////////////////////////////////////preprocess part
	//MbufChild2d(pApp->m_CalImage,(long)(iSize.off_x),(long)(iSize.off_y),(long)(iSize.width),(long)(iSize.height),&childbuffer);
	//MbufAlloc2d(pApp->m_Sys,(long)(iSize.width), (long)(iSize.height), M_UNSIGNED + 8L, M_PROC+M_IMAGE, &MaskBuffer);
	//OutputDebugStringA("******FindCircle MbufChild2d******");
	///*MbufExport(M_INTERACTIVE,M_BMP,childbuffer);
	//MbufExport(M_INTERACTIVE,M_BMP,pApp->m_CalImage);*/
	////MbufExport(M_INTERACTIVE,M_BMP,childbuffer);
	//if(m_processtype == 0)
	//{
	//	MimConvolve(childbuffer,childbuffer,M_SHEN_FILTER(M_EDGE_DETECT,pApp->m_ShenFilterSmoothness));
	//	OutputDebugStringA("******FindCircle MimConvolve******");
	//}
	//if(m_LowThreshold == 0)
	//{
	//	MimBinarize(childbuffer,childbuffer,M_GREATER_OR_EQUAL,m_HighThreshold,M_NULL);
	//	OutputDebugStringA("******FindCircle MimBinarize******");
	//	MimClose(childbuffer, childbuffer, 1, M_BINARY);
	//}
	//else
	//{
	//	MimBinarize(childbuffer,childbuffer,M_LESS_OR_EQUAL,m_LowThreshold,M_NULL);
	//	OutputDebugStringA("******FindCircle MimBinarize******");
	//	MimClose(childbuffer, childbuffer, 1, M_BINARY);
	//	MimMorphic(childbuffer, childbuffer, M_3X3_RECT, M_AREA_OPEN, 10, M_BINARY);
	//	MimMorphic(childbuffer, childbuffer, M_3X3_RECT, M_AREA_CLOSE, 10, M_BINARY);
	//}
	////MbufExport(M_INTERACTIVE,M_BMP,childbuffer);
	//sprintf_s(s_s, "iSize.off_x=%d, iSize.off_y=%d, iSize.width=%d, iSize.height=%d, m_OuterRad=%d, m_InnerRad=%d, m_fPosDif=%.3f, m_fRadDif=%.3f, m_fRatio=%.3f", iSize.off_x, iSize.off_y, iSize.width, iSize.height, m_OuterRad, m_InnerRad, m_fPosDif, m_fRadDif, m_fRatio);
	//OutputDebugStringA(s_s);
	//if (pApp->m_bExcuteBlobCalculate[iCircleIndex] == '0')
	//{
	//	GetContour(pSour, iSize.off_x, iSize.off_y, iSize.width, iSize.height);
	//	OutputDebugStringA("******FindCircle GetContour******");
	//	obj = HoughCircle(iSour, iSize, m_OuterRad, m_InnerRad, m_fPosDif, m_fRadDif, m_fRatio);
	//}
	//else
	//{
	//	MIL_ID blob, blobResult;
	//	double ct_x, ct_y, ct_r;
	//	MIL_INT blobnum;
	//	MblobAllocFeatureList(pApp->m_Sys,&blob);
	//	MblobAllocResult(pApp->m_Sys,&blobResult);
	//	MblobSelectFeature(blob, M_CENTER_OF_GRAVITY);
	//	MblobSelectFeature(blob, M_FERET_MEAN_DIAMETER);
	//	MimClose(childbuffer, childbuffer, 1, M_BINARY);
	//	MimMorphic(childbuffer, childbuffer, M_3X3_RECT, M_AREA_OPEN, 5000, M_BINARY);
	//	MimMorphic(childbuffer, childbuffer, M_3X3_RECT, M_AREA_CLOSE, 5000, M_BINARY);
	//	MgraColor(M_DEFAULT, M_COLOR_WHITE);
	//	MbufClear(MaskBuffer,0);
	//	MgraArcFill(M_DEFAULT, MaskBuffer, (double)(iSize.width / 2), (double)(iSize.height / 2), m_OuterRad, m_OuterRad,0,360);
	//	MimArith(childbuffer, MaskBuffer, childbuffer, M_AND);
	//	MblobCalculate(childbuffer,M_NULL,blob,blobResult);
	//	MblobGetNumber(blobResult,&blobnum);
	//	if (blobnum == 1)
	//	{
	//		MblobGetResult(blobResult, M_CENTER_OF_GRAVITY_X, &ct_x);
	//		MblobGetResult(blobResult, M_CENTER_OF_GRAVITY_Y, &ct_y);
	//		MblobGetResult(blobResult, M_FERET_MEAN_DIAMETER, &ct_r);
	//		obj.center_x = ct_x + (double)(iSize.off_x);
	//		obj.center_y = ct_y + (double)(iSize.off_y);
	//		obj.circle_rad = ct_r / 2.0;
	//		obj.circleCount = 1;
	//	}
	//	else
	//	{
	//		obj.center_x = 0;
	//		obj.center_y = 0;
	//		obj.circle_rad = 0;
	//		obj.circleCount = 0;
	//	}
	//}
	//if (pApp->m_bDebugSaveBmp != 0)
	//{
	//	char tt_ss[MAX_PATH];
	//	char tt_s[MAX_PATH];
	//	strcpy(s_s, pApp->m_ModelPath);
	//	strcpy(tt_ss, pApp->m_ModelPath);
	//	sprintf_s(text_s, "\\Circle(%d,%d)_part.bmp", m_Center_x, m_Center_y);
	//	sprintf_s(tt_s, "\\Circle(%d,%d)_whole.bmp", m_Center_x, m_Center_y);
	//	strcat(s_s, text_s);
	//	strcat(tt_ss, tt_s);
	//	MbufExportA(s_s, M_BMP, childbuffer);
	//	//MbufExportA(tt_ss, M_BMP, pApp->m_CalImage);
	//}
	//MbufFree(childbuffer); MbufChild2d(pApp->m_CalImage, (long)(iSize.off_x), (long)(iSize.off_y), (long)(iSize.width), (long)(iSize.height), &childbuffer);
	//MbufFree(MaskBuffer);
	//return 0;

	ImageInfo iSour;
	MIL_ID childbuffer;
	//MIL_ID tempMemory; //201415 gtc delete
	char s_s[MAX_PATH];
	char text_s[MAX_PATH];
	iSour.BitsPerPixel = 8;
	iSour.Buffer = pSour;
	iSour.Height = m_ImageHeight;
	iSour.Width = m_ImageWidth;
	iSour.Index = 0;
	iSour.SurfaceTypeIndex = 0;
	HoughSize iSize;
	int redundance = pApp->m_CircleSizeRedundance;
	iSize.off_x = (m_Center_x - m_OuterRad - redundance) < 0 ? 0 : (m_Center_x - m_OuterRad - redundance);
	iSize.off_y = (m_Center_y - m_OuterRad - redundance) < 0 ? 0 : (m_Center_y - m_OuterRad - redundance);
	iSize.width = (iSize.off_x + (m_OuterRad + redundance) * 2) > m_ImageWidth ? m_ImageWidth - iSize.off_x - 1 : (m_OuterRad + redundance) * 2;
	iSize.height = (m_Center_y + m_OuterRad + redundance) > m_ImageHeight ? m_ImageHeight - iSize.off_y - 1 : (m_OuterRad + redundance) * 2;
	////////////////////////////////////////////////preprocess part
	sprintf_s(s_s, "iSize.off_x=%d, iSize.off_y=%d, iSize.width=%d, iSize.height=%d, m_OuterRad=%d, m_InnerRad=%d, m_fPosDif=%.3f, m_fRadDif=%.3f, m_fRatio=%.3f", iSize.off_x, iSize.off_y, iSize.width, iSize.height, m_OuterRad, m_InnerRad, m_fPosDif, m_fRadDif, m_fRatio);
	OutputDebugStringA(s_s);
	MbufChild2d(pApp->m_CalImage, (long)(iSize.off_x), (long)(iSize.off_y), (long)(iSize.width), (long)(iSize.height), &childbuffer);
	//MbufAlloc2d(pApp->m_Sys, (long)(iSize.width), (long)(iSize.height), M_UNSIGNED + 8L, M_PROC + M_IMAGE, &tempMemory);//201415 gtc delete
	//MbufCopy(childbuffer, tempMemory);//201415 gtc delete
	OutputDebugStringA("******FindCircle MbufChild2d******");
	/*MbufExport(M_INTERACTIVE,M_BMP,childbuffer);
	MbufExport(M_INTERACTIVE,M_BMP,pApp->m_CalImage);*/
	//MbufExport(M_INTERACTIVE,M_BMP,childbuffer);
	m_bExcuteBlobAnalysis = 0;
	if (m_processtype == 0) // filter
	{
		MimConvolve(childbuffer, childbuffer, M_SHEN_FILTER(M_EDGE_DETECT, pApp->m_ShenFilterSmoothness));
		//MimConvolve(childbuffer,childbuffer,M_SHEN_FILTER(M_EDGE_DETECT_SQR,pApp->m_ShenFilterSmoothness));
		OutputDebugStringA("******FindCircle MimConvolve******");
		//MimBinarize(childbuffer, childbuffer, M_OUT_RANGE, m_LowThreshold, m_HighThreshold);//20141215 gtc delete
		//MimClose(childbuffer, childbuffer, 1, M_BINARY);//20141215 gtc delete
		MimThin(childbuffer, childbuffer, M_TO_SKELETON, M_GRAYSCALE);//20141215 gtc add
		MimBinarize(childbuffer, childbuffer, M_GREATER, m_HighThreshold, M_NULL);//20141215 gtc add
		GetContour(pSour, iSize.off_x, iSize.off_y, iSize.width, iSize.height);
		OutputDebugStringA("******FindCircle MimConvolve GetContour******");
		obj = HoughCircle(iSour, iSize, m_OuterRad, m_InnerRad, m_fPosDif, m_fRadDif, m_fRatio);
		m_bExcuteBlobAnalysis = 0;

	}
	else if (m_processtype == 1) // binary
	{
		MimBinarize(childbuffer, childbuffer, M_OUT_RANGE, m_LowThreshold, m_HighThreshold);
		MimClose(childbuffer, childbuffer, 1, M_BINARY);
		MimMorphic(childbuffer, childbuffer, M_3X3_RECT, M_AREA_OPEN, 10, M_BINARY);
		MimMorphic(childbuffer, childbuffer, M_3X3_RECT, M_AREA_CLOSE, 10, M_BINARY);
		OutputDebugStringA("******FindCircle MimBinarize******");
		GetContour(pSour, iSize.off_x, iSize.off_y, iSize.width, iSize.height);
		OutputDebugStringA("******FindCircle  MimBinarize GetContour******");
		obj = HoughCircle(iSour, iSize, m_OuterRad, m_InnerRad, m_fPosDif, m_fRadDif, m_fRatio);
		m_bExcuteBlobAnalysis = 0;
	}
	else // blob
	{
		/*MimBinarize(childbuffer, childbuffer, M_OUT_RANGE, m_LowThreshold, m_HighThreshold);
		MIL_ID blob, blobResult;
		MIL_ID MaskBuffer;
		double ct_x, ct_y, ct_r;
		MIL_INT blobnum;
		MbufAlloc2d(pApp->m_Sys, (long)(iSize.width), (long)(iSize.height), M_UNSIGNED + 8L, M_PROC + M_IMAGE, &MaskBuffer);
		MblobAllocFeatureList(pApp->m_Sys, &blob);
		MblobAllocResult(pApp->m_Sys, &blobResult);
		MblobSelectFeature(blob, M_CENTER_OF_GRAVITY);
		MblobSelectFeature(blob, M_FERET_MEAN_DIAMETER);
		MgraColor(M_DEFAULT, M_COLOR_WHITE);
		MbufClear(MaskBuffer, 0);
		MgraArcFill(M_DEFAULT, MaskBuffer, (double)(iSize.width / 2), (double)(iSize.height / 2), m_OuterRad, m_OuterRad, 0, 360);
		MimArith(childbuffer, MaskBuffer, childbuffer, M_AND);
		MimClose(childbuffer, childbuffer, 1, M_BINARY);
		MimMorphic(childbuffer, childbuffer, M_3X3_RECT, M_AREA_OPEN, 5000, M_BINARY);
		MimMorphic(childbuffer, childbuffer, M_3X3_RECT, M_AREA_CLOSE, (MIL_INT)(3.1415926 * (double)m_InnerRad * (double)m_InnerRad * 0.6), M_BINARY);
		MblobCalculate(childbuffer, M_NULL, blob, blobResult);
		MblobGetNumber(blobResult, &blobnum);
		if (blobnum == 1)
		{
			MblobGetResult(blobResult, M_CENTER_OF_GRAVITY_X, &ct_x);
			MblobGetResult(blobResult, M_CENTER_OF_GRAVITY_Y, &ct_y);
			MblobGetResult(blobResult, M_FERET_MEAN_DIAMETER, &ct_r);
			obj.center_x = ct_x + (double)(iSize.off_x);
			obj.center_y = ct_y + (double)(iSize.off_y);
			obj.circle_rad = ct_r / 2.0;
			obj.circleCount = 1;
		}
		else
		{
			obj.center_x = 0;
			obj.center_y = 0;
			obj.circle_rad = 0;
			obj.circleCount = 0;
		}
		MbufFree(MaskBuffer);
		MblobFree(blobResult);
		MblobFree(blob);*/ //20141215 gtc delete
		MIL_ID MilMetrolContext, MilMetrolResult;
		MIL_DOUBLE Value, PosX, PosY;
		MmetAlloc(pApp->m_Sys, M_DEFAULT, &MilMetrolContext);
		MmetAllocResult(pApp->m_Sys, M_DEFAULT, &MilMetrolResult);
		if (m_LowThreshold == 0)
			MmetAddFeature(MilMetrolContext, M_MEASURED, M_CIRCLE, M_DEFAULT, M_INNER_FIT, M_NULL, M_NULL, M_DEFAULT, M_DEFAULT);
		else
			MmetAddFeature(MilMetrolContext, M_MEASURED, M_CIRCLE, M_DEFAULT, M_FIT, M_NULL, M_NULL, M_DEFAULT, M_DEFAULT);
		MmetSetRegion(MilMetrolContext, M_FEATURE_INDEX(1), M_DEFAULT, M_RING, (MIL_DOUBLE)(m_Center_x), (MIL_DOUBLE)(m_Center_y),
			(MIL_DOUBLE)(m_InnerRad - pApp->m_CircleSizeRedundance), (MIL_DOUBLE)(m_OuterRad + pApp->m_CircleSizeRedundance), M_NULL, M_NULL);
		MmetCalculate(MilMetrolContext, pApp->m_CalImage, MilMetrolResult, M_DEFAULT);
		MmetGetResult(MilMetrolResult, M_FEATURE_INDEX(1), M_RADIUS, &Value);
		MmetGetResult(MilMetrolResult, M_FEATURE_INDEX(1), M_POSITION_X, &PosX);
		MmetGetResult(MilMetrolResult, M_FEATURE_INDEX(1), M_POSITION_Y, &PosY);
		obj.center_x = PosX;
		obj.center_y = PosY;
		obj.circle_rad = Value;
		obj.circleCount = 1;
		MmetFree(MilMetrolResult);
		MmetFree(MilMetrolContext);
		m_bExcuteBlobAnalysis = 1;
	}

	if ((m_processtype == 0 || m_processtype == 1)
		&& (obj.circle_rad == 0 || obj.circleCount == 0))
	{
		char secondtext[MAX_PATH];
		sprintf(secondtext, "@@@@@The %02dth Cirlce Filter or Binary Fail, Continue MilMetrol Analysis@@@@@", iCircleIndex + 1);
		OutputDebugStringA(secondtext);
		//MbufCopy(tempMemory, childbuffer);
		//MimBinarize(childbuffer, childbuffer, M_OUT_RANGE, 25, 255);//20141215 Adjust Binary Threshold
		//MIL_ID blob1, blobResult1;
		//MIL_ID MaskBuffer1;
		//double ct_x1, ct_y1, ct_r1;
		//MIL_INT blobnum1;
		//MbufAlloc2d(pApp->m_Sys, (long)(iSize.width), (long)(iSize.height), M_UNSIGNED + 8L, M_PROC + M_IMAGE, &MaskBuffer1);
		//MblobAllocFeatureList(pApp->m_Sys, &blob1);
		//MblobAllocResult(pApp->m_Sys, &blobResult1);
		//MblobSelectFeature(blob1, M_CENTER_OF_GRAVITY);
		//MblobSelectFeature(blob1, M_FERET_MEAN_DIAMETER);
		//MgraColor(M_DEFAULT, M_COLOR_WHITE);
		//MbufClear(MaskBuffer1, 0);
		//MgraArcFill(M_DEFAULT, MaskBuffer1, (double)(iSize.width / 2), (double)(iSize.height / 2), m_OuterRad, m_OuterRad, 0, 360);
		//MimArith(childbuffer, MaskBuffer1, childbuffer, M_AND);
		//MimClose(childbuffer, childbuffer, 1, M_BINARY);
		//MimMorphic(childbuffer, childbuffer, M_3X3_RECT, M_AREA_OPEN, 5000, M_BINARY);
		//MimMorphic(childbuffer, childbuffer, M_3X3_RECT, M_AREA_CLOSE, (MIL_INT)(3.1415926 * (double)m_InnerRad * (double)m_InnerRad * 0.6), M_BINARY);
		//MblobCalculate(childbuffer, M_NULL, blob1, blobResult1);
		//MblobGetNumber(blobResult1, &blobnum1);
		//if (blobnum1 == 1)
		//{
		//	MblobGetResult(blobResult1, M_CENTER_OF_GRAVITY_X, &ct_x1);
		//	MblobGetResult(blobResult1, M_CENTER_OF_GRAVITY_Y, &ct_y1);
		//	MblobGetResult(blobResult1, M_FERET_MEAN_DIAMETER, &ct_r1);
		//	obj.center_x = ct_x1 + (double)(iSize.off_x);
		//	obj.center_y = ct_y1 + (double)(iSize.off_y);
		//	obj.circle_rad = ct_r1 / 2.0;
		//	obj.circleCount = 1;
		//}
		//else
		//{
		//	obj.center_x = 0;
		//	obj.center_y = 0;
		//	obj.circle_rad = 0;
		//	obj.circleCount = 0;
		//}
		//MbufFree(MaskBuffer1);
		//MblobFree(blobResult1);
		//MblobFree(blob1); //20141215 gtc delete
		MIL_ID MilMetrolContext, MilMetrolResult;
		MIL_DOUBLE Value, PosX, PosY;
		MmetAlloc(pApp->m_Sys, M_DEFAULT, &MilMetrolContext);
		MmetAllocResult(pApp->m_Sys, M_DEFAULT, &MilMetrolResult);
		if (m_LowThreshold == 0)
			MmetAddFeature(MilMetrolContext, M_MEASURED, M_CIRCLE, M_DEFAULT, M_INNER_FIT, M_NULL, M_NULL, M_DEFAULT, M_DEFAULT);
		else
			MmetAddFeature(MilMetrolContext, M_MEASURED, M_CIRCLE, M_DEFAULT, M_FIT, M_NULL, M_NULL, M_DEFAULT, M_DEFAULT);
		MmetSetRegion(MilMetrolContext, M_FEATURE_INDEX(1), M_DEFAULT, M_RING, (MIL_DOUBLE)(m_Center_x), (MIL_DOUBLE)(m_Center_y),
			(MIL_DOUBLE)(m_InnerRad - pApp->m_CircleSizeRedundance), (MIL_DOUBLE)(m_OuterRad + pApp->m_CircleSizeRedundance), M_NULL, M_NULL);
		MmetCalculate(MilMetrolContext, pApp->m_CalImage, MilMetrolResult, M_DEFAULT);
		MmetGetResult(MilMetrolResult, M_FEATURE_INDEX(1), M_RADIUS, &Value);
		MmetGetResult(MilMetrolResult, M_FEATURE_INDEX(1), M_POSITION_X, &PosX);
		MmetGetResult(MilMetrolResult, M_FEATURE_INDEX(1), M_POSITION_Y, &PosY);
		obj.center_x = PosX;
		obj.center_y = PosY;
		obj.circle_rad = Value;
		obj.circleCount = 1;
		MmetFree(MilMetrolResult);
		MmetFree(MilMetrolContext);
		m_bExcuteBlobAnalysis = 1;
	}
	//MbufExport(M_INTERACTIVE,M_BMP,childbuffer);
	if (pApp->m_bDebugSaveBmp != 0)
	{
		char tt_ss[MAX_PATH];
		char tt_s[MAX_PATH];
		strcpy(s_s, pApp->m_ModelPath);
		strcpy(tt_ss, pApp->m_ModelPath);
		sprintf_s(text_s, "\\Circle(%d,%d)_part.bmp", m_Center_x, m_Center_y);
		sprintf_s(tt_s, "\\Circle(%d,%d)_whole.bmp", m_Center_x, m_Center_y);
		strcat(s_s, text_s);
		strcat(tt_ss, tt_s);
		MbufExportA(s_s, M_BMP, childbuffer);
		//MbufExportA(tt_ss, M_BMP, pApp->m_CalImage);
	}
	MbufFree(childbuffer);
	//MbufFree(tempMemory);//201415 gtc delete
	return 0;
}
int CircleClass::InitCircle(AppClass* pA, int width, int height, unsigned char* ptrSour, double Center_x, double Center_y, double InnRad, double OutRad, int LowThre, int HighTre, double PosDif, double RadDif, double ratio, int processtype)
{
	if(Center_x > width-1 || Center_y > height-1 || pA == NULL
		|| pA->m_CurCircleIndex >= pA->m_CircleClassNumber)
		return -1;
	pApp = pA;
	m_ImageWidth	= width;
	m_ImageHeight	= height;
	pSour			= ptrSour;
	m_Center_x		= static_cast<int>(Center_x);
	m_Center_y		= static_cast<int>(Center_y);
	m_OuterRad		= static_cast<int>(OutRad);
	m_InnerRad		= static_cast<int>(InnRad);
	m_LowThreshold	= LowThre;
	m_HighThreshold	= HighTre;
	m_fPosDif		= PosDif;
	m_fRadDif		= RadDif;
	m_fRatio		= ratio;
	m_processtype	= processtype;
	return 0;
}
int CircleClass::ModifyCircle(double Center_x, double Center_y, double InnRad, double OutRad, int LowThre, int HighTre, double PosDif, double RadDif, double ratio)
{
	if(Center_x > m_ImageWidth-1 || Center_y > m_ImageHeight-1)
		return -1;
	m_Center_x		= static_cast<int>(Center_x);
	m_Center_y		= static_cast<int>(Center_y);
	m_OuterRad		= static_cast<int>(OutRad);
	m_InnerRad		= static_cast<int>(InnRad);
	m_LowThreshold	= LowThre;
	m_HighThreshold	= HighTre;
	m_fPosDif		= PosDif;
	m_fRadDif		= RadDif;
	m_fRatio		= ratio;
	return 0;
}
int CircleClass::GetContour(unsigned char* pSr, int offx, int offy, int nWd, int nHt)
{
	int byteline = (nWd + 3) / 4 * 4;
	int grayline = pApp->m_ImageByteLine;
	int i,j;
	unsigned char* pTemp = (unsigned char*)malloc(byteline * nHt);
	for(j=offy+1; j<offy+nHt-1; ++j)
	{
		for(i=offx+1; i<offx+nWd-1; ++i)
		{
			if(*(pSr + j * grayline + i) == 255 &&
				(*(pSr + j * grayline + i-1) == 0
				||*(pSr + j * grayline + i+1) == 0
				|| *(pSr + (j-1) * grayline + i) == 0
				|| *(pSr + (j+1) * grayline + i) == 0))
				*(pTemp + (j-offy-1) * byteline + (i-offx-1)) = 255;
			else
				*(pTemp + (j-offy-1) * byteline + (i-offx-1)) = 0;
		}
	}
	for(j=offy+1; j<offy+nHt-1; ++j)
	{
		for(i=offx+1; i<offx+nWd-1; ++i)
		{
			*(pSr + j * grayline + i) = *(pTemp + (j-offy-1) * byteline + (i-offx-1));
		}
	}
	free(pTemp);
	return 0;
}
int CircleClass::UinitCircle()
{
	OutputDebugStringA("****UinitCircle Function Excute Begin****");
	//if (obj.center_x)
	//{
	//	free(obj.center_x);
	//	obj.center_x = NULL;
	//}
	//if (obj.center_y)
	//{
	//	free(obj.center_y);
	//	obj.center_y = NULL;
	//}
	//if (obj.circle_rad)
	//{
	//	free(obj.circle_rad);
	//	obj.circle_rad = NULL;
	//}
	OutputDebugStringA("****UinitCircle Function Excute End****");
	return 0;
}








///////////////////////////////////////////////////////////////////////////////////////////
EdgeClass::EdgeClass()
{
	m_pApp = NULL;
	m_startPointX	=0;
	m_startPointY	=0;
	m_endPointX		=0;
	m_endPointY		=0;
	m_roiWidth		=0;
	m_polarity		=0;
	m_orientation	=0;
	memset(&m_rect1, 0, sizeof(SearchRect));
	memset(&m_rect2, 0, sizeof(SearchRect));
	m_Mark1			=M_NULL;
	m_Mark2			=M_NULL;
	memset(&m_point1, 0, sizeof(Point));
	memset(&m_point2, 0, sizeof(Point));
	memset(&m_InterPoint, 0, sizeof(Point));
	OutputDebugStringA("*******************EdgeClass Constructor********************");
}
EdgeClass::~EdgeClass()
{
	if(m_Mark1)
	{
		MmeasFree(m_Mark1);
		m_Mark1 = M_NULL;
	}
	if(m_Mark2)
	{
		MmeasFree(m_Mark2);
		m_Mark2 = M_NULL;
	}
	OutputDebugStringA("*******************EdgeClass Destructor****************************");
}
void EdgeClass::CreateBox(double startPointX,double startPointY,double endPointX,double endPointY,double roiWidth)
{
	if(m_orientation == G_ORIENTATION_ANY)
	{
		if(startPointX == endPointX && startPointY != endPointY)
		{
			m_rect1.nOffSetX = (int)(startPointX - roiWidth) < 0 ? 0 : (int)(startPointX - roiWidth);
			m_rect1.nOffSetY = (int)(endPointY < startPointY ? endPointY : startPointY);
			m_rect1.nWidth = (int)(roiWidth * 2.0);
			if((m_rect1.nOffSetX + m_rect1.nWidth) > m_pApp->m_ImageWidth-1)
				m_rect1.nWidth = m_pApp->m_ImageWidth-1-m_rect1.nOffSetX;
			m_rect1.nHeight = (int)(abs(endPointY - startPointY));
			m_rect2.nOffSetX = m_rect1.nOffSetX;
			m_rect2.nOffSetY = m_rect1.nOffSetY;
			m_rect2.nWidth = m_rect1.nWidth;
			m_rect2.nHeight = m_rect1.nHeight;
		}
		else if(startPointX != endPointX && startPointY == endPointY)
		{
			m_rect1.nOffSetY = (int)(startPointY - roiWidth) < 0 ? 0 : (int)(startPointY - roiWidth);
			m_rect1.nOffSetX = (int)(endPointX < startPointX ? endPointX : startPointX);
			m_rect1.nHeight = (int)(roiWidth * 2.0);
			if((m_rect1.nOffSetY + m_rect1.nHeight) > m_pApp->m_ImageHeight-1)
				m_rect1.nHeight = m_pApp->m_ImageHeight-1-m_rect1.nOffSetY;
			m_rect1.nWidth = (int)(abs(endPointX - startPointX));
			m_rect2.nOffSetX = m_rect1.nOffSetX;
			m_rect2.nOffSetY = m_rect1.nOffSetY;
			m_rect2.nWidth = m_rect1.nWidth;
			m_rect2.nHeight = m_rect1.nHeight;
		}
		else
		{
			double delta = atan((endPointY - startPointY) / (endPointX - startPointX));
			double sina = abs(sin(delta) * roiWidth);
			double cosa = abs(cos(delta) * roiWidth);
			m_rect1.nOffSetX = (int)(endPointX < startPointX ? endPointX-sina : startPointX-sina);
			m_rect1.nOffSetY = (int)(endPointY < startPointY ? endPointY-cosa : startPointY-cosa);
			m_rect1.nWidth = (int)(abs(endPointX - startPointX) + sina * 2.0);
			m_rect1.nHeight = (int)(abs(endPointY - startPointY) + cosa * 2.0);
			if(m_rect1.nOffSetX < 0) m_rect1.nOffSetX = 0;
			if(m_rect1.nOffSetY < 0) m_rect1.nOffSetY = 0;
			if((m_rect1.nOffSetX + m_rect1.nWidth) > m_pApp->m_ImageWidth-1) 
				m_rect1.nWidth = m_pApp->m_ImageWidth-1 - m_rect1.nOffSetX;
			if((m_rect1.nOffSetY + m_rect1.nHeight) > m_pApp->m_ImageHeight-1) 
				m_rect1.nHeight = m_pApp->m_ImageHeight-1 - m_rect1.nOffSetY;
			m_rect2.nOffSetX = m_rect1.nOffSetX;
			m_rect2.nOffSetY = m_rect1.nOffSetY;
			m_rect2.nWidth = m_rect1.nWidth;
			m_rect2.nHeight = m_rect1.nHeight;
		}
	}
	else if(m_orientation == G_ORIENTATION_VERTICAL)
	{
		SearchRect tempRect;
		if(startPointX == endPointX && startPointY != endPointY)
		{
			tempRect.nOffSetX = (int)(startPointX - roiWidth);
			tempRect.nOffSetY = (int)(endPointY < startPointY ? endPointY : startPointY);
			tempRect.nWidth = (int)(roiWidth * 2.0);
			tempRect.nHeight = (int)(abs(endPointY - startPointY));
			if(tempRect.nOffSetX < 0) tempRect.nOffSetX = 0;
			if((tempRect.nOffSetX + tempRect.nWidth) > m_pApp->m_ImageWidth-1)
				tempRect.nWidth = m_pApp->m_ImageWidth-1-tempRect.nOffSetX;
		}
		else if(startPointX != endPointX && startPointY == endPointY)
		{
			tempRect.nOffSetY = (int)(startPointY - roiWidth);
			tempRect.nOffSetX = (int)(endPointX < startPointX ? endPointX : startPointX);
			tempRect.nHeight = (int)(roiWidth * 2.0);
			tempRect.nWidth = (int)(abs(endPointX - startPointX));
			if(tempRect.nOffSetY < 0) tempRect.nOffSetY = 0;
			if((tempRect.nOffSetY + tempRect.nHeight) > m_pApp->m_ImageHeight-1)
				tempRect.nHeight = m_pApp->m_ImageHeight-1-tempRect.nOffSetY;
		}
		else
		{
			double delta = atan((endPointY - startPointY) / (endPointX - startPointX));
			double sina = abs(sin(delta) * roiWidth);
			double cosa = abs(cos(delta) * roiWidth);
			tempRect.nOffSetX = (int)(endPointX < startPointX ? endPointX-sina : startPointX-sina);
			tempRect.nOffSetY = (int)(endPointY < startPointY ? endPointY-cosa : startPointY-cosa);
			tempRect.nWidth = (int)(abs(endPointX - startPointX) + sina * 2.0);
			tempRect.nHeight = (int)(abs(endPointY - startPointY) + cosa * 2.0);
			if(tempRect.nOffSetX < 0) tempRect.nOffSetX = 0;
			if(tempRect.nOffSetY < 0) tempRect.nOffSetY = 0;
			if((tempRect.nOffSetX + tempRect.nWidth) > m_pApp->m_ImageWidth-1)
				tempRect.nWidth = m_pApp->m_ImageWidth-1-tempRect.nOffSetX;
			if((tempRect.nOffSetY + tempRect.nHeight) > m_pApp->m_ImageHeight-1)
				tempRect.nHeight = m_pApp->m_ImageHeight-1-tempRect.nOffSetY;
		}
		m_rect1.nOffSetX = tempRect.nOffSetX;
		m_rect1.nOffSetY = tempRect.nOffSetY;
		m_rect1.nWidth = tempRect.nWidth;
		m_rect1.nHeight = tempRect.nHeight / 2;
		m_rect2.nOffSetX = tempRect.nOffSetX;
		m_rect2.nOffSetY = tempRect.nOffSetY + tempRect.nHeight / 2;
		m_rect2.nWidth = tempRect.nWidth;
		m_rect2.nHeight = tempRect.nHeight / 2;
	}
	else
	{
		SearchRect tempRect;
		if(startPointX == endPointX && startPointY != endPointY)
		{
			tempRect.nOffSetX = (int)(startPointX - roiWidth);
			tempRect.nOffSetY = (int)(endPointY < startPointY ? endPointY : startPointY);
			tempRect.nWidth = (int)(roiWidth * 2.0);
			tempRect.nHeight = (int)(abs(endPointY - startPointY));
			if(tempRect.nOffSetX < 0) tempRect.nOffSetX = 0;
			if((tempRect.nOffSetX + tempRect.nWidth) > m_pApp->m_ImageWidth-1)
				tempRect.nWidth = m_pApp->m_ImageWidth-1-tempRect.nOffSetX;
		}
		else if(startPointX != endPointX && startPointY == endPointY)
		{
			tempRect.nOffSetY = (int)(startPointY - roiWidth);
			tempRect.nOffSetX = (int)(endPointX < startPointX ? endPointX : startPointX);
			tempRect.nHeight = (int)(roiWidth * 2.0);
			tempRect.nWidth = (int)(abs(endPointX - startPointX));
			if(tempRect.nOffSetY < 0) tempRect.nOffSetY = 0;
			if((tempRect.nOffSetY + tempRect.nHeight) > m_pApp->m_ImageHeight-1)
				tempRect.nHeight = m_pApp->m_ImageHeight-1-tempRect.nOffSetY;
		}
		else
		{
			double delta = atan((endPointY - startPointY) / (endPointX - startPointX));
			double sina = abs(sin(delta) * roiWidth);
			double cosa = abs(cos(delta) * roiWidth);
			tempRect.nOffSetX = (int)(endPointX < startPointX ? endPointX-sina : startPointX-sina);
			tempRect.nOffSetY = (int)(endPointY < startPointY ? endPointY-cosa : startPointY-cosa);
			tempRect.nWidth = (int)(abs(endPointX - startPointX) + sina * 2.0);
			tempRect.nHeight = (int)(abs(endPointY - startPointY) + cosa * 2.0);
			if(tempRect.nOffSetX < 0) tempRect.nOffSetX = 0;
			if(tempRect.nOffSetY < 0) tempRect.nOffSetY = 0;
			if((tempRect.nOffSetX + tempRect.nWidth) > m_pApp->m_ImageWidth-1)
				tempRect.nWidth = m_pApp->m_ImageWidth-1-tempRect.nOffSetX;
			if((tempRect.nOffSetY + tempRect.nHeight) > m_pApp->m_ImageHeight-1)
				tempRect.nHeight = m_pApp->m_ImageHeight-1-tempRect.nOffSetY;
		}
		m_rect1.nOffSetX = tempRect.nOffSetX;
		m_rect1.nOffSetY = tempRect.nOffSetY;
		m_rect1.nWidth = tempRect.nWidth / 2;
		m_rect1.nHeight = tempRect.nHeight;
		m_rect2.nOffSetX = tempRect.nOffSetX + tempRect.nWidth / 2;
		m_rect2.nOffSetY = tempRect.nOffSetY;
		m_rect2.nWidth = tempRect.nWidth / 2;
		m_rect2.nHeight = tempRect.nHeight;
	}
}
int EdgeClass::InitEdge(AppClass* pA,double startPointX,double startPointY,double endPointX,double endPointY,double roiWidth,int polarity,int orientation)
{
	m_pApp = pA;
	m_startPointY = startPointY;
	m_startPointX = startPointX;
	m_endPointY = endPointY;
	m_endPointX = endPointX;
	m_roiWidth = roiWidth;
	m_polarity = polarity;
	m_orientation = orientation;
	CreateBox(startPointX,startPointY,endPointX,endPointY,roiWidth);
	MmeasAllocMarker(m_pApp->m_Sys,M_EDGE,M_DEFAULT,&m_Mark1);
	MmeasAllocMarker(m_pApp->m_Sys,M_EDGE,M_DEFAULT,&m_Mark2);
	MmeasSetMarker(m_Mark1,M_NUMBER,1L,M_NULL);
	MmeasSetMarker(m_Mark2,M_NUMBER,1L,M_NULL);
	MmeasSetMarker(m_Mark1, M_FILTER_TYPE, M_PREWITT, M_NULL);
	MmeasSetMarker(m_Mark2, M_FILTER_TYPE, M_PREWITT, M_NULL);
	//MmeasSetMarker(m_Mark1, M_BOX_ANGLE_MODE, M_ENABLE, M_NULL);//20141216 gtc delete
	//MmeasSetMarker(m_Mark2, M_BOX_ANGLE_MODE, M_ENABLE, M_NULL);//20141216 gtc delete
	MmeasSetMarker(m_Mark1, M_EDGE_THRESHOLD, 3.0, M_NULL); //20141216 gtc modify 2.0->3.0
	MmeasSetMarker(m_Mark2, M_EDGE_THRESHOLD, 3.0, M_NULL); //20141216 gtc modify 2.0->3.0
	//MmeasSetMarker(m_Mark1, M_BOX_ANGLE, M_ANY, M_NULL); //20141216 gtc delete
	//MmeasSetMarker(m_Mark2, M_BOX_ANGLE, M_ANY, M_NULL);//20141216 gtc delete
	MmeasSetMarker(m_Mark1, M_EDGE_STRENGTH, M_ANY, M_NULL);
	MmeasSetMarker(m_Mark2, M_EDGE_STRENGTH, M_ANY, M_NULL);
	MmeasSetMarker(m_Mark1, M_CONTRAST, M_ANY, M_NULL);
	MmeasSetMarker(m_Mark2, M_CONTRAST, M_ANY, M_NULL);
	if(polarity == G_POLARITY_POSITIVE)
	{
		MmeasSetMarker(m_Mark1, M_POLARITY, M_POSITIVE , M_NULL);
		MmeasSetMarker(m_Mark2, M_POLARITY, M_POSITIVE , M_NULL);
		OutputDebugStringA("@@@@Polarity Positive@@@@");
	}
	else if(polarity == G_POLARITY_NEGATIVE)
	{
		MmeasSetMarker(m_Mark1, M_POLARITY, M_NEGATIVE , M_NULL);
		MmeasSetMarker(m_Mark2, M_POLARITY, M_NEGATIVE , M_NULL);
		OutputDebugStringA("@@@@Polarity Negative@@@@");
	}
	else
	{
		MmeasSetMarker(m_Mark1, M_POLARITY, M_ANY , M_NULL);
		MmeasSetMarker(m_Mark2, M_POLARITY, M_ANY , M_NULL);
		OutputDebugStringA("@@@@Polarity Any@@@@");
	}
	if(orientation == G_ORIENTATION_HORIZONTAL)
	{
		MmeasSetMarker(m_Mark1, M_ORIENTATION, M_HORIZONTAL , M_NULL);
		MmeasSetMarker(m_Mark2, M_ORIENTATION, M_HORIZONTAL , M_NULL);
		OutputDebugStringA("@@@@Orientation Horizontal@@@@");
	}
	else if (orientation == G_ORIENTATION_VERTICAL)
	{
		MmeasSetMarker(m_Mark1, M_ORIENTATION, M_VERTICAL , M_NULL);
		MmeasSetMarker(m_Mark2, M_ORIENTATION, M_VERTICAL , M_NULL);
		OutputDebugStringA("@@@@Orientation Vertical@@@@");
	}
	else
	{
		MmeasSetMarker(m_Mark1, M_ORIENTATION, M_ANY , M_NULL);
		MmeasSetMarker(m_Mark2, M_ORIENTATION, M_ANY , M_NULL);
		OutputDebugStringA("@@@@Orientation Any@@@@");
	}
	MmeasSetMarker(m_Mark1, M_BOX_ORIGIN,m_rect1.nOffSetX,m_rect1.nOffSetY);
	MmeasSetMarker(m_Mark1, M_BOX_SIZE,m_rect1.nWidth,m_rect1.nHeight);
	MmeasSetMarker(m_Mark2, M_BOX_ORIGIN,m_rect2.nOffSetX,m_rect2.nOffSetY);
	MmeasSetMarker(m_Mark2, M_BOX_SIZE,m_rect2.nWidth,m_rect2.nHeight);

	char s_s[MAX_PATH];
	sprintf_s(s_s, MAX_PATH, "Point(%.3f,%.3f)->Point(%.3f,%.3f),Rect(%d,%d,%d,%d) and Rect(%d,%d,%d,%d)", 
		m_startPointX,m_startPointY,m_endPointX,m_endPointY,
		m_rect1.nOffSetX, m_rect1.nOffSetY, m_rect1.nWidth, m_rect1.nHeight, 
		m_rect2.nOffSetX, m_rect2.nOffSetY, m_rect2.nWidth, m_rect2.nHeight);
	OutputDebugStringA(s_s);
	return 0;
}
int EdgeClass::ModifyEdge()
{
	return 0;
}
int EdgeClass::FindEdge(int iEdgeIndex)
{
	double countNum;
	char s_s[MAX_PATH];
	MIL_ID childbuffer1,childbuffer2;
	MbufChild2d(m_pApp->m_CalImage, m_rect1.nOffSetX, m_rect1.nOffSetY, m_rect1.nWidth, m_rect1.nHeight, &childbuffer1);
	MbufChild2d(m_pApp->m_CalImage, m_rect2.nOffSetX, m_rect2.nOffSetY, m_rect2.nWidth, m_rect2.nHeight, &childbuffer2);
	MimHistogramEqualize(childbuffer1, childbuffer1, M_UNIFORM, M_NULL, 0, 255);
	MimHistogramEqualize(childbuffer2, childbuffer2, M_UNIFORM, M_NULL, 0, 255);
	//MimConvolve(childbuffer1, childbuffer1, M_SHEN_FILTER(M_EDGE_DETECT, 50));
	//MimConvolve(childbuffer2, childbuffer2, M_SHEN_FILTER(M_EDGE_DETECT, 50));
	MmeasFindMarker(M_DEFAULT, m_pApp->m_CalImage, m_Mark1, M_DEFAULT);
	MmeasFindMarker(M_DEFAULT, m_pApp->m_CalImage, m_Mark2, M_DEFAULT);
	MmeasGetResult(m_Mark1, M_NUMBER, &countNum, M_NULL);
	if (countNum <= 0.0)
	{
		m_point1.X = 0;
		m_point1.Y = 0;
		m_point2.X = 0;
		m_point2.Y = 0;
		m_InterPoint.X = (m_point1.X + m_point2.X) * 0.5;
		m_InterPoint.Y = (m_point1.Y + m_point2.Y) * 0.5;
		sprintf_s(s_s, MAX_PATH, "@@@Find %dth Edge Fail@@@\n", iEdgeIndex);
		OutputDebugStringA(s_s);
		return -1;
	}
	MmeasGetResult(m_Mark2, M_NUMBER, &countNum, M_NULL);
	if (countNum <= 0.0)
	{
		m_point1.X = 0;
		m_point1.Y = 0;
		m_point2.X = 0;
		m_point2.Y = 0;
		m_InterPoint.X = (m_point1.X + m_point2.X) * 0.5;
		m_InterPoint.Y = (m_point1.Y + m_point2.Y) * 0.5;
		sprintf_s(s_s, MAX_PATH, "@@@Find %dth Edge Fail@@@\n", iEdgeIndex);
		OutputDebugStringA(s_s);
		return -1;
	}
	MmeasGetResult(m_Mark1,M_POSITION,&(m_point1.X),&(m_point1.Y));
	MmeasGetResult(m_Mark2,M_POSITION,&(m_point2.X),&(m_point2.Y));
	double k1,b1,k2,b2;
	if(m_startPointX == m_endPointX && m_startPointY != m_endPointY)
	{
		if(m_point1.X == m_point2.X && m_point1.Y != m_point2.Y)
		{
			m_InterPoint.X = 0;
			m_InterPoint.Y = 0;
		}
		else if(m_point1.X != m_point2.X && m_point1.Y == m_point2.Y)
		{
			m_InterPoint.X = m_startPointX;
			m_InterPoint.Y = m_point1.Y;
		}
		else
		{
			m_InterPoint.X = m_startPointX;
			k1 = (m_point1.Y - m_point2.Y) / (m_point1.X - m_point2.X);
			b1 = m_point1.Y - k1 * m_point1.X;
			m_InterPoint.Y = k1 * m_InterPoint.X + b1;
		}
	}
	else if(m_startPointX != m_endPointX && m_startPointY == m_endPointY)
	{
		if(m_point1.X == m_point2.X && m_point1.Y != m_point2.Y)
		{
			m_InterPoint.X = m_point1.X;
			m_InterPoint.Y = m_startPointY;
		}
		else if(m_point1.X != m_point2.X && m_point1.Y == m_point2.Y)
		{
			m_InterPoint.X = 0;
			m_InterPoint.Y = 0;
		}
		else
		{
			m_InterPoint.Y = m_startPointY;
			k1 = (m_point1.Y - m_point2.Y) / (m_point1.X - m_point2.X);
			b1 = m_point1.Y - k1 * m_point1.X;
			m_InterPoint.X = (m_InterPoint.Y - b1) / k1;
		}
	}
	else
	{
		k1 = (m_endPointY - m_startPointY) / (m_endPointX - m_startPointX);
		b1 = m_endPointY - k1 * m_endPointX;
		if(m_point1.X == m_point2.X && m_point1.Y != m_point2.Y)
		{
			m_InterPoint.X = m_point1.X;
			m_InterPoint.Y = k1 * m_InterPoint.X + b1;
		}
		else if(m_point1.X != m_point2.X && m_point1.Y == m_point2.Y)
		{
			m_InterPoint.Y = m_point1.Y;
			m_InterPoint.X = (m_InterPoint.Y - b1) / k1;
		}
		else
		{
			k2 = (m_point1.Y - m_point2.Y) / (m_point1.X - m_point2.X);
			b2 = m_point1.Y - k2 * m_point1.X;
			m_InterPoint.X = (b2 - b1) / (k1 - k2);
			m_InterPoint.Y = (k1 * b2 - k2 * b1) / (k1 - k2);
		}
	}
	m_InterPoint.X = (m_point1.X + m_point2.X) * 0.5;
	m_InterPoint.Y = (m_point1.Y + m_point2.Y) * 0.5;
	sprintf_s(s_s, MAX_PATH, "Point1=(%.3f,%.3f), Point2=(%.3f,%.3f), InterPoint=(%.3f,%.3f)\n", 
		m_point1.X, m_point1.Y, m_point2.X, m_point2.Y,m_InterPoint.X,m_InterPoint.Y);
	OutputDebugStringA(s_s);
	MbufFree(childbuffer1);
	MbufFree(childbuffer2);
	return 0;
}
int EdgeClass::UinitEdge()
{
	if(m_Mark1)
	{
		MmeasFree(m_Mark1);
		m_Mark1 = M_NULL;
	}
	if(m_Mark2)
	{
		MmeasFree(m_Mark2);
		m_Mark2 = M_NULL;
	}
	return 0;
}







///////////////////////////////////////////////////////////////////////////////
DefectClass::DefectClass()
{
	m_pApp = NULL;
	m_MilBlob = M_NULL;
	m_MilBlobResult = M_NULL;
	m_MilImage = M_NULL;
	m_Width = 0;
	m_Height = 0;
	m_Byteline = 0;
	m_pImage = NULL;
	m_pGrayImage = NULL;
	m_pAnalysisImage = NULL;
	m_ChildAnalysis = M_NULL;
	m_MilChildImage = M_NULL;
	m_GrayImage = M_NULL;
	m_AnalysisImage = M_NULL;
	m_blobnum = 0;
	m_pBlobArea = NULL;
	m_pBlobMeanPixel = NULL;
	m_pBlobPosX = NULL;
	m_pBlobPosY = NULL;
	m_pBlobLeft = NULL;
	m_pBlobRight = NULL;
	m_pBlobTop = NULL;
	m_pBlobBottom = NULL;
	m_pBlobWidth = NULL;
	m_pBlobHeight = NULL;
	m_HistLowThreshold = 0;
	m_HistHighThreshold = 0;
	m_HistOpenValue = 0;
	m_HistCloseValue = 0;
	m_SelectOpenValue = 0;
	m_BinaryValue = 0;
	m_BlobLeastArea = 0;
	m_BlobLeastMeanValue = 0;
	m_BlobLeastWidth = 0;
	m_BlobLeastHeight = 0;
	m_bInitDefect = FALSE;
	m_pBlobArea_cur = NULL;
	m_pBlobMeanPixel_cur = NULL;
	m_pBlobPosX_cur = NULL;
	m_pBlobPosY_cur = NULL;
	m_pBlobLeft_cur = NULL;
	m_pBlobRight_cur = NULL;
	m_pBlobTop_cur = NULL;
	m_pBlobBottom_cur = NULL;
	m_pBlobWidth_cur = NULL;
	m_pBlobHeight_cur = NULL;
	OutputDebugStringA("*****************DefectClass Constructor*******************");
}
DefectClass::~DefectClass()
{
	UinitDefect();
	OutputDebugStringA("******************DefectClass Destructor**************************");
}
int DefectClass::InitDefect(AppClass* pA, int nWidth, int nHeight)
{
	if (m_bInitDefect)
		return 0;
	m_pApp = pA;
	m_Width = nWidth;
	m_Height = nHeight;
	m_Byteline = (m_Width + 3) / 4 * 4;
	m_pImage = (unsigned char*)malloc(m_Byteline * m_Height);
	m_pGrayImage = (unsigned char*)malloc(m_Byteline * m_Height);
	m_pAnalysisImage = (unsigned char*)malloc(m_Byteline * m_Height);
	MbufCreate2d(m_pApp->m_Sys, m_Width, m_Height, M_UNSIGNED + 8, M_PROC + M_IMAGE + M_DISP, M_HOST_ADDRESS + M_PITCH, m_Byteline, (void*)m_pImage, &m_MilImage);
	MbufCreate2d(m_pApp->m_Sys, m_Width, m_Height, M_UNSIGNED + 8, M_PROC + M_IMAGE + M_DISP, M_HOST_ADDRESS + M_PITCH, m_Byteline, (void*)m_pGrayImage, &m_GrayImage);
	MbufCreate2d(m_pApp->m_Sys, m_Width, m_Height, M_UNSIGNED + 8, M_PROC + M_IMAGE + M_DISP, M_HOST_ADDRESS + M_PITCH, m_Byteline, (void*)m_pAnalysisImage, &m_AnalysisImage);
	MbufChild2d(m_MilImage, 1850, 500, 5300, 9800, &m_MilChildImage);
	MbufChild2d(m_AnalysisImage, 1850, 500, 5300, 9800, &m_ChildAnalysis);
	MblobAllocFeatureList(m_pApp->m_Sys, &m_MilBlob);
	MblobAllocResult(m_pApp->m_Sys, &m_MilBlobResult);
	MblobSelectFeature(m_MilBlob, M_AREA);
	MblobSelectFeature(m_MilBlob, M_BOX);
	MblobSelectFeature(m_MilBlob, M_CENTER_OF_GRAVITY);
	MblobSelectFeature(m_MilBlob, M_MEAN_PIXEL + M_GRAYSCALE);
	m_HistLowThreshold = m_pApp->m_DefectInitPara.m_HistLowThreshold;
	m_HistHighThreshold = m_pApp->m_DefectInitPara.m_HistHighThreshold;
	m_HistOpenValue = m_pApp->m_DefectInitPara.m_HistOpenValue;
	m_HistCloseValue = m_pApp->m_DefectInitPara.m_HistCloseValue;
	m_SelectOpenValue = m_pApp->m_DefectInitPara.m_SelectOpenValue;
	m_BinaryValue = m_pApp->m_DefectInitPara.m_BinaryValue;
	m_BlobLeastArea = m_pApp->m_DefectInitPara.m_BlobLeastArea;
	m_BlobLeastMeanValue = m_pApp->m_DefectInitPara.m_BlobLeastMeanValue;
	m_BlobLeastWidth = m_pApp->m_DefectInitPara.m_BlobLeastWidth;
	m_BlobLeastHeight = m_pApp->m_DefectInitPara.m_BlobLeastHeight;
	m_BlobMaxArea = m_pApp->m_DefectInitPara.m_BlobMaxArea;
	char debugstr[MAX_PATH];
	sprintf_s(debugstr, MAX_PATH, "m_HistLowThreshold=%d, m_HistHighThreshold=%d, m_HistOpenValue=%d, m_HistCloseValue=%d, m_SelectOpenValue=%d,m_BinaryValue=%d\n",
		m_HistLowThreshold, m_HistHighThreshold, m_HistOpenValue, m_HistCloseValue, m_SelectOpenValue, m_BinaryValue);
	OutputDebugStringA(debugstr);
	sprintf_s(debugstr, MAX_PATH, "m_BlobLeastArea=%d, m_BlobLeastMeanValue=%d, m_BlobLeastWidth=%d, m_BlobLeastHeight=%d, m_BlobMaxArea=%d\n",
		m_BlobLeastArea, m_BlobLeastMeanValue, m_BlobLeastWidth, m_BlobLeastHeight, m_BlobMaxArea);
	OutputDebugStringA(debugstr);
	m_bInitDefect = TRUE;
	return 0;
}
int DefectClass::UinitDefect()
{
	if (m_pBlobArea)
	{
		free(m_pBlobArea);
		m_pBlobArea = NULL;
	}
	if (m_pBlobMeanPixel)
	{
		free(m_pBlobMeanPixel);
		m_pBlobMeanPixel = NULL;
	}
	if (m_pBlobPosX)
	{
		free(m_pBlobPosX);
		m_pBlobPosX = NULL;
	}
	if (m_pBlobPosY)
	{
		free(m_pBlobPosY);
		m_pBlobPosY = NULL;
	}
	if (m_pBlobLeft)
	{
		free(m_pBlobLeft);
		m_pBlobLeft = NULL;
	}
	if (m_pBlobRight)
	{
		free(m_pBlobRight);
		m_pBlobRight = NULL;
	}
	if (m_pBlobTop)
	{
		free(m_pBlobTop);
		m_pBlobTop = NULL;
	}
	if (m_pBlobBottom)
	{
		free(m_pBlobBottom);
		m_pBlobBottom = NULL;
	}
	if (m_pBlobWidth)
	{
		free(m_pBlobWidth);
		m_pBlobWidth = NULL;
	}
	if (m_pBlobHeight)
	{
		free(m_pBlobHeight);
		m_pBlobHeight = NULL;
	}
	//////////////////
	if (m_pBlobArea_cur)
	{
		free(m_pBlobArea_cur);
		m_pBlobArea_cur = NULL;
	}
	if (m_pBlobMeanPixel_cur)
	{
		free(m_pBlobMeanPixel_cur);
		m_pBlobMeanPixel_cur = NULL;
	}
	if (m_pBlobPosX_cur)
	{
		free(m_pBlobPosX_cur);
		m_pBlobPosX_cur = NULL;
	}
	if (m_pBlobPosY_cur)
	{
		free(m_pBlobPosY_cur);
		m_pBlobPosY_cur = NULL;
	}
	if (m_pBlobLeft_cur)
	{
		free(m_pBlobLeft_cur);
		m_pBlobLeft_cur = NULL;
	}
	if (m_pBlobRight_cur)
	{
		free(m_pBlobRight_cur);
		m_pBlobRight_cur = NULL;
	}
	if (m_pBlobTop_cur)
	{
		free(m_pBlobTop_cur);
		m_pBlobTop_cur = NULL;
	}
	if (m_pBlobBottom_cur)
	{
		free(m_pBlobBottom_cur);
		m_pBlobBottom_cur = NULL;
	}
	if (m_pBlobWidth_cur)
	{
		free(m_pBlobWidth_cur);
		m_pBlobWidth_cur = NULL;
	}
	if (m_pBlobHeight_cur)
	{
		free(m_pBlobHeight_cur);
		m_pBlobHeight_cur = NULL;
	}
	//////////////////
	if (m_pImage)
	{
		free(m_pImage);
		m_pImage = NULL;
	}
	if (m_pGrayImage)
	{
		free(m_pGrayImage);
		m_pGrayImage = NULL;
	}
	if (m_pAnalysisImage)
	{
		free(m_pAnalysisImage);
		m_pAnalysisImage = NULL;
	}
	if (m_MilChildImage)
	{
		MbufFree(m_MilChildImage);
		m_MilChildImage = M_NULL;
	}
	if (m_ChildAnalysis)
	{
		MbufFree(m_ChildAnalysis);
		m_ChildAnalysis = M_NULL;
	}
	if (m_MilImage)
	{
		MbufFree(m_MilImage);
		m_MilImage = M_NULL;
	}
	if (m_GrayImage)
	{
		MbufFree(m_GrayImage);
		m_GrayImage = M_NULL;
	}
	if (m_AnalysisImage)
	{
		MbufFree(m_AnalysisImage);
		m_AnalysisImage = M_NULL;
	}
	if (m_MilBlobResult)
	{
		MblobFree(m_MilBlobResult);
		m_MilBlobResult = M_NULL;
	}
	if (m_MilBlob)
	{
		MblobFree(m_MilBlob);
		m_MilBlob = M_NULL;
	}
	m_bInitDefect = FALSE;
	OutputDebugStringA("********UinitDefect**********");
	return 0;
}
int DefectClass::CalculateDefect(ImageInfo imageinfo, ImageInfo imagemask)
{
	if (imageinfo.Buffer == NULL || imageinfo.Height != m_Height || imageinfo.Width != m_Width || imageinfo.BitsPerPixel != 8
		||imagemask.Buffer == NULL || imagemask.Height != m_Height || imagemask.Width != m_Width || imagemask.BitsPerPixel != 8)//20141222 gtc add
		return -1;
	if (m_pBlobArea)
	{
		free(m_pBlobArea);
		m_pBlobArea = NULL;
	}
	if (m_pBlobMeanPixel)
	{
		free(m_pBlobMeanPixel);
		m_pBlobMeanPixel = NULL;
	}
	if (m_pBlobPosX)
	{
		free(m_pBlobPosX);
		m_pBlobPosX = NULL;
	}
	if (m_pBlobPosY)
	{
		free(m_pBlobPosY);
		m_pBlobPosY = NULL;
	}
	if (m_pBlobLeft)
	{
		free(m_pBlobLeft);
		m_pBlobLeft = NULL;
	}
	if (m_pBlobRight)
	{
		free(m_pBlobRight);
		m_pBlobRight = NULL;
	}
	if (m_pBlobTop)
	{
		free(m_pBlobTop);
		m_pBlobTop = NULL;
	}
	if (m_pBlobBottom)
	{
		free(m_pBlobBottom);
		m_pBlobBottom = NULL;
	}
	if (m_pBlobWidth)
	{
		free(m_pBlobWidth);
		m_pBlobWidth = NULL;
	}
	if (m_pBlobHeight)
	{
		free(m_pBlobHeight);
		m_pBlobHeight = NULL;
	}
	//////////////////////////
	if (m_pBlobArea_cur)
	{
		free(m_pBlobArea_cur);
		m_pBlobArea_cur = NULL;
	}
	if (m_pBlobMeanPixel_cur)
	{
		free(m_pBlobMeanPixel_cur);
		m_pBlobMeanPixel_cur = NULL;
	}
	if (m_pBlobPosX_cur)
	{
		free(m_pBlobPosX_cur);
		m_pBlobPosX_cur = NULL;
	}
	if (m_pBlobPosY_cur)
	{
		free(m_pBlobPosY_cur);
		m_pBlobPosY_cur = NULL;
	}
	if (m_pBlobLeft_cur)
	{
		free(m_pBlobLeft_cur);
		m_pBlobLeft_cur = NULL;
	}
	if (m_pBlobRight_cur)
	{
		free(m_pBlobRight_cur);
		m_pBlobRight_cur = NULL;
	}
	if (m_pBlobTop_cur)
	{
		free(m_pBlobTop_cur);
		m_pBlobTop_cur = NULL;
	}
	if (m_pBlobBottom_cur)
	{
		free(m_pBlobBottom_cur);
		m_pBlobBottom_cur = NULL;
	}
	if (m_pBlobWidth_cur)
	{
		free(m_pBlobWidth_cur);
		m_pBlobWidth_cur = NULL;
	}
	if (m_pBlobHeight_cur)
	{
		free(m_pBlobHeight_cur);
		m_pBlobHeight_cur = NULL;
	}
	//////////////////////////
	////MbufCopy(m_pApp->m_CalImage, m_MilImage);
	//memcpy_s(m_pImage, m_Byteline * m_Height, imageinfo.Buffer, m_Byteline * m_Height);
	//MbufCopy(m_MilImage, m_GrayImage);
	////MbufClear(m_MilImage, 0);
	//MbufClear(m_AnalysisImage, 0);
	////MbufClear(m_GrayImage, 0);
	///////////////////////////////////////////////////////////
	////MimHistogramEqualize(m_MilChildImage,m_MilChildImage,M_UNIFORM, M_NULL, 0, 255);
	//MbufCopy(m_MilChildImage, m_ChildAnalysis);
	//MbufClear(m_MilImage, 0);
	//MbufCopy(m_AnalysisImage, m_MilImage);
	//MbufClear(m_AnalysisImage, 0);
	//MimBinarize(m_MilImage, m_MilImage, M_IN_RANGE, m_HistLowThreshold, m_HistHighThreshold);
	//MimClose(m_MilImage, m_MilImage, 2, M_BINARY);
	//MimMorphic(m_MilImage, m_MilImage, M_3X3_RECT, M_AREA_CLOSE, m_HistCloseValue, M_BINARY);
	//MimErode(m_MilImage, m_MilImage, 2, M_BINARY);
	//MimMorphic(m_MilImage, m_MilImage, M_3X3_RECT, M_AREA_OPEN, m_HistOpenValue, M_BINARY);
	////MimMorphic(m_MilImage,m_MilImage, M_3X3_RECT, M_AREA_CLOSE, m_HistCloseValue, M_BINARY);
	///////////////////////////////////////////////////////////
	////MbufCopy(m_MilChildImage, m_ChildAnalysis);
	////MimClose(m_AnalysisImage,m_AnalysisImage,1,M_BINARY);
	////MimErode(m_AnalysisImage,m_AnalysisImage,1,M_BINARY);
	////MimMorphic(m_AnalysisImage,m_AnalysisImage,M_3X3_RECT,M_AREA_OPEN,m_SelectOpenValue,M_BINARY);
	////MbufClear(m_MilImage, 0);
	////SelectFrame(m_pAnalysisImage, m_pImage, m_Width, m_Height);
	//ModifyFrame(m_pImage, m_Width, m_Height, 100);
	////MbufClear(m_AnalysisImage, 0);
	//for (int j = 0; j<m_Height; ++j)
	//{
	//	unsigned char* ptr = m_pImage + j * m_Byteline;
	//	unsigned char* ptrD = m_pAnalysisImage + j * m_Byteline;
	//	unsigned char* ptrR = m_pGrayImage + j * m_Byteline;
	//	for (int i = 0; i<m_Width; ++i)
	//	{
	//		if (*(ptr + i) == 255)
	//		{
	//			if (*(ptrR + i) < m_BinaryValue)
	//				*(ptrD + i) = 255;
	//			else
	//				*(ptrD + i) = 0;
	//		}
	//		else
	//		{
	//			*(ptrD + i) = 0;
	//		}
	//	}
	//} //20141222 gtc delete
	memcpy_s(m_pImage, m_Byteline * m_Height, imageinfo.Buffer, m_Byteline * m_Height); //20141222 gtc add
	memcpy_s(m_pAnalysisImage, m_Byteline * m_Height, imagemask.Buffer, m_Byteline * m_Height); //20141222 gtc add
	MimMorphic(m_AnalysisImage, m_AnalysisImage, M_3X3_RECT, M_AREA_OPEN, m_SelectOpenValue, M_BINARY);
	//ModifyFrame(m_pAnalysisImage, m_Width, m_Height, 100);
	//MimDilate(m_AnalysisImage, m_AnalysisImage, 1, M_BINARY);
	for (int j = 0; j<m_Height; ++j)
	{
		unsigned char* ptr = m_pImage + j * m_Byteline;
		unsigned char* ptrD = m_pAnalysisImage + j * m_Byteline;
		unsigned char* ptrR = m_pGrayImage + j * m_Byteline;
		for (int i = 0; i<m_Width; ++i)
		{
			if (*(ptrD + i) == 255)
			{
				if (*(ptr + i) < m_BinaryValue)
					*(ptrR + i) = 255;
				else
					*(ptrR + i) = 0;
			}
			else
			{
				*(ptrR + i) = 0;
			}
		}
	}
	//MblobCalculate(m_AnalysisImage, m_GrayImage, m_MilBlob, m_MilBlobResult);//20141222 gtc delete
	MblobCalculate(m_GrayImage, m_MilImage, m_MilBlob, m_MilBlobResult);
	MblobSelect(m_MilBlobResult, M_EXCLUDE, M_AREA, M_LESS_OR_EQUAL, m_BlobLeastArea, M_NULL);
	MblobSelect(m_MilBlobResult, M_EXCLUDE, M_MEAN_PIXEL, M_GREATER_OR_EQUAL, m_BlobLeastMeanValue, M_NULL);
	MblobGetNumber(m_MilBlobResult, &m_blobnum);
	int loop = 0, tempsum = 0;
	if (m_blobnum > 0)
	{
		m_pBlobArea = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobMeanPixel = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobPosX = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobPosY = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobLeft = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobRight = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobTop = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobBottom = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobWidth = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobHeight = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobArea_cur = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobMeanPixel_cur = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobPosX_cur = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobPosY_cur = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobLeft_cur = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobRight_cur = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobTop_cur = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobBottom_cur = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobWidth_cur = (double*)malloc(m_blobnum * sizeof(double));
		m_pBlobHeight_cur = (double*)malloc(m_blobnum * sizeof(double));
		MblobGetResult(m_MilBlobResult, M_AREA, m_pBlobArea);
		MblobGetResult(m_MilBlobResult, M_MEAN_PIXEL, m_pBlobMeanPixel);
		MblobGetResult(m_MilBlobResult, M_BOX_X_MAX, m_pBlobRight);
		MblobGetResult(m_MilBlobResult, M_BOX_X_MIN, m_pBlobLeft);
		MblobGetResult(m_MilBlobResult, M_BOX_Y_MAX, m_pBlobBottom);
		MblobGetResult(m_MilBlobResult, M_BOX_Y_MIN, m_pBlobTop);
		for (loop = 0; loop<m_blobnum; ++loop)
		{
			m_pBlobPosX[loop] = (m_pBlobLeft[loop] + m_pBlobRight[loop]) * 0.5;
			m_pBlobPosY[loop] = (m_pBlobTop[loop] + m_pBlobBottom[loop]) * 0.5;
			m_pBlobWidth[loop] = (m_pBlobRight[loop] - m_pBlobLeft[loop]) + 1.0;
			m_pBlobHeight[loop] = (m_pBlobBottom[loop] - m_pBlobTop[loop]) + 1.0;
		}
		for (loop = 0; loop<m_blobnum; ++loop)
		{
			if (m_pBlobWidth[loop] > m_BlobLeastWidth && m_pBlobHeight[loop] > m_BlobLeastHeight && m_pBlobArea[loop] < m_BlobMaxArea)
			{
				m_pBlobArea_cur[tempsum] = m_pBlobArea[loop];
				m_pBlobMeanPixel_cur[tempsum] = m_pBlobMeanPixel[loop];
				m_pBlobPosX_cur[tempsum] = m_pBlobPosX[loop];
				m_pBlobPosY_cur[tempsum] = m_pBlobPosY[loop];
				m_pBlobLeft_cur[tempsum] = m_pBlobLeft[loop];
				m_pBlobRight_cur[tempsum] = m_pBlobRight[loop];
				m_pBlobTop_cur[tempsum] = m_pBlobTop[loop];
				m_pBlobBottom_cur[tempsum] = m_pBlobBottom[loop];
				m_pBlobWidth_cur[tempsum] = m_pBlobWidth[loop];
				m_pBlobHeight_cur[tempsum] = m_pBlobHeight[loop];
				tempsum++;
			}
		}
	}
	m_blobnum = tempsum;
	if (m_pApp->m_bDebugSaveBmp)
	{
		MimArith(m_MilImage,m_AnalysisImage,m_GrayImage,M_AND);
		char tt_s[MAX_PATH];
		strcpy(tt_s, m_pApp->m_ModelPath);
		strcat(tt_s, "\\MaskFrame.bmp");
		MbufExportA(tt_s, M_BMP, m_GrayImage);
		char tt_ss[MAX_PATH];
		strcpy(tt_ss, m_pApp->m_ModelPath);
		strcat(tt_ss, "\\mask.bmp");
		MbufExportA(tt_ss, M_BMP, m_AnalysisImage);
	}
	return 0;
}
int DefectClass::ModifyFrame(unsigned char* pSour, int nWidth, int nHeight, int nMaxGap)
{
	if (pSour == NULL || nWidth <= 0 || nHeight <= 0)
		return -1;
	int nByteline = (nWidth + 3) / 4 * 4;
	int i, j, loop;
	unsigned char* pRow = NULL;

	int stRow = -1, edRow = -1;
	for (j = 625; j<825; ++j)
	{
		pRow = pSour + j * nByteline;
		stRow = -1;
		edRow = -1;
		for (i = 2700; i<6300; ++i)
		{
			if (*(pRow + i) == 0 && *(pRow + i - 1) == 255) // 1 0
			{
				stRow = i;
			}
			else if (*(pRow + i) == 255 && *(pRow + i - 1) == 0 && stRow != -1) // 0 1
			{
				edRow = i;
				if (edRow - stRow < nMaxGap)
				{
					for (loop = stRow; loop <= edRow; ++loop)
					{
						*(pRow + loop) = 255;
					}
				}
				stRow = -1;
				edRow = -1;
			}
		}
	}
	for (j = 10100; j<10300; ++j)
	{
		pRow = pSour + j * nByteline;
		stRow = -1;
		edRow = -1;
		for (i = 2700; i<6300; ++i)
		{
			if (*(pRow + i) == 0 && *(pRow + i - 1) == 255) // 1 0
			{
				stRow = i;
			}
			else if (*(pRow + i) == 255 && *(pRow + i - 1) == 0 && stRow != -1) // 0 1
			{
				edRow = i;
				if (edRow - stRow < nMaxGap)
				{
					for (loop = stRow; loop <= edRow; ++loop)
					{
						*(pRow + loop) = 255;
					}
				}
				stRow = -1;
				edRow = -1;
			}
		}
	}
	for (j = 1950; j<2250; ++j)
	{
		stRow = -1;
		edRow = -1;
		for (i = 1500; i<9500; ++i)
		{
			if (*(pSour + i * nByteline + j) == 0 && *(pSour + (i - 1) * nByteline + j) == 255) // 1 0
			{
				stRow = i;
			}
			else if (*(pSour + i * nByteline + j) == 255 && *(pSour + (i - 1) * nByteline + j) == 0 && stRow != -1) // 0 1
			{
				edRow = i;
				if (edRow - stRow < nMaxGap)
				{
					for (loop = stRow; loop <= edRow; ++loop)
					{
						*(pSour + loop * nByteline + j) = 255;
					}
				}
				stRow = -1;
				edRow = -1;
			}
		}
	}
	for (j = 6750; j<7150; ++j)
	{
		stRow = -1;
		edRow = -1;
		for (i = 1500; i<9500; ++i)
		{
			if (*(pSour + i * nByteline + j) == 0 && *(pSour + (i - 1) * nByteline + j) == 255) // 1 0
			{
				stRow = i;
			}
			else if (*(pSour + i * nByteline + j) == 255 && *(pSour + (i - 1) * nByteline + j) == 0 && stRow != -1) // 0 1
			{
				edRow = i;
				if (edRow - stRow < nMaxGap)
				{
					for (loop = stRow; loop <= edRow; ++loop)
					{
						*(pSour + loop * nByteline + j) = 255;
					}
				}
				stRow = -1;
				edRow = -1;
			}
		}
	}
	return 0;
}