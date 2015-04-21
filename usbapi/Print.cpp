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
#include "DM2OEM.H"
#include "DEVMODE.H"
#include <winspool.h>

#pragma comment(lib, "Shlwapi.lib")
#pragma comment(lib, "gdiplus.lib")

#pragma pack(8)

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


USBAPI_API int __stdcall PrintFile(const TCHAR * strPrinterName, const TCHAR * strFileName, bool fitToPage);
USBAPI_API BOOL __stdcall PrintInit(const TCHAR * strPrinterName, const TCHAR * jobDescription, int idCardType, IdCardSize *size, bool fitToPage);
USBAPI_API void __stdcall AddImagePath(const TCHAR * fileName);
USBAPI_API void __stdcall AddImageSource(IStream * imageSource);
USBAPI_API int __stdcall DoPrintImage();
USBAPI_API int __stdcall DoPrintIdCard();

USBAPI_API void __stdcall SetPrinterInfo(const TCHAR * strPrinterName,
	UINT8 PaperSize,
	UINT8 PaperOrientation,
	UINT8 MediaType,
	UINT8 PaperOrder,
	UINT8 PrintQuality,
	UINT8 ScalingType,
	UINT8 ScalingRatio,
	UINT8 NupNum,
	UINT8 TypeofPB,
	UINT8 PosterType,
	UINT8 ADJColorBalance,
	UINT8 ColorBalanceTo,
	UINT8 Density,
	UINT8 DuplexPrint,
	UINT8 ReversePrint,
	UINT8 TonerSaving);

USBAPI_API int __stdcall GetPrinterInfo(const TCHAR * strPrinterName,
	BYTE* ptr_paperSize,
	BYTE* ptr_paperOrientation,
	BYTE* ptr_mediaType,
	BYTE* ptr_paperOrder,
	BYTE* ptr_printQuality,//byte
	BYTE* ptr_scalingType,//byte
	BYTE* ptr_scalingRatio,
	BYTE* ptr_nupNum,//byte
	BYTE* ptr_typeofPB,//byte
	BYTE* ptr_posterType,
	BYTE* ptr_ADJColorBalance,
	BYTE* ptr_colorBalanceTo,
	BYTE* ptr_density,
	BYTE* ptr_duplexPrint,
	BYTE* ptr_reversePrint,//byte
	BYTE* ptr_tonerSaving);//byte

USBAPI_API int __stdcall OpenDocumentProperties(const TCHAR * strPrinterName,
	BYTE* ptr_paperSize,
	BYTE* ptr_paperOrientation,
	BYTE* ptr_mediaType,
	BYTE* ptr_paperOrder,
	BYTE* ptr_printQuality,//byte
	BYTE* ptr_scalingType,//byte
	BYTE* ptr_scalingRatio,
	BYTE* ptr_nupNum,//byte
	BYTE* ptr_typeofPB,//byte
	BYTE* ptr_posterType,
	BYTE* ptr_ADJColorBalance,
	BYTE* ptr_colorBalanceTo,
	BYTE* ptr_density,
	BYTE* ptr_duplexPrint,
	BYTE* ptr_reversePrint,//byte
	BYTE* ptr_tonerSaving);//byte

USBAPI_API void __stdcall SetCopies(const TCHAR * strPrinterName, UINT8 Copies);

static std::vector<PrintItem> g_vecImagePaths;
static std::vector<IStream*>  g_vecIdCardImageSources;
static DOCINFO  di = { sizeof (DOCINFO) };
static HDC dc = NULL;

static GdiplusStartupInput gdiplusStartupInput;
static ULONG_PTR gdiplusToken;
static int currentIdCardType = 0;
static IdCardSize currentIdCardSize = { 0 };
static bool needFitToPage = false;
Size A4Size( 21, 29.7 ); //unit cm

USBAPI_API int __stdcall PrintFile(const TCHAR * strPrinterName, const TCHAR * strFileName, bool fitToPage)
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
		if (PrintInit(strPrinterName, L"Print Image Files", 0, NULL, fitToPage))
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

