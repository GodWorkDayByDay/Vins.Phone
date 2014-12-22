#ifndef _FUNCTIONINTERFACE_HHH_HHHHHHHHHH
#define _FUNCTIONINTERFACE_HHH_HHHHHHHHHH

#include <Windows.h>
#include <stdio.h>
#include "./../Hdcmvbyd/defstruct.h"

typedef int (*g_InitDllApp)(int,int);
typedef int (*g_AddEdge)(double, double, double, double, double, int, int);
typedef int (*g_AddCircle)(double, double, double, double, int, int,double,double,double,int);
typedef int (*g_GetEdgeCount)(int*);
typedef int (*g_GetCircleCount)(int*);
typedef int (*g_FreeDefinition)();
typedef int (*g_CalibrateImage)(ImageInfo, ImageInfo*);
typedef int (*g_Calculate)(ImageInfo);
typedef int (*g_GetEdgeResult)(int, Line*, Point*);
typedef int (*g_GetCircleResult)(int, Circle*, double*);
typedef int (*g_RelativeCoordinate)(Line, double);
typedef int (*g_DistanceOfTwoLines)(Line, Line, double*, double*, double*);
typedef int (*g_DistanceOfTwoPoints)(Point, Point, double*, double*, double*);
typedef int (*g_DistancePointToLine)(Point, Line, double*, double*, Point*);
typedef int (*g_FreeApp)();
typedef int (*g_GetRelativePoint)(Point,Point*);

////////////////////////////////////////////

class FunctionInterfaceClass
{
public:
	FunctionInterfaceClass()
	{
		GDLL_InitApp = NULL;
		GDLL_AddEdge = NULL;
		GDLL_AddCircle = NULL;
		GDLL_GetEdgeCount = NULL;
		GDLL_GetCircleCount = NULL;
		GDLL_FreeDefinition = NULL;
		GDLL_CalibrateImage = NULL;
		GDLL_Calculate = NULL;
		GDLL_GetEdgeResult = NULL;
		GDLL_GetCircleResult = NULL;
		GDLL_RelativeCoordinate = NULL;
		GDLL_DistanceOfLines = NULL;
		GDLL_DistanceOfPoints = NULL;
		GDLL_DistancePointToLine = NULL;
		GDLL_FreeApp = NULL;
		GDLL_GetRelativePoint = NULL;
	}
	~FunctionInterfaceClass()
	{

	}
public:
	g_InitDllApp			GDLL_InitApp;
	g_AddEdge				GDLL_AddEdge;
	g_AddCircle				GDLL_AddCircle;
	g_GetEdgeCount			GDLL_GetEdgeCount;
	g_GetCircleCount		GDLL_GetCircleCount;
	g_FreeDefinition		GDLL_FreeDefinition;
	g_CalibrateImage		GDLL_CalibrateImage;
	g_Calculate				GDLL_Calculate;
	g_GetEdgeResult			GDLL_GetEdgeResult;
	g_GetCircleResult		GDLL_GetCircleResult;
	g_RelativeCoordinate	GDLL_RelativeCoordinate;
	g_DistanceOfTwoLines	GDLL_DistanceOfLines;
	g_DistanceOfTwoPoints	GDLL_DistanceOfPoints;
	g_DistancePointToLine	GDLL_DistancePointToLine;
	g_FreeApp				GDLL_FreeApp;
	g_GetRelativePoint		GDLL_GetRelativePoint;
public:
	int LoadDll(HINSTANCE &hInstance, TCHAR* dllName)
	{
		hInstance = ::LoadLibrary(dllName);
		DWORD ret;
		char retstr[20];
		if(hInstance == NULL) 
		{
			ret = GetLastError();
			return ret;
		}
		GDLL_InitApp = (g_InitDllApp)GetProcAddress(hInstance,"InitApp");
		if(GDLL_InitApp == NULL) 
		{
			ret = GetLastError();
			sprintf(retstr,"err=%d\n",ret);
			printf(retstr);
			return ret;
		}
		GDLL_AddEdge = (g_AddEdge)GetProcAddress(hInstance,"AddEdgeDefinition");
		GDLL_AddCircle = (g_AddCircle)GetProcAddress(hInstance,"AddCircleDefinition");
		GDLL_GetEdgeCount = (g_GetEdgeCount)GetProcAddress(hInstance,"GetEdgeDefinitionsCount");
		GDLL_GetCircleCount = (g_GetCircleCount)GetProcAddress(hInstance,"GetCircleDefinitionsCount");
		GDLL_FreeDefinition = (g_FreeDefinition)GetProcAddress(hInstance,"CleanDefinitions");
		GDLL_CalibrateImage = (g_CalibrateImage)GetProcAddress(hInstance,"CalibrateImage");
		GDLL_Calculate = (g_Calculate)GetProcAddress(hInstance,"Calculate");
		GDLL_GetEdgeResult = (g_GetEdgeResult)GetProcAddress(hInstance,"GetEdgeResult");
		GDLL_GetCircleResult = (g_GetCircleResult)GetProcAddress(hInstance,"GetCircleResult");
		GDLL_RelativeCoordinate = (g_RelativeCoordinate)GetProcAddress(hInstance,"CreateRelativeCoordinate");
		GDLL_DistanceOfLines = (g_DistanceOfTwoLines)GetProcAddress(hInstance,"GetDistanceBetweenLines");
		GDLL_DistanceOfPoints = (g_DistanceOfTwoPoints)GetProcAddress(hInstance,"GetDistanceBetweenPoints");
		GDLL_DistancePointToLine = (g_DistancePointToLine)GetProcAddress(hInstance,"GetDistancePointToLine");
		GDLL_FreeApp = (g_FreeApp)GetProcAddress(hInstance,"FreeApp");
		GDLL_GetRelativePoint = (g_GetRelativePoint)GetProcAddress(hInstance,"GetRelativeCoordinatePoint");
		if(GDLL_InitApp == NULL
			||GDLL_AddEdge == NULL
			||GDLL_AddCircle == NULL
			||GDLL_GetEdgeCount == NULL
			||GDLL_GetCircleCount == NULL
			||GDLL_FreeDefinition == NULL
			||GDLL_CalibrateImage == NULL
			||GDLL_Calculate == NULL
			||GDLL_GetEdgeResult == NULL
			||GDLL_GetCircleResult == NULL
			||GDLL_RelativeCoordinate == NULL
			||GDLL_DistanceOfLines == NULL
			||GDLL_DistanceOfPoints == NULL
			||GDLL_DistancePointToLine == NULL
			||GDLL_FreeApp == NULL
			||GDLL_GetRelativePoint == NULL)
			return -1;
		return 0;
	}
	int FreeDll(HINSTANCE &hInstance)
	{
		int retval = ::FreeLibrary(hInstance);
		if(retval == 0) 
		{
			return -1;
		}
		else 
		{
			hInstance = NULL;
			return 0;
		}
	}
};

#endif