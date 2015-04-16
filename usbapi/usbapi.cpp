// usbapi.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "usbapi.h"
#include "scanner.h"
#include "bitmapScaling.h"

#include <windows.h>
#include <usbprint.h>
#include <stdio.h>
#include <stdlib.h>
#include <tchar.h>
#include <StrSafe.h>
#include <Winspool.h>
#include "Global.h"
#include <winsock.h>
#include <Iphlpapi.h>

#pragma comment(lib, "Ws2_32.lib")
#pragma comment(lib, "Iphlpapi.lib")
//--------------------------------macro---------------------------------------

#define _UnknowJob        0
#define _PrintJob         1
#define _NormalCopyJob    2
#define _ScanJob          3
#define _FaxJob           4
#define _IFaxJob          5
#define _ReportJob        6
#define _Nin1CopyJob      7
#define _IDCardCopyJob    8

#define _SCANMODE_1BIT_BLACKWHITE 0
#define _SCANMODE_8BIT_GRAYSCALE  1
#define _SCANMODE_24BIT_COLOR     2

#define MAX_PIXEL_PREVIEW     100*1024*1024
#define MAX_PIXEL_THUMB       1024*1024

#define _PSAVE_TIME_GET     0x00
#define _PSAVE_TIME_SET     0x01
#define _USER_CONFIG_GET    0x02
#define _USER_CONFIG_SET    0x03
#define _PRN_PASSWD_SET		0x06
#define _PRN_PASSWD_GET		0x07
#define _PRN_PASSWD_COMFIRM	0x08
#define _Fusing_SC_Reset    0x0B

#define     MAGIC_NUM           0x1A2B3C4D

#define  _ACK                          0
#define  _CMD_invalid                  1
#define  _Parameter_invalid            2
#define  _Do_not_support_this_function 3
#define  _Printer_busy                 4
#define  _Printer_error                5
#define  _Set_parameter_error          6
#define  _Get_parameter_error          7
#define  _Printer_is_Sleeping          8

#define  _SW_USB_OPEN_FAIL                11
#define  _SW_USB_ERROR_OTHER              12
#define  _SW_USB_WRITE_TIMEOUT            13
#define  _SW_USB_READ_TIMEOUT             14
#define  _SW_USB_DATA_FORMAT_ERROR        15

#define  _SW_NET_DLL_LOAD_FAIL            21
#define  _SW_NET_DATA_FORMAT_ERROR        22

#define  _SW_UNKNOWN_PORT                 31
#define  _SW_INVALID_PARAMETER            32
#define  _SW_INVALID_RETURN_VALUE         33


#define MAX_SIZE_BUFF  1460 
#define REGKEY_UTILITY_LENOVO_3in1  L"SOFTWARE\\Lenovo\\Lenovo ABC M001\\Express Scan Manager"
#define _COMM_ERROR_NUM				0x2D1C

#define DLL_NAME_NET L"NetIO"

#define PT_UNKNOWN 0
#define PT_TCPIP   1
#define PT_USB     2
#define PT_WSD     3

#define PORT_STD_TCPIP	L"TCPMON.DLL"
#define PORT_STD_USB	L"Dynamic Print Monitor"
#define PORT_STD_WSD	L"WSD Port Monitor"

#define MAX_DEVICEID_LEN	2048

#define        __Ready                      0x00
#define        __Printing                   0x01
#define        __PowerSaving                0x02
#define        __WarmingUp                  0x03
#define        __PrintCanceling             0x04
#define        __Processing                 0x07
#define        __CopyScanning               0x60
#define        __CopyScanNextPage           0x61
#define        __CopyPrinting               0x62
#define        __CopyCanceling              0x63
#define        __IDCardMode                 0x64
#define        __ScanScanning               0x6A
#define        __ScanSending                0x6B
#define        __ScanCanceling              0x6C
#define        __ScannerBusy				0x6D
#define        __TonerEnd                   0x7F
#define        __TonerNearEnd               0x81
#define        __ManualFeedRequired         0x85
#define        __InitializeJam              0xBC
#define        __NofeedJam                  0xBD
#define        __JamAtRegistStayOn          0xBE
#define        __JamAtExitNotReach          0xBF
#define        __JamAtExitStayOn            0xC0
#define        __CoverOpen                  0xC1
#define        __NoTonerCartridge           0xC5
#define        __WasteTonerFull             0xC6
#define        __FWUpdate                   0xC7
#define        __PolygomotorOnTimeoutError  0xD0
#define        __PolygomotorOffTimeoutError 0xD1
#define        __PolygomotorLockSignalError 0xD2
#define        __BeamSynchronizeError       0xD3
#define        __BiasLeak                   0xD4
#define        __MainmotorError             0xD5
#define        __FuserThermistorError       0xD6
#define        __FuserReloadError           0xD7
#define        __HighTemperatureErrorSoft   0xD8
#define        __HighTemperatureErrorHard   0xD9
#define        __FuserFullHeaterError       0xDA
#define        __Fuser3timesJamError        0xDB
#define        __LowVoltageFuserReloadError 0xDC
#define        __MotorThermistorError       0xDD
#define        __EEPROMCommunicationError   0xDE
#define        __CTL_PRREQ_NSignalNoCome    0xDF
#define        __ScanPCUnkownCommandUSB     0xE0
#define        __SCANUSBDisconnect          0xE1
#define        __ScanPCUnkownCommandNET     0xE3
#define        __ScanNETDisconnect          0xE4
#define        __ScanMotorError             0xE5
#define        __Unknown                    0xF0 // status added by SW
#define        __Offline                    0xF1 // status added by SW
#define        __PowerOff                   0xF2 // status added by SW

#define	_DEF_VID	0
#define _DEF_PID	0
//--------------------------------type define---------------------------------

#pragma pack(1)
struct st_printer_reg
{
    wchar_t* sz_printer;
    wchar_t* sz_reg;
}st_printer_reg;


typedef struct st_ap_info 
{
    char  ssid[33];
    UINT8 encryption;
}st_ap_info;

typedef struct 
{
	////////////////////////////////////////////////////
	// Consumable 
	////////////////////////////////////////////////////
	BYTE	TonelStatusLevelK; 	
	BYTE	TonelStatusLevelC; 	
	BYTE	TonelStatusLevelM; 
	BYTE	TonelStatusLevelY; 	
	BYTE	DrumStatusLifeRemain;

	////////////////////////////////////////////////////
	// Covers 
	////////////////////////////////////////////////////
	BYTE	CoverStatusFlags; 

	////////////////////////////////////////////////////
	// Paper Tray
	////////////////////////////////////////////////////
	BYTE	PaperTrayStatus; 	
	BYTE	PaperSize;	

	////////////////////////////////////////////////////
	// Output Tray
	////////////////////////////////////////////////////
	BYTE	OutputTrayLevel; 

	////////////////////////////////////////////////////
	// General Status and information
	////////////////////////////////////////////////////
	BYTE	PrinterStatus;
	wchar_t	OwnerName[16];
	wchar_t	DocuName[16];
	BYTE	ErrorCodeGroup;
	BYTE	ErrorCodeID;
	WORD	PrintingPage;	
	WORD	Copies;
	DWORD	TotalCounter;

	BYTE         reserved[12];

	BYTE	TonerSize[4];
	BYTE	PaperType;
	BYTE	NonDellTonerMode;
	BYTE	AioStatus;
	BYTE	job;
	WORD	wReserved1;
	WORD	wReserved2;
} PRINTER_STATUS;

typedef struct net_info_st
{
    UINT8 IPMode            ; // 0-ipv4,1-ipv6
    UINT8 IPAddressMode     ; // 0 AutoIP,1 BOOTP,2 RARP,3 DHCP,4 Panel (Manual)
    UINT8 IPAddress[4]      ; // 0.0.0.0 ~ 223.255.255.255
    UINT8 SubnetMask[4]     ; // 0.0.0.0 ~ 223.255.255.255
    UINT8 GatewayAddress[4] ; // 0.0.0.0 ~ 223.255.255.255
} net_info_st;

typedef struct _COMM_HEADER
{
	UINT32 magic;
	UINT16 id;
	UINT16 len;     // lenght of data structure and its data

    // data structure
    UINT8 subid;
    UINT8 len2;

    // data append
    UINT8 subcmd;
}COMM_HEADER;

typedef enum _CMD_SUBID
{
	_LS_SUB_RO	= 0,		// RO Limit Address
	_LS_SUB_RW,				// RW Limit Address
	_LS_SUB_ZI,				// ZI Limit Address
	_LS_SUB_TCP,			// TCP Port
	_LS_SUB_MAC,			// MAC Address
	_LS_SUB_MDL,			// Machine Model String
	_LS_SUB_VER,			// FW Version String
	_LS_SUB_PAR,			// Param address, for func call or var show/modify
	_LS_SUB_OFS,			// Data xfer, offset of the data
	_LS_SUB_REM,			// Data xfer, remain size
	_LS_SUB_KID,			// Panel Key ID
	_LS_SUB_KTM,			// Panel Key re-simulate times
	_LS_SUB_ACK,			// ACK in the command comm
	_LS_SUB_NAK,			// NAK in the command comm
	_LS_SUB_LED,			// Panel Led ID + Status
	_LS_SUB_RAM,			// Upload/Download RAM start offset
	_LS_SUB_ROM,			// Upload/Download ROM start offset
	_LS_SUB_LEN,			// Upload/Download RAM/ROM length
	_LS_SUB_ZIP, 			// Is data compressed (zipped)
	_LS_SUB_ID				// Legacy WiFi & P2P commands or Configuration
}CMD_SUBID;

typedef enum _CMD_ID
{
	_LS_SEARCH		= 0x0000, 	// Search Machine
	_LS_CONNECT		= 0x0001,	// Connect Machine
	_LS_DISCONNECT	= 0x0002,	// Disconnect Machine
	_LS_CALLFUNC	= 0x0100,	// Call Function
	_LS_SHOWVAR		= 0x0101, 	// Show Variable/Structure
	_LS_MODIFYVAR	= 0x0102,	// Modify Variable/Structure
	_LS_PRIVEXEC	= 0x0103,	// Private Execution
	_LS_ENGCMD		= 0x0104,	// Engine Command
	_LS_NETCMD		= 0x0105,	// Network Command
	_LS_WIFICMD		= 0x0106,	// Wireless Command
	_LS_PRNCMD		= 0x0107,	// Print Command
	_LS_SCACMD		= 0x0108,	// Scan Command
	_LS_CPYCMD		= 0x0109,	// Copy Command
	_LS_FAXCMD		= 0x010A,	// Fax Command
	_LS_DBGMSG		= 0x0200, 	// Debug Message
	_LS_HEARTBEAT 	= 0x0201,	// Heart Beat, Null Packet, keep activated
	_LS_PANKEY		= 0x0300,	// Panel Key Simulation
	_LS_PANIMG		= 0x0301,	// Panel Frame & LED status
	_LS_DATADOWN	= 0x0400,	// Download Data
	_LS_DATAUPLD	= 0x0401	// Upload Data
}CMD_ID;

typedef struct _copycmdset
{
        UINT8 Density         ; // 0  -   0~6
        UINT8 copyNum         ; // 1  -   1~99
        UINT8 scanMode        ; // 2  -   0: Photo, 1: Text, 2: ID card
        UINT8 orgSize         ; // 3  -   0: A4, 1: A5, 2: B5, 3: Letter, 4: Executive
        UINT8 paperSize       ; // 4  -   0: Letter, 1: A4, 2: A5, 3: A6, 4: B5, 5: B6, 6: Executive, 7: 16K
        UINT8 nUp             ; // 5  -   0:1up, 1: 2up, 3: 4up, 4: 9up
        UINT8 dpi             ; // 6  -   0: 300*300, 1: 600*600
        UINT8 mediaType       ; // 7  -   0: plain paper 1: Recycled paper 2: Thick paper 3: Thin paper 4: Label
        UINT16 scale          ; // 8  -   25~400, disabled for 2/4/9up
} copycmdset;

typedef struct cmdst_userconfig
{
    INT8 LeadingEdge     ; // Leading Edge Registration 
    INT8 SideToSide      ; // Side-to-size Registration 
    INT8 ImageDensity    ; // Image Desity              
    INT8 LowHumidityMode ; // Low Humidity Mode     
	INT8 PlateControlMode;
	INT8 PrimaryCoolingMode;
} cmdst_userconfig;

typedef struct cmdst_softap
{
    UINT8 wifiEnable ; // bit0: Wi-Fi Enable, bit1: GO Enable, bit2: P2P Enable
    UINT8 reserved[7]; //
    char  ssid[32]   ; // used by both Legacy WiFi SSID and Wi-Fi Direct GO SSID
    char  pwd[64]    ; // used by both Legacy WiFi Passphrase & WEPKey and Wi-Fi Direct GO Passphrase
} cmdst_softap;

typedef struct cmdst_wifi_get
{
    UINT8 wifiEnable ; // bit0: Wi-Fi Enable, bit1: GO Enable, bit2: P2P Enable
    UINT8 sigLevel   ; //
    UINT8 reserved0  ; //
    UINT8 netType    ; // lenovo always 0
    UINT8 encryption ; // 0:No Security 1:WEP 64/128 bit 2.WPA-PSK-TKIP  3. WPA2-PSK-AES 4.Mixed Mode PSK
    UINT8 wepKeyId   ; //
    UINT8 reserved1  ; //
    UINT8 channel    ; //
    char  ssid[32]   ; // used by both Legacy WiFi SSID and Wi-Fi Direct GO SSID
    char  pwd[64]    ; // used by both Legacy WiFi Passphrase & WEPKey and Wi-Fi Direct GO Passphrase
    char  pinCode[8] ; //
    UINT8 reserved2[64]; //
    UINT8 ipAddr[4]  ; //
}cmdst_wifi_get;

typedef struct cmdst_wifi_set
{
    UINT8 wifiEnable ; // bit0: Wi-Fi Enable, bit1: GO Enable, bit2: P2P Enable
    UINT8 reserved0[2]; //
    UINT8 netType    ; //
    UINT8 encryption ; //
    UINT8 wepKeyId   ; //
	UINT8 reserved1; //
	UINT8 channel; //
	char  ssid[32]; // used by both Legacy WiFi SSID and Wi-Fi Direct GO SSID
    char  pwd[64]    ; // used by both Legacy WiFi Passphrase & WEPKey and Wi-Fi Direct GO Passphrase
}cmdst_wifi_set;

