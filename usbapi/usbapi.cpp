// usbapi.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "usbapi.h"
#include "scanner.h"
#include "bitmapScaling.h"

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
#include "tcpxcv.h"

#include <ws2tcpip.h>
#include "dns_sd.h"


#pragma comment(lib, "dnssd.lib")
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

#define MAX_PIXEL_PREVIEW     30*1024*1024
#define MAX_PIXEL_THUMB       1024*1024

#define _PSAVE_TIME_GET     0x00
#define _PSAVE_TIME_SET     0x01
#define _USER_CONFIG_GET    0x02
#define _USER_CONFIG_SET    0x03
#define _PRN_INFO		    0x04
#define _PRN_PASSWD_SET		0x06
#define _PRN_PASSWD_GET		0x07
#define _PRN_PASSWD_COMFIRM	0x08
#define _Fusing_SC_Reset    0x0B
#define _PRN_USER_CENTER    0x10

#define _PRN_POWEROFF_GET   0x0E
#define _PRN_POWEROFF_SET   0x0F
#define _PRN_TONEREND_GET   0x11
#define _PRN_TONEREND_SET   0x12
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
#define REGKEY_UTILITY_LENOVO_3in1  L"SOFTWARE\\Lenovo\\Lenovo M72X8\\Express Scan Manager"
#define REGKEY_UTILITY_LENOVO_SFP   L"SOFTWARE\\Lenovo\\Lenovo LJ22X8\\Express Scan Manager"
#define REGKEY_UTILITY_LENOVO_3in1_7218  L"SOFTWARE\\Lenovo\\Lenovo M72X8\\Express Scan Manager"
#define REGKEY_UTILITY_LENOVO_SFP_2218   L"SOFTWARE\\Lenovo\\Lenovo LJ22X8\\Express Scan Manager"
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

#define        __Ready                       0x00
#define        __Printing                    0x01
#define        __PowerSaving                 0x02
#define        __WarmingUp                   0x03
#define        __PrintCanceling              0x04
#define        __Processing                  0x07
#define        __CopyScanning                0x60
#define        __CopyScanNextPage            0x61
#define        __CopyPrinting                0x62
#define        __CopyCanceling               0x63
#define        __IDCardMode                  0x64
#define        __ScanScanning                0x6A
#define        __ScanSending                 0x6B
#define        __ScanCanceling               0x6C
#define        __ScannerBusy                 0x6D
#define        __TonerEnd1                   0x7F//For china maket
#define        __TonerEnd2                   0x80
#define        __TonerNearEnd                0x81
#define        __ManualFeedRequired          0x85
#define        __InitializeJam               0xBC
#define        __NofeedJam                   0xBD
#define        __JamAtRegistStayOn           0xBE
#define        __JamAtExitNotReach           0xBF
#define        __JamAtExitStayOn             0xC0
#define        __CoverOpen                   0xC1
#define        __NoTonerCartridge            0xC5
#define        __WasteTonerFull              0xC6
#define        __PDLMemoryOver               0xC2
#define        __FWUpdate                    0xC7
#define        __OverHeat                    0xC8
#define        __PolygomotorOnTimeoutError   0xCD
#define        __PolygomotorOffTimeoutError  0xCE
#define        __PolygomotorLockSignalError  0xCF
#define        __BeamSynchronizeError        0xD1
#define        __BiasLeak                    0xD2
#define        __PlateActionError            0xD3
#define        __MainmotorError              0xD4
#define        __MainFanMotorEorror          0xD5
#define        __JoinerThermistorError       0xD6
#define        __JoinerReloadError           0xD7
#define        __HighTemperatureErrorSoft    0xD8
#define        __HighTemperatureErrorHard    0xD9
#define        __JoinerFullHeaterError       0xDA
#define        __Joiner3timesJamError        0xDB
#define        __LowVoltageJoinerReloadError 0xDC
#define        __MotorThermistorError        0xDD
#define        __EEPROMCommunicationError    0xDE
#define        __CTL_PRREQ_NSignalNoCome     0xDF
#define        __SCAN_USB_Disconnect         0xE1
#define        __SCAN_NET_Disconnect         0xE4
#define        __ScanMotorError              0xE5
#define        __SCAN_DRV_CALIB_FAIL         0xE9
#define        __NetWirelessDongleCfgFail    0xE8
#define        __PrinterDataError            0xEF
#define        __Unknown                     0xF0 // status added by SW
#define        __Offline                     0xF1 // status added by SW
#define        __PowerOff                    0xF2 // status added by SW

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

typedef struct net_ipv6info_st
{
	UINT8 UseManualAddress;	//0 Disabled,1 Enabled
	char ManualAddress[40];
	UINT32 ManualMask;
	char StatelessAddress1[44];
	char StatelessAddress2[44];
	char StatelessAddress3[44];
	char LinkLocalAddress[40];
	char IPv6ManualGatewayAddress[40];
	char AutoGatewayAddress[40];
	char AutoStatefulAddress[44];
	UINT8 DHCPv6;
} net_ipv6info_st;

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


typedef struct _cmdst_user_center
{
	char	_2ndSerialNO[20];
	UINT32	_totalCounter;
	char	_SerialNO4AIO[16];
}cmdst_user_center;

typedef struct _fw_info_st
{
	char ServTag[32];
	char cPrinterSerialNumber[32];
	char cPrinterType[32];
	char cAssetTagNumber[32];
	char cMemoryCapacity[32];
	char cProcessorSpeed[32];
	char cFirmwareVersion[32];
	char cNetworkFirmwareVersion[32];
	char cEngFirmwareVersion[32];
	char cPrintingSpeedColor[32];
	char cPrintingSpeedMonochrome[32];
	char cBootCodeVersion[32];
	char cColorTableVersion[32];
	char cMacAddress[32];
} fw_info_st;

typedef int (* LPFN_NETWORK_CONNECT ) (char *server, int port, int timeout ) ;
typedef int(*LPFN_NETWORK_CONNECT_BLOCK) (char *server, int port);
// return the number of bytes successfully read.
typedef int (* LPFN_NETWORK_READ    ) (int sd, void* buff, DWORD len       ) ;
typedef int (* LPFN_NETWORK_WRITE   ) (int sd, void* buff, DWORD len       ) ;
typedef void(* LPFN_NETWORK_CLOSE   ) (int sd                              ) ;
typedef int (__cdecl *LPFNNETWORKREADSTATUSEX) (char *server, char *community, PRINTER_STATUS *status, char* pMfg, char* pMdl);
typedef int(__cdecl *LPFNNETWORKREADSTATUSEXPRO) (char *server, char *community, PRINTER_STATUS *status, char* pMfg, char* pMdl, int timeout, int redundant_packets);

typedef BOOL(*LPFNADDHOSTV4EX2)(char* ip, char* hostname, char* mfgname, char* mdlname);
typedef BOOL(*LPFNADDHOSTV6EX2)(BYTE* ip, DWORD scope_id, char* hostname, char* mfgname, char* mdlname);
typedef BOOL(*LPFINDSNMPAGENTPROEX2)(char* community, BOOL isBroadcast, char* ipV4, char* ipv6, int pktCount, int pktInterval, int delay, int loops, char* oidlist, LPFNADDHOSTV4EX2  addHostV4, LPFNADDHOSTV6EX2 addHostV6);
typedef BOOL(*LPGETREMOTEPHYSADDRESS)(char* host, char* community, int timeout, int index, BYTE* physAddr, int* physAddrLen);

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
USBAPI_API int __stdcall CheckPortAPI2(const wchar_t* szPrinter, char* ipAddress);

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

USBAPI_API int __stdcall GetIpv6Info(const wchar_t* szPrinter,
	BYTE* UseManualAddress,
	char* ManualAddress,
	UINT32 *ManualMask,
	char* StatelessAddress1,
	char* StatelessAddress2,
	char* StatelessAddress3,
	char* LinkLocalAddress,
	char* IPv6ManualGatewayAddress,
	char* AutoGatewayAddress,
	char* AutoStatefulAddress,
	BYTE* DHCPv6);

USBAPI_API int __stdcall GetApList(const wchar_t* szPrinter,
	char* pssid0, BYTE* ptr_encryption0, BYTE* ptr_connected0,
	char* pssid1, BYTE* ptr_encryption1, BYTE* ptr_connected1,
	char* pssid2, BYTE* ptr_encryption2, BYTE* ptr_connected2,
	char* pssid3, BYTE* ptr_encryption3, BYTE* ptr_connected3,
	char* pssid4, BYTE* ptr_encryption4, BYTE* ptr_connected4,
	char* pssid5, BYTE* ptr_encryption5, BYTE* ptr_connected5,
	char* pssid6, BYTE* ptr_encryption6, BYTE* ptr_connected6,
	char* pssid7, BYTE* ptr_encryption7, BYTE* ptr_connected7,
	char* pssid8, BYTE* ptr_encryption8, BYTE* ptr_connected8,
	char* pssid9, BYTE* ptr_encryption9, BYTE* ptr_connected9);

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
USBAPI_API int __stdcall SetIPv6Info(
	const wchar_t* _szPrinter,
	BYTE _UseManualAddress,
	wchar_t* _wsManualAddress,
	UINT32 _ManualMask,
	wchar_t* _wsStatelessAddress1,
	wchar_t* _wsStatelessAddress2,
	wchar_t* _wsStatelessAddress3,
	wchar_t* _wsLinkLocalAddress,
	wchar_t* _wsIPv6ManualGatewayAddress,
	wchar_t* _wsAutoGatewayAddress,
	wchar_t* _wsAutoStatefulAddress,
	BYTE _DHCPv6);

USBAPI_API int __stdcall SetPowerOff(const wchar_t* szPrinter, BYTE isEnable);
USBAPI_API int __stdcall GetPowerOff(const wchar_t* szPrinter, BYTE* ptrIsEnable);

USBAPI_API int __stdcall SetTonerEnd(const wchar_t* szPrinter, BYTE isEnable);
USBAPI_API int __stdcall GetTonerEnd(const wchar_t* szPrinter, BYTE* ptrIsEnable);

USBAPI_API int __stdcall ConfirmPassword(const wchar_t* szPrinter, const wchar_t* ws_pwd);
USBAPI_API int __stdcall GetPassword(const wchar_t* szPrinter, char* pwd);
USBAPI_API int __stdcall SetPassword(const wchar_t* szPrinter, const wchar_t* ws_pwd);

USBAPI_API void __stdcall CancelScanning();
USBAPI_API BOOL __stdcall IsMetricCountry();
USBAPI_API int __stdcall GetWifiChangeStatus(const wchar_t* szPrinter, BYTE* wifiInit);
USBAPI_API int __stdcall GetUserCenterInfo(const wchar_t* szPrinter, char* _2ndSerialNO, UINT32* _totalCounter, char* _serialNO4AIO);
USBAPI_API int __stdcall GetFWInfo(const wchar_t* szPrinter, char * FWVersion);

