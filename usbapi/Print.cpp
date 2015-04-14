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
#include "print.h"
#include <winspool.h>

#pragma comment(lib, "Shlwapi.lib")
#pragma comment(lib, "gdiplus.lib")

using namespace Gdiplus;

enum IdCardType
{
	NonIdCard,
	HouseholdRegister,
	IdCard,
	MarriageCertificate,
	Passport,
	RealEstateEvaluator,
	DriverLicense,
	Diploma,
	StudentIDcard,
	BirthCertificate,
	BankCards
};

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

typedef struct _IdCardSize
{
	double Width;
	double Height;
}IdCardSize;


USBAPI_API int __stdcall PrintFile(const TCHAR * strPrinterName, const TCHAR * strFileName);
USBAPI_API BOOL __stdcall PrintInit(const TCHAR * strPrinterName, const TCHAR * jobDescription, int idCardType = 0, IdCardSize *size = NULL);
USBAPI_API void __stdcall AddImagePath(const TCHAR * fileName);
USBAPI_API void __stdcall AddImageSource(IStream * imageSource);
USBAPI_API int __stdcall DoPrintImage();
USBAPI_API int __stdcall DoPrintIdCard();

USBAPI_API void __stdcall SetPrinterInfo(const TCHAR * strPrinterName,
	short sPaperSize,
	short sPaperOrientation,
	short sMediaType,
	short sPaperOrder,
	short sPrintQuality,
	short sScalingType,
	short sScalingRatio,
	short sNupNum,
	short sTypeofPB,
	short sPosterType,
	short sColorBalanceTo,
	short sDensity,
	short sDuplexPrint,
	short sReversePrint,
	short sTonerSaving);


static std::vector<PrintItem> g_vecImagePaths;
static std::vector<IStream*>  g_vecIdCardImageSources;
static DOCINFO  di = { sizeof (DOCINFO) };
static HDC dc = NULL;

static GdiplusStartupInput gdiplusStartupInput;
static ULONG_PTR gdiplusToken;
static int currentIdCardType = 0;
static IdCardSize currentIdCardSize = { 0 };
Size A4Size( 21, 29.7 ); //unit cm

USBAPI_API int __stdcall PrintFile(const TCHAR * strPrinterName, const TCHAR * strFileName)
{
	PrintError error = Print_OK;
	DWORD bufferSize = 500;
	TCHAR defaultPrinterName[500];
	int shellExeRes = 0;
	const TCHAR *fileExt = NULL;

	fileExt = PathFindExtension(strFileName);
	std::wstring strExt(fileExt);
	std::transform(strExt.begin(), strExt.end(), strExt.begin(), ::tolower);

	if (strExt.compare(L".bmp") == 0
		|| strExt.compare(L".ico") == 0
		|| strExt.compare(L".gif") == 0
		|| strExt.compare(L".jpg") == 0
		|| strExt.compare(L".exif") == 0
		|| strExt.compare(L".png") == 0
		|| strExt.compare(L".tif") == 0
		|| strExt.compare(L".wmf") == 0
		|| strExt.compare(L".emf") == 0)
	{
		if (PrintInit(strPrinterName, L"Print Image Files"))
		{
			AddImagePath(strFileName);
			DoPrintImage();
		}
		else
		{
			error = Print_Operation_Fail;
		}
	}
	else
	{
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
	}

	return error;
}

USBAPI_API BOOL __stdcall PrintInit(const TCHAR * strPrinterName, const TCHAR * jobDescription, int idCardType, IdCardSize *size)
{
	g_vecImagePaths.clear();
	g_vecIdCardImageSources.clear();

	currentIdCardType = idCardType;

	if (size != NULL)
		currentIdCardSize = *size;

	ZeroMemory(&di, sizeof(di));
	di.cbSize = sizeof(di);
	di.lpszDocName = jobDescription;

	dc = CreateDCW(L"WINSPOOL", strPrinterName, NULL, NULL);

	if (dc == NULL)
		return FALSE;

	Status status;
	if ((status = Gdiplus::GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL)) != Ok)
	{
		return FALSE;
	}

	return TRUE;
}