typedef int (* LPFN_NETWORK_CONNECT ) (char *server, int port, int timeout ) ;
// return the number of bytes successfully read.
typedef int (* LPFN_NETWORK_READ    ) (int sd, void* buff, DWORD len       ) ;
typedef int (* LPFN_NETWORK_WRITE   ) (int sd, void* buff, DWORD len       ) ;
typedef void(* LPFN_NETWORK_CLOSE   ) (int sd                              ) ;
typedef int (__cdecl *LPFNNETWORKREADSTATUSEX) (char *server, char *community, PRINTER_STATUS *status, char* pMfg, char* pMdl);

//--------------------------------declare-internal----------------------------
static bool DoseHasEnoughSpace(
        const wchar_t* szPath,
        int nWidth,
        int nHeight 
        );

static bool CreateFileWithSize(
        const wchar_t* pszPath,
        LONG size 
        );

static int GetByteNumPerLineWidthPad( int scanMode, int nPixels );
static void FillBitmapHeader(BYTE* pBuffer, int nScanMode, int nWidth, int nHeight, int resolution, double rate, ULONG* ptrActualSize);
static double GetScalingRate( UINT32 uMax, UINT32 uInput );
static bool GetDevMonPath( const wchar_t* sz_printer, LPTSTR lptstrDevMonPath, int nSize);
static bool get_name_driver( const wchar_t* sz_printer, wchar_t* sz_driver, int size );
static int _base64_char_value(char base64char);
static int _base64_decode_triple(char quadruple[4], unsigned char *result);
static size_t Base64Decode(char *source, unsigned char *target, size_t targetlen);
static BOOL DecodeStatusFromDeviceID(char* device_id, PRINTER_STATUS* status);
static BOOL GetPortName( const wchar_t *szPrinterName, wchar_t *szPortName, int iLen);
static int WriteDataViaUSB( const wchar_t* szPrinter, char* ptrInput, int cbInput, char* ptrOuput, int cbOutput );
static int WriteDataViaNetwork( const wchar_t* szIP, char* ptrInput, int cbInput, char* ptrOutput, int cbOutput );
static int CheckPort( const wchar_t* pprintername_, wchar_t* str_ );

//--------------------------------declare-external----------------------------
USBAPI_API int __stdcall CheckPortAPI( const wchar_t* szPrinter );

USBAPI_API int __stdcall ScanEx( const wchar_t* sz_printer,
        const wchar_t* szOrig,
        const wchar_t* szView,
        const wchar_t* szThumb,
        int scanMode,
        int resolution,
        int width,
        int height,
        int contrast,
        int brightness,
        int docutype,
        UINT32 uMsg );

USBAPI_API int __stdcall GetPowerSaveTime( const wchar_t* szPrinter, BYTE* ptrTime );
USBAPI_API int __stdcall SetPowerSaveTime( const wchar_t* szPrinter, BYTE time );
USBAPI_API int __stdcall GetWiFiInfo(const wchar_t* szPrinter, UINT8* ptr_wifienable, char* ssid, char* pwd, UINT8* ptr_encryption, UINT8* ptr_wepKeyId);

USBAPI_API int __stdcall GetIPInfo( 
        const wchar_t* szPrinter,
        BYTE* ptr_mode_ipversion,
        BYTE* ptr_mode_ipaddress,

        BYTE* ptr_ip0,
        BYTE* ptr_ip1,
        BYTE* ptr_ip2,
        BYTE* ptr_ip3,
        
        BYTE* ptr_mask0,
        BYTE* ptr_mask1,
        BYTE* ptr_mask2,
        BYTE* ptr_mask3,

        BYTE* ptr_gate0,
        BYTE* ptr_gate1,
        BYTE* ptr_gate2,
        BYTE* ptr_gate3);


USBAPI_API int __stdcall GetApList( const wchar_t* szPrinter, 
        char* pssid0,  BYTE* ptr_encryption0,
        char* pssid1,  BYTE* ptr_encryption1,
        char* pssid2,  BYTE* ptr_encryption2,
        char* pssid3,  BYTE* ptr_encryption3,
        char* pssid4,  BYTE* ptr_encryption4,
        char* pssid5,  BYTE* ptr_encryption5,
        char* pssid6,  BYTE* ptr_encryption6,
        char* pssid7,  BYTE* ptr_encryption7,
        char* pssid8,  BYTE* ptr_encryption8,
        char* pssid9,  BYTE* ptr_encryption9 );

USBAPI_API int __stdcall SendCopyCmd( const wchar_t* szPrinter, UINT8 Density, UINT8 copyNum, UINT8 scanMode, UINT8 orgSize, UINT8 paperSize, UINT8 nUp, UINT8 dpi, UINT16 scale, UINT8 mediaType );
USBAPI_API int __stdcall SetWiFiInfo(const wchar_t* szPrinter, UINT8 wifiEnable, UINT8 wifichangeflag, const wchar_t* ws_ssid, const wchar_t* ws_pwd, UINT8 encryption, UINT8 wepKeyId);
USBAPI_API int __stdcall SetSoftAp( const wchar_t* szPrinter, const wchar_t* ws_ssid, const wchar_t* ws_pwd, bool isEnableSoftAp );
USBAPI_API int __stdcall GetSoftAp( const wchar_t* szPrinter, char* ssid, char* pwd, BYTE* ptr_wifi_enable  );
USBAPI_API int __stdcall GetUserCfg(const wchar_t* szPrinter, BYTE* ptr_leadingedge, BYTE* ptr_sidetoside, BYTE* ptr_imagedensity, BYTE* ptr_lowhumiditymode, BYTE* ptr_platecontrolmode, BYTE* ptr_primarycoolingmode);
USBAPI_API int __stdcall SetFusingSCReset(const wchar_t* szPrinter);
USBAPI_API int __stdcall SetUserCfg(const wchar_t* szPrinter, UINT8 LeadingEdge, UINT8 SideToSide, UINT8 ImageDensity, UINT8 LowHumidityMode, UINT8 PlateControlMode, UINT8 PrimaryCoolingMode);
USBAPI_API bool __stdcall GetPrinterStatus( const wchar_t* szPrinter, BYTE* ptr_status, BYTE* ptr_toner, BYTE* pJob );
USBAPI_API int __stdcall SetIPInfo( 
        const wchar_t* szPrinter,
        BYTE* ptr_mode_ipversion,
        BYTE* ptr_mode_ipaddress,

        BYTE* ptr_ip0,
        BYTE* ptr_ip1,
        BYTE* ptr_ip2,
        BYTE* ptr_ip3,
        
        BYTE* ptr_mask0,
        BYTE* ptr_mask1,
        BYTE* ptr_mask2,
        BYTE* ptr_mask3,

        BYTE* ptr_gate0,
        BYTE* ptr_gate1,
        BYTE* ptr_gate2,
        BYTE* ptr_gate3);

USBAPI_API int __stdcall ConfirmPassword(const wchar_t* szPrinter, const wchar_t* ws_pwd);
USBAPI_API int __stdcall GetPassword(const wchar_t* szPrinter, char* pwd);
USBAPI_API int __stdcall SetPassword(const wchar_t* szPrinter, const wchar_t* ws_pwd);

USBAPI_API void __stdcall CancelScanning();
//--------------------------------global--------------------------------------
static const unsigned char INIT_VALUE = 0xfe;
static bool bCancelScanning = false; // Scanning cancel falg, only use in ScanEx(). 

//--------------------------------implement-----------------------------------
static BOOL GetPrinterPortName(wchar_t *pPrinterName, wchar_t* portName, size_t portName_len)
{
	HANDLE		hPrinter;
	PRINTER_INFO_2	*info2;
	DWORD		need2;

	if (pPrinterName==NULL || portName==NULL || !OpenPrinter(pPrinterName,&hPrinter,NULL))
    {
		return FALSE;
    }

	GetPrinter(hPrinter,2,NULL,0,&need2);
	info2 = (PRINTER_INFO_2*)new BYTE[need2];
	if (!GetPrinter(hPrinter,2,(LPBYTE)info2,need2,&need2)) {
		ClosePrinter(hPrinter);
		delete []info2;
		return FALSE;
	} else {
		wcscpy_s (portName,portName_len,info2->pPortName);
		delete []info2;
		return TRUE;
	}
}

static BOOL USBGetSymbolicNameByPortEx(wchar_t *pPortName, WORD wVid, WORD wPid, wchar_t* symbolicname, size_t symbolicname_len)
{
	HKEY	RegKeyDevClassPrn;
	HKEY	RegKeyDevParam;
	wchar_t	*prnClass=L"SYSTEM\\CurrentControlSet\\Control\\DeviceClasses\\{28d78fad-5a12-11d1-ae5b-0000f803a8c2}";
	wchar_t	tempKey1[512];
	wchar_t	tempKey2[512];	
	wchar_t	keyPrefix[64];
	DWORD	nKeyPrefixLen;

	int	nPortNum=-1;

	if (symbolicname==NULL || wcsncmp(pPortName,L"USB",3))
		return FALSE;
	swscanf_s(pPortName+3,L"%d",&nPortNum);
	if (nPortNum<0)
		return FALSE;
	if (wVid && wPid) {
	} else {
		keyPrefix[0] = L'\0';
	}
	nKeyPrefixLen = wcslen(keyPrefix);

	if (RegOpenKeyEx(HKEY_LOCAL_MACHINE,prnClass,0,KEY_ENUMERATE_SUB_KEYS|KEY_QUERY_VALUE,&RegKeyDevClassPrn)!=ERROR_SUCCESS)  {
		return FALSE;
	}
	int iIndex1 = 0;
	BOOL bFound = FALSE;
	while (TRUE) {
		DWORD	subkeysize = 512;
		DWORD eResult=RegEnumKeyEx(RegKeyDevClassPrn,  iIndex1++, tempKey1, &subkeysize, NULL, NULL, NULL, NULL);
		if (eResult==ERROR_NO_MORE_ITEMS) {
			break;
		} else if (eResult==ERROR_SUCCESS) {
			if (nKeyPrefixLen && _wcsnicmp(tempKey1,keyPrefix,nKeyPrefixLen))
				continue;
			// check port number
			wcscpy_s(tempKey2, 512, prnClass);
			wcscat_s(tempKey2,L"\\");
			wcscat_s(tempKey2,tempKey1);
			wcscat_s(tempKey2,L"\\#\\Device Parameters");

			if (RegOpenKeyEx (HKEY_LOCAL_MACHINE,tempKey2,0,KEY_QUERY_VALUE,&RegKeyDevParam)!=ERROR_SUCCESS)  {
				continue;			
			}
			DWORD	PortNumSize = sizeof(DWORD);
			DWORD	nTempPortNum;
			if (RegQueryValueEx(RegKeyDevParam,L"Port Number",NULL,NULL,(LPBYTE)&nTempPortNum,&PortNumSize)!=ERROR_SUCCESS)  {
				RegCloseKey(RegKeyDevParam);
				continue;
			}
			RegCloseKey(RegKeyDevParam);
			if (nTempPortNum==nPortNum) { // Found specified port
				bFound = TRUE;
				wcscpy_s(symbolicname,symbolicname_len, tempKey1);
				symbolicname[0] = L'\\';
				symbolicname[1] = L'\\';
				symbolicname[2] = L'.';
				symbolicname[3] = L'\\';
				break;
			}			
			continue;
		} else {
			break;
		}
	}
	RegCloseKey(RegKeyDevClassPrn);
	if (bFound)
		return TRUE;
	else	return FALSE;
}

static int CheckPort( const wchar_t* pprintername_, wchar_t* str_ )
{
    wchar_t pprintername[MAX_PATH];
    wcscpy_s(pprintername, MAX_PATH, pprintername_ );

	HANDLE hPrinter = NULL;
	PRINTER_DEFAULTS Defaults = { NULL, NULL, PRINTER_ACCESS_USE};
	if (!OpenPrinter(pprintername, &hPrinter, &Defaults) || hPrinter == NULL)
        return PT_UNKNOWN;

	DWORD		cbNeeded,cReturned, dwStatus, cbBuf;
	PORT_INFO_2	*port;
	PRINTER_INFO_2	*pPrinterInfo ;

	if (hPrinter==NULL)
		return PT_UNKNOWN;
	cbNeeded = 0;
	if (GetPrinter(hPrinter, 2, NULL, 0, &cbNeeded)==TRUE || cbNeeded==0)
		return PT_UNKNOWN;
	pPrinterInfo = (PRINTER_INFO_2*)malloc(cbNeeded);
	if(!pPrinterInfo)
		return PT_UNKNOWN;
	if (!GetPrinter(hPrinter, 2, (PBYTE) pPrinterInfo, cbNeeded, &cReturned))	{
		free(pPrinterInfo); 
		return PT_UNKNOWN;
	}	

	cbNeeded = 0;
	if (EnumPorts(NULL, 2, (LPBYTE)NULL, 0, &cbNeeded, &cReturned)==TRUE) {
		free(pPrinterInfo); 
		return PT_UNKNOWN;
	}
	if (GetLastError()!=ERROR_INSUFFICIENT_BUFFER) {
		free(pPrinterInfo); 
		return PT_UNKNOWN;
	}
	cbBuf = cbNeeded;
	port = (PORT_INFO_2 *)malloc(cbBuf);
	if (EnumPorts(NULL, 2, (LPBYTE)port, cbBuf, &cbNeeded, &cReturned)==FALSE) {
		free(pPrinterInfo); 
		free(port);
		return PT_UNKNOWN;
	}

	int ptType = PT_UNKNOWN;
	for(int i=0; i< cReturned; i++)
    {
		if ( 0 == wcscmp(pPrinterInfo->pPortName, port[i].pPortName) )
        {
            if (port[i].pMonitorName==NULL)
                break;
            else if(!wcscmp(port[i].pMonitorName, PORT_STD_USB)) 
                ptType = PT_USB;
            else if (!wcscmp(port[i].pMonitorName, PORT_STD_TCPIP)) 
                ptType = PT_TCPIP;
            // else if (IsWSDPort(port[i].pMonitorName)) 
            //     ptType = PT_WSD;
            else
                break;

            if ( NULL != str_ && ( PT_TCPIP == ptType || PT_WSD == ptType ) )
            {
                wchar_t szPrinterName[2*MAX_PATH];

                wcscpy_s(szPrinterName, 2*MAX_PATH, TEXT(",XcvPort "));
                wcscat_s(szPrinterName, 2*MAX_PATH, pPrinterInfo->pPortName);

                HANDLE hXcv = NULL;
                PRINTER_DEFAULTS Defaults = { NULL, NULL, SERVER_READ};
                if (OpenPrinter(szPrinterName, &hXcv, &Defaults )) 
                {
                    XcvData(hXcv, L"IPAddress", NULL, 0, (PBYTE)str_, 256, &cReturned, &dwStatus);
                    ClosePrinter(hXcv);
                }
            }
        }
	}

	free(pPrinterInfo);
	free(port);	

	return ptType;	
}


