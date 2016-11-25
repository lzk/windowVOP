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
};

extern CRITICAL_SECTION g_csCriticalSection_UsbTest;
extern CRITICAL_SECTION g_csCriticalSection_NetWorkTest;
extern BOOL TestIpConnected(wchar_t* szIP);
extern BOOL TestIpConnected(wchar_t* szIP, Scan_RET *status);

wchar_t g_ipAddress[256] = { 0 };
BOOL g_connectMode_usb = TRUE;
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
	UINT32 uMsg,
	SAFEARRAY** fileNames);

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

	if (g_pointer_lDrv != NULL)
	{
		return g_pointer_lDrv->_cancel();
	}

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

	float brightness = Brightness / 50.0f; // no change in brightness
	float contrast = Contrast / 50.0f; // twice the contrast
	float gamma = 1.0f; // no change in gamma

	float adjustedBrightness = brightness - 1.0f;
	// create matrix that will brighten and contrast the image
	Gdiplus::ColorMatrix ptsArray = {
		contrast, 0, 0, 0, 0,	// scale red
		0, contrast, 0, 0, 0,	// scale green
		0, 0, contrast, 0, 0,	// scale blue
		0, 0, 0, 1.0f, 0,		// don't scale alpha
		adjustedBrightness, adjustedBrightness, adjustedBrightness, 0, 1 };

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

