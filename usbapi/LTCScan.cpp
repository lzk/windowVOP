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
	RETSCAN_ADF_NOT_READY = 15,
	RETSCAN_HOME_NOT_READY = 16,
	RETSCAN_ULTRA_SONIC = 17,
};

extern UINT WM_VOPSCAN_PROGRESS;
extern UINT WM_VOPSCAN_UPLOAD;
extern UINT WM_VOPSCAN_PAGECOMPLETE;

extern CRITICAL_SECTION g_csCriticalSection_UsbTest;
extern CRITICAL_SECTION g_csCriticalSection_NetWorkTest;

extern BOOL TestIpConnected(wchar_t* szIP);
extern BOOL TestIpConnected(wchar_t* szIP, Scan_RET *status);

wchar_t g_ipAddress[256] = { 0 };
BOOL g_connectMode_usb = FALSE;
static Gdiplus::GdiplusStartupInput gdiplusStartupInput;
static ULONG_PTR gdiplusToken;

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
	SAFEARRAY** fileNames)
{
	
	BSTR bstrArray[500] = { 0 };
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


	glDrv.sc_pardata.acquire = ((MultiFeed ? 1 : 0) * ACQ_ULTRA_SONIC) | ((AutoCrop ? 1 : 0) * ACQ_CROP_DESKEW) |  0 * ACQ_NO_GAMMA;

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
		if (TestIpConnected(g_ipAddress, &re_status) == TRUE)
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
	if (glDrv._OpenDevice() == TRUE)
	{
		
		if (!glDrv.NetScanReady())
		{
			if (imgBuffer)
				delete imgBuffer;
			glDrv._CloseDevice();

			return RETSCAN_BUSY;
		}

	/*	result = glDrv.paperReady();
		if (!result) {

			if (imgBuffer)
				delete imgBuffer;
			glDrv._CloseDevice();
			return RETSCAN_PAPER_NOT_READY;
		}
		MyOutputString(L"paperReady");*/

		result = glDrv._JobCreate();
		if (result != 0)
		{
			int errorcode = RETSCAN_CREATE_JOB_FAIL;

			if (imgBuffer)
				delete imgBuffer;
			glDrv._CloseDevice();

			switch (result) {
			case ADF_NOT_READY_ERR:
				errorcode = RETSCAN_ADF_NOT_READY;
				break;
			case DOC_NOT_READY_ERR:
				errorcode = RETSCAN_PAPER_NOT_READY;
				break;
			case HOME_NOT_READY_ERR:
				errorcode = RETSCAN_HOME_NOT_READY;
				break;
			case SCAN_JAM_ERR:
				errorcode = RETSCAN_PAPER_JAM;
				break;
			case COVER_OPEN_ERR:
				errorcode = RETSCAN_COVER_OPEN;
				break;
			}

			return errorcode;
		}
		

		MyOutputString(L"_JobCreate");


		//gamma
		int i, numread;
		unsigned int gGammaData[768];
		U32 up, down;
		double gamma = 1.3;
		unsigned int Red[65536];
		unsigned int Green[65536];
		unsigned int Blue[65536];
		unsigned int *pbyRed = Red;
		unsigned int *pbyGreen = Green;
		unsigned int *pbyBlue = Blue;


		//unsigned int *gGammaData;	
		for (i = 0; i<65536; i++) {
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

			Red[65535 - i] = Temp;
			Green[65535 - i] = Temp;
			Blue[65535 - i] = Temp;
		}

		GammaTransLTCtoGL(pbyRed, pbyGreen, pbyBlue, gGammaData);

		result = glDrv._gamma(gGammaData);
		if (!result)
		{
			if (imgBuffer)
				delete imgBuffer;
			glDrv._CloseDevice();
			return RETSCAN_ERRORPARAMETER;
		}

		result = glDrv._parameters();
		MyOutputString(L"_parameters");
		if (!result)
		{
			if (imgBuffer)
				delete imgBuffer;
			glDrv._CloseDevice();
			return RETSCAN_ERRORPARAMETER;
		}
		

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
		BOOL isCoverOpen = FALSE;
		BOOL isPaperJam = FALSE;
		BOOL isUltraSonic = FALSE;

		while (!start_cancel)
		{

			if (!glDrv._info())
			{
				/*Sleep(100);
				continue;*/
			}
				
			if (glDrv.sc_infodata.CoverOpen)
			{
				isCoverOpen = TRUE;
				break;
			}
				

			if (glDrv.sc_infodata.PaperJam)
			{
				if (glDrv.sc_infodata.AdfSensor)
				{
					isPaperJam = FALSE;
				}
				else {
					/*There is "scan jam" let scan flow finish for save image*/
					isPaperJam = TRUE;
					break;
				}
			}
			
			if (glDrv.sc_infodata.UltraSonic)
			{
				isUltraSonic = TRUE;
				break;
			}

			if ((!(duplex & 1) || glDrv.sc_infodata.EndScan[0]) && (!(duplex & 2) || glDrv.sc_infodata.EndScan[1]))
				break;
		/*	if (_kbhit()) {
				_getch();
				_cancel(JobID);
				cancel = TRUE;
			}*/
			bool bFinished = false;
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

						if (dup == 0)
						{
							side = 'A';
						}
						else if (dup == 1)
						{
							side = 'B';
						}

//						sprintf(fileName, "%s_%c%d_%c%02d.%s", filePath, (ImgFile[dup].img.bit > 16) ? 'C' : 'G', ImgFile[dup].img.dpi.x, side, page[dup], &ImgFile[dup].img.format);
						sprintf(fileName, "%s%03d%c.%s", filePath, page[dup],side,&ImgFile[dup].img.format);//#BMS1075
						ImgFile_Open(&ImgFile[dup], fileName);
						lineCount = 0;

						::MultiByteToWideChar(CP_ACP, 0, fileName, strlen(fileName), fileNameOut, 256);
						bstrArray[fileCount] = ::SysAllocString(fileNameOut);
						MyOutputString(fileNameOut);
						fileCount++;
						page[dup]++;
					}

					//add by yunying shang 2017-10-12 for BMS1082 and 842
					if(currentImgSize > 0 && lineCount != 0 && !bFinished)
						::SendNotifyMessage(HWND_BROADCAST, WM_VOPSCAN_UPLOAD/*uMsg*/, 0, 0);
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
					}
					if ((TotalImgSize >= (int)glDrv.sc_infodata.ValidPageSize[dup]) && glDrv.sc_infodata.EndPage[dup])
					{
						//add by yunying shang 2017-10-12 for BMS1082
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
		if (!start_cancel && !glDrv.sc_infodata.CoverOpen && !glDrv.sc_infodata.PaperJam && !glDrv.sc_infodata.UltraSonic)
		{
			if (brightness != 50 || contrast != 50)
			{
				Gdiplus::Status status;
				if ((status = Gdiplus::GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL)) != Gdiplus::Ok)
				{
					return RETSCAN_ERROR;
				}

				for (UINT i = 0; i < fileCount; i++)
				{
					BrightnessAndContrast(bstrArray[i], brightness, contrast);
				}

				GdiplusShutdown(gdiplusToken);
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

		if (isCoverOpen)
		{
			if (imgBuffer)
				delete imgBuffer;
			return RETSCAN_COVER_OPEN;
		}
			
		if (isPaperJam)
		{ 
			if (imgBuffer)
				delete imgBuffer;
			return RETSCAN_PAPER_JAM;
		}

		if (isUltraSonic)
		{
			if (imgBuffer)
				delete imgBuffer;
			return RETSCAN_ULTRA_SONIC;
		}

	}
	else
	{
		if (imgBuffer)
			delete imgBuffer;

		if(g_connectMode_usb == TRUE)
			return RETSCAN_OPENFAIL;
		else
			return RETSCAN_OPENFAIL_NET;
	}

	if (imgBuffer)
		delete imgBuffer;

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

USBAPI_API int __stdcall CheckUsbScan(
	char* interfaceName)
{
	HANDLE hDev = NULL;
	TCHAR strPort[32] = { 0 };
	TCHAR strPortAlt[32] = { 0 };
	int  iCnt;
	int error = 0;

	//EnterCriticalSection(&g_csCriticalSection_UsbTest);

	for (iCnt = 0; iCnt <= MAX_DEVICES; iCnt++) {
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

	g_connectMode_usb = TRUE;//add by yunying shang 2017-11-10 for BMS 1381

	if (glDrv._OpenDevice() == FALSE)//#bms1005
	{
		return 0;		
	}
	return 1;
}

USBAPI_API void __stdcall SetConnectionMode(
	const wchar_t* deviceName, BOOL isUsb)
{
	_tcscpy_s(g_ipAddress, 256, deviceName);
	g_connectMode_usb = isUsb;
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

//********************************************************************************************
//Do Calibration //Devid added 2017/10/30
#include "ImgFile\EdgeDetect.h"

SC_PAR_DATA_T k_scan_par;
U8 SCAN_DOC_SIZE = DOC_SIZE_FULL;


static IMG_FILE_T g_ImgFile[2] = { {(unsigned int)&k_scan_par.img,0 },{ (unsigned int)&k_scan_par.img,0 } };
static int bFiling[2] = { 0, 0 };
static SC_INFO_DATA_T Info;
static U8 ScanBuf[0x80000];  // 512MB(?) //Park:512KB	//For 存放 A or B 面一次 bulk 傳來的區段影像
static int ScanBufSize = sizeof(ScanBuf);
static char ImgFileName[64];

int K_BatchNum = 0;
int K_PageNum = 0;
int bSaveFile = 1;

U16 *K_img[2];  // pointer to K_img_buf image buffer
U8 K_img_buf[2][0x3200000];		//2 x 50 MB for prefeed/postfeed scan buf 
U32 K_img_size[2];	//Currently K image data size
U8 K_shad16_data[2][184 * 1024]; // 2 x 184KB  // 1200dpi one line (13" max)
U8 K_shad_data[2][184 * 1024];
#define K_LINES 48
U32 K_lines;

CALIBRATION_CAP_T K_Cap;
CALIBRATION_SET_T K_Set;

#define CALIBRATION_16BIT_DARK

//-------- Calibration target define------------
//--------AFE offset cal---------
#define CAL_AFE_DARK		(6 * 0x100)	//color level	//20170220
#define CAL_AFE_DARK_THD	(2 * 0x100)	//color level
#define CAL_AFE_DARK_ABORT	60	//loop counts

//--------LED exposure time cal---------
#define CAL_EXP_WHITE		(175 * 0x100)	//color level
#define CAL_EXP_THD			(5  * 0x100)	//color level
#define CAL_EXP_ABORT		60	//loop counts
#define CAL_EXP_MINUS_C		15	//Pixel count
#define CAL_EXP_MINUS_G		5


#define CAL_AFE_WHITE		(175 * 0x100)	//color level

#define DARK_DROP			IMG_K_PRUN_300_DOT_Y/4		
#define WHITE_DROP			IMG_K_PRUN_300_DOT_Y/4	

#define CAL_SHADING_WHITE_A_R	(200 * 0x100)
#define CAL_SHADING_WHITE_A_G	(209 * 0x100)
#define CAL_SHADING_WHITE_A_B	(210 * 0x100)

#define CAL_SHADING_WHITE_B_R	(200 * 0x100)
#define CAL_SHADING_WHITE_B_G	(209 * 0x100)
#define CAL_SHADING_WHITE_B_B	(210 * 0x100)

U8 AFE_OFFSET_ABORT = 0;
U8 EXP_ABORT = 0;

char IniFile[256], Profile[64];

SC_PAR_DATA_T sc_par = { SCAN_SOURCE, SCAN_ACQUIRE, SCAN_OPTION, SCAN_DUPLEX, SCAN_PAGE,
{ IMG_FORMAT, IMG_OPTION, IMG_BIT, IMG_MONO, { IMG_DPI_X, IMG_DPI_Y },{ IMG_ORG_X, IMG_ORG_Y }, IMG_WIDTH, IMG_HEIGHT },
{ { 0 },{ 0 } },
0, 0, 0, 0, 0, 0, 0, 0 };

int WritePrivateProfileInt(LPCSTR   lpAppName, LPCSTR   lpKeyName, INT   Value, LPCSTR   lpFileName)
{
	CHAR   ValBuf[16];

	StringCbPrintfA(ValBuf, sizeof(ValBuf), "%i", Value);

	return(WritePrivateProfileStringA(lpAppName, lpKeyName, ValBuf, lpFileName));
}



U32 write_to_ini(void)
{

	GetModuleFileNameA(NULL, IniFile, sizeof(IniFile));
	strcpy(strrchr(IniFile, '.') + 1, "ini");

	WritePrivateProfileStringA("OPTION", "SOURCE", (LPSTR)(sc_par.source == I3('FLB') ? "FLB" : "ADF"), IniFile);
	WritePrivateProfileStringA("OPTION", "FORMAT", (LPSTR)(sc_par.img.format == I3('JPG') ? "JPG" : "TIF"), IniFile);
	WritePrivateProfileInt("OPTION", "COLOR_MODE", sc_par.img.mono, IniFile);
	WritePrivateProfileInt("OPTION", "PIXEL_DEPTH", sc_par.img.bit, IniFile);
	WritePrivateProfileInt("OPTION", "MAIN_SOLU", sc_par.img.dpi.x, IniFile);
	WritePrivateProfileInt("OPTION", "SUB_SOLU", sc_par.img.dpi.y, IniFile);
	WritePrivateProfileInt("OPTION", "SCAN_SIDE", sc_par.duplex, IniFile);
	if ((SCAN_DOC_SIZE != DOC_S_PRNU) && (SCAN_DOC_SIZE != DOC_K_PRNU))
		WritePrivateProfileInt("OPTION", "SCAN_SIZE", SCAN_DOC_SIZE, IniFile);


	return TRUE;
}

U32 read_from_ini(void)
{
	GetModuleFileNameA(NULL, IniFile, sizeof(IniFile));
	strcpy(strrchr(IniFile, '.') + 1, "ini");

	GetPrivateProfileStringA("OPTION", "SOURCE", "FLB", (LPSTR)&sc_par.source, 4, IniFile);
	//sc_par.acquire = 0;
	//if(GetPrivateProfileInt(Profile, "SCAN_TEST_PATTERN", 0, IniFile))
	//  par->acquire |= ACQ_TEST_PATTERN;
	//par->option = 0;
	//par->duplex = GetPrivateProfileInt(Profile, "SCAN_DUPLEX", 1, IniFile);
	//par->page = GetPrivateProfileInt(Profile, "SCAN_PAGE", 0, IniFile);

	GetPrivateProfileStringA("OPTION", "FORMAT", "JPG", (LPSTR)&sc_par.img.format, 4, IniFile);
	if (sc_par.img.format != I3('JPG'))
		sc_par.img.format = I3('RAW');

	sc_par.img.mono = GetPrivateProfileIntA("OPTION", "COLOR_MODE", IMG_MONO, IniFile);
	sc_par.img.bit = GetPrivateProfileIntA("OPTION", "PIXEL_DEPTH", IMG_BIT, IniFile);
	sc_par.img.dpi.x = GetPrivateProfileIntA("OPTION", "MAIN_SOLU", IMG_DPI_X, IniFile);
	sc_par.img.dpi.y = GetPrivateProfileIntA("OPTION", "SUB_SOLU", IMG_DPI_Y, IniFile);
	SCAN_DOC_SIZE = GetPrivateProfileIntA("OPTION", "SCAN_SIZE", DOC_SIZE_FULL, IniFile);
	sc_par.duplex = GetPrivateProfileIntA("OPTION", "SCAN_SIDE", SCAN_AB_SIDE, IniFile);
	return TRUE;
}

int Load_CalibrationParameter(SC_PAR_DATA_T *par)
{
	IMAGE_T *img = (IMAGE_T *)&par->img;

	/***************************IMAGE_T**********************************/
	par->acq_opt = 0;

	if (img->format == I3('JPG')) {
		img->option = IMG_OPT_JPG_FMT444;
	}


	switch (SCAN_DOC_SIZE) {
	case DOC_SIZE_FULL:
		img->width = IMG_300_DOT_X * img->dpi.x / 300;
		img->org.x = IMG_300_ORG_X * img->dpi.x / 300;

		img->height = IMG_300_DOT_Y *img->dpi.y / 300;
		break;

	case DOC_SIZE_A4:
		img->width = IMG_A4_300_DOT_X * img->dpi.x / 300;
		img->org.x = IMG_A4_300_ORG_X * img->dpi.x / 300;

		img->height = IMG_A4_300_DOT_Y *img->dpi.y / 300;
		break;

	case DOC_SIZE_LT:
		img->width = IMG_LT_300_DOT_X * img->dpi.x / 300;
		img->org.x = IMG_LT_300_ORG_X * img->dpi.x / 300;

		img->height = IMG_LT_300_DOT_Y *img->dpi.y / 300;
		break;

	case DOC_SIZE_LL:
		img->width = IMG_LL_300_DOT_X * img->dpi.x / 300;
		img->org.x = IMG_LL_300_ORG_X * img->dpi.x / 300;

		img->height = IMG_LL_300_DOT_Y *img->dpi.y / 300;
		break;

	case DOC_FB_LIFE:
		img->width = IMG_FB_LIFE_300_DOT_X * img->dpi.x / 300;
		img->org.x = 0 * img->dpi.x / 300;

		img->height = IMG_FB_LIFE_300_DOT_Y *img->dpi.y / 300;
		break;

	case DOC_S_PRNU:
		img->width = IMG_S_PRNU_300_DOT_X * img->dpi.x / 300;
		img->org.x = 0 * img->dpi.x / 300;

		img->height = IMG_S_PRUN_300_DOT_Y *img->dpi.y / 300;
		break;

	case DOC_K_PRNU:
		img->width = IMG_K_PRNU_300_DOT_X * img->dpi.x / 300;
		img->org.x = 0 * img->dpi.x / 300;

		img->height = IMG_K_PRUN_300_DOT_Y *img->dpi.y / 300;
		//img->height = IMG_K_PRUN_300_DOT_Y *300/img->dpi.y; //Image buf issue
		break;

	case DOC_K_PREFEED:
		img->width = IMG_K_PREFEED_300_DOT_X * img->dpi.x / 300;
		img->org.x = 0 * img->dpi.x / 300;

		img->height = IMG_K_PREFEED_300_DOT_Y *img->dpi.y / 300;
		break;
	}
	img->width -= (img->width % 8);
	img->height -= (img->height % 8);



	/***************************MTR_T**********************************/
	if (par->acquire & ACQ_SET_MTR) {

		if (par->source == I3('FLB')) {
			/*FB scan*/
			//par->mtr[0].drive_target = MT_PH;
			//par->mtr[0].state_mechine = SCAN_STATE_MECHINE;
			switch (par->img.dpi.x) {
			case 300:
				par->mtr[0].speed_pps = GetPrivateProfileIntA("FB300x300M", "PPS", 0, IniFile);
				par->mtr[0].direction = GetPrivateProfileIntA("FB300x300M", "DIR", 0, IniFile);
				par->mtr[0].micro_step = GetPrivateProfileIntA("FB300x300M", "MS", 0, IniFile);
				par->mtr[0].currentLV = GetPrivateProfileIntA("FB300x300M", "CLV", 0, IniFile);
				break;
			case 600:
				par->mtr[0].speed_pps = GetPrivateProfileIntA("FB600x600", "PPS", 0, IniFile);
				par->mtr[0].direction = GetPrivateProfileIntA("FB600x600", "DIR", 0, IniFile);
				par->mtr[0].micro_step = GetPrivateProfileIntA("FB600x600", "MS", 0, IniFile);
				par->mtr[0].currentLV = GetPrivateProfileIntA("FB600x600", "CLV", 0, IniFile);
				break;
			}
		}
		else {
			/*ADF scan*/
			par->mtr[1].pick_ss_step = GetPrivateProfileIntA("PICK_SS_STEP", "STEP", 0, IniFile);

			//par->mtr[0].drive_target = CMT_PH;
			//par->mtr[0].state_mechine = SCAN_STATE_MECHINE;
			//par->mtr[1].drive_target = BMT_PH;
			//par->mtr[1].state_mechine = STATE_MECHINE_1;
			if (par->img.bit < 24) {
				/*Mono scan*/
				par->mtr[1].speed_pps = GetPrivateProfileIntA("ADFgray", "PICK_PPS", 0, IniFile);
				par->mtr[1].direction = GetPrivateProfileIntA("ADFgray", "PICK_DIR", 0, IniFile);
				par->mtr[1].micro_step = GetPrivateProfileIntA("ADFgray", "PICK_MS", 0, IniFile);
				par->mtr[1].currentLV = GetPrivateProfileIntA("ADFgray", "PICK_CLV", 0, IniFile);

				par->mtr[0].speed_pps = GetPrivateProfileIntA("ADFgray", "FEED_PPS", 0, IniFile);
				par->mtr[0].direction = GetPrivateProfileIntA("ADFgray", "FEED_DIR", 0, IniFile);
				par->mtr[0].micro_step = GetPrivateProfileIntA("ADFgray", "FEED_MS", 0, IniFile);
				par->mtr[0].currentLV = GetPrivateProfileIntA("ADFgray", "FEED_CLV", 0, IniFile);
			}
			else {
				/*Color scan*/
				if ((par->img.dpi.x == 300) && (par->img.dpi.y == 300)) {
					/*300x300DPI scan*/
					par->mtr[1].speed_pps = GetPrivateProfileIntA("ADF300x300color", "PICK_PPS", 0, IniFile);
					par->mtr[1].direction = GetPrivateProfileIntA("ADF300x300color", "PICK_DIR", 0, IniFile);
					par->mtr[1].micro_step = GetPrivateProfileIntA("ADF300x300color", "PICK_MS", 0, IniFile);
					par->mtr[1].currentLV = GetPrivateProfileIntA("ADF300x300color", "PICK_CLV", 0, IniFile);

					par->mtr[0].speed_pps = GetPrivateProfileIntA("ADF300x300color", "FEED_PPS", 0, IniFile);
					par->mtr[0].direction = GetPrivateProfileIntA("ADF300x300color", "FEED_DIR", 0, IniFile);
					par->mtr[0].micro_step = GetPrivateProfileIntA("ADF300x300color", "FEED_MS", 0, IniFile);
					par->mtr[0].currentLV = GetPrivateProfileIntA("ADF300x300color", "FEED_CLV", 0, IniFile);
				}
				else if ((par->img.dpi.x == 300) && (par->img.dpi.y == 600)) {
					/*300x600DPI scan*/
					par->mtr[1].speed_pps = GetPrivateProfileIntA("ADF300x600color", "PICK_PPS", 0, IniFile);
					par->mtr[1].direction = GetPrivateProfileIntA("ADF300x600color", "PICK_DIR", 0, IniFile);
					par->mtr[1].micro_step = GetPrivateProfileIntA("ADF300x600color", "PICK_MS", 0, IniFile);
					par->mtr[1].currentLV = GetPrivateProfileIntA("ADF300x600color", "PICK_CLV", 0, IniFile);

					par->mtr[0].speed_pps = GetPrivateProfileIntA("ADF300x600color", "FEED_PPS", 0, IniFile);
					par->mtr[0].direction = GetPrivateProfileIntA("ADF300x600color", "FEED_DIR", 0, IniFile);
					par->mtr[0].micro_step = GetPrivateProfileIntA("ADF300x600color", "FEED_MS", 0, IniFile);
					par->mtr[0].currentLV = GetPrivateProfileIntA("ADF300x600color", "FEED_CLV", 0, IniFile);
				}
				else {
					/*600DPI scan*/
					par->mtr[1].speed_pps = GetPrivateProfileIntA("ADF600x600color", "PICK_PPS", 0, IniFile);
					par->mtr[1].direction = GetPrivateProfileIntA("ADF600x600color", "PICK_DIR", 0, IniFile);
					par->mtr[1].micro_step = GetPrivateProfileIntA("ADF600x600color", "PICK_MS", 0, IniFile);
					par->mtr[1].currentLV = GetPrivateProfileIntA("ADF600x600color", "PICK_CLV", 0, IniFile);

					par->mtr[0].speed_pps = GetPrivateProfileIntA("ADF600x600color", "FEED_PPS", 0, IniFile);
					par->mtr[0].direction = GetPrivateProfileIntA("ADF600x600color", "FEED_DIR", 0, IniFile);
					par->mtr[0].micro_step = GetPrivateProfileIntA("ADF600x600color", "FEED_MS", 0, IniFile);
					par->mtr[0].currentLV = GetPrivateProfileIntA("ADF600x600color", "FEED_CLV", 0, IniFile);
				}

			}
		}

		switch (par->mtr[0].currentLV) {
		case 1:
			par->mtr[0].currentLV = 0;
			break;
		case 2:
			par->mtr[0].currentLV = 2;
			break;
		case 3:
			par->mtr[0].currentLV = 1;
			break;
		case 4:
			par->mtr[0].currentLV = 3;
			break;
		}

		switch (par->mtr[1].currentLV) {
		case 1:
			par->mtr[1].currentLV = 0;
			break;
		case 2:
			par->mtr[1].currentLV = 2;
			break;
		case 3:
			par->mtr[1].currentLV = 1;
			break;
		case 4:
			par->mtr[1].currentLV = 3;
			break;
		}

	}
	/***************************ME_T**********************************/
	if (par->acquire & ACQ_SET_ME) {
		par->leading_edge = GetPrivateProfileIntA("FB_LEADING", "FB_leading", 0, IniFile);
		par->img_gap = GetPrivateProfileIntA("CIS_GAP", "cis_gap", 0, IniFile);
		par->prefed = GetPrivateProfileIntA("PREFEED", "prefeed", 0, IniFile);
		par->postfed = GetPrivateProfileIntA("POSTFEED", "postfeed", 0, IniFile);
		par->side_edgeA = GetPrivateProfileIntA("START_PIXEL_A", "start_pixel_A", 0, IniFile);
		par->side_edgeB = GetPrivateProfileIntA("START_PIXEL_B", "start_pixel_B", 0, IniFile);
	}


	write_to_ini();

	return TRUE;
}

U32 user_param(U32 action)
{
	k_scan_par.acquire = action;
	Load_CalibrationParameter(&k_scan_par);

	return TRUE;
}

int Scan_Param(void)
{
	int result;

	result = g_pointer_lDrv->_parameters();

	return result;
}

void Save_LED_AFE(U8 data_type, SC_PAR_DATA_T *par, U32 *data, U8 dup)
{
	FILE *fcsv;
	char fcsvName[80];

	sprintf(fcsvName, "%c%c%d%c_led_afe.csv", (U8)par->source, (par->img.bit >= 24) ? 'C' : 'G', par->img.dpi.x, 'A' + dup);
	fcsv = fopen(fcsvName, "a");
	//fcsv = fopen(fcsvName, "wb");

	if (!fcsv)
		printf("can't open file shading csv!!\n");
	else {
		switch (data_type) {
		case 1:
			//Save LED data
			fprintf(fcsv, "led_r, led_g, led_b\n");
			fprintf(fcsv, "%d, %d, %d\n", data[0], data[1], data[2]);
			break;

		case 2:
			//Save AFE offset data
		{
			S16 *tmp_data = (S16*)data;
			fprintf(fcsv, "afe_offset_1, afe_offset_2, afe_offset_3, afe_offset_4, afe_offset_5, afe_offset_6\n");
			fprintf(fcsv, "%d, %d, %d, %d, %d, %d\n", tmp_data[0], tmp_data[1], tmp_data[2], tmp_data[3], tmp_data[4], tmp_data[5]);
		}
		break;

		case 3:
			//Save AFE gain data
		{
			U16 *tmp_data = (U16*)data;
			fprintf(fcsv, "afe_gain_1, afe_gain_2, afe_gain_3, afe_gain_4, afe_gain_5, afe_gain_6\n");
			fprintf(fcsv, "%d, %d, %d, %d, %d, %d\n", tmp_data[0], tmp_data[1], tmp_data[2], tmp_data[3], tmp_data[4], tmp_data[5]);
		}
		break;
		}

		fclose(fcsv);
	}
}

void Save_Shading(SC_PAR_DATA_T *par, U16 *img_buf, U32 *shd_buf, U32 gain, U8 dup)
{
	FILE *fcsv;
	char fcsvName[80];
	U32 i, dot_x;
	U8 color_loop = (par->img.mono == IMG_COLOR) ? 3 : 1;

	dot_x = par->img.width;

	sprintf(fcsvName, "%c%c%d%c_shd_%s.csv", (U8)par->source, (par->img.bit >= 24) ? 'C' : 'G', par->img.dpi.x, 'A' + dup, (par->acquire & ACQ_LAMP_OFF) ? "offset" : "gain");
	fcsv = fopen(fcsvName, "wb");

	if (!fcsv)
		printf("can't open file shading csv!!\n");
	else {
		if (par->acquire & ACQ_LAMP_OFF) {
			/*Offset*/
			fprintf(fcsv, "dr, dg, db, offset_r, offset_g, offset_b\n");
			for (i = 0;i<dot_x;i++) {
				if (par->img.mono == IMG_COLOR) {
					//Color
					fprintf(fcsv, "%d, %d, %d, %d, %d, %d\n",
						img_buf[i * 3], img_buf[i * 3 + 1], img_buf[i * 3 + 2],
						shd_buf[i] - gain, shd_buf[i + dot_x] - gain, shd_buf[i + dot_x * 2] - gain
						);
				}
				else {
					//Mono
					fprintf(fcsv, "%d, %d\n", img_buf[i], shd_buf[i] - gain);
				}
			}
		}
		else {
			/*Gain*/
			fprintf(fcsv, "wr, wg, wb, gainr, gaing, gainb\n");
			for (i = 0;i<dot_x;i++) {
				if (par->img.mono == IMG_COLOR) {
					//Color
					fprintf(fcsv, "%d, %d, %d, %f, %f, %f\n",
						img_buf[i * 3], img_buf[i * 3 + 1], img_buf[i * 3 + 2],
						((float)(shd_buf[i] >> 16) / ((float)gain)),
						((float)(shd_buf[i + dot_x] >> 16) / (float)gain),
						((float)(shd_buf[i + dot_x * 2] >> 16) / (float)gain)
						);
				}
				else {
					//Mono
					fprintf(fcsv, "%d, %f\n", img_buf[i], ((float)(shd_buf[i] >> 16) / (float)gain));
				}
			}
		}


		fclose(fcsv);
	}
}


int cal_img_buf_store(int dup, void *img, int size)
{
	if (img) {
		int buf_size = sizeof(K_img_buf) / 2 - ((U32)K_img[dup] - (U32)K_img_buf[dup]);
		if (size > buf_size) {
			printf("Calibration buffer too small.\n");
			return FALSE;
		}
		memcpy(K_img[dup], img, size);
		K_img[dup] += (size / sizeof(U16));
		K_img_size[dup] += size;
	}
	else {  // reset memory pointer
		K_img[0] = (U16*)K_img_buf[0];
		K_img[1] = (U16*)K_img_buf[1];
		K_img_size[0] = 0;
		K_img_size[1] = 0;
	}
	return TRUE;
}

__inline U16 _cal_average_data(U16 *data, int next, int num)
{
	U16 *last_data = data + next*num;
	U32 sum;
	for (sum = 0; data < last_data; data += next)
		sum += *data;
	return (U16)(sum / num);
}

void _cal_average_iterate(U16 *data, int num_x, int num_y)
{
	U16 *last_data;
	for (last_data = data + num_x; data < last_data; data++)
		*data = _cal_average_data(data, num_x, num_y);
}

/*find min of *data*/
U16 _cal_find_min(U16 *data, int next, int num)
{
	U16 *last_data = data + next*num;
	U16 min;
	for (min = 0xffff; data < last_data; data += next) {
		if (min > *data)
			min = *data;
	}
	return min;
}

U16 _cal_find_max(U16 *data, int next, int num)
{
	U16 *last_data = data + next*num;
	U16 max;
	for (max = 0; data < last_data; data += next) {
		if (max < *data)
			max = *data;
	}
	return max;
}

void _cal_check_offset(S16 *offset, int channel, int max, int min)
{
	int i;
	for (i = 0; i < channel; i++) {
		if (offset[i] > max)
			offset[i] = max;
		else if (offset[i] < min)
			offset[i] = min;
	}
}

int Scan_EnableSaveFile(int enable)
{
	int old_setting = bSaveFile;
	bSaveFile = enable ? 1 : 0;
	return old_setting;
}

int Scan_CloseFile(int dup, int lines, int width)
{
	if (bSaveFile)
		return ImgFile_Close(&g_ImgFile[dup], lines);
	return TRUE;
}

int Scan_OpenFile(int dup, char *filename)
{
	if (bSaveFile)
		return ImgFile_Open(&g_ImgFile[dup], filename);
	return TRUE;
}

int Scan_WriteFile(int dup, char*buf, int length)
{
	if (bSaveFile)
		return ImgFile_Write(&g_ImgFile[dup], buf, length);
	return TRUE;
}

#define JOB_WAIT_TIMEOUT  5000
int job_Wait(int job, int wait_motor_stop)
{
	U32 tick = GetTickCount();
	while ((GetTickCount() - tick) < JOB_WAIT_TIMEOUT) {
		if (!g_pointer_lDrv->_info())
			break;
		if (!(Info.JobState & job) && (!wait_motor_stop || !Info.MotorMove))
			return TRUE;
		Sleep(100);
	}
	return FALSE;
}

int _scan_start()
{
	g_ImgFile[0].stream = g_ImgFile[1].stream = 0;
	return g_pointer_lDrv->_StartScan();
}

int _scan_info()
{

	if (!g_pointer_lDrv->_info())
		return -1;


	if ((!(k_scan_par.duplex & 1) || Info.EndScan[0]) &&
		(!(k_scan_par.duplex & 2) || Info.EndScan[1]))
		return -1;

	return 1;
}

int _scan_image(void)
{
	ACQUIRE_T *acq = (ACQUIRE_T *)&k_scan_par;
	IMAGE_T *img = &k_scan_par.img;
	int dup, length, PageStart, PageEnd;
	//ScanBufSize=((img->format==I3('JPG')) && !(img->option & IMG_OPT_JPG_PAGE))? sizeof(ScanBuf)/8: sizeof(ScanBuf);
	int lineSize = (img->format != I3('JPG')) ? ((img->bit * img->width + 7) / 8) : 0;
	ScanBufSize = sizeof(ScanBuf);
	for (dup = 0; dup < 2; dup++) {
		if (!(acq->duplex & (1 << dup)) || !Info.ValidPageSize[dup])
			continue;
		length = min(ScanBufSize, (int)Info.ValidPageSize[dup]);
		if (lineSize)
			length -= (length % lineSize);
		if(g_pointer_lDrv->_ReadImageEX(dup, &length, ScanBuf, length)){
		//if (Scan_Img(dup, &length) && Scan_Read(ScanBuf, length)) {
			//cal_img_buf_store(dup, ScanBuf, length);  // Jason debug

			PageStart = 0, PageEnd = 0;
			if (!bFiling[dup]) {
				bFiling[dup]++;
				sprintf(ImgFileName, "%02d_%c%c%d_%02d%c.%s", K_BatchNum, (U8)acq->source, (img->bit >= 24) ? 'C' : 'G', img->dpi.x, /*Info.PageNum[0]+1*/K_PageNum, 'A' + dup, img->format == I3('JPG') ? "JPG" : "TIF");
				Scan_OpenFile(dup, ImgFileName);
				PageStart++;
			}
			Scan_WriteFile(dup, (char*)ScanBuf, length);
			if ((length >= (int)Info.ValidPageSize[dup]) && Info.EndPage[dup]) {
				//printf("File close ");
				//printf("%c\n", dup? 'B': 'A');
				Scan_CloseFile(dup, Info.ImageHeight[dup], 0);
				PageEnd = Info.ImageHeight[dup];
				bFiling[dup]--;
			}

			cal_img_buf_store(dup, ScanBuf, length);  // Jason debug
		}
	}
	return TRUE;
}

int _scan_stop()
{
	if (g_ImgFile[0].stream)
		Scan_CloseFile(1, Info.ImageHeight[0], 0);
	if (g_ImgFile[1].stream)
		Scan_CloseFile(2, Info.ImageHeight[1], 0);
	return g_pointer_lDrv->_stop();
}

int job_Scan(void)
{
	int data_ready;
	if (!_scan_start())
		goto EXIT;
LOOP:
	data_ready = _scan_info();
	if (data_ready < 0)
		goto EXIT;
	if (data_ready == 0)
		goto LOOP;
	_scan_image();
	goto LOOP;
EXIT:
	return _scan_stop();
}

int cal_set_def(CALIBRATION_CAP_T *cap, CALIBRATION_SET_T *set)
{
	int i;
	U32 *exp;
	S16 *offset;
	U16 *gain;
	U32 exp_def;
	S16 offset_def;
	U16 gain_def;

	user_param(ACQ_CALIBRATION | ACQ_MOTOR_OFF | ACQ_NO_MIRROR | ACQ_NO_SHADING);  //Park test

	if (!Scan_Param())
		return FALSE;
	if (!g_pointer_lDrv->_Scan_Cap_Calibration(cap))
		return FALSE;

	//memset(set, 0, sizeof(CALIBRATION_SET_T));
	//memset(set, 0, (sizeof(CALIBRATION_SET_T) - sizeof(ME_SET_T))); //Not to write ME par
	for (i = 0; i < 2; i++) {
		//exp_def = cap->ccd[i].exp_max; //cap->ccd[i].exp_def;
		exp_def = cap->ccd[i].exp_def;
		exp = set->ccd[i].exp;
		exp[0] = exp[1] = exp[2] = exp_def;
		//offset_def = cap->afe[i].offset_max/2; //cap->afe[i].offset_def;
		offset_def = cap->afe[i].offset_def;
		offset = set->afe[i].offset;
		offset[0] = offset[1] = offset[2] = offset[3] = offset[4] = offset[5] = offset_def;
		//gain_def = cap->afe[i].gain_min; //cap->afe[i].gain_def;
		gain_def = cap->afe[i].gain_def;
		if (gain_def < 1000)
			gain_def = 1000;
		gain = set->afe[i].gain;
		gain[0] = gain[1] = gain[2] = gain[3] = gain[4] = gain[5] = gain_def;
		set->shd[i].mono = (k_scan_par.img.mono ? (cap->ccd[i].mono) : 0);
	}

	return TRUE;
}

int cal_prefeed(CALIBRATION_CAP_T *cap, CALIBRATION_SET_T *set)
{
	U8 i = 0;
	U8 TMP_DOC_SIZE = 0;
	SC_PAR_DATA_T tmp_scan_par;
	U16 *buf;
	int leadingEdge = 0, leftEdge = 0, rightEdge = 0, isSideB = 0;

	TMP_DOC_SIZE = SCAN_DOC_SIZE;
	memcpy(&tmp_scan_par, &k_scan_par, sizeof(SC_PAR_DATA_T));


	SCAN_DOC_SIZE = DOC_K_PREFEED;

	k_scan_par.source = I3('ADF');
	k_scan_par.duplex = 3;
	k_scan_par.img.format = I3('RAW');
	k_scan_par.img.bit = IMG_24_BIT;
	k_scan_par.img.mono = 0;
	k_scan_par.img.dpi.x = 300;
	k_scan_par.img.dpi.y = 300;


	cal_set_def(cap, set);

	for (i = 0; i < 2; i++) {
		set->ccd[i].exp[0] = set->ccd[i].exp[1] = set->ccd[i].exp[2] = cap->ccd[i].exp_max;
		set->afe[i].gain[0] = set->afe[i].gain[1] = set->afe[i].gain[2] = set->afe[i].gain[3] = set->afe[i].gain[4] = set->afe[i].gain[5] = cap->afe[i].gain_max;
	}

	set->me.prefeed = cap->me.prefeed;

	user_param(ACQ_CALIBRATION | ACQ_NO_MIRROR | ACQ_NO_SHADING);
	if (Scan_Param() != 0)
		return FALSE;

	cal_img_buf_store(0, 0, 0);
	if (!g_pointer_lDrv->_Scan_Shad_Calibration(set) || !job_Scan() || !job_Wait(JOB_SCAN, 1))
		return FALSE;

	buf = (U16*)K_img_buf[0]; //Calculate for side A

	EdgeDetectColor8((unsigned char*)buf, IMG_K_PREFEED_300_DOT_X, IMG_K_PREFEED_300_DOT_Y, &leadingEdge, &leftEdge, &rightEdge, isSideB);

	printf("leadingEdge = %d\n", leadingEdge);
	set->me.prefeed = 100 * leadingEdge / 300;

	SCAN_DOC_SIZE = TMP_DOC_SIZE;
	memcpy(&k_scan_par, &tmp_scan_par, sizeof(SC_PAR_DATA_T));

	for (i = 0; i < 2; i++) {
		set->ccd[i].exp[0] = set->ccd[i].exp[1] = set->ccd[i].exp[2] = cap->ccd[i].exp_def;
		set->afe[i].gain[0] = set->afe[i].gain[1] = set->afe[i].gain[2] = set->afe[i].gain[3] = set->afe[i].gain[4] = set->afe[i].gain[5] = cap->afe[i].gain_def;
	}

	return TRUE;
}

int cal_postfeed(CALIBRATION_CAP_T *cap, CALIBRATION_SET_T *set)
{
	int i, j;
	U16 *shad_data;
	U32 dot, dark_digit;
	U16 *buf;
	int trailingEdge = 0, isSideB = 0;

	U8 TMP_DOC_SIZE = 0;
	SC_PAR_DATA_T tmp_scan_par;

	TMP_DOC_SIZE = SCAN_DOC_SIZE;
	memcpy(&tmp_scan_par, &k_scan_par, sizeof(SC_PAR_DATA_T));


	SCAN_DOC_SIZE = DOC_SIZE_FULL;

	k_scan_par.source = I3('ADF');
	k_scan_par.duplex = 3;
	k_scan_par.img.format = I3('RAW');
	k_scan_par.img.bit = IMG_24_BIT;
	k_scan_par.img.mono = 0;
	k_scan_par.img.dpi.x = 300;
	k_scan_par.img.dpi.y = 300;

	cal_set_def(cap, set);

	for (i = 0; i < 2; i++) {
		set->ccd[i].exp[0] = set->ccd[i].exp[1] = set->ccd[i].exp[2] = cap->ccd[i].exp_max;
		set->afe[i].gain[0] = set->afe[i].gain[1] = set->afe[i].gain[2] = set->afe[i].gain[3] = set->afe[i].gain[4] = set->afe[i].gain[5] = cap->afe[i].gain_max;
	}

	set->me.postfeed = cap->me.postfeed;

	user_param(ACQ_NO_MIRROR | ACQ_NO_SHADING);

	if (!Scan_Param())  //Park test
		return FALSE;

	cal_img_buf_store(0, 0, 0);
	if (!g_pointer_lDrv->_Scan_Shad_Calibration(set) || !job_Scan() || !job_Wait(JOB_SCAN, 1))
		return FALSE;

	buf = (U16*)K_img_buf[0]; //Calculate for side A

							  //printf("K_img_size[0] = %d\n", K_img_size[0]);
							  //printf("K_img_size[0]/3/IMG_K_PREFEED_300_DOT_X = %d\n", K_img_size[0]/3/IMG_K_PREFEED_300_DOT_X);

	EdgeDetectColor8Trailing((unsigned char*)buf, IMG_K_PREFEED_300_DOT_X, K_img_size[0] / 3 / IMG_K_PREFEED_300_DOT_X, &trailingEdge, isSideB);

	//printf("trailingEdge = %d\n", trailingEdge);

	set->me.postfeed = cap->me.postfeed - 100 * trailingEdge / 300;

	SCAN_DOC_SIZE = TMP_DOC_SIZE;
	memcpy(&k_scan_par, &tmp_scan_par, sizeof(SC_PAR_DATA_T));

	return TRUE;
}

void _cal_check_gain(U16 *gain, int channel, int max, int min)
{
	int i;
	for (i = 0; i < channel; i++) {
		if (gain[i] > max)
			gain[i] = max;
		else if (gain[i] < min)
			gain[i] = min;
	}
}

int cal_AFE_gain(CALIBRATION_CAP_T *cap, CALIBRATION_SET_T *set)
{
	int i, j, seg;
	int color_loop = (k_scan_par.img.mono == 4) ? 1 : 3;
	U32 dot, seg_dot;
	U16 *buf, *gain;
	float gain_cl = 0;

	user_param(ACQ_CALIBRATION | ACQ_MOTOR_OFF | ACQ_NO_PP_SENSOR | ACQ_NO_MIRROR | ACQ_NO_SHADING);  //Park test
	if (!Scan_Param())
		return FALSE;

	cal_img_buf_store(0, 0, 0);
	if (!g_pointer_lDrv->_Scan_Shad_Calibration(set) || !job_Scan() || !job_Wait(JOB_SCAN, 1))
		return FALSE;

	for (i = 0; i < 2; i++) {
		seg = (cap->ccd[i].type == I4('CIS6')) ? 6 : ((cap->ccd[i].type == I4('CIS3')) ? 3 : 1);
		buf = (U16*)K_img_buf[i];
		dot = cap->ccd[i].dot;
		seg_dot = dot / seg;
		gain = set->afe[i].gain;
		_cal_average_iterate(buf, dot * color_loop, k_scan_par.img.height);
		if (seg > 1) {
			U32 new_gain = 0;
			for (j = 0; j < seg; j++) {

				//gain_cl = (float)CAL_AFE_WHITE/_cal_average_data(&buf[seg_dot*color_loop*j], 1, seg_dot*color_loop);
				gain[j] = gain[j] * ((float)CAL_AFE_WHITE / _cal_average_data(&buf[seg_dot*color_loop*j], 1, seg_dot*color_loop));
			}
		}
		else {
			for (j = 0; j < color_loop; j++)
				gain[j] = gain[j] * CAL_AFE_WHITE / _cal_average_data(&buf[j], color_loop, dot);
			for (; j < 3; j++)
				gain[j] = gain[0];
		}
		_cal_check_gain(gain, j, cap->afe[i].gain_max, cap->afe[i].gain_min);

		Save_LED_AFE(3, &k_scan_par, (unsigned int*)gain, i);
	}

	if (/*ini_get_shading_dump()*/1) {
		Scan_Param();
		cal_img_buf_store(0, 0, 0);
		if (!g_pointer_lDrv->_Scan_Shad_Calibration(set) || !job_Scan() || !job_Wait(JOB_SCAN, 1))
			return FALSE;
	}
	return TRUE;
}

int cal_AFE_offset(CALIBRATION_CAP_T *cap, CALIBRATION_SET_T *set)
{
	int i, j, seg;
	int color_loop = (k_scan_par.img.mono == 4) ? 1 : 3;
	U32 dot, seg_dot;
	U16 SEG_AFE_DARK;
	S16 *offset;
	U16 *gain, *buf;

	U8 SIDE_OK[2] = { 0, 0 };
	U8 CYCLE_COUNT = 0;
	U8 TMP_NOT_OK = 0;

	user_param(ACQ_CALIBRATION | ACQ_NO_PP_SENSOR | ACQ_MOTOR_OFF | ACQ_LAMP_OFF | ACQ_NO_MIRROR | ACQ_NO_SHADING);

AFE_OFFSET_CHK:

	if (!Scan_Param())
		return FALSE;

	cal_img_buf_store(0, 0, 0);  // reset image buffer pointer
	if (!g_pointer_lDrv->_Scan_Shad_Calibration(set) || !job_Scan() || !job_Wait(JOB_SCAN, 1))
		return FALSE;

	for (i = 0; i < 2; i++) {

		if (SIDE_OK[i])
			continue;

		seg = (cap->ccd[i].type == I4('CIS6')) ? 6 : ((cap->ccd[i].type == I4('CIS3')) ? 3 : 1);
		buf = (U16*)K_img_buf[i];
		dot = cap->ccd[i].dot;
		seg_dot = dot / seg;
		offset = set->afe[i].offset;
		gain = set->afe[i].gain;
		_cal_average_iterate(buf, dot * color_loop, k_scan_par.img.height);
		if (seg > 1) {
			TMP_NOT_OK = 0;
			for (j = 0; j < seg; j++) {
				SEG_AFE_DARK = _cal_average_data(&buf[seg_dot*color_loop*j], 1, seg_dot*color_loop);
				if ((SEG_AFE_DARK < (CAL_AFE_DARK - CAL_AFE_DARK_THD))
					|| (SEG_AFE_DARK >(CAL_AFE_DARK + CAL_AFE_DARK_THD))) {
					TMP_NOT_OK = 1;
					offset[j] += 1000 * ((S16)CAL_AFE_DARK - SEG_AFE_DARK) / gain[j];

				}
			}
			if (!TMP_NOT_OK) {
				SIDE_OK[i] = 1;
			}
			else {
				CYCLE_COUNT++;
			}

		}
		else {
			for (j = 0; j < color_loop; j++)
				offset[j] -= 1000 * _cal_average_data(&buf[j], color_loop, dot) / gain[j];
			for (; j < 3; j++)
				offset[j] = offset[0] * gain[0] / gain[j];
		}
		_cal_check_offset(offset, j, cap->afe[i].offset_max, cap->afe[i].offset_min);

		Save_LED_AFE(2, &k_scan_par, (unsigned int*)offset, i);
	}

	if ((SIDE_OK[0] & SIDE_OK[1]) || (CYCLE_COUNT == (CAL_AFE_DARK_ABORT + 1))) {
		if (CYCLE_COUNT == (CAL_AFE_DARK_ABORT + 1)) {
			AFE_OFFSET_ABORT = 1;
		}
		goto AFE_OFFSET_OK;
	}
	else {
		goto AFE_OFFSET_CHK;
	}


AFE_OFFSET_OK:

	return TRUE;
}

void _cal_do_shift_dark(U32 *src, U16 *dst, int num, int dark_digit, int dark_shift)
{
	U32 *last_src;
	U32 data;
	U32 gain_mask = (0xffff >> dark_digit) << dark_digit;
	U32 dark_mask = (gain_mask ^ 0xffff) << dark_shift;
	if (dark_digit == 16) {
		if ((U32)src != (U32)dst)
			memcpy(dst, src, num * 4);
	}
	else {
		for (last_src = src + num; src < last_src; src++, dst++) {
			data = *src;
			*dst = (U16)((data & dark_mask) >> dark_shift) | ((data >> 16) & gain_mask);
		}
	}
}

void _cal_construct_dark16(U16 *data, U32 *shad, int next_data, int next_shad, int num, U32 gain)
{
	U16 *last_data = data + next_data*num;
	while (data < last_data) {
		*shad = gain + *data;
		data += next_data;
		shad += next_shad;
	}
}

int cal_dark_shading(CALIBRATION_CAP_T *cap, CALIBRATION_SET_T *set)
{
	int i, j;
	int color_loop = (k_scan_par.img.mono == 4) ? 1 : 3;
	U32 dot;
	U16 *buf, *shad_data/*, dark_min, dark_max*/;
	U32 gain, *dark_buf, dark_shift, dark_digit;
	U8 SIDE_OK[2] = { 0, 0 };

	if (k_scan_par.source == I3('ADF')) { //Park test
		user_param(ACQ_CALIBRATION | ACQ_NO_PP_SENSOR | ACQ_LAMP_OFF | ACQ_MOTOR_OFF | ACQ_NO_MIRROR | ACQ_NO_SHADING);
	}
	else {
		user_param(ACQ_CALIBRATION | ACQ_LAMP_OFF | ACQ_NO_MIRROR | ACQ_NO_SHADING);
	}
	if (!Scan_Param())
		return FALSE;
	cal_img_buf_store(0, 0, 0);

	if (!g_pointer_lDrv->_Scan_Shad_Calibration(set) || !job_Scan() || !job_Wait(JOB_SCAN, 1))
		return FALSE;

	for (i = 0; i < 2; i++) {
		buf = (U16*)K_img_buf[i];
		dot = cap->ccd[i].dot;
		dark_buf = (U32*)K_shad16_data[i];
		set->shd[i].gain_base = 8;  // default digital gain base
		gain = (0x10000 / set->shd[i].gain_base) << 16;
		_cal_average_iterate(buf, dot*color_loop, k_scan_par.img.height);

		for (j = 0; j < color_loop; j++) {
			_cal_construct_dark16(&buf[j], &dark_buf[j*dot], color_loop, 1, dot, gain);
		}

		Save_Shading(&k_scan_par, buf, dark_buf, gain, i);

		dark_shift = set->shd[i].dark_shift = 0;
		dark_digit = set->shd[i].dark_digit = 16;
		_cal_do_shift_dark(dark_buf, (U16*)K_shad_data[i], dot*color_loop, dark_digit, dark_shift);
	}

	Scan_Param();
	cal_img_buf_store(0, 0, 0);
	if (!g_pointer_lDrv->_Scan_Shad_Calibration(set))
		return FALSE;

	for (i = 0; i < 2; i++) {
		dark_digit = set->shd[i].dark_digit;
		dot = cap->ccd[i].dot * ((dark_digit == 16) ? 2 : 1);
		shad_data = (U16*)K_shad_data[i];
		if (k_scan_par.img.mono)
			//Scan_Shad_Shading(i, set->shd[i].mono, shad_data, dot*2);  // GL orginal
			g_pointer_lDrv->_Scan_Shad_Shading(i, 1, shad_data, dot * 2);  //Park test 
		else {
			for (j = 0; j < 3; j++)
				g_pointer_lDrv->_Scan_Shad_Shading(i, j + 1, &shad_data[j*dot], dot * 2);
		}
	}

	if (!job_Scan() || !job_Wait(JOB_SCAN, 1))
		return FALSE;

	return TRUE;
}

void _cal_check_exposure_time(U32 *exp, int color, int max, int min)
{
	int i;
	for (i = 0; i < color; i++) {
		if ((int)exp[i] > max)
			exp[i] = max;
		else if ((int)exp[i] < min)
			exp[i] = min;
	}
}

int cal_exposure_time(CALIBRATION_CAP_T *cap, CALIBRATION_SET_T *set)
{
	int i, j, k;
	int color_loop = (k_scan_par.img.mono == 4) ? 1 : 3;
	U32 *exp, dot;
	U16 white[3], *buf;
	U16 white_min[2] = { 0, 0 };

	U8 SIDE_OK[2] = { 0, 0 };
	U8 CYCLE_COUNT[2] = { 0, 0 };
	U8 TMP_NOT_OK = 0;

	user_param(ACQ_CALIBRATION | ACQ_MOTOR_OFF | ACQ_NO_PP_SENSOR | ACQ_NO_MIRROR | ACQ_NO_SHADING);  //Park test

EXP_CHK:

	if (!Scan_Param())
		return FALSE;
	Sleep(200);
	cal_img_buf_store(0, 0, 0);
	if (!g_pointer_lDrv->_Scan_Shad_Calibration(set) || !job_Scan() || !job_Wait(JOB_SCAN, 1))
		return FALSE;


	for (i = 0; i < 2; i++) {
		if (SIDE_OK[i])
			continue;

		buf = (U16*)K_img_buf[i];
		dot = cap->ccd[i].dot;
		exp = set->ccd[i].exp;

		_cal_average_iterate(buf, dot * color_loop, k_scan_par.img.height);

		for (j = 0; j < color_loop; j++)
			//white[j] = _cal_find_max(&buf[j], color, dot);
			white[j] = _cal_average_data(&buf[j], color_loop, dot);

		if (CYCLE_COUNT[i] == 0) {
			white_min[i] = _cal_find_min(white, 1, color_loop);

			white_min[i] = (white_min[i] > CAL_EXP_WHITE) ? CAL_EXP_WHITE : white_min[i];
		}

		TMP_NOT_OK = 0;
		for (j = 0; j < color_loop; j++) {
			if (white[j] < (white_min[i] - CAL_EXP_THD)) {
				TMP_NOT_OK = 1;
				//exp[j] = exp[j] * white_min / white[j];
				if (k_scan_par.img.mono)
					exp[j] += CAL_EXP_MINUS_G; //park test
											   //exp[j] += (CAL_EXP_MINUS*5);
				else
					exp[j] += CAL_EXP_MINUS_C;
			}
			else if (white[j] > (white_min[i] + CAL_EXP_THD)) {
				TMP_NOT_OK = 1;
				if (k_scan_par.img.mono)
					exp[j] -= CAL_EXP_MINUS_G; //park test
											   //exp[j] -= (CAL_EXP_MINUS*5);
				else
					exp[j] -= CAL_EXP_MINUS_C;
			}
		}

		if (!TMP_NOT_OK) {
			SIDE_OK[i] = 1;
		}
		else {
			CYCLE_COUNT[i]++;
		}

		for (;j < 3; j++)
			exp[j] = exp[0];


		_cal_check_exposure_time(exp, j, cap->ccd[i].exp_max, cap->ccd[i].exp_min);

		Save_LED_AFE(1, &k_scan_par, exp, i);

	}



	if ((SIDE_OK[0] & SIDE_OK[1]) || (CYCLE_COUNT[0] == (CAL_EXP_ABORT + 1)) || (CYCLE_COUNT[1] == (CAL_EXP_ABORT + 1))) {
		if ((CYCLE_COUNT[0] == (CAL_EXP_ABORT + 1)) || CYCLE_COUNT[1] == (CAL_EXP_ABORT + 1)) {
			EXP_ABORT = 1;
		}
		goto EXP_OK;
	}
	else {
		goto EXP_CHK;
	}

EXP_OK:

	return TRUE;
}

//============================
static int data1[256];
//---------------------------------
void quicksort(int left, int right)
{
	int i, j, s, temp;

	if (left < right) {
		s = data1[(left + right) / 2];
		i = left - 1;
		j = right + 1;

		while (1) {
			while (data1[++i] < s);
			while (data1[--j] > s);
			if (i >= j)
				break;
			temp = data1[i];
			data1[i] = data1[j];
			data1[j] = temp;
		}

		quicksort(left, i - 1);
		quicksort(j + 1, right);
	}
}
//---------------------------------
int average_quicksort(unsigned short *input, int left, int right, int number, int offset)
{
	int i, result = 0;
	//offset /= sizeof(unsigned short);
	for (i = 0; i < number; i++, input += offset)
		data1[i] = *input;
	quicksort(0, number - 1);
	for (i = left, number -= right; i < number; i++)
		result += data1[i];
	result /= (number - left);
	return result;
}
//============================

void _cal_ave_sort_iterate(U16 *data, int num_x, int num_y)
{
	U16 *last_data;
	//int left = num_y / 4;
	//int right = num_y / 4;
	int left = DARK_DROP;
	int right = WHITE_DROP;
	for (last_data = data + num_x; data < last_data; data++)
		*data = average_quicksort(data, left, right, num_y, num_x);
}

__inline void _cal_construct_white16(U16 *data, U32 *shad, int next_data, int next_shad, int num, U32 gain_base, U16 white_target)
{
	U32 white_gain, white;
	U16 *last_data = data + next_data*num;
	while (data < last_data) {
		white = *data;
		if (white > 0) {
			white_gain = white_target * gain_base / white;
			if (white_gain > 0xffff)
				white_gain = 0xffff;
		}
		else {
			white_gain = 0xffff;
		}
		*shad = (*shad & 0xffff) + (white_gain << 16);
		data += next_data;
		shad += next_shad;
	}
}

U32 _cal_set_white_gain(SHD_SET_T *set, U32 white_min)
{
	U32 gain_base;
	gain_base = 0x2000;
	set->gain_base = 8;
	return gain_base;
}


int cal_white_shading(CALIBRATION_CAP_T *cap, CALIBRATION_SET_T *set)
{
	int i, j;
	int color_loop = (k_scan_par.img.mono == 4) ? 1 : 3;
	U16 *buf, *shad_data, white_min;
	U32 dot, *white_buf, gain_base, dark_digit;
	U16 white_target[3];

	if (k_scan_par.source == I3('ADF')) { //Park test
		user_param(ACQ_CALIBRATION | ACQ_NO_PP_SENSOR | ACQ_NO_MIRROR | ACQ_NO_SHADING);
	}
	else {
		user_param(ACQ_CALIBRATION | ACQ_NO_MIRROR | ACQ_NO_SHADING);
	}

	if (!Scan_Param()) //Park test
		return FALSE;

	cal_img_buf_store(0, 0, 0);


	if (!g_pointer_lDrv->_Scan_Shad_Calibration(set))
		return FALSE;

	for (i = 0; i < 2; i++) {
		dark_digit = set->shd[i].dark_digit;
		dot = cap->ccd[i].dot * ((dark_digit == 16) ? 2 : 1);
		shad_data = (U16*)K_shad_data[i];
		if (k_scan_par.img.mono) {
			//Scan_Shad_Shading(i, set->shd[i].mono, shad_data, dot*2);  // GL orginal
			g_pointer_lDrv->_Scan_Shad_Shading(i, 1, shad_data, dot * 2);  //Park test 
		}
		else {
			for (j = 0; j < 3; j++)
				g_pointer_lDrv->_Scan_Shad_Shading(i, j + 1, &shad_data[j*dot], dot * 2);
		}
	}

	if (!job_Scan() || !job_Wait(JOB_SCAN, 1))
		return FALSE;

	if (k_scan_par.source == I3('ADF')) {
		white_target[0] = CAL_SHADING_WHITE_B_R;
		white_target[1] = CAL_SHADING_WHITE_B_G;
		white_target[2] = CAL_SHADING_WHITE_B_B;
	}
	else {
		white_target[0] = CAL_SHADING_WHITE_A_R;
		white_target[1] = CAL_SHADING_WHITE_A_G;
		white_target[2] = CAL_SHADING_WHITE_A_B;
	}

	for (i = 0; i < 2; i++) {
		buf = (U16*)K_img_buf[i];
		dot = cap->ccd[i].dot;
		white_buf = (U32*)K_shad16_data[i];

		_cal_ave_sort_iterate(buf, dot*color_loop, k_scan_par.img.height);
		white_min = _cal_find_min(buf, 1, dot*color_loop);
		gain_base = _cal_set_white_gain(&set->shd[i], white_min);

		for (j = 0; j < color_loop; j++) {
			_cal_construct_white16(&buf[j], &white_buf[j*dot], color_loop, 1, dot, gain_base, white_target[j]);
		}

		Save_Shading(&k_scan_par, buf, white_buf, gain_base, i);
		_cal_do_shift_dark(white_buf, (U16*)K_shad_data[i], dot*color_loop, 16, 0);
	}


	if (k_scan_par.source == I3('ADF')) { //Park test
		user_param(ACQ_CALIBRATION | ACQ_NO_PP_SENSOR | ACQ_NO_MIRROR | ACQ_NO_SHADING);
	}
	else {
		user_param(ACQ_CALIBRATION | ACQ_NO_MIRROR | ACQ_NO_SHADING);
	}

	if (!Scan_Param())  //Park test
		return FALSE;

	cal_img_buf_store(0, 0, 0);
	if (!g_pointer_lDrv->_Scan_Shad_Calibration(set))
		return FALSE;

	for (i = 0; i < 2; i++) {
		dark_digit = 16; //set->shd[i].dark_digit;
		dot = cap->ccd[i].dot * ((dark_digit == 16) ? 2 : 1);
		shad_data = (U16*)K_shad_data[i];
		if (k_scan_par.img.mono) {
			//Scan_Shad_Shading(i, set->shd[i].mono, shad_data, dot*2);  // GL orginal
			g_pointer_lDrv->_Scan_Shad_Shading(i, 1, shad_data, dot * 2);  //Park test 
		}
		else {
			for (j = 0; j < 3; j++)
				g_pointer_lDrv->_Scan_Shad_Shading(i, j + 1, &shad_data[j*dot], dot * 2);
		}
	}

	if (!job_Scan() || !job_Wait(JOB_SCAN, 1))
		return FALSE;

	return TRUE;
}

int cal_save_shd_flash(CALIBRATION_CAP_T *cap, CALIBRATION_SET_T *set)
{
	FLASH_SHD_T *shd = (FLASH_SHD_T*)K_img_buf;
	U8 *src, *dst;
	int i, j, channel, len;

	shd->source = (U8)k_scan_par.source;  //(U8)acq->source;
	shd->duplex = k_scan_par.duplex;    //acq->duplex;
	shd->bit = k_scan_par.img.bit;    //acq->img->bit;
	shd->dot = cap->ccd[0].dot;
	shd->dpi = cap->ccd[0].dpi;
	shd->bank_num = ((shd->bit < 24) || (cap->ccd[0].type == I3('CCD'))) ? 1 : 3;
	shd->bank_size = shd->dot * (shd->bit / 8) / shd->bank_num * 2;
	for (i = 0, dst = (U8*)shd + sizeof(FLASH_SHD_T); i < 2; i++) {
		for (j = 0; j < 3; j++)
			shd->exp[i][j] = set->ccd[i].exp[j];
		channel = (cap->ccd[i].type == I4('CIS6')) ? 6 : 3;
		for (j = 0; j < channel; j++) {
			shd->offset[i][j] = set->afe[i].offset[j];
			shd->gain[i][j] = set->afe[i].gain[j];
		}
		if (shd->duplex & (1 << i)) {
			src = K_shad_data[i];
			memcpy(dst, src, shd->bank_num*shd->bank_size);
			dst += (shd->bank_num*shd->bank_size);
		}
	}

	len = (int)dst - (int)shd;
	g_pointer_lDrv->_Scan_Shad_Flash(shd, len);
	return TRUE;
}

int cal_save_me_flash(CALIBRATION_SET_T *set)
{
	FLASH_ME_T me;
	U8 *src, *dst;
	int i, j, channel, len;

	me.prefeed = set->me.prefeed;
	me.postfeed = set->me.postfeed;

	g_pointer_lDrv->_Scan_ME_Flash(&me, sizeof(FLASH_ME_T));
	return TRUE;
}

USBAPI_API int __stdcall DoCalibration()
{
	int nResult = FALSE;

	CGLDrv glDrv;

	g_pointer_lDrv = &glDrv;

	if (glDrv._OpenDevice() == TRUE)
	{
		int CalibrationMode[] = { 300,600 };
		int CalibrationTimes = sizeof(CalibrationMode) / sizeof(int);

		memset(&k_scan_par, 0, sizeof(SC_PAR_DATA_T));
		k_scan_par.source = I3('ADF');
		k_scan_par.duplex = 3;
		k_scan_par.img.format = I3('RAW');
		k_scan_par.img.bit = 48;
		k_scan_par.img.mono = 0;

		SCAN_DOC_SIZE = DOC_K_PRNU;

		nResult = glDrv._JobCreate();
		if (nResult == 0)
		{
			K_BatchNum++;
			K_PageNum = 0;

			if (cal_prefeed(&K_Cap, &K_Set))
			{
				nResult = TRUE;

				for (int i = 0; i < CalibrationTimes;i++)
				{
					k_scan_par.img.dpi.x = CalibrationMode[i];
					k_scan_par.img.dpi.y = CalibrationMode[i];

					if (!cal_set_def(&K_Cap, &K_Set))
					{
						nResult = TRUE;
						break;
					}

					if (!cal_AFE_offset(&K_Cap, &K_Set))
					{
						nResult = TRUE;
						break;
					}

					if (!cal_exposure_time(&K_Cap, &K_Set))
					{
						nResult = TRUE;
						break;
					}

					if (!cal_AFE_gain(&K_Cap, &K_Set))
					{
						nResult = TRUE;
						break;
					}

					if (!cal_exposure_time(&K_Cap, &K_Set))
					{
						nResult = TRUE;
						break;
					}

					if (!cal_dark_shading(&K_Cap, &K_Set))
					{
						nResult = TRUE;
						break;
					}

					if (!cal_white_shading(&K_Cap, &K_Set))
					{
						nResult = TRUE;
						break;
					}

					cal_save_shd_flash(&K_Cap, &K_Set);
				}
			}

			glDrv._JobEnd();
		}

		glDrv._CloseDevice();
	}

	return nResult;
}