USBAPI_API int __stdcall SendCopyCmd( const wchar_t* szPrinter, UINT8 Density, UINT8 copyNum, UINT8 scanMode, UINT8 orgSize, UINT8 paperSize, UINT8 nUp, UINT8 dpi, UINT16 scale, UINT8 mediaType )
{
    if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;

    int nResult = _ACK;
    wchar_t szIP[MAX_PATH] = { 0 };

    int nPortType = CheckPort( szPrinter, szIP );

    if ( PT_UNKNOWN == nPortType ) 
    {
        nResult = _SW_UNKNOWN_PORT;
    }
    else
    {
        char* buffer = new char[sizeof(COMM_HEADER)+128];
        memset( buffer, INIT_VALUE, sizeof(COMM_HEADER)+128 );
        COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>( buffer );

        ppkg->magic = MAGIC_NUM ;
        ppkg->id = _LS_CPYCMD;
        ppkg->len = 3+128;

        // For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
        // it just stand for the sub id. The real data length is defined by the lib
        ppkg->subid = 0x13; // copy set command
        ppkg->len2 = 1;
        ppkg->subcmd = 1;   // set copy command

        copycmdset* pcopycmd = reinterpret_cast<copycmdset*>( buffer+sizeof(COMM_HEADER));

        pcopycmd->Density   = Density;
        pcopycmd->copyNum   = copyNum;
        pcopycmd->scanMode  = scanMode;
        pcopycmd->orgSize   = orgSize;
        pcopycmd->paperSize = paperSize;
        pcopycmd->nUp       = nUp;
        pcopycmd->dpi       = dpi;
        pcopycmd->scale     = scale;
        pcopycmd->mediaType = mediaType;

        if ( PT_TCPIP == nPortType || PT_WSD == nPortType )
        {
            nResult = WriteDataViaNetwork( szIP, buffer, sizeof(COMM_HEADER)+128, NULL, 0 );
        }
        else if ( PT_USB == nPortType )
        {
            nResult = WriteDataViaUSB( szPrinter, buffer, sizeof(COMM_HEADER)+128, NULL, 0 );
        }

        if ( buffer )
        {
            delete[] buffer;
            buffer = NULL;
        }
    }

    return nResult;
}

static int WriteDataViaNetwork( const wchar_t* szIP, char* ptrInput, int cbInput, char* ptrOutput, int cbOutput )
{
    int nResult = _ACK;

    HMODULE hmod = LoadLibrary( DLL_NAME_NET );

    LPFN_NETWORK_CONNECT  lpfnNetworkConnect   = NULL;
    LPFN_NETWORK_READ     lpfnNetworkRead      = NULL;
    LPFN_NETWORK_WRITE    lpfnNetworkWrite     = NULL;
    LPFN_NETWORK_CLOSE    lpfnNetworkClose     = NULL;

    lpfnNetworkConnect = ( LPFN_NETWORK_CONNECT    ) GetProcAddress( hmod , "NetworkConnectNonBlock" );
    lpfnNetworkRead    = ( LPFN_NETWORK_READ       ) GetProcAddress( hmod , "NetworkRead"            );
    lpfnNetworkWrite   = ( LPFN_NETWORK_WRITE      ) GetProcAddress( hmod , "NetworkWrite"           );
    lpfnNetworkClose   = ( LPFN_NETWORK_CLOSE      ) GetProcAddress( hmod , "NetworkClose"           );

    if ( hmod && \
            lpfnNetworkConnect && \
            lpfnNetworkRead    && \
            lpfnNetworkWrite   && \
            lpfnNetworkClose )
    {
		int nCount = 0;
		bool bWriteSuccess = false;
		while (nCount++ < 1 && !bWriteSuccess)
		{
			char szAsciiIP[1024] = { 0 };
			::WideCharToMultiByte(CP_ACP, 0, szIP, -1, szAsciiIP, 1024, NULL, NULL);

			//int nPingCount = 0;
			//bool bPingSuccess = true;
			//while (nPingCount++ < 6)
			//{
			//	if (!CheckIPReachable(szAsciiIP))
			//	{
			//		bPingSuccess = false;
			//		continue;
			//	}
			//	else
			//	{
			//		bPingSuccess = true;
			//		break;
			//	}
			//}

			//if (!bPingSuccess)
			//{
			//	nResult = _SW_NET_DATA_FORMAT_ERROR;
			//	break;
			//}

			int m_iSocketID = lpfnNetworkConnect(szAsciiIP, 9100, 500);
			lpfnNetworkWrite(m_iSocketID, ptrInput, cbInput);

			if (ptrOutput && cbOutput > 0)
			{
				if (cbOutput == lpfnNetworkRead(m_iSocketID, ptrOutput, cbOutput))
				{
					nResult = _ACK;
					bWriteSuccess = true;
					OutputDebugStringToFileA("\r\n####VP:WriteDataViaNetwork(): wirte data success. (nResult == _ACK)");
				}
				else
				{
					nResult = _SW_NET_DATA_FORMAT_ERROR;
					OutputDebugStringToFileA("\r\n####VP:WriteDataViaNetwork(): data format error.");
//					Sleep(200);
				}
			}
			else
			{
				COMM_HEADER cmdHeader = {0};
				if (sizeof(COMM_HEADER) == lpfnNetworkRead(m_iSocketID, &cmdHeader, sizeof(COMM_HEADER))
					&& cmdHeader.magic == MAGIC_NUM)
				{
					nResult = cmdHeader.subcmd;
					bWriteSuccess = true;
					OutputDebugStringToFileA("\r\n####VP:WriteDataViaNetwork(): wirte data success. magic = 0x%x  id = 0x%x len = %u subid = 0x%x len2 = %u subcmd = %u", \
						cmdHeader.magic, cmdHeader.id, cmdHeader.len, cmdHeader.subid, cmdHeader.len2, cmdHeader.subcmd);
				}
				else
				{
					nResult = _SW_NET_DATA_FORMAT_ERROR;
					OutputDebugStringToFileA("\r\n####VP:WriteDataViaNetwork(): data format error. magic = 0x%x  id = 0x%x len = %u subid = 0x%x len2 = %u subcmd = %u" ,\
						cmdHeader.magic, cmdHeader.id, cmdHeader.len, cmdHeader.subid, cmdHeader.len2, cmdHeader.subcmd);
				}
			}

			lpfnNetworkClose(m_iSocketID);
		}
    }
    else
    {
        nResult = _SW_NET_DLL_LOAD_FAIL;
		OutputDebugStringToFileA("\r\n####VP:WriteDataViaNetwork(): Load dll fail.");
	}

    lpfnNetworkConnect = NULL;
    lpfnNetworkRead    = NULL;
    lpfnNetworkWrite   = NULL;
    lpfnNetworkClose   = NULL;

    FreeLibrary( hmod );

    return nResult;
}

static int WriteDataViaUSB( const wchar_t* szPrinter, char* ptrInput, int cbInput, char* ptrOuput, int cbOutput )
{
    static char buffMax[MAX_SIZE_BUFF];
	OutputDebugStringToFileA("\r\n####VP:WriteDataViaUSB() begin");
    int nResult = _ACK;

    wchar_t pszPort[MAX_PATH]         = { 0 };
    wchar_t pSymbolname[MAX_PATH]     = { 0 };
    wchar_t plocprintername[MAX_PATH] = { 0 };

    wcscpy_s( plocprintername, MAX_PATH, szPrinter );

    if ( GetPrinterPortName( plocprintername, pszPort, MAX_PATH ) )
    {
		int nCount = 0;
		bool bWriteSuccess = false;
		while (nCount++ < 5 && !bWriteSuccess)
		{
			USBGetSymbolicNameByPortEx(pszPort, 0, 0, pSymbolname, MAX_PATH);
			HANDLE ctlPipe = CreateFile(pSymbolname, GENERIC_WRITE | GENERIC_READ, FILE_SHARE_WRITE | FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, NULL);

			if (INVALID_HANDLE_VALUE != ctlPipe)
			{
				char   inBuf[15];
				char	outBuf[MAX_DEVICEID_LEN] = { 0 };
				unsigned long     inBufSize = sizeof(inBuf);
				unsigned long     outBufSize = sizeof(outBuf);
				unsigned long     tmpBytes = 0;

				DWORD dwActualSize = 0;
				int nWriteTry = 20;
			
				int rc = DeviceIoControl(ctlPipe,
					//IOCTL_GET_DEVICE_ID,
					IOCTL_USBPRINT_GET_1284_ID,
					inBuf,
					inBufSize,
					outBuf,
					outBufSize,
					&tmpBytes,
					NULL);

				if (!rc)
				{
					TCHAR szMutexName[512] = { 0 };
					wsprintf(szMutexName, L"Global\\LT%c-Port-%s", L'C', pszPort);
					HANDLE hAccessMutex = CreateMutex(NULL, TRUE, szMutexName);
					if (hAccessMutex != NULL && GetLastError() != ERROR_ALREADY_EXISTS)
					{
						unsigned char inBuffer[522] = { 0 };
						unsigned char outBuffer[12] = { 0 };

						DWORD dwWritten = 0;

						memset(inBuffer, 0, sizeof(inBuffer));

						inBuffer[0] = 0x1B;
						inBuffer[1] = 0x4D;
						inBuffer[2] = 0x53;
						inBuffer[3] = 0x55;
						inBuffer[4] = 0xE0;
						inBuffer[5] = 0x2B;

						WriteFile(ctlPipe, inBuffer, 10, &dwWritten, NULL);
						WriteFile(ctlPipe, &inBuffer[10], 512, &dwWritten, NULL);

						// acorrding the mail from Gerard:
						// " The reply of wakeup cmd is defined in Toolbox cmd spec,
						// 12 bytes in all, 1c 00 e0 2b  00 00 00 00  00 00 00 00
						// ". We read the "Print Bulk-in" package.

						ReadFile(ctlPipe, outBuffer, sizeof(outBuffer), &dwWritten, NULL);
						CloseHandle(hAccessMutex);
						ReleaseMutex(hAccessMutex);
					}
				}

				DeviceIoControl(ctlPipe, IOCTL_USBPRINT_VENDOR_GET_COMMAND, buffMax, 0, buffMax, 0x3FF, &dwActualSize, NULL);

				while (0 == DeviceIoControl(ctlPipe, IOCTL_USBPRINT_VENDOR_SET_COMMAND, ptrInput, cbInput, NULL, 0, &dwActualSize, NULL)
					&& nWriteTry--)
				{
					Sleep(200);
				}

				if (nWriteTry > 0)
				{
					int nErrCnt = 0;
					memset(buffMax, INIT_VALUE, sizeof(buffMax));
					while (DeviceIoControl(ctlPipe, IOCTL_USBPRINT_VENDOR_GET_COMMAND, buffMax, 0, buffMax, sizeof(buffMax), &dwActualSize, NULL))
					{
						if (buffMax[0] == 0x1C && buffMax[1] == 0x2D) // sync info
						{
							if (20 < nErrCnt++)
							{
								nResult = _SW_USB_READ_TIMEOUT;
								break;
							}
							Sleep(100);
						}
						else if (dwActualSize > 0 && (NULL == ptrOuput || 0 >= cbOutput)) // return info for setting command
						{
							const COMM_HEADER* ptrRet = reinterpret_cast<const COMM_HEADER*>(buffMax);

							if (ptrRet->magic == MAGIC_NUM)
							{
								bWriteSuccess = true;
								nResult = ptrRet->subcmd;
							}
							else
								nResult = _SW_USB_DATA_FORMAT_ERROR;

							break;
						}
						else if (dwActualSize > 0 && INIT_VALUE != buffMax[0])    // return info for getting command
						{
							nResult = _ACK;
							break;
						}
						else
						{
							nResult = _SW_USB_ERROR_OTHER;
							break;
						}
					}
				}
				else
				{
					OutputDebugStringToFileA("\r\n####VP:WriteDataViaUSB(): write usb timeout.");
					nResult = _SW_USB_WRITE_TIMEOUT;
				}

				OutputDebugStringToFileA("\r\n####VP:WriteDataViaUSB(): nCount[%d] nResult [%d]", nCount, nResult);

				if (_ACK == nResult
					&& NULL != ptrOuput
					&& 0 < cbOutput
					&& sizeof(buffMax) > cbOutput)
				{
					const COMM_HEADER* ptrCmdIn = reinterpret_cast<const COMM_HEADER*>(ptrInput);
					const COMM_HEADER* ptrCmdOut = reinterpret_cast<const COMM_HEADER*>(buffMax);

					if (ptrCmdOut->magic == MAGIC_NUM && ptrCmdIn->subid == ptrCmdOut->subid)
					{
						memcpy(ptrOuput, buffMax, cbOutput);
						nResult = _ACK;
						bWriteSuccess = true;
						OutputDebugStringToFileA("\r\n####VP:WriteDataViaUSB(): wirte data success. magic = 0x%x  id = 0x%x len = %u subid = 0x%x len2 = %u subcmd = %u", \
							ptrCmdOut->magic, ptrCmdOut->id, ptrCmdOut->len, ptrCmdOut->subid, ptrCmdOut->len2, ptrCmdOut->subcmd);
					}
					else
					{
						OutputDebugStringToFileA("\r\n####VP:WriteDataViaUSB(): data format error. magic = 0x%x  id = 0x%x len = %u subid = 0x%x len2 = %u subcmd = %u", \
							ptrCmdOut->magic, ptrCmdOut->id, ptrCmdOut->len, ptrCmdOut->subid, ptrCmdOut->len2, ptrCmdOut->subcmd);
						nResult = _SW_USB_DATA_FORMAT_ERROR;
					}
				}

				CloseHandle(ctlPipe);
			}
			else
			{
				nResult = _SW_USB_OPEN_FAIL;
				OutputDebugStringToFileA("\r\n####VP:WriteDataViaUSB(): open usb fail.");
			}

			if (!bWriteSuccess)
				Sleep(200);
		}
    }
    else
    {
        nResult = _SW_USB_ERROR_OTHER;
		OutputDebugStringToFileA("\r\n####VP:WriteDataViaUSB(): GetPrinterPortName error.");
	}
	OutputDebugStringToFileA("\r\n####VP:WriteDataViaUSB(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:WriteDataViaUSB() end");
    return nResult;
}

