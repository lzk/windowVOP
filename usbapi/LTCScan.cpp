#include "stdafx.h"
#include "usbapi.h"
#include <windows.h>

#include <usbprint.h>
#include <stdio.h>
#include <stdlib.h>
#include <tchar.h>
#include <StrSafe.h>
#include <Winspool.h>
#include "Global.h"
#include <Iphlpapi.h>
#include <string>
#include <algorithm>
#include <cctype>

#include <ws2tcpip.h>
#include "dns_sd.h"
#include "ImgFile\ImgFile.h"
#include <usbscan.h>
#include "Global.h"
#include <gdiplus.h>
#include <Shlwapi.h>
#include <Wininet.h>
#include <Sensapi.h>


using namespace Gdiplus;

#pragma comment(lib, "dnssd.lib")
#pragma comment(lib, "Ws2_32.lib")
#pragma comment(lib, "Iphlpapi.lib")
#pragma comment(lib, "Shlwapi.lib")

#define _SCANMODE_1BIT_BLACKWHITE 1
#define _SCANMODE_8BIT_GRAYSCALE  8
#define _SCANMODE_24BIT_COLOR     24


#define START_STAGE					0x1
#define SCANNING_STAGE				0x2
#define PUSH_TRANSFER_STAGE		0x3

#define JOB_FLB			'F'
#define JOB_ADF			'A'

enum Scan_RET
{
	RETSCAN_OK = 0,
	RETSCAN_ERRORDLL = 1,
	RETSCAN_OPENFAIL = 2,
	RETSCAN_ERRORPARAMETER = 3,
	RETSCAN_NO_ENOUGH_SPACE = 5,
	RETSCAN_ERROR_PORT = 6,
	RETSCAN_CANCEL = 7,
	RETSCAN_BUSY = 8,
	RETSCAN_ERROR = 9,
	RETSCAN_OPENFAIL_NET = 10,
	RETSCAN_PAPER_JAM = 11,
	RETSCAN_COVER_OPEN = 12,
	RETSCAN_PAPER_NOT_READY = 13,
	RETSCAN_CREATE_JOB_FAIL = 14,
	RETSCAN_ADFCOVER_NOT_READY = 15,
	RETSCAN_HOME_NOT_READY = 16,
	RETSCAN_ULTRA_SONIC = 17,
	RETSCAN_ERROR_POWER1 = 18,
	RETSCAN_ERROR_POWER2 = 19,
	RETSCAN_JOB_MISSING = 20,
	RETSCAN_JOB_GOGING = 21,
	RETSCAN_TIME_OUT = 22,
	RETSCAN_USB_TRANSFERERROR = 23,
	RETSCAN_WIFI_TRANSFERERROR = 24,
	RETSCAN_ADFPATH_NOT_READY = 25,
	RETSCAN_ADFDOC_NOT_READY = 26,
	RETSCAN_GETINFO_FAIL = 27,
};

extern UINT WM_VOPSCAN_PROGRESS;
extern UINT WM_VOPSCAN_UPLOAD;
extern UINT WM_VOPSCAN_PAGECOMPLETE;

extern CRITICAL_SECTION g_csCriticalSection_UsbTest;
extern CRITICAL_SECTION g_csCriticalSection_NetWorkTest;

USBAPI_API BOOL __stdcall TestIpConnected(wchar_t* szIP);
extern BOOL TestIpConnected1(wchar_t* szIP, Scan_RET *status);

wchar_t g_ipAddress[256] = { 0 };
wchar_t g_deviceName[256] = { 0 };
BOOL g_connectMode_usb = FALSE;
static Gdiplus::GdiplusStartupInput gdiplusStartupInput;
static ULONG_PTR gdiplusToken;

CGLNet g_GLnet;
USBAPI_API int __stdcall ADFScan(const wchar_t* sz_printer,
	const wchar_t* tempPath,
	int BitsPerPixel,
	int resolution,
	int width,
	int height,
	int contrast,
	int brightness,
	BOOL ADFMode,
	BOOL onepage,
	UINT32 uMsg,
	BOOL bColorDetect,
	BOOL bSkipBlankPage,
	double gammaValue,
	SAFEARRAY** fileNames);

USBAPI_API BOOL __stdcall CheckConnection();

static int GetByteNumPerLineWidthPad(int scanMode, int nPixels)
{
	int cbPerLine = 0;

	switch (scanMode)
	{
	case _SCANMODE_1BIT_BLACKWHITE:
		cbPerLine = (nPixels + 7) / 8;
		break;
	case _SCANMODE_8BIT_GRAYSCALE:
		cbPerLine = nPixels;
		break;
	case _SCANMODE_24BIT_COLOR:
		cbPerLine = 3 * nPixels;
		break;
	default:
		cbPerLine = 0;
	}

	return cbPerLine;
}


CGLDrv *g_pointer_lDrv = NULL;
int start_cancel = 0;
USBAPI_API int __stdcall ADFCancel()
{
	start_cancel = 1;

	//if (g_pointer_lDrv != NULL)
	//{
	//	return g_pointer_lDrv->_cancel();
	//}

	return 1;
}

int GetEncoderClsid(const WCHAR* format, CLSID* pClsid)
{
	UINT  num = 0;          // number of image encoders
	UINT  size = 0;         // size of the image encoder array in bytes

	ImageCodecInfo* pImageCodecInfo = NULL;

	GetImageEncodersSize(&num, &size);
	if (size == 0)
		return -1;  // Failure

	pImageCodecInfo = (ImageCodecInfo*)(malloc(size));
	if (pImageCodecInfo == NULL)
		return -1;  // Failure

	GetImageEncoders(num, size, pImageCodecInfo);

	for (UINT j = 0; j < num; ++j)
	{
		if (wcscmp(pImageCodecInfo[j].MimeType, format) == 0)
		{
			*pClsid = pImageCodecInfo[j].Clsid;
			free(pImageCodecInfo);
			return j;  // Success
		}
	}

	free(pImageCodecInfo);
	return -1;  // Failure
}

void BrightnessAndContrast(const wchar_t *filename, int Brightness, int Contrast)
{
	HRESULT hr;
	Gdiplus::Image *pImg = NULL;

	pImg = Gdiplus::Image::FromFile(filename);


	float brightness = (Brightness - 50.f) /100.0f; // no change in brightness
	float contrast = Contrast / 50.0f; // twice the contrast
	float gamma = 1.0f; // no change in gamma

	float adjustedBrightness = brightness < -0.3 ? -0.3 : brightness;
	adjustedBrightness = brightness > 0.3 ? 0.3 : brightness;

	float adjustedcontrast = contrast < 0.5 ? 0.5 : contrast;
	float T = 0.5f * (1.0f - adjustedcontrast);

	// create matrix that will brighten and contrast the image
	Gdiplus::ColorMatrix ptsArray = {
		adjustedcontrast, 0, 0, 0, 0,	// scale red
		0, adjustedcontrast, 0, 0, 0,	// scale green
		0, 0, adjustedcontrast, 0, 0,	// scale blue
		0, 0, 0, 1.0f, 0,		// don't scale alpha
		adjustedBrightness + T, adjustedBrightness + T, adjustedBrightness + T, 0, 1 };

	Gdiplus::ImageAttributes imageAttributes;
	imageAttributes.ClearColorMatrix();
	imageAttributes.SetColorMatrix(&ptsArray, Gdiplus::ColorMatrixFlagsDefault, Gdiplus::ColorAdjustTypeBitmap);

	RectF r(0, 0, pImg->GetWidth(), pImg->GetHeight());
	Graphics *g = Graphics::FromImage(pImg);
	g->DrawImage(pImg, r, 0, 0, pImg->GetWidth(), pImg->GetHeight(), Gdiplus::UnitPixel, &imageAttributes);

	CLSID pngClsid;
	GetEncoderClsid(L"image/jpeg", &pngClsid);

	std::wstring str(filename);
	size_t found = str.find_last_of(L"\\");
	std::wstring file_path = str.substr(0, found);

	found = str.find_last_of('.');
	std::wstring file_without_extension = str.substr(0, found);
	std::wstring file_extension = str.substr(found, str.length());

	TCHAR new_name[4096] = { 0 };
	wsprintf(new_name, _T("%s%s%s"), file_without_extension.c_str(), L"_bc", file_extension.c_str());

	pImg->Save(new_name, &pngClsid);

	if (pImg)
		delete pImg;
	if (g)
		delete g;

	ReplaceFile(filename, new_name, NULL, REPLACEFILE_IGNORE_MERGE_ERRORS, NULL, NULL);
}

int GammaTransLTCtoGL(unsigned int *pbyRed, unsigned int *pbyGreen, unsigned int *pbyBlue, unsigned int *GLGamma)
{
	int i;
	for (i = 0; i<256; i++)
	{
		if (i<255) {
			GLGamma[i] = ((unsigned int)(*(pbyRed + i * 256)) & 0xffff) + (unsigned int)(((*(pbyRed + ((i + 1) * 256))) & 0xffff) << 16);
			GLGamma[i + 256] = ((unsigned int)(*(pbyGreen + i * 256)) & 0xffff) + (unsigned int)(((*(pbyGreen + ((i + 1) * 256))) & 0xffff) << 16);
			GLGamma[i + 256 * 2] = ((unsigned int)(*(pbyBlue + i * 256)) & 0xffff) + (unsigned int)(((*(pbyBlue + ((i + 1) * 256))) & 0xffff) << 16);
		}
		else {
			GLGamma[i] = (unsigned int)(*(pbyRed + i * 256)) & 0xffff | ((unsigned int)(0xffff) << 16);
			GLGamma[i + 256] = (unsigned int)(*(pbyGreen + i * 256)) & 0xffff | ((unsigned int)(0xffff) << 16);
			GLGamma[i + 256 * 2] = (unsigned int)(*(pbyBlue + i * 256)) & 0xffff | ((unsigned int)(0xffff) << 16);
		}

	}
	return 1;
}

