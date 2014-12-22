// bydDemo.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include "Interface.h"
#include <conio.h>
#include <math.h>

int ReadBmp(char* pFilePath, ImageInfo &iSour)
{
	FILE *fr;
	if(errno_t err = fopen_s(&fr,pFilePath,"rb") == 0)
	{
		BITMAPINFOHEADER bi;
		fseek(fr,14,SEEK_SET);
		fread(&bi,sizeof(BITMAPINFOHEADER),1,fr);
		iSour.BitsPerPixel = bi.biBitCount;
		iSour.Width = bi.biWidth;
		iSour.Height = bi.biHeight;
		iSour.Buffer = (unsigned char*)malloc(iSour.Height * ((iSour.Width * iSour.BitsPerPixel / 8 + 3) / 4 * 4));
		if(iSour.BitsPerPixel == 24)
		{
			fseek(fr,54,SEEK_SET);
		}
		else if(iSour.BitsPerPixel == 8)
		{
			fseek(fr,1078,SEEK_SET);
		}
		else
		{
			return -1;
		}
		fread(iSour.Buffer,sizeof(unsigned char),iSour.Height * ((iSour.Width * iSour.BitsPerPixel / 8 + 3) / 4 * 4),fr);
		fclose(fr);
		return 0;
	}
	return -1;
}
int SaveBmp(char* pFilePath, ImageInfo &iDest)
{
	FILE* fw;
	errno_t err = fopen_s(&fw,pFilePath,"wb");
	printf("save bmp err = %d\n",err);
	if(err != 0 || iDest.Buffer == NULL || iDest.Width <= 0 || iDest.Height <= 0 || iDest.BitsPerPixel <= 0) return -1;
	BITMAPFILEHEADER wBH;
	if(iDest.BitsPerPixel == 8)
		wBH.bfOffBits = 1078;
	else if(iDest.BitsPerPixel == 24)
		wBH.bfOffBits = 54;
	else 
		return -1;
	wBH.bfReserved1 = 0;
	wBH.bfReserved2 = 0;
	wBH.bfSize = 14;
	wBH.bfType = 0x4D42;
	BITMAPINFOHEADER wBI;
	wBI.biBitCount = iDest.BitsPerPixel;
	wBI.biClrImportant = 0;
	wBI.biClrUsed = 0;
	wBI.biCompression = BI_RGB;
	wBI.biHeight = iDest.Height;
	wBI.biPlanes = 1;
	wBI.biSize = 40;
	wBI.biSizeImage = 0;
	wBI.biWidth = iDest.Width;
	wBI.biXPelsPerMeter = 0;
	wBI.biYPelsPerMeter = 0;
	RGBQUAD rgbq[256];
	for(int i=0; i<256; ++i)
	{
		rgbq[i].rgbBlue = i;
		rgbq[i].rgbGreen = i;
		rgbq[i].rgbRed = i;
		rgbq[i].rgbReserved = 0;
	}
	fwrite(&wBH,sizeof(BITMAPFILEHEADER),1,fw);
	fwrite(&wBI,sizeof(BITMAPINFOHEADER),1,fw);
	if(iDest.BitsPerPixel == 8)
		fwrite(rgbq,sizeof(RGBQUAD), 256, fw);
	fwrite(iDest.Buffer,sizeof(unsigned char),iDest.Height * ((iDest.Width * iDest.BitsPerPixel / 8 + 3) / 4 * 4),fw);
	fclose(fw);
	return 0;
}
int _tmain(int argc, _TCHAR* argv[])
{
	FunctionInterfaceClass funInterface;
	HINSTANCE hInstance;
	TCHAR dllName[20];
	_tcscpy(dllName,_TEXT("Hdcmvbyd.dll"));
	int retval = funInterface.LoadDll(hInstance,dllName);
	printf("LoadDll function return %d\n",retval);
	//////////////////////////////////////////////////////////////////
	int ImageWidth = 8192, ImageHeight = 12500;
	retval = funInterface.GDLL_InitApp(ImageWidth,ImageHeight);
	printf("GDLL_InitApp function return %d\n",retval);
	/////////////////////////////////////////////////////////////////////
	ImageInfo iSour,iCal,iTemp,iColor;
	int i,j;
	retval = ReadBmp("D:\\00008.bmp", iSour);
	iTemp.BitsPerPixel = 8;
	iTemp.Buffer = (unsigned char*)malloc(ImageWidth * ImageHeight);
	iTemp.Height = ImageHeight;
	iTemp.Index = 0;
	iTemp.SurfaceTypeIndex = 0;
	iTemp.Width = ImageWidth;
	iColor.BitsPerPixel = 24;
	iColor.Buffer = (unsigned char*)malloc(ImageWidth * 3 * ImageHeight);
	iColor.Height = ImageHeight;
	iColor.Index = 0;
	iColor.SurfaceTypeIndex = 0;
	iColor.Width = ImageWidth;
	for(j=0; j<ImageHeight; ++j)
	{
		for(i=0; i<ImageWidth; ++i)
		{
			*(iTemp.Buffer + j * ImageWidth + i) = *(iSour.Buffer + (ImageHeight - 1 - j) * ImageWidth + i);
		}
	}
	memcpy_s(iSour.Buffer,ImageWidth * ImageHeight, iTemp.Buffer, ImageWidth * ImageHeight);
	printf("ReadBmp function return %d, Width=%d,Height=%d,Buffer=%d\n",retval,iSour.Width,iSour.Height,iSour.Buffer);
	retval = funInterface.GDLL_CalibrateImage(iSour,&iCal);
	printf("GDLL_CalibrateImage function return %d, Width=%d,Height=%d,Buffer=%d\n",retval,iCal.Width,iCal.Height,iCal.Buffer);
	for(j=0; j<ImageHeight; ++j)
	{
		for(i=0; i<ImageWidth; ++i)
		{
			*(iColor.Buffer + j * ImageWidth * 3 + i * 3) = *(iCal.Buffer + j * ImageWidth + i);
			*(iColor.Buffer + j * ImageWidth * 3 + i * 3 + 1) = *(iCal.Buffer + j * ImageWidth + i);
			*(iColor.Buffer + j * ImageWidth * 3 + i * 3 + 2) = *(iCal.Buffer + j * ImageWidth + i);
		}
	}
	retval = funInterface.GDLL_AddCircle(3340,4218,85,90,0,20,6.0,4.0,0.2,0);//5000 2900
	printf("GDLL_AddCircle function return %d\n",retval);
	retval = funInterface.GDLL_Calculate(iCal);
	printf("GDLL_Calculate function return %d\n",retval);
	retval = funInterface.GDLL_GetCircleCount(&i);
	printf("GDLL_GetCircleCount function return %d, Circle Number=%d\n",retval,i);
	Circle nCircle;
	double roundness;
	retval = funInterface.GDLL_GetCircleResult(0,&nCircle,&roundness);
	printf("GDLL_GetCircleCount function return %d   ",retval);
	printf("Circle:(%.3f,%.3f,%.3f),roundness=%.3f\n",nCircle.CenterX,nCircle.CenterY,nCircle.Radius,roundness);
	////////////////////////////////////////////////////////////////////
	int tempX,tempY;
	for(j=0; j<360; ++j)
	{
		tempX = (int)((double)(nCircle.CenterX) - (double)(nCircle.Radius) * cos((double)(j) / 180.0 * 3.1415926) + 0.5);
		tempY = (int)((double)(nCircle.CenterY) - (double)(nCircle.Radius) * sin((double)(j) / 180.0 * 3.1415926) + 0.5);
		*(iColor.Buffer + tempY * ImageWidth * 3 + tempX * 3) = 0;
		*(iColor.Buffer + tempY * ImageWidth * 3 + tempX * 3 + 1) = 255;
		*(iColor.Buffer + tempY * ImageWidth * 3 + tempX * 3 + 2) = 0;
	}
	for(j=(int)tempX-5;j<=(int)tempX+5;++j)
	{
		*(iColor.Buffer + tempY * ImageWidth * 3 + tempX * 3) = 0;
		*(iColor.Buffer + tempY * ImageWidth * 3 + tempX * 3 + 1) = 255;
		*(iColor.Buffer + tempY * ImageWidth * 3 + tempX * 3 + 2) = 0;
	}
	for(j=(int)tempY-5;j<=(int)tempY+5;++j)
	{
		*(iColor.Buffer + tempY * ImageWidth * 3 + tempX * 3) = 0;
		*(iColor.Buffer + tempY * ImageWidth * 3 + tempX * 3 + 1) = 255;
		*(iColor.Buffer + tempY * ImageWidth * 3 + tempX * 3 + 2) = 0;
	}
	printf("Save Color Bmp return %d\n",SaveBmp("E:\\BYD Image\\20141204bydimage\\mark.bmp",iColor));
	////////////////////////////////////////////////////////////////////////
	retval = funInterface.GDLL_FreeDefinition();
	printf("GDLL_FreeDefinition function return %d\n",retval);
	retval = funInterface.GDLL_GetCircleCount(&i);
	printf("GDLL_GetCircleCount function return %d, Circle Number=%d\n",retval,i);
	retval = funInterface.GDLL_GetCircleResult(0,&nCircle,&roundness);
	printf("GDLL_GetCircleCount function return %d   ",retval);
	printf("Circle:(%.3f,%.3f,%.3f),roundness=%.3f\n",nCircle.CenterX,nCircle.CenterY,nCircle.Radius,roundness);
	retval = funInterface.GDLL_Calculate(iCal);
	printf("GDLL_Calculate function return %d\n",retval);
	retval = funInterface.GDLL_GetCircleResult(0,&nCircle,&roundness);
	printf("GDLL_GetCircleCount function return %d   ",retval);
	printf("Circle:(%.3f,%.3f,%.3f),roundness=%.3f\n",nCircle.CenterX,nCircle.CenterY,nCircle.Radius,roundness);
	retval = funInterface.GDLL_AddCircle(3300,4325,85,100,0,130,6.0,4.0,0.2,1);
	printf("GDLL_AddCircle function return %d\n",retval);
	retval = funInterface.GDLL_Calculate(iCal);
	printf("GDLL_Calculate function return %d\n",retval);
	retval = funInterface.GDLL_GetCircleCount(&i);
	printf("GDLL_GetCircleCount function return %d, Circle Number=%d\n",retval,i);
	retval = funInterface.GDLL_GetCircleResult(0,&nCircle,&roundness);
	printf("GDLL_GetCircleCount function return %d   ",retval);
	printf("Circle:(%.3f,%.3f,%.3f),roundness=%.3f\n",nCircle.CenterX,nCircle.CenterY,nCircle.Radius,roundness);
	Point pt,pt2;
	pt.X = 100;
	pt.Y = 678;
	retval = funInterface.GDLL_GetRelativePoint(pt,&pt2);
	printf("GDLL_GetRelativePoint function return %d, (%.3f,%.3f)\n",retval,pt2.X,pt2.Y);
	/////////////////////////////////////////////////////////////////////
	printf("Press [Enter] Key to Free Application\n");
	getch();
	retval = funInterface.GDLL_FreeApp();
	printf("GDLL_FreeApp function return %d\n",retval);
	//////////////////////////////////////////////////////////////////
	retval = funInterface.FreeDll(hInstance);
	printf("FreeDll function return %d\n",retval);
	return 0;
}