USBAPI_API int __stdcall SetIPInfo( 
        const wchar_t* szPrinter,
        BYTE* ptr_mode_ipversion,
        BYTE* ptr_mode_ipaddress,

        BYTE* ptr_ip0,
        BYTE* ptr_ip1,
        BYTE* ptr_ip2,
        BYTE* ptr_ip3,
        
        BYTE* ptr_mask0,
        BYTE* ptr_mask1,
        BYTE* ptr_mask2,
        BYTE* ptr_mask3,

        BYTE* ptr_gate0,
        BYTE* ptr_gate1,
        BYTE* ptr_gate2,
        BYTE* ptr_gate3)
{
    if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;

	OutputDebugStringToFileA("\r\n####VP:SetIPInfo() begin");

	int nResult = _ACK;
    wchar_t szIP[MAX_PATH];
    int nPortType = CheckPort( szPrinter, szIP );

    if ( PT_UNKNOWN == nPortType ) 
    {
        nResult = _SW_UNKNOWN_PORT;
    }
    else
    {
        char* buffer = new char[sizeof(COMM_HEADER)+128];
        memset( buffer, INIT_VALUE, sizeof(COMM_HEADER)+128 );
        COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>( buffer );

        ppkg->magic = MAGIC_NUM ;
        ppkg->id = _LS_NETCMD;
        ppkg->len = 3+128;

        // For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
        // it just stand for the sub id. The real data length is defined by the lib
        ppkg->subid = 0x13; 
        ppkg->len2 = 1;

        net_info_st* ptr_net_info = reinterpret_cast<net_info_st*>( buffer+sizeof(COMM_HEADER));

        ppkg->subcmd = 1;   // _NET_SETV4 0x01

        ptr_net_info->IPMode            = *ptr_mode_ipversion;
        ptr_net_info->IPAddressMode     = *ptr_mode_ipaddress;
        ptr_net_info->IPAddress[0]      = *ptr_ip0           ;
        ptr_net_info->IPAddress[1]      = *ptr_ip1           ;
        ptr_net_info->IPAddress[2]      = *ptr_ip2           ;
        ptr_net_info->IPAddress[3]      = *ptr_ip3           ;
        ptr_net_info->SubnetMask[0]     = *ptr_mask0         ;
        ptr_net_info->SubnetMask[1]     = *ptr_mask1         ;
        ptr_net_info->SubnetMask[2]     = *ptr_mask2         ;
        ptr_net_info->SubnetMask[3]     = *ptr_mask3         ;
        ptr_net_info->GatewayAddress[0] = *ptr_gate0         ;
        ptr_net_info->GatewayAddress[1] = *ptr_gate1         ;
        ptr_net_info->GatewayAddress[2] = *ptr_gate2         ;
        ptr_net_info->GatewayAddress[3] = *ptr_gate3         ;

        if ( PT_TCPIP == nPortType || PT_WSD == nPortType )
        {
            nResult = WriteDataViaNetwork( szIP, buffer, sizeof(COMM_HEADER)+128, NULL, 0 );
        }
        else if ( PT_USB == nPortType )
        {
            nResult = WriteDataViaUSB( szPrinter, buffer, sizeof(COMM_HEADER)+128, NULL, 0 );
        }

        if ( buffer )
        {
            delete[] buffer;
            buffer = NULL;
        }
    }
	OutputDebugStringToFileA("\r\n####VP:SetIPInfo(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:SetIPInfo() end");
    return nResult;
}

USBAPI_API bool __stdcall GetPrinterStatus( const wchar_t* szPrinter, BYTE* ptr_status, BYTE* ptr_toner, BYTE* pJob )
{
    bool isSuccess = false;

    if ( NULL == szPrinter )
        return isSuccess;

    wchar_t ip_address[MAX_PATH];
    int port_type = PT_UNKNOWN;
	port_type = CheckPort(szPrinter, ip_address);

	if (PT_UNKNOWN == port_type)
	{
		*ptr_status = __Unknown;
	}
	else
	{
		if (port_type == PT_USB)
			*ptr_status = __PowerOff;
		else
			*ptr_status = __Unknown;
	}

    *ptr_toner = 0;
    *pJob = _UnknowJob;
	
	//Add by Kevin Yin for BMS Bug 56139 begin
	BOOL bOffline = false;
	wchar_t szPrinterName[2 * MAX_PATH] = {0};
	wcscpy_s(szPrinterName, MAX_PATH, szPrinter );

	LPBYTE lpBuff = NULL;
	PPRINTER_INFO_2 pInfo2 = NULL;
	PRINTER_DEFAULTS Defaults = { NULL, NULL, PRINTER_ACCESS_USE };
	HANDLE hPrinter = NULL;
	if (OpenPrinter(szPrinterName, &hPrinter, &Defaults) )
	{
		DWORD cbNeeded;
		GetPrinter(hPrinter, 2, NULL, 0, &cbNeeded);
		lpBuff = new BYTE[cbNeeded];
		ZeroMemory(lpBuff, cbNeeded);
		if (NULL != lpBuff && hPrinter != NULL)
		{
			if ( GetPrinter(hPrinter, 2, lpBuff, cbNeeded, &cbNeeded))
			{
				pInfo2 = (PPRINTER_INFO_2)lpBuff;
				if (pInfo2->Attributes & PRINTER_ATTRIBUTE_WORK_OFFLINE)
				{
					bOffline = true;
				}
			}
		}
		ClosePrinter(hPrinter);
	}

	if (NULL != lpBuff)
		delete[] lpBuff;

	//Add by Kevin Yin for BMS Bug 56139 end
    if ( PT_UNKNOWN == port_type ) 
    {
        // TODO: fail 
		OutputDebugStringA("\r\n####VP:GetPrinterStatus(): PT_UNKNOWN == port_type");
	}
    else
    {
        if ( port_type == PT_USB )
        {
            wchar_t		symbolicname[MAX_PATH];
            char   inBuf[15];
			char	outBuf[MAX_DEVICEID_LEN] = {0};
            unsigned long     inBufSize = sizeof(inBuf);
            unsigned long     outBufSize = sizeof(outBuf);
            unsigned long     tmpBytes = 0;

            PRINTER_STATUS ps;

            ps.PrinterStatus = __Unknown;

            if ( szPrinter ==NULL) {
                return isSuccess;
            }

            wchar_t	PortName[MAX_PATH];
            if (!GetPortName( szPrinter , PortName, MAX_PATH)) {
                return isSuccess;
            }

            if (!USBGetSymbolicNameByPortEx(PortName, _DEF_VID, _DEF_PID, symbolicname,MAX_PATH)) {
                return isSuccess;
            }

            HANDLE ctlPipe = CreateFile(symbolicname,
                    GENERIC_WRITE | GENERIC_READ,
                    FILE_SHARE_WRITE | FILE_SHARE_READ,
                    NULL,
                    OPEN_EXISTING,
                    0,
                    NULL);

            if (NULL == ctlPipe || INVALID_HANDLE_VALUE==ctlPipe)
            {
                return isSuccess;
            }

            int rc = DeviceIoControl(ctlPipe,
                    //IOCTL_GET_DEVICE_ID,
                    IOCTL_USBPRINT_GET_1284_ID,
                    inBuf,
                    inBufSize,
                    outBuf,
                    outBufSize,
                    &tmpBytes,
                    NULL);

            CloseHandle(ctlPipe);

            if (!rc) 
            {
                // Test with FW 32.01, when the machine enter sleep mode
                // DeviceIoControl return false
                ps.PrinterStatus = __PowerSaving;
                *ptr_status = __PowerSaving;
                return true;
            }

            if (!DecodeStatusFromDeviceID((char*)outBuf+2, &ps)) 
            {
                ps.PrinterStatus = __Unknown;
            } 

			if (bOffline)
			{
				if( __Ready == ps.PrinterStatus)
					ps.PrinterStatus = __Offline;
				else
					ps.PrinterStatus = __PowerOff;
			}
			
			*ptr_toner = ps.TonelStatusLevelK;
			*ptr_status = ps.PrinterStatus;
			*pJob = ps.job;

            isSuccess   = true;
        }
        else
        {
            HMODULE hmod = LoadLibrary( DLL_NAME_NET );
            LPFNNETWORKREADSTATUSEX lpfn_net_getstatus = NULL;
            lpfn_net_getstatus = ( LPFNNETWORKREADSTATUSEX ) GetProcAddress( hmod , "NetworkReadStatusEx"    );

			int nCount = 0;
			if (lpfn_net_getstatus)
            {
				while (nCount++ < 1 && !isSuccess)
				{			
					char sz_ip[1024] = {0};
					::WideCharToMultiByte(CP_ACP, 0, ip_address, -1, sz_ip, 1024, NULL, NULL);

					PRINTER_STATUS ps = {0};				

					char sz_community[32] = "public";
					char sz_mfg[1024] = {0};
					char sz_mdl[1024] = {0};

					if (lpfn_net_getstatus(sz_ip, sz_community, &ps, sz_mfg, sz_mdl))
					{
						OutputDebugStringA("\r\n####VP:GetPrinterStatus(): lpfn_net_getstatus Success");

						if (bOffline)
						{
							if (__Ready == ps.PrinterStatus)
								ps.PrinterStatus = __Offline;
						}

						if (__Unknown == ps.PrinterStatus)
						{
							OutputDebugStringToFileA_("vopstatus.txt", "####VP:lpfn_net_getstatus get status:%s", ps.PrinterStatus);
						}
					
						*ptr_toner = ps.TonelStatusLevelK;
						*ptr_status = ps.PrinterStatus;
						*pJob = ps.job;
						isSuccess = true;
					}
					else
					{
						OutputDebugStringA("\r\n####VP:GetPrinterStatus(): lpfn_net_getstatus Fail");
					}
				}
            }
			else
			{
				OutputDebugStringA("\r\n####VP:GetPrinterStatus(): lpfn_net_getstatus == NULL");
			}

			if (!isSuccess)
				OutputDebugStringToFileA_("vopstatus.txt", "####VP:GetPrinterStatus(): Fail ptr_status:%x", (BYTE)*ptr_status);

            FreeLibrary( hmod );
        }
    }

    return isSuccess;
}

static BOOL GetPortName( const wchar_t *szPrinterName, wchar_t *szPortName, int iLen)
{
	HANDLE	hPrinter;
	DWORD	cbNeeded, cReturned;

    wchar_t pprintername[MAX_PATH];
    wcscpy_s(pprintername, MAX_PATH, szPrinterName );

	if (!OpenPrinter(pprintername,&hPrinter,NULL) || hPrinter==NULL) {
		return FALSE;
	}

	cbNeeded = 0;
	if (GetPrinter(hPrinter, 2, NULL, 0, &cbNeeded)==TRUE || cbNeeded==0) {
		ClosePrinter(hPrinter);
		return FALSE;
	}
	
	PRINTER_INFO_2* pPrinterInfo = (PRINTER_INFO_2*)malloc(cbNeeded);
	if(!pPrinterInfo)	{
		ClosePrinter(hPrinter);
		return FALSE;
	}
	if (! GetPrinter(hPrinter, 2, (PBYTE) pPrinterInfo, cbNeeded, &cReturned) )	{
		free(pPrinterInfo); 
		ClosePrinter(hPrinter);
		return FALSE;
	}	
	wcscpy(szPortName,pPrinterInfo->pPortName);
	free(pPrinterInfo); 
	ClosePrinter(hPrinter);
	return TRUE;
}

static BOOL DecodeStatusFromDeviceID(char* device_id, PRINTER_STATUS* status)
{
	if (device_id==NULL || status==NULL) {
#ifdef _DEBUG
		OutputDebugString(L"USBIO: DecodeStatusFromDeviceID: NULL parameter(s)");
#endif
		return FALSE;
	}
#ifdef _DEBUG
	OutputDebugStringA(device_id);
#endif
	char *p = device_id;
	
	while (*p!=NULL && strncmp(p,"STS:",4)!=0) // Look for "STS:"
		p++;
	
	if (*p==NULL)	{ // "STS:" not found
#ifdef _DEBUG
		OutputDebugString(L"ERROR: STS: not found!");
#endif
		OutputDebugStringToFileA_("vopstatus.txt", "###ERROR: STS: not found! %s", device_id);
		return FALSE;
	}
	p += 4;	// Skip "STS:"
#ifdef _DEBUG
	OutputDebugString(L"USBIO: DecodeStatusFromDeviceID: >> Base64Decode!");
#endif

	//if (Base64Decode(p,(unsigned char*)status,sizeof(PRINTER_STATUS))!=sizeof(PRINTER_STATUS))
	if (Base64Decode(p, (unsigned char*)status, sizeof(PRINTER_STATUS)) != sizeof(PRINTER_STATUS))
	{
		OutputDebugStringToFileA_("vopstatus.txt", "### Incorrect: %s", device_id);
		OutputDebugStringToFileA_("vopstatus.txt", "### Vop status: %s", "Base64Decode fail");
		return FALSE;
	}
#ifdef _DEBUG
	OutputDebugString(L"USBIO: DecodeStatusFromDeviceID: << Base64Decode!");
#endif
	return TRUE;
}

static size_t Base64Decode(char *source, unsigned char *target, size_t targetlen)
 {
    char *src, *tmpptr;
    char quadruple[4], tmpresult[3];
    int i, tmplen = 3;
    size_t converted = 0;

    src = (char *)malloc(strlen(source)+5);
    if (src == NULL)
         return -1;
    strcpy(src, source);
    strcat(src, "====");
    tmpptr = src;
    while (tmplen == 3)
    {
         /* get 4 characters to convert */
         for (i=0; i<4; i++)
         {
             while (*tmpptr != '=' && _base64_char_value(*tmpptr)<0)
                  tmpptr++;
             quadruple[i] = *(tmpptr++);
         }
         tmplen = _base64_decode_triple(quadruple, (unsigned char*)tmpresult);
         if (targetlen < tmplen)
         {
             free(src);
             return converted;
         }
         memcpy(target, tmpresult, tmplen);
         target += tmplen;
         targetlen -= tmplen;
         converted += tmplen;
    }
    free(src);
    return converted;
}

