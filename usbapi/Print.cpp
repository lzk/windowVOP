#include "stdafx.h"
#include "usbapi.h"
#include <windows.h>
#include "dibhelp.h"
#include <vector>


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
	int rotation;
}PrintItem;

USBAPI_API BOOL __stdcall PrintInit(const TCHAR * jobDescription, HWND hwnd);
USBAPI_API void __stdcall AddImagePath(const TCHAR * fileName, int rotation);
USBAPI_API BOOL __stdcall DoPrint();

static std::vector<PrintItem> g_vecImagePaths;
static PRINTDLGEX pdx;
static DOCINFO  di = { sizeof (DOCINFO) };
LPPRINTPAGERANGE pPageRanges = NULL;

static int DisplayDib(HDC hdc, HBITMAP hBitmap, int x, int y,
	int cxClient, int cyClient,
	WORD wShow, BOOL fHalftonePalette);

static HDIB DibRotateRight(HDIB hdibSrc);

USBAPI_API BOOL __stdcall PrintInit(const TCHAR * jobDescription, HWND hwnd)
{
	g_vecImagePaths.clear();
	di.lpszDocName = jobDescription;

	// Allocate an array of PRINTPAGERANGE structures.
	pPageRanges = (LPPRINTPAGERANGE)GlobalAlloc(GPTR, 10 * sizeof(PRINTPAGERANGE));
	if (!pPageRanges)
		return E_OUTOFMEMORY;

	pdx.lStructSize = sizeof (PRINTDLGEX);
	pdx.hwndOwner = hwnd;
	pdx.hDevMode = NULL;
	pdx.hDevNames = NULL;
	pdx.hDC = NULL;
	pdx.Flags = PD_RETURNDC | PD_COLLATE;
	pdx.Flags2 = 0;
	pdx.ExclusionFlags = 0;
	pdx.nPageRanges = 0;
	pdx.nMaxPageRanges = 10;
	pdx.lpPageRanges = pPageRanges;
	pdx.nMinPage = 1;
	pdx.nMaxPage = 1000;
	pdx.nCopies = 1;
	pdx.hInstance = 0;
	pdx.lpPrintTemplateName = NULL;
	pdx.lpCallback = NULL;
	pdx.nPropertyPages = 0;
	pdx.lphPropertyPages = NULL;
	pdx.nStartPage = START_PAGE_GENERAL;
	pdx.dwResultAction = 0;
	

	if (PrintDlgEx(&pdx) != S_OK || pdx.dwResultAction != PD_RESULT_PRINT)
		return FALSE;
	
	if (NULL == pdx.hDC)
		return FALSE;

	return TRUE;
}

USBAPI_API void __stdcall AddImagePath(const TCHAR * fileName, int rotation)
{
	PrintItem item = { fileName, rotation };
	g_vecImagePaths.push_back(item);
}

USBAPI_API BOOL __stdcall DoPrint()
{
	BOOL  bSuccess = TRUE;
	HDIB  hdib, hdibNew;
	HBITMAP hBitmap;
	HDC   hdcPrn = pdx.hDC;
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