USBAPI_API BOOL __stdcall PrintInit(const TCHAR * strPrinterName, const TCHAR * jobDescription, int idCardType, IdCardSize *size, bool fitToPage)
{
	g_vecImagePaths.clear();
	g_vecIdCardImageSources.clear();

	currentIdCardType = idCardType;
	needFitToPage = fitToPage;

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

						if (needFitToPage)
						{
							IsFitted = FALSE;
						}
						else
						{
							if (w < cxPage && h < cyPage)
							{
								IsFitted = TRUE;
							}
							else
							{
								IsFitted = FALSE;
							}
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
	UINT8 PaperSize,
	UINT8 PaperOrientation,
	UINT8 MediaType,
	UINT8 PaperOrder,
	UINT8 PrintQuality,//byte
	UINT8 ScalingType,//byte
	UINT8 ScalingRatio,
	UINT8 NupNum,//byte
	UINT8 TypeofPB,//byte
	UINT8 PosterType,
	UINT8 ADJColorBalance,
	UINT8 ColorBalanceTo,
	UINT8 Density,
	UINT8 DuplexPrint,
	UINT8 ReversePrint,//byte
	UINT8 TonerSaving)//byte
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

				DWORD dwSize = sizeof(devmode)- sizeof(DEVMODE);

				devmode = *(LPPCLDEVMODE)printer_info->pDevMode;
				switch (PaperSize)
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

				devmode.dmPublic.dmOrientation = PaperOrientation;
				devmode.dmPublic.dmMediaType = MediaType + DMMEDIA_USER;
				devmode.dmPrivate.par.wMediaType = MediaType;
				devmode.dmPublic.dmCollate = PaperOrder;
				devmode.dmPrivate.PrintQuality = PrintQuality;
				devmode.dmPrivate.nup.bNupNum = NupNum;//multiple-page 2in1: 2, 4in1: 4, 6in1: 6, 9in1: 9, 16 in1: 16

				devmode.dmPrivate.bpmrdata.TypeofPB = TypeofPB;//multiple-page, 1in nxn pages
				devmode.dmPrivate.poster.wPosterType = PosterType;// 0: 1 in 2x2, 1: 1 in 3x3, 2: 1 in 4x4 pages.

				devmode.dmPrivate.sfp.ISFSet = ScalingType;// scale 
				devmode.dmPrivate.sfp.SRatio = ScalingRatio;//scale 25~400

				devmode.dmPrivate.graphics.isADJColorBalance = ADJColorBalance;
				devmode.dmPrivate.graphics.bColorBalanceTo = ColorBalanceTo;

				devmode.dmPrivate.graphics.ColorBalanceIndex[0][0] = Density;

				devmode.dmPublic.dmDuplex = DuplexPrint; //DUPLEX�� DMDUP_VERTICAL: ���� DMDUP_HORIZONTAL���̱�

				devmode.dmPrivate.bPaperReverseOrder = ReversePrint;
				devmode.dmPrivate.graphics.TonerSaving = TonerSaving;


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

USBAPI_API void __stdcall SetCopies(const TCHAR * strPrinterName, UINT8 Copies)
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

				DWORD dwSize = sizeof(devmode)-sizeof(DEVMODE);

				devmode = *(LPPCLDEVMODE)printer_info->pDevMode;
				
				devmode.dmPublic.dmCopies = Copies;

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

USBAPI_API int __stdcall GetPrinterInfo(const TCHAR * strPrinterName,
	BYTE* ptr_paperSize,
	BYTE* ptr_paperOrientation,
	BYTE* ptr_mediaType,
	BYTE* ptr_paperOrder,
	BYTE* ptr_printQuality,//byte
	BYTE* ptr_scalingType,//byte
	BYTE* ptr_scalingRatio,
	BYTE* ptr_nupNum,//byte
	BYTE* ptr_typeofPB,//byte
	BYTE* ptr_posterType,
	BYTE* ptr_ADJColorBalance,
	BYTE* ptr_colorBalanceTo,
	BYTE* ptr_density,
	BYTE* ptr_duplexPrint,
	BYTE* ptr_reversePrint,//byte
	BYTE* ptr_tonerSaving)//byte
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

				DWORD dwSize = sizeof(devmode)-sizeof(DEVMODE);

				devmode = *(LPPCLDEVMODE)printer_info->pDevMode;
				
				switch (devmode.dmPublic.dmPaperSize)
				{
				case DMPAPER_A4:
					*ptr_paperSize = 0;
					break;
				case DMPAPER_LETTER:
					*ptr_paperSize = 1;
					break;
				case DMPAPER_B5:
					*ptr_paperSize = 2;
					break;
				case DMPAPER_A5:
					*ptr_paperSize = 3;
					break;
				case DMPAPER_A5LEF:
					*ptr_paperSize = 4;
					break;
				case DMPAPER_B6_JIS:
					*ptr_paperSize = 5;
					break;
				case DMPAPER_B6LEF:
					*ptr_paperSize = 6;
					break;
				case DMPAPER_A6:
					*ptr_paperSize = 7;
					break;
				case DMPAPER_EXECUTIVE:
					*ptr_paperSize = 8;
					break;
				case DMPAPER_P16K:
					*ptr_paperSize = 9;
					break;
				case DMPAPER_USER:
					*ptr_paperSize = 10;
					break;
				default:
					*ptr_paperSize = 1;
					break;
				}

				*ptr_paperOrientation = static_cast<BYTE>(devmode.dmPublic.dmOrientation);				
				*ptr_mediaType = static_cast<BYTE>(devmode.dmPrivate.par.wMediaType);
				*ptr_paperOrder = static_cast<BYTE>(devmode.dmPublic.dmCollate);				
				*ptr_printQuality = static_cast<short>(devmode.dmPrivate.PrintQuality);
				*ptr_nupNum = devmode.dmPrivate.nup.bNupNum;
				*ptr_typeofPB = devmode.dmPrivate.bpmrdata.TypeofPB;
				*ptr_posterType = static_cast<BYTE>(devmode.dmPrivate.poster.wPosterType);
				*ptr_scalingType = devmode.dmPrivate.sfp.ISFSet;
				*ptr_scalingRatio = static_cast<BYTE>(devmode.dmPrivate.sfp.SRatio);
				*ptr_ADJColorBalance = devmode.dmPrivate.graphics.isADJColorBalance;
				*ptr_colorBalanceTo = devmode.dmPrivate.graphics.bColorBalanceTo;
				*ptr_density = devmode.dmPrivate.graphics.ColorBalanceIndex[0][0];
				*ptr_duplexPrint = static_cast<BYTE>(devmode.dmPublic.dmDuplex);
				*ptr_reversePrint = devmode.dmPrivate.bPaperReverseOrder;
				*ptr_tonerSaving = devmode.dmPrivate.graphics.TonerSaving;

				*((LPPCLDEVMODE)printer_info->pDevMode) = devmode;

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

	return true;
}