static int _base64_decode_triple(char quadruple[4], unsigned char *result)
 {
    int i, triple_value, bytes_to_decode = 3, only_equals_yet = 1;
    int char_value[4];

    for (i=0; i<4; i++)
         char_value[i] = _base64_char_value(quadruple[i]);

    for (i=3; i>=0; i--)
    {
         if (char_value[i]<0)
         {
             if (only_equals_yet && quadruple[i]=='=')
             {
                  char_value[i]=0;
                  bytes_to_decode--;
                  continue;
             }
             return 0;
         }
         only_equals_yet = 0;
    }

    if (bytes_to_decode < 0)
         bytes_to_decode = 0;

    triple_value = char_value[0];
    triple_value *= 64;
    triple_value += char_value[1];
    triple_value *= 64;
    triple_value += char_value[2];
    triple_value *= 64;
    triple_value += char_value[3];

    for (i=bytes_to_decode; i<3; i++)
         triple_value /= 256;
    for (i=bytes_to_decode-1; i>=0; i--)
    {
         result[i] = triple_value%256;
         triple_value /= 256;
    }

    return bytes_to_decode;
} 

static int _base64_char_value(char base64char)
 {
    if (base64char >= 'A' && base64char <= 'Z')
         return base64char-'A';
    if (base64char >= 'a' && base64char <= 'z')
         return base64char-'a'+26;
    if (base64char >= '0' && base64char <= '9')
         return base64char-'0'+2*26;
    if (base64char == '+')
         return 2*26+10;
    if (base64char == '/')
         return 2*26+11;
    return -1;
} 

USBAPI_API int __stdcall SetUserCfg(const wchar_t* szPrinter, UINT8 LeadingEdge, UINT8 SideToSide, UINT8 ImageDensity, UINT8 LowHumidityMode, UINT8 PlateControlMode, UINT8 PrimaryCoolingMode)
{
    if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;

	OutputDebugStringToFileA("\r\n####VP:SetUserCfg() begin");
	int nResult = _ACK;
    wchar_t szIP[MAX_PATH];
    int nPortType = CheckPort( szPrinter, szIP );

    if ( PT_UNKNOWN == nPortType ) 
    {
        nResult = _SW_UNKNOWN_PORT;
    }
    else
    {
        char* buffer = new char[sizeof(COMM_HEADER)+16];
        memset( buffer, INIT_VALUE, sizeof(COMM_HEADER)+16 );
        COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>( buffer );

        ppkg->magic = MAGIC_NUM ;
        ppkg->id = _LS_PRNCMD;
        ppkg->len = 3+16;

        // For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
        // it just stand for the sub id. The real data length is defined by the lib
        ppkg->subid = 0x13;
        ppkg->len2 = 1;
        ppkg->subcmd = _USER_CONFIG_SET;
	
        cmdst_userconfig* pcmd_usercfg = reinterpret_cast<cmdst_userconfig*>( buffer+sizeof(COMM_HEADER));

        pcmd_usercfg->LeadingEdge     = LeadingEdge     ;
        pcmd_usercfg->SideToSide      = SideToSide      ;
        pcmd_usercfg->ImageDensity    = ImageDensity    ;
        pcmd_usercfg->LowHumidityMode = LowHumidityMode ;
		pcmd_usercfg->PlateControlMode = PlateControlMode;
		pcmd_usercfg->PrimaryCoolingMode = PrimaryCoolingMode;

        if ( PT_TCPIP == nPortType || PT_WSD == nPortType )
        {
            nResult = WriteDataViaNetwork( szIP, buffer, sizeof(COMM_HEADER)+16, NULL, 0 );
        }
        else if ( PT_USB == nPortType )
        {
            nResult = WriteDataViaUSB( szPrinter, buffer, sizeof(COMM_HEADER)+16, NULL, 0 );
        }

        if ( buffer )
        {
            delete[] buffer;
            buffer = NULL;
        }
    }
	OutputDebugStringToFileA("\r\n####VP:SetUserCfg(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:SetUserCfg() end");
	return nResult;
}

USBAPI_API int __stdcall GetSoftAp( const wchar_t* szPrinter, char* ssid, char* pwd, BYTE* ptr_wifi_enable  )
{
    if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;

	OutputDebugStringToFileA("\r\n####VP:GetSoftAp() begin");
	int nResult = _ACK;
    wchar_t szIP[MAX_PATH];
    int nPortType = CheckPort( szPrinter, szIP );

    if ( PT_UNKNOWN == nPortType ) 
    {
        nResult = _SW_UNKNOWN_PORT;
    }
    else
    {
        char* buffer = new char[sizeof(COMM_HEADER)+180];
        memset( buffer, INIT_VALUE, sizeof(COMM_HEADER)+180 );

        COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>( buffer );

        ppkg->magic = MAGIC_NUM ;
        ppkg->id = _LS_WIFICMD;
        ppkg->len = 3;

        // For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
        // it just stand for the sub id. The real data length is defined by the lib
        ppkg->subid = 0x13;
        ppkg->len2 = 1;
        ppkg->subcmd = 0x10;   // _P2P_GET  0x10

        cmdst_softap* pcmd_softap = reinterpret_cast<cmdst_softap*>( buffer+sizeof(COMM_HEADER));

        if ( PT_TCPIP == nPortType || PT_WSD == nPortType )
        {
            nResult = WriteDataViaNetwork( szIP, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+180 );
        }
        else if ( PT_USB == nPortType )
        {
            nResult = WriteDataViaUSB( szPrinter, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+180 );
        }

        if ( _ACK == nResult )
        {
            // TODO: while using the following two line the program will crash
            // Expression: (L"Buffer is too small" && 0 )
            // strcpy_s( ssid, 32, pcmd_softap->ssid );
            // strcpy_s( pwd, 64, pcmd_softap->pwd );

            memcpy( ssid, pcmd_softap->ssid, 32); ssid[32] = 0;
            memcpy( pwd, pcmd_softap->pwd, 64); pwd[64] = 0;

			*ptr_wifi_enable = (0x06 == (pcmd_softap->wifiEnable & 0x06));
        }

        if ( buffer )
        {
            delete[] buffer;
            buffer = NULL;
        }
    }

	OutputDebugStringToFileA("\r\n####VP:GetSoftAp(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:GetSoftAp() end");
    return nResult;
}

USBAPI_API int __stdcall SetSoftAp( const wchar_t* szPrinter, const wchar_t* ws_ssid, const wchar_t* ws_pwd, bool isEnableSoftAp )
{
    if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;

	OutputDebugStringToFileA("\r\n####VP:SetSoftAp() begin");
	int nResult = _ACK;
	wchar_t szIP[MAX_PATH] = {0};
    int nPortType = CheckPort( szPrinter, szIP );

    if ( PT_UNKNOWN == nPortType ) 
    {
        nResult = _SW_UNKNOWN_PORT;
    }
    else
    {
        char* buffer = new char[sizeof(COMM_HEADER)+180];
        memset( buffer, INIT_VALUE, sizeof(COMM_HEADER)+180 );
        COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>( buffer );

        ppkg->magic = MAGIC_NUM ;
        ppkg->id = _LS_WIFICMD;
        ppkg->len = 3+180;

        // For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
        // it just stand for the sub id. The real data length is defined by the lib
        ppkg->subid = 0x13;
        ppkg->len2 = 1;
        ppkg->subcmd = 0x11;   // _P2P_SET 0x11

        cmdst_softap* pcmd_softap = reinterpret_cast<cmdst_softap*>( buffer+sizeof(COMM_HEADER));

        char ssid[32] = { 0 };
        char pwd[64] = { 0 };
        // TODO: ws_ssid and ws_pwd length vailidate
        ::WideCharToMultiByte( CP_ACP, 0, ws_ssid, -1, ssid, 32, NULL, NULL );
        ::WideCharToMultiByte( CP_ACP, 0, ws_pwd, -1, pwd, 64, NULL, NULL );

        if ( isEnableSoftAp )
        {
            // set Go and P2P to enable ApSoft called by Leo Luo.
            // bit0: Wi-Fi Enable, bit1: GO Enable, bit2: P2P Enable
            pcmd_softap->wifiEnable = 0x06; 
        }
        else
        {
            pcmd_softap->wifiEnable = 0x00; 
        }

        memcpy( pcmd_softap->ssid, ssid, 32 );
        memcpy( pcmd_softap->pwd, pwd, 64 );

        if ( PT_TCPIP == nPortType || PT_WSD == nPortType )
        {
            nResult = WriteDataViaNetwork( szIP, buffer, sizeof(COMM_HEADER)+180, NULL, 0 );
        }
        else if ( PT_USB == nPortType )
        {
            nResult = WriteDataViaUSB( szPrinter, buffer, sizeof(COMM_HEADER)+180, NULL, 0 );
        }

        if ( buffer )
        {
            delete[] buffer;
            buffer = NULL;
        }
    }

	OutputDebugStringToFileA("\r\n####VP:SetSoftAp(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:SetSoftAp() end");
	return nResult;
}

USBAPI_API int __stdcall ConfirmPassword(const wchar_t* szPrinter, const wchar_t* ws_pwd)
{
	if (NULL == szPrinter)
		return _SW_INVALID_PARAMETER;

	OutputDebugStringToFileA("\r\n####VP:ConfirmPassword() begin");
	int nResult = _ACK;
	wchar_t szIP[MAX_PATH] = {0};
	int nPortType = CheckPort(szPrinter, szIP);

	if (PT_UNKNOWN == nPortType)
	{
		nResult = _SW_UNKNOWN_PORT;
	}
	else
	{
		char* buffer = new char[sizeof(COMM_HEADER)+32];
		memset(buffer, INIT_VALUE, sizeof(COMM_HEADER)+32);

		COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>(buffer);

		ppkg->magic = MAGIC_NUM;
		ppkg->id = _LS_PRNCMD;
		ppkg->len = 3 + 32;

		// For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
		// it just stand for the sub id. The real data length is defined by the lib
		ppkg->subid = 0x13;
		ppkg->len2 = 1;
		ppkg->subcmd = _PRN_PASSWD_COMFIRM;

		char pwd[32] = { 0 };
		// TODO: ws_pwd length vailidate
		::WideCharToMultiByte(CP_ACP, 0, ws_pwd, -1, pwd, 32, NULL, NULL);

		BYTE* pData = reinterpret_cast<BYTE*>(buffer + sizeof(COMM_HEADER));
		memcpy(pData, pwd, 32);

		if (PT_TCPIP == nPortType || PT_WSD == nPortType)
		{
			nResult = WriteDataViaNetwork(szIP, buffer, sizeof(COMM_HEADER)+32, NULL, 0);
		}
		else if (PT_USB == nPortType)
		{
			nResult = WriteDataViaUSB(szPrinter, buffer, sizeof(COMM_HEADER)+32, NULL, 0);
		}

		if (_ACK == nResult)
		{
			
		}

		if (buffer)
		{
			delete[] buffer;
			buffer = NULL;
		}
	}

	OutputDebugStringToFileA("\r\n####VP:ConfirmPassword(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:ConfirmPassword() end");
	return nResult;
}

USBAPI_API int __stdcall GetPassword(const wchar_t* szPrinter, char* pwd)
{
	if (NULL == szPrinter)
		return _SW_INVALID_PARAMETER;
	OutputDebugStringToFileA("\r\n####VP:GetPassword() begin");

	int nResult = _ACK;
	wchar_t szIP[MAX_PATH] = { 0 };
	int nPortType = CheckPort(szPrinter, szIP);

	if (PT_UNKNOWN == nPortType)
	{
		nResult = _SW_UNKNOWN_PORT;
	}
	else
	{
		char* buffer = new char[sizeof(COMM_HEADER)+32];
		memset(buffer, INIT_VALUE, sizeof(COMM_HEADER)+32);
		COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>(buffer);

		ppkg->magic = MAGIC_NUM;
		ppkg->id = _LS_PRNCMD;
		ppkg->len = 3;

		// For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
		// it just stand for the sub id. The real data length is defined by the lib
		ppkg->subid = 0x13;
		ppkg->len2 = 1;
		ppkg->subcmd = _PRN_PASSWD_GET;

		if (PT_TCPIP == nPortType || PT_WSD == nPortType)
		{
			nResult = WriteDataViaNetwork(szIP, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+32);
		}
		else if (PT_USB == nPortType)
		{
			nResult = WriteDataViaUSB(szPrinter, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+32);
		}

		if (_ACK == nResult)
		{
			memcpy(pwd, buffer + sizeof(COMM_HEADER), 32); pwd[32] = 0;
		}

		if (buffer)
		{
			delete[] buffer;
			buffer = NULL;
		}
	}

	OutputDebugStringToFileA("\r\n####VP:GetPassword(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:GetPassword() end");
	return nResult;
}


USBAPI_API int __stdcall SetPassword(const wchar_t* szPrinter, const wchar_t* ws_pwd)
{
	if (NULL == szPrinter)
		return _SW_INVALID_PARAMETER;
	OutputDebugStringToFileA("\r\n####VP:SetPassword() begin");

	int nResult = _ACK;
	wchar_t szIP[MAX_PATH] = {0};
	int nPortType = CheckPort(szPrinter, szIP);

	if (PT_UNKNOWN == nPortType)
	{
		nResult = _SW_UNKNOWN_PORT;
	}
	else
	{
		char* buffer = new char[sizeof(COMM_HEADER)+32];
		memset(buffer, INIT_VALUE, sizeof(COMM_HEADER)+32);
		COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>(buffer);

		ppkg->magic = MAGIC_NUM;
		ppkg->id = _LS_PRNCMD;
		ppkg->len = 3 + 32;

		// For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
		// it just stand for the sub id. The real data length is defined by the lib
		ppkg->subid = 0x13;
		ppkg->len2 = 1;
		ppkg->subcmd = _PRN_PASSWD_SET;

		char pwd[32] = { 0 };
		// TODO: ws_pwd length vailidate
		::WideCharToMultiByte(CP_ACP, 0, ws_pwd, -1, pwd, 32, NULL, NULL);
		
		BYTE* pData = reinterpret_cast<BYTE*>(buffer + sizeof(COMM_HEADER));
		memcpy(pData, pwd, 32);

		if (PT_TCPIP == nPortType || PT_WSD == nPortType)
		{
			nResult = WriteDataViaNetwork(szIP, buffer, sizeof(COMM_HEADER)+32, NULL, 0);
		}
		else if (PT_USB == nPortType)
		{
			nResult = WriteDataViaUSB(szPrinter, buffer, sizeof(COMM_HEADER)+32, NULL, 0);
		}

		if (_ACK == nResult)
		{

		}

		if (buffer)
		{
			delete[] buffer;
			buffer = NULL;
		}
	}

	OutputDebugStringToFileA("\r\n####VP:SetPassword(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:SetPassword() end");
	return nResult;
}

