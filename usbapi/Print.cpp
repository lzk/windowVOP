#include "stdafx.h"
#include "usbapi.h"
#include <windows.h>
#include "dibhelp.h"
#include <vector>
#include <gdiplus.h>
#include <Shlwapi.h>
#include <tchar.h>
#include <algorithm>
#include <string>

#pragma comment(lib, "Shlwapi.lib")
#pragma comment(lib, "gdiplus.lib")

using namespace Gdiplus;

enum PrintError
{
	Print_Memory_Fail,
	Print_File_Not_Support,
	Print_Get_Default_Printer_Fail,
	Print_Operation_Fail,
	Print_OK,
};

enum PrintShowMode
{
	IDM_SHOW_NORMAL,
	IDM_SHOW_CENTER,
	IDM_SHOW_STRETCH,
	IDM_SHOW_ISOSTRETCH
};

typedef struct _PrintItem
{
	const TCHAR * imagePath;
	//int rotation;
}PrintItem;

USBAPI_API int __stdcall PrintFile(const TCHAR * strPrinterName, const TCHAR * strFileName);
USBAPI_API BOOL __stdcall PrintInit(const TCHAR * strPrinterName, const TCHAR * jobDescription);
USBAPI_API void __stdcall AddImagePath(const TCHAR * fileName);
USBAPI_API int __stdcall DoPrint();

static std::vector<PrintItem> g_vecImagePaths;
//static PRINTDLGEX pdx;
static DOCINFO  di = { sizeof (DOCINFO) };
static HDC dc = NULL;
//LPPRINTPAGERANGE pPageRanges = NULL;

static GdiplusStartupInput gdiplusStartupInput;
static ULONG_PTR gdiplusToken;

//static int DisplayDib(HDC hdc, HBITMAP hBitmap, int x, int y,
//	int cxClient, int cyClient,
//	WORD wShow, BOOL fHalftonePalette);
//static HDIB DibRotateRight(HDIB hdibSrc);

static void BeginDrawImage(HDC hdc, int cxClient, int cyClient, int cxImage, int cyImage);
static void EndDrawImage(HDC hdc);

USBAPI_API int __stdcall PrintFile(const TCHAR * strPrinterName, const TCHAR * strFileName)
{
	PrintError error = Print_OK;
	DWORD bufferSize = 500;
	TCHAR defaultPrinterName[500];
	int shellExeRes = 0;

	if (GetDefaultPrinter(defaultPrinterName, &bufferSize))
	{
		::SetDefaultPrinter(strPrinterName);
		CoInitializeEx(NULL, COINIT_APARTMENTTHREADED | COINIT_DISABLE_OLE1DDE);

		if ((shellExeRes = (int)::ShellExecute(NULL, L"print", strFileName, NULL, NULL, SW_HIDE)) > 32)
		{
			
		}
		else
		{
			if (shellExeRes == SE_ERR_OOM || shellExeRes == 0)
			{
				error = Print_Memory_Fail;
			}
			else
			{
				error = Print_File_Not_Support;
			}
		}

		::SetDefaultPrinter(defaultPrinterName);
	}
	else
	{
		error = Print_Get_Default_Printer_Fail;
	}

	return error;
}

USBAPI_API BOOL __stdcall PrintInit(const TCHAR * strPrinterName, const TCHAR * jobDescription)
{
	g_vecImagePaths.clear();

	ZeroMemory(&di, sizeof(di));
	di.cbSize = sizeof(di);
	di.lpszDocName = jobDescription;

	dc = CreateDCW(L"WINSPOOL", strPrinterName, NULL, NULL);

	if (dc == NULL)
		return FALSE;

	Status status = GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);

	if (status != Ok)
		return FALSE;

	return TRUE;
}

USBAPI_API void __stdcall AddImagePath(const TCHAR * fileName)
{
	PrintItem item = { fileName };
	g_vecImagePaths.push_back(item);
}

