// Hdcmvbyd.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "Hdcmvbyd.h"
#include "MvClass.h"

AppClass* pClass = NULL;
int __stdcall InitApp(int width, int height)
{
	pClass = new AppClass;
	int retval = pClass->InitMil(width, height);
	return retval;
}


int __stdcall AddEdgeDefinition(
	_In_ double startPointX,
	_In_ double startPointY,
	_In_ double endPointX,
	_In_ double endPointY,
	_In_ double roiWidth, // half of width
	_In_ int polarity,
	_In_ int orientation
	)
{
	int retval=0;
	char s_s[MAX_PATH];
	if (pClass->m_CurEdgeIndex >= pClass->m_EdgeClassNumber)
	{
		sprintf_s(s_s, "AddEdgeDefinition_1 Return %d,(%d,%d)\n", -1, pClass->m_CurEdgeIndex, pClass->m_EdgeClassNumber);
		OutputDebugStringA(s_s);
		return -1;
	}
	pClass->InitDefinitions();
	retval = pClass->pEdgeClass[pClass->m_CurEdgeIndex].InitEdge(pClass,startPointX,startPointY,endPointX,endPointY,roiWidth,polarity,orientation);
	sprintf_s(s_s, "AddEdgeDefinition_2 Return %d,(%d,%d)\n", -1, pClass->m_CurEdgeIndex, pClass->m_EdgeClassNumber);
	OutputDebugStringA(s_s);
	pClass->m_CurEdgeIndex++;
	return retval;
}

 
int __stdcall AddCircleDefinition(
	_In_ double circleCenterX,
	_In_ double circleCenterY,
	_In_ double innerCircleRadius,
	_In_ double outerCircleRadius,
	_In_ int lowThreshold,
	_In_ int highThreshold,
	_In_ double fPosDiff,
	_In_ double fRadDiff,
	_In_ double fRatio,
	_In_ int processtype
	)
{
	int retval=0;
	char s_s[100];
	if(pClass->m_CurCircleIndex >= pClass->m_CircleClassNumber)
	{
		sprintf_s(s_s, "AddCirleDefinition_1 Return %d,(%d,%d)\n", -1, pClass->m_CurCircleIndex, pClass->m_CircleClassNumber);
		OutputDebugStringA(s_s);
		return -1;
	}
	pClass->InitDefinitions();
	retval = pClass->pCircleClass[pClass->m_CurCircleIndex].InitCircle(pClass,
				pClass->m_ImageWidth,
				pClass->m_ImageHeight,
				pClass->m_pCal, //calibrated image ptr
				circleCenterX,
				circleCenterY,
				innerCircleRadius,
				outerCircleRadius,
				lowThreshold,
				highThreshold,
				fPosDiff,
				fRadDiff,
				fRatio,
				processtype);
	sprintf_s(s_s, "AddCirleDefinition_2 Return %d,(%d,%d)\n", retval, pClass->m_CurCircleIndex, pClass->m_CircleClassNumber);
	OutputDebugStringA(s_s);
	pClass->m_CurCircleIndex++;
	return retval;
}


int __stdcall GetEdgeDefinitionsCount(_Out_ int* count)
{
	*count = pClass->m_CurEdgeIndex; 
	if(*count > pClass->m_EdgeClassNumber)
		*count = pClass->m_EdgeClassNumber;
	return 0;
}


int __stdcall GetCircleDefinitionsCount(_Out_ int* count)
{
	*count = pClass->m_CurCircleIndex; 
	if(*count > pClass->m_CircleClassNumber)
		*count = pClass->m_CircleClassNumber;
	return 0;
}


int __stdcall CleanDefinitions()
{
	int retval = pClass->FreeDefinitions();
	if(retval != 0)
		OutputDebugStringA("****CleanDefinitions Function Fail****");
	return retval;
}


int __stdcall CalibrateImage(_In_ ImageInfo originalImageInfo,_Out_ ImageInfo* calibratedImageInfo)
{
	int retval;
	retval = pClass->CalibrateOriginImage(originalImageInfo,calibratedImageInfo);
	return retval;
}


