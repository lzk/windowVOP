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
#include <math.h>
#include <cctype>

#pragma comment(lib, "Shlwapi.lib")
#pragma comment(lib, "gdiplus.lib")

#pragma pack(8)


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

enum DuplexPrintType
{
	NonDuplex,
	FlipOnLongEdge,
	FlipOnShortEdge
};

enum PrintError
{
	Print_Memory_Fail,
	Print_Insufficient_Memory_Or_Disk_Space,
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

USBAPI_API BOOL __stdcall IsMetricCountry();

USBAPI_API BOOL __stdcall PrintInitDialog(const TCHAR * jobDescription, HWND hwnd);
USBAPI_API int __stdcall GetPaperNames(TCHAR * strPrinterName, SAFEARRAY** paperNames);
USBAPI_API int __stdcall PrintFile(const TCHAR * strPrinterName, const TCHAR * strFileName, bool fitToPage, int duplexType, bool IsPortrait, int copies, int scalingValue);
USBAPI_API BOOL __stdcall PrintInit(const TCHAR * strPrinterName, const TCHAR * jobDescription, int idCardType, IdCardSize *size, bool fitToPage, int duplexType, bool IsPortrait, int scalingValue);
USBAPI_API void __stdcall AddImagePath(const TCHAR * fileName);
USBAPI_API void __stdcall AddImageSource(IStream * imageSource);
USBAPI_API void __stdcall AddImageRotation(int rotation);
USBAPI_API int __stdcall DoPrintImage();
USBAPI_API int __stdcall DoPrintIdCard();
USBAPI_API int __stdcall SaveDefaultPrinter();
USBAPI_API int __stdcall ResetDefaultPrinter();
USBAPI_API int __stdcall VopSetDefaultPrinter(const TCHAR * strPrinterName);
USBAPI_API int __stdcall DoPrintIdCard();
USBAPI_API void __stdcall SavePrinterSettingsData(const TCHAR * strPrinterName,
	UINT8 PaperSize,
	UINT8 PaperOrientation,
	UINT8 MediaType,
	UINT8 PaperOrder,
	UINT8 PrintQuality,//byte
	UINT8 ScalingType,//byte
	UINT16 DrvScalingRatio,
	UINT8 NupNum,//byte
	UINT8 TypeofPB,//byte
	UINT8 PosterType,
	UINT8 ADJColorBalance,
	UINT8 ColorBalanceTo,
	UINT8 Density,
	UINT8 DuplexPrint,
	UINT8 DocumentStyle,
	UINT8 ReversePrint,//byte
	UINT8 TonerSaving,
	UINT8 Copies,
	UINT8 Booklet,
	UINT8 Watermark);
USBAPI_API void __stdcall GetPrinterDefaultInfo(const TCHAR * strPrinterName);
USBAPI_API void __stdcall SetPrinterInfo(const TCHAR * strPrinterName, UINT8 m_PrintType);
USBAPI_API void __stdcall SetPrinterSettingsInitData();
USBAPI_API void __stdcall GetPrinterSettingsData(
	BYTE* ptr_paperSize,
	BYTE* ptr_paperOrientation,
	BYTE* ptr_mediaType,
	BYTE* ptr_paperOrder,
	BYTE* ptr_printQuality,//byte
	BYTE* ptr_scalingType,//byte
	UINT16* ptr_drvScalingRatio,
	BYTE* ptr_nupNum,//byte
	BYTE* ptr_typeofPB,//byte
	BYTE* ptr_posterType,
	BYTE* ptr_ADJColorBalance,
	BYTE* ptr_colorBalanceTo,
	BYTE* ptr_density,
	BYTE* ptr_duplexPrint,
	BYTE* ptr_documentStyle,
	BYTE* ptr_reversePrint,//byte
	BYTE* ptr_tonerSaving,
	BYTE* ptr_copies,
	BYTE* ptr_booklet,
	BYTE* ptr_watermark);//byte

USBAPI_API int __stdcall GetPrinterInfo(const TCHAR * strPrinterName,
	BYTE* ptr_paperSize,
	BYTE* ptr_paperOrientation,
	BYTE* ptr_mediaType,
	BYTE* ptr_paperOrder,
	BYTE* ptr_printQuality,//byte
	BYTE* ptr_scalingType,//byte
	UINT16* ptr_drvScalingRatio,
	BYTE* ptr_nupNum,//byte
	BYTE* ptr_typeofPB,//byte
	BYTE* ptr_posterType,
	BYTE* ptr_ADJColorBalance,
	BYTE* ptr_colorBalanceTo,
	BYTE* ptr_density,
	BYTE* ptr_duplexPrint,
	BYTE* ptr_documentStyle,
	BYTE* ptr_reversePrint,//byte
	BYTE* ptr_tonerSaving,
	BYTE* ptr_copies,
	BYTE* ptr_booklet,
	BYTE* ptr_watermark);//byte

USBAPI_API int __stdcall OpenDocumentProperties(HWND hWnd,const TCHAR * strPrinterName,
	BYTE* ptr_paperSize,
	BYTE* ptr_paperOrientation,
	BYTE* ptr_mediaType,
	BYTE* ptr_paperOrder,
	BYTE* ptr_printQuality,//byte
	BYTE* ptr_scalingType,//byte
	UINT16* ptr_drvScalingRatio,
	BYTE* ptr_nupNum,//byte
	BYTE* ptr_typeofPB,//byte
	BYTE* ptr_posterType,
	BYTE* ptr_ADJColorBalance,
	BYTE* ptr_colorBalanceTo,
	BYTE* ptr_density,
	BYTE* ptr_duplexPrint,
	BYTE* ptr_documentStyle,
	BYTE* ptr_reversePrint,//byte
	BYTE* ptr_tonerSaving,
	BYTE* ptr_copies,
	BYTE* ptr_booklet,
	BYTE* ptr_watermark);//byte

USBAPI_API void __stdcall SetCopies(const TCHAR * strPrinterName, UINT8 Copies);
USBAPI_API void __stdcall InitPrinterData(const TCHAR * strPrinterName);
USBAPI_API void __stdcall RecoverDevModeData();
USBAPI_API void __stdcall SetInitData(const TCHAR * strPrinterName);

static std::vector<std::wstring> g_vecImagePaths;
static std::vector<IStream*>  g_vecIdCardImageSources;
static std::vector<int>  g_vecIdCardImageRotation;
static DOCINFO  di = { sizeof (DOCINFO) };
static HDC dc = NULL;
static PRINTDLGEX pdx;
LPPRINTPAGERANGE pPageRanges = NULL;


static Gdiplus::GdiplusStartupInput gdiplusStartupInput;
static ULONG_PTR gdiplusToken;
static int currentIdCardType = 0;
static IdCardSize currentIdCardSize = { 0 };
static bool needFitToPage = true;
static bool IsPrintSettingPortrait = true;
static DuplexPrintType currentDuplexType = NonDuplex;
static int ScalingValue = 100;

static PCLDEVMODE getdevmode;
static PCLDEVMODE getDocumentPropertiesData;
static PCLDEVMODE getOutputData;
static PirntSettingsData g_PrintSettingsData;
static bool isOpenDocumentProperties = false;
static const TCHAR * g_strPrinterName = NULL;

static DWORD bufferSize = 500;
static TCHAR defaultPrinterName[500];
static bool IsInitPrinterName = true;

static int TcsNiCmp(TCHAR* c1, TCHAR* c2)
{
	int iNum = _tcslen(c1) > _tcslen(c2) ? _tcslen(c1) : _tcslen(c2);
	return _tcsnicmp(c1, c2, iNum);
}

USBAPI_API int __stdcall CheckPrinterStatus(WCHAR* strPrinterName);

USBAPI_API int __stdcall SaveDefaultPrinter()
{
	if (::GetDefaultPrinter(defaultPrinterName, &bufferSize))
	{
		return Print_OK;
	}
	else
	{
		return Print_Get_Default_Printer_Fail;
	}
}

USBAPI_API int __stdcall ResetDefaultPrinter()
{
	if (::SetDefaultPrinter(defaultPrinterName))
	{
		return Print_OK;
	}
	else
	{
		return Print_Get_Default_Printer_Fail;
	}
}

USBAPI_API int __stdcall VopSetDefaultPrinter(const TCHAR * strPrinterName)
{
	if (::SetDefaultPrinter(strPrinterName))
	{
		return Print_OK;
	}
	else
	{
		return Print_Get_Default_Printer_Fail;
	}
}

long CreateSafeArrayFromBSTRArray
(
BSTR* pBSTRArray,
ULONG ulArraySize,
SAFEARRAY** ppSafeArrayReceiver
)
{
	HRESULT hrRetTemp = S_OK;
	SAFEARRAY* pSAFEARRAYRet = NULL;
	SAFEARRAYBOUND rgsabound[1];
	ULONG ulIndex = 0;
	long lRet = 0;

	// Initialise receiver.
	if (ppSafeArrayReceiver)
	{
		*ppSafeArrayReceiver = NULL;
	}

	if (pBSTRArray)
	{
		rgsabound[0].lLbound = 0;
		rgsabound[0].cElements = ulArraySize;

		pSAFEARRAYRet = (SAFEARRAY*)SafeArrayCreate
			(
			(VARTYPE)VT_BSTR,
			(unsigned int)1,
			(SAFEARRAYBOUND*)rgsabound
			);
	}

	for (ulIndex = 0; ulIndex < ulArraySize; ulIndex++)
	{
		long lIndexVector[1];

		lIndexVector[0] = ulIndex;

		// Since pSAFEARRAYRet is created as a SafeArray of VT_BSTR,
		// SafeArrayPutElement() will create a copy of each BSTR
		// inserted into the SafeArray.
		SafeArrayPutElement
			(
			(SAFEARRAY*)pSAFEARRAYRet,
			(long*)lIndexVector,
			(void*)(pBSTRArray[ulIndex])
			);
	}

	if (pSAFEARRAYRet)
	{
		*ppSafeArrayReceiver = pSAFEARRAYRet;
	}

	return lRet;
}

USBAPI_API int __stdcall GetPaperNames(TCHAR * strPrinterName, SAFEARRAY** paperNames)
{
	TCHAR pBuffer[100][64] = { 0 };
	BSTR bstrArray[100] = { 0 };
	HANDLE hPrinter = NULL;
	PDEVMODE lpDefaultData = NULL;
	PDEVMODE lpInitData = NULL;
	DWORD dwSize;

	if (OpenPrinter(strPrinterName, &hPrinter, NULL))
	{
		long dmsize;
		dmsize = DocumentProperties(NULL, hPrinter, strPrinterName, NULL, NULL, 0);

		lpDefaultData = (PDEVMODE)malloc(dmsize);
		lpInitData = (PDEVMODE)malloc(dmsize);

		while (lpDefaultData == NULL && lpInitData == NULL)
		{
			dmsize = DocumentProperties(NULL, hPrinter, strPrinterName, NULL, NULL, 0);
			lpDefaultData = (PDEVMODE)malloc(dmsize);
			lpInitData = (PDEVMODE)malloc(dmsize);
		}

		if (lpDefaultData && lpInitData)
		{
			DocumentProperties(NULL, hPrinter, strPrinterName,
				lpDefaultData, NULL, DM_OUT_BUFFER);

			DocumentProperties(NULL, hPrinter, strPrinterName,
				lpInitData, lpDefaultData, DM_IN_BUFFER | DM_OUT_BUFFER);

			dwSize = DeviceCapabilities(strPrinterName, NULL, DC_PAPERNAMES, NULL, NULL);
			
			if (dwSize)
			{
				if (pBuffer)
				{
					DeviceCapabilities(strPrinterName, NULL, DC_PAPERNAMES, *pBuffer, lpInitData);

					for (UINT i = 0; i < dwSize; i++)
					{
						//wcscpy(paperNamesLocal[i], pBuffer[i]);
						bstrArray[i] = ::SysAllocString(pBuffer[i]);
					}

					CreateSafeArrayFromBSTRArray
						(
						bstrArray,
						dwSize,
						paperNames
						);

					for (UINT i = 0; i < dwSize; i++)
					{
						::SysFreeString(bstrArray[i]);
					}

				}
			}

			free(lpDefaultData);
			free(lpInitData);
		}
	}

	ClosePrinter(hPrinter);

	return 1;
}

USBAPI_API int __stdcall PrintFile(const TCHAR * strPrinterName, const TCHAR * strFileName, bool fitToPage, int duplexType, bool IsPortrait, int copies, int scalingValue)
{
	PrintError error = Print_OK;
	
	int shellExeRes = 0;
	int count = 1;
	const TCHAR *fileExt = NULL;
	const TCHAR *fileName = NULL;
	SHELLEXECUTEINFO ShExecInfo;

	ShExecInfo.cbSize = sizeof(SHELLEXECUTEINFO);
	ShExecInfo.fMask = SEE_MASK_FLAG_NO_UI;
	ShExecInfo.hwnd = NULL;
	ShExecInfo.lpVerb = L"Print";
	ShExecInfo.lpFile = strFileName;
	ShExecInfo.lpParameters = NULL;
	ShExecInfo.lpDirectory = NULL;
	ShExecInfo.nShow = SW_SHOW;
	ShExecInfo.hInstApp = NULL;

	fileExt = PathFindExtension(strFileName);
	std::wstring strExt(fileExt);
	std::transform(strExt.begin(), strExt.end(), strExt.begin(), std::tolower);

	fileName = PathFindFileName(strFileName);

	/*if (   _tcscmp(fileExt, L".bmp") == 0
		|| _tcscmp(fileExt, L".ico") == 0
		|| _tcscmp(fileExt, L".gif") == 0
		|| _tcscmp(fileExt, L".jpg") == 0
		|| _tcscmp(fileExt, L".exif") == 0
		|| _tcscmp(fileExt, L".png") == 0
		|| _tcscmp(fileExt, L".tif") == 0
		|| _tcscmp(fileExt, L".wmf") == 0
		|| _tcscmp(fileExt, L".emf") == 0)*/
	if (    strExt.compare(L".bmp") == 0
		||  strExt.compare(L".ico") == 0
		||  strExt.compare(L".gif") == 0
		||  strExt.compare(L".jpg") == 0
		||  strExt.compare(L".exif") == 0
		||  strExt.compare(L".png") == 0
		||  strExt.compare(L".tif") == 0
		||  strExt.compare(L".wmf") == 0
		||  strExt.compare(L".emf") == 0)
	{
		if (PrintInit(strPrinterName, fileName, 0, NULL, fitToPage, duplexType, IsPortrait, scalingValue))
		{
			AddImagePath(strFileName);
			DoPrintImage();
		}
		else
		{
			error = Print_Operation_Fail;

			char Debug[256] = "PrintInit fail";
			OutputDebugStringA(Debug);
		}
	}
	else
	{
		if (::SetDefaultPrinter(strPrinterName) == TRUE)
		{
			CoInitializeEx(NULL, COINIT_APARTMENTTHREADED | COINIT_DISABLE_OLE1DDE);

			if (strExt.compare(L".txt") == 0)
			{
				count = copies;
			}

			for (int i = 0; i < count; i++)
			{

				BOOL res = ::ShellExecuteEx(&ShExecInfo);

				if (res == FALSE)
				{
					if ((int)ShExecInfo.hInstApp == SE_ERR_OOM || (int)ShExecInfo.hInstApp == 0)
					{
						error = Print_Memory_Fail;
					}
					else
					{
						error = Print_File_Not_Support;
					}
				}
			}
	
		}
		else
		{
			error = Print_Get_Default_Printer_Fail;
		}
	}

	return error;
}

USBAPI_API BOOL __stdcall PrintInitDialog(const TCHAR * jobDescription, HWND hwnd)
{
	int ret = 0;

	g_vecImagePaths.clear();

	ZeroMemory(&di, sizeof(di));
	di.cbSize = sizeof(di);
	di.lpszDocName = jobDescription;

	// Allocate an array of PRINTPAGERANGE structures.
	pPageRanges = (LPPRINTPAGERANGE)GlobalAlloc(GPTR, 10 * sizeof(PRINTPAGERANGE));
	if (!pPageRanges)
		return -30;

	pdx.lStructSize = sizeof(PRINTDLGEX);
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
		return -30;

	if (NULL == pdx.hDC)
		return -30;

	dc = pdx.hDC;

	DEVMODE *devmode = (DEVMODE*)::GlobalLock(pdx.hDevMode);
	if (wcslen(devmode->dmDeviceName))
	{
		ret = CheckPrinterStatus(devmode->dmDeviceName);
	}

	Gdiplus::Status status;
	if ((status = Gdiplus::GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL)) != Gdiplus::Ok)
	{
		int errorCode = GetLastError();
		char Debug[256] = "";
		_snprintf(Debug, sizeof(Debug), "\nPrintInit GdiplusStartup Fail %d", errorCode);
		OutputDebugStringA(Debug);
		return -30;
	}