Scan_RET ScannerStatusCheck(CGLDrv glDrv, char stage)
{
	Scan_RET result = RETSCAN_OK;

	memset(&glDrv.sc_infodata, 0, sizeof(SC_INFO_T));
	if (!glDrv._info())
	{
		//printf("INFO command error!!");
		result = RETSCAN_GETINFO_FAIL;
	}
	else
	{
		if (glDrv.sc_infodata.JobID)
		{
			if (stage == START_STAGE)
			{
				//printf("Last job not finish!!\n");
				result = RETSCAN_JOB_GOGING;
			}
		}
		else 
		{
			if (stage == SCANNING_STAGE)
			{
				//printf("Scan job missing!!\n");
				result = RETSCAN_JOB_MISSING;
			}
		}


		if (glDrv.sc_infodata.ErrorStatus.cover_open_err) 
		{
			if (stage != PUSH_TRANSFER_STAGE) 
			{
				//printf("COVER_OPEN_ERR\n");
				result = RETSCAN_COVER_OPEN;
			}
		}
		if (glDrv.sc_infodata.ErrorStatus.scan_jam_err)
		{
			//printf("SCAN_JAM_ERR\n");
			result = RETSCAN_PAPER_JAM;
		}
		if (glDrv.sc_infodata.ErrorStatus.scan_canceled_err)
		{
			//printf("SCAN_CANCELED_ERR\n");
			result = RETSCAN_CANCEL;
		}
		if (glDrv.sc_infodata.ErrorStatus.scan_timeout_err) 
		{
			//printf("SCAN_TIMEOUT_ERR\n");
			result = RETSCAN_TIME_OUT;
		}
		if (glDrv.sc_infodata.ErrorStatus.multi_feed_err) 
		{
			//printf("MULTI_FEED_ERR\n");
			result = RETSCAN_ULTRA_SONIC;
		}
		if (glDrv.sc_infodata.ErrorStatus.usb_transfer_err)
		{
			//printf("USB_TRANSFER_ERR\n");
			result = RETSCAN_USB_TRANSFERERROR;
		}
		if (glDrv.sc_infodata.ErrorStatus.wifi_transfer_err) 
		{
			//printf("WiFi_TRANSFER_ERR\n");
			result = RETSCAN_WIFI_TRANSFERERROR;
		}
		//if (glDrv.sc_infodata.ErrorStatus.usb_disk_transfer_err)
		//{
		//	printf("USBDISK_TRANSFER_ERR\n");
		//	result = RETSCAN_DISK_TRANSFERERROR;
		//}
		//if (glDrv.sc_infodata.ErrorStatus.ftp_transfer_err) 
		//{
		//	//printf("FTP_TRANSFER_ERR\n");
		//	result = FALSE;
		//}
		//if (glDrv.sc_infodata.ErrorStatus.smb_transfer_err)
		//{
		//	printf("SMB_TRANSFER_ERR\n");
		//	result = FALSE;
		//}

		if (stage == START_STAGE) 
		{
			//ADD BY YUNYING SHANG 2018-01-23 for BMS 
			if (glDrv.sc_infodata.SystemStatus.scanning)
			{
				result = RETSCAN_BUSY;
			}

			if (glDrv.sc_infodata.SensorStatus.adf_document_sensor)
			{
				//printf("ADF document not ready.\n");
				result = RETSCAN_ADFDOC_NOT_READY;
			}

			if (glDrv.sc_infodata.SensorStatus.adf_paper_sensor)
			{
				//printf("ADF path not ready.\n");
				result = RETSCAN_ADFPATH_NOT_READY;
			}

			if (glDrv.sc_infodata.SensorStatus.cover_sensor)
			{
				//printf("ADF cover not ready.\n");
				result = RETSCAN_ADFCOVER_NOT_READY;
			}
		}
	}

	return result;
}