int __stdcall Calculate(_In_ ImageInfo imageInfo)
{
	int retval,i;
	char s_s[MAX_PATH];
	if(imageInfo.Buffer == NULL || imageInfo.Width != pClass->m_ImageWidth
		|| imageInfo.Height != pClass->m_ImageHeight)
	{
		OutputDebugStringA("**********Calculate Function Input Wrong ImageInfo*********");
		return -1;
	}
	memcpy_s(pClass->m_pCal,pClass->m_ImageHeight * pClass->m_ImageByteLine,imageInfo.Buffer,pClass->m_ImageHeight * pClass->m_ImageByteLine);
	sprintf_s(s_s,MAX_PATH,"No#%02d*******memcpy_s over**********",pClass->m_CurCircleIndex);
	OutputDebugStringA(s_s);
	for(i=0;i<pClass->m_CurCircleIndex;++i)
	{
		retval = pClass->pCircleClass[i].FindCircle(i);
		if(retval != 0)
		{
			sprintf_s(s_s,MAX_PATH,"****Calculate Function No#%02d FindCircle Fail****",i);
			OutputDebugStringA(s_s);
			return retval;
		}
	}
	OutputDebugStringA("********CircleFind Over*********");
	for(i=0;i<pClass->m_CurEdgeIndex;++i)
	{
		retval = pClass->pEdgeClass[i].FindEdge(i);
		if(retval != 0)
		{
			sprintf_s(s_s,MAX_PATH,"****Calculate Function No#%02d FindEdge Fail****",i);
			OutputDebugStringA(s_s);
			return retval;
		}
	}
	pClass->SaveCalBmp();
	return 0;
}


int __stdcall GetEdgeResult(_In_ int index,_Out_ Line* edgeLine,_Out_ Point* intersectionPoint)
{
	if(index >= pClass->m_CurEdgeIndex)
	{
		OutputDebugStringA("****GetEdgeResult Function Input Wrong [index]****");
		edgeLine->X1 = 0;
		edgeLine->X2 = 0;
		edgeLine->Y1 = 0;
		edgeLine->Y2 = 0;
		return -1;
	}
	edgeLine->X1 = pClass->pEdgeClass[index].m_point1.X;
	edgeLine->Y1 = pClass->pEdgeClass[index].m_point1.Y;
	edgeLine->X2 = pClass->pEdgeClass[index].m_point2.X;
	edgeLine->Y2 = pClass->pEdgeClass[index].m_point2.Y;
	*intersectionPoint = pClass->pEdgeClass[index].m_InterPoint;
	return 0;
}


int __stdcall GetCircleResult(_In_ int index,_Out_ Circle* foundCircle,_Out_ int* roundness)
{
	if(index >= pClass->m_CurCircleIndex)
	{
		OutputDebugStringA("****GetCircleResult Function Input Wrong [index]****");
		memset(foundCircle, 0, sizeof(Circle));
		*roundness = 0;
		return -1;
	}
	if (pClass->pCircleClass[index].obj.center_x == 0
		|| pClass->pCircleClass[index].obj.center_y == 0
		|| pClass->pCircleClass[index].obj.circle_rad == 0
		|| pClass->pCircleClass[index].obj.circleCount == 0)
	{
		OutputDebugStringA("****GetCircleResult Function Not Find Circle****");
		memset(foundCircle, 0, sizeof(Circle));
		*roundness = 0;
		return -1;
	}
	foundCircle->CenterX = pClass->pCircleClass[index].obj.center_x;
	foundCircle->CenterY = pClass->pCircleClass[index].obj.center_y;
	foundCircle->Radius = pClass->pCircleClass[index].obj.circle_rad;
	*roundness = pClass->pCircleClass[index].m_bExcuteBlobAnalysis;
	return 0;
} 


int __stdcall CreateRelativeCoordinate(_In_ Line baseLine,_In_ double angle)
{
	int retval = pClass->ToRelativeCoord(baseLine,angle);
	if(retval != 0)
		OutputDebugStringA("****CreateRelativeCoordinate Function Fail****");
	return retval;
}


