#include "stdafx.h"
#include "usbapi.h"
#include <windows.h>
#include "dibhelp.h"
#include <vector>
#include <gdiplus.h>

using namespace Gdiplus;

enum PrintError
{
	Print_Memory_Fail,
	Print_File_Not_Support,
	Print_Get_Default_Printer_Fail,
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

static int DisplayDib(HDC hdc, HBITMAP hBitmap, int x, int y,
	int cxClient, int cyClient,
	WORD wShow, BOOL fHalftonePalette);

static HDIB DibRotateRight(HDIB hdibSrc);


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
	BOOL  bSuccess = TRUE;
	HDIB  hdib, hdibNew;
	HBITMAP hBitmap;
	HDC   hdcPrn = dc;
	int   cxPage;
	int	  cyPage;

	if (StartDoc(hdcPrn, &di) > 0)
	{
		// Start the page
		for (int i = 0; i < g_vecImagePaths.size(); i++)
		{
			cxPage = GetDeviceCaps(hdcPrn, HORZRES);
			cyPage = GetDeviceCaps(hdcPrn, VERTRES);

			hdib = DibFileLoad(g_vecImagePaths[i].imagePath);

			if (!hdib)
				return FALSE;

			if (StartPage(hdcPrn) < 0)
			{
				bSuccess = FALSE;
				DibDelete(hdib);
				break;
			}
			
			int times = (g_vecImagePaths[i].rotation % 360) / 90;
			for (int j = 0; j < times; j++)
			{
				if (hdibNew = DibRotateRight(hdib))
				{
					DibDelete(hdib);
					hdib = hdibNew;
				}

			}
			
			DisplayDib(hdcPrn, DibBitmapHandle(hdib), 0, 0,
				cxPage, cyPage, IDM_SHOW_ISOSTRETCH, FALSE);

			if (EndPage(hdcPrn) < 0)
			{
				bSuccess = FALSE;
				DibDelete(hdib);
				break;
			}

			DibDelete(hdib);
		}
	
	}
	else
		bSuccess = FALSE;

	if (bSuccess)
		EndDoc(hdcPrn);

	if (pdx.hDevMode != NULL)
		GlobalFree(pdx.hDevMode);
	if (pdx.hDevNames != NULL)
		GlobalFree(pdx.hDevNames);
	if (pdx.lpPageRanges != NULL)
		GlobalFree(pPageRanges);

	if (hdcPrn != NULL)
		DeleteDC(hdcPrn);

	return bSuccess;
}

static int DisplayDib(HDC hdc, HBITMAP hBitmap, int x, int y,
	int cxClient, int cyClient,
	WORD wShow, BOOL fHalftonePalette)
{
	BITMAP bitmap;
	HDC    hdcMem;
	int    cxBitmap, cyBitmap, iReturn;

	GetObject(hBitmap, sizeof (BITMAP), &bitmap);
	cxBitmap = bitmap.bmWidth;
	cyBitmap = bitmap.bmHeight;

	SaveDC(hdc);

	if (fHalftonePalette)
		SetStretchBltMode(hdc, HALFTONE);
	else
		SetStretchBltMode(hdc, COLORONCOLOR);

	hdcMem = CreateCompatibleDC(hdc);
	SelectObject(hdcMem, hBitmap);

	switch (wShow)
	{
	case IDM_SHOW_NORMAL:
		if (fHalftonePalette)
			iReturn = StretchBlt(hdc, 0, 0,
			min(cxClient, cxBitmap - x),
			min(cyClient, cyBitmap - y),
			hdcMem, x, y,
			min(cxClient, cxBitmap - x),
			min(cyClient, cyBitmap - y),
			SRCCOPY);
		else
			iReturn = BitBlt(hdc, 0, 0,
			min(cxClient, cxBitmap - x),
			min(cyClient, cyBitmap - y),
			hdcMem, x, y, SRCCOPY);
		break;

	case IDM_SHOW_CENTER:
		if (fHalftonePalette)
			iReturn = StretchBlt(hdc, (cxClient - cxBitmap) / 2,
			(cyClient - cyBitmap) / 2,
			cxBitmap, cyBitmap,
			hdcMem, 0, 0, cxBitmap, cyBitmap, SRCCOPY);
		else
			iReturn = BitBlt(hdc, (cxClient - cxBitmap) / 2,
			(cyClient - cyBitmap) / 2,
			cxBitmap, cyBitmap,
			hdcMem, 0, 0, SRCCOPY);
		break;

	case IDM_SHOW_STRETCH:
		iReturn = StretchBlt(hdc, 0, 0, cxClient, cyClient,
			hdcMem, 0, 0, cxBitmap, cyBitmap, SRCCOPY);
		break;

	case IDM_SHOW_ISOSTRETCH:
		SetMapMode(hdc, MM_ISOTROPIC);
		SetWindowExtEx(hdc, cxBitmap, cyBitmap, NULL);
		SetViewportExtEx(hdc, cxClient, cyClient, NULL);
		SetWindowOrgEx(hdc, cxBitmap / 2, cyBitmap / 2, NULL);
		SetViewportOrgEx(hdc, cxClient / 2, cyClient / 2, NULL);

		iReturn = StretchBlt(hdc, 0, 0, cxBitmap, cyBitmap,
			hdcMem, 0, 0, cxBitmap, cyBitmap, SRCCOPY);
		break;
	}
	DeleteDC(hdcMem);
	RestoreDC(hdc, -1);
	return iReturn;
}


static HDIB DibRotateRight(HDIB hdibSrc)
{
	HDIB hdibDst;
	int  cx, cy, x, y;

	if (!DibIsAddressable(hdibSrc))
		return NULL;

	if (NULL == (hdibDst = DibCopy(hdibSrc, TRUE)))
		return NULL;

	cx = DibWidth(hdibSrc);
	cy = DibHeight(hdibSrc);

	switch (DibBitCount(hdibSrc))
	{
	case  1:
		for (x = 0; x < cx; x++)
		for (y = 0; y < cy; y++)
			DibSetPixel1(hdibDst, cy - y - 1, x,
			DibGetPixel1(hdibSrc, x, y));
		break;

	case  4:
		for (x = 0; x < cx; x++)
		for (y = 0; y < cy; y++)
			DibSetPixel4(hdibDst, cy - y - 1, x,
			DibGetPixel4(hdibSrc, x, y));
		break;

	case  8:
		for (x = 0; x < cx; x++)
		for (y = 0; y < cy; y++)
			DibSetPixel8(hdibDst, cy - y - 1, x,
			DibGetPixel8(hdibSrc, x, y));
		break;

	case 16:
		for (x = 0; x < cx; x++)
		for (y = 0; y < cy; y++)
			DibSetPixel16(hdibDst, cy - y - 1, x,
			DibGetPixel16(hdibSrc, x, y));
		break;

	case 24:
		for (x = 0; x < cx; x++)
		for (y = 0; y < cy; y++)
			DibSetPixel24(hdibDst, cy - y - 1, x,
			DibGetPixel24(hdibSrc, x, y));
		break;

	case 32:
		for (x = 0; x < cx; x++)
		for (y = 0; y < cy; y++)
			DibSetPixel32(hdibDst, cy - y - 1, x,
			DibGetPixel32(hdibSrc, x, y));
		break;
	}
	return hdibDst;
}