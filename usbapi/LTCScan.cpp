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
#include "LTCInterface\LTCDriveApi.h"
#include "ImgFile\ImgFile.h"
#include <usbscan.h>

#pragma comment(lib, "dnssd.lib")
#pragma comment(lib, "Ws2_32.lib")
#pragma comment(lib, "Iphlpapi.lib")

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
};

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

int start_cancel = 0;
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

	int lineNumber = 1000;
	int nColPixelNumOrig = 0;   
	int nLinePixelNumOrig = 0;  
	int imgBufferSize = 0;
  
	BYTE* imgBuffer = NULL; 

	nLinePixelNumOrig = width*resolution / 1000;
	nColPixelNumOrig = height*resolution / 1000;

	imgBufferSize = GetByteNumPerLineWidthPad(BitsPerPixel, nLinePixelNumOrig) * lineNumber;

	imgBuffer = new BYTE[imgBufferSize];


	IMG_FILE_T ImgFile[2];
	float ADF_SideEdge = (8.5 - 8.4528) / 2;

	ImgFile[0].img.org.x = ADF_SideEdge * resolution;
	ImgFile[0].img.org.y = 0;
	ImgFile[0].img.dot.w = ImgFile[1].img.dot.w = nLinePixelNumOrig;
	ImgFile[0].img.dot.h = ImgFile[1].img.dot.h = nColPixelNumOrig;


	ImgFile[0].img.format = ImgFile[1].img.format = I3('JPG');
	ImgFile[0].img.bit = ImgFile[1].img.bit = BitsPerPixel;
	ImgFile[0].img.dpi.x = ImgFile[1].img.dpi.x = resolution;
	ImgFile[0].img.dpi.y = ImgFile[1].img.dpi.y = resolution;



	glDrv.sc_job_create.mode = I1('D');
	glDrv.sc_pardata.source = I3('ADF');
	glDrv.sc_pardata.duplex = ADFMode ? 3 : 1;
	glDrv.sc_pardata.page = 0;
	glDrv.sc_pardata.format = I3('JPG');
	glDrv.sc_pardata.bit = BitsPerPixel;
	glDrv.sc_pardata.dpi.x = resolution;
	glDrv.sc_pardata.dpi.y = resolution;
	glDrv.sc_pardata.org.x = ImgFile[0].img.org.x;
	glDrv.sc_pardata.org.y = ImgFile[0].img.org.y;
	glDrv.sc_pardata.dot.w = ImgFile[0].img.dot.w;
	glDrv.sc_pardata.dot.h = ImgFile[0].img.dot.h;

	glDrv.sc_pardata.mono = 0;
	glDrv.sc_pardata.acquire = (1 * ACQ_SHADING) | (1 * ACQ_GAMMA) | ACQ_MIRROR | (0 * ACQ_START_HOME) | ACQ_AUTO_GO_HOME | (0*ACQ_LAMP_OFF);

	int result = 0;
	int end_page[2] = { 0 };
	int end_doc = 0;
	int duplex = 3, dup = 0;
	int page_line[2] = { 0 };
	int ImgSize = 0, ImgSize_last = 0;
	int RunInCounter = 0;
	U8  CancelKey[64];
	U8 open[2] = { 0 };
	U8 stop_start_t = 0;
	int page[2] = { 0 };
	int fileCount = 0;
	

	if (glDrv._OpenDevice() == TRUE)
	{
		
		result = glDrv._StatusGet();
		if (!result) {
			return RETSCAN_ERROR;
		}

		result = glDrv._StatusCheck_Start();
		if (!result) {
			return RETSCAN_ERROR;
		}

		if (glDrv.sc_status_data.system & 0x10) {
			result = glDrv._ResetScan();
			if (!result)
				return RETSCAN_ERROR;
		}

		result = glDrv._JobCreate();

		if (glDrv.sc_pardata.source == I3('ADF')) {
			if (!(glDrv.sc_pardata.acquire & ACQ_RODLENS)){ //set for Rod lens calibration
				result = glDrv._StatusGet();
				if (!result) {
					glDrv._JobEnd();
					return RETSCAN_ERROR;
				}
				result = glDrv._StatusCheck_ADF_Check();
				if (!result) {
					glDrv._JobEnd();
					return RETSCAN_ERROR;
				}
			}
		}

		result = glDrv._parameters();
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
		if (!result)
		{
			glDrv._JobEnd();
			return RETSCAN_ERROR;
		}

		duplex = glDrv.sc_pardata.duplex;


		while (result && (end_doc == 0)) {

			int lineCount = 0;
			end_page[0] = end_page[1] = end_doc = 0;
			page_line[0] = page_line[1] = 0;

			Sleep(200);
			result = glDrv._info();

			if (!result) {
				glDrv._StatusGet();
				glDrv._StatusCheck_Scanning();
				break;
			}

			end_doc = glDrv.sc_infodata.EndDocument;

		
			if ((glDrv.sc_infodata.ValidPageSize[0] > 0) || (glDrv.sc_infodata.ValidPageSize[1] > 0)) {

				char fileName[256] = { 0 };
				char filePath[256] = { 0 };
				TCHAR fileNameOut[256] = { 0 };

				::WideCharToMultiByte(CP_ACP, 0, tempPath, -1, filePath, 256, NULL, NULL);

				if (duplex & 1) {		
					sprintf(fileName, "%s_%c%d_A%02d.%s", filePath, (ImgFile[0].img.bit > 16) ? 'C' : 'G', ImgFile[0].img.dpi.x, page[0], &ImgFile[0].img.format);
					ImgFile_Open(&ImgFile[0], fileName);
					open[0] = 1;

					::MultiByteToWideChar(CP_ACP, 0, fileName, strlen(fileName), fileNameOut, 256);
					bstrArray[fileCount] = ::SysAllocString(fileNameOut);
					fileCount++;
				}
				if (duplex & 2) {
					sprintf(fileName, "%s_%c%d_B%02d.%s", filePath, (ImgFile[1].img.bit > 16) ? 'C' : 'G', ImgFile[1].img.dpi.x, page[1], &ImgFile[1].img.format);
					ImgFile_Open(&ImgFile[1], fileName);
					open[1] = 1;

					::MultiByteToWideChar(CP_ACP, 0, fileName, strlen(fileName), fileNameOut, 256);
					bstrArray[fileCount] = ::SysAllocString(fileNameOut);
					fileCount++;
				}
				
				while (result && (((duplex & 1) && (end_page[0] == 0)) || ((duplex & 2) && (end_page[1] == 0))))
				{
					result = glDrv._info();
					if (!result) {

						glDrv._StatusGet();

						glDrv._StatusCheck_Scanning();

						if (start_cancel) {
							start_cancel = 0;
							glDrv._JobEnd();
							return RETSCAN_ERROR;
						}
						else
						{
							if ((duplex & 1) && open[0]) {
								ImgFile_Close(&ImgFile[0], page_line[0]);
								open[0] = 0;
							}
							if ((duplex & 2) && open[1]) {
								ImgFile_Close(&ImgFile[1], page_line[1]);
								open[1] = 0;
							}

							return RETSCAN_ERROR;
						}
							
					}
					end_doc = glDrv.sc_infodata.EndDocument;

					if ((duplex & 1) && (end_page[0] == 0)) {
						ImgSize = 0;
						if (glDrv.sc_infodata.ValidPageSize[0] > 0) {
							result = glDrv._ReadImageEX(0, &ImgSize, imgBuffer, imgBufferSize) &&
								ImgFile_Write(&ImgFile[0], imgBuffer, ImgSize);
							if (!result)
							{
								if ((duplex & 1) && open[0]) {
									ImgFile_Close(&ImgFile[0], page_line[0]);
									open[0] = 0;
								}
							
							}
								
						}
						if (ImgSize >= glDrv.sc_infodata.ValidPageSize[0]) {
							end_page[0] = glDrv.sc_infodata.EndPage[0];
							if ((page_line[0] == 0) && end_page[0])
								page_line[0] = glDrv.sc_infodata.ImageLength[0];
						}
					}
					if ((duplex & 2) && (end_page[1] == 0)) {
						ImgSize = 0;
						if (glDrv.sc_infodata.ValidPageSize[1] > 0) {
							result = glDrv._ReadImageEX(1, &ImgSize, imgBuffer, imgBufferSize) &&
								ImgFile_Write(&ImgFile[1], imgBuffer, ImgSize);
							if (!result)
							{
							
								if ((duplex & 2) && open[1]) {
									ImgFile_Close(&ImgFile[1], page_line[1]);
									open[1] = 0;
								}
							}

						}
						if (ImgSize >= glDrv.sc_infodata.ValidPageSize[1]) {
							end_page[1] = glDrv.sc_infodata.EndPage[1];
							if ((page_line[1] == 0) && end_page[1])
								page_line[1] = glDrv.sc_infodata.ImageLength[1];
						}
					}

					int percent = 0;
					lineCount += lineNumber;
					percent = lineCount * 100 / nColPixelNumOrig;

					if (percent > 100)
						percent = 100;

					::SendNotifyMessage(HWND_BROADCAST, uMsg, percent, 0);
					Sleep(100);

				}



				if ((duplex & 1) && open[0]) {
					ImgFile_Close(&ImgFile[0], page_line[0]);
					open[0] = 0;
				}
				if ((duplex & 2) && open[1]) {
					ImgFile_Close(&ImgFile[1], page_line[1]);
					open[1] = 0;
				}

				page[0]++;
				page[1]++;



			}

		}


		if (glDrv.sc_infodata.Cancel == 0)
			glDrv._stop();
		else
			glDrv._cancel();

	
		glDrv._JobEnd();

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
		return RETSCAN_OPENFAIL;
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
}