USBAPI_API int __stdcall SetWiFiInfo(const wchar_t* szPrinter, UINT8 wifiEnable, UINT8 wifichangeflag, const wchar_t* ws_ssid, const wchar_t* ws_pwd, UINT8 encryption, UINT8 wepKeyId)
{
    if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;

	OutputDebugStringToFileA("\r\n####VP:SetWiFiInfo() begin");
	int nResult = _ACK;
    wchar_t szIP[MAX_PATH];
    int nPortType = CheckPort( szPrinter, szIP );

    if ( PT_UNKNOWN == nPortType ) 
    {
        nResult = _SW_UNKNOWN_PORT;
		OutputDebugStringToFileA("\r\n####VP:SetWiFiInfo(): PT_UNKNOWN == port_type");
    }
    else
    {
        char* buffer = new char[sizeof(COMM_HEADER)+180];
        memset( buffer, INIT_VALUE, sizeof(COMM_HEADER)+180 );

        COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>( buffer );

        ppkg->magic = MAGIC_NUM ;
        ppkg->id = _LS_WIFICMD;
        ppkg->len = 3+180;

        // For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
        // it just stand for the sub id. The real data length is defined by the lib
        ppkg->subid = 0x13;
        ppkg->len2 = 1;
        ppkg->subcmd = 0x01;   // _WIFI_SET 0x01

        cmdst_wifi_set* pcmd_wifi_set = reinterpret_cast<cmdst_wifi_set*>( buffer+sizeof(COMM_HEADER));

        char ssid[32] = { 0 };
        char pwd[64] = { 0 };
        // TODO: ws_ssid and ws_pwd length vailidate
        ::WideCharToMultiByte( CP_ACP, 0, ws_ssid, -1, ssid, 32, NULL, NULL );
        ::WideCharToMultiByte( CP_ACP, 0, ws_pwd, -1, pwd, 64, NULL, NULL );

		pcmd_wifi_set->wifiEnable = wifiEnable;      // bit0: Wi-Fi Enable, bit1: GO Enable, bit2: P2P Enable
		pcmd_wifi_set->channel = wifichangeflag;	//For fw request enable or disable wifi set 1
        pcmd_wifi_set->encryption = encryption; //
        pcmd_wifi_set->wepKeyId = wepKeyId;     //

        memcpy( pcmd_wifi_set->ssid, ssid, 32 );
        memcpy( pcmd_wifi_set->pwd, pwd, 64 );

        if ( PT_TCPIP == nPortType || PT_WSD == nPortType )
        {
            nResult = WriteDataViaNetwork( szIP, buffer, sizeof(COMM_HEADER)+180, NULL, 0 );
        }
        else if ( PT_USB == nPortType )
        {
            nResult = WriteDataViaUSB( szPrinter, buffer, sizeof(COMM_HEADER)+180, NULL, 0 );
        }

        if ( buffer )
        {
            delete[] buffer;
            buffer = NULL;
        }
    }
	
	OutputDebugStringToFileA("\r\n####VP:SetWiFiInfo(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:SetWiFiInfo() end");

    return nResult;
}

// wchar_t will have problem with StringBuilder
USBAPI_API int __stdcall GetApList( const wchar_t* szPrinter, 
        char* pssid0,  BYTE* ptr_encryption0,
        char* pssid1,  BYTE* ptr_encryption1,
        char* pssid2,  BYTE* ptr_encryption2,
        char* pssid3,  BYTE* ptr_encryption3,
        char* pssid4,  BYTE* ptr_encryption4,
        char* pssid5,  BYTE* ptr_encryption5,
        char* pssid6,  BYTE* ptr_encryption6,
        char* pssid7,  BYTE* ptr_encryption7,
        char* pssid8,  BYTE* ptr_encryption8,
        char* pssid9,  BYTE* ptr_encryption9 )
{
    if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;

	OutputDebugStringToFileA("\r\n####VP:GetApList() begin");
    int nResult = _ACK;
    wchar_t szIP[MAX_PATH];
    int nPortType = CheckPort( szPrinter, szIP );

    if ( PT_UNKNOWN == nPortType ) 
    {
        nResult = _SW_UNKNOWN_PORT;
    }
    else
    {
        char* buffer = new char[sizeof(COMM_HEADER)+340];
        memset( buffer, INIT_VALUE, sizeof(COMM_HEADER)+340 );

        COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>( buffer );

        ppkg->magic = MAGIC_NUM ;
        ppkg->id = _LS_WIFICMD;
        ppkg->len = 3;

        // For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
        // it just stand for the sub id. The real data length is defined by the lib
        ppkg->subid = 0x13;
        ppkg->len2 = 1;
        ppkg->subcmd = 0x07;   // _WIFI_AP_LIST 0x07

        st_ap_info* ptr_ap_info = reinterpret_cast<st_ap_info*>( buffer+sizeof(COMM_HEADER));
        
        // initialize
        for ( int i=0; i<=9; i++ )
        {
            ptr_ap_info[i].encryption = 0xff;
        }
	
        if ( PT_TCPIP == nPortType || PT_WSD == nPortType )
        {
            nResult = WriteDataViaNetwork( szIP, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+340 );
        }
        else if ( PT_USB == nPortType )
        {
            nResult = WriteDataViaUSB( szPrinter, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+340 );
        }

        if ( _ACK == nResult )
        {
            ptr_ap_info = reinterpret_cast<st_ap_info*>( buffer+sizeof(COMM_HEADER));
			int i = 0;
            for ( i=0; i<=9; i++ )
            {
                if ( 0xff == ptr_ap_info[i].encryption )
                    break;

                switch ( i )
                {
                    case 0: *ptr_encryption0 = ptr_ap_info[i].encryption; memcpy(pssid0, ptr_ap_info[i].ssid, 33); pssid0[33] = 0; break;
                    case 1: *ptr_encryption1 = ptr_ap_info[i].encryption; memcpy(pssid1, ptr_ap_info[i].ssid, 33); pssid0[33] = 0; break;
                    case 2: *ptr_encryption2 = ptr_ap_info[i].encryption; memcpy(pssid2, ptr_ap_info[i].ssid, 33); pssid0[33] = 0; break;
                    case 3: *ptr_encryption3 = ptr_ap_info[i].encryption; memcpy(pssid3, ptr_ap_info[i].ssid, 33); pssid0[33] = 0; break;
                    case 4: *ptr_encryption4 = ptr_ap_info[i].encryption; memcpy(pssid4, ptr_ap_info[i].ssid, 33); pssid0[33] = 0; break;
                    case 5: *ptr_encryption5 = ptr_ap_info[i].encryption; memcpy(pssid5, ptr_ap_info[i].ssid, 33); pssid0[33] = 0; break;
                    case 6: *ptr_encryption6 = ptr_ap_info[i].encryption; memcpy(pssid6, ptr_ap_info[i].ssid, 33); pssid0[33] = 0; break;
                    case 7: *ptr_encryption7 = ptr_ap_info[i].encryption; memcpy(pssid7, ptr_ap_info[i].ssid, 33); pssid0[33] = 0; break;
                    case 8: *ptr_encryption8 = ptr_ap_info[i].encryption; memcpy(pssid8, ptr_ap_info[i].ssid, 33); pssid0[33] = 0; break;
                    case 9: *ptr_encryption9 = ptr_ap_info[i].encryption; memcpy(pssid9, ptr_ap_info[i].ssid, 33); pssid0[33] = 0; break;
                }
            }

			for (int nIdx = 0; nIdx < i; nIdx++)
			{
				if (0xff == ptr_ap_info[nIdx].encryption)
					break;

				if (0 != strlen(ptr_ap_info[nIdx].ssid))
				{
					CHAR szDebug[2 * MAX_PATH] = { 0 };
					StringCbPrintfA(szDebug, sizeof(szDebug), "\r\n####VP:GetApList():[%d] Encryption = %02x SSID=%s",
						nIdx, ptr_ap_info[nIdx].encryption, ptr_ap_info[nIdx].ssid);

					OutputDebugStringToFileA(szDebug);
				}
			}
        }
		else
		{
			CHAR szDebug[2 * MAX_PATH] = { 0 };
			StringCbPrintfA(szDebug, sizeof(szDebug), "\r\n####VP:GetApList() Fail: (_ACK[0x00] != nResult[0x%x])", nResult);
			OutputDebugStringToFileA(szDebug);
		}

        if ( buffer )
        {
            delete[] buffer;
            buffer = NULL;
        }
    }

	OutputDebugStringToFileA("\r\n####VP:GetApList(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:GetApList() end");
    return nResult;
}

USBAPI_API int __stdcall GetUserCfg(const wchar_t* szPrinter, BYTE* ptr_leadingedge, BYTE* ptr_sidetoside, BYTE* ptr_imagedensity, BYTE* ptr_lowhumiditymode, BYTE* ptr_platecontrolmode, BYTE* ptr_primarycoolingmode)
{
    if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;

	OutputDebugStringToFileA("\r\n####VP:GetUserCfg() begin");

	int nResult = _ACK;
    wchar_t szIP[MAX_PATH];
    int nPortType = CheckPort( szPrinter, szIP );

    if ( PT_UNKNOWN == nPortType ) 
    {
        nResult = _SW_UNKNOWN_PORT;
    }
    else
    {
        char* buffer = new char[sizeof(COMM_HEADER)+16];
        memset( buffer, INIT_VALUE, sizeof(COMM_HEADER)+16 );
        COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>( buffer );

        ppkg->magic = MAGIC_NUM ;
        ppkg->id = _LS_PRNCMD;
        ppkg->len = 3;

        // For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
        // it just stand for the sub id. The real data length is defined by the lib
        ppkg->subid = 0x13;
        ppkg->len2 = 1;
        ppkg->subcmd = _USER_CONFIG_GET;   

        if ( PT_TCPIP == nPortType || PT_WSD == nPortType )
        {
            nResult = WriteDataViaNetwork( szIP, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+16 );
        }
        else if ( PT_USB == nPortType )
        {
            nResult = WriteDataViaUSB( szPrinter, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+16 );
        }

        if ( _ACK == nResult )
        {
            cmdst_userconfig* pcmd_usercfg = reinterpret_cast<cmdst_userconfig*>( buffer+sizeof(COMM_HEADER));

            if ( -6 <= pcmd_usercfg->LeadingEdge    && 6 >= pcmd_usercfg->LeadingEdge
                    && -6 <= pcmd_usercfg->SideToSide     && 6 >= pcmd_usercfg->SideToSide
                    && -3 <= pcmd_usercfg->ImageDensity   && 3 >= pcmd_usercfg->ImageDensity
                    && 0 <= pcmd_usercfg->LowHumidityMode && 1 >= pcmd_usercfg->LowHumidityMode
					&& 0 <= pcmd_usercfg->PlateControlMode && 2 >= pcmd_usercfg->PlateControlMode
					&& 0 <= pcmd_usercfg->PrimaryCoolingMode && 1 >= pcmd_usercfg->PrimaryCoolingMode)
            {
                *ptr_leadingedge     = pcmd_usercfg->LeadingEdge     ;
                *ptr_sidetoside      = pcmd_usercfg->SideToSide      ;
                *ptr_imagedensity    = pcmd_usercfg->ImageDensity    ;
                *ptr_lowhumiditymode = pcmd_usercfg->LowHumidityMode ;
				*ptr_platecontrolmode = pcmd_usercfg->PlateControlMode;
				*ptr_primarycoolingmode = pcmd_usercfg->PrimaryCoolingMode;

                nResult = _ACK;
            }
            else
            {
                nResult = _SW_INVALID_RETURN_VALUE;
            }
        }

        if ( buffer )
        {
            delete[] buffer;
            buffer = NULL;
        }
    }

	OutputDebugStringToFileA("\r\n####VP:GetUserCfg(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:GetUserCfg() end");
    return nResult;
}

USBAPI_API int __stdcall SetFusingSCReset(const wchar_t* szPrinter)
{
	if (NULL == szPrinter)
		return _SW_INVALID_PARAMETER;
	OutputDebugStringToFileA("\r\n####VP:SetFusingSCReset() begin");

	int nResult = _ACK;
	wchar_t szIP[MAX_PATH];
	int nPortType = CheckPort(szPrinter, szIP);

	if (PT_UNKNOWN == nPortType)
	{
		nResult = _SW_UNKNOWN_PORT;
	}
	else
	{
		char* buffer = new char[sizeof(COMM_HEADER)+1];
		memset(buffer, INIT_VALUE, sizeof(COMM_HEADER)+1);
		COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>(buffer);

		ppkg->magic = MAGIC_NUM;
		ppkg->id = _LS_PRNCMD;
		ppkg->len = 3 + 1;

		// For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
		// it just stand for the sub id. The real data length is defined by the lib
		ppkg->subid = 0x13;
		ppkg->len2 = 1;
		ppkg->subcmd = _Fusing_SC_Reset;

		BYTE* ptrEngineCMD = reinterpret_cast<BYTE*>(buffer + sizeof(COMM_HEADER));
		*ptrEngineCMD = 0;

		if (PT_TCPIP == nPortType || PT_WSD == nPortType)
		{
			nResult = WriteDataViaNetwork(szIP, buffer, sizeof(COMM_HEADER)+1, NULL, 0);
		}
		else if (PT_USB == nPortType)
		{
			nResult = WriteDataViaUSB(szPrinter, buffer, sizeof(COMM_HEADER)+1, NULL, 0);
		}

		if (buffer)
		{
			delete[] buffer;
			buffer = NULL;
		}
	}

	OutputDebugStringToFileA("\r\n####VP:SetFusingSCReset(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:SetFusingSCReset() end");
	return nResult;
}

