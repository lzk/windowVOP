/*******************************************************************

   Copyright (C), 2006, LiteON

   File name: Scanner.h

   Author: James Yu   Version: 1.0   Date: 2008-09-23

   Description: 

   History: 
      James Yu  2008-09-23   1.0   build this module
      
*******************************************************************/

#if !defined(AFX_WIADEVICE_H__3B404E09_994A_42C7_B36B_EFEE89861E23__INCLUDED_)
#define AFX_WIADEVICE_H__3B404E09_994A_42C7_B36B_EFEE89861E23__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <windows.h>

//////////////////////////////////////////////////////////////////////
// Error Code

const int DEVMON_ERROR_SCAN_STATUS_STOP = 9;
const int DEVMON_ERROR_STILL_WARMUP = 3;
const int DEVMON_STATUS_PAPERINTRAY = 1;
const int DEVMON_STATUS_OK = 0;

const int DEVMON_ERROR = -1;
const int DEVMON_ERROR_IN_USE= -2;
const int DEVMON_ERROR_OPEN_FAILED = -3;
const int DEVMON_ERROR_INVALID_HANDLE = -4;
const int DEVMON_ERROR_POWER_OFF= -5;
const int DEVMON_ERROR_MFP_STOP = -6;
const int DEVMON_ERROR_ADF_JAM = -7;
const int DEVMON_ERROR_NET_INTRP = -8;
const int DEVMON_ERROR_TIMEOUT = -9;
const int DEVMON_ERROR_INVALID_FLASH = -10;
const int DEVMON_ERROR_INCOMPL_FLASH = -11;
const int DEVMON_ERROR_USB_INTRP = -12;
const int DEVMON_ERROR_DRIVER_IN_USE = -21;
const int DEVMON_ERROR_ADF_EMPTY = -71;
const int DEVMON_ERROR_ADF_OPEN = -72;

const int DEVMON_ERROR_OUTOFMEMORY = -40;
const int DEVMON_ERROR_INVALIDARG = -41;

const int DEVMON_ERROR_PAPEREMPTY = -50;
const int DEVMON_ERROR_PATHNOTEXIST = -51;
const int DEVMON_ERROR_INVALID_FILENAME = -52;
const int DEVMON_ERROR_EMPTY_FILENAME = -53;
const int DEVMON_ERROR_NOT_DISK_FREE = -54;
const int DEVMON_ERROR_PATH_NOT_AVAILABLE = -55;
const int DEVMON_ERROR_WIA_NOT_START = -56;
const int DEVMON_ERROR_WIA_NOT_INSTALL = -57;
const int DEVMON_ERROR_PAPER_SIZE_NOT_SUPPORT = -58;

const int BUTTON_SCAN = 0xB1;


const int DEVMON_ERROR_USER_CANCEL_SCAN = 200;
const int DEVMON_STATUS_END_OF_MEDIA = 201;
const int DEVMON_STATUS_CANCEL = 202;


//////////////////////////////////////////////////////////////////////
// Command Code

const int DevMon_LOCKSCANNER = 0x14;
const int DevMon_UNLOCKSCANNER = 0x15;
const int DevMon_ABORTSCAN = 0x3D;
const int DevMon_PROGRESS_ABORTSCAN = 0x3E;
const int DevMon_STOPSCAN = 0x3F;
const int DevMon_ADFSTATUS = 0x46;
const int DevMon_FLASHUP_SERNUM = 0x91;
const int DevMon_FLASHDLPARMZR = 0x92;
const int DevMon_FLASHDLPARMTP = 0x93;
const int DevMon_MOTORDL = 0x94;
const int DevMon_MOTORFBSEND = 0x95;
const int DevMon_MOTORADFSEND = 0x96;
const int DevMon_FLASHDLSEND = 0x97;
const int DevMon_FLASHDLEXEC = 0x98;
const int DevMon_FLASHDLABRT = 0x99;



//////////////////////////////////////////////////////////////////////
// Structure

enum _ScanMode
{
	ScanMode_Mono,
	ScanMode_Gray,
	ScanMode_Color,
	ScanMode_Preview,
	ScanMode_48BitsColor
};

enum _ScanSource
{
	ScanSource_Flatbed,
	ScanSource_ADF,
	ScanSource_Duplex
};

//////////////////////////////////////////////////////////////////////
// Structure