int __stdcall GetDistanceBetweenLines(
	_In_ Line line1,
	_In_ Line line2,
	_Out_ double* distanceInPixel,
	_Out_ double* distanceInWorld,
	_Out_ double* angle
	)
{
	return 0;
}


int __stdcall GetDistanceBetweenPoints(
	_In_ Point point1,
	_In_ Point point2,
	_Out_ double* distanceInPixel,
	_Out_ double* distanceInWorld,
	_Out_ double* angle
	)
{
	return 0;
}


int __stdcall GetDistancePointToLine(
	_In_ Point point,
	_In_ Line  line,
	_Out_ double* distanceInPixel,
	_Out_ double* distanceInWorld,
	_Out_ Point* pt
	)
{
	return 0;
}


int __stdcall FreeApp()
{
	if(pClass)
	{
		pClass->UinitMil();
		delete pClass;
	}
	return 0;
}
int __stdcall GetRelativeCoordinatePoint(Point originpoint, _Out_ Point* relativepoint)
{
	*relativepoint = pClass->FromOriginToRelative(originpoint);
	return 0;
}

int __stdcall InspectCalculate(_In_ ImageInfo imageinfo, _In_ ImageInfo imagemask, _Out_ InspectInfo* pInspect)//20141218 gtc add
{
	char s_s[MAX_PATH];
	sprintf_s(s_s, MAX_PATH, "imageinfo.PixelWidth: %d", imageinfo.Width);
	OutputDebugStringA(s_s);
	sprintf_s(s_s, MAX_PATH, "imageinfo.PixelHeight: %d", imageinfo.Height);
	OutputDebugStringA(s_s);

	int retval = 0;
	if (imageinfo.Buffer == NULL || imageinfo.Width <= 0 || imageinfo.Height <= 0 || imageinfo.BitsPerPixel != 8
		|| pInspect == NULL || pClass == NULL)
		return -1;
	retval = pClass->pDefectClass->CalculateDefect(imageinfo, imagemask);
	if (retval != 0)
	{
		pInspect->HasError = -1;
		pInspect->DefectsCount = 0;
		pInspect->MeasurementsCount = 0;
		pInspect->Index = 0;
		pInspect->SurfaceTypeIndex = 0;
		return retval;
	}
	if (pClass->pDefectClass->m_blobnum > 0)
	{
		pInspect->HasError = 1;
		pInspect->DefectsCount = pClass->pDefectClass->m_blobnum;
		pInspect->MeasurementsCount = 0;
		pInspect->Index = 0;
		pInspect->SurfaceTypeIndex = 0;
	}
	else
	{
		pInspect->HasError = 0;
		pInspect->DefectsCount = 0;
		pInspect->MeasurementsCount = 0;
		pInspect->Index = 0;
		pInspect->SurfaceTypeIndex = 0;
	}
	return retval;
}
int __stdcall GetInspectDefect(_In_ int index, _Out_ DefectInfo* pDefectDetail)//20141218 gtc add
{
	if (pClass == NULL)
		return -1;
	if (index<0 || index > pClass->pDefectClass->m_blobnum - 1 || pDefectDetail == NULL)
		return -1;
	pDefectDetail->Index = index;
	pDefectDetail->TypeCode = 0;
	pDefectDetail->X = pClass->pDefectClass->m_pBlobPosX_cur[index];
	pDefectDetail->Y = pClass->pDefectClass->m_pBlobPosY_cur[index];
	pDefectDetail->Width = pClass->pDefectClass->m_pBlobWidth_cur[index];
	pDefectDetail->Height = pClass->pDefectClass->m_pBlobHeight_cur[index];
	pDefectDetail->Size = pClass->pDefectClass->m_pBlobArea_cur[index];
	pDefectDetail->X_Real = 0;
	pDefectDetail->Y_Real = 0;
	pDefectDetail->Width_Real = 0;
	pDefectDetail->Height_Real = 0;
	pDefectDetail->Size_Real = 0;
	return 0;
}

	