USBAPI_API int __stdcall ADFScan(const wchar_t* sz_printer,
	const wchar_t* tempPath,
	int BitsPerPixel,
	int resolution,
	int width,
	int height,
	int contrast,
	int brightness,
	BOOL ADFMode,
	UINT32 uMsg,
	SAFEARRAY** fileNames)
{
	
	BSTR bstrArray[500] = { 0 };
	CGLDrv glDrv;
	g_pointer_lDrv = &glDrv;

	int lineNumber = 1000;
	int nColPixelNumOrig = 0;   
	int nLinePixelNumOrig = 0;  
	int imgBufferSize = 0;
  
	BYTE* imgBuffer = NULL; 

	nLinePixelNumOrig = width*resolution / 1000;
	nLinePixelNumOrig = nLinePixelNumOrig - nLinePixelNumOrig % 8;

	nColPixelNumOrig = height*resolution / 1000;

	imgBufferSize = GetByteNumPerLineWidthPad(BitsPerPixel, nLinePixelNumOrig) * lineNumber;

	imgBuffer = new BYTE[imgBufferSize];


	IMG_FILE_T ImgFile[2];
	float ADF_SideEdge = (8.5 - 8.4528) / 2;

	ImgFile[0].img.org.x = 0;//ADF_SideEdge * resolution;
	ImgFile[0].img.org.y = 0;

	ImgFile[0].img.width = ImgFile[1].img.width = nLinePixelNumOrig;
	ImgFile[0].img.height = ImgFile[1].img.height = nColPixelNumOrig;


	ImgFile[0].img.format = ImgFile[1].img.format = I3('JPG');
	ImgFile[0].img.bit = ImgFile[1].img.bit = BitsPerPixel;
	ImgFile[0].img.dpi.x = ImgFile[1].img.dpi.x = resolution;
	ImgFile[0].img.dpi.y = ImgFile[1].img.dpi.y = resolution;


	//glDrv.sc_pardata.acquire = (1 * ACQ_NO_SHADING) | (0 * ACQ_SET_MTR) | ACQ_NO_MIRROR | (1 * ACQ_NO_PP_SENSOR) | (0 * ACQ_PICK_SS );

	//glDrv.sc_job_create.mode = I1('D');
	glDrv.sc_job_create.mode = 0;

	glDrv.sc_pardata.source = I3('ADF');
	glDrv.sc_pardata.duplex = ADFMode ? SCAN_AB_SIDE : SCAN_A_SIDE;
	glDrv.sc_pardata.page = 0;
	glDrv.sc_pardata.img.format = I3('JPG');
	glDrv.sc_pardata.img.bit = BitsPerPixel;
	glDrv.sc_pardata.img.dpi.x = resolution;
	glDrv.sc_pardata.img.dpi.y = resolution;
	glDrv.sc_pardata.img.org.x = ImgFile[0].img.org.x;
	glDrv.sc_pardata.img.org.y = ImgFile[0].img.org.y;
	glDrv.sc_pardata.img.width = ImgFile[0].img.width;
	glDrv.sc_pardata.img.height = ImgFile[0].img.height;
	glDrv.sc_pardata.img.mono = IMG_COLOR;

	if (glDrv.sc_pardata.img.format == I3('JPG')) {
		glDrv.sc_pardata.img.option = IMG_OPT_JPG_FMT444;
	}

	//Advanced
	if (glDrv.sc_pardata.acquire & ACQ_SET_MTR) {

			/*ADF scan*/
		glDrv.sc_pardata.mtr[1].pick_ss_step = 600;//GetPrivateProfileInt("PICK_SS_STEP", "STEP", 0, IniFile);

			//par->mtr[0].drive_target = CMT_PH;
			//par->mtr[0].state_mechine = SCAN_STATE_MECHINE;
			//par->mtr[1].drive_target = BMT_PH;
			//par->mtr[1].state_mechine = STATE_MECHINE_1;
		if (glDrv.sc_pardata.img.bit < 24) {
			/*Mono scan*/
			glDrv.sc_pardata.mtr[1].speed_pps = 3456;// GetPrivateProfileInt("ADFgray", "PICK_PPS", 0, IniFile);
			glDrv.sc_pardata.mtr[1].direction = 1;// GetPrivateProfileInt("ADFgray", "PICK_DIR", 0, IniFile);
			glDrv.sc_pardata.mtr[1].micro_step = 2;// GetPrivateProfileInt("ADFgray", "PICK_MS", 0, IniFile);
			glDrv.sc_pardata.mtr[1].currentLV = 4;// GetPrivateProfileInt("ADFgray", "PICK_CLV", 0, IniFile);

			glDrv.sc_pardata.mtr[0].speed_pps = 4577;// GetPrivateProfileInt("ADFgray", "FEED_PPS", 0, IniFile);
			glDrv.sc_pardata.mtr[0].direction = 0;// GetPrivateProfileInt("ADFgray", "FEED_DIR", 0, IniFile);
			glDrv.sc_pardata.mtr[0].micro_step = 4;// GetPrivateProfileInt("ADFgray", "FEED_MS", 0, IniFile);
			glDrv.sc_pardata.mtr[0].currentLV = 4;// GetPrivateProfileInt("ADFgray", "FEED_CLV", 0, IniFile);
		}
		else {
			/*Color scan*/
			if ((glDrv.sc_pardata.img.dpi.x == 300) && (glDrv.sc_pardata.img.dpi.y == 300)) {
				/*300x300DPI scan*/
				glDrv.sc_pardata.mtr[1].speed_pps = 2602;// GetPrivateProfileInt("ADF300x300color", "PICK_PPS", 0, IniFile);
				glDrv.sc_pardata.mtr[1].direction = 1;// GetPrivateProfileInt("ADF300x300color", "PICK_DIR", 0, IniFile);
				glDrv.sc_pardata.mtr[1].micro_step = 2;// GetPrivateProfileInt("ADF300x300color", "PICK_MS", 0, IniFile);
				glDrv.sc_pardata.mtr[1].currentLV = 4;// GetPrivateProfileInt("ADF300x300color", "PICK_CLV", 0, IniFile);

				glDrv.sc_pardata.mtr[0].speed_pps = 350;// GetPrivateProfileInt("ADF300x300color", "FEED_PPS", 0, IniFile);
				glDrv.sc_pardata.mtr[0].direction = 0;// GetPrivateProfileInt("ADF300x300color", "FEED_DIR", 0, IniFile);
				glDrv.sc_pardata.mtr[0].micro_step = 4;// GetPrivateProfileInt("ADF300x300color", "FEED_MS", 0, IniFile);
				glDrv.sc_pardata.mtr[0].currentLV = 4;// GetPrivateProfileInt("ADF300x300color", "FEED_CLV", 0, IniFile);
			}
			else if ((glDrv.sc_pardata.img.dpi.x == 300) && (glDrv.sc_pardata.img.dpi.y == 600)) {
				/*300x600DPI scan*/
				glDrv.sc_pardata.mtr[1].speed_pps = 2317;// GetPrivateProfileInt("ADF300x600color", "PICK_PPS", 0, IniFile);
				glDrv.sc_pardata.mtr[1].direction = 1;// GetPrivateProfileInt("ADF300x600color", "PICK_DIR", 0, IniFile);
				glDrv.sc_pardata.mtr[1].micro_step = 4;// GetPrivateProfileInt("ADF300x600color", "PICK_MS", 0, IniFile);
				glDrv.sc_pardata.mtr[1].currentLV = 4;// GetPrivateProfileInt("ADF300x600color", "PICK_CLV", 0, IniFile);

				glDrv.sc_pardata.mtr[0].speed_pps = 2612;// GetPrivateProfileInt("ADF300x600color", "FEED_PPS", 0, IniFile);
				glDrv.sc_pardata.mtr[0].direction = 0;// GetPrivateProfileInt("ADF300x600color", "FEED_DIR", 0, IniFile);
				glDrv.sc_pardata.mtr[0].micro_step = 4;// GetPrivateProfileInt("ADF300x600color", "FEED_MS", 0, IniFile);
				glDrv.sc_pardata.mtr[0].currentLV = 4;// GetPrivateProfileInt("ADF300x600color", "FEED_CLV", 0, IniFile);
			}
			else {
				/*600DPI scan*/
				glDrv.sc_pardata.mtr[1].speed_pps = 1293;// GetPrivateProfileInt("ADF600x600color", "PICK_PPS", 0, IniFile);
				glDrv.sc_pardata.mtr[1].direction = 1;// GetPrivateProfileInt("ADF600x600color", "PICK_DIR", 0, IniFile);
				glDrv.sc_pardata.mtr[1].micro_step = 4;// GetPrivateProfileInt("ADF600x600color", "PICK_MS", 0, IniFile);
				glDrv.sc_pardata.mtr[1].currentLV = 4;// GetPrivateProfileInt("ADF600x600color", "PICK_CLV", 0, IniFile);

				glDrv.sc_pardata.mtr[0].speed_pps = 1557;// GetPrivateProfileInt("ADF600x600color", "FEED_PPS", 0, IniFile);
				glDrv.sc_pardata.mtr[0].direction = 0;// GetPrivateProfileInt("ADF600x600color", "FEED_DIR", 0, IniFile);
				glDrv.sc_pardata.mtr[0].micro_step = 4;// GetPrivateProfileInt("ADF600x600color", "FEED_MS", 0, IniFile);
				glDrv.sc_pardata.mtr[0].currentLV = 4;// GetPrivateProfileInt("ADF600x600color", "FEED_CLV", 0, IniFile);
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
	int ImgSize = 0, ImgSize_last = 0;
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
				return RETSCAN_BUSY;
			}
		}
	}
	
	MyOutputString(L"ADF Enter");
	if (glDrv._OpenDevice() == TRUE)
	{
		
		result = glDrv.paperReady();
		if (!result) {
			return RETSCAN_ERROR;
		}
		MyOutputString(L"paperReady");

		result = glDrv._JobCreate();
		if (!result)
			return RETSCAN_ERROR;

		MyOutputString(L"_JobCreate");


		result = glDrv._parameters();
		MyOutputString(L"_parameters");
		if (!result)
			return RETSCAN_ERROR;

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

		result = glDrv._StartScan();
		MyOutputString(L"_StartScan");
		if (!result)
		{
			glDrv._JobEnd();
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
		while (!start_cancel) {
			glDrv._info();
			if ((!(duplex & 1) || glDrv.sc_infodata.EndScan[0]) && (!(duplex & 2) || glDrv.sc_infodata.EndScan[1]))
				break;
		/*	if (_kbhit()) {
				_getch();
				_cancel(JobID);
				cancel = TRUE;
			}*/
			for (dup = 0; dup < 2; dup++) {
				if ((duplex & (1 << dup)) && glDrv.sc_infodata.ValidPageSize[dup]) {
					ImgSize = 0;
					if (glDrv._ReadImageEX(dup, &ImgSize, imgBuffer, imgBufferSize)) {
						if (!bFiling[dup]) {
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

							sprintf(fileName, "%s_%c%d_%c%02d.%s", filePath, (ImgFile[dup].img.bit > 16) ? 'C' : 'G', ImgFile[dup].img.dpi.x, side, page[dup], &ImgFile[dup].img.format);
							ImgFile_Open(&ImgFile[dup], fileName);
							lineCount = 0;

							::MultiByteToWideChar(CP_ACP, 0, fileName, strlen(fileName), fileNameOut, 256);
							bstrArray[fileCount] = ::SysAllocString(fileNameOut);
							MyOutputString(fileNameOut);
							fileCount++;
							page[dup]++;
						}
						ImgFile_Write(&ImgFile[dup], imgBuffer, ImgSize);

						int percent = 0;
						int L = (int)round((double)ImgSize / (double)GetByteNumPerLineWidthPad(BitsPerPixel, nLinePixelNumOrig));
						lineCount += lineNumber;
						percent = lineCount * 100 / nColPixelNumOrig;
						//MyOutputString(L"Percent ", lineCount);
						if (percent > 100)
							percent = 100;

						::SendNotifyMessage(HWND_BROADCAST, uMsg, percent, 0);
						Sleep(100);


						if ((ImgSize >= (int)glDrv.sc_infodata.ValidPageSize[dup]) && glDrv.sc_infodata.EndPage[dup]) {
							ImgFile_Close(&ImgFile[dup], glDrv.sc_infodata.ImageHeight[dup]);
							bFiling[dup]--;
							lineCount = 0;
						}
					}
				}
				if (start_cancel && bFiling[dup]) {
					ImgFile_Close(&ImgFile[dup], glDrv.sc_infodata.ImageHeight[dup]);
					bFiling[dup] = 0;
					lineCount = 0;
				}
			}
		}

		glDrv._stop();
		glDrv.waitJobFinish(0);

		glDrv._JobEnd();
		MyOutputString(L"_JobEnd");

		//contrast, brightness
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
	}
	else
	{
		if(g_connectMode_usb == TRUE)
			return RETSCAN_OPENFAIL;
		else
			return RETSCAN_OPENFAIL_NET;
	}


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
	int  iCnt;

	//EnterCriticalSection(&g_csCriticalSection_UsbTest);

	for (iCnt = 0; iCnt <= MAX_DEVICES; iCnt++) {
		_stprintf_s(strPort, L"%s%d", USBSCANSTRING, iCnt);
		hDev = CreateFile(strPort,
			GENERIC_READ | GENERIC_WRITE,
			FILE_SHARE_READ | FILE_SHARE_WRITE,
			NULL,
			OPEN_EXISTING,
			FILE_FLAG_OVERLAPPED, NULL);

		if (hDev != INVALID_HANDLE_VALUE) {
			::WideCharToMultiByte(CP_ACP, 0, strPort, -1, interfaceName, 32, NULL, NULL);
			break;
		}
	}

	if (hDev == INVALID_HANDLE_VALUE)
	{
		LeaveCriticalSection(&g_csCriticalSection_UsbTest);
		return 0;
	}
		
	if (hDev != INVALID_HANDLE_VALUE) {
		CloseHandle(hDev);
	}

	//LeaveCriticalSection(&g_csCriticalSection_UsbTest);

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