USBAPI_API int __stdcall SearchValidedIP(const char* macAddress, BOOL ipV4, BOOL isSFP, char * ipFound);
USBAPI_API int __stdcall SetPortIP(const wchar_t * pPrinterName, const char * ipAddress);
USBAPI_API void __stdcall ResetBonjourAddr();
//--------------------------------global--------------------------------------
static const unsigned char INIT_VALUE = 0xfe;
static bool bCancelScanning = false; // Scanning cancel falg, only use in ScanEx(). 
extern CRITICAL_SECTION g_csCriticalSection;
extern CRITICAL_SECTION g_csCriticalSection_bonjour;
extern CRITICAL_SECTION g_csCriticalSection_connect;
extern CRITICAL_SECTION g_csCriticalSection_UsbTest;
extern CRITICAL_SECTION g_csCriticalSection_NetWorkTest;

//--------------------------------implement-----------------------------------

//-----Search machine ip------------//
typedef struct _NODE_INFO {
	char		ip[50];
	char		hostname[64];
	char		modelname[64];
	char		mfgname[64];
	BOOL		isV4;
	char        macAddress[18];
	BOOL		bSNMPV3;
}NODE_INFO, FAR * LPNODE_INFO;

int g_nTotalPrinter = 0;
NODE_INFO g_ip_listbuf[32];
NODE_INFO g_ip_listbuftemp[32];
LPGETREMOTEPHYSADDRESS g_lpfnGetRemotePhysAddress = NULL;

void Addr6toStr(BYTE* ipv6addr, DWORD scope_id, char* addrstr)
{
	char temp[128];
	int  state = 0; // 0:nonzero, 1:zero, 2:nonzero
	int i;

	if (NULL == addrstr)
		return;
	addrstr[0] = '\0';
	for (i = 0; i<8; i++)
	{
		switch (state)
		{
		case 0:
			if (ipv6addr[i * 2] == 0x00 && ipv6addr[i * 2 + 1] == 0x00)
			{
				if (i == 0)
					strcat(addrstr, ":");
				strcat(addrstr, ":");
				state = 1;
			}
			else
			{
				sprintf(temp, "%02x", (unsigned char)ipv6addr[i * 2]);
				strcat(addrstr, temp);
				sprintf(temp, "%02x", (unsigned char)ipv6addr[i * 2 + 1]);
				strcat(addrstr, temp);
				if (i<7)
					strcat(addrstr, ":");
			}
			break;
		case 1:

			if (ipv6addr[i * 2] == 0x00 && ipv6addr[i * 2 + 1] == 0x00) {
			}
			else {
				sprintf(temp, "%02x", (unsigned char)ipv6addr[i * 2]);
				strcat(addrstr, temp);
				sprintf(temp, "%02x", (unsigned char)ipv6addr[i * 2 + 1]);
				strcat(addrstr, temp);
				if (i<7)
					strcat(addrstr, ":");
				state = 2;
			}
			break;
		case 2:
		default:
			sprintf(temp, "%02x", (unsigned char)ipv6addr[i * 2]);
			strcat(addrstr, temp);
			sprintf(temp, "%02x", (unsigned char)ipv6addr[i * 2 + 1]);
			strcat(addrstr, temp);
			if (i<7)
				strcat(addrstr, ":");
			break;
		}
	}

	//Add by kevinYin for BMS Bug(50921) begin[2014.06.17]
	//#if defined(XC6027)
	if (scope_id > 0)
	{
		sprintf(temp, "%%%d", scope_id%100);
		strcat(addrstr, temp);
	}
	//#endif
	//Add by kevinYin for BMS Bug(50921) End[2014.06.17]

	return;
}

BOOL AddHostV4EX2(char* ip, char* hostname, char* mfgname, char* mdlname)
{
	if (g_nTotalPrinter < 32 && NULL != ip)
	{
		memset(&g_ip_listbuftemp[g_nTotalPrinter], 0, sizeof (NODE_INFO));

		g_ip_listbuftemp[g_nTotalPrinter].bSNMPV3 = FALSE;
		strcpy(g_ip_listbuftemp[g_nTotalPrinter].ip, ip);
		strcpy(g_ip_listbuftemp[g_nTotalPrinter].hostname, hostname);
		strcpy(g_ip_listbuftemp[g_nTotalPrinter].modelname, mdlname);
		strcpy(g_ip_listbuftemp[g_nTotalPrinter].mfgname, mfgname);
		g_ip_listbuftemp[g_nTotalPrinter].isV4 = TRUE;
		g_nTotalPrinter++;
	}
	return TRUE;
}



BOOL AddHostV6EX2(BYTE* ip, DWORD scope_id, char* hostname, char* mfgname, char* mdlname)
{
	char	temp[256];

	if (g_nTotalPrinter < 32)
	{
		memcpy(temp, ip, 16);
		memset(&g_ip_listbuftemp[g_nTotalPrinter], 0, sizeof (NODE_INFO));

		g_ip_listbuftemp[g_nTotalPrinter].bSNMPV3 = FALSE;
		Addr6toStr((BYTE*)temp, scope_id, g_ip_listbuftemp[g_nTotalPrinter].ip);

		strcpy(g_ip_listbuftemp[g_nTotalPrinter].modelname, mdlname);
		strcpy(g_ip_listbuftemp[g_nTotalPrinter].mfgname, mfgname);
		g_ip_listbuftemp[g_nTotalPrinter].isV4 = FALSE;

		//	strcpy(ip_listbuf[totalprinter].hostname,hostname);
		// add by power hu by power hu 

		std::string strTemp1, strTemp2;

		strTemp1 = "fe80";
		strTemp2 = g_ip_listbuftemp[g_nTotalPrinter].ip;
		strTemp2 = strTemp2.substr(0, 4);

		if (strTemp2 == strTemp1)
		{
			g_nTotalPrinter++;
		}
		else
		{
			memset(&g_ip_listbuftemp[g_nTotalPrinter], 0, sizeof (NODE_INFO));
		}

	}

	return TRUE;
}

static int GetMacAddress(char *pIPAddress, char *pMacAddress)
{
	OutputDebugString(L"### ACT:GetMacAddress ");
	OutputDebugStringA(pIPAddress);
	char strTemp[MAX_PATH];
	memset(strTemp, 0, sizeof(strTemp));
	char szTemp[MAX_PATH];
	memset(szTemp, 0, sizeof(szTemp));
	int nLen = MAX_PATH;
	int nRet = 0;

	nRet = g_lpfnGetRemotePhysAddress(pIPAddress, "public", 3000, 2, (BYTE*)strTemp, &nLen);	

	int iWaitTime = 0;
	while (FALSE == nRet && iWaitTime < 3)
	{

		nRet = g_lpfnGetRemotePhysAddress(pIPAddress, "public", 3000, 2, (BYTE*)strTemp, &nLen);	

		iWaitTime++;
		Sleep(100);
		//		if(!nRet)
		//		OutputDebugString(_T("### ACT:GetMacAddress fail"));
	}
	//	sprintf(pMacAddress, "%.2X：%.2X：%.2X：%.2X：%.2X：%.2X", strTemp[0] & 0x00ff, strTemp[1] & 0x00ff, strTemp[2] & 0x00ff,
	//					strTemp[3] & 0x00ff, strTemp[4] & 0x00ff, strTemp[5] & 0x00ff);

	if (nRet)
	{
		for (int i = 0; i < 6; i++)
		{
			sprintf(szTemp, "%.2X", strTemp[i] & 0x00ff);
			strcat(pMacAddress, szTemp);
			if (i<5)
			{
				strcat(pMacAddress, ":");
			}
		}
		OutputDebugStringA(pMacAddress);
	}
	else
	{
		OutputDebugString(_T("### ACT:GetMacAddress fail"));
	}
	return nRet;
}

static BOOL ProcessMacAddress(const char * mac, BOOL ipV4, char * ipFound)
{
	for (int i = 0; i < g_nTotalPrinter; i++)
	{
		GetMacAddress(g_ip_listbuftemp[i].ip, g_ip_listbuftemp[i].macAddress);

		if (strcmp(mac, g_ip_listbuftemp[i].macAddress) == 0)
		{
			if (ipV4 == g_ip_listbuftemp[i].isV4)
			{
				strcpy(ipFound, g_ip_listbuftemp[i].ip);
				return TRUE;
			}
		}
	}

	return FALSE;
}

USBAPI_API int __stdcall SearchValidedIP2(SAFEARRAY** ipList)
{
	BSTR bstrArray[500] = { 0 };
	int nResult = 1;
	char sysObjectID[] = "1.3.6.1.4.1.26266.86.10.2.1\000";
	DWORD dwEngineEnterpriseId = 0;

	HMODULE hmod = LoadLibrary(DLL_NAME_NET);

	LPFINDSNMPAGENTPROEX2 FindAgentProEX2 = NULL;

	FindAgentProEX2 = (LPFINDSNMPAGENTPROEX2)GetProcAddress(hmod, "FindSnmpAgentProEx2");

	if (FindAgentProEX2)
	{
		char community[129] = { 0 };
		strcpy(community, "public");
		g_nTotalPrinter = 0;

		FindAgentProEX2(community, TRUE, "255.255.255.255", "", 3, 30, 6000, 2, sysObjectID, AddHostV4EX2, AddHostV6EX2);

		for (int i = 0; i < g_nTotalPrinter; i++)
		{
			TCHAR ipAddress[256] = { 0 };
			::MultiByteToWideChar(CP_ACP, 0, g_ip_listbuftemp[i].ip, strlen(g_ip_listbuftemp[i].ip), ipAddress, 256);
			
			bstrArray[i] = ::SysAllocString(ipAddress);
		}


		CreateSafeArrayFromBSTRArray
			(
				bstrArray,
				g_nTotalPrinter,
				ipList
				);

		for (int i = 0; i < g_nTotalPrinter; i++)
		{
			::SysFreeString(bstrArray[i]);
		}

		OutputDebugStringA("\r\n####VP:SearchValidedIP() FindAgentProEX2 End.");
	}
	else
	{
		nResult = 0;
		OutputDebugStringA("\r\n####VP:SearchValidedIP() FindAgentProNBNEX2 NULL.");
	}

	FindAgentProEX2 = NULL;

	FreeLibrary(hmod);

	return nResult;
}
USBAPI_API int __stdcall SearchValidedIP(const char * macAddress, BOOL ipV4, BOOL isSFP, char * ipFound)
{
	int nResult = 1;
	char cbOID_M[] = "1.3.6.1.4.1.19046.101.2.1\000";
	char cbOID_S[] = "1.3.6.1.4.1.19046.101.1.1\000";
	DWORD dwEngineEnterpriseId = 0;

	HMODULE hmod = LoadLibrary(DLL_NAME_NET);

	LPFINDSNMPAGENTPROEX2 FindAgentProEX2 = NULL;

	FindAgentProEX2 = (LPFINDSNMPAGENTPROEX2)GetProcAddress(hmod, "FindSnmpAgentProEx2");
	g_lpfnGetRemotePhysAddress = (LPGETREMOTEPHYSADDRESS)GetProcAddress(hmod, "GetRemotePhysAddress");

	if (FindAgentProEX2)
	{
		char community[129] = { 0 };
		strcpy(community, "public");
		g_nTotalPrinter = 0;

		FindAgentProEX2(community, TRUE, "255.255.255.255", "", 3, 30, 6000, 2, isSFP ? cbOID_S : cbOID_M, AddHostV4EX2, AddHostV6EX2);

		if (g_lpfnGetRemotePhysAddress)
		{
			ProcessMacAddress(macAddress, ipV4, ipFound);
		}

		OutputDebugStringA("\r\n####VP:SearchValidedIP() FindAgentProEX2 End.");
	}
	else
	{
		nResult = 0;
		OutputDebugStringA("\r\n####VP:SearchValidedIP() FindAgentProNBNEX2 NULL.");
	}

	FindAgentProEX2 = NULL;
	g_lpfnGetRemotePhysAddress = NULL;

	FreeLibrary(hmod);

	return nResult;
}
//-----------------------------------//