	return ret;
}

USBAPI_API BOOL __stdcall PrintInit(const TCHAR * strPrinterName, const TCHAR * jobDescription, int idCardType, IdCardSize *size, bool fitToPage, int duplexType, bool IsPortrait, int scalingValue)
{
	g_vecImagePaths.clear();
	g_vecIdCardImageSources.clear();
	g_vecIdCardImageRotation.clear();

	currentIdCardType = idCardType;
	needFitToPage = fitToPage;
	currentDuplexType = (DuplexPrintType)duplexType;
	IsPrintSettingPortrait = IsPortrait;
	ScalingValue = scalingValue;

	if (size != NULL)
		currentIdCardSize = *size;

	ZeroMemory(&di, sizeof(di));
	di.cbSize = sizeof(di);

	di.lpszDocName = jobDescription;

	dc = CreateDCW(L"WINSPOOL", strPrinterName, NULL, NULL);

	if (dc == NULL)
	{
		int errorCode = GetLastError();
		TCHAR Debug[256] = L"";
		_snwprintf(Debug, sizeof(Debug), L"\nPrintInit CreateDCW error code %d name %s", errorCode, strPrinterName);
		OutputDebugString(Debug);
		return FALSE;
	}
		

	Gdiplus::Status status;
	if ((status = Gdiplus::GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL)) != Gdiplus::Ok)
	{
		int errorCode = GetLastError();
		char Debug[256] = "";
		_snprintf(Debug, sizeof(Debug), "\nPrintInit GdiplusStartup Fail %d", errorCode);
		OutputDebugStringA(Debug);
		return FALSE;
	}

	return TRUE;
}

USBAPI_API void __stdcall AddImagePath(const TCHAR * fileName)
{
	std::wstring str(fileName);
	g_vecImagePaths.push_back(str);
}

USBAPI_API void __stdcall AddImageSource(IStream * imageSource)
{
	g_vecIdCardImageSources.push_back(imageSource);
}

USBAPI_API void __stdcall AddImageRotation(int rotation)
{
	g_vecIdCardImageRotation.push_back(rotation);
}