USBAPI_API int __stdcall DoPrint()
{
	PrintError error = Print_OK;
	HDC   hdcPrn = dc;
	const TCHAR *fileExt = NULL;
	int   cxPage;
	int	  cyPage;

	GUID* pDimensionIDs;
	UINT frameCount = 0;

	UINT count = 0;
	UINT fIndex = 0;
	BOOL IsFitted = FALSE;

	if (StartDoc(hdcPrn, &di) > 0)
	{
		cxPage = GetDeviceCaps(hdcPrn, HORZRES);
		cyPage = GetDeviceCaps(hdcPrn, VERTRES);

		// Start the page
		for (UINT i = 0; i < g_vecImagePaths.size(); i++)
		{
			fileExt = PathFindExtension(g_vecImagePaths[i].imagePath);
			std::wstring strExt(fileExt);
			std::transform(strExt.begin(), strExt.end(), strExt.begin(), ::tolower);
		
			if ( strExt.compare(L".bmp") == 0
			  || strExt.compare(L".ico") == 0
			  || strExt.compare(L".gif") == 0
			  || strExt.compare(L".jpg") == 0
			  || strExt.compare(L".exif") == 0
			  || strExt.compare(L".png") == 0
			  || strExt.compare(L".tif") == 0
			  || strExt.compare(L".wmf") == 0
			  || strExt.compare(L".emf") == 0)
			{
				Image img(g_vecImagePaths[i].imagePath);
				count = img.GetFrameDimensionsCount();

				if (count > 0)
				{
					pDimensionIDs = (GUID*)malloc(sizeof(GUID)*count);
					img.GetFrameDimensionsList(pDimensionIDs, count);
					frameCount = img.GetFrameCount(&pDimensionIDs[0]);

					fIndex = 0;
					while (fIndex < frameCount)
					{
						img.SelectActiveFrame(&pDimensionIDs[0], fIndex);
						if (StartPage(hdcPrn) < 0)
						{
							error = Print_Operation_Fail;
							break;
						}

						int x = 0;
						int y = 0;
						int w = img.GetWidth();
						int h = img.GetHeight();

						double whRatio = (double)w / h;
						double scaleRatioX = (double)w / cxPage;
						double scaleRatioY = (double)h / cyPage;

						if (w < cxPage && h < cyPage)
						{
							IsFitted = TRUE;
						}
						else
						{
							IsFitted = FALSE;
						}

						if (IsFitted == TRUE)
						{
							w = img.GetWidth();
							h = img.GetHeight();
							x = (cxPage - w) / 2;
							y = (cyPage - h) / 2;
						}
						else
						{
							if (scaleRatioX > scaleRatioY)
							{
								w = cxPage;
								h = (int)((double)cxPage / whRatio);
								y = (cyPage - h) / 2;
							}
							else if (scaleRatioX < scaleRatioY)
							{
								w = (int)((double)cyPage * whRatio);
								h = cyPage;
								x = (cxPage - w) / 2;
							}
							else
							{
								w = cxPage;
								h = cyPage;
							}
						}

						Graphics graphics(hdcPrn);
						graphics.SetPageUnit(UnitPixel);
						graphics.DrawImage(&img, x, y, w, h);
					
						fIndex++;

						if (EndPage(hdcPrn) < 0)
						{
							error = Print_Operation_Fail;
							break;
						}
					}

					free(pDimensionIDs);
				}
				
			}
			else
			{
				error = Print_File_Not_Support;
				break;
			}
		}
	}
	else
		error = Print_Operation_Fail;

	GdiplusShutdown(gdiplusToken);

	if (error == Print_OK)
		EndDoc(hdcPrn);

	if (hdcPrn != NULL)
		DeleteDC(hdcPrn);

	return error;
}

static void BeginDrawImage(HDC hdc, int cxClient, int cyClient, int cxImage, int cyImage)
{
	SaveDC(hdc);
	SetMapMode(hdc, MM_ISOTROPIC);
	/*SetWindowExtEx(hdc, cxImage, cyImage, NULL);
	SetViewportExtEx(hdc, cxClient, cyClient, NULL);
	SetWindowOrgEx(hdc, cxImage / 2, cyImage / 2, NULL);
	SetViewportOrgEx(hdc, cxClient / 2, cyClient / 2, NULL);*/
}

static void EndDrawImage(HDC hdc)
{
	RestoreDC(hdc, -1);
}