//---------Set printer port ip---------//
void AddTCPIPPort(wchar_t* strPortName, const wchar_t* strIPAddress)
{

	char szText[256];
	DWORD errorcode;

	DWORD				cbInputData = 100;
	PBYTE				pOutputData = NULL;
	DWORD				cbOutputNeeded = 0;

	HANDLE hXcv = INVALID_HANDLE_VALUE;
	PORT_DATA_1			portData;
	DELETE_PORT_DATA_1  DelPortData;
	ZeroMemory(&portData, sizeof(PORT_DATA_1));
	ZeroMemory(&DelPortData, sizeof(DELETE_PORT_DATA_1));

	WCHAR wszTemp[64], wszTemp1[49];//, wsztSNMPCommunity[64];
	//char szTemp[256];
	//WCHAR pwstr;
	//memcpy(szTemp,InstallInfo.szPrinterName,sizeof(szTemp));
	char szTemp[64] = "lp";
	mbstowcs(wszTemp, szTemp, sizeof(szTemp));

	int jj = 0;

	wcscpy(portData.sztPortName, strPortName);
	wcscpy(DelPortData.psztPortName, strPortName);

	//*/

	DelPortData.dwVersion = 1;
	DelPortData.dwReserved = 0;
	portData.dwVersion = 1;
	portData.dwProtocol = PROTOCOL_LPR_TYPE;
	portData.cbSize = sizeof(PORT_DATA_1);
	portData.dwReserved = 0L;

	WCHAR wszTempQue[64];

	char szTempQue[64] = "lp";
	mbstowcs(wszTempQue, szTempQue, sizeof(szTempQue));
	wcscpy(portData.sztQueue, wszTempQue);
	portData.dwSNMPDevIndex = 1;


	wcscpy(portData.sztHostAddress, strIPAddress);
	wcscpy(portData.sztIPAddress, strIPAddress);


	portData.dwDoubleSpool = FALSE;
	//	portData.dwSNMPEnabled = TRUE;

	portData.dwPortNumber = 515;

	DWORD code = 0;
	PRINTER_DEFAULTS Defaults = { NULL, NULL, SERVER_ACCESS_ADMINISTER };
	DWORD dwStatus = 0;

	pOutputData = new BYTE[cbInputData];

	if (OpenPrinter(L",XcvMonitor Standard TCP/IP Port", &hXcv, &Defaults))
	{
		// hXcv contains an Xcv data handle to the monitor <MonitorName>
		XcvData(hXcv, L"DeletePort", (PBYTE)&DelPortData, sizeof(DELETE_PORT_DATA_1), pOutputData, cbInputData, &cbOutputNeeded, &dwStatus);
		code = GetLastError();
		sprintf(szText, "delete port driver%d", code);

		::OutputDebugStringA(szText);

		XcvData(hXcv, L"AddPort", (PBYTE)&portData, sizeof(PORT_DATA_1), pOutputData, cbInputData, &cbOutputNeeded, &dwStatus);
		code = GetLastError();
		sprintf(szText, "delete port driver%d", code);

		::OutputDebugStringA(szText);

	}


	//
	code = GetLastError();
	errorcode = GetLastError();
	sprintf(szText, "add port error%d", errorcode);

	::OutputDebugStringA(szText);
}

BOOL CheckPortExist(wchar_t* szPortName)
{

	//////////////////////enum current add port name /////
	PORT_INFO_2	*info2 = NULL;
	DWORD		sizeneed = 0;
	DWORD		nBuffer = 0;
	BOOL		ret = FALSE;
	int			i = 0;
	//	TCHAR portname[256];

	ret = EnumPorts(NULL, 2, NULL, 0, &sizeneed, &nBuffer);
	if (!ret && GetLastError() == ERROR_INSUFFICIENT_BUFFER)
	{
		info2 = (PORT_INFO_2*)malloc(sizeneed);
		ret = EnumPorts(NULL, 2, (LPBYTE)info2, sizeneed, &sizeneed, &nBuffer);
		if (!ret)
		{
			goto clean_up;
		}
	}
	if ((int)nBuffer > 0)
	{
		for (i = 0; i<(int)nBuffer; i++)
		{
			wchar_t *strTemp1, *strTemp2;
			strTemp1 = szPortName;
			strTemp2 = info2[i].pPortName;

			if (_wcsicmp(strTemp1, strTemp2) == 0)
			{
				free(info2);
				return FALSE;
			}
		}
	}
clean_up:
	if (info2 != NULL)
	{
		free(info2);
		info2 = NULL;
	}

	//////////////////////enum current add port name  end//
	return TRUE;
}

BOOL SetPrinterPort(TCHAR szPrinterName[MAX_PATH * 2], TCHAR szPortName[MAX_PATH * 2])
{
	HANDLE hPrinter = NULL;
	DWORD dwNeeded;
	PRINTER_INFO_2 *pi2 = NULL;
	DEVMODE *pDevMode = NULL;
	PRINTER_DEFAULTS pd;
	BOOL bFlag;
	LONG lFlag;

	::OutputDebugString(szPrinterName);
	DWORD errorcode;
	char sztext[256];

	ZeroMemory(&pd, sizeof(pd));
	pd.DesiredAccess = PRINTER_ALL_ACCESS;
	bFlag = OpenPrinter(szPrinterName, &hPrinter, &pd);
	errorcode = ::GetLastError();
	if (!bFlag || (hPrinter == NULL))
		return FALSE;
	errorcode = ::GetLastError();

	sprintf(sztext, "OpenPrinter  is %d", errorcode);
	::OutputDebugStringA(sztext);
	// The first GetPrinter tells you how big the buffer should be in 
	// order to hold all of PRINTER_INFO_2. Note that this should fail with 
	// ERROR_INSUFFICIENT_BUFFER.  If GetPrinter fails for any other reason 
	// or dwNeeded isn't set for some reason, then there is a problem...
	SetLastError(0);
	bFlag = GetPrinter(hPrinter, 2, 0, 0, &dwNeeded);
	if ((!bFlag) && (GetLastError() != ERROR_INSUFFICIENT_BUFFER) ||
		(dwNeeded == 0))
	{
		ClosePrinter(hPrinter);
		return FALSE;
	}

	errorcode = ::GetLastError();



	SetLastError(0);

	pi2 = (PRINTER_INFO_2 *)GlobalAlloc(GPTR, dwNeeded + 4);
	errorcode = ::GetLastError();



	if (pi2 == NULL)
	{
		ClosePrinter(hPrinter);
		return FALSE;
	}

	// The second GetPrinter fills in all the current settings, so all you
	// need to do is modify what you're interested in...
	bFlag = GetPrinter(hPrinter, 2, (LPBYTE)pi2, dwNeeded, &dwNeeded);
	errorcode = 0;
	errorcode = ::GetLastError();


	if (!bFlag)
	{
		GlobalFree(pi2);
		ClosePrinter(hPrinter);
		return FALSE;
	}

	// change printer port 

	pi2->pPortName = LPWSTR(szPortName);//

	// Update printer information...
	bFlag = SetPrinter(hPrinter, 2, (LPBYTE)pi2, 0);
	errorcode = ::GetLastError();
	sprintf(sztext, "SetPrinter  is %d", errorcode);
	::OutputDebugStringA(sztext);


	if (!bFlag)
		// The driver doesn't support, or it is unable to make the change...
	{
		GlobalFree(pi2);
		ClosePrinter(hPrinter);
		if (pDevMode)
			GlobalFree(pDevMode);
		return FALSE;
	}

	// Tell other apps that there was a change...

	// Clean up...
	if (pi2)
		GlobalFree(pi2);
	if (hPrinter)
		ClosePrinter(hPrinter);
	if (pDevMode)
		GlobalFree(pDevMode);

	return TRUE;

}

USBAPI_API int __stdcall SetPortIP(const wchar_t * pPrinterName, const wchar_t * ipAddress)
{
	TCHAR strPortName[100] = { 0 };
	int i = 0;

	wsprintfW(strPortName, _T("NtwkPort%02d"), i);

	while (!CheckPortExist(strPortName))
	{
		i++;
		wsprintfW(strPortName, _T("NtwkPort%02d"), i);
	}

	// add port 
	AddTCPIPPort(strPortName, ipAddress);

	TCHAR tPrinterName[MAX_PATH * 2], tPortName[MAX_PATH * 2];
	wcscpy(tPrinterName, pPrinterName);
	wcscpy(tPortName, strPortName);

	if (!SetPrinterPort(tPrinterName, tPortName))
	{
		return 0;
	}

	return 1;
}
//-------------------------------------//

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

//Bonjour tranlate hostname to ip address//

// Note: the select() implementation on Windows (Winsock2) fails with any timeout much larger than this
typedef int        pid_t;
#define getpid     _getpid
#define strcasecmp _stricmp
#define snprintf   _snprintf
static const char kFilePathSep = '\\';
#ifndef HeapEnableTerminationOnCorruption
#     define HeapEnableTerminationOnCorruption (HEAP_INFORMATION_CLASS)1
#endif
#if !defined(IFNAMSIZ)
#define IFNAMSIZ 16
#endif
#define if_nametoindex if_nametoindex_win
#define if_indextoname if_indextoname_win

typedef PCHAR(WINAPI * if_indextoname_funcptr_t)(ULONG index, PCHAR name);
typedef ULONG(WINAPI * if_nametoindex_funcptr_t)(PCSTR name);

unsigned if_nametoindex_win(const char *ifname)
{
	HMODULE library;
	unsigned index = 0;

	// Try and load the IP helper library dll
	if ((library = LoadLibrary(TEXT("Iphlpapi"))) != NULL)
	{
		if_nametoindex_funcptr_t if_nametoindex_funcptr;

		// On Vista and above there is a Posix like implementation of if_nametoindex
		if ((if_nametoindex_funcptr = (if_nametoindex_funcptr_t)GetProcAddress(library, "if_nametoindex")) != NULL)
		{
			index = if_nametoindex_funcptr(ifname);
		}

		FreeLibrary(library);
	}

	return index;
}

char * if_indextoname_win(unsigned ifindex, char *ifname)
{
	HMODULE library;
	char * name = NULL;

	// Try and load the IP helper library dll
	if ((library = LoadLibrary(TEXT("Iphlpapi"))) != NULL)
	{
		if_indextoname_funcptr_t if_indextoname_funcptr;

		// On Vista and above there is a Posix like implementation of if_indextoname
		if ((if_indextoname_funcptr = (if_indextoname_funcptr_t)GetProcAddress(library, "if_indextoname")) != NULL)
		{
			name = if_indextoname_funcptr(ifindex, ifname);
		}

		FreeLibrary(library);
	}

	return name;
}

static size_t _sa_len(const struct sockaddr *addr)
{
	if (addr->sa_family == AF_INET) return (sizeof(struct sockaddr_in));
	else if (addr->sa_family == AF_INET6) return (sizeof(struct sockaddr_in6));
	else return (sizeof(struct sockaddr));
}