USBAPI_API int __stdcall ADFScan(const wchar_t* sz_printer,
	const wchar_t* tempPath,
	int BitsPerPixel,
	int resolution,
	int width,
	int height,
	int contrast,
	int brightness,
	BOOL ADFMode,
	BOOL MultiFeed,
	BOOL AutoCrop,
	BOOL onepage,
	UINT32 uMsg,
	BOOL bColorDetect,
	BOOL bSkipBlankPage,
	double gammaValue,
	int type,
	SAFEARRAY** fileNames)
{
	
	BSTR bstrArray[500] = { 0 };
	BYTE emptyPages[500] = { 0 };
	BOOL bEmptyPage = false;
	CGLDrv glDrv;
	g_pointer_lDrv = &glDrv;

	int lineNumber = 50;
	int nColPixelNumOrig = 0;   
	int nLinePixelNumOrig = 0;  
	int imgBufferSize = 0;
  
	BYTE* imgBuffer = NULL; 

	nLinePixelNumOrig = width*resolution / 1000;
	nLinePixelNumOrig = nLinePixelNumOrig - nLinePixelNumOrig % 8;

	nColPixelNumOrig = height*resolution / 1000;

	if (g_connectMode_usb == TRUE)
	{
		lineNumber = 1500;
	}
	else
	{
		//imgBufferSize = 4096;
		lineNumber = 1500;
	}

	imgBufferSize = GetByteNumPerLineWidthPad(BitsPerPixel, nLinePixelNumOrig) * lineNumber;

	imgBuffer = new BYTE[imgBufferSize];


	IMG_FILE_T ImgFile[2];
	IMAGE_T ImgTemp[2];

	//modified by yunying shang 2017-10-25 for BMS 1234
	ImgFile[0].img.width = ImgFile[1].img.width = nLinePixelNumOrig;
	ImgFile[0].img.height = ImgFile[1].img.height = nColPixelNumOrig;

	//float ADF_SideEdge = (8.5 - 8.4528) / 2;
	float ADF_SideEdge = (8.5*resolution - ImgFile[0].img.width ) / 2;
	float ADF_HighEdge = (14*resolution - ImgFile[0].img.height) / 2;

	//ImgFile[0].img.org.x = 0;//ADF_SideEdge * resolution;]
	ImgFile[0].img.org.x = ADF_SideEdge;
	ImgFile[0].img.org.y = 0;
	//<<===================1234


	ImgFile[0].img.format = ImgFile[1].img.format = I3('JPG');
	ImgFile[0].img.bit = ImgFile[1].img.bit = BitsPerPixel;
	ImgFile[0].img.dpi.x = ImgFile[1].img.dpi.x = resolution;
	ImgFile[0].img.dpi.y = ImgFile[1].img.dpi.y = resolution;


	glDrv.sc_pardata.acquire = ((MultiFeed ? 1 : 0) * ACQ_ULTRA_SONIC) | ((AutoCrop ? 1 : 0) * ACQ_CROP_DESKEW)
		|  1 * ACQ_NO_GAMMA|((bColorDetect?1:0)*ACQ_DETECT_COLOR)|((bSkipBlankPage?1:0)*ACQ_SKIP_BLANKPAGE);

	//glDrv.sc_job_create.mode = I1('D');
	glDrv.sc_job_create.mode = 0;

	glDrv.sc_pardata.source = I3('ADF');
	glDrv.sc_pardata.duplex = ADFMode ? SCAN_AB_SIDE : SCAN_A_SIDE;
	glDrv.sc_pardata.page = onepage ? 1 : 0;
	glDrv.sc_pardata.img.format = I3('JPG');
	glDrv.sc_pardata.img.bit = BitsPerPixel;
	glDrv.sc_pardata.img.dpi.x = resolution;
	glDrv.sc_pardata.img.dpi.y = resolution;
	glDrv.sc_pardata.img.org.x = ImgFile[0].img.org.x;
	glDrv.sc_pardata.img.org.y = ImgFile[0].img.org.y;
	glDrv.sc_pardata.img.width = ImgFile[0].img.width;
	glDrv.sc_pardata.img.height = ImgFile[0].img.height;
	glDrv.sc_pardata.img.mono = BitsPerPixel == IMG_24_BIT ? IMG_COLOR : IMG_3CH_TRUE_MONO;

	if (glDrv.sc_pardata.img.format == I3('JPG')) {
		glDrv.sc_pardata.img.option = IMG_OPT_JPG_FMT444;
	}

	//Advanced
	if (glDrv.sc_pardata.acquire & ACQ_SET_MTR) {

			/*ADF scan*/
		glDrv.sc_pardata.mtr[1].pick_ss_step = 600;//GetPrivateProfileIntA("PICK_SS_STEP", "STEP", 0, IniFile);

			//par->mtr[0].drive_target = CMT_PH;
			//par->mtr[0].state_mechine = SCAN_STATE_MECHINE;
			//par->mtr[1].drive_target = BMT_PH;
			//par->mtr[1].state_mechine = STATE_MECHINE_1;
		if (glDrv.sc_pardata.img.bit < 24) {
			/*Mono scan*/
			glDrv.sc_pardata.mtr[1].speed_pps = 3456;// GetPrivateProfileIntA("ADFgray", "PICK_PPS", 0, IniFile);
			glDrv.sc_pardata.mtr[1].direction = 1;// GetPrivateProfileIntA("ADFgray", "PICK_DIR", 0, IniFile);
			glDrv.sc_pardata.mtr[1].micro_step = 2;// GetPrivateProfileIntA("ADFgray", "PICK_MS", 0, IniFile);
			glDrv.sc_pardata.mtr[1].currentLV = 4;// GetPrivateProfileIntA("ADFgray", "PICK_CLV", 0, IniFile);

			glDrv.sc_pardata.mtr[0].speed_pps = 4577;// GetPrivateProfileIntA("ADFgray", "FEED_PPS", 0, IniFile);
			glDrv.sc_pardata.mtr[0].direction = 0;// GetPrivateProfileIntA("ADFgray", "FEED_DIR", 0, IniFile);
			glDrv.sc_pardata.mtr[0].micro_step = 4;// GetPrivateProfileIntA("ADFgray", "FEED_MS", 0, IniFile);
			glDrv.sc_pardata.mtr[0].currentLV = 4;// GetPrivateProfileIntA("ADFgray", "FEED_CLV", 0, IniFile);
		}
		else {
			/*Color scan*/
			if ((glDrv.sc_pardata.img.dpi.x == 300) && (glDrv.sc_pardata.img.dpi.y == 300)) {
				/*300x300DPI scan*/
				glDrv.sc_pardata.mtr[1].speed_pps = 2602;// GetPrivateProfileIntA("ADF300x300color", "PICK_PPS", 0, IniFile);
				glDrv.sc_pardata.mtr[1].direction = 1;// GetPrivateProfileIntA("ADF300x300color", "PICK_DIR", 0, IniFile);
				glDrv.sc_pardata.mtr[1].micro_step = 2;// GetPrivateProfileIntA("ADF300x300color", "PICK_MS", 0, IniFile);
				glDrv.sc_pardata.mtr[1].currentLV = 4;// GetPrivateProfileIntA("ADF300x300color", "PICK_CLV", 0, IniFile);

				glDrv.sc_pardata.mtr[0].speed_pps = 350;// GetPrivateProfileIntA("ADF300x300color", "FEED_PPS", 0, IniFile);
				glDrv.sc_pardata.mtr[0].direction = 0;// GetPrivateProfileIntA("ADF300x300color", "FEED_DIR", 0, IniFile);
				glDrv.sc_pardata.mtr[0].micro_step = 4;// GetPrivateProfileIntA("ADF300x300color", "FEED_MS", 0, IniFile);
				glDrv.sc_pardata.mtr[0].currentLV = 4;// GetPrivateProfileIntA("ADF300x300color", "FEED_CLV", 0, IniFile);
			}
			else if ((glDrv.sc_pardata.img.dpi.x == 300) && (glDrv.sc_pardata.img.dpi.y == 600)) {
				/*300x600DPI scan*/
				glDrv.sc_pardata.mtr[1].speed_pps = 2317;// GetPrivateProfileIntA("ADF300x600color", "PICK_PPS", 0, IniFile);
				glDrv.sc_pardata.mtr[1].direction = 1;// GetPrivateProfileIntA("ADF300x600color", "PICK_DIR", 0, IniFile);
				glDrv.sc_pardata.mtr[1].micro_step = 4;// GetPrivateProfileIntA("ADF300x600color", "PICK_MS", 0, IniFile);
				glDrv.sc_pardata.mtr[1].currentLV = 4;// GetPrivateProfileIntA("ADF300x600color", "PICK_CLV", 0, IniFile);

				glDrv.sc_pardata.mtr[0].speed_pps = 2612;// GetPrivateProfileIntA("ADF300x600color", "FEED_PPS", 0, IniFile);
				glDrv.sc_pardata.mtr[0].direction = 0;// GetPrivateProfileIntA("ADF300x600color", "FEED_DIR", 0, IniFile);
				glDrv.sc_pardata.mtr[0].micro_step = 4;// GetPrivateProfileIntA("ADF300x600color", "FEED_MS", 0, IniFile);
				glDrv.sc_pardata.mtr[0].currentLV = 4;// GetPrivateProfileIntA("ADF300x600color", "FEED_CLV", 0, IniFile);
			}
			else {
				/*600DPI scan*/
				glDrv.sc_pardata.mtr[1].speed_pps = 1293;// GetPrivateProfileIntA("ADF600x600color", "PICK_PPS", 0, IniFile);
				glDrv.sc_pardata.mtr[1].direction = 1;// GetPrivateProfileIntA("ADF600x600color", "PICK_DIR", 0, IniFile);
				glDrv.sc_pardata.mtr[1].micro_step = 4;// GetPrivateProfileIntA("ADF600x600color", "PICK_MS", 0, IniFile);
				glDrv.sc_pardata.mtr[1].currentLV = 4;// GetPrivateProfileIntA("ADF600x600color", "PICK_CLV", 0, IniFile);

				glDrv.sc_pardata.mtr[0].speed_pps = 1557;// GetPrivateProfileIntA("ADF600x600color", "FEED_PPS", 0, IniFile);
				glDrv.sc_pardata.mtr[0].direction = 0;// GetPrivateProfileIntA("ADF600x600color", "FEED_DIR", 0, IniFile);
				glDrv.sc_pardata.mtr[0].micro_step = 4;// GetPrivateProfileIntA("ADF600x600color", "FEED_MS", 0, IniFile);
				glDrv.sc_pardata.mtr[0].currentLV = 4;// GetPrivateProfileIntA("ADF600x600color", "FEED_CLV", 0, IniFile);
			}

		}
	}

	int JobID = 0;
	int result = 0;
	char ImgFileName[64];
	int bFiling[2] = { 0, 0 };
	int length, cancel, lineSize;

	int end_page[2] = { 0 };
	int end_doc = 0;
	int duplex = 3, dup = 0, time;
	int page_line[2] = { 0 };
	int ImgSize = 0, currentImgSize = 0, LastImgSize = 0, TotalImgSize = 0;
	int RunInCounter = 0;
	U8  CancelKey[64];
	U8 open[2] = { 0 };
	U8 stop_start_t = 0;
	int page[2] = { 0 };
	int fileCount = 0;
	
	Scan_RET re_status = RETSCAN_OK;
	if (g_connectMode_usb != TRUE)
	{
		if (TestIpConnected1(g_ipAddress, &re_status) == TRUE)
		{
			if (re_status == RETSCAN_BUSY)
			{
				if (imgBuffer)
					delete imgBuffer;
				return RETSCAN_BUSY;
			}
		}
	}
	
	MyOutputString(L"ADF Enter");
	BYTE openRet = FALSE;
	if ((openRet = glDrv._OpenDevice()) == TRUE)
	{
		Scan_RET scanRet = RETSCAN_OK;

		if (!glDrv.NetScanReady())
		{
			if (imgBuffer)
				delete imgBuffer;
			glDrv._CloseDevice();

			return RETSCAN_BUSY;
		}
		MyOutputString(L"Check Net Busy");
		
		scanRet = ScannerStatusCheck(glDrv, START_STAGE);

		if (scanRet != RETSCAN_OK)
		{
			if (imgBuffer)
				delete imgBuffer;

			glDrv._CloseDevice();
			return scanRet;
		}
		MyOutputString(L"Check Device Status");
	/*	result = glDrv.paperReady();
		if (!result) {

			if (imgBuffer)
				delete imgBuffer;
			glDrv._CloseDevice();
			return RETSCAN_PAPER_NOT_READY;
		}
		MyOutputString(L"paperReady");*/
		
		BYTE power_mode = glDrv._GetPowerSupply();
		if (1 < power_mode )
		{
			MyOutputString(L"Power Mode", power_mode);
			MyOutputString(L"ADFMode", ADFMode);
			MyOutputString(L"AutoCrop", AutoCrop);
			MyOutputString(L"MultiFeed", MultiFeed);
			MyOutputString(L"height", height);
			MyOutputString(L"type", type);
			//modified by yunying shang 2018-01-09 for BMS 2021
			if (2 == power_mode && (  AutoCrop  || height > 14000 || type > 0)) //|| MultiFeed || ADFMode ||
			{
				if (imgBuffer)
					delete imgBuffer;

				glDrv._CloseDevice();
				return RETSCAN_ERROR_POWER1;
			}
			//modified by yunying shang 2017-01-03 for BMS 1924
			if(power_mode == 3 && (ADFMode || AutoCrop || MultiFeed || height > 14000 || type > 0 || g_connectMode_usb == false))
			{
				if (imgBuffer)
					delete imgBuffer;

				glDrv._CloseDevice();
				return RETSCAN_ERROR_POWER2;
			}
		}
		MyOutputString(L"Check Power Mode");

		if (!glDrv._JobCreate(JOB_ADF, g_connectMode_usb))
		{
			if (imgBuffer)
				delete imgBuffer;

			glDrv._CloseDevice();
			return RETSCAN_CREATE_JOB_FAIL;
		}


		//result = glDrv._JobCreate();
		//if (result != 0)
		//{
		//	int errorcode = RETSCAN_CREATE_JOB_FAIL;

		//	if (imgBuffer)
		//		delete imgBuffer;
		//	glDrv._CloseDevice();

		//	switch (result) {
		//	case ADF_NOT_READY_ERR:
		//		errorcode = RETSCAN_ADF_NOT_READY;
		//		break;
		//	case DOC_NOT_READY_ERR:
		//		errorcode = RETSCAN_PAPER_NOT_READY;
		//		break;
		//	case HOME_NOT_READY_ERR:
		//		errorcode = RETSCAN_HOME_NOT_READY;
		//		break;
		//	case SCAN_JAM_ERR:
		//		errorcode = RETSCAN_PAPER_JAM;
		//		break;
		//	case COVER_OPEN_ERR:
		//		errorcode = RETSCAN_COVER_OPEN;
		//		break;
		//	}

		//	return errorcode;
		//}
		

		MyOutputString(L"_JobCreate");


		//gamma
		int i, numread;
		unsigned int gGammaData[768];
		U32 up, down;
		double gamma = gammaValue;//1.8;
		unsigned int Red[65536];
		unsigned int Green[65536];
		unsigned int Blue[65536];
		unsigned int *pbyRed = Red;
		unsigned int *pbyGreen = Green;
		unsigned int *pbyBlue = Blue;


		//unsigned int *gGammaData;	
		for (i = 0; i<65536; i++)
		{
			/*Red[i] = (unsigned int)(65536 - i);
			Green[i] = (unsigned int)(65536 - i);
			Blue[i] = (unsigned int)(65536 - i);*/

			unsigned int Temp = (long)ceil(65535 * pow((double)(i) / 65535, 1.0 / gamma));

			if (Temp>65535)
			{
				Temp = 65535;
			}
			else
			{
				if (Temp<0)
				{
					Temp = 0;
				}
			}

			//Red[65535 - i] = Temp;
			//Green[65535 - i] = Temp;
			//Blue[65535 - i] = Temp;
			Red[i] = Temp;
			Green[i] = Temp;
			Blue[i] = Temp;
		}

		GammaTransLTCtoGL(pbyRed, pbyGreen, pbyBlue, gGammaData);

		result = glDrv._parameters();
		MyOutputString(L"_parameters");
		if (!result)
		{
			if (imgBuffer)
				delete imgBuffer;
			glDrv._CloseDevice();
			return RETSCAN_ERRORPARAMETER;
		}

		result = glDrv._gamma(gGammaData);
		if (!result)
		{
			if (imgBuffer)
				delete imgBuffer;
			glDrv._CloseDevice();
			return RETSCAN_ERRORPARAMETER;
		}	
		MyOutputString(L"Set Gamma");
	/*	unsigned int gGammaData[768];

		for (int i = 0; i < 256; i++) {
			if (i <= 254) {
				gGammaData[i] = (unsigned int)0x0100 * i + ((unsigned int)0x0100 * (i + 1) << 16);
				gGammaData[i + 256] = (unsigned int)0x0100 * i + ((unsigned int)0x0100 * (i + 1) << 16);
				gGammaData[i + 256 * 2] = (unsigned int)0x0100 * i + ((unsigned int)0x0100 * (i + 1) << 16);
			}
			else {
				gGammaData[i] = (unsigned int)0x0100 * i + ((unsigned int)(0xffff) << 16);
				gGammaData[i + 256] = (unsigned int)0x0100 * i + ((unsigned int)(0xffff) << 16);
				gGammaData[i + 256 * 2] = (unsigned int)0x0100 * i + ((unsigned int)(0xffff) << 16);
			}
		}

		if (glDrv.sc_pardata.acquire & ACQ_GAMMA) {
			result = glDrv._gamma(gGammaData);
			if (!result)
			{
				glDrv._JobEnd();
				return RETSCAN_ERROR;
			}
		}

		float Matrix[9] = { 1, 0, 0,
			0, 1, 0,
			0, 0, 1 };

		if (glDrv.sc_pardata.bit == 24) {
			result = glDrv._matrix(Matrix);
			if (!result)
			{
				glDrv._JobEnd();
				return RETSCAN_ERROR;
			}
		}*/
		::SendNotifyMessage(HWND_BROADCAST, WM_VOPSCAN_PROGRESS/*uMsg*/, 0, 0);
		result = glDrv._StartScan();
		MyOutputString(L"_StartScan");
		if (!result)
		{
			if (imgBuffer)
				delete imgBuffer;
			glDrv._JobEnd();
			glDrv._CloseDevice();
			return RETSCAN_ERROR;
		}

#pragma region MyRegion
		//while (!start_cancel) {

//	int lineCount = 0;
//	end_page[0] = end_page[1] = end_doc = 0;
//	page_line[0] = page_line[1] = 0;

//	Sleep(200);
//	result = glDrv._info();
//	MyOutputString(L"_info");
//	if (!result) {
//		break;
//	}

//	if ((!(duplex & 1) || glDrv.sc_infodata.EndScan[0]) && (!(duplex & 2) || glDrv.sc_infodata.EndScan[1]))
//		break;

//
//	if ((glDrv.sc_infodata.ValidPageSize[0] > 0) || (glDrv.sc_infodata.ValidPageSize[1] > 0)) {

//		char fileName[256] = { 0 };
//		char filePath[256] = { 0 };
//		TCHAR fileNameOut[256] = { 0 };

//		::WideCharToMultiByte(CP_ACP, 0, tempPath, -1, filePath, 256, NULL, NULL);

//		if (duplex & 1) {		
//			sprintf(fileName, "%s_%c%d_A%02d.%s", filePath, (ImgFile[0].img.bit > 16) ? 'C' : 'G', ImgFile[0].img.dpi.x, page[0], &ImgFile[0].img.format);
//			ImgFile_Open(&ImgFile[0], fileName);
//			MyOutputString(L"ImgFile_Open 0");

//			open[0] = 1;

//			::MultiByteToWideChar(CP_ACP, 0, fileName, strlen(fileName), fileNameOut, 256);
//			bstrArray[fileCount] = ::SysAllocString(fileNameOut);
//			MyOutputString(fileNameOut);
//			fileCount++;
//		}
//		if (duplex & 2) {
//			sprintf(fileName, "%s_%c%d_B%02d.%s", filePath, (ImgFile[1].img.bit > 16) ? 'C' : 'G', ImgFile[1].img.dpi.x, page[1], &ImgFile[1].img.format);
//			ImgFile_Open(&ImgFile[1], fileName);
//			MyOutputString(L"ImgFile_Open 1");

//			open[1] = 1;

//			::MultiByteToWideChar(CP_ACP, 0, fileName, strlen(fileName), fileNameOut, 256);
//			bstrArray[fileCount] = ::SysAllocString(fileNameOut);
//			MyOutputString(fileNameOut);
//			fileCount++;
//		}
//		
//		while (result && (((duplex & 1) && (end_page[0] == 0)) || ((duplex & 2) && (end_page[1] == 0))))
//		{
//			result = glDrv._info();
//			if (!result) {

//				glDrv._StatusGet();

//				glDrv._StatusCheck_Scanning();

//				if (start_cancel) {
//					start_cancel = 0;
//					glDrv._JobEnd();
//					if ((duplex & 1) && open[0]) {
//						ImgFile_Close(&ImgFile[0], page_line[0]);
//						open[0] = 0;
//					}
//					if ((duplex & 2) && open[1]) {
//						ImgFile_Close(&ImgFile[1], page_line[1]);
//						open[1] = 0;
//					}

//					return RETSCAN_OK;
//				}
//				else
//				{
//					if ((duplex & 1) && open[0]) {
//						ImgFile_Close(&ImgFile[0], page_line[0]);
//						open[0] = 0;
//					}
//					if ((duplex & 2) && open[1]) {
//						ImgFile_Close(&ImgFile[1], page_line[1]);
//						open[1] = 0;
//					}

//					return RETSCAN_ERROR;
//				}
//					
//			}
//			end_doc = glDrv.sc_infodata.EndDocument;

//			if ((duplex & 1) && (end_page[0] == 0)) {
//				ImgSize = 0;
//				if (glDrv.sc_infodata.ValidPageSize[0] > 0) {
//					result = glDrv._ReadImageEX(0, &ImgSize, imgBuffer, imgBufferSize) &&
//						ImgFile_Write(&ImgFile[0], imgBuffer, ImgSize);

//					MyOutputString(L"_ReadImageEX ImgFile_Write 0");
//					if (!result)
//					{
//						MyOutputString(L"_ReadImageEX Fail 0");
//						if ((duplex & 1) && open[0]) {
//							ImgFile_Close(&ImgFile[0], page_line[0]);
//							open[0] = 0;
//						}
//					
//					}
//						
//				}
//				if (ImgSize >= glDrv.sc_infodata.ValidPageSize[0]) {
//					end_page[0] = glDrv.sc_infodata.EndPage[0];
//					if ((page_line[0] == 0) && end_page[0])
//						page_line[0] = glDrv.sc_infodata.ImageLength[0];
//				}
//			}
//			if ((duplex & 2) && (end_page[1] == 0)) {
//				ImgSize = 0;
//				if (glDrv.sc_infodata.ValidPageSize[1] > 0) {
//					result = glDrv._ReadImageEX(1, &ImgSize, imgBuffer, imgBufferSize) &&
//						ImgFile_Write(&ImgFile[1], imgBuffer, ImgSize);

//					MyOutputString(L"_ReadImageEX ImgFile_Write 1");

//					if (!result)
//					{
//						MyOutputString(L"_ReadImageEX Fail 1");
//						if ((duplex & 2) && open[1]) {
//							ImgFile_Close(&ImgFile[1], page_line[1]);
//							open[1] = 0;
//						}
//					}

//				}
//				if (ImgSize >= glDrv.sc_infodata.ValidPageSize[1]) {
//					end_page[1] = glDrv.sc_infodata.EndPage[1];
//					if ((page_line[1] == 0) && end_page[1])
//						page_line[1] = glDrv.sc_infodata.ImageLength[1];
//				}
//			}

//			int percent = 0;
//			lineCount += lineNumber;
//			percent = lineCount * 100 / nColPixelNumOrig;

//			if (percent > 100)
//				percent = 100;

//			::SendNotifyMessage(HWND_BROADCAST, uMsg, percent, 0);
//			Sleep(100);

//		}



//		if ((duplex & 1) && open[0]) {
//			ImgFile_Close(&ImgFile[0], page_line[0]);
//			open[0] = 0;
//		}
//		if ((duplex & 2) && open[1]) {
//			ImgFile_Close(&ImgFile[1], page_line[1]);
//			open[1] = 0;
//		}

//		page[0]++;
//		page[1]++;



//	}

//}


//if (glDrv.sc_infodata.Cancel == 0)
//{
//	MyOutputString(L"_stop");
//	glDrv._stop();
//}
//else
//{
//	MyOutputString(L"_cancel");
//	glDrv._cancel();
//}
//	

#pragma endregion

		duplex = glDrv.sc_pardata.duplex;
		start_cancel = FALSE;
		int lineCount = 0;
		//BOOL isCoverOpen = FALSE;
		//BOOL isPaperJam = FALSE;
		//BOOL isUltraSonic = FALSE;
		BOOL isScanError = FALSE;
		bool bFinished = false;
		TCHAR debugBuf[256];

		while (!start_cancel)
		{
			if (!glDrv._info())
			{
				scanRet = RETSCAN_GETINFO_FAIL;
			}
			else
			{
				if (glDrv.sc_infodata.JobID==0)
				{				
					scanRet = RETSCAN_JOB_MISSING;					
				}

				if (glDrv.sc_infodata.ErrorStatus.cover_open_err)
				{							
					scanRet = RETSCAN_COVER_OPEN;					
				}

				if (glDrv.sc_infodata.ErrorStatus.scan_jam_err)
				{
					scanRet = RETSCAN_PAPER_JAM;
				}

				if (glDrv.sc_infodata.ErrorStatus.scan_canceled_err)
				{
					scanRet = RETSCAN_CANCEL;
					//add by yunying shang 2018-01-25 for BMS 2117
					start_cancel = true;
					break;
					//<<=============2117
				}

				if (glDrv.sc_infodata.ErrorStatus.scan_timeout_err)
				{
					scanRet = RETSCAN_TIME_OUT;
				}

				if (glDrv.sc_infodata.ErrorStatus.multi_feed_err)
				{
					scanRet = RETSCAN_ULTRA_SONIC;
				}

				if (glDrv.sc_infodata.ErrorStatus.usb_transfer_err)
				{
					scanRet = RETSCAN_USB_TRANSFERERROR;
				}

				if (glDrv.sc_infodata.ErrorStatus.wifi_transfer_err)
				{
					scanRet = RETSCAN_WIFI_TRANSFERERROR;
				}

			}

			if (scanRet == RETSCAN_GETINFO_FAIL)
			{
				continue;
			}

			if (scanRet != RETSCAN_OK)
			{
				isScanError = TRUE;
				break;
			}
			//if (!glDrv._info())
			//{
				/*Sleep(100);
				continue;*/
			//}
				
			//if (glDrv.sc_infodata.CoverOpen)
			//{
			//	isCoverOpen = TRUE;
			//	break;
			//}				

			//if (glDrv.sc_infodata.PaperJam)
			//{
			//	if (glDrv.sc_infodata.AdfSensor)
			//	{
			//		isPaperJam = FALSE;
			//	}
			//	else
			//	{
			//		/*There is "scan jam" let scan flow finish for save image*/
			//		isPaperJam = TRUE;
			//		break;
			//	}
			//}

			//if (glDrv.sc_infodata.Cancel)
			//{
			//	start_cancel = TRUE;				
			//	break;
			//}
			
			//if (glDrv.sc_infodata.UltraSonic)
			//{
			//	isUltraSonic = TRUE;
			//	break;
			//}

			if (start_cancel)
			{
				break;
			}

			if ((!(duplex & 1) || glDrv.sc_infodata.ImgStatus[0].EndScan) && (!(duplex & 2) || glDrv.sc_infodata.ImgStatus[1].EndScan))
				break;

		/*	if (_kbhit()) {
				_getch();
				_cancel(JobID);
				cancel = TRUE;
			}*/

			bFinished = false;
			for (dup = 0; dup < 2; dup++) 
			{					
				if ((duplex & (1 << dup)) && glDrv.sc_infodata.ValidPageSize[dup]) 
				{
					ImgSize = 0;
					TotalImgSize = 0;
					currentImgSize = glDrv.sc_infodata.ValidPageSize[dup];

					if (!bFiling[dup])
					{
						bFiling[dup]++;

						char side = 0;
						char fileName[256] = { 0 };
						char filePath[256] = { 0 };
						TCHAR fileNameOut[256] = { 0 };

						::WideCharToMultiByte(CP_ACP, 0, tempPath, -1, filePath, 256, NULL, NULL);

						if (duplex == SCAN_AB_SIDE)
						{
							if (dup == 0)
							{
								side = 'A';
							}
							else if (dup == 1)
							{
								side = 'B';
							}
							if (bColorDetect)
							{
								if(glDrv.sc_infodata.ImgStatus[dup].IsColor)
									sprintf(fileName, "%s%03d%c_color.%s", filePath, page[dup], side, &ImgFile[dup].img.format);//#BMS1075
								else
									sprintf(fileName, "%s%03d%c_gray.%s", filePath, page[dup], side, &ImgFile[dup].img.format);
								if (((ImgFile[dup].img).bit) >= 24)
								{
									if (glDrv.sc_infodata.ImgStatus[dup].IsColor == 0) 
									{
										((ImgFile[dup].img).bit) /= 3;
									}
								}
							}
							else 
							{

								//sprintf(fileName, "%s_%c%d_%c%02d.%s", filePath, (ImgFile[dup].img.bit > 16) ? 'C' : 'G', ImgFile[dup].img.dpi.x, side, page[dup], &ImgFile[dup].img.format);
								sprintf(fileName, "%s%03d%c.%s", filePath, page[dup], side, &ImgFile[dup].img.format);//#BMS1075
							}
						}
						else
						{
							if (bColorDetect)
							{
								if (glDrv.sc_infodata.ImgStatus[dup].IsColor)
									sprintf(fileName, "%s%03d_color.%s", filePath, page[dup], &ImgFile[dup].img.format);//#BMS1075
								else
									sprintf(fileName, "%s%03d_gray.%s", filePath, page[dup], &ImgFile[dup].img.format);
								if (((ImgFile[dup].img).bit) >= 24)
								{
									if (glDrv.sc_infodata.ImgStatus[dup].IsColor == 0)
									{
										((ImgFile[dup].img).bit) /= 3;
									}
								}
							}
							else
							{
								sprintf(fileName, "%s%03d.%s", filePath, page[dup], &ImgFile[dup].img.format);//#BMS1075
							}
						}
						
						ImgFile_Open(&ImgFile[dup], fileName);
						lineCount = 0;

						::MultiByteToWideChar(CP_ACP, 0, fileName, strlen(fileName), fileNameOut, 256);
						bstrArray[fileCount] = ::SysAllocString(fileNameOut);
						MyOutputString(fileNameOut);
						fileCount++;
						page[dup]++;						
					}

					//modified by yunying sahng 2017-12-04 for BMS1035
					//add by yunying shang 2017-10-12 for BMS1082 and 842
					//wsprintf(debugBuf, L"the linecount is %d, bFinished is %d, currentImgSize is %d", lineCount, bFinished, currentImgSize);
					//MyOutputString(debugBuf);
					if (currentImgSize > 0 && lineCount == 0 && !bFinished)
					{
						//MyOutputString(L"uploading!");
						::SendNotifyMessage(HWND_BROADCAST, WM_VOPSCAN_UPLOAD/*uMsg*/, 0, 0);
					}
					////////////1082

					while (currentImgSize > 0)
					{
						LastImgSize = currentImgSize % imgBufferSize;
						if (glDrv._ReadImageEX(dup, &ImgSize, imgBuffer, currentImgSize == LastImgSize ? LastImgSize : imgBufferSize))
						{
							currentImgSize -= ImgSize;
							TotalImgSize += ImgSize;

							ImgFile_Write(&ImgFile[dup], imgBuffer, ImgSize);

							int percent = 0;
							//int L = (int)round((double)ImgSize / (double)GetByteNumPerLineWidthPad(BitsPerPixel, nLinePixelNumOrig));
							lineCount += 50;
							percent = lineCount;
							//MyOutputString(L"Data size ", ImgSize);
							if (percent > 100)
								percent = 100;
							Sleep(100);
						}
						else
						{
							if (start_cancel)
							{
								break;
							}
						}
					}

					if ((TotalImgSize >= (int)glDrv.sc_infodata.ValidPageSize[dup]) && glDrv.sc_infodata.ImgStatus[dup].EndPage)
					{
						//add by yunying shang 2017-10-12 for BMS1082
						MyOutputString(L"SCanning Complete!");
						if (duplex >= 3)
						{
							::SendNotifyMessage(HWND_BROADCAST, WM_VOPSCAN_PAGECOMPLETE/*uMsg*/, (fileCount+1) / 2, 0);
						}
						else
						{
						
							::SendNotifyMessage(HWND_BROADCAST, WM_VOPSCAN_PAGECOMPLETE/*uMsg*/, (fileCount), 0);
						}
						Sleep(50);
						//////////////1082 yunying
						MyOutputString(L"ImgFile Close ", glDrv.sc_infodata.ImageHeight[dup]);

						if (glDrv.sc_infodata.ImgStatus[dup].IsBlank)
						{
							emptyPages[fileCount - 1] = 1;
							bEmptyPage = true;
						}
						else
							emptyPages[fileCount-1] = 0;

						ImgFile_Close(&ImgFile[dup], glDrv.sc_infodata.ImageHeight[dup]);
						bFiling[dup]--;
						lineCount = 0;
						bFinished = true;
					}
				}
				if (start_cancel && bFiling[dup])
				{
					MyOutputString(L"start_cancel Close");
					ImgFile_Close(&ImgFile[dup], glDrv.sc_infodata.ImageHeight[dup]);
					bFiling[dup] = 0;
					lineCount = 0;
				}
			}
		}

		if (start_cancel)
		{
			glDrv._cancel();
			MyOutputString(L"_cancel");

			//add by yunying shang 2017-11-10 for BMS 1372
			glDrv.waitJobFinish(0);
			MyOutputString(L"waitJobFinish");
			//<<==============1372

			glDrv._JobEnd();
			MyOutputString(L"_JobEnd");

			glDrv._CloseDevice();

			if (imgBuffer)
				delete imgBuffer;

			for (UINT i = 0; i < fileCount; i++)
			{
				::SysFreeString(bstrArray[i]);
			}

			return RETSCAN_CANCEL;
		}

		glDrv._stop();
		MyOutputString(L"_stop");

		glDrv.waitJobFinish(0);
		MyOutputString(L"waitJobFinish");

		glDrv._JobEnd();
		MyOutputString(L"_JobEnd");

		glDrv._CloseDevice();

		//contrast, brightness
		if (!start_cancel && !glDrv.sc_infodata.ErrorStatus.cover_open_err && 
			!glDrv.sc_infodata.ErrorStatus.scan_jam_err && !glDrv.sc_infodata.ErrorStatus.multi_feed_err)
		{
			if (brightness != 50 || contrast != 50)
			{
				Gdiplus::Status status;
				if ((status = Gdiplus::GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL)) != Gdiplus::Ok)
				{
					if (imgBuffer)
						delete imgBuffer;
					return RETSCAN_ERROR;
				}

				for (UINT i = 0; i < fileCount; i++)
				{
					BrightnessAndContrast(bstrArray[i], brightness, contrast);
				}

				GdiplusShutdown(gdiplusToken);
			}
		}

		if (bEmptyPage)
		{
			int count = fileCount;
			for (UINT i = 0; i < count; i++)
			{
				if (emptyPages[i])
				{

					for (UINT j = i; j < (count - 1);j++)
					{
						bstrArray[j] = bstrArray[j + 1];
					}
					fileCount--;
				}
			}
		}
		CreateSafeArrayFromBSTRArray
			(
			bstrArray,
			fileCount,
			fileNames
			);

		for (UINT i = 0; i < fileCount; i++)
		{
			::SysFreeString(bstrArray[i]);
		}

		//if (isCoverOpen)
		//{
		//	if (imgBuffer)
		//		delete imgBuffer;
		//	return RETSCAN_COVER_OPEN;
		//}
		//	
		//if (isPaperJam)
		//{ 
		//	if (imgBuffer)
		//		delete imgBuffer;
		//	return RETSCAN_PAPER_JAM;
		//}

		//if (isUltraSonic)
		//{
		//	if (imgBuffer)
		//		delete imgBuffer;
		//	return RETSCAN_ULTRA_SONIC;
		//}

		if (isScanError)
		{
			if (imgBuffer)
				delete imgBuffer;
			return scanRet;
		}
	}
	else
	{
		if (imgBuffer)
			delete imgBuffer;
		MyOutputString(L"Open Device Fail!");
		if(g_connectMode_usb == TRUE)
			return RETSCAN_OPENFAIL;
		else
		{
			if (openRet == 0xFF)
				return RETSCAN_BUSY;
			else
				return RETSCAN_OPENFAIL_NET;
		}
	}

	if (imgBuffer)
		delete imgBuffer;
	MyOutputString(L"SCan Finished!");
	return RETSCAN_OK;

}

