#pragma once

#ifdef HOUGHDLL_EXPORTS
#define HOUGHDLL_API __declspec(dllexport)
#else
#define HOUGHDLL_API __declspec(dllimport)
#endif

#include "./../Hdcmvbyd/defstruct.h"

#ifdef __cplusplus
extern "C"{
#endif

HOUGHDLL_API Houghlineobj __stdcall HoughLine(ImageInfo &iSour, HoughSize iSize, int nMaxDegree, int nMinDegree, int nDegreeDif, int nSlopeDif, size_t nLeastCount);
HOUGHDLL_API Houghcircleobj __stdcall HoughCircle(ImageInfo &iSour, HoughSize iSize, int nOuterRadius, int nInnerRadius, double fPosDif, double fRadDif, double ratio);
HOUGHDLL_API int __stdcall SaveLineBmp(char* pFilePath, ImageInfo &iDest, Houghlineobj obj);
HOUGHDLL_API int __stdcall SaveCircleBmp(char* pFilePath, ImageInfo &iDest, Houghcircleobj obj);
HOUGHDLL_API int __stdcall OstuBinary(ImageInfo &iSour, HoughSize iSize, int* nThreshold);
HOUGHDLL_API int __stdcall SelectFrame(unsigned char* pSour, unsigned char* pDest, int nWidth, int nHeight);

#ifdef __cplusplus
}
#endif