BOOL GetJobs(HANDLE hPrinter, JOB_INFO_2 **ppJobInfo, int *pcJobs, DWORD *pStatus, DWORD *pAttributes)
{
	DWORD       cByteNeeded, nReturned, cByteUsed;
	JOB_INFO_2          *pJobStorage = NULL;
	PRINTER_INFO_2       *pPrinterInfo = NULL;

	if (!GetPrinter(hPrinter, 2, NULL, 0, &cByteNeeded))
	{
		DWORD dwErrorCode = ::GetLastError();
		if (dwErrorCode != ERROR_INSUFFICIENT_BUFFER)
			return FALSE;
	}
	pPrinterInfo = (PRINTER_INFO_2 *)malloc(cByteNeeded);
	if (!(pPrinterInfo))
		return FALSE;

	if (!GetPrinter(hPrinter, 2, (LPBYTE)pPrinterInfo, cByteNeeded, &cByteUsed))
	{
		free(pPrinterInfo);
		pPrinterInfo = NULL;
		return FALSE;
	}
	*pAttributes = pPrinterInfo->Attributes;

	if (!EnumJobs(hPrinter, 0, pPrinterInfo->cJobs, 2, NULL, 0, (LPDWORD)&cByteNeeded, (LPDWORD)&nReturned))
	{
		if (GetLastError() != ERROR_INSUFFICIENT_BUFFER)
		{
			free(pPrinterInfo);
			pPrinterInfo = NULL;
			return FALSE;
		}
	}
	pJobStorage = (JOB_INFO_2 *)malloc(cByteNeeded);
	if (!pJobStorage)
	{
		free(pPrinterInfo);
		pPrinterInfo = NULL;
		return FALSE;
	}
	ZeroMemory(pJobStorage, cByteNeeded);
	if (!EnumJobs(hPrinter, 0, pPrinterInfo->cJobs, 2, (LPBYTE)pJobStorage, cByteNeeded,
		(LPDWORD)&cByteUsed, (LPDWORD)&nReturned))
	{
		free(pPrinterInfo);
		free(pJobStorage);
		pJobStorage = NULL;
		pPrinterInfo = NULL;
		return FALSE;
	}
	*pcJobs = nReturned;
	*pStatus = pPrinterInfo->Status;
	*ppJobInfo = pJobStorage;
	free(pPrinterInfo);
	return TRUE;
}