#define USBSCANSTRING	  L"\\\\.\\usbscan"
#define MAX_DEVICES       127
USBAPI_API int __stdcall TestUsbScanInit(
	TCHAR* interfaceName,
	SAFEARRAY** endPointNames)
{
	HANDLE hDev = NULL;
	BSTR bstrArray[500] = { 0 };
	TCHAR strPort[32] = { 0 };
	int  iCnt;

	for (iCnt = 0; iCnt <= MAX_DEVICES; iCnt++) {
		_stprintf_s(strPort, L"%s%d", USBSCANSTRING, iCnt);
		 hDev = CreateFile(strPort,
			GENERIC_READ | GENERIC_WRITE,
			FILE_SHARE_READ | FILE_SHARE_WRITE,
			NULL,
			OPEN_EXISTING,
			FILE_FLAG_OVERLAPPED, NULL);

		if (hDev != INVALID_HANDLE_VALUE) {
			_tcscpy_s(interfaceName, 32, strPort);
			break;
		}
	}

	if (hDev == INVALID_HANDLE_VALUE)
		return 0;

	USBSCAN_PIPE_CONFIGURATION PipeConfig;
	DWORD cbRet;

	int result = DeviceIoControl(hDev,
		(DWORD)IOCTL_GET_PIPE_CONFIGURATION,
		NULL,
		0,
		&PipeConfig,
		sizeof(PipeConfig),
		&cbRet,
		NULL);


	TCHAR usbscan_out[256];
	TCHAR usbscan[256];
	HANDLE handle;
	int i = 0;
	if (result) {
		for (i = 0; i < (int)PipeConfig.NumberOfPipes; i++)
		{
			if (PipeConfig.PipeInfo[i].PipeType == USBSCAN_PIPE_BULK) {
				_stprintf_s(usbscan, L"%s\\%d  USBSCAN_PIPE_BULK", strPort, i);
				bstrArray[i] = ::SysAllocString(usbscan);
			}
			else if (PipeConfig.PipeInfo[i].PipeType == USBSCAN_PIPE_CONTROL) {
				_stprintf_s(usbscan, L"%s\\%d  USBSCAN_PIPE_CONTROL", strPort, i);
				bstrArray[i] = ::SysAllocString(usbscan);
			}
			else if (PipeConfig.PipeInfo[i].PipeType == USBSCAN_PIPE_ISOCHRONOUS) {
				_stprintf_s(usbscan, L"%s\\%d  USBSCAN_PIPE_ISOCHRONOUS", strPort, i);
				bstrArray[i] = ::SysAllocString(usbscan);
			}
			else if (PipeConfig.PipeInfo[i].PipeType == USBSCAN_PIPE_INTERRUPT) {
				_stprintf_s(usbscan, L"%s\\%d  USBSCAN_PIPE_INTERRUPT", strPort, i);
				bstrArray[i] = ::SysAllocString(usbscan);
			}
		}
	}
	
	CreateSafeArrayFromBSTRArray
		(
			bstrArray,
			i,
			endPointNames
			);

	for (UINT j = 0; j < 1; j++)
	{
		::SysFreeString(bstrArray[j]);
	}

	if (hDev != INVALID_HANDLE_VALUE) {
		CloseHandle(hDev);
	}
}