//static int DisplayDib(HDC hdc, HBITMAP hBitmap, int x, int y,
//	int cxClient, int cyClient,
//	WORD wShow, BOOL fHalftonePalette)
//{
//	BITMAP bitmap;
//	HDC    hdcMem;
//	int    cxBitmap, cyBitmap, iReturn;
//
//	GetObject(hBitmap, sizeof (BITMAP), &bitmap);
//	cxBitmap = bitmap.bmWidth;
//	cyBitmap = bitmap.bmHeight;
//
//	SaveDC(hdc);
//
//	if (fHalftonePalette)
//		SetStretchBltMode(hdc, HALFTONE);
//	else
//		SetStretchBltMode(hdc, COLORONCOLOR);
//
//	hdcMem = CreateCompatibleDC(hdc);
//	SelectObject(hdcMem, hBitmap);
//
//	switch (wShow)
//	{
//	case IDM_SHOW_NORMAL:
//		if (fHalftonePalette)
//			iReturn = StretchBlt(hdc, 0, 0,
//			min(cxClient, cxBitmap - x),
//			min(cyClient, cyBitmap - y),
//			hdcMem, x, y,
//			min(cxClient, cxBitmap - x),
//			min(cyClient, cyBitmap - y),
//			SRCCOPY);
//		else
//			iReturn = BitBlt(hdc, 0, 0,
//			min(cxClient, cxBitmap - x),
//			min(cyClient, cyBitmap - y),
//			hdcMem, x, y, SRCCOPY);
//		break;
//
//	case IDM_SHOW_CENTER:
//		if (fHalftonePalette)
//			iReturn = StretchBlt(hdc, (cxClient - cxBitmap) / 2,
//			(cyClient - cyBitmap) / 2,
//			cxBitmap, cyBitmap,
//			hdcMem, 0, 0, cxBitmap, cyBitmap, SRCCOPY);
//		else
//			iReturn = BitBlt(hdc, (cxClient - cxBitmap) / 2,
//			(cyClient - cyBitmap) / 2,
//			cxBitmap, cyBitmap,
//			hdcMem, 0, 0, SRCCOPY);
//		break;
//
//	case IDM_SHOW_STRETCH:
//		iReturn = StretchBlt(hdc, 0, 0, cxClient, cyClient,
//			hdcMem, 0, 0, cxBitmap, cyBitmap, SRCCOPY);
//		break;
//
//	case IDM_SHOW_ISOSTRETCH:
//		SetMapMode(hdc, MM_ISOTROPIC);
//		SetWindowExtEx(hdc, cxBitmap, cyBitmap, NULL);
//		SetViewportExtEx(hdc, cxClient, cyClient, NULL);
//		SetWindowOrgEx(hdc, cxBitmap / 2, cyBitmap / 2, NULL);
//		SetViewportOrgEx(hdc, cxClient / 2, cyClient / 2, NULL);
//
//		iReturn = StretchBlt(hdc, 0, 0, cxBitmap, cyBitmap,
//			hdcMem, 0, 0, cxBitmap, cyBitmap, SRCCOPY);
//		break;
//	}
//	DeleteDC(hdcMem);
//	RestoreDC(hdc, -1);
//	return iReturn;
//}
//
//
//static HDIB DibRotateRight(HDIB hdibSrc)
//{
//	HDIB hdibDst;
//	int  cx, cy, x, y;
//
//	if (!DibIsAddressable(hdibSrc))
//		return NULL;
//
//	if (NULL == (hdibDst = DibCopy(hdibSrc, TRUE)))
//		return NULL;
//
//	cx = DibWidth(hdibSrc);
//	cy = DibHeight(hdibSrc);
//
//	switch (DibBitCount(hdibSrc))
//	{
//	case  1:
//		for (x = 0; x < cx; x++)
//		for (y = 0; y < cy; y++)
//			DibSetPixel1(hdibDst, cy - y - 1, x,
//			DibGetPixel1(hdibSrc, x, y));
//		break;
//
//	case  4:
//		for (x = 0; x < cx; x++)
//		for (y = 0; y < cy; y++)
//			DibSetPixel4(hdibDst, cy - y - 1, x,
//			DibGetPixel4(hdibSrc, x, y));
//		break;
//
//	case  8:
//		for (x = 0; x < cx; x++)
//		for (y = 0; y < cy; y++)
//			DibSetPixel8(hdibDst, cy - y - 1, x,
//			DibGetPixel8(hdibSrc, x, y));
//		break;
//
//	case 16:
//		for (x = 0; x < cx; x++)
//		for (y = 0; y < cy; y++)
//			DibSetPixel16(hdibDst, cy - y - 1, x,
//			DibGetPixel16(hdibSrc, x, y));
//		break;
//
//	case 24:
//		for (x = 0; x < cx; x++)
//		for (y = 0; y < cy; y++)
//			DibSetPixel24(hdibDst, cy - y - 1, x,
//			DibGetPixel24(hdibSrc, x, y));
//		break;
//
//	case 32:
//		for (x = 0; x < cx; x++)
//		for (y = 0; y < cy; y++)
//			DibSetPixel32(hdibDst, cy - y - 1, x,
//			DibGetPixel32(hdibSrc, x, y));
//		break;
//	}
//	return hdibDst;
//}