USBAPI_API void __stdcall AddImagePath(const TCHAR * fileName)
{
	PrintItem item = { fileName };
	g_vecImagePaths.push_back(item);
}

USBAPI_API void __stdcall AddImageSource(IStream * imageSource)
{
	g_vecIdCardImageSources.push_back(imageSource);
}

USBAPI_API int __stdcall DoPrintImage()
{
	PrintError error = Print_OK;
	Status status;
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
							x = 0; //Align Top left
							y = 0;
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

						if ((status = graphics.DrawImage(&img, x, y, w, h)) != Ok)
						{
							if (status == OutOfMemory)
							{
								error = Print_Memory_Fail;
							}
							else
							{
								error = Print_Operation_Fail;
							}
							break;
						}
					
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

	Gdiplus::GdiplusShutdown(gdiplusToken);

	if (error == Print_OK)
		EndDoc(hdcPrn);

	if (hdcPrn != NULL)
		DeleteDC(hdcPrn);

	return error;
}

USBAPI_API int __stdcall DoPrintIdCard()
{
	PrintError error = Print_OK;
	HDC   hdcPrn = dc;
	int   cxPage;
	int	  cyPage;
	double imageWidth = 0;
	double imageHeight = 0;
	double imageToLeft = 0;
	double imageToTop = 0;
	Status status;

	if (StartDoc(hdcPrn, &di) > 0)
	{
		cxPage = GetDeviceCaps(hdcPrn, HORZRES);
		cyPage = GetDeviceCaps(hdcPrn, VERTRES);

		switch (currentIdCardType)
		{
		case IdCard:
			if (g_vecIdCardImageSources.size() == 2)
			{
				imageWidth = 0;
				imageHeight = 0;
				imageToLeft = 0;
				imageToTop = 0;

				imageWidth = cxPage * (currentIdCardSize.Width / A4Size.Width);
				imageHeight = cyPage * (currentIdCardSize.Height / A4Size.Height);

				imageToLeft = (cxPage - imageWidth) / 2;
				imageToTop = (cyPage / 2 - imageHeight) / 2;

				Image img1(g_vecIdCardImageSources[0]);
				Image img2(g_vecIdCardImageSources[1]);

				if (StartPage(hdcPrn) < 0)
				{
					error = Print_Operation_Fail;
					break;
				}
			
				Graphics graphics(hdcPrn);
				graphics.SetPageUnit(UnitPixel);

				if ((status = graphics.DrawImage(&img1, (int)imageToLeft, (int)imageToTop, (int)imageWidth, (int)imageHeight)) != Ok)
				{
					if (status == OutOfMemory)
					{
						error = Print_Memory_Fail;
					}
					else
					{
						error = Print_Operation_Fail;
					}
					break;
				}

				if ((status = graphics.DrawImage(&img2, (int)imageToLeft, (int)imageToTop + cyPage / 2, (int)imageWidth, (int)imageHeight)) != Ok)
				{
					if (status == OutOfMemory)
					{
						error = Print_Memory_Fail;
					}
					else
					{
						error = Print_Operation_Fail;
					}
					break;
				}

				if (EndPage(hdcPrn) < 0)
				{
					error = Print_Operation_Fail;
					break;
				}

			}
			break;
		case MarriageCertificate:
			if (g_vecIdCardImageSources.size() == 1)
			{
				imageWidth = 0;
				imageHeight = 0;
				imageToLeft = 0;
				imageToTop = 0;

				imageWidth = cxPage * (currentIdCardSize.Width / A4Size.Width);
				imageHeight = cyPage * (currentIdCardSize.Height / A4Size.Height);

				imageToLeft = (cxPage - imageWidth) / 2;
				imageToTop = (cyPage - imageHeight) / 2;

				Image img1(g_vecIdCardImageSources[0]);

				if (StartPage(hdcPrn) < 0)
				{
					error = Print_Operation_Fail;
					break;
				}

				Graphics graphics(hdcPrn);
				graphics.SetPageUnit(UnitPixel);

				if ((status = graphics.DrawImage(&img1, (int)imageToLeft, (int)imageToTop, (int)imageWidth, (int)imageHeight)) != Ok)
				{
					if (status == OutOfMemory)
					{
						error = Print_Memory_Fail;
					}
					else
					{
						error = Print_Operation_Fail;
					}
					break;
				}

				if (EndPage(hdcPrn) < 0)
				{
					error = Print_Operation_Fail;
					break;
				}
			}
			break;
		case HouseholdRegister:
		case Passport:
		case RealEstateEvaluator:
		case DriverLicense:
		case StudentIDcard:
		case BirthCertificate:
		case BankCards:
		case Diploma:
			if (g_vecIdCardImageSources.size() == 1)
			{
				imageWidth = 0;
				imageHeight = 0;
				imageToLeft = 0;
				imageToTop = 0;

				imageWidth = cyPage * (currentIdCardSize.Width / A4Size.Height);
				imageHeight = cxPage * (currentIdCardSize.Height / A4Size.Width);

				imageToLeft = (cxPage - imageHeight) / 2;
				imageToTop = (cyPage - imageWidth) / 2 + imageWidth;

				Image *pImg1 = NULL;
				pImg1 = Image::FromStream(g_vecIdCardImageSources[0]);

				status = pImg1->GetLastStatus();
				if (status != Ok)
				{
					if (pImg1)
						delete pImg1;

					if (status == OutOfMemory)
					{
						error = Print_Memory_Fail;
					}
					else
					{
						error = Print_Operation_Fail;
					}
					break;
				}

				if (StartPage(hdcPrn) < 0)
				{
					if (pImg1)
						delete pImg1;

					error = Print_Operation_Fail;
					break;
				}

				Graphics graphics(hdcPrn);
				graphics.SetPageUnit(UnitPixel);
				graphics.TranslateTransform((int)imageToLeft, (int)imageToTop);
				graphics.RotateTransform(270.0f);

				if ((status = graphics.DrawImage(pImg1, 0, 0, (int)imageWidth, (int)imageHeight)) != Ok)
				{
					if (pImg1)
						delete pImg1;

					if (status == OutOfMemory)
					{
						error = Print_Memory_Fail;
					}
					else
					{
						error = Print_Operation_Fail;
					}
					break;
				}

				if (EndPage(hdcPrn) < 0)
				{
					if (pImg1)
						delete pImg1;

					error = Print_Operation_Fail;
					break;
				}

				if (pImg1)
					delete pImg1;
			}
			break;
		default:
			break;
		}
	}
	else
		error = Print_Operation_Fail;

    Gdiplus::GdiplusShutdown(gdiplusToken);

	if (error == Print_OK)
		EndDoc(hdcPrn);

	if (hdcPrn != NULL)
		DeleteDC(hdcPrn);

	return error;
}