USBAPI_API int __stdcall CheckUsbScanByName(
	WCHAR* interfaceName)
{
	HANDLE hDev = NULL;
	TCHAR strPortAlt[32] = { 0 };
	int  iCnt;
	int error = 0;

	hDev = CreateFile(interfaceName,
		GENERIC_READ | GENERIC_WRITE,
		FILE_SHARE_READ | FILE_SHARE_WRITE,
		NULL,
		OPEN_EXISTING,
		FILE_FLAG_OVERLAPPED, NULL);

	if (hDev != INVALID_HANDLE_VALUE)
	{
		CloseHandle(hDev);
	}
	else
	{
		error = GetLastError();
		return 0;
	}
	

	CGLDrv glDrv;

	if (glDrv._OpenUSBDevice(interfaceName) == FALSE)
	{
		glDrv._CloseDevice();
		return 0;
	}
	return 1;
}


USBAPI_API int __stdcall CheckUsbScan(
	char* interfaceName)
{
	HANDLE hDev = NULL;
	TCHAR strPort[32] = { 0 };
	TCHAR strPortAlt[32] = { 0 };
	int  iCnt;
	int error = 0;

	//EnterCriticalSection(&g_csCriticalSection_UsbTest);

	for (iCnt = 0; iCnt <= MAX_DEVICES; iCnt++) 
	{
		_stprintf_s(strPort, L"%s%d", USBSCANSTRING, iCnt);
		_stprintf_s(strPortAlt, L"%s%d", L"USB Device ", iCnt);
		hDev = CreateFile(strPort,
			GENERIC_READ | GENERIC_WRITE,
			FILE_SHARE_READ | FILE_SHARE_WRITE,
			NULL,
			OPEN_EXISTING,
			FILE_FLAG_OVERLAPPED, NULL);
	
		if (hDev != INVALID_HANDLE_VALUE) 
		{
			::WideCharToMultiByte(CP_ACP, 0, strPortAlt, -1, interfaceName, 32, NULL, NULL);
			break;
		}
		else
		{
			error = GetLastError();
		}
	}

	if (hDev == INVALID_HANDLE_VALUE)
	{
		//LeaveCriticalSection(&g_csCriticalSection_UsbTest);
		return 0;
	}
		
	if (hDev != INVALID_HANDLE_VALUE) {
		CloseHandle(hDev);
	}

	//LeaveCriticalSection(&g_csCriticalSection_UsbTest);
	CGLDrv glDrv;

	//g_connectMode_usb = TRUE;//add by yunying shang 2017-11-10 for BMS 1381

	if (glDrv._OpenUSBDevice(strPort) == FALSE)//#bms1005
	{
		glDrv._CloseDevice();
		return 0;		
	}
	return 1;
}

BOOL TestIpConnected1(wchar_t* szIP, Scan_RET *re_status)
{
	int nResult = TRUE;
	TCHAR showIp[256] = { 0 };
	int count = 3;
	//CGLNet m_GLnet;

	while (count-- > 0)
	{
		if (g_GLnet.CMDIO_Connect(szIP, 23011))
		{
			TCHAR showIp[256] = { 0 };
			wsprintf(showIp, L"\nTestIpConnected() success %s", szIP);
			OutputDebugString(showIp);

			U8 cmd[4] = { 'J','D','G','S' };
			U8 status[8] = { 0 };

			if (g_GLnet.CMDIO_Write(cmd, 4) == TRUE)
			{
				if (g_GLnet.CMDIO_Read(status, 8))
				{
					if (status[0] == 'J'
						&& status[1] == 'D'
						&& status[2] == 'A'
						&& status[3] == 'T'
						&& status[4] == 0x00)
					{
						*re_status = RETSCAN_OK;
					}
					else
					{
						*re_status = RETSCAN_BUSY;
					}

					nResult = TRUE;

					g_GLnet.CMDIO_Close();
					break;
				}
				else
				{
					wsprintf(showIp, L"\nTestIpConnected() read command Fail, %s", szIP);
					OutputDebugString(showIp);
					nResult = FALSE;
				}

			}
			else
			{
				wsprintf(showIp, L"\nTestIpConnected() Write command Fail, %s", szIP);
				OutputDebugString(showIp);
				nResult = FALSE;
			}

			g_GLnet.CMDIO_Close();
		}
		else
		{

			wsprintf(showIp, L"\nTestIpConnected() Fail %s", szIP);
			OutputDebugString(showIp);

			nResult = FALSE;
		}
	}

	return nResult;
}
BOOL TestIpConnected2(wchar_t* szIP, Scan_RET *re_status)
{
	int nResult = TRUE;
	TCHAR showIp[256] = { 0 };

	if (g_GLnet.CMDIO_Connect(szIP, 23011))
	{
		TCHAR showIp[256] = { 0 };
		wsprintf(showIp, L"\nTestIpConnected() success %s", szIP);
		OutputDebugString(showIp);

		U8 cmd[4] = { 'J','D','G','S' };
		U8 status[8] = { 0 };

		if (g_GLnet.CMDIO_Write(cmd, 4) == TRUE)
		{
			if (g_GLnet.CMDIO_Read(status, 8))
			{
				if (status[0] == 'J'
					&& status[1] == 'D'
					&& status[2] == 'A'
					&& status[3] == 'T'
					&& status[4] == 0x00)
				{
					*re_status = RETSCAN_OK;
				}
				else
				{
					*re_status = RETSCAN_BUSY;
				}

				nResult = TRUE;

				g_GLnet.CMDIO_Close();
			}
			else
			{
				wsprintf(showIp, L"\nTestIpConnected() read command Fail, %s", szIP);
				OutputDebugString(showIp);
				nResult = FALSE;
			}

		}
		else
		{
			wsprintf(showIp, L"\nTestIpConnected() Write command Fail, %s", szIP);
			OutputDebugString(showIp);
			nResult = FALSE;
		}

		g_GLnet.CMDIO_Close();
	}
	else
	{

		wsprintf(showIp, L"\nTestIpConnected() Fail %s", szIP);
		OutputDebugString(showIp);

		nResult = FALSE;
	}


	return nResult;
}
USBAPI_API BOOL __stdcall TestIpConnected(wchar_t* szIP)
{
	Scan_RET re_status = RETSCAN_OK;

	if (wcslen(szIP) == 0)
		return false;

	if (TestIpConnected1(szIP, &re_status) == TRUE)
	{
		return true;
	}
	else
	{
		return false;
	}

	int nResult = TRUE;
	//CGLNet m_GLnet;

	if (g_GLnet.CMDIO_Connect(szIP, 23011))
	{
		TCHAR showIp[256] = { 0 };
		wsprintf(showIp, L"\nTestIpConnected() success %s", szIP);
		//OutputDebugString(showIp);

		nResult = TRUE;
		g_GLnet.CMDIO_Close();
	}
	else
	{

		TCHAR showIp[256] = { 0 };
		wsprintf(showIp, L"\nTestIpConnected() Fail %s", szIP);
		OutputDebugString(showIp);

		nResult = FALSE;
	}

	return nResult;
}

USBAPI_API void __stdcall SetConnectionMode(
	const wchar_t* deviceName, BOOL isUsb)
{
	if(isUsb)
		_tcscpy_s(g_deviceName, 256, deviceName);
	else
		_tcscpy_s(g_ipAddress, 256, deviceName);
	g_connectMode_usb = isUsb;
}

USBAPI_API BOOL __stdcall CheckConnectionByName(WCHAR* interfaceName)
{
	//MyOutputString(L"CheckConnectionByName===>Enter");
	//MyOutputString(interfaceName);
	if (g_connectMode_usb)
	{
		HANDLE hDev = NULL;
		int error = 0;
		//char _usbname[256] = { 0 };
		//::WideCharToMultiByte(CP_ACP, 0, interfaceName, -1, _usbname, 256, NULL, NULL);
		hDev = CreateFile(interfaceName,
			GENERIC_READ | GENERIC_WRITE,
			FILE_SHARE_READ | FILE_SHARE_WRITE,
			NULL,
			OPEN_EXISTING,
			FILE_FLAG_OVERLAPPED, NULL);

		if (hDev != INVALID_HANDLE_VALUE)
		{
			return true;
		}
		else
		{
			error = GetLastError();
			return false;
		}
		//return (BOOL)CheckUsbScan(_usbname);
	}
	else
	{
		return TestIpConnected(interfaceName);
	}

}

