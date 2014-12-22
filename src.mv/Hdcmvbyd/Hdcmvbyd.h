#pragma once

#ifdef HDCMVBYD_EXPORTS
#define HDCMVBYDINSPECT_API __declspec(dllexport)
#else
#define HDCMVBYDINSPECT_API __declspec(dllimport)
#endif

#include "defstruct.h"


extern "C" HDCMVBYDINSPECT_API int __stdcall InitApp(int width, int height);

extern "C" HDCMVBYDINSPECT_API int __stdcall AddEdgeDefinition(
		_In_ double startPointX,
		_In_ double startPointY,
		_In_ double endPointX,
		_In_ double endPointY,
		_In_ double roiWidth, 
		_In_ int polarity,
		_In_ int orientation
		);
extern "C" HDCMVBYDINSPECT_API int __stdcall AddCircleDefinition(
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
		);
extern "C" HDCMVBYDINSPECT_API int __stdcall GetEdgeDefinitionsCount(_Out_ int* count);
extern "C" HDCMVBYDINSPECT_API int __stdcall GetCircleDefinitionsCount(_Out_ int* count);
extern "C" HDCMVBYDINSPECT_API int __stdcall CleanDefinitions();

extern "C" HDCMVBYDINSPECT_API int __stdcall CalibrateImage(_In_ ImageInfo originalImageInfo,_Out_ ImageInfo* calibratedImageInfo);

extern "C" HDCMVBYDINSPECT_API int __stdcall Calculate(_In_ ImageInfo imageInfo);

extern "C" HDCMVBYDINSPECT_API int __stdcall GetEdgeResult(_In_ int index,_Out_ Line* edgeLine,_Out_ Point* intersectionPoint);
extern "C" HDCMVBYDINSPECT_API int __stdcall GetCircleResult(_In_ int index,_Out_ Circle* foundCircle,_Out_ int* roundness);

extern "C" HDCMVBYDINSPECT_API int __stdcall CreateRelativeCoordinate(_In_ Line baseLine,_In_ double angle);

extern "C" HDCMVBYDINSPECT_API int __stdcall GetDistanceBetweenLines(
		_In_ Line line1,
		_In_ Line line2,
		_Out_ double* distanceInPixel,
		_Out_ double* distanceInWorld,
		_Out_ double* angle
		);
extern "C" HDCMVBYDINSPECT_API int __stdcall GetDistanceBetweenPoints(
		_In_ Point point1,
		_In_ Point point2,
		_Out_ double* distanceInPixel,
		_Out_ double* distanceInWorld,
		_Out_ double* angle
		);

extern "C" HDCMVBYDINSPECT_API int __stdcall GetDistancePointToLine(
		_In_ Point point,
		_In_ Line  line,
		_Out_ double* distanceInPixel,
		_Out_ double* distanceInWorld,
		_Out_ Point* pt
		);

extern "C" HDCMVBYDINSPECT_API int __stdcall FreeApp();

extern "C" HDCMVBYDINSPECT_API int __stdcall GetRelativeCoordinatePoint(Point originpoint, _Out_ Point* relativepoint);

extern "C" HDCMVBYDINSPECT_API int __stdcall InspectCalculate(_In_ ImageInfo imageinfo, _In_ ImageInfo imagemask, _Out_ InspectInfo* pInspect);//20141218 gtc add

extern "C" HDCMVBYDINSPECT_API int __stdcall GetInspectDefect(_In_ int index, _Out_ DefectInfo* pDefectDetail);//20141218 gtc add