USBAPI_API void __stdcall SetPrinterInfo(const TCHAR * strPrinterName,
	short sPaperSize,
	short sPaperOrientation,
	short sMediaType,
	short sPaperOrder,
	short sPrintQuality,//byte
	short sScalingType,//byte
	short sScalingRatio,
	short sNupNum,//byte
	short sTypeofPB,//byte
	short sPosterType,
	short sColorBalanceTo,
	short sDensity,
	short sDuplexPrint,
	short sReversePrint,//byte
	short sTonerSaving)//byte
{
	HANDLE   phandle;
	DWORD dmsize;

	phandle = NULL;
	wchar_t szprintername[MAX_PATH] = { 0 };
	wcscpy_s(szprintername, MAX_PATH, strPrinterName);

	if (OpenPrinter(szprintername, &phandle, NULL))
	{

		LPPRINTER_INFO_2 printer_info;

		GetPrinter(phandle, 2, (LPBYTE)NULL, 0, &dmsize);

		printer_info = (LPPRINTER_INFO_2)malloc(dmsize);

		if (printer_info != NULL)
		{
			if (GetPrinter(phandle, 2, (LPBYTE)printer_info, dmsize, &dmsize))
			{
				PCLDEVMODE devmode;

				devmode = *(LPPCLDEVMODE)printer_info->pDevMode;
				switch (sPaperSize)
				{
				case 0:
					devmode.dmPublic.dmPaperSize = DMPAPER_A4;
					break;
				case 1:
					devmode.dmPublic.dmPaperSize = DMPAPER_LETTER;
					break;
				case 2:
					devmode.dmPublic.dmPaperSize = DMPAPER_B5;
					break;
				case 3:
					devmode.dmPublic.dmPaperSize = DMPAPER_A5;
					break;
				case 4:
					devmode.dmPublic.dmPaperSize = DMPAPER_A5LEF;
					break;
				case 5:
					devmode.dmPublic.dmPaperSize = DMPAPER_B6_JIS;
					break;
				case 6:
					devmode.dmPublic.dmPaperSize = DMPAPER_B6LEF;
					break;
				case 7:
					devmode.dmPublic.dmPaperSize = DMPAPER_A6;
					break;
				case 8:
					devmode.dmPublic.dmPaperSize = DMPAPER_EXECUTIVE;
					break;
				case 9:
					devmode.dmPublic.dmPaperSize = DMPAPER_P16K;
					break;
				case 10:
					devmode.dmPublic.dmPaperSize = DMPAPER_USER;
					break;
				default:
					devmode.dmPublic.dmPaperSize = DMPAPER_LETTER;
					break;
				}
				
				devmode.dmPublic.dmOrientation = sPaperOrientation;
				devmode.dmPublic.dmMediaType = sMediaType + DMMEDIA_USER;
				devmode.dmPrivate.par.wMediaType = sMediaType;
				devmode.dmPublic.dmCollate = sPaperOrder;
				//devmode.dmPublic.dmPrintQuality = sPrintQuality;
				devmode.dmPrivate.PrintQuality = static_cast<BYTE>(sPrintQuality);
				devmode.dmPrivate.nup.bNupNum = static_cast<BYTE>(sNupNum);//multiple-page 2in1: 2, 4in1: 4, 6in1: 6, 9in1: 9, 16 in1: 16

				devmode.dmPrivate.bpmrdata.TypeofPB = static_cast<BYTE>(sTypeofPB);//multiple-page, 1in nxn pages
				devmode.dmPrivate.poster.wPosterType = sPosterType;// 0: 1 in 2x2, 1: 1 in 3x3, 2: 1 in 4x4 pages.

				devmode.dmPrivate.sfp.ISFSet = static_cast<BYTE>(sScalingType);// scale 
				devmode.dmPrivate.sfp.SRatio = sScalingRatio;//scale 25~400
				devmode.dmPrivate.graphics.bColorBalanceTo = static_cast<BYTE>(sColorBalanceTo);

				devmode.dmPrivate.graphics.ColorBalanceIndex[0][0] = static_cast<signed char>(sDensity);

				devmode.dmPublic.dmDuplex = sDuplexPrint; //DUPLEX£¬ DMDUP_VERTICAL: ³¤±ß DMDUP_HORIZONTAL£¬¶Ì±ß

				devmode.dmPrivate.bPaperReverseOrder = static_cast<BYTE>(sReversePrint);
				devmode.dmPrivate.graphics.TonerSaving = static_cast<BYTE>(sTonerSaving);


				*((LPPCLDEVMODE)printer_info->pDevMode) = devmode;

				SetPrinter(phandle, 2, (LPBYTE)printer_info, 0);
			}
			free(printer_info);
		}
	}
	if (phandle != NULL)
	{
		ClosePrinter(phandle);
		phandle = NULL;
	}
}