static bool get_name_driver( const wchar_t* sz_printer, wchar_t* sz_driver, int size )
{
    bool result = false;

    if ( NULL != sz_printer )
    {
        wchar_t sz_loc_printer[1024];
        wcscpy_s( sz_loc_printer, _countof(sz_loc_printer), sz_printer );

        HANDLE hPrinter = NULL;
        OpenPrinter( sz_loc_printer, &hPrinter, NULL);

        if ( hPrinter )
        {
            DWORD cbBuf2 = 0;
            DWORD cbNeeded2 = 0;
            LPBYTE pPrinter = NULL;
            GetPrinter( hPrinter, 2, pPrinter, cbBuf2, &cbNeeded2 );
            if ( cbNeeded2 )
            {
                LPBYTE pPrinter = NULL;
                pPrinter = new BYTE[cbNeeded2];
                cbBuf2 = cbNeeded2;
                GetPrinter( hPrinter, 2, pPrinter, cbBuf2, &cbNeeded2 );

                wcscpy_s( sz_driver, size, reinterpret_cast<PPRINTER_INFO_2>(pPrinter)->pDriverName );

                delete[] pPrinter;
                pPrinter = NULL;

                result = true;
            }
        }
    }

    return result;
}

static bool GetDevMonPath( const wchar_t* sz_printer, LPTSTR lptstrDevMonPath, int nSize)
{
    struct st_printer_reg map_printer2reg[] = 
    {
        { L"Lenovo ABC M001"   , REGKEY_UTILITY_LENOVO_3in1 } ,
        { L"Lenovo ABC M001 w" , REGKEY_UTILITY_LENOVO_3in1 } ,
        { L"Lenovo ABC P001"   , REGKEY_UTILITY_LENOVO_3in1 } ,
        { L"Lenovo ABC P001 w" , REGKEY_UTILITY_LENOVO_3in1 } ,
    };

    wchar_t sz_driver[1024] = { 0 };
    get_name_driver( sz_printer, sz_driver, _countof(sz_driver) );

    wchar_t* sz_reg = NULL;

    for ( int i=0; i<_countof(map_printer2reg); i++ )
    {
        if ( wcscmp( sz_driver, map_printer2reg[i].sz_printer ) == 0 )
        {
            sz_reg = map_printer2reg[i].sz_reg;
            break;
        }
    }


	BOOL  bReadFromReg = FALSE;

	if ( sz_reg && lptstrDevMonPath )
    {
        HKEY  hKey;
        DWORD dwDisposition;

        if (ERROR_SUCCESS == RegCreateKeyEx(HKEY_LOCAL_MACHINE, sz_reg, 
                    0, 0, 0, KEY_READ, NULL, &hKey, &dwDisposition))
        {
            DWORD dwType;
            DWORD dwcbData;

            TCHAR szDevMonPath[MAX_PATH];

            dwcbData = MAX_PATH*sizeof(TCHAR);

            if (ERROR_SUCCESS == RegQueryValueEx(hKey, _T("LLDPath"), NULL, &dwType, (LPBYTE)szDevMonPath, &dwcbData))
            {
                _tcscpy_s(lptstrDevMonPath, nSize, szDevMonPath);

                bReadFromReg = TRUE;
            }

            RegCloseKey(hKey);
        }
    }

    if ( FALSE == bReadFromReg )
        _tcscpy_s(lptstrDevMonPath, nSize, L"C:\\Windows\\twain_32\\Lenovo ABC\\lenovodm.dll");

    return true;
}

USBAPI_API int __stdcall GetIPInfo( 
        const wchar_t* szPrinter,
        BYTE* ptr_mode_ipversion,
        BYTE* ptr_mode_ipaddress,

        BYTE* ptr_ip0,
        BYTE* ptr_ip1,
        BYTE* ptr_ip2,
        BYTE* ptr_ip3,
        
        BYTE* ptr_mask0,
        BYTE* ptr_mask1,
        BYTE* ptr_mask2,
        BYTE* ptr_mask3,

        BYTE* ptr_gate0,
        BYTE* ptr_gate1,
        BYTE* ptr_gate2,
        BYTE* ptr_gate3)
{
    if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;
	
	OutputDebugStringToFileA("\r\n####VP:GetIPInfo() begin");

    int nResult = _ACK;
    wchar_t szIP[MAX_PATH];
    int nPortType = CheckPort( szPrinter, szIP );
	
    if ( PT_UNKNOWN == nPortType ) 
    {
        nResult = _SW_UNKNOWN_PORT;
    }
    else
    {
        char* buffer = new char[sizeof(COMM_HEADER)+128];
        memset( buffer, INIT_VALUE, sizeof(COMM_HEADER)+128 );
        COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>( buffer );

        ppkg->magic = MAGIC_NUM ;
        ppkg->id = _LS_NETCMD;
        ppkg->len = 3;

        // For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
        // it just stand for the sub id. The real data length is defined by the lib
        ppkg->subid = 0x13; 
        ppkg->len2 = 1;

        net_info_st* ptr_net_info = reinterpret_cast<net_info_st*>( buffer+sizeof(COMM_HEADER));

        ppkg->subcmd = 0;       // _NET_GETV4  0x00

        if ( PT_TCPIP == nPortType || PT_WSD == nPortType )
        {
            nResult = WriteDataViaNetwork( szIP, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+128 );
        }
        else if ( PT_USB == nPortType )
        {
            nResult = WriteDataViaUSB( szPrinter, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+128 );
        }

        if ( _ACK == nResult )
        {
            // AutoIP = 0, DHCP   = 3, Manual = 4,
            if ( ptr_net_info->IPAddressMode == 3
                    || ptr_net_info->IPAddressMode == 0
                    || ptr_net_info->IPAddressMode == 4)
            {
                // convert auto IP to DHCP
                if ( 0 == ptr_net_info->IPAddressMode )
                    ptr_net_info->IPAddressMode = 3;

                *ptr_mode_ipversion     = ptr_net_info->IPMode           ;
                *ptr_mode_ipaddress     = ptr_net_info->IPAddressMode    ;
                *ptr_ip0                = ptr_net_info->IPAddress[0]     ;
                *ptr_ip1                = ptr_net_info->IPAddress[1]     ;
                *ptr_ip2                = ptr_net_info->IPAddress[2]     ;
                *ptr_ip3                = ptr_net_info->IPAddress[3]     ;
                *ptr_mask0              = ptr_net_info->SubnetMask[0]    ;
                *ptr_mask1              = ptr_net_info->SubnetMask[1]    ;
                *ptr_mask2              = ptr_net_info->SubnetMask[2]    ;
                *ptr_mask3              = ptr_net_info->SubnetMask[3]    ;
                *ptr_gate0              = ptr_net_info->GatewayAddress[0];
                *ptr_gate1              = ptr_net_info->GatewayAddress[1];
                *ptr_gate2              = ptr_net_info->GatewayAddress[2];
                *ptr_gate3              = ptr_net_info->GatewayAddress[3];
                nResult = _ACK;
            }
            else
            {
                nResult = _SW_INVALID_RETURN_VALUE;
            }
        }

        if ( buffer )
        {
            delete[] buffer;
            buffer = NULL;
        }
    }

	OutputDebugStringToFileA("\r\n####VP:GetIPInfo(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:GetIPInfo() end");
    return nResult;
}

USBAPI_API int __stdcall GetWiFiInfo(const wchar_t* szPrinter, UINT8* ptr_wifienable, char* ssid, char* pwd, UINT8* ptr_encryption, UINT8* ptr_wepKeyId)
{
    if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;

	OutputDebugStringToFileA("\r\n####VP:GetWiFiInfo() begin");

    int nResult = _ACK;
    wchar_t szIP[MAX_PATH];
    int nPortType = CheckPort( szPrinter, szIP );

    if ( PT_UNKNOWN == nPortType ) 
    {
        nResult = _SW_UNKNOWN_PORT;
    }
    else
    {
        char* buffer = new char[sizeof(COMM_HEADER)+180];
        memset( buffer, INIT_VALUE, sizeof(COMM_HEADER)+180 );
        COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>( buffer );

        ppkg->magic = MAGIC_NUM ;
        ppkg->id = _LS_WIFICMD;
        ppkg->len = 3;

        // For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
        // it just stand for the sub id. The real data length is defined by the lib
        ppkg->subid = 0x13;
        ppkg->len2 = 1;
        ppkg->subcmd = 0x00;   // _WIFI_GET   0x00

        if ( PT_TCPIP == nPortType || PT_WSD == nPortType )
        {
            nResult = WriteDataViaNetwork( szIP, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+180 );
        }
        else if ( PT_USB == nPortType )
        {
            nResult = WriteDataViaUSB( szPrinter, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+180 );
        }

        if ( _ACK == nResult )
        {
            cmdst_wifi_get* pcmd_wifi_get = reinterpret_cast<cmdst_wifi_get*>( buffer+sizeof(COMM_HEADER));

            if ( 0 <= pcmd_wifi_get->encryption 
                    && 4 >= pcmd_wifi_get->encryption )
            {
				*ptr_wifienable = pcmd_wifi_get->wifiEnable;
                *ptr_encryption = pcmd_wifi_get->encryption;
                *ptr_wepKeyId   = pcmd_wifi_get->wepKeyId;
                memcpy( ssid, pcmd_wifi_get->ssid, 32); ssid[32] = 0;
                memcpy( pwd, pcmd_wifi_get->pwd, 64); pwd[64] = 0;
                nResult = _ACK;
            }
            else
            {
                nResult = _SW_INVALID_RETURN_VALUE;
            }
        }

        if ( buffer )
        {
            delete[] buffer;
            buffer = NULL;
        }
    }

	OutputDebugStringToFileA("\r\n####VP:GetWiFiInfo(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:GetWiFiInfo() end");
	return nResult;
}

USBAPI_API int __stdcall SetPowerSaveTime( const wchar_t* szPrinter, BYTE time )
{
    if ( NULL == szPrinter ) 
        return _SW_INVALID_PARAMETER;
	OutputDebugStringToFileA("\r\n####VP:SetPowerSaveTime() begin");

    int nResult = _ACK;
    wchar_t szIP[MAX_PATH];
    int nPortType = CheckPort( szPrinter, szIP );

    if ( PT_UNKNOWN == nPortType ) 
    {
        nResult = _SW_UNKNOWN_PORT;
    }
    else
    {
        char* buffer = new char[sizeof(COMM_HEADER)+1];
        memset( buffer, INIT_VALUE, sizeof(COMM_HEADER)+1 );
        COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>( buffer );

        ppkg->magic = MAGIC_NUM ;
        ppkg->id = _LS_PRNCMD;
        ppkg->len = 3+1;

        // For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
        // it just stand for the sub id. The real data length is defined by the lib
        ppkg->subid = 0x13;
        ppkg->len2 = 1;
        ppkg->subcmd = _PSAVE_TIME_SET;
	
        BYTE* ptrTime = reinterpret_cast<BYTE*>( buffer+sizeof(COMM_HEADER));
        *ptrTime = time;

        if ( PT_TCPIP == nPortType || PT_WSD == nPortType )
        {
            nResult = WriteDataViaNetwork( szIP, buffer, sizeof(COMM_HEADER)+1, NULL, 0 );
        }
        else if ( PT_USB == nPortType )
        {
            nResult = WriteDataViaUSB( szPrinter, buffer, sizeof(COMM_HEADER)+1, NULL, 0 );
        }

        if ( buffer )
        {
            delete[] buffer;
            buffer = NULL;
        }
    }

	OutputDebugStringToFileA("\r\n####VP:SetPowerSaveTime(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:SetPowerSaveTime() end");
    return nResult;
}

USBAPI_API int __stdcall GetPowerSaveTime( const wchar_t* szPrinter, BYTE* ptrTime )
{
    if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;

	OutputDebugStringToFileA("\r\n####VP:GetPowerSaveTime() begin");

    int nResult = _ACK;
    wchar_t szIP[MAX_PATH];

    int nPortType = CheckPort( szPrinter, szIP );

    if ( PT_UNKNOWN == nPortType ) 
    {
        nResult = _SW_UNKNOWN_PORT;
    }
    else
    {
        char* buffer = new char[sizeof(COMM_HEADER)+1];
        memset( buffer, INIT_VALUE, sizeof(COMM_HEADER)+1 );
        COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>( buffer );

        ppkg->magic = MAGIC_NUM ;
        ppkg->id = _LS_PRNCMD;
        ppkg->len = 3;

        // For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
        // it just stand for the sub id. The real data length is defined by the lib
        ppkg->subid = 0x13;
        ppkg->len2 = 1;
        ppkg->subcmd = _PSAVE_TIME_GET;   

        if ( PT_TCPIP == nPortType || PT_WSD == nPortType )
        {
            nResult = WriteDataViaNetwork( szIP, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+1 );
        }
        else if ( PT_USB == nPortType )
        {
            nResult = WriteDataViaUSB( szPrinter, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+1 );
        }

        if ( _ACK == nResult )
        {
            BYTE* ptr = reinterpret_cast<BYTE*>( buffer+sizeof(COMM_HEADER));

            if ( 1 <= *ptr && 30 >= *ptr )
            {
                *ptrTime =*ptr;
                nResult = _ACK;
            }
            else
            {
                nResult = _SW_INVALID_RETURN_VALUE;
            }
        }

        if ( buffer )
        {
            delete[] buffer;
            buffer = NULL;
        }
    }

	OutputDebugStringToFileA("\r\n####VP:GetPowerSaveTime(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:GetPowerSaveTime() end");
    return nResult;
}