USBAPI_API int __stdcall CheckPrinterStatus(WCHAR* strPrinterName)
{
	DWORD value = 0;
	HANDLE printerHandle;
	JOB_INFO_2  *pJobs = NULL;
	int         cJobs = 0, i;
	DWORD       dwPrinterStatus, dwAttributes;
	DWORD status = 0;
	int result = 0;

	if (!OpenPrinter(strPrinterName, &printerHandle, NULL))
	{
		value = GetLastError();
		TCHAR Debug[256] = L"";
		_snwprintf_s(Debug, sizeof(Debug), L"\n OpenPrinter() error:  the return value %d", value);
		OutputDebugString(Debug);
		status = -25;//PRINTER_STATUS_USER_INTERVENTION;
		goto getLastStatus;
	}

	GetJobs(printerHandle, &pJobs, &cJobs, &dwPrinterStatus, &dwAttributes);

	if (dwAttributes & PRINTER_ATTRIBUTE_WORK_OFFLINE)
	{
		OutputDebugString(L"\n the Printer is offline!");
		status = -14;//PRINTER_STATUS_OFFLINE;
		goto getLastStatus;
	}

	if (dwPrinterStatus & PRINTER_STATUS_PAUSED)
	{
		OutputDebugString(L"\n The printer job is paused!");
		status = -2;//PRINTER_STATUS_PAUSED;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_BUSY)
	{
		OutputDebugString(L"\n The printer is busy!");
		status = -3;//PRINTER_STATUS_BUSY;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_WAITING)
	{
		OutputDebugString(L"\n The printer is waiting!");
		status = -4;//PRINTER_STATUS_WAITING;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_SERVER_OFFLINE)
	{
		OutputDebugString(L"\n The printer is not connect printer server!");
		status = -5;//PRINTER_STATUS_SERVER_OFFLINE;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_ERROR)
	{
		OutputDebugString(L"\n The printer is at wrong status!");
		status = -6;//PRINTER_STATUS_ERROR;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_PAPER_JAM)
	{
		OutputDebugString(L"\n The printer is paper jam!");
		status = -7;//PRINTER_STATUS_PAPER_JAM;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_PAPER_OUT)
	{
		OutputDebugString(L"\n The printer is paper out!");
		status = -8;//PRINTER_STATUS_PAPER_OUT;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_PAPER_PROBLEM)
	{
		OutputDebugString(L"\n The printer is paper problem!");
		status = -9;//PRINTER_STATUS_PAPER_PROBLEM;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_OUTPUT_BIN_FULL)
	{
		OutputDebugString(L"\n The printer is output bin full!");
		status = -10;//PRINTER_STATUS_OUTPUT_BIN_FULL;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_NOT_AVAILABLE)
	{
		OutputDebugString(L"\n The printer is not avalible!");
		status = -11;//PRINTER_STATUS_NOT_AVAILABLE;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_TONER_LOW)
	{
		OutputDebugString(L"\n The printer is toner low!");
		status = -12;//PRINTER_STATUS_TONER_LOW;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_NO_TONER)
	{
		OutputDebugString(L"\n The printer is no toner!");
		status = -28;//PRINTER_STATUS_NO_TONER;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_OUT_OF_MEMORY)
	{
		OutputDebugString(L"\n The printer is out of memory!");
		status = -13;//PRINTER_STATUS_OUT_OF_MEMORY;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_OFFLINE)
	{
		OutputDebugString(L"\n The printer is offline!");
		status = -14;//PRINTER_STATUS_OFFLINE;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_DOOR_OPEN)
	{
		OutputDebugString(L"\n The printer door is open!");
		status = -15;//PRINTER_STATUS_DOOR_OPEN;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_POWER_SAVE)
	{
		OutputDebugString(L"\n The printer is in power-save mode!");
		status = -16;//PRINTER_STATUS_POWER_SAVE;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_PENDING_DELETION)
	{
		OutputDebugString(L"\n The printer is being deleted as a result of a client's call to RpcDeletePrinter. No new jobs can be submitted on existing printer objects for that printer!");
		status = -17;//PRINTER_STATUS_PENDING_DELETION;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_MANUAL_FEED)
	{
		OutputDebugString(L"\n The printer is in a manual feed state!");
		status = -18;//PRINTER_STATUS_MANUAL_FEED;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_IO_ACTIVE)
	{
		OutputDebugString(L"\n The printer is in an active input and output state!");
		status = -19;//PRINTER_STATUS_IO_ACTIVE;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_PRINTING)
	{
		OutputDebugString(L"\n The printer is printing!");
		status = -20;//PRINTER_STATUS_PRINTING;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_PROCESSING)
	{
		OutputDebugString(L"\n The printer is processing a job!");
		status = -21;//PRINTER_STATUS_PROCESSING;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_INITIALIZING)
	{
		OutputDebugString(L"\n The printer is initializing!");
		status = -22;//PRINTER_STATUS_INITIALIZING;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_WARMING_UP)
	{
		OutputDebugString(L"\n The printer is warm up!");
		status = -23;//PRINTER_STATUS_WARMING_UP;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_PAGE_PUNT)
	{
		OutputDebugString(L"\n The printer can not print current page!");
		status = -24;//PRINTER_STATUS_PAGE_PUNT;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_USER_INTERVENTION)
	{
		OutputDebugString(L"\n The printer is intervention!");
		status = -25;//PRINTER_STATUS_USER_INTERVENTION;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_SERVER_UNKNOWN)
	{
		OutputDebugString(L"\n The printer is unknown!");
		status = -26;//PRINTER_STATUS_SERVER_UNKNOWN;
		goto getLastStatus;
	}
	else if (dwPrinterStatus & PRINTER_STATUS_DRIVER_UPDATE_NEEDED)
	{
		OutputDebugString(L"\n The printer driver is update needed!");
		status = -27;//PRINTER_STATUS_DRIVER_UPDATE_NEEDED;
		goto getLastStatus;
	}


	if (cJobs > 0 && pJobs != NULL)
	{
		for (i = 0; i < cJobs; ++i)
		{
			/// �����ӡҳ���ڴ�ӡ
			if (pJobs[i].Status & JOB_STATUS_PRINTING)
			{
				OutputDebugString(L"\n The printer is on working!");
				status = -20;//PRINTER_STATUS_PRINTING;
				goto getLastStatus;
			}
			else if (pJobs[i].Status & JOB_STATUS_ERROR)
			{
				OutputDebugString(L"\n The job  is error!");
				status = -6;//PRINTER_STATUS_ERROR;
				goto getLastStatus;
			}
			else if (pJobs[i].Status & JOB_STATUS_OFFLINE)
			{
				OutputDebugString(L"\n The printer  is offline!");
				status = -14;//PRINTER_STATUS_OFFLINE;
				goto getLastStatus;
			}
			else if (pJobs[i].Status & JOB_STATUS_PAPEROUT)
			{
				OutputDebugString(L"\n The Printer  is paper out!");
				status = -8;//PRINTER_STATUS_PAPER_OUT;
				goto getLastStatus;
			}
			else if (pJobs[i].Status & JOB_STATUS_BLOCKED_DEVQ)
			{
				OutputDebugString(L"\n The printer can not print the job!");
				status = -24;//PRINTER_STATUS_PAGE_PUNT;
				goto getLastStatus;
			}
			/// �����ӡҳ�Ѿ���ӡ
			else if (pJobs[i].Status & JOB_STATUS_PRINTED)
			{
				OutputDebugString(L"\n The job has printed!");
			}

			/// ����Ѿ�ɾ����ӡ��ҵ
			else if (pJobs[i].Status & JOB_STATUS_DELETED)
			{
				OutputDebugString(L"\n The job is deleted!");
			}

			else if (pJobs[i].Status & JOB_STATUS_PAUSED)
			{
				OutputDebugString(L"\n The printer is paused!");
				status = -2;//PRINTER_STATUS_PAUSED;
				goto getLastStatus;
			}
			else if (pJobs[i].Status & JOB_STATUS_ERROR)
			{
				OutputDebugString(L"\n The printer can not print the job!");
				status = -6;//PRINTER_STATUS_ERROR;
				goto getLastStatus;
			}
			else if (pJobs[i].Status & JOB_STATUS_SPOOLING)
			{
				status = -21;//PRINTER_STATUS_PROCESSING;
				OutputDebugString(L"\n The jobs is spooling!");
			}
			else if (pJobs[i].Status & JOB_STATUS_OFFLINE)
			{
				OutputDebugString(L"\n The printer  is offline!");
				status = -14;//PRINTER_STATUS_OFFLINE;
				goto getLastStatus;
			}
			else if (pJobs[i].Status & JOB_STATUS_PAPEROUT)
			{
				status = -8;//PRINTER_STATUS_PAPER_OUT;
				OutputDebugString(L"\n The printer is paper out!");
				goto getLastStatus;
			}
			else if (pJobs[i].Status & JOB_STATUS_RESTART)
			{
				status = -29;//JOB_STATUS_RESTART;
				OutputDebugString(L"\n The printer job is restart!");
			}
			else
			{
				goto getLastStatus;
			}
		}
	}

getLastStatus:

	if (pJobs)
	{
		free(pJobs);
		pJobs = NULL;
	}
	ClosePrinter(printerHandle);

	return status;
}
USBAPI_API int __stdcall DoPrintImage()
{
	PrintError error = Print_OK;
	Gdiplus::Status status;
	HDC   hdcPrn = dc;
	const TCHAR *fileExt = NULL;
	int   cxPage;
	int	  cyPage;

	GUID* pDimensionIDs = NULL;
	UINT frameCount = 0;

	UINT count = 0;
	UINT fIndex = 0;
	UINT pageCount = 0;
	BOOL IsFitted = FALSE;

	if (StartDoc(hdcPrn, &di) > 0)
	{
		// Start the page
		for (UINT i = 0; i < g_vecImagePaths.size(); i++)
		{
			fileExt = PathFindExtension(g_vecImagePaths[i].c_str());

			std::wstring strExt(fileExt);
			std::transform(strExt.begin(), strExt.end(), strExt.begin(), std::tolower);

			if (   strExt.compare(L".bmp") == 0
				|| strExt.compare(L".ico") == 0
				|| strExt.compare(L".gif") == 0
				|| strExt.compare(L".jpg") == 0
				|| strExt.compare(L".exif") == 0
				|| strExt.compare(L".png") == 0
				|| strExt.compare(L".tif") == 0
				|| strExt.compare(L".wmf") == 0
				|| strExt.compare(L".emf") == 0)
			{
				Gdiplus::Image *pImg = NULL;
				pImg = Gdiplus::Image::FromFile(g_vecImagePaths[i].c_str());
				

				status = pImg->GetLastStatus();
				if (status != Gdiplus::Ok)
				{
					if (pImg)
					{
						delete pImg;
						pImg = NULL;
					}
						
					if (status == Gdiplus::OutOfMemory)
					{
						error = Print_Memory_Fail;
					}
					else
					{
						error = Print_Insufficient_Memory_Or_Disk_Space;
					}
					break;
				}

				count = pImg->GetFrameDimensionsCount();

				if (count > 0)
				{
					pDimensionIDs = (GUID*)malloc(sizeof(GUID)*count);
					pImg->GetFrameDimensionsList(pDimensionIDs, count);
					frameCount = pImg->GetFrameCount(&pDimensionIDs[0]);

					fIndex = 0;
					while (fIndex < frameCount)
					{
						pImg->SelectActiveFrame(&pDimensionIDs[0], fIndex);

						if (StartPage(hdcPrn) < 0)
						{
							if (pImg)
							{
								delete pImg;
								pImg = NULL;
							}

							if (pDimensionIDs)
							{
								free(pDimensionIDs);
								pDimensionIDs = NULL;
							}
						
							//error = Print_Operation_Fail;
							break;
						}

						int x = 0; 
						int y = 0;
						double imageToLeft = 0;
						double imageToTop = 0;
						Gdiplus::REAL dpiX = pImg->GetHorizontalResolution();
						Gdiplus::REAL dpiY = pImg->GetVerticalResolution();
						cxPage = GetDeviceCaps(hdcPrn, HORZRES);
						cyPage = GetDeviceCaps(hdcPrn, VERTRES);

						int w = (int)round(pImg->GetWidth() * (600 / dpiX));
						int h = (int)round(pImg->GetHeight()* (600 / dpiY));
		
						//Combined document all needed protrait
						if ((IsPrintSettingPortrait && w > h && strExt.compare(L".tif") == 0 && frameCount > 1)
							|| (!IsPrintSettingPortrait && w < h && strExt.compare(L".tif") == 0 && frameCount > 1))
						{	
							int temp = cyPage;
							cyPage = cxPage;
							cxPage = temp;				
						}

						double whRatio = (double)w / h;
						double scaleRatioX = (double)w / cxPage;
						double scaleRatioY = (double)h / cyPage;

						Gdiplus::Graphics *pGraphics = NULL;
						pGraphics = Gdiplus::Graphics::FromHDC(hdcPrn);
						pGraphics->SetPageUnit(Gdiplus::UnitPixel);

						if (needFitToPage)
						{
							IsFitted = FALSE;
						}
						else
						{
							IsFitted = TRUE;
						}
						
						if (IsFitted == TRUE)
						{
							w = (int)round(pImg->GetWidth() * (600 / dpiX) * (ScalingValue / 100.0));
							h = (int)round(pImg->GetHeight()* (600 / dpiY) * (ScalingValue / 100.0));
							//x = 0; //Align Top left
							//y = 0;
							x = (cxPage - w) / 2;
							y = (cyPage - h) / 2;
						}
						else
						{
							if (scaleRatioX > scaleRatioY)
							{
								w = cxPage;
								h = (int)round(((double)cxPage / whRatio));
								y = (cyPage - h) / 2;
								//y = 0;
							}
							else if (scaleRatioX < scaleRatioY)
							{
								w = (int)round(((double)cyPage * whRatio));
								h = cyPage;
								x = (cxPage - w) / 2;
								//x = 0;
							}
							else
							{
								w = cxPage;
								h = cyPage;
							}
						}

						//Combined document all needed protrait
						if (IsPrintSettingPortrait && w > h && strExt.compare(L".tif") == 0 && frameCount > 1)
						{
							x = 0;
							y = 0;

							imageToLeft = (cyPage - h) / 2 + h;
							imageToTop = (cxPage - w) / 2;

							pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
							pGraphics->RotateTransform(90.0f);
						}
						else if (!IsPrintSettingPortrait && w < h && strExt.compare(L".tif") == 0 && frameCount > 1)
						{
							x = 0;
							y = 0;

							imageToLeft = (cyPage - h) / 2;
							imageToTop = (cxPage - w) / 2 + w;

							pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
							pGraphics->RotateTransform(-90.0f);
						}

					/*	if (pageCount % 2 == 1 && IsFitted == TRUE)
						{
							switch (currentDuplexType)
							{
							case FlipOnLongEdge:

								if (IsPrintSettingPortrait)
								{
									x = cxPage - w;
									y = 0;
								}
								else
								{
									x = 0;
									y = cyPage - h;
								}
							
								break;
							case FlipOnShortEdge:

								if (IsPrintSettingPortrait)
								{
									x = 0;
									y = cyPage - h;
								}
								else
								{
									x = cxPage - w;
									y = 0;
								}
							
								break;
							case NonDuplex:
								x = 0;
								y = 0;
								break;
							default:
								break;
							}
						}*/


						if ((status = pGraphics->DrawImage(pImg, x, y, w, h)) != Gdiplus::Ok)
						{
							if (pImg)
							{
								delete pImg;
								pImg = NULL;
							}

							if (pGraphics)
							{
								delete pGraphics;
								pGraphics = NULL;
							}
								
							if (pDimensionIDs)
							{
								free(pDimensionIDs);
								pDimensionIDs = NULL;
							}

							if (status == Gdiplus::OutOfMemory)
							{
								error = Print_Memory_Fail;
							}
							else
							{
								error = Print_Insufficient_Memory_Or_Disk_Space;
							}
							break;
						}
					

						if (pGraphics)
						{
							delete pGraphics;
							pGraphics = NULL;
						}

						fIndex++;

						if (EndPage(hdcPrn) < 0)
						{
							if (pImg)
							{
								delete pImg;
								pImg = NULL;
							}

							if (pDimensionIDs)
							{
								free(pDimensionIDs);
								pDimensionIDs = NULL;
							}

							//error = Print_Operation_Fail;
							break;
						}

						pageCount++;
					}

					if (pDimensionIDs)
					{
						free(pDimensionIDs);
						pDimensionIDs = NULL;
					}
				}

				if (pImg)
				{
					delete pImg;
					pImg = NULL;
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
	{
		error = Print_Operation_Fail;
		char Debug[256] = "DoPrintImage startdoc Fail";
		OutputDebugStringA(Debug);
	}
		

    int errorCode = GetLastError();

	char Debug[256] = "";
	_snprintf(Debug, sizeof(Debug), "\nDoPrintImage startdoc Fail %d", errorCode);
	OutputDebugStringA(Debug);


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

	Gdiplus::Status status;
	int docStatus;

	if ((docStatus = StartDoc(hdcPrn, &di)) > 0)
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
		
				Gdiplus::Image *pImg1 = NULL;
				pImg1 = Gdiplus::Image::FromStream(g_vecIdCardImageSources[0]);

				status = pImg1->GetLastStatus();
				if (status != Gdiplus::Ok)
				{
					if (pImg1)
						delete pImg1;

					if (status == Gdiplus::OutOfMemory)
					{
						error = Print_Memory_Fail;
					}
					else
					{
						error = Print_Insufficient_Memory_Or_Disk_Space;
					}
					break;
				}

				Gdiplus::Image *pImg2 = NULL;
				pImg2 = Gdiplus::Image::FromStream(g_vecIdCardImageSources[1]);

				status = pImg2->GetLastStatus();
				if (status != Gdiplus::Ok)
				{
					if (pImg1)
						delete pImg1;

					if (pImg2)
						delete pImg2;

					if (status == Gdiplus::OutOfMemory)
					{
						error = Print_Memory_Fail;
					}
					else
					{
						error = Print_Insufficient_Memory_Or_Disk_Space;
					}
					break;
				}
	
				if (StartPage(hdcPrn) < 0)
				{
					if (pImg1)
						delete pImg1;

					if (pImg2)
						delete pImg2;

					error = Print_Operation_Fail;
					break;
				}
			
				Gdiplus::Graphics *pGraphics = NULL;
				pGraphics = Gdiplus::Graphics::FromHDC(hdcPrn);
				pGraphics->SetPageUnit(Gdiplus::UnitPixel);
		
				switch (g_vecIdCardImageRotation[0])
				{
				case 0:
					imageWidth = (currentIdCardSize.Width / 2.54) * 600;
					imageHeight = (currentIdCardSize.Height / 2.54) * 600;

					imageToLeft = (cxPage - imageWidth) / 2;
					imageToTop = (cyPage / 2 - imageHeight) / 2;

					pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
					pGraphics->RotateTransform(0.0f);
					break;
				case 90:
					imageWidth = (currentIdCardSize.Height / 2.54) * 600;
					imageHeight = (currentIdCardSize.Width / 2.54) * 600;

					imageToLeft = (cxPage - imageHeight) / 2 + imageHeight;
					imageToTop = (cyPage / 2 - imageWidth) / 2;

					pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
					pGraphics->RotateTransform(90.0f);
					break;
				case 180:
					imageWidth = (currentIdCardSize.Width / 2.54) * 600;
					imageHeight = (currentIdCardSize.Height / 2.54) * 600;

					imageToLeft = (cxPage - imageWidth) / 2 + imageWidth;
					imageToTop = (cyPage / 2 - imageHeight) / 2 + imageHeight;

					pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
					pGraphics->RotateTransform(180.0f);
					break;
				case 270:
					imageWidth = (currentIdCardSize.Height / 2.54) * 600;
					imageHeight = (currentIdCardSize.Width / 2.54) * 600;

					imageToLeft = (cxPage - imageHeight) / 2;
					imageToTop = (cyPage / 2 - imageWidth) / 2 + imageWidth;

					pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
					pGraphics->RotateTransform(270.0f);
					break;
				}

				if ((status = pGraphics->DrawImage(pImg1, 0, 0, (int)round(imageWidth), (int)round(imageHeight))) != Gdiplus::Ok)
				{
					if (pImg1)
						delete pImg1;

					if (pImg2)
						delete pImg2;

					if (pGraphics)
					{
						delete pGraphics;
						pGraphics = NULL;
					}

					if (status == Gdiplus::OutOfMemory)
					{
						error = Print_Memory_Fail;
					}
					else
					{
						error = Print_Insufficient_Memory_Or_Disk_Space;
					}
					break;
				}

				pGraphics->ResetTransform();

				switch (g_vecIdCardImageRotation[1])
				{
				case 0:
					imageWidth = (currentIdCardSize.Width / 2.54) * 600;
					imageHeight = (currentIdCardSize.Height / 2.54) * 600;

					imageToLeft = (cxPage - imageWidth) / 2;
					imageToTop = (cyPage / 2 - imageHeight) / 2 + cyPage / 2;

					pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
					pGraphics->RotateTransform(0.0f);
					break;
				case 90:
					imageWidth = (currentIdCardSize.Height / 2.54) * 600;
					imageHeight = (currentIdCardSize.Width / 2.54) * 600;

					imageToLeft = (cxPage - imageHeight) / 2 + imageHeight;
					imageToTop = (cyPage / 2 - imageWidth) / 2 + cyPage / 2;

					pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
					pGraphics->RotateTransform(90.0f);
					break;
				case 180:
					imageWidth = (currentIdCardSize.Width / 2.54) * 600;
					imageHeight = (currentIdCardSize.Height / 2.54) * 600;

					imageToLeft = (cxPage - imageWidth) / 2 + imageWidth;
					imageToTop = (cyPage / 2 - imageHeight) / 2 + imageHeight + cyPage / 2;

					pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
					pGraphics->RotateTransform(180.0f);
					break;
				case 270:
					imageWidth = (currentIdCardSize.Height / 2.54) * 600;
					imageHeight = (currentIdCardSize.Width / 2.54) * 600;

					imageToLeft = (cxPage - imageHeight) / 2;
					imageToTop = (cyPage / 2 - imageWidth) / 2 + imageWidth + cyPage / 2;

					pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
					pGraphics->RotateTransform(270.0f);
					break;
				}

				if ((status = pGraphics->DrawImage(pImg2, 0, 0, (int)round(imageWidth), (int)round(imageHeight))) != Gdiplus::Ok)
				{
					if (pImg1)
						delete pImg1;

					if (pImg2)
						delete pImg2;

					if (pGraphics)
					{
						delete pGraphics;
						pGraphics = NULL;
					}

					if (status == Gdiplus::OutOfMemory)
					{
						error = Print_Memory_Fail;
					}
					else
					{
						error = Print_Insufficient_Memory_Or_Disk_Space;
					}
					break;
				}

				if (pGraphics)
				{
					delete pGraphics;
					pGraphics = NULL;
				}

				if (EndPage(hdcPrn) < 0)
				{
					if (pImg1)
						delete pImg1;

					if (pImg2)
						delete pImg2;

					error = Print_Operation_Fail;
					break;
				}

				if (pImg1)
					delete pImg1;

				if (pImg2)
					delete pImg2;
			}
			break;
		case MarriageCertificate:
			if (g_vecIdCardImageSources.size() == 1)
			{
				imageWidth = 0;
				imageHeight = 0;
				imageToLeft = 0;
				imageToTop = 0;

				Gdiplus::Image *pImg1 = NULL;
				pImg1 = Gdiplus::Image::FromStream(g_vecIdCardImageSources[0]);

				status = pImg1->GetLastStatus();
				if (status != Gdiplus::Ok)
				{
					if (pImg1)
						delete pImg1;

					if (status == Gdiplus::OutOfMemory)
					{
						error = Print_Memory_Fail;
					}
					else
					{
						error = Print_Insufficient_Memory_Or_Disk_Space;
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

				Gdiplus::Graphics *pGraphics = NULL;
				pGraphics = Gdiplus::Graphics::FromHDC(hdcPrn);
				pGraphics->SetPageUnit(Gdiplus::UnitPixel);

				switch (g_vecIdCardImageRotation[0])
				{
				case 0:
					imageWidth = (currentIdCardSize.Width / 2.54) * 600;
					imageHeight = (currentIdCardSize.Height / 2.54) * 600;

					imageToLeft = (cxPage - imageWidth) / 2;
					imageToTop = (cyPage - imageHeight) / 2;

					pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
					pGraphics->RotateTransform(0.0f);
					break;
				case 90:
					imageWidth = (currentIdCardSize.Height / 2.54) * 600;
					imageHeight = (currentIdCardSize.Width / 2.54) * 600;

					imageToLeft = (cxPage - imageHeight) / 2 + imageHeight;
					imageToTop = (cyPage - imageWidth) / 2;

					pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
					pGraphics->RotateTransform(90.0f);
					break;
				case 180:
					imageWidth = (currentIdCardSize.Width / 2.54) * 600;
					imageHeight = (currentIdCardSize.Height / 2.54) * 600;

					imageToLeft = (cxPage - imageWidth) / 2 + imageWidth;
					imageToTop = (cyPage - imageHeight) / 2 + imageHeight;

					pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
					pGraphics->RotateTransform(180.0f);
					break;
				case 270:
					imageWidth = (currentIdCardSize.Height / 2.54) * 600;
					imageHeight = (currentIdCardSize.Width / 2.54) * 600;

					imageToLeft = (cxPage - imageHeight) / 2;
					imageToTop = (cyPage - imageWidth) / 2 + imageWidth;

					pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
					pGraphics->RotateTransform(270.0f);
					break;
				}

				if ((status = pGraphics->DrawImage(pImg1, 0, 0, (int)round(imageWidth), (int)round(imageHeight))) != Gdiplus::Ok)
				{
					if (pImg1)
						delete pImg1;

					if (pGraphics)
					{
						delete pGraphics;
						pGraphics = NULL;
					}

					if (status == Gdiplus::OutOfMemory)
					{
						error = Print_Memory_Fail;
					}
					else
					{
						error = Print_Insufficient_Memory_Or_Disk_Space;
					}
					break;
				}

				if (pGraphics)
				{
					delete pGraphics;
					pGraphics = NULL;
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

				Gdiplus::Image *pImg1 = NULL;
				pImg1 = Gdiplus::Image::FromStream(g_vecIdCardImageSources[0]);

				status = pImg1->GetLastStatus();
				if (status != Gdiplus::Ok)
				{
					if (pImg1)
						delete pImg1;

					if (status == Gdiplus::OutOfMemory)
					{
						error = Print_Memory_Fail;
					}
					else
					{
						error = Print_Insufficient_Memory_Or_Disk_Space;
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

				Gdiplus::Graphics *pGraphics = NULL;
				pGraphics = Gdiplus::Graphics::FromHDC(hdcPrn);
				pGraphics->SetPageUnit(Gdiplus::UnitPixel);

				switch (g_vecIdCardImageRotation[0])
				{
				case 0:
					imageWidth = (currentIdCardSize.Width / 2.54) * 600;
					imageHeight = (currentIdCardSize.Height / 2.54) * 600;

					imageToLeft = (cxPage - imageHeight) / 2;
					imageToTop = (cyPage - imageWidth) / 2 + imageWidth;

					pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
					pGraphics->RotateTransform(270.0f);
					break;
				case 90:
					imageWidth = (currentIdCardSize.Height / 2.54) * 600;
					imageHeight = (currentIdCardSize.Width / 2.54) * 600;

					imageToLeft = (cxPage - imageWidth) / 2;
					imageToTop = (cyPage - imageHeight) / 2;

					pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
					pGraphics->RotateTransform(0.0f);
					break;
				case 180:
					imageWidth = (currentIdCardSize.Width / 2.54) * 600;
					imageHeight = (currentIdCardSize.Height / 2.54) * 600;

					imageToLeft = (cxPage - imageHeight) / 2 + imageHeight;
					imageToTop = (cyPage - imageWidth) / 2;

					pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
					pGraphics->RotateTransform(90.0f);
					break;
				case 270:
					imageWidth = (currentIdCardSize.Height / 2.54) * 600;
					imageHeight = (currentIdCardSize.Width / 2.54) * 600;

					imageToLeft = (cxPage - imageWidth) / 2 + imageWidth;
					imageToTop = (cyPage - imageHeight) / 2 + imageHeight;

					pGraphics->TranslateTransform((Gdiplus::REAL)imageToLeft, (Gdiplus::REAL)imageToTop);
					pGraphics->RotateTransform(180.0f);
					break;
				}


				if ((status = pGraphics->DrawImage(pImg1, 0, 0, (int)round(imageWidth), (int)round(imageHeight))) != Gdiplus::Ok)
				{
					if (pImg1)
						delete pImg1;

					if (pGraphics)
					{
						delete pGraphics;
						pGraphics = NULL;
					}

					if (status == Gdiplus::OutOfMemory)
					{
						error = Print_Memory_Fail;
					}
					else
					{
						error = Print_Insufficient_Memory_Or_Disk_Space;
					}
					break;
				}

				if (pGraphics)
				{
					delete pGraphics;
					pGraphics = NULL;
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

USBAPI_API void __stdcall SavePrinterSettingsData(
	UINT8 PaperSize,
	UINT8 PaperOrientation,
	UINT8 MediaType,
	UINT8 PaperOrder,
	UINT8 PrintQuality,//byte
	UINT8 ScalingType,//byte
	UINT16 DrvScalingRatio,
	UINT8 NupNum,//byte
	UINT8 TypeofPB,//byte
	UINT8 PosterType,
	UINT8 ADJColorBalance,
	UINT8 ColorBalanceTo,
	UINT8 Density,
	UINT8 DuplexPrint,
	UINT8 DocumentStyle,
	UINT8 ReversePrint,//byte
	UINT8 TonerSaving,
	UINT8 Copies,
	UINT8 Booklet,
	UINT8 Watermark)//byte
{
	g_PrintSettingsData.m_paperSize = PaperSize;
	g_PrintSettingsData.m_paperOrientation = PaperOrientation;
	g_PrintSettingsData.m_mediaType = MediaType;
	g_PrintSettingsData.m_paperOrder = PaperOrder;
	g_PrintSettingsData.m_printQuality = PrintQuality;
	g_PrintSettingsData.m_scalingType = ScalingType;
	g_PrintSettingsData.m_drvScalingRatio = DrvScalingRatio;
	g_PrintSettingsData.m_nupNum = NupNum;
	g_PrintSettingsData.m_typeofPB = TypeofPB;
	g_PrintSettingsData.m_posterType = PosterType;
	g_PrintSettingsData.m_ADJColorBalance = ADJColorBalance;
	g_PrintSettingsData.m_colorBalanceTo = ColorBalanceTo;
	g_PrintSettingsData.m_densityValue = Density - 4;
	g_PrintSettingsData.m_duplexPrint = DuplexPrint;
	g_PrintSettingsData.m_documentStyle = DocumentStyle;
	g_PrintSettingsData.m_reversePrint = ReversePrint;
	g_PrintSettingsData.m_tonerSaving = TonerSaving;
	g_PrintSettingsData.m_copies = Copies;
	g_PrintSettingsData.m_booklet = Booklet;
	g_PrintSettingsData.m_watermark = Watermark;

	getOutputData = getDocumentPropertiesData;

}
USBAPI_API void __stdcall SetPrinterSettingsInitData()
{
	g_PrintSettingsData.m_paperOrientation = 1;
	g_PrintSettingsData.m_mediaType = 0;
	g_PrintSettingsData.m_paperOrder = 1;
	g_PrintSettingsData.m_printQuality = 0;
	g_PrintSettingsData.m_scalingType = 0;
	g_PrintSettingsData.m_drvScalingRatio = 100;
	g_PrintSettingsData.m_nupNum = 1;
	g_PrintSettingsData.m_typeofPB = 0;
	g_PrintSettingsData.m_posterType = 0;
	g_PrintSettingsData.m_ADJColorBalance = 0;
	g_PrintSettingsData.m_colorBalanceTo = 0;
	g_PrintSettingsData.m_densityValue = 0;
	g_PrintSettingsData.m_duplexPrint = 1;
	g_PrintSettingsData.m_documentStyle = 0;
	g_PrintSettingsData.m_reversePrint = 1;
	g_PrintSettingsData.m_tonerSaving = 0;
	g_PrintSettingsData.m_copies = 1;
	g_PrintSettingsData.m_booklet = 0;
	g_PrintSettingsData.m_watermark = 0;
	OutputDebugString(L"SetPrinterSettingsInitData");
	BOOL bIsMetrice = IsMetricCountry();
	if (bIsMetrice)
	{
		g_PrintSettingsData.m_paperSize = 0;
		OutputDebugString(L"A4");
	}
	else
	{
		g_PrintSettingsData.m_paperSize = 1;
		OutputDebugString(L"Letter");
	}
	isOpenDocumentProperties = false;

}
USBAPI_API void __stdcall GetPrinterDefaultInfo(const TCHAR * strPrinterName)
{
	HANDLE   phandle;
	DWORD dmsize = 0;

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
				getdevmode = *(LPPCLDEVMODE)printer_info->pDevMode;
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
USBAPI_API void __stdcall SetPrinterInfo(const TCHAR * strPrinterName, UINT8 m_PrintType)//byte
{
	HANDLE   phandle;
	DWORD dmsize = 0;

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

				if (isOpenDocumentProperties)
				{
					devmode = getOutputData;
				}
				else
				{
					devmode = *(LPPCLDEVMODE)printer_info->pDevMode;
				}
				if (g_PrintSettingsData.m_paperSize > 10)
				{
					devmode.dmPublic.dmPaperSize = DMPAPER_USER + (g_PrintSettingsData.m_paperSize - 10);
				}
				else
				{
					switch (g_PrintSettingsData.m_paperSize)
					{
					case 0:
						devmode.dmPublic.dmPaperSize = DMPAPER_A4;
						OutputDebugString(L"A4");
						break;
					case 1:
						devmode.dmPublic.dmPaperSize = DMPAPER_LETTER;
						OutputDebugString(L"Letter");
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
				}				

				devmode.dmPublic.dmOrientation = g_PrintSettingsData.m_paperOrientation;
				devmode.dmPublic.dmMediaType = g_PrintSettingsData.m_mediaType + DMMEDIA_USER;
				devmode.dmPrivate.par.wMediaType = g_PrintSettingsData.m_mediaType;
				devmode.dmPublic.dmCollate = g_PrintSettingsData.m_paperOrder;
				devmode.dmPrivate.PrintQuality = static_cast<BYTE>(g_PrintSettingsData.m_printQuality);
				devmode.dmPrivate.nup.bNupNum = static_cast<BYTE>(g_PrintSettingsData.m_nupNum);//multiple-page 2in1: 2, 4in1: 4, 6in1: 6, 9in1: 9, 16 in1: 16
				devmode.dmPrivate.bpmrdata.TypeofPB = static_cast<BYTE>(g_PrintSettingsData.m_typeofPB); //TypeofPB;//multiple-page, 1in nxn pages
				devmode.dmPrivate.poster.wPosterType = g_PrintSettingsData.m_posterType;// 0: 1 in 2x2, 1: 1 in 3x3, 2: 1 in 4x4 pages.

				devmode.dmPrivate.sfp.ISFSet = static_cast<BYTE>(g_PrintSettingsData.m_scalingType);// scale 
				devmode.dmPrivate.sfp.SRatio = g_PrintSettingsData.m_drvScalingRatio;//scale 25~400

				devmode.dmPrivate.graphics.isADJColorBalance = g_PrintSettingsData.m_ADJColorBalance;
				devmode.dmPrivate.graphics.bColorBalanceTo = static_cast<BYTE>(g_PrintSettingsData.m_colorBalanceTo);

				devmode.dmPrivate.graphics.ColorBalanceIndex[0][0] = static_cast<BYTE>(g_PrintSettingsData.m_densityValue);
				devmode.dmPrivate.graphics.ColorBalanceIndex[0][1] = static_cast<BYTE>(g_PrintSettingsData.m_densityValue);
				devmode.dmPrivate.graphics.ColorBalanceIndex[0][2] = static_cast<BYTE>(g_PrintSettingsData.m_densityValue);

				devmode.dmPublic.dmDuplex = g_PrintSettingsData.m_duplexPrint; //DUPLEX�� DMDUP_VERTICAL: ���� DMDUP_HORIZONTAL���̱�
				devmode.dmPrivate.bDocumentStyle = static_cast<BYTE>(g_PrintSettingsData.m_documentStyle);

				devmode.dmPrivate.bPaperReverseOrder = static_cast<BYTE>(g_PrintSettingsData.m_reversePrint);
				devmode.dmPrivate.graphics.TonerSaving = static_cast<BYTE>(g_PrintSettingsData.m_tonerSaving);
				devmode.dmPublic.dmCopies = g_PrintSettingsData.m_copies;

				if (2 == m_PrintType)
				{
					devmode.dmPrivate.bEnableBooklet = false;
					devmode.dmPrivate.bEnableWM = false;
					devmode.dmPrivate.headerData.bEnable = false;
					devmode.dmPrivate.sPrintTextAsBlack = 0;
					devmode.dmPrivate.sSkipBlankPage = 0;
				}
				else
				{
					devmode.dmPrivate.bEnableBooklet = static_cast<BYTE>(g_PrintSettingsData.m_booklet);
					devmode.dmPrivate.bEnableWM = static_cast<BYTE>(g_PrintSettingsData.m_watermark);
				}

				*((LPPCLDEVMODE)printer_info->pDevMode) = devmode;

				SetPrinter(phandle, 2, (LPBYTE)printer_info, 0);
				OutputDebugString(L"setprinter ok");
				isOpenDocumentProperties = false;
				Sleep(200);

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

USBAPI_API void __stdcall RecoverDevModeData()
{
	HANDLE   phandle;
	DWORD dmsize = 0;

	phandle = NULL;
	if (g_PrintSettingsData.g_szprintername != NULL)
	{
		wchar_t szprintername[MAX_PATH] = { 0 };
		wcscpy_s(szprintername, MAX_PATH, g_PrintSettingsData.g_szprintername);
		OutputDebugString(L"RecoverDevModeData");
		OutputDebugString(L"szprintername:");
		OutputDebugString(szprintername);
		if (OpenPrinter(szprintername, &phandle, NULL))
		{
			LPPRINTER_INFO_2 printer_info;

			GetPrinter(phandle, 2, (LPBYTE)NULL, 0, &dmsize);

			printer_info = (LPPRINTER_INFO_2)malloc(dmsize);

			if (printer_info != NULL)
			{
				if (GetPrinter(phandle, 2, (LPBYTE)printer_info, dmsize, &dmsize))
				{
					*((LPPCLDEVMODE)printer_info->pDevMode) = getdevmode;

					SetPrinter(phandle, 2, (LPBYTE)printer_info, 0);
					OutputDebugString(L"RecoverDevModeData ok");
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
}



USBAPI_API void __stdcall InitPrinterData(const TCHAR * strPrinterName)
{
	HANDLE   phandle;
	DWORD dmsize = 0;
	DWORD dwRet = 0;
	phandle = NULL;
	LPPCLDEVMODE lpInitData = NULL;
	LPPCLDEVMODE lpDefaultData = NULL;
	wchar_t szprintername[MAX_PATH] = { 0 };
	wcscpy_s(szprintername, MAX_PATH, strPrinterName);	
	if (IsInitPrinterName)
	{
		wcscpy_s(g_PrintSettingsData.g_szprintername, MAX_PATH, strPrinterName);
		GetPrinterDefaultInfo(strPrinterName);
		IsInitPrinterName = false;
	}
	else
	{
		if (0 != TcsNiCmp(g_PrintSettingsData.g_szprintername, szprintername))
		{
			RecoverDevModeData();
			wcscpy_s(g_PrintSettingsData.g_szprintername, MAX_PATH, strPrinterName);
			GetPrinterDefaultInfo(strPrinterName);
		}
	}
	if (OpenPrinter(szprintername, &phandle, NULL))
	{
		LPPRINTER_INFO_2 printer_info;

		GetPrinter(phandle, 2, (LPBYTE)NULL, 0, &dmsize);

		printer_info = (LPPRINTER_INFO_2)malloc(dmsize);

		if (printer_info != NULL)
		{
			if (GetPrinter(phandle, 2, (LPBYTE)printer_info, dmsize, &dmsize))
			{
				dmsize = DocumentProperties(NULL, phandle, szprintername, NULL, NULL, 0);
				TCHAR szDebug[256] = { 0 };
				wsprintf(szDebug, _T("dmsize = %d"), dmsize);
				OutputDebugString(szDebug);
				lpDefaultData = (LPPCLDEVMODE)malloc(dmsize);
				lpInitData = (LPPCLDEVMODE)malloc(dmsize);
				while (lpDefaultData == NULL && lpInitData == NULL)
				{
					dmsize = DocumentProperties(NULL, phandle, szprintername, NULL, NULL, 0);
					wsprintf(szDebug, _T("dmsize = %d"), dmsize);
					OutputDebugString(szDebug);
					lpDefaultData = (LPPCLDEVMODE)malloc(dmsize);
					lpInitData = (LPPCLDEVMODE)malloc(dmsize);
				}				
				if (lpDefaultData && lpInitData)
				{
					dwRet = DocumentProperties(NULL, phandle, szprintername,
						(LPDEVMODE)lpDefaultData, NULL, DM_OUT_BUFFER);					
					while (dwRet != IDOK)
					{
						dwRet = DocumentProperties(NULL, phandle, szprintername,
							(LPDEVMODE)lpDefaultData, NULL, DM_OUT_BUFFER);
						OutputDebugString(L"fail1");
					}

					dwRet = DocumentProperties(NULL, phandle, szprintername,
						(LPDEVMODE)lpInitData, (LPDEVMODE)lpDefaultData, DM_IN_BUFFER | DM_OUT_BUFFER);
					while (dwRet != IDOK)
					{
						dwRet = DocumentProperties(NULL, phandle, szprintername,
							(LPDEVMODE)lpInitData, (LPDEVMODE)lpDefaultData, DM_IN_BUFFER | DM_OUT_BUFFER);
						OutputDebugString(L"fail2");
					}

					PCLDEVMODE devmode;
					devmode = *(LPPCLDEVMODE)lpDefaultData;
					*((LPPCLDEVMODE)printer_info->pDevMode) = devmode;

					SetPrinter(phandle, 2, (LPBYTE)printer_info, 0);
					Sleep(200);
				}				
			}
			free(printer_info);
		}			
	}

	if (lpDefaultData)
		free(lpDefaultData);

	if (lpInitData)
		free(lpInitData);
	if (phandle != NULL)
	{
		ClosePrinter(phandle);
		phandle = NULL;
	}
}

USBAPI_API void __stdcall SetCopies(const TCHAR * strPrinterName, UINT8 Copies)
{
	HANDLE   phandle;
	DWORD dmsize = 0;

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
				
				devmode.dmPublic.dmCopies = Copies;
				g_PrintSettingsData.m_copies = Copies;

				*((LPPCLDEVMODE)printer_info->pDevMode) = devmode;

				SetPrinter(phandle, 2, (LPBYTE)printer_info, 0);
				OutputDebugString(L"SetCopies ok");
				Sleep(200);

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
USBAPI_API void __stdcall GetPrinterSettingsData(
	BYTE* ptr_PaperSize,
	BYTE* ptr_paperOrientation,
	BYTE* ptr_mediaType,
	BYTE* ptr_paperOrder,
	BYTE* ptr_printQuality,//byte
	BYTE* ptr_scalingType,//byte
	UINT16* ptr_drvScalingRatio,
	BYTE* ptr_nupNum,//byte
	BYTE* ptr_typeofPB,//byte
	BYTE* ptr_posterType,
	BYTE* ptr_ADJColorBalance,
	BYTE* ptr_colorBalanceTo,
	BYTE* ptr_density,
	BYTE* ptr_duplexPrint,
	BYTE* ptr_documentStyle,
	BYTE* ptr_reversePrint,//byte
	BYTE* ptr_tonerSaving,
	BYTE* ptr_copies,
	BYTE* ptr_booklet,
	BYTE* ptr_watermark)//byte
{
	*ptr_PaperSize = static_cast<BYTE>(g_PrintSettingsData.m_paperSize);
	*ptr_paperOrientation = static_cast<BYTE>(g_PrintSettingsData.m_paperOrientation);
	*ptr_mediaType = static_cast<BYTE>(g_PrintSettingsData.m_mediaType);
	*ptr_paperOrder = static_cast<BYTE>(g_PrintSettingsData.m_paperOrder);
	*ptr_printQuality = static_cast<BYTE>(g_PrintSettingsData.m_printQuality);
	*ptr_scalingType = static_cast<BYTE>(g_PrintSettingsData.m_scalingType);
	*ptr_drvScalingRatio = g_PrintSettingsData.m_drvScalingRatio;
	*ptr_nupNum = static_cast<BYTE>(g_PrintSettingsData.m_nupNum);
	*ptr_typeofPB = static_cast<BYTE>(g_PrintSettingsData.m_typeofPB);
	*ptr_posterType = static_cast<BYTE>(g_PrintSettingsData.m_posterType);
	*ptr_ADJColorBalance = static_cast<BYTE>(g_PrintSettingsData.m_ADJColorBalance);
	*ptr_colorBalanceTo = static_cast<BYTE>(g_PrintSettingsData.m_colorBalanceTo);
	*ptr_density = static_cast<BYTE>(g_PrintSettingsData.m_densityValue);
	*ptr_duplexPrint = static_cast<BYTE>(g_PrintSettingsData.m_duplexPrint);
	*ptr_documentStyle = static_cast<BYTE>(g_PrintSettingsData.m_documentStyle);
	*ptr_reversePrint = static_cast<BYTE>(g_PrintSettingsData.m_reversePrint);
	*ptr_tonerSaving = static_cast<BYTE>(g_PrintSettingsData.m_tonerSaving);
	*ptr_copies = static_cast<BYTE>(g_PrintSettingsData.m_copies);
	*ptr_booklet = static_cast<BYTE>(g_PrintSettingsData.m_booklet);
	*ptr_watermark = static_cast<BYTE>(g_PrintSettingsData.m_watermark);
	getDocumentPropertiesData = getOutputData;
}

USBAPI_API int __stdcall GetPrinterInfo(const TCHAR * strPrinterName,
	BYTE* ptr_paperSize,
	BYTE* ptr_paperOrientation,
	BYTE* ptr_mediaType,
	BYTE* ptr_paperOrder,
	BYTE* ptr_printQuality,//byte
	BYTE* ptr_scalingType,//byte
	UINT16* ptr_drvScalingRatio,
	BYTE* ptr_nupNum,//byte
	BYTE* ptr_typeofPB,//byte
	BYTE* ptr_posterType,
	BYTE* ptr_ADJColorBalance,
	BYTE* ptr_colorBalanceTo,
	BYTE* ptr_density,
	BYTE* ptr_duplexPrint,
	BYTE* ptr_documentStyle,
	BYTE* ptr_reversePrint,//byte
	BYTE* ptr_tonerSaving,
	BYTE* ptr_copies,
	BYTE* ptr_booklet,
	BYTE* ptr_watermark)//byte
{
	HANDLE   phandle;
	DWORD dmsize = 0;

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
				if (devmode.dmPublic.dmPaperSize > 256)
				{
					*ptr_paperSize = (devmode.dmPublic.dmPaperSize - 256) + 10;
				}
				else
				{
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
				}				

				*ptr_paperOrientation = static_cast<BYTE>(devmode.dmPublic.dmOrientation);				
				*ptr_mediaType = static_cast<BYTE>(devmode.dmPrivate.par.wMediaType);
				*ptr_paperOrder = static_cast<BYTE>(devmode.dmPublic.dmCollate);				
				*ptr_printQuality = static_cast<short>(devmode.dmPrivate.PrintQuality);
				*ptr_nupNum = devmode.dmPrivate.nup.bNupNum;
				*ptr_typeofPB = devmode.dmPrivate.bpmrdata.TypeofPB;
				*ptr_posterType = static_cast<BYTE>(devmode.dmPrivate.poster.wPosterType);
				*ptr_scalingType = devmode.dmPrivate.sfp.ISFSet;
				*ptr_drvScalingRatio = static_cast<short>(devmode.dmPrivate.sfp.SRatio);
				*ptr_ADJColorBalance = devmode.dmPrivate.graphics.isADJColorBalance;
				*ptr_colorBalanceTo = devmode.dmPrivate.graphics.bColorBalanceTo;
				*ptr_density = devmode.dmPrivate.graphics.ColorBalanceIndex[0][0];
				*ptr_duplexPrint = static_cast<BYTE>(devmode.dmPublic.dmDuplex);
				*ptr_documentStyle = devmode.dmPrivate.bDocumentStyle;
				*ptr_reversePrint = devmode.dmPrivate.bPaperReverseOrder;
				*ptr_tonerSaving = devmode.dmPrivate.graphics.TonerSaving;
				*ptr_copies = static_cast<BYTE>(devmode.dmPublic.dmCopies);
				*ptr_booklet = devmode.dmPrivate.bEnableBooklet;
				*ptr_watermark = devmode.dmPrivate.bEnableWM;

				*((LPPCLDEVMODE)printer_info->pDevMode) = devmode;

				getDocumentPropertiesData = devmode;

//				SetPrinter(phandle, 2, (LPBYTE)printer_info, 0);

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

USBAPI_API int __stdcall OpenDocumentProperties(HWND hWnd,const TCHAR * strPrinterName,
	BYTE* ptr_paperSize,
	BYTE* ptr_paperOrientation,
	BYTE* ptr_mediaType,
	BYTE* ptr_paperOrder,
	BYTE* ptr_printQuality,//byte
	BYTE* ptr_scalingType,//byte
	UINT16* ptr_drvScalingRatio,
	BYTE* ptr_nupNum,//byte
	BYTE* ptr_typeofPB,//byte
	BYTE* ptr_posterType,
	BYTE* ptr_ADJColorBalance,
	BYTE* ptr_colorBalanceTo,
	BYTE* ptr_density,
	BYTE* ptr_duplexPrint,
	BYTE* ptr_documentStyle,
	BYTE* ptr_reversePrint,//byte
	BYTE* ptr_tonerSaving,
	BYTE* ptr_copies,
	BYTE* ptr_booklet,
	BYTE* ptr_watermark)//byte
{
	HANDLE   phandle;
	LPPCLDEVMODE lpOutputData = NULL;
	LPPCLDEVMODE lpInputData = NULL;

	wchar_t szprintername[MAX_PATH] = { 0 };
	wcscpy_s(szprintername, MAX_PATH, strPrinterName);

	phandle = NULL;

	if (OpenPrinter(szprintername, &phandle, NULL))
	{
		DWORD dmsize = 0;
		int iNeeded = 0;
		LPPRINTER_INFO_2 printer_info;
		GetPrinter(phandle, 2, (LPBYTE)NULL, 0, &dmsize);

		printer_info = (LPPRINTER_INFO_2)malloc(dmsize);

		if (printer_info != NULL)
		{
			if (GetPrinter(phandle, 2, (LPBYTE)printer_info, dmsize, &dmsize))
			{
				PCLDEVMODE inputDevmode;
				PCLDEVMODE backupDevmode;
				backupDevmode = *(LPPCLDEVMODE)printer_info->pDevMode;
				if (isOpenDocumentProperties)
				{
					inputDevmode = getDocumentPropertiesData;					
				}
				else
				{
					inputDevmode = *(LPPCLDEVMODE)printer_info->pDevMode;
				}	
				if (*ptr_paperSize > 10)
				{
					inputDevmode.dmPublic.dmPaperSize = DMPAPER_USER + (*ptr_paperSize - 10);
				}
				else
				{
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
				inputDevmode.dmPrivate.sfp.SRatio = *ptr_drvScalingRatio;//scale 25~400

				inputDevmode.dmPrivate.graphics.isADJColorBalance = *ptr_ADJColorBalance;
				inputDevmode.dmPrivate.graphics.bColorBalanceTo = *ptr_colorBalanceTo;

				*ptr_density = *ptr_density - 4;
				inputDevmode.dmPrivate.graphics.ColorBalanceIndex[0][0] = *ptr_density ;
				inputDevmode.dmPrivate.graphics.ColorBalanceIndex[0][1] = *ptr_density;
				inputDevmode.dmPrivate.graphics.ColorBalanceIndex[0][2] = *ptr_density;

				inputDevmode.dmPublic.dmDuplex = *ptr_duplexPrint; //DUPLEX�� DMDUP_VERTICAL: ���� DMDUP_HORIZONTAL���̱�
				inputDevmode.dmPrivate.bDocumentStyle = *ptr_documentStyle;

				inputDevmode.dmPrivate.bPaperReverseOrder = *ptr_reversePrint;
				inputDevmode.dmPrivate.graphics.TonerSaving = *ptr_tonerSaving;		
				inputDevmode.dmPublic.dmCopies = *ptr_copies;
				inputDevmode.dmPrivate.bEnableBooklet = *ptr_booklet;
				inputDevmode.dmPrivate.bEnableWM = *ptr_watermark;

				//if (0 == *ptr_booklet)
				//{
				//	inputDevmode.dmPrivate.bEnableBooklet = false;
				//}
				
				dmsize = DocumentProperties(hWnd, phandle, szprintername, NULL, NULL, 0);
				TCHAR szDebug[256] = { 0 };
				lpOutputData = (LPPCLDEVMODE)malloc(dmsize);
				lpInputData = (LPPCLDEVMODE)malloc(dmsize);				
				wsprintf(szDebug, _T("dmsize = %d"), dmsize);
				OutputDebugString(szDebug);
				while (lpOutputData == NULL && lpInputData == NULL)
				{
					dmsize = DocumentProperties(hWnd, phandle, szprintername, NULL, NULL, 0);
					wsprintf(szDebug, _T("dmsize = %d"), dmsize);
					OutputDebugString(szDebug);
					lpOutputData = (LPPCLDEVMODE)malloc(dmsize);
					lpInputData = (LPPCLDEVMODE)malloc(dmsize);
				}
				if (lpOutputData && lpInputData)
				{
					*((LPPCLDEVMODE)printer_info->pDevMode) = inputDevmode;

					lpInputData = (LPPCLDEVMODE)printer_info->pDevMode;

					iNeeded = DocumentProperties(hWnd, phandle, szprintername,
						(LPDEVMODE)lpOutputData, (LPDEVMODE)lpInputData, DM_OUT_BUFFER | DM_IN_BUFFER | DM_IN_PROMPT);

					if (1 == iNeeded)
					{
						PCLDEVMODE devmode;
						devmode = *lpOutputData;
						if (devmode.dmPublic.dmPaperSize > 256)
						{
							*ptr_paperSize = (devmode.dmPublic.dmPaperSize - 256) + 10;
						}
						else
						{
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
						}				

						*ptr_paperOrientation = static_cast<BYTE>(devmode.dmPublic.dmOrientation);						
						*ptr_mediaType = static_cast<BYTE>(devmode.dmPrivate.par.wMediaType);
						*ptr_paperOrder = static_cast<BYTE>(devmode.dmPublic.dmCollate);						
						*ptr_printQuality = static_cast<short>(devmode.dmPrivate.PrintQuality);
						*ptr_nupNum = devmode.dmPrivate.nup.bNupNum;
						*ptr_typeofPB = devmode.dmPrivate.bpmrdata.TypeofPB;
						*ptr_posterType = static_cast<BYTE>(devmode.dmPrivate.poster.wPosterType);
						*ptr_scalingType = devmode.dmPrivate.sfp.ISFSet;
						*ptr_drvScalingRatio = static_cast<short>(devmode.dmPrivate.sfp.SRatio);
						*ptr_ADJColorBalance = devmode.dmPrivate.graphics.isADJColorBalance;
						*ptr_colorBalanceTo = devmode.dmPrivate.graphics.bColorBalanceTo;
						*ptr_density = devmode.dmPrivate.graphics.ColorBalanceIndex[0][0];
						*ptr_duplexPrint = static_cast<BYTE>(devmode.dmPublic.dmDuplex);
						*ptr_documentStyle = devmode.dmPrivate.bDocumentStyle;
						*ptr_reversePrint = devmode.dmPrivate.bPaperReverseOrder;
						*ptr_tonerSaving = devmode.dmPrivate.graphics.TonerSaving;
						*ptr_copies = static_cast<BYTE>(devmode.dmPublic.dmCopies);
						*ptr_booklet = devmode.dmPrivate.bEnableBooklet;
						*ptr_watermark = devmode.dmPrivate.bEnableWM;

						*((LPPCLDEVMODE)printer_info->pDevMode) = devmode;
						getDocumentPropertiesData = devmode;
						isOpenDocumentProperties = true;

//						SetPrinter(phandle, 2, (LPBYTE)printer_info, 0);
					}
					else
					{
						*((LPPCLDEVMODE)printer_info->pDevMode) = backupDevmode;
//						getDocumentPropertiesData = backupDevmode;
//						isOpenDocumentProperties = true;
						SetPrinter(phandle, 2, (LPBYTE)printer_info, 0);
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
USBAPI_API void __stdcall GetFixToPaperSizeData(BYTE* ptr_fixToPaperSize, UINT16* ptr_scalingRatio)
{
	*ptr_fixToPaperSize = static_cast<BYTE>(g_PrintSettingsData.m_fixToPaperSize);
	*ptr_scalingRatio = g_PrintSettingsData.m_scalingRatio;
}
USBAPI_API void __stdcall SaveFixToPaperSizeData(UINT8 FixToPaperSize, UINT16 ScalingRatio)
{
	g_PrintSettingsData.m_fixToPaperSize = FixToPaperSize;
	g_PrintSettingsData.m_scalingRatio = ScalingRatio;
}