typedef struct _ScanParameter
{
    int ScanSource; // 0 Flatbed
                              // 1 ADF
                              // Duplex ADF
                              
    int ScanMode;   // 0 Black and White
                            // 1 Grayscale (8 bit)
                            // 2 Color (24 bit)
                            // 3 Preview
                            // 4 Color (48 bit)

    int XRes;           // X resolution
    int YRes;           // Y resolution
    int Left;             // left position of scan window (inch/1000)
    int Top;             // top position of scan window (inch/1000)
    int Width;          // width of scan window (inch/1000)
    int Height;        // height of scan window (inch/1000)

    DWORD PixelPerLine;   // pixel number per line (final result)
    DWORD TotalLines;      // total line number (final result)
    
    int Contrast;       // -100 ~ 100
    int Brightness;   // -100 ~ 100
    int Threshold;    // -100 ~ 100

	DWORD MarginTopBottom;
	DWORD MarginLeftRight;
	DWORD MarginMiddle;

} SCANPARAMETER, *LPSCANPARAMETER;

typedef struct ADJUSTSTR
{
	int Flag;       // 0:NO Adjustment
					// 1:Temperature and Gamma(profile setting)
					// 2:Icc Profile(profile setting)
					// 4:Brightness, Contrast and Chroma
					// 8:Balance(Balance setting)
					//16:Thermal RGB Gamma 

	char* ProfilePathI;     //Input profile path    
	char* ProfilePathO;     //Output profile path   
	int Temperature;        //Temperature(0:5000k, 1:6500k, 2:9300k) and Intent: 16: LCS_GM_IMAGES; 32: LCS_GM_GRAPHICS; 48: LCS_GM_BUSINESS;    64: LCS_GM_ABS_COLORIMETRIC
	int GammaValue;         //Gamma(0:1.0, 1:1.4, 2:1.8, 3:2.2, 4:2.6) 
	int Brightness[4];      //Brightness setting for background,text, graphic, photo
	int Contrast[4];        //Contrast setting for background,text, graphic, photo
	int Chroma[4];          // Chroma setting for background,text, graphic, photo
							// height Density                  medium Density          low Density
							// 12bit index        [1536,1280,1024,768,512,256]+[ 96,80,64,48,32,16]+[ 6, 5, 4, 3, 2, 1] 
	int BalanceK;           //Balance setting for Black             [       3,      2,       1, -1, -2, -3]+[  3, 2, 1,-1,-2,-3]+[ 3, 2, 1,-1,-2,-3]
	int BalanceC;           //Balance setting for Cyan              [       3,      2,       1, -1, -2, -3]+[  3, 2, 1,-1,-2,-3]+[ 3, 2, 1,-1,-2,-3]
	int BalanceM;           //Balance setting for Magenta   [       3,      2,       1, -1, -2, -3]+[  3, 2, 1,-1,-2,-3]+[ 3, 2, 1,-1,-2,-3]
	int BalanceY;           //Balance setting for Yellow    [       3,      2,       1, -1, -2, -3]+[  3, 2, 1,-1,-2,-3]+[ 3, 2, 1,-1,-2,-3]

}ADJUSTSTR, *LPADJUSTSTR;



//////////////////////////////////////////////////////////////////////
// Function Types (Export from DevMon)

typedef int (WINAPI *LPFUNC_DevMon_Open)(int *Version, int *ADFAbility, int *NetworkAbility, int *ModelCode, char *CustomerCode);
typedef int (WINAPI *LPFUNC_DevMon_WriteCommand)(int CmdID);
typedef int (WINAPI *LPFUNC_DevMon_SetScanParameter)(int ResoX, int ResoY, int ScanMode, DWORD StartX, DWORD StartY, DWORD EndX, DWORD EndY, int ThrdB, int ScanSouce);
typedef int (WINAPI *LPFUNC_DevMon_SetScanParameterAdj)(unsigned char MediaFlag, unsigned char FilterFlag, unsigned char DescrnFlag, ADJUSTSTR *AdjustInput);
typedef int (WINAPI *LPFUNC_DevMon_GetScanParameter)(DWORD *pPixelPerLine, DWORD *pTotalLines);
typedef int (WINAPI *LPFUNC_DevMon_StartScan)();
typedef int (WINAPI *LPFUNC_DevMon_ReadScanData)(unsigned char * DataBuf, int DataLen, DWORD *BytesRead, int& ImageLength);
typedef int (WINAPI *LPFUNC_DevMon_AbortScan)();
typedef int (WINAPI *LPFUNC_DevMon_AbortScanEx)();
typedef int (WINAPI *LPFUNC_DevMon_Close)();
//typedef BYTE (WINAPI *LPFUNC_ReadPushButton)(LPBYTE pButtonIndex);
//typedef unsigned char UINT8;
//typedef int (WINAPI *LPFUNC_UsbInitializeMFP)();
//typedef int (WINAPI *LPFUNC_writeESCCommand)(UINT8 b0,UINT8 b1,UINT8 b2,UINT8 b3);
typedef int (WINAPI *LPFUNC_DevMon_BroadcastMFP)(int *DevNum, char **DevName);
typedef int (WINAPI *LPFUNC_DevMon_DetectNetMFP)(const char *DevNamIP);
typedef int (WINAPI *LPFUNC_DevMon_LocateMFP)(const char* DevName);
typedef int (WINAPI *LPFUNC_DevMon_PushScan)();