#   define SA_LEN(addr) (_sa_len(addr))


#define LONG_TIME 1

static volatile int stopNow = 0;
static volatile int timeOut = LONG_TIME;
static DNSServiceRef client = NULL;
static char addr[256] = "";
static BOOL HasObtainIPv6 = FALSE;

USBAPI_API void __stdcall ResetBonjourAddr()
{
	::memset(addr, 0, 256);
}

BOOL TestIpConnected(char* szIP)
{
	int nResult = TRUE;

	HMODULE hmod = LoadLibrary(DLL_NAME_NET);

	LPFN_NETWORK_CONNECT_BLOCK lpfnNetworkConnectBlock = NULL;
	LPFN_NETWORK_CONNECT  lpfnNetworkConnect = NULL;
	LPFN_NETWORK_CLOSE    lpfnNetworkClose = NULL;

	lpfnNetworkConnectBlock = (LPFN_NETWORK_CONNECT_BLOCK)GetProcAddress(hmod, "NetworkConnect");
	lpfnNetworkConnect = (LPFN_NETWORK_CONNECT)GetProcAddress(hmod, "NetworkConnectNonBlock");
	lpfnNetworkClose = (LPFN_NETWORK_CLOSE)GetProcAddress(hmod, "NetworkClose");


	if (hmod && \
		//lpfnNetworkConnect && 
		lpfnNetworkConnectBlock && \
		lpfnNetworkClose)
	{

		//int socketID = lpfnNetworkConnect(szIP, 9100, 1000);
		int socketID = lpfnNetworkConnectBlock(szIP, 9100);

		if (-1 == socketID)
		{
			char showIp[256] = "";
			snprintf(showIp, sizeof(showIp), "\nTestIpConnected() Fail %s", szIP);
			OutputDebugStringA(showIp);

			nResult = FALSE;
		}
		else
		{
			char showIp[256] = "";
			snprintf(showIp, sizeof(showIp), "\nTestIpConnected() success %s", szIP);
			OutputDebugStringA(showIp);

			nResult = TRUE;
		}

		lpfnNetworkClose(socketID);

	}
	else
	{
		nResult = FALSE;
	}

	lpfnNetworkConnect = NULL;
	lpfnNetworkClose = NULL;

	FreeLibrary(hmod);

	return nResult;
}

static void DNSSD_API addrinfo_reply(DNSServiceRef sdref, DNSServiceFlags flags, uint32_t interfaceIndex, DNSServiceErrorType errorCode, const char *hostname, const struct sockaddr *address, uint32_t ttl, void *context)
{
	if (address && address->sa_family == AF_INET)
	{
		const unsigned char *b = (const unsigned char *)&((struct sockaddr_in *)address)->sin_addr;
		snprintf(addr, sizeof(addr), "%d.%d.%d.%d", b[0], b[1], b[2], b[3]);

		char showIp[256] = "";
		snprintf(showIp, sizeof(showIp), "\naddrinfo_reply(IPv4) %s", addr);
		OutputDebugStringA(showIp);
	}
	else if (address && address->sa_family == AF_INET6)
	{
		char tempAddr[256] = "";
		char if_name[IFNAMSIZ] = { 0 };		// Older Linux distributions don't define IF_NAMESIZE
		const struct sockaddr_in6 *s6 = (const struct sockaddr_in6 *)address;
		const unsigned char       *b = (const unsigned char *)&(s6->sin6_addr);

		Addr6toStr((BYTE*)b, s6->sin6_scope_id, tempAddr);

		char showIp[256] = "";
		snprintf(showIp, sizeof(showIp), "\naddrinfo_reply(IPv6) %s", tempAddr);
		OutputDebugStringA(showIp);

		if (HasObtainIPv6 == FALSE)
		{
			if (TestIpConnected(tempAddr))
			{
				snprintf(showIp, sizeof(showIp), "\naddrinfo_reply(IPv6) available %s", tempAddr);
				OutputDebugStringA(showIp);

				memcpy(addr, tempAddr, 256);
				HasObtainIPv6 = TRUE;
			}
		}

		//std::string strTemp;
		//strTemp = tempAddr;
		//strTemp = strTemp.substr(0, 4);

		//if (strTemp != "fe80")
		//{
		//	memcpy(addr, tempAddr, 256);
		//	//snprintf(addr, sizeof(addr), "%02x%02x:%02x%02x:%02x%02x:%02x%02x:%02x%02x:%02x%02x:%02x%02x:%02x%02x%%%s",
		//	//	b[0x0], b[0x1], b[0x2], b[0x3], b[0x4], b[0x5], b[0x6], b[0x7],
		//	//	b[0x8], b[0x9], b[0xA], b[0xB], b[0xC], b[0xD], b[0xE], b[0xF], if_name);
		//}


	/*	if (!if_indextoname(s6->sin6_scope_id, if_name))
			snprintf(if_name, sizeof(if_name), "%d", s6->sin6_scope_id);*/

	}
}

static void HandleEvents(void)
{
	int dns_sd_fd = client ? DNSServiceRefSockFD(client) : -1;
	int nfds = dns_sd_fd + 1;
	fd_set readfds;
	struct timeval tv;
	int result;

	stopNow = 0;

	while (!stopNow)
	{
		// 1. Set up the fd_set as usual here.
		// This example client has no file descriptors of its own,
		// but a real application would call FD_SET to add them to the set here
		FD_ZERO(&readfds);

		// 2. Add the fd for our client(s) to the fd_set
		if (client) FD_SET(dns_sd_fd, &readfds);

		// 3. Set up the timeout.
		tv.tv_sec = timeOut;
		tv.tv_usec = 0;

		result = select(nfds, &readfds, (fd_set*)NULL, (fd_set*)NULL, &tv);

		if (result > 0)
		{
			DNSServiceErrorType err = kDNSServiceErr_NoError;
			if (client && FD_ISSET(dns_sd_fd, &readfds))
			{
				err = DNSServiceProcessResult(client);
			}	
			if (err) { 
				stopNow = 1; 
			}
		}
		else if (result == 0)
			stopNow = 1;
		else
		{
			if (errno != EINTR) stopNow = 1;
		}
	}
}

static bool BonjourGetAddrInfo(wchar_t* hostname, wchar_t* ipAddress)
{
	DNSServiceErrorType err;

	wchar_t szIP[MAX_PATH] = { 0 };
	char _hostname[100] = { 0 };

	::WideCharToMultiByte(CP_ACP, 0, hostname, -1, _hostname, 100, NULL, NULL);

	::memset(addr, 0, 256);

	EnterCriticalSection(&g_csCriticalSection_bonjour);

	HasObtainIPv6 = FALSE;

	err = DNSServiceGetAddrInfo(&client,
		kDNSServiceFlagsReturnIntermediates, kDNSServiceInterfaceIndexAny, kDNSServiceProtocol_IPv4, _hostname, addrinfo_reply, NULL);


	if (!client || err != kDNSServiceErr_NoError)
	{ 
		LeaveCriticalSection(&g_csCriticalSection_bonjour);
		return FALSE;
	}

	HandleEvents();
	if (client) DNSServiceRefDeallocate(client);
	

	if (TestIpConnected(addr))
	{
		::MultiByteToWideChar(CP_ACP, 0, addr, strlen(addr), ipAddress, 100);
	}
	else
	{

		::memset(addr, 0, 256);

		//EnterCriticalSection(&g_csCriticalSection_bonjour);
		err = DNSServiceGetAddrInfo(&client,
			kDNSServiceFlagsReturnIntermediates, kDNSServiceInterfaceIndexAny, kDNSServiceProtocol_IPv6, _hostname, addrinfo_reply, NULL);

		if (!client || err != kDNSServiceErr_NoError)
		{
			LeaveCriticalSection(&g_csCriticalSection_bonjour);
			return FALSE;
		}

		HandleEvents();
		if (client) DNSServiceRefDeallocate(client);
		//LeaveCriticalSection(&g_csCriticalSection_bonjour);

		::MultiByteToWideChar(CP_ACP, 0, addr, strlen(addr), ipAddress, 100);
	}
	LeaveCriticalSection(&g_csCriticalSection_bonjour);
	return TRUE;
}

//--------------------------------------//

static int CheckPort( const wchar_t* pprintername_, wchar_t* str_ )
{
    wchar_t pprintername[MAX_PATH];
	wchar_t ipString[MAX_PATH] = { 0 };
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
	for(DWORD i=0; i< cReturned; i++)
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
					//XcvData(hXcv, L"IPAddress", NULL, 0, (PBYTE)ipString, 256, &cReturned, &dwStatus);
					XcvData(hXcv, L"IPAddress", NULL, 0, (PBYTE)str_, 256, &cReturned, &dwStatus);
                    ClosePrinter(hXcv);
                }

				//TCHAR showIp[256] = L"";
				//wsprintf(showIp, L"\nCheckPort() hostname %s", ipString);
				//OutputDebugString(showIp);

				//std::wstring str(ipString);
				//if (str.substr(str.length() - 1, 1) == L".") //Is a hostname?
				//{
				//	if (strcmp(addr, "") == 0)
				//	{
				//		BonjourGetAddrInfo(ipString, str_);
				//	}
				//	else
				//	{
				//		if (TestIpConnected(addr))
				//		{
				//			::MultiByteToWideChar(CP_ACP, 0, addr, strlen(addr), str_, 100);
				//		}
				//		else
				//		{
				//			BonjourGetAddrInfo(ipString, str_);
				//		}
				//	}
				//}
				//else
				//{
				//	wmemcpy(str_, ipString, 100);
				//}

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

	EnterCriticalSection(&g_csCriticalSection_connect);
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
		while (nCount++ < 2 && !bWriteSuccess)
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
		
			int m_iSocketID = lpfnNetworkConnect(szAsciiIP, 9100, 1000);
			lpfnNetworkWrite(m_iSocketID, ptrInput, cbInput);

			if (ptrOutput && cbOutput > 0)
			{
				int cbRead = lpfnNetworkRead(m_iSocketID, ptrOutput, cbOutput);
				if (cbOutput == cbRead)
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
	LeaveCriticalSection(&g_csCriticalSection_connect);

    return nResult;
}