USBAPI_API int __stdcall OpenDocumentProperties(const TCHAR * strPrinterName,
	BYTE* ptr_paperSize,
	BYTE* ptr_paperOrientation,
	BYTE* ptr_mediaType,
	BYTE* ptr_paperOrder,
	BYTE* ptr_printQuality,//byte
	BYTE* ptr_scalingType,//byte
	BYTE* ptr_scalingRatio,
	BYTE* ptr_nupNum,//byte
	BYTE* ptr_typeofPB,//byte
	BYTE* ptr_posterType,
	BYTE* ptr_ADJColorBalance,
	BYTE* ptr_colorBalanceTo,
	BYTE* ptr_density,
	BYTE* ptr_duplexPrint,
	BYTE* ptr_reversePrint,//byte
	BYTE* ptr_tonerSaving)//byte
{
	HANDLE   phandle;
	LPPCLDEVMODE lpOutputData = NULL;
	LPPCLDEVMODE lpInputData = NULL;

	wchar_t szprintername[MAX_PATH] = { 0 };
	wcscpy_s(szprintername, MAX_PATH, strPrinterName);

	phandle = NULL;

	if (OpenPrinter(szprintername, &phandle, NULL))
	{
		DWORD dmsize;

		LPPRINTER_INFO_2 printer_info;
		GetPrinter(phandle, 2, (LPBYTE)NULL, 0, &dmsize);

		printer_info = (LPPRINTER_INFO_2)malloc(dmsize);

		if (printer_info != NULL)
		{
			if (GetPrinter(phandle, 2, (LPBYTE)printer_info, dmsize, &dmsize))
			{
				PCLDEVMODE inputDevmode;
				
				inputDevmode = *(LPPCLDEVMODE)printer_info->pDevMode;

				switch (*ptr_paperSize)
				{
				case 0:
					inputDevmode.dmPublic.dmPaperSize = DMPAPER_A4;
					break;
				case 1:
					inputDevmode.dmPublic.dmPaperSize = DMPAPER_LETTER;
					break;
				case 2:
					inputDevmode.dmPublic.dmPaperSize = DMPAPER_B5;
					break;
				case 3:
					inputDevmode.dmPublic.dmPaperSize = DMPAPER_A5;
					break;
				case 4:
					inputDevmode.dmPublic.dmPaperSize = DMPAPER_A5LEF;
					break;
				case 5:
					inputDevmode.dmPublic.dmPaperSize = DMPAPER_B6_JIS;
					break;
				case 6:
					inputDevmode.dmPublic.dmPaperSize = DMPAPER_B6LEF;
					break;
				case 7:
					inputDevmode.dmPublic.dmPaperSize = DMPAPER_A6;
					break;
				case 8:
					inputDevmode.dmPublic.dmPaperSize = DMPAPER_EXECUTIVE;
					break;
				case 9:
					inputDevmode.dmPublic.dmPaperSize = DMPAPER_P16K;
					break;
				case 10:
					inputDevmode.dmPublic.dmPaperSize = DMPAPER_USER;
					break;
				default:
					inputDevmode.dmPublic.dmPaperSize = DMPAPER_LETTER;
					break;
				}

				inputDevmode.dmPublic.dmOrientation = *ptr_paperOrientation;
				inputDevmode.dmPublic.dmMediaType = *ptr_mediaType + DMMEDIA_USER;
				inputDevmode.dmPrivate.par.wMediaType = *ptr_mediaType;
				inputDevmode.dmPublic.dmCollate = *ptr_paperOrder;
				inputDevmode.dmPrivate.PrintQuality = *ptr_printQuality;
				inputDevmode.dmPrivate.nup.bNupNum = *ptr_nupNum;//multiple-page 2in1: 2, 4in1: 4, 6in1: 6, 9in1: 9, 16 in1: 16

				inputDevmode.dmPrivate.bpmrdata.TypeofPB = *ptr_typeofPB;//multiple-page, 1in nxn pages
				inputDevmode.dmPrivate.poster.wPosterType = *ptr_posterType;// 0: 1 in 2x2, 1: 1 in 3x3, 2: 1 in 4x4 pages.

				inputDevmode.dmPrivate.sfp.ISFSet = *ptr_scalingType;// scale 
				inputDevmode.dmPrivate.sfp.SRatio = *ptr_scalingRatio;//scale 25~400

				inputDevmode.dmPrivate.graphics.isADJColorBalance = *ptr_ADJColorBalance;
				inputDevmode.dmPrivate.graphics.bColorBalanceTo = *ptr_colorBalanceTo;

				inputDevmode.dmPrivate.graphics.ColorBalanceIndex[0][0] = *ptr_density;

				inputDevmode.dmPublic.dmDuplex = *ptr_duplexPrint; //DUPLEX�� DMDUP_VERTICAL: ���� DMDUP_HORIZONTAL���̱�

				inputDevmode.dmPrivate.bPaperReverseOrder = *ptr_reversePrint;
				inputDevmode.dmPrivate.graphics.TonerSaving = *ptr_tonerSaving;


				dmsize = DocumentProperties(NULL, phandle, szprintername, NULL, NULL, 0);

				lpOutputData = (LPPCLDEVMODE)malloc(dmsize);
				lpInputData = (LPPCLDEVMODE)malloc(dmsize);

				*((LPPCLDEVMODE)printer_info->pDevMode) = inputDevmode;

				lpInputData = (LPPCLDEVMODE)printer_info->pDevMode;

				if (lpOutputData && lpInputData)
				{

					int iNeeded = DocumentProperties(NULL, phandle, szprintername,
						(LPDEVMODE)lpOutputData, (LPDEVMODE)lpInputData, DM_OUT_BUFFER | DM_IN_BUFFER | DM_IN_PROMPT);

					if (1 == iNeeded)
					{
						PCLDEVMODE devmode;
						devmode = *lpOutputData;

						switch (devmode.dmPublic.dmPaperSize)
						{
						case DMPAPER_A4:
							*ptr_paperSize = 0;
							break;
						case DMPAPER_LETTER:
							*ptr_paperSize = 1;
							break;
						case DMPAPER_B5:
							*ptr_paperSize = 2;
							break;
						case DMPAPER_A5:
							*ptr_paperSize = 3;
							break;
						case DMPAPER_A5LEF:
							*ptr_paperSize = 4;
							break;
						case DMPAPER_B6_JIS:
							*ptr_paperSize = 5;
							break;
						case DMPAPER_B6LEF:
							*ptr_paperSize = 6;
							break;
						case DMPAPER_A6:
							*ptr_paperSize = 7;
							break;
						case DMPAPER_EXECUTIVE:
							*ptr_paperSize = 8;
							break;
						case DMPAPER_P16K:
							*ptr_paperSize = 9;
							break;
						case DMPAPER_USER:
							*ptr_paperSize = 10;
							break;
						default:
							*ptr_paperSize = 1;
							break;
						}

						*ptr_paperOrientation = static_cast<BYTE>(devmode.dmPublic.dmOrientation);						
						*ptr_mediaType = static_cast<BYTE>(devmode.dmPrivate.par.wMediaType);
						*ptr_paperOrder = static_cast<BYTE>(devmode.dmPublic.dmCollate);						
						*ptr_printQuality = static_cast<short>(devmode.dmPrivate.PrintQuality);
						*ptr_nupNum = devmode.dmPrivate.nup.bNupNum;
						*ptr_typeofPB = devmode.dmPrivate.bpmrdata.TypeofPB;
						*ptr_posterType = static_cast<BYTE>(devmode.dmPrivate.poster.wPosterType);
						*ptr_scalingType = devmode.dmPrivate.sfp.ISFSet;
						*ptr_scalingRatio = static_cast<BYTE>(devmode.dmPrivate.sfp.SRatio);
						*ptr_ADJColorBalance = devmode.dmPrivate.graphics.isADJColorBalance;
						*ptr_colorBalanceTo = devmode.dmPrivate.graphics.bColorBalanceTo;
						*ptr_density = devmode.dmPrivate.graphics.ColorBalanceIndex[0][0];
						*ptr_duplexPrint = static_cast<BYTE>(devmode.dmPublic.dmDuplex);
						*ptr_reversePrint = devmode.dmPrivate.bPaperReverseOrder;
						*ptr_tonerSaving = devmode.dmPrivate.graphics.TonerSaving;

						*((LPPCLDEVMODE)printer_info->pDevMode) = devmode;

//						SetPrinter(phandle, 2, (LPBYTE)printer_info, 0);
					}
				}
			}
			free(printer_info);
		}
	}
	if (lpOutputData)
		free(lpOutputData);

	if (phandle != NULL)
	{
		ClosePrinter(phandle);
		phandle = NULL;
	}
	return true;
}