//////////////////////////////////////////////////////////////////////
// CScanner

class CScanner  
{
public:
	CScanner();
	~CScanner();

	int Initialize(LPTSTR lptszLibPath);
	
	bool FindDevice(bool bLocal, char **ppszDevicePathList);

	bool FindNetworkDevice(int *DevNum, char **DevName);

    int Open(LPSTR szDevicePath, const char* szIP, bool bPushScan, int nCapability, int nPushScanFlag );

	int Close();

	int StartScan();

	int SetScanParameterEx(LPSCANPARAMETER lpScanParam); 

	int StopScan();

	int ReadBitmapInfo(BYTE *pBuffer, ULONG ulBufferSize, ULONG *pulBytesRead);
	int ReadData(BYTE *pBuffer, ULONG ulBufferSize,ULONG *pulBytesRead, LONG *plPercentComplete);

	int GetADFStatus();
	int PushScan();
   
public:
	// Reading parameter for current page (Flatbed or one time of ADF scan)
	DWORD	m_dwTotalLinesRead;      // The line number of already readed
	DWORD	m_dwTotalLinesToRead;    // The line number that need to read
	DWORD   m_dwTotalLinesToScan;	 // the line number that nee to read from scanner.

private:
	DWORD	m_dwBytesPerLine;        // contains the number of bytes to copy from each scanline
	DWORD	m_dwPixelsPerLine;
	DWORD	m_dwBitsPerPixel;
	DWORD	m_dwResolution;

	DWORD	m_dwBytesPerLineFromDevice;

	DWORD m_dwMarginTopBottom;
	DWORD m_dwMarginLeftRight;
	DWORD m_dwMarginMiddle;

	int LockScanner();
	int UnLockScanner();

	int SetScanParameter(LPSCANPARAMETER lpScanParam);
	int GetScanParameter(DWORD *pPixelPerLine, DWORD *pTotalLines);

	bool SwapRToBFor24BitsData(BYTE *pBuffer, DWORD dwNumBytesInBuffer, DWORD dwNumBytesPerLine);
	BOOL EdgesErase(LPBYTE pData);
private:
	// pointer to DevMon LLD
	HMODULE m_hLLD;

	//HANDLE m_hMutex;

	bool	m_bScannerLocked;
	bool m_bDeviceOpened;

	// pointers to DevMon LLD functions
	LPFUNC_DevMon_Open DevMon_Open;
	LPFUNC_DevMon_WriteCommand DevMon_WriteCommand;
	LPFUNC_DevMon_SetScanParameter DevMon_SetScanParameter;
	LPFUNC_DevMon_SetScanParameterAdj DevMon_SetScanParameterAdj;
	LPFUNC_DevMon_GetScanParameter DevMon_GetScanParameter;
	LPFUNC_DevMon_StartScan DevMon_StartScan;
	LPFUNC_DevMon_ReadScanData DevMon_ReadScanData;
	LPFUNC_DevMon_AbortScan DevMon_AbortScan;
	LPFUNC_DevMon_AbortScanEx DevMon_AbortScanEx;
	LPFUNC_DevMon_Close DevMon_Close;
	//LPFUNC_ReadPushButton ReadPushButton;
	//LPFUNC_UsbInitializeMFP UsbInitializeMFP;
	//LPFUNC_writeESCCommand writeESCCommand;
	LPFUNC_DevMon_BroadcastMFP DevMon_BroadcastMFP;
	LPFUNC_DevMon_DetectNetMFP DevMon_DetectNetMFP;
	LPFUNC_DevMon_LocateMFP DevMon_LocateMFP;
};


#endif // !defined(AFX_WIADEVICE_H__3B404E09_994A_42C7_B36B_EFEE89861E23__INCLUDED_)