static int WriteDataViaUSB( const wchar_t* szPrinter, char* ptrInput, int cbInput, char* ptrOuput, int cbOutput )
{
    static char buffMax[MAX_SIZE_BUFF];
	OutputDebugStringToFileA("\r\n####VP:WriteDataViaUSB() begin");
    int nResult = _ACK;

	CGLUsb *m_GLusb = new CGLUsb();
 /*   wchar_t pszPort[MAX_PATH]         = { 0 };
    wchar_t pSymbolname[MAX_PATH]     = { 0 };
    wchar_t plocprintername[MAX_PATH] = { 0 };

    wcscpy_s( plocprintername, MAX_PATH, szPrinter );*/

   // if ( GetPrinterPortName( plocprintername, pszPort, MAX_PATH ) )
    {

	

		int nCount = 0;
		bool bWriteSuccess = false;
		while (nCount++ < 5 && !bWriteSuccess)
		{
		//	USBGetSymbolicNameByPortEx(pszPort, 0, 0, pSymbolname, MAX_PATH);
			//HANDLE ctlPipe = CreateFile(pSymbolname, GENERIC_WRITE | GENERIC_READ, FILE_SHARE_WRITE | FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, NULL);

			if (m_GLusb->CMDIO_OpenDevice() == TRUE)
			{
				char   inBuf[15];
				char	outBuf[MAX_DEVICEID_LEN] = { 0 };
				unsigned long     inBufSize = sizeof(inBuf);
				unsigned long     outBufSize = sizeof(outBuf);
				unsigned long     tmpBytes = 0;

				DWORD dwActualSize = 0;
				int nWriteTry = 20;
			
				//int rc = DeviceIoControl(ctlPipe,
				//	//IOCTL_GET_DEVICE_ID,
				//	IOCTL_USBPRINT_GET_1284_ID,
				//	inBuf,
				//	inBufSize,
				//	outBuf,
				//	outBufSize,
				//	&tmpBytes,
				//	NULL);

			//	if (!rc)
				{
					//TCHAR szMutexName[512] = { 0 };
					//wsprintf(szMutexName, L"Global\\LT%c-Port-%s", L'C', m_GLusb.m_strPort);
					//HANDLE hAccessMutex = CreateMutex(NULL, TRUE, szMutexName);
					//if (hAccessMutex != NULL && GetLastError() != ERROR_ALREADY_EXISTS)
					//{
					//	unsigned char inBuffer[522] = { 0 };
					//	unsigned char outBuffer[12] = { 0 };

					//	DWORD dwWritten = 0;

					//	memset(inBuffer, 0, sizeof(inBuffer));

					//	inBuffer[0] = 0x1B;
					//	inBuffer[1] = 0x4D;
					//	inBuffer[2] = 0x53;
					//	inBuffer[3] = 0x55;
					//	inBuffer[4] = 0xE0;
					//	inBuffer[5] = 0x2B;

					///*	OutputDebugStringToFileA("\r\n### vop WriteFile begin() ######\r\n");
					//	OutputDebugStringToFileA("\r\n### ctlPipe=%x ######\r\n", ctlPipe);*/


					//	//WriteFile(ctlPipe, inBuffer, 10, &dwWritten, NULL);
					//	m_GLusb.CMDIO_BulkWriteEx(0, inBuffer, 10);
					//	//WriteFile(ctlPipe, &inBuffer[10], 512, &dwWritten, NULL);
					//	m_GLusb.CMDIO_BulkWriteEx(0, &inBuffer[10], 512);

					//	// acorrding the mail from Gerard:
					//	// " The reply of wakeup cmd is defined in Toolbox cmd spec,
					//	// 12 bytes in all, 1c 00 e0 2b  00 00 00 00  00 00 00 00
					//	// ". We read the "Print Bulk-in" package.

					//	//ReadFile(ctlPipe, outBuffer, sizeof(outBuffer), &dwWritten, NULL);
					//	m_GLusb.CMDIO_BulkReadEx(0, outBuffer, sizeof(outBuffer));
					//	//CloseHandle(hAccessMutex);
					//	ReleaseMutex(hAccessMutex);
					//}
				}

				EnterCriticalSection(&g_csCriticalSection);
				OutputDebugStringToFileA("\r\n### vop EnterCriticalSection ######\r\n");

				//DeviceIoControl(ctlPipe, IOCTL_USBPRINT_VENDOR_GET_COMMAND, buffMax, 0, buffMax, MAX_SIZE_BUFF, &dwActualSize, NULL);
				//m_GLusb.CMDIO_BulkReadEx(0, buffMax, MAX_SIZE_BUFF, &dwActualSize);

			/*	while (0 == DeviceIoControl(ctlPipe, IOCTL_USBPRINT_VENDOR_SET_COMMAND, ptrInput, cbInput, NULL, 0, &dwActualSize, NULL)
					&& nWriteTry--)
				{
					Sleep(200);
				}
*/
				//DeviceIoControl(ctlPipe, IOCTL_USBPRINT_VENDOR_SET_COMMAND, ptrInput, cbInput, buffMax, MAX_SIZE_BUFF, &dwActualSize, NULL);

				//First 8 bytes header
				m_GLusb->CMDIO_BulkWriteEx(0, ptrInput, 8, &dwActualSize);

				//the rest
				m_GLusb->CMDIO_BulkWriteEx(0, ptrInput + 8, cbInput - 8, &dwActualSize);

				DWORD error = GetLastError();

				TCHAR szDebug[256] = { 0 };
				wsprintf(szDebug, _T("GetLastError() = %d"), error);
				OutputDebugString(szDebug);

			//	if (nWriteTry > 0)
				{
					int nErrCnt = 0;
					memset(buffMax, INIT_VALUE, sizeof(buffMax));
					while (m_GLusb->CMDIO_BulkReadEx(0, buffMax, sizeof(buffMax), &dwActualSize))
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
				/*else
				{
					OutputDebugStringToFileA("\r\n####VP:WriteDataViaUSB(): write usb timeout.");
					nResult = _SW_USB_WRITE_TIMEOUT;
				}*/

				LeaveCriticalSection(&g_csCriticalSection);
				OutputDebugStringToFileA("\r\n### vop LeaveCriticalSection ######\r\n");
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

				m_GLusb->CMDIO_CloseDevice();
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
  /*  else
    {
        nResult = _SW_USB_ERROR_OTHER;
		OutputDebugStringToFileA("\r\n####VP:WriteDataViaUSB(): GetPrinterPortName error.");
	}*/

	//delete m_GLusb;
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
   /* if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;*/

	OutputDebugStringToFileA("\r\n####VP:SetIPInfo() begin");

	int nResult = _ACK;
	/*wchar_t szIP[MAX_PATH] = { 0 };
    int nPortType = CheckPort( szPrinter, szIP );*/

   /* if ( PT_UNKNOWN == nPortType ) 
    {
        nResult = _SW_UNKNOWN_PORT;
    }
    else*/
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

		if (g_connectMode_usb != TRUE)
        {
            nResult = WriteDataViaNetwork(g_ipAddress, buffer, sizeof(COMM_HEADER)+128, NULL, 0 );
        }
        else 
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

USBAPI_API int __stdcall SetIPv6Info(
	const wchar_t* _szPrinter,
	BYTE _UseManualAddress,
	wchar_t* _wsManualAddress,
	UINT32 _ManualMask,
	wchar_t* _wsStatelessAddress1,
	wchar_t* _wsStatelessAddress2,
	wchar_t* _wsStatelessAddress3,
	wchar_t* _wsLinkLocalAddress,
	wchar_t* _wsIPv6ManualGatewayAddress,
	wchar_t* _wsAutoGatewayAddress,
	wchar_t* _wsAutoStatefulAddress,
	BYTE _DHCPv6)
{
	if (NULL == _szPrinter)
		return _SW_INVALID_PARAMETER;

	OutputDebugStringToFileA("\r\n####VP:SetIPv6Info() begin");

	//TCHAR szDebug[256] = { 0 };
	//wsprintf(szDebug, _T("\r\n####VP:SetIPv6Info() begin."));
	//OutputDebugString(szDebug);

	int nResult = _ACK;
	wchar_t szIP[MAX_PATH] = { 0 };
	int nPortType = CheckPort(_szPrinter, szIP);

	if (PT_UNKNOWN == nPortType)
	{
		nResult = _SW_UNKNOWN_PORT;
	}
	else
	{
		char* buffer = new char[sizeof(COMM_HEADER)+360];
		memset(buffer, INIT_VALUE, sizeof(COMM_HEADER)+360);
		COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>(buffer);

		ppkg->magic = MAGIC_NUM;
		ppkg->id = _LS_NETCMD;
		ppkg->len = 3 + 360;

		// For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
		// it just stand for the sub id. The real data length is defined by the lib
		ppkg->subid = 0x13;
		ppkg->len2 = 1;

		net_ipv6info_st* ptr_net_info = reinterpret_cast<net_ipv6info_st*>(buffer + sizeof(COMM_HEADER));

		ppkg->subcmd = 0x03;   // _NET_SETV6 0x03

		char cbManualAddress[40] = { 0 };
		char cbStatelessAddress1[44] = { 0 };
		char cbStatelessAddress2[44] = { 0 };
		char cbStatelessAddress3[44] = { 0 };
		char cbLinkLocalAddress[40] = { 0 };
		char cbIPv6ManualGatewayAddress[40] = { 0 };
		char cbAutoGatewayAddress[40] = { 0 };
		char cbAutoStatefulAddress[44] = { 0 };

		::WideCharToMultiByte(CP_ACP, 0, _wsManualAddress, -1, cbManualAddress, 40, NULL, NULL);
		::WideCharToMultiByte(CP_ACP, 0, _wsStatelessAddress1, -1, cbStatelessAddress1, 44, NULL, NULL);
		::WideCharToMultiByte(CP_ACP, 0, _wsStatelessAddress2, -1, cbStatelessAddress2, 44, NULL, NULL);
		::WideCharToMultiByte(CP_ACP, 0, _wsStatelessAddress3, -1, cbStatelessAddress3, 44, NULL, NULL);
		::WideCharToMultiByte(CP_ACP, 0, _wsLinkLocalAddress, -1, cbLinkLocalAddress, 40, NULL, NULL);
		::WideCharToMultiByte(CP_ACP, 0, _wsIPv6ManualGatewayAddress, -1, cbIPv6ManualGatewayAddress, 40, NULL, NULL);
		::WideCharToMultiByte(CP_ACP, 0, _wsAutoGatewayAddress, -1, cbAutoGatewayAddress, 40, NULL, NULL);
		::WideCharToMultiByte(CP_ACP, 0, _wsAutoStatefulAddress, -1, cbAutoStatefulAddress, 44, NULL, NULL);

		ptr_net_info->UseManualAddress = _UseManualAddress;
		ptr_net_info->ManualMask = _ManualMask;
		ptr_net_info->DHCPv6 = _DHCPv6;
		memcpy(ptr_net_info->ManualAddress, cbManualAddress, 40);
		memcpy(ptr_net_info->StatelessAddress1, cbStatelessAddress1, 44);
		memcpy(ptr_net_info->StatelessAddress2, cbStatelessAddress2, 44);
		memcpy(ptr_net_info->StatelessAddress3, cbStatelessAddress3, 44);
		memcpy(ptr_net_info->LinkLocalAddress, cbLinkLocalAddress, 40);
		memcpy(ptr_net_info->IPv6ManualGatewayAddress, cbIPv6ManualGatewayAddress, 40);
		memcpy(ptr_net_info->AutoGatewayAddress, cbAutoGatewayAddress, 40);
		memcpy(ptr_net_info->AutoStatefulAddress, cbAutoStatefulAddress, 44);


		if (PT_TCPIP == nPortType || PT_WSD == nPortType)
		{
			nResult = WriteDataViaNetwork(szIP, buffer, sizeof(COMM_HEADER)+360, NULL, 0);
		}
		else if (PT_USB == nPortType)
		{
			nResult = WriteDataViaUSB(_szPrinter, buffer, sizeof(COMM_HEADER)+360, NULL, 0);
		}

		if (buffer)
		{
			delete[] buffer;
			buffer = NULL;
		}
	}

	//TCHAR szDebug2[256] = { 0 };
	//wsprintf(szDebug2, _T("\r\n####VP:SetIPv6Info() end."));
	//OutputDebugString(szDebug2);

	OutputDebugStringToFileA("\r\n####VP:SetIPv6Info(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:SetIPv6Info() end");
	return nResult;
}

USBAPI_API bool __stdcall GetPrinterStatus( const wchar_t* szPrinter, BYTE* ptr_status, BYTE* ptr_toner, BYTE* pJob )
{
    bool isSuccess = false;

    if ( NULL == szPrinter )
        return isSuccess;

	wchar_t ip_address[MAX_PATH] = { 0 };
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
                // Fixed #0059441: According the mail from Victor,
                // DeviceIoControl wouldn't return fail when enter sleep mode.
                ps.PrinterStatus = __Unknown;
                *ptr_status = __Unknown;
                return true;
            }

            if (!DecodeStatusFromDeviceID((char*)outBuf+2, &ps)) 
            {
                ps.PrinterStatus = __Unknown;
            } 

			if (bOffline)
			{
                ps.TonelStatusLevelK = 0;

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

			LPFNNETWORKREADSTATUSEXPRO lpfn_net_getstatus_pro = NULL;
			lpfn_net_getstatus_pro = (LPFNNETWORKREADSTATUSEXPRO)GetProcAddress(hmod, "NetworkReadStatusExPro");

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

				/*	if (lpfn_net_getstatus(sz_ip, sz_community, &ps, sz_mfg, sz_mdl))*/
					if (lpfn_net_getstatus_pro(sz_ip, sz_community, &ps, sz_mfg, sz_mdl, 2000, 0)) //justin provided new NetIo dll 2015/8/27
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
	wcscpy_s(szPortName, iLen, pPrinterInfo->pPortName);
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
	wchar_t szIP[MAX_PATH] = { 0 };
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
  /*  if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;*/

	OutputDebugStringToFileA("\r\n####VP:GetSoftAp() begin");
	int nResult = _ACK;
	/*wchar_t szIP[MAX_PATH] = { 0 };
    int nPortType = CheckPort( szPrinter, szIP );

    if ( PT_UNKNOWN == nPortType ) 
    {
        nResult = _SW_UNKNOWN_PORT;
    }
    else*/
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

        if ( g_connectMode_usb != TRUE)
        {
            nResult = WriteDataViaNetwork( g_ipAddress, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+180 );
        }
        else
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
			OutputDebugStringToFileA("\r\nssid: %s", ssid);
            memcpy( pwd, pcmd_softap->pwd, 64); pwd[64] = 0;
			OutputDebugStringToFileA("\r\npwd: %s", pwd);

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
  /*  if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;*/

	OutputDebugStringToFileA("\r\n####VP:SetSoftAp() begin");
	int nResult = _ACK;
	/*wchar_t szIP[MAX_PATH] = {0};
    int nPortType = CheckPort( szPrinter, szIP );

    if ( PT_UNKNOWN == nPortType ) 
    {
        nResult = _SW_UNKNOWN_PORT;
    }
    else*/
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

		if (g_connectMode_usb != TRUE)
        {
            nResult = WriteDataViaNetwork( g_ipAddress, buffer, sizeof(COMM_HEADER)+180, NULL, 0 );
        }
        else
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
	/*if (NULL == szPrinter)
		return _SW_INVALID_PARAMETER;*/

	OutputDebugStringToFileA("\r\n####VP:ConfirmPassword() begin");
	int nResult = _ACK;
	//wchar_t szIP[MAX_PATH] = {0};
	//int nPortType = CheckPort(szPrinter, szIP);

	/*if (PT_UNKNOWN == nPortType)
	{
		nResult = _SW_UNKNOWN_PORT;
	}
	else*/
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

		if (g_connectMode_usb != TRUE)
		{
			nResult = WriteDataViaNetwork(g_ipAddress, buffer, sizeof(COMM_HEADER)+32, NULL, 0);
		}
		else 
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
   /* if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;*/

	OutputDebugStringToFileA("\r\n####VP:SetWiFiInfo() begin");
	int nResult = _ACK;
	//wchar_t szIP[MAX_PATH] = { 0 };
 //   int nPortType = CheckPort( szPrinter, szIP );

 //   if ( PT_UNKNOWN == nPortType ) 
 //   {
 //       nResult = _SW_UNKNOWN_PORT;
	//	OutputDebugStringToFileA("\r\n####VP:SetWiFiInfo(): PT_UNKNOWN == port_type");
 //   }
 //   else
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

		if (g_connectMode_usb != TRUE)
        {
            nResult = WriteDataViaNetwork(g_ipAddress, buffer, sizeof(COMM_HEADER)+180, NULL, 0 );
        }
        else
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
	char* pssid0, BYTE* ptr_encryption0, BYTE* ptr_connected0,
	char* pssid1, BYTE* ptr_encryption1, BYTE* ptr_connected1,
	char* pssid2, BYTE* ptr_encryption2, BYTE* ptr_connected2,
	char* pssid3, BYTE* ptr_encryption3, BYTE* ptr_connected3,
	char* pssid4, BYTE* ptr_encryption4, BYTE* ptr_connected4,
	char* pssid5, BYTE* ptr_encryption5, BYTE* ptr_connected5,
	char* pssid6, BYTE* ptr_encryption6, BYTE* ptr_connected6,
	char* pssid7, BYTE* ptr_encryption7, BYTE* ptr_connected7,
	char* pssid8, BYTE* ptr_encryption8, BYTE* ptr_connected8,
	char* pssid9, BYTE* ptr_encryption9, BYTE* ptr_connected9 )
{
  /*  if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;*/

	OutputDebugStringToFileA("\r\n####VP:GetApList() begin");
    int nResult = _ACK;
	//wchar_t szIP[MAX_PATH] = { 0 };
 //   int nPortType = CheckPort( szPrinter, szIP );

   /* if ( PT_UNKNOWN == nPortType ) 
    {
        nResult = _SW_UNKNOWN_PORT;
    }
    else*/
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
	
		if (g_connectMode_usb != TRUE)
        {
            nResult = WriteDataViaNetwork(g_ipAddress, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+340 );
        }
        else
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
                {   //If most significant bit is 1 (0x80) indicate connected status is true
                    case 0: *ptr_encryption0 = ptr_ap_info[i].encryption & 0x0f; *ptr_connected0 = ptr_ap_info[i].encryption & 0xf0; memcpy(pssid0, ptr_ap_info[i].ssid, 33); pssid0[33] = 0; break;
					case 1: *ptr_encryption1 = ptr_ap_info[i].encryption & 0x0f; *ptr_connected1 = ptr_ap_info[i].encryption & 0xf0; memcpy(pssid1, ptr_ap_info[i].ssid, 33); pssid1[33] = 0; break;
					case 2: *ptr_encryption2 = ptr_ap_info[i].encryption & 0x0f; *ptr_connected2 = ptr_ap_info[i].encryption & 0xf0; memcpy(pssid2, ptr_ap_info[i].ssid, 33); pssid2[33] = 0; break;
					case 3: *ptr_encryption3 = ptr_ap_info[i].encryption & 0x0f; *ptr_connected3 = ptr_ap_info[i].encryption & 0xf0; memcpy(pssid3, ptr_ap_info[i].ssid, 33); pssid3[33] = 0; break;
					case 4: *ptr_encryption4 = ptr_ap_info[i].encryption & 0x0f; *ptr_connected4 = ptr_ap_info[i].encryption & 0xf0; memcpy(pssid4, ptr_ap_info[i].ssid, 33); pssid4[33] = 0; break;
					case 5: *ptr_encryption5 = ptr_ap_info[i].encryption & 0x0f; *ptr_connected5 = ptr_ap_info[i].encryption & 0xf0; memcpy(pssid5, ptr_ap_info[i].ssid, 33); pssid5[33] = 0; break;
					case 6: *ptr_encryption6 = ptr_ap_info[i].encryption & 0x0f; *ptr_connected6 = ptr_ap_info[i].encryption & 0xf0; memcpy(pssid6, ptr_ap_info[i].ssid, 33); pssid6[33] = 0; break;
					case 7: *ptr_encryption7 = ptr_ap_info[i].encryption & 0x0f; *ptr_connected7 = ptr_ap_info[i].encryption & 0xf0; memcpy(pssid7, ptr_ap_info[i].ssid, 33); pssid7[33] = 0; break;
					case 8: *ptr_encryption8 = ptr_ap_info[i].encryption & 0x0f; *ptr_connected8 = ptr_ap_info[i].encryption & 0xf0; memcpy(pssid8, ptr_ap_info[i].ssid, 33); pssid8[33] = 0; break;
					case 9: *ptr_encryption9 = ptr_ap_info[i].encryption & 0x0f; *ptr_connected9 = ptr_ap_info[i].encryption & 0xf0; memcpy(pssid9, ptr_ap_info[i].ssid, 33); pssid9[33] = 0; break;
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
	wchar_t szIP[MAX_PATH] = { 0 };
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
	wchar_t szIP[MAX_PATH] = { 0 };
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
        { L"Lenovo M7208"   , REGKEY_UTILITY_LENOVO_3in1 } ,
        { L"Lenovo M7208W"  , REGKEY_UTILITY_LENOVO_3in1 } ,
        { L"Lenovo LJ2208"  , REGKEY_UTILITY_LENOVO_SFP  } ,
        { L"Lenovo LJ2208W" , REGKEY_UTILITY_LENOVO_SFP  } ,

		{ L"Lenovo M7218", REGKEY_UTILITY_LENOVO_3in1_7218 },
		{ L"Lenovo M7218W", REGKEY_UTILITY_LENOVO_3in1_7218 },
		{ L"Lenovo LJ2218", REGKEY_UTILITY_LENOVO_SFP_2218 },
		{ L"Lenovo LJ2218W", REGKEY_UTILITY_LENOVO_SFP_2218 },
    } ;

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
  /*  if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;*/
	
	OutputDebugStringToFileA("\r\n####VP:GetIPInfo() begin");

    int nResult = _ACK;
	//wchar_t szIP[MAX_PATH] = { 0 };
 //   int nPortType = CheckPort( szPrinter, szIP );
	
   /* if ( PT_UNKNOWN == nPortType ) 
    {
        nResult = _SW_UNKNOWN_PORT;
    }
    else*/
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

		if (g_connectMode_usb != TRUE)
        {
            nResult = WriteDataViaNetwork(g_ipAddress, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+128 );
        }
        else
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

				OutputDebugStringToFileA("\r\n#### IPMode:%d IPAddressMode:%d IPAddress:%d.%d.%d.%d SubnetMask:%d.%d.%d.%d GatewayAddress:%d.%d.%d.%d #### ", 
											ptr_net_info->IPMode, 
											ptr_net_info->IPAddressMode,
											ptr_net_info->IPAddress[0],
											ptr_net_info->IPAddress[1],
											ptr_net_info->IPAddress[2],
											ptr_net_info->IPAddress[3],
											ptr_net_info->SubnetMask[0],
											ptr_net_info->SubnetMask[1], 
											ptr_net_info->SubnetMask[2], 
											ptr_net_info->SubnetMask[3],
											ptr_net_info->GatewayAddress[0],
											ptr_net_info->GatewayAddress[1],
											ptr_net_info->GatewayAddress[2],
											ptr_net_info->GatewayAddress[3]);

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

USBAPI_API int __stdcall GetIpv6Info(const wchar_t* szPrinter,
	BYTE* UseManualAddress,
	char* ManualAddress,
	UINT32 *ManualMask,
	char* StatelessAddress1,
	char* StatelessAddress2,
	char* StatelessAddress3,
	char* LinkLocalAddress,
	char* IPv6ManualGatewayAddress,
	char* AutoGatewayAddress,
	char* AutoStatefulAddress,
	BYTE* DHCPv6)
{
	OutputDebugStringToFileA("\r\n####VP:GetIpv6Info() begin");

	int nResult = _ACK;
	wchar_t szIP[MAX_PATH] = { 0 };
	int nPortType = CheckPort(szPrinter, szIP);

	if (PT_UNKNOWN == nPortType)
	{
		nResult = _SW_UNKNOWN_PORT;
	}
	else
	{
		char* buffer = new char[sizeof(COMM_HEADER)+360];
		memset(buffer, INIT_VALUE, sizeof(COMM_HEADER)+360);
		COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>(buffer);

		ppkg->magic = MAGIC_NUM;
		ppkg->id = _LS_NETCMD;
		ppkg->len = 3;

		// For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
		// it just stand for the sub id. The real data length is defined by the lib
		ppkg->subid = 0x13;
		ppkg->len2 = 0x01;

		net_ipv6info_st* ptr_net_info = reinterpret_cast<net_ipv6info_st*>(buffer + sizeof(COMM_HEADER));

		ppkg->subcmd = 02;       // _NET_GETV6  0x02

		if (PT_TCPIP == nPortType || PT_WSD == nPortType)
		{
			nResult = WriteDataViaNetwork(szIP, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+360);
		}
		else if (PT_USB == nPortType)
		{
			nResult = WriteDataViaUSB(szPrinter, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+360);
		}

		if (_ACK == nResult)
		{
			if (ptr_net_info->UseManualAddress == 0
				|| ptr_net_info->UseManualAddress == 1)
			{
				*UseManualAddress = ptr_net_info->UseManualAddress;
				memcpy(ManualAddress, ptr_net_info->ManualAddress, 40); ManualAddress[40] = 0;
				*ManualMask = ptr_net_info->ManualMask;
				memcpy(StatelessAddress1, ptr_net_info->StatelessAddress1, 44); StatelessAddress1[44] = 0;
				memcpy(StatelessAddress2, ptr_net_info->StatelessAddress2, 44); StatelessAddress2[44] = 0;
				memcpy(StatelessAddress3, ptr_net_info->StatelessAddress3, 44); StatelessAddress3[44] = 0;
				memcpy(LinkLocalAddress, ptr_net_info->LinkLocalAddress, 40); LinkLocalAddress[40] = 0;
				memcpy(IPv6ManualGatewayAddress, ptr_net_info->IPv6ManualGatewayAddress, 40); IPv6ManualGatewayAddress[40] = 0;
				memcpy(AutoGatewayAddress, ptr_net_info->AutoGatewayAddress, 40); AutoGatewayAddress[40] = 0;
				memcpy(AutoStatefulAddress, ptr_net_info->AutoStatefulAddress, 44); AutoStatefulAddress[44] = 0;
				*DHCPv6 = ptr_net_info->DHCPv6;

				nResult = _ACK;
			}
			else
			{
				nResult = _SW_INVALID_RETURN_VALUE;
			}
		}

		if (buffer)
		{
			delete[] buffer;
			buffer = NULL;
		}
	}

	OutputDebugStringToFileA("\r\n####VP:GetIpv6Info(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:GetIpv6Info() end");
	return nResult;
}

USBAPI_API int __stdcall GetWiFiInfo(const wchar_t* szPrinter, UINT8* ptr_wifienable, char* ssid, char* pwd, UINT8* ptr_encryption, UINT8* ptr_wepKeyId)
{
   /* if ( NULL == szPrinter )
        return _SW_INVALID_PARAMETER;*/

	OutputDebugStringToFileA("\r\n####VP:GetWiFiInfo() begin");

    int nResult = _ACK;
	//wchar_t szIP[MAX_PATH] = { 0 };
 //   int nPortType = CheckPort( szPrinter, szIP );

 //   if ( PT_UNKNOWN == nPortType ) 
 //   {
 //       nResult = _SW_UNKNOWN_PORT;
 //   }
 //   else
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

		if (g_connectMode_usb != TRUE)
        {
            nResult = WriteDataViaNetwork(g_ipAddress, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+180 );
        }
        else
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
	wchar_t szIP[MAX_PATH] = { 0 };
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
	wchar_t szIP[MAX_PATH] = { 0 };

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

USBAPI_API int __stdcall GetUserCenterInfo(const wchar_t* szPrinter, char* _2ndSerialNO, UINT32* _totalCounter, char* _serialNO4AIO)
{
	if (NULL == szPrinter)
		return _SW_INVALID_PARAMETER;

	OutputDebugStringToFileA("\r\n####VP:GetUserCenterInfo() begin");

	int nResult = _ACK;
	wchar_t szIP[MAX_PATH] = { 0 };
	int nPortType = CheckPort(szPrinter, szIP);

	if (PT_UNKNOWN == nPortType)
	{
		nResult = _SW_UNKNOWN_PORT;
	}
	else
	{
		char* buffer = new char[sizeof(COMM_HEADER)+64];
		memset(buffer, INIT_VALUE, sizeof(COMM_HEADER)+64);
		COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>(buffer);

		ppkg->magic = MAGIC_NUM;
		ppkg->id = _LS_PRNCMD;
		ppkg->len = 3;

		// For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
		// it just stand for the sub id. The real data length is defined by the lib
		ppkg->subid = 0x13;
		ppkg->len2 = 1;
		ppkg->subcmd = _PRN_USER_CENTER;

		if (PT_TCPIP == nPortType || PT_WSD == nPortType)
		{
			nResult = WriteDataViaNetwork(szIP, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+64);
		}
		else if (PT_USB == nPortType)
		{
			nResult = WriteDataViaUSB(szPrinter, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+64);
		}

		if (_ACK == nResult)
		{
			cmdst_user_center* pcmd_user_center = reinterpret_cast<cmdst_user_center*>(buffer + sizeof(COMM_HEADER));

			*_totalCounter = pcmd_user_center->_totalCounter;
			memcpy(_2ndSerialNO,  pcmd_user_center->_2ndSerialNO,  20); _2ndSerialNO[20] = 0;
			memcpy(_serialNO4AIO, pcmd_user_center->_SerialNO4AIO, 16); _2ndSerialNO[16] = 0;
		}

		if (buffer)
		{
			delete[] buffer;
			buffer = NULL;
		}
	}

	OutputDebugStringToFileA("\r\n####VP:GetUserCenterInfo(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:GetUserCenterInfo() end");
	return nResult;
}

USBAPI_API int __stdcall GetFWInfo(const wchar_t* szPrinter, char * FWVersion)
{
	OutputDebugStringToFileA("\r\n####VP:GetFWInfo() begin");

	int nResult = _ACK;
	//wchar_t szIP[MAX_PATH] = { 0 };
	//int nPortType = CheckPort(szPrinter, szIP);

	//if (PT_UNKNOWN == nPortType)
	//{
	//	nResult = _SW_UNKNOWN_PORT;
	//}
	//else
	{
		char* buffer = new char[sizeof(COMM_HEADER)+480];
		memset(buffer, INIT_VALUE, sizeof(COMM_HEADER)+480);
		COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>(buffer);

		ppkg->magic = MAGIC_NUM;
		ppkg->id = _LS_PRNCMD;
		ppkg->len = 3;

		// For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
		// it just stand for the sub id. The real data length is defined by the lib
		ppkg->subid = 0x13;
		ppkg->len2 = 0x01;

		fw_info_st* ptr_info = reinterpret_cast<fw_info_st*>(buffer + sizeof(COMM_HEADER));

		ppkg->subcmd = _PRN_INFO;    

		if (g_connectMode_usb != TRUE)
		{
			nResult = WriteDataViaNetwork(g_ipAddress, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+480);
		}
		else
		{
			nResult = WriteDataViaUSB(szPrinter, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+480);
		}

		if (_ACK == nResult)
		{
			memcpy(FWVersion, ptr_info->cFirmwareVersion, 32); FWVersion[32] = 0;
			nResult = _ACK;		
		}

		if (buffer)
		{
			delete[] buffer;
			buffer = NULL;
		}
	}

	OutputDebugStringToFileA("\r\n####VP:GetFWInfo(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:GetFWInfo() end");
	return nResult;
}

USBAPI_API int __stdcall SetTonerEnd(const wchar_t* szPrinter, BYTE isEnable)
{
	if (NULL == szPrinter)
		return _SW_INVALID_PARAMETER;
	OutputDebugStringToFileA("\r\n####VP:SetTonerEnd() begin");

	int nResult = _ACK;
	wchar_t szIP[MAX_PATH] = { 0 };
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
		ppkg->subcmd = _PRN_TONEREND_SET;

		BYTE* ptrData = reinterpret_cast<BYTE*>(buffer + sizeof(COMM_HEADER));
		*ptrData = isEnable;

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

	OutputDebugStringToFileA("\r\n####VP:SetTonerEnd(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:SetTonerEnd() end");
	return nResult;
}

USBAPI_API int __stdcall GetTonerEnd(const wchar_t* szPrinter, BYTE* ptrIsEnable)
{
	if (NULL == szPrinter)
		return _SW_INVALID_PARAMETER;

	OutputDebugStringToFileA("\r\n####VP:GetTonerEnd() begin");

	int nResult = _ACK;
	wchar_t szIP[MAX_PATH] = { 0 };

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
		ppkg->len = 3;

		// For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
		// it just stand for the sub id. The real data length is defined by the lib
		ppkg->subid = 0x13;
		ppkg->len2 = 1;
		ppkg->subcmd = _PRN_TONEREND_GET;

		if (PT_TCPIP == nPortType || PT_WSD == nPortType)
		{
			nResult = WriteDataViaNetwork(szIP, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+1);
		}
		else if (PT_USB == nPortType)
		{
			nResult = WriteDataViaUSB(szPrinter, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+1);
		}

		if (_ACK == nResult)
		{
			BYTE* ptr = reinterpret_cast<BYTE*>(buffer + sizeof(COMM_HEADER));

			if (1 == *ptr || 0 == *ptr)
			{
				*ptrIsEnable = *ptr;
				nResult = _ACK;
			}
			else
			{
				nResult = _SW_INVALID_RETURN_VALUE;
			}
		}

		if (buffer)
		{
			delete[] buffer;
			buffer = NULL;
		}
	}

	OutputDebugStringToFileA("\r\n####VP:GetTonerEnd(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:GetTonerEnd() end");
	return nResult;
}

USBAPI_API int __stdcall SetPowerOff(const wchar_t* szPrinter, BYTE isEnable)
{
	if (NULL == szPrinter)
		return _SW_INVALID_PARAMETER;
	OutputDebugStringToFileA("\r\n####VP:SetPowerOff() begin");

	int nResult = _ACK;
	wchar_t szIP[MAX_PATH] = { 0 };
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
		ppkg->subcmd = _PRN_POWEROFF_SET;

		BYTE* ptrData = reinterpret_cast<BYTE*>(buffer + sizeof(COMM_HEADER));
		*ptrData = isEnable;

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

	OutputDebugStringToFileA("\r\n####VP:SetPowerOff(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:SetPowerOff() end");
	return nResult;
}

USBAPI_API int __stdcall GetPowerOff(const wchar_t* szPrinter, BYTE* ptrIsEnable)
{
	if (NULL == szPrinter)
		return _SW_INVALID_PARAMETER;

	OutputDebugStringToFileA("\r\n####VP:GetPowerOff() begin");

	int nResult = _ACK;
	wchar_t szIP[MAX_PATH] = { 0 };

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
		ppkg->len = 3;

		// For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
		// it just stand for the sub id. The real data length is defined by the lib
		ppkg->subid = 0x13;
		ppkg->len2 = 1;
		ppkg->subcmd = _PRN_POWEROFF_GET;

		if (PT_TCPIP == nPortType || PT_WSD == nPortType)
		{
			nResult = WriteDataViaNetwork(szIP, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+1);
		}
		else if (PT_USB == nPortType)
		{
			nResult = WriteDataViaUSB(szPrinter, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+1);
		}

		if (_ACK == nResult)
		{
			BYTE* ptr = reinterpret_cast<BYTE*>(buffer + sizeof(COMM_HEADER));

			if (1 == *ptr || 0 == *ptr)
			{
				*ptrIsEnable = *ptr;
				nResult = _ACK;
			}
			else
			{
				nResult = _SW_INVALID_RETURN_VALUE;
			}
		}

		if (buffer)
		{
			delete[] buffer;
			buffer = NULL;
		}
	}

	OutputDebugStringToFileA("\r\n####VP:GetPowerOff(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:GetPowerOff() end");
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
    static const int RETSCAN_OK              = 0;
    static const int RETSCAN_ERRORDLL        = 1;
    static const int RETSCAN_OPENFAIL        = 2;
    static const int RETSCAN_ERRORPARAMETER  = 3;
    static const int RETSCAN_NO_ENOUGH_SPACE = 5;
    static const int RETSCAN_ERROR_PORT      = 6;
    static const int RETSCAN_CANCEL          = 7;
    static const int RETSCAN_BUSY            = 8;
    static const int RETSCAN_ERROR           = 9;

    if ( false == 3*DoseHasEnoughSpace( szOrig, width, height ) )
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
            int devmonCode = DEVMON_STATUS_OK;

            devmonCode = obj.Open( &ch, szIP, false, Capability, PushScanFlag );
            if ( DEVMON_STATUS_OK == devmonCode )
            {
                HANDLE hFileOrig  = NULL;

                ULONG ulBytesRead      = 0;
                LONG  lPercentComplete = 0;
                static BYTE headOrig [4*1024];           // buffer for bitmap header structure

                DWORD dwHeadSizeOrig  = 0;    // Size of bitmap header and color table.
                int nColPixelNumOrig  = 0;    // Number of pixel in one column of orig imgage.
                int nLinePixelNumOrig = 0;    // Number of pixel in one line of orig imgage.
                int cbStrideRawOrig   = 0;    // Bytes actually needed per line of orig image.
                int cbStridePadOrig   = 0;    // Bytes per line after padding of orig image.
                BYTE* strideOrig      = NULL; // Buffer pointer of single line.
                long lWroteOrig       = 0;
                int nRowsCnt          = 0;    // Mount of row has read from scanner.

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
		
                devmonCode = obj.StartScan(); 
                if ( DEVMON_STATUS_OK != devmonCode )
                {
                    // According the e-mail from Jerry Chen, this two value present busy.
                    if ( DEVMON_ERROR_IN_USE == devmonCode 
                            || DEVMON_ERROR_DRIVER_IN_USE == devmonCode )
                    {
                        nResult = RETSCAN_BUSY;
                    }
                    else
                    {
                        nResult = RETSCAN_ERROR;
                    }
                }
                else 
                {
                    devmonCode = obj.SetScanParameterEx( &scanparam );
                    if ( DEVMON_STATUS_OK != devmonCode )
                    {
                        if ( DEVMON_ERROR_IN_USE == devmonCode 
                                || DEVMON_ERROR_DRIVER_IN_USE == devmonCode )
                            nResult = RETSCAN_BUSY;
                        else
                            nResult = RETSCAN_ERROR;
                    }
                    else
                    {
                        FillBitmapHeader(headOrig, scanMode, nLinePixelNumOrig, nColPixelNumOrig, resolution, 1.0, &dwHeadSizeOrig);

                        CreateFileWithSize( szOrig, reinterpret_cast<const BITMAPFILEHEADER*>(headOrig)->bfSize );

                        hFileOrig  = CreateFile( szOrig , GENERIC_READ | GENERIC_WRITE, 0, (LPSECURITY_ATTRIBUTES) NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, (HANDLE) NULL);

                        WriteFile( hFileOrig , headOrig , dwHeadSizeOrig , &dwHeadSizeOrig , NULL);

                        int nMod = nColPixelNumOrig/100;
                        bCancelScanning = false;

                        while ( true )
                        {
                            if ( true == bCancelScanning )
                                break;

                            devmonCode = obj.ReadData(strideOrig, cbStridePadOrig, &ulBytesRead, &lPercentComplete);

                            if (  DEVMON_ERROR_SCAN_STATUS_STOP == devmonCode )
                            {
                                bCancelScanning = true;
                                break;
                            }
                            else if ( DEVMON_STATUS_OK != devmonCode )
                            {
                                break;
                            }

                            if ( ++nRowsCnt > nColPixelNumOrig )
                                break;

                            if ( 0 == nRowsCnt%nMod )
                                ::SendNotifyMessage( HWND_BROADCAST, uMsg, nRowsCnt*100/nColPixelNumOrig, 0); 

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

                            if ( nRowsCnt >= nColPixelNumOrig )
                                ::SendNotifyMessage( HWND_BROADCAST, uMsg, 100, 0); 
                            else
                                nResult = RETSCAN_ERROR;
                        }
                        else
                        {
                            // TODO: clear cache file. add sync mechanism.
                            nResult = RETSCAN_CANCEL;
                        }
                    }

                    obj.StopScan();
                }

                obj.Close();

                if ( NULL != strideOrig )
                {
                    delete[] strideOrig;
                    strideOrig = NULL;
                }
            }
            else if ( DEVMON_ERROR_IN_USE == devmonCode )
            {
                nResult = RETSCAN_BUSY;
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
        wcsncpy_s( szBuffer, sizeof(szBuffer)/sizeof(wchar_t), szPath, ptr-szPath );

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

USBAPI_API int __stdcall CheckPortAPI2(const wchar_t* szPrinter, char* ipAddress)
{
	wchar_t szIP[MAX_PATH] = { 0 };
	char ip[100] = { 0 };

	int nPortType = CheckPort(szPrinter, szIP);
	::WideCharToMultiByte(CP_ACP, 0, szIP, -1, ip, 100, NULL, NULL);

	memcpy(ipAddress, ip, 100);

	return nPortType;
}

USBAPI_API void __stdcall CancelScanning()
{
    bCancelScanning = true;
}

/*++
Routine Description:
                Determine if the current country is using metric system.
Arguments:
                NONE
Return Value:
                TRUE if the current country uses metric system
                FALSE otherwise
--*/
USBAPI_API BOOL __stdcall IsMetricCountry()
{
    INT    cChar        = 0;
    LONG   lCountryCode = 0;
    LPTSTR pwstr        = NULL;
    BOOL   bMetric      = FALSE;

    // Determine the size of the buffer needed to retrieve information.
    cChar = GetLocaleInfoW( LOCALE_USER_DEFAULT, LOCALE_ICOUNTRY, NULL, 0);

    if (cChar > 0) 
    {
        // Allocate the necessary buffers.
        pwstr = new wchar_t[cChar];

        if (pwstr != NULL )
        {
            // We now have a buffer, so get the country code.
            cChar = GetLocaleInfoW( LOCALE_USER_DEFAULT, LOCALE_ICOUNTRY, pwstr, cChar);

            if (cChar > 0) {

                lCountryCode = _wtol(pwstr);

                // This is the Win31 algorithm based on AT&T international
                // dialing codes.

                // Reference: https://msdn.microsoft.com/en-us/library/windows/hardware/ff561927(v=vs.85).aspx
                // Use the default system locale obtained from GetLocaleInfo to determine metric or non-metric paper size.
                //     Non-metric if default system locale is:
                //         CTRY_UNITED_STATES, or
                //         CTRY_CANADA, or
                //         Greater than or equal to 50, but less than 60 and not CTRY_BRAZIL, or
                //         Greater than or equal to 500, but less than 600
                bMetric = ((lCountryCode == CTRY_UNITED_STATES) ||
                        (lCountryCode == CTRY_CANADA) ||
                        (lCountryCode >= 50 && lCountryCode < 60 && lCountryCode != CTRY_BRAZIL) ||
                        (lCountryCode >= 500 && lCountryCode < 600)) ? FALSE : TRUE;

            } 

            delete[] pwstr;
            pwstr = NULL;
        }
    }

    return bMetric;
}

USBAPI_API int __stdcall GetWifiChangeStatus(const wchar_t* szPrinter, BYTE* wifiInit)
{
	//if (NULL == szPrinter)
	//	return _SW_INVALID_PARAMETER;

	OutputDebugStringToFileA("\r\n####VP:GetWifiChangeStatus() begin");

	int nResult = _ACK;
	//wchar_t szIP[MAX_PATH] = {0};
	//int nPortType = CheckPort(szPrinter, szIP);

	/*if (PT_UNKNOWN == nPortType)
	{
		nResult = _SW_UNKNOWN_PORT;
	}
	else*/
	{
		char* buffer = new char[sizeof(COMM_HEADER)+16];
		memset(buffer, INIT_VALUE, sizeof(COMM_HEADER)+16);
		COMM_HEADER* ppkg = reinterpret_cast<COMM_HEADER*>(buffer);

		ppkg->magic = MAGIC_NUM;
		ppkg->id = _LS_WIFICMD;
		ppkg->len = 3;

		// For the simple data setting, e.g. copy/scan/prn/wifi/net, SubID is always 0x13, len is always 0x01,
		// it just stand for the sub id. The real data length is defined by the lib
		ppkg->subid = 0x13;
		ppkg->len2 = 1;
		ppkg->subcmd = 0x08; // _WIFI_STATUS 0x08

		if (g_connectMode_usb != TRUE)
		{
			nResult = WriteDataViaNetwork(g_ipAddress, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+1);
		}
		else
		{
			nResult = WriteDataViaUSB(szPrinter, buffer, sizeof(COMM_HEADER), buffer, sizeof(COMM_HEADER)+1);
		}

		if (_ACK == nResult)
		{
			BYTE* ptr = reinterpret_cast<BYTE*>(buffer + sizeof(COMM_HEADER));

			if (1 == *ptr || 0 == *ptr)
			{
				*wifiInit = *ptr;
				nResult = _ACK;
			}
			else
			{
				nResult = _SW_INVALID_RETURN_VALUE;
			}
		}

		if (buffer)
		{
			delete[] buffer;
			buffer = NULL;
		}
	}

	OutputDebugStringToFileA("\r\n####VP:GetWifiChangeStatus(): nResult == 0x%x", nResult);
	OutputDebugStringToFileA("\r\n####VP:GetWifiChangeStatus() end");
	return nResult;
}