USBAPI_API BYTE __stdcall GetPowerSupply()
{
	BYTE power = 0;
	CGLDrv glDrv;
	char interfaceName[32] = { 0 };
	if (g_connectMode_usb == 1)
	{
		HANDLE hDev = NULL;
		TCHAR strPort[32] = { 0 };
		int  iCnt;
		int error = 0;

		for (iCnt = 0; iCnt <= MAX_DEVICES; iCnt++) 
		{
			_stprintf_s(strPort, L"%s%d", USBSCANSTRING, iCnt);

			hDev = CreateFile(strPort,
				GENERIC_READ | GENERIC_WRITE,
				FILE_SHARE_READ | FILE_SHARE_WRITE,
				NULL,
				OPEN_EXISTING,
				FILE_FLAG_OVERLAPPED, NULL);

			if (hDev != INVALID_HANDLE_VALUE)
			{
				break;
			}
			else
			{
				error = GetLastError();
			}
		}

		if (hDev == INVALID_HANDLE_VALUE)
		{
			return 0;
		}

		if (hDev != INVALID_HANDLE_VALUE) 
		{
			CloseHandle(hDev);
		}

		if (glDrv._OpenUSBDevice(strPort) != FALSE)
		{
			power = glDrv._GetPowerSupply();
			glDrv._CloseDevice();
		}	
	}
	else
	{
		Scan_RET re_status = RETSCAN_OK;

		if (TestIpConnected2(g_ipAddress, &re_status) == TRUE)
		{
			if (re_status != RETSCAN_BUSY)
			{
				if (glDrv._OpenDevice(g_ipAddress) == TRUE)
				{
					if (!glDrv.NetScanReady())
					{

					}
					else
					{
						power = glDrv._GetPowerSupply();
					}
					glDrv._CloseDevice();
				}
			}

		}
	}
	return power;

}

USBAPI_API int __stdcall GetScanCount(//byte mode, 
	int* count1, int* count2, int* count3)
{
	BYTE data[4] = { 0 };
	CGLDrv glDrv;
	int result = FALSE;
	int addr = 0x48;
	//switch (mode)
	//{
	//case 0:
	//	addr = 0x48;
	//	break;
	//case 1:
	//	addr = 0x4c;
	//	break;
	//case 2:
	//	addr = 0x00;
	//	break;
	//default:
	//	addr = 0x48;
	//	break;
	//}
	if (g_connectMode_usb == 1)
	{
		HANDLE hDev = NULL;
		TCHAR strPort[32] = { 0 };
		int  iCnt;
		int error = 0;

		for (iCnt = 0; iCnt <= MAX_DEVICES; iCnt++)
		{
			_stprintf_s(strPort, L"%s%d", USBSCANSTRING, iCnt);

			hDev = CreateFile(strPort,
				GENERIC_READ | GENERIC_WRITE,
				FILE_SHARE_READ | FILE_SHARE_WRITE,
				NULL,
				OPEN_EXISTING,
				FILE_FLAG_OVERLAPPED, NULL);

			if (hDev != INVALID_HANDLE_VALUE)
			{
				break;
			}
			else
			{
				error = GetLastError();
			}
		}

		if (hDev == INVALID_HANDLE_VALUE)
		{
			return 0;
		}

		if (hDev != INVALID_HANDLE_VALUE)
		{
			CloseHandle(hDev);
		}

		if (glDrv._OpenUSBDevice(strPort) != FALSE)
		{
			//if (glDrv.NVRAM_read(addr, 4, data))
			//{
			//	*count = *((DWORD*)data);
			//	result = true;
			//}

			if (glDrv.NVRAM_read(0x48, 4, data))
			{
				*count1 = *((DWORD*)data);
				result = true;
			}

			if (glDrv.NVRAM_read(0x4c, 4, data))
			{
				*count2 = *((DWORD*)data);
				result = true;
			}

			if (glDrv.NVRAM_read(0x00, 4, data))
			{
				*count3 = *((DWORD*)data);
				result = true;
			}
			glDrv._CloseDevice();
		}
	}
	else
	{
		Scan_RET re_status = RETSCAN_OK;

		if (TestIpConnected2(g_ipAddress, &re_status) == TRUE)
		{
			if (re_status != RETSCAN_BUSY)
			{
				if (glDrv._OpenDevice(g_ipAddress) == TRUE)
				{
					if (!glDrv.NetScanReady())
					{

					}
					else
					{
						//if (glDrv.NVRAM_read(addr, 4, data))
						//{
						//	*count = (((DWORD)data[3] << 24 & 0xFF000000) + ((DWORD)data[2] << 16 & 0x00FF0000)
						//		+ ((DWORD)data[1] << 8 & 0x0000FF00) + ((DWORD)data[0] & 0xff));
						//	result = TRUE;
						//}

						if (glDrv.NVRAM_read(0x48, 4, data))
						{
							*count1 = (((DWORD)data[3] << 24 & 0xFF000000) + ((DWORD)data[2] << 16 & 0x00FF0000)
								+ ((DWORD)data[1] << 8 & 0x0000FF00) + ((DWORD)data[0] & 0xff));
							result = TRUE;
						}

						if (glDrv.NVRAM_read(0x4c, 4, data))
						{
							*count2 = (((DWORD)data[3] << 24 & 0xFF000000) + ((DWORD)data[2] << 16 & 0x00FF0000)
								+ ((DWORD)data[1] << 8 & 0x0000FF00) + ((DWORD)data[0] & 0xff));
							result = TRUE;
						}

						if (glDrv.NVRAM_read(0x00, 4, data))
						{
							*count3 = (((DWORD)data[3] << 24 & 0xFF000000) + ((DWORD)data[2] << 16 & 0x00FF0000)
								+ ((DWORD)data[1] << 8 & 0x0000FF00) + ((DWORD)data[0] & 0xff));
							result = TRUE;
						}
					}
					glDrv._CloseDevice();
				}
			}

		}
	}
	return result;
}

USBAPI_API int __stdcall ClearScanCount(byte mode)
{
	BYTE data[4] = { 0 };
	CGLDrv glDrv;
	int result = FALSE;
	int addr = 0x48;
	switch (mode)
	{
	case 0:
		addr = 0x48;
		break;
	case 1:
		addr = 0x4c;
		break;
	default:
		addr = 0x48;
		break;
	}
	if (g_connectMode_usb == 1)
	{
		HANDLE hDev = NULL;
		TCHAR strPort[32] = { 0 };
		int  iCnt;
		int error = 0;

		for (iCnt = 0; iCnt <= MAX_DEVICES; iCnt++)
		{
			_stprintf_s(strPort, L"%s%d", USBSCANSTRING, iCnt);

			hDev = CreateFile(strPort,
				GENERIC_READ | GENERIC_WRITE,
				FILE_SHARE_READ | FILE_SHARE_WRITE,
				NULL,
				OPEN_EXISTING,
				FILE_FLAG_OVERLAPPED, NULL);

			if (hDev != INVALID_HANDLE_VALUE)
			{
				break;
			}
			else
			{
				error = GetLastError();
			}
		}

		if (hDev == INVALID_HANDLE_VALUE)
		{
			return 0;
		}

		if (hDev != INVALID_HANDLE_VALUE)
		{
			CloseHandle(hDev);
		}

		if (glDrv._OpenUSBDevice(strPort) != FALSE)
		{
			if (glDrv.NVRAM_write(addr, 4, data))
			{
				result = TRUE;
			}
			glDrv._CloseDevice();
		}
	}
	else
	{
		Scan_RET re_status = RETSCAN_OK;

		if (TestIpConnected2(g_ipAddress, &re_status) == TRUE)
		{
			if (re_status != RETSCAN_BUSY)
			{
				if (glDrv._OpenDevice(g_ipAddress) == TRUE)
				{
					if (!glDrv.NetScanReady())
					{

					}
					else
					{
						if (glDrv.NVRAM_write(addr, 4, data))
						{
							result = TRUE;
						}
					}
					glDrv._CloseDevice();
				}
			}

		}
	}
	return result;
}


USBAPI_API BYTE __stdcall GetButtonPressed()
{
	BYTE pressed = 0;
	CGLDrv glDrv;
	char interfaceName[32] = { 0 };
	if (g_connectMode_usb == 1)
	{
		HANDLE hDev = NULL;
		TCHAR strPort[32] = { 0 };
		int  iCnt;
		int error = 0;

		for (iCnt = 0; iCnt <= MAX_DEVICES; iCnt++)
		{
			_stprintf_s(strPort, L"%s%d", USBSCANSTRING, iCnt);

			hDev = CreateFile(strPort,
				GENERIC_READ | GENERIC_WRITE,
				FILE_SHARE_READ | FILE_SHARE_WRITE,
				NULL,
				OPEN_EXISTING,
				FILE_FLAG_OVERLAPPED, NULL);

			if (hDev != INVALID_HANDLE_VALUE)
			{
				break;
			}
			else
			{
				error = GetLastError();
			}
		}

		if (hDev == INVALID_HANDLE_VALUE)
		{
			return 0;
		}

		if (hDev != INVALID_HANDLE_VALUE)
		{
			CloseHandle(hDev);
		}

		if (glDrv._OpenUSBDevice(strPort) != FALSE)
		{
			pressed = glDrv._GetButtonPressed();
			glDrv._CloseDevice();
		}
	}
	//else
	//{
	//	Scan_RET re_status = RETSCAN_OK;

	//	if (TestIpConnected1(g_ipAddress, &re_status) == TRUE)
	//	{
	//		if (re_status != RETSCAN_BUSY)
	//		{
	//			if (glDrv._OpenDevice(g_ipAddress) == TRUE)
	//			{
	//				if (!glDrv.NetScanReady())
	//				{

	//				}
	//				else
	//				{
	//					pressed = glDrv._GetButtonPressed();
	//				}
	//				glDrv._CloseDevice();
	//			}
	//		}

	//	}
	//}
	return pressed;

}