// width  unit: mil ( 0.001 inch ) 
// height unit: mil ( 0.001 inch ) 
USBAPI_API int __stdcall ScanEx( const wchar_t* sz_printer,
        const wchar_t* szOrig,
        const wchar_t* szView,
        const wchar_t* szThumb,
        int scanMode,
        int resolution,
        int width,
        int height,
        int contrast,
        int brightness,
        int docutype,
        UINT32 uMsg )
{
    static const int RETSCAN_OK             = 0;
    static const int RETSCAN_ERRORDLL       = 1;
    static const int RETSCAN_OPENFAIL       = 2;
    static const int RETSCAN_ERRORPARAMETER = 3;
    static const int RETSCAN_CMDFAIL        = 4;
    static const int RETSCAN_NO_ENOUGH_SPACE= 5;
    static const int RETSCAN_ERROR_PORT     = 6;
    static const int RETSCAN_CANCEL         = 7;

    if ( false == DoseHasEnoughSpace( szOrig, width, height ) )
    {
        return RETSCAN_NO_ENOUGH_SPACE;
    }

    int nResult = RETSCAN_OK;

    // get the ip of printer
    wchar_t szwIP[MAX_PATH] = { 0 };
    char szIP[MAX_PATH] = { 0 };
    int nPortType = CheckPort( sz_printer, szwIP );
    if ( PT_UNKNOWN == nPortType )
    {
        nResult = RETSCAN_ERROR_PORT;
    }
    else
    {
        if ( PT_TCPIP == nPortType || PT_WSD == nPortType ) 
        {
            ::WideCharToMultiByte( CP_ACP, 0, szwIP, -1, szIP, _countof(szIP), NULL, NULL );
        }


        if ( NULL == sz_printer )
            return RETSCAN_ERRORPARAMETER;

        SCANPARAMETER scanparam = { 0 };

        scanparam.ScanSource = 0;                      // 0 Flatbed
        // 1 ADF
        // Duplex ADF
        scanparam.ScanMode = scanMode;                 // 0 Black and White
        // 1 Grayscale (8 bit)
        // 2 Color (24 bit)
        // 3 Preview
        // 4 Color (48 bit)
        scanparam.XRes            = resolution;        // X resolution
        scanparam.YRes            = resolution;        // Y resolution
        scanparam.Left            = 0;                 // left position of scan window (inch/1000)
        scanparam.Top             = 0;                 // top position of scan window (inch/1000)
        scanparam.Width           = width;             // width of scan window (inch/1000)
        scanparam.Height          = height;            // height of scan window (inch/1000)
        scanparam.PixelPerLine    = 0;                 // pixel number per line (final result)
        scanparam.TotalLines      = 0;                 // total line number (final result)
        scanparam.Contrast        = 2*(contrast-50);   // -100 ~ 100
        scanparam.Brightness      = 2*(brightness-50); // -100 ~ 100
        scanparam.Threshold       = 0;                 // -100 ~ 100
        scanparam.MarginTopBottom = 0;
        scanparam.MarginLeftRight = 0;
        scanparam.MarginMiddle    = 0;

		scanparam.Threshold = (scanparam.ScanMode == 0) ? 180 : 127;

        // Capability: ADF/Network ability of MFP
        // Bit0: Flatbed Flag
        // Bit1: Simplex ADF Flag
        // Bit2: Duplex ADF Flag
        // Bit3: Read File mode
        // Bit4: Support Network Flag
        // Bit5: Support Jpeg Flag
        int Capability = 0;

        // 0: Normal pull scan
        // 1: Push scan
        // 2: VOP Photo scan
        // 3: VOP Graphic scan
        // 4: VOP Text scan.
        int PushScanFlag = docutype;

        CScanner obj;  

        wchar_t path_dll[MAX_PATH] = { 0 };
        if ( true == GetDevMonPath( sz_printer, path_dll, _countof(path_dll) ) 
                && DEVMON_STATUS_OK == obj.Initialize( path_dll ) )
        {
            char ch = 0;    // device path is not used 
            if ( DEVMON_STATUS_OK == obj.Open( &ch, szIP, false, Capability, PushScanFlag ) )
            {
                HANDLE hFileOrig  = NULL;

                ULONG ulBytesRead      = 0;
                LONG  lPercentComplete = 0;
                static BYTE headOrig [4*1024];           // buffer for bitmap header structure

                DWORD dwHeadSizeOrig  = 0;    // size of bitmap header and color table

                int nColPixelNumOrig  = 0; // number of pixel in one column of orig imgage

                int nLinePixelNumOrig  = 0; // number of pixel in one line of orig imgage

                int cbStrideRawOrig  = 0; // bytes actually needed per line of orig image

                int cbStridePadOrig  = 0; // bytes per line after padding of orig image

                BYTE* strideOrig       = NULL;  // buffer pointer of single line
                long lWroteOrig = 0;
                int nRowsCnt = 0;

                // calculate pixel number
                nLinePixelNumOrig  = width*resolution/1000;
                nColPixelNumOrig  = height*resolution/1000; 

                // calculate bytes per line 
                cbStrideRawOrig  = GetByteNumPerLineWidthPad( scanMode, nLinePixelNumOrig );

                // calculate bytes per line after padding
                cbStridePadOrig  = (cbStrideRawOrig+3)/4  * 4;

			
                // allocate memory for one line 
                nColPixelNumOrig  = height*resolution/1000; 
				strideOrig = new BYTE[cbStridePadOrig];
		
                if ( DEVMON_STATUS_OK == obj.StartScan() 
                        && DEVMON_STATUS_OK == obj.SetScanParameterEx( &scanparam ) )
                {

					FillBitmapHeader(headOrig, scanMode, nLinePixelNumOrig, nColPixelNumOrig, resolution, 1.0, &dwHeadSizeOrig);

                    CreateFileWithSize( szOrig, reinterpret_cast<const BITMAPFILEHEADER*>(headOrig)->bfSize );

                    hFileOrig  = CreateFile( szOrig , GENERIC_READ | GENERIC_WRITE, 0, (LPSECURITY_ATTRIBUTES) NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, (HANDLE) NULL);

                    WriteFile( hFileOrig , headOrig , dwHeadSizeOrig , &dwHeadSizeOrig , NULL);

                    int nMod = nColPixelNumOrig/100;
                    int nPercent = 0;
                    bCancelScanning = false;
					while (obj.ReadData(strideOrig, cbStridePadOrig, &ulBytesRead, &lPercentComplete) == DEVMON_STATUS_OK)
                    {
                        if ( 0 == ++nRowsCnt%nMod )
                        {
                            nPercent++;
                            ::SendNotifyMessage( HWND_BROADCAST, uMsg, nPercent, 0); 
                        }

                        if ( true == bCancelScanning )
                            break;

                        lWroteOrig += cbStridePadOrig;
                        SetFilePointer( hFileOrig, 0-lWroteOrig, NULL, FILE_END );
                        WriteFile( hFileOrig, strideOrig, cbStridePadOrig, &ulBytesRead, NULL);
                    }

                    CloseHandle( hFileOrig  );

                    if ( false == bCancelScanning )
                    {
                        if ( 0 == scanMode )
                        {
                            if (0 != obj.m_dwTotalLinesRead)
                            {
                                ScalingBitmap( szOrig, szView, 1 );
                                ScalingBitmap( szOrig, szThumb, 1 );
                            }
                        }
                        else
                        {
                            // calculate scaling rate
                            if (0 != obj.m_dwTotalLinesRead)
                            {
                                unsigned int uPixelNumber = (width*resolution / 1000) * (height*resolution / 1000);
                                double rView = GetScalingRate(MAX_PIXEL_PREVIEW, uPixelNumber);
                                double rThumb = GetScalingRate(MAX_PIXEL_THUMB, uPixelNumber);
                                ScalingBitmap(szOrig, szView, rView);
                                ScalingBitmap(szOrig, szThumb, rThumb);
                            }
                        }

                        ::SendNotifyMessage( HWND_BROADCAST, uMsg, 100, 0); 
                    }
                    else
                    {
                        // TODO: clear cache file. add sync mechanism.
                        nResult = RETSCAN_CANCEL;
                    }

                    obj.StopScan();
                }
                else
                {
                    nResult = RETSCAN_CMDFAIL;
                }

                obj.Close();

                if ( NULL != strideOrig )
                {
                    delete[] strideOrig;
                    strideOrig = NULL;
                }
            }
            else
            {
                nResult = RETSCAN_OPENFAIL;
            }
        }
        else
        {
            nResult = RETSCAN_ERRORDLL;
        }
    }

    return nResult;
    
}   

static double GetScalingRate( UINT32 uMax, UINT32 uInput )
{
    double rate = 1.0;

    while ( uMax < uInput )
    {
        uInput = uInput/4;  // 1/2 * 1/2
        rate   = rate * 0.5;
    }

    return rate;
}


// nWidth in pixel
// nHeight in pixel
static void FillBitmapHeader(BYTE* pBuffer, int nScanMode, int nWidth, int nHeight, int resolution, double rate, ULONG* ptrActualSize)
{
    int nBitCount = 0;
    int nClrCnt = 0;

    switch ( nScanMode )
    {
        case _SCANMODE_1BIT_BLACKWHITE:
            nBitCount = 1;
            nClrCnt = 2;
            break;
        case _SCANMODE_8BIT_GRAYSCALE:
            nBitCount = 8;
            nClrCnt = 256;
            break;
        case _SCANMODE_24BIT_COLOR:
            nBitCount = 24;
            nClrCnt = 0;
            break;
    }

    int nActualWidth  = static_cast<int>(nWidth*rate);
    int nActualHeight = static_cast<int>(nHeight*rate); 

    int nBytesPerLine = (nActualWidth*nBitCount+31)/32 * 4;

    UINT32 nClrTableSize = nClrCnt*sizeof(RGBQUAD);
    UINT32 nDataOffset   = sizeof(BITMAPFILEHEADER) + sizeof( BITMAPINFOHEADER ) + nClrTableSize;
    UINT32 nTotalSize    = nDataOffset + nBytesPerLine*nActualHeight;

    *ptrActualSize = nDataOffset;

    // fill the BITMAPFILEHEADER structure
    BITMAPFILEHEADER* pBmpFile = reinterpret_cast<BITMAPFILEHEADER*>( pBuffer );
    pBmpFile->bfType           = ((WORD)('M'<< 8)|'B');
    pBmpFile->bfSize           = nTotalSize;
    pBmpFile->bfReserved1      = 0;
    pBmpFile->bfReserved2      = 0;
    pBmpFile->bfOffBits        = nDataOffset;

    // fill the BITMAPINFOHEADER structrue
    BITMAPINFO* pBmpInfo                = reinterpret_cast<BITMAPINFO*>(pBuffer+sizeof( BITMAPFILEHEADER ));
    pBmpInfo->bmiHeader.biSize          = sizeof( BITMAPINFOHEADER );
    pBmpInfo->bmiHeader.biWidth         = nActualWidth;
    pBmpInfo->bmiHeader.biHeight        = nActualHeight;
    pBmpInfo->bmiHeader.biPlanes        = 1;   // MSDN:  This value must be set to 1.
    pBmpInfo->bmiHeader.biBitCount      = nBitCount;
    pBmpInfo->bmiHeader.biCompression   = BI_RGB;
    pBmpInfo->bmiHeader.biSizeImage     = 0; // MSDN: This may be set to zero for BI_RGB bitmaps.
	pBmpInfo->bmiHeader.biXPelsPerMeter = (DWORD)((double)resolution * 39.37);
	pBmpInfo->bmiHeader.biYPelsPerMeter = (DWORD)((double)resolution * 39.37);
    pBmpInfo->bmiHeader.biClrUsed       = nClrCnt;
    pBmpInfo->bmiHeader.biClrImportant  = 0;

    if ( 0 < nClrCnt )
    {
        RGBQUAD* pClrTableBase = reinterpret_cast<RGBQUAD*>( pBuffer+sizeof(BITMAPFILEHEADER)+sizeof(BITMAPINFOHEADER) );

        for ( int i=0; i<nClrCnt; i++ )
        {
            BYTE val = static_cast<BYTE>( 255 * i / (nClrCnt-1) );

            pClrTableBase[i].rgbBlue     = val;
            pClrTableBase[i].rgbGreen    = val;
            pClrTableBase[i].rgbRed      = val;
            pClrTableBase[i].rgbReserved = 0;
        }
    }
}

static int GetByteNumPerLineWidthPad( int scanMode, int nPixels )
{
    int cbPerLine = 0;

    switch ( scanMode )
    {
        case _SCANMODE_1BIT_BLACKWHITE:
            cbPerLine = (nPixels+7)/8;
            break;
        case _SCANMODE_8BIT_GRAYSCALE:
            cbPerLine = nPixels;
            break;
        case _SCANMODE_24BIT_COLOR:
            cbPerLine = 3*nPixels;
            break;
        default:
            cbPerLine = 0;
    }

    return cbPerLine;
}

/**************************************************************************************
 * Function
 *      Create a file with specified size.
 *
 * Parameter
 *       pszPath [in]: The full path of target file. 
 *       size    [in]: The size of file. 
 *
 * Return value
 *       If success, return true, otherwise return false.
 *
**************************************************************************************/
static bool CreateFileWithSize(
        const wchar_t* pszPath,
        LONG size 
        )
{
    bool bResult = false;

    HANDLE hFile  = CreateFile( pszPath , GENERIC_READ | GENERIC_WRITE, 0, (LPSECURITY_ATTRIBUTES) NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, (HANDLE) NULL);

    if ( INVALID_HANDLE_VALUE != hFile )
    {
        if ( INVALID_SET_FILE_POINTER != SetFilePointer( hFile, size, NULL, FILE_BEGIN ) )
        {
            SetEndOfFile( hFile );
            bResult = true;
        }

        CloseHandle( hFile  );
    }

    return bResult;
}

/**************************************************************************************
 * Function
 *      Detest whether the disk of dst file has enough space for specified
 *      size bitmap. 
 *
 * Parameter
 *       szPath [in]: The full path of dst file. 
 *       nWidth [in]: Size of width in pixel.
 *       nHeight[in]: Size of height in pixel.
 *
 * Return value
 *       If has enough sapce return true, otherwise return false.
 *
**************************************************************************************/
static bool DoseHasEnoughSpace(
        const wchar_t* szPath,
        int nWidth,
        int nHeight 
        )
{
    bool bResult = true;

    const wchar_t* ptr = wcsrchr( szPath, '\\' );

    if ( NULL != ptr )
    {
        wchar_t szBuffer[1024];
        memset( szBuffer, 0, sizeof(szBuffer) );
        wcsncpy( szBuffer, szPath, ptr-szPath );

        ULARGE_INTEGER freeBytesAvailable = {0};
        if ( GetDiskFreeSpaceEx( 
                    szBuffer,
                    &freeBytesAvailable,
                    NULL,
                    NULL
                    ) )
        {
            if ( freeBytesAvailable.QuadPart <= nWidth*nHeight*4 )
            {   
                bResult = false;
            }
        }
    }

    return bResult;
}

USBAPI_API int __stdcall CheckPortAPI( const wchar_t* szPrinter )
{
    return CheckPort( szPrinter, NULL );
}

USBAPI_API void __stdcall CancelScanning()
{
    bCancelScanning = true;
}