USBAPI_API BOOL __stdcall CheckConnection()
{

	if (g_connectMode_usb)
	{
		char interfaceName[32] = { 0 };
		return (BOOL)CheckUsbScan(interfaceName);
	}
	else
	{
		/*char _hostname[256] = { 0 };
		::WideCharToMultiByte(CP_ACP, 0, g_ipAddress, -1, _hostname, 256, NULL, NULL);*/
		return TestIpConnected(g_ipAddress);
	}

}

USBAPI_API int __stdcall SetPowerSaveTime(const wchar_t* szPrinter, WORD sleepTime, WORD offTime)
{
	int nResult = 4;

	CGLDrv glDrv;
	char interfaceName[32] = { 0 };
	if (g_connectMode_usb == 1)
	{
		HANDLE hDev = NULL;
		TCHAR strPort[32] = { 0 };
		int  iCnt;
		int error = 0;

		for (iCnt = 0; iCnt <= MAX_DEVICES; iCnt++)
		{
			_stprintf_s(strPort, L"%s%d", USBSCANSTRING, iCnt);

			hDev = CreateFile(strPort,
				GENERIC_READ | GENERIC_WRITE,
				FILE_SHARE_READ | FILE_SHARE_WRITE,
				NULL,
				OPEN_EXISTING,
				FILE_FLAG_OVERLAPPED, NULL);

			if (hDev != INVALID_HANDLE_VALUE)
			{
				break;
			}
			else
			{
				error = GetLastError();
			}
		}

		if (hDev == INVALID_HANDLE_VALUE)
		{
			return 31;
		}

		if (hDev != INVALID_HANDLE_VALUE)
		{
			CloseHandle(hDev);
		}

		if (glDrv._OpenUSBDevice(strPort) != FALSE)
		{
			nResult = glDrv._SetPowerSaveTime(sleepTime, offTime);
			glDrv._CloseDevice();
		}
		else
		{
			nResult = 11;
		}
	}
	else
	{
		Scan_RET re_status = RETSCAN_OK;

		if (TestIpConnected2(g_ipAddress, &re_status) == TRUE)
		{
			if (re_status != RETSCAN_BUSY)
			{
				if (glDrv._OpenDevice() == TRUE)
				{
					nResult = glDrv._SetPowerSaveTime(sleepTime, offTime);
					glDrv._CloseDevice();
				}
				else
				{
					nResult = 21;
				}
			}
		}
		else
		{
			nResult = 31;
		}
	}
	return nResult;
}

USBAPI_API int __stdcall GetPowerSaveTime(const wchar_t* szPrinter, WORD* ptrSleepTime, WORD* ptrOffTime)
{
	int nResult = 4;

	CGLDrv glDrv;
	char interfaceName[32] = { 0 };
	if (g_connectMode_usb == 1)
	{
		HANDLE hDev = NULL;
		TCHAR strPort[32] = { 0 };
		int  iCnt;
		int error = 0;

		for (iCnt = 0; iCnt <= MAX_DEVICES; iCnt++)
		{
			_stprintf_s(strPort, L"%s%d", USBSCANSTRING, iCnt);

			hDev = CreateFile(strPort,
				GENERIC_READ | GENERIC_WRITE,
				FILE_SHARE_READ | FILE_SHARE_WRITE,
				NULL,
				OPEN_EXISTING,
				FILE_FLAG_OVERLAPPED, NULL);

			if (hDev != INVALID_HANDLE_VALUE)
			{
				break;
			}
			else
			{
				error = GetLastError();
			}
		}

		if (hDev == INVALID_HANDLE_VALUE)
		{
			return 31;
		}

		if (hDev != INVALID_HANDLE_VALUE)
		{
			CloseHandle(hDev);
		}

		if (glDrv._OpenUSBDevice(strPort) != FALSE)
		{
			nResult = glDrv._GetPowerSaveTime(ptrSleepTime, ptrOffTime);
			glDrv._CloseDevice();
		}
		else
		{
			nResult = 11;
		}
	}
	else
	{
		Scan_RET re_status = RETSCAN_OK;

		if (TestIpConnected2(g_ipAddress, &re_status) == TRUE)
		{
			if (re_status != RETSCAN_BUSY)
			{
				if (glDrv._OpenDevice() == TRUE)
				{
					nResult = glDrv._GetPowerSaveTime(ptrSleepTime, ptrOffTime);
					glDrv._CloseDevice();
				}
				else
				{
					nResult = 21;
				}
			}

		}
	}
	return nResult;
}

//add by yunying shang 2018-01-19 for Push Scan
USBAPI_API int __stdcall GetScanButton()
{
	U8  Key[64];
	CGLDrv glDrv;
	char interfaceName[32] = { 0 };
	if (g_connectMode_usb == 1)
	{
		HANDLE hDev = NULL;
		TCHAR strPort[32] = { 0 };
		int  iCnt;
		int error = 0;

		for (iCnt = 0; iCnt <= MAX_DEVICES; iCnt++)
		{
			_stprintf_s(strPort, L"%s%d", USBSCANSTRING, iCnt);

			hDev = CreateFile(strPort,
				GENERIC_READ | GENERIC_WRITE,
				FILE_SHARE_READ | FILE_SHARE_WRITE,
				NULL,
				OPEN_EXISTING,
				FILE_FLAG_OVERLAPPED, NULL);

			if (hDev != INVALID_HANDLE_VALUE)
			{
				break;
			}
			else
			{
				error = GetLastError();
			}
		}

		if (hDev == INVALID_HANDLE_VALUE)
		{
			return 0;
		}

		if (hDev != INVALID_HANDLE_VALUE)
		{
			CloseHandle(hDev);
		}

		if (glDrv._OpenUSBDevice(strPort) != FALSE)
		{
			int iRet = glDrv._GetScanButton(Key, 64);
			if (iRet)
			{
				//if (Key[0] == 0x10)
				{
					glDrv._CloseDevice();
					return TRUE;
				}
			}
			glDrv._CloseDevice();
		}
	}
	return FALSE;
}
USBAPI_API int __stdcall GetScanType(int* mode)
{
	BYTE data[1] = { 0 };
	CGLDrv glDrv;
	int result = FALSE;
	int addr = 0x48;
	int type = 0;
	if (g_connectMode_usb == 1)
	{
		HANDLE hDev = NULL;
		TCHAR strPort[32] = { 0 };
		int  iCnt;
		int error = 0;

		for (iCnt = 0; iCnt <= MAX_DEVICES; iCnt++)
		{
			_stprintf_s(strPort, L"%s%d", USBSCANSTRING, iCnt);

			hDev = CreateFile(strPort,
				GENERIC_READ | GENERIC_WRITE,
				FILE_SHARE_READ | FILE_SHARE_WRITE,
				NULL,
				OPEN_EXISTING,
				FILE_FLAG_OVERLAPPED, NULL);

			if (hDev != INVALID_HANDLE_VALUE)
			{
				break;
			}
			else
			{
				error = GetLastError();
			}
		}

		if (hDev == INVALID_HANDLE_VALUE)
		{
			return 0;
		}

		if (hDev != INVALID_HANDLE_VALUE)
		{
			CloseHandle(hDev);
		}

		if (glDrv._OpenUSBDevice(strPort) != FALSE)
		{
			if (glDrv.NVRAM_read(0xc3, 1, data))
			{
				type = data[0];
				if (type == 3)
					*mode = 0;
				else
					*mode = 1;

				result = true;
			}

			glDrv._CloseDevice();
		}
	}

	return result;
}
//add by yunying shang 2018-01-19 for Push Scan
USBAPI_API int __stdcall SetScanType(int mode)
{
	U8  Key[64];
	CGLDrv glDrv;
	char interfaceName[32] = { 0 };
	if (g_connectMode_usb == 1)
	{
		HANDLE hDev = NULL;
		TCHAR strPort[32] = { 0 };
		int  iCnt;
		int error = 0;

		for (iCnt = 0; iCnt <= MAX_DEVICES; iCnt++)
		{
			_stprintf_s(strPort, L"%s%d", USBSCANSTRING, iCnt);

			hDev = CreateFile(strPort,
				GENERIC_READ | GENERIC_WRITE,
				FILE_SHARE_READ | FILE_SHARE_WRITE,
				NULL,
				OPEN_EXISTING,
				FILE_FLAG_OVERLAPPED, NULL);

			if (hDev != INVALID_HANDLE_VALUE)
			{
				break;
			}
			else
			{
				error = GetLastError();
			}
		}

		if (hDev == INVALID_HANDLE_VALUE)
		{
			return 0;
		}

		if (hDev != INVALID_HANDLE_VALUE)
		{
			CloseHandle(hDev);
		}

		if (glDrv._OpenUSBDevice(strPort) != FALSE)
		{
			BYTE data[1] = { 3 };
			if (mode == 0)
				data[0] = 3;
			else
				data[0] = 2;//0
			int iRet = glDrv.NVRAM_write(0xc3, 1, data);
			if (iRet)
			{	
				glDrv._CloseDevice();
				return TRUE;				
			}
			glDrv._CloseDevice();
		}
	}
	return FALSE;
}
//********************************************************************************************
//Do Calibration //Devid added 2017/10/30

#include "Calibration/usbio.h"
#include "Calibration/ScanCMD.h"

extern void job_Set_Calibration_Par(unsigned char type, SC_PAR_T_ *_par);
extern int job_Calibration(SC_PAR_T_ *_par);
extern U32 read_from_ini(void);

extern SC_PAR_T_ sc_par;

U8 SCAN_DOC_SIZE = DOC_SIZE_FULL;
char IniFile[256], Profile[64];

extern Scan_RET ScannerStatusCheck_(char stage);

USBAPI_API int __stdcall DoCalibration()
{
	Scan_RET nResult = RETSCAN_OK;

	read_from_ini();

	if (!CMDIO_OpenDevice()) 
	{
		MyOutputString(L"Get Info Fail!");
		return RETSCAN_OPENFAIL;
	}

	job_Set_Calibration_Par(1, &sc_par);

	nResult = ScannerStatusCheck_(START_STAGE);

	if (nResult == RETSCAN_OK)
	{
		int nCalibration = FALSE;
		nCalibration = job_Calibration(&sc_par);

		if (nCalibration == TRUE)
		{
			nResult = RETSCAN_OK;
		}
		else
		{
			nResult = ScannerStatusCheck_(SCANNING_STAGE);
			//	nResult = RETSCAN_CANCEL;
		}
	}

	CMDIO_CloseDevice();

	return nResult;
}

