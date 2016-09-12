

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.00.0603 */
/* at Mon Sep 05 09:38:17 2016
 */
/* Compiler settings for hpgtblues.idl:
    Oicf, W1, Zp8, env=Win64 (32b run), target_arch=AMD64 8.00.0603 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __hpgtblues_h__
#define __hpgtblues_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IScanner_FWD_DEFINED__
#define __IScanner_FWD_DEFINED__
typedef interface IScanner IScanner;

#endif 	/* __IScanner_FWD_DEFINED__ */


#ifndef __IScannerClassFactory_FWD_DEFINED__
#define __IScannerClassFactory_FWD_DEFINED__
typedef interface IScannerClassFactory IScannerClassFactory;

#endif 	/* __IScannerClassFactory_FWD_DEFINED__ */


#ifndef __IButton_FWD_DEFINED__
#define __IButton_FWD_DEFINED__
typedef interface IButton IButton;

#endif 	/* __IButton_FWD_DEFINED__ */


#ifndef __IADF_FWD_DEFINED__
#define __IADF_FWD_DEFINED__
typedef interface IADF IADF;

#endif 	/* __IADF_FWD_DEFINED__ */


#ifndef __IADF2_FWD_DEFINED__
#define __IADF2_FWD_DEFINED__
typedef interface IADF2 IADF2;

#endif 	/* __IADF2_FWD_DEFINED__ */


#ifndef __IADFSetFeature_FWD_DEFINED__
#define __IADFSetFeature_FWD_DEFINED__
typedef interface IADFSetFeature IADFSetFeature;

#endif 	/* __IADFSetFeature_FWD_DEFINED__ */


#ifndef __IADF3_FWD_DEFINED__
#define __IADF3_FWD_DEFINED__
typedef interface IADF3 IADF3;

#endif 	/* __IADF3_FWD_DEFINED__ */


#ifndef __IADF4_FWD_DEFINED__
#define __IADF4_FWD_DEFINED__
typedef interface IADF4 IADF4;

#endif 	/* __IADF4_FWD_DEFINED__ */


#ifndef __IXPA_FWD_DEFINED__
#define __IXPA_FWD_DEFINED__
typedef interface IXPA IXPA;

#endif 	/* __IXPA_FWD_DEFINED__ */


#ifndef __IXPA2_FWD_DEFINED__
#define __IXPA2_FWD_DEFINED__
typedef interface IXPA2 IXPA2;

#endif 	/* __IXPA2_FWD_DEFINED__ */


#ifndef __IXPA3_FWD_DEFINED__
#define __IXPA3_FWD_DEFINED__
typedef interface IXPA3 IXPA3;

#endif 	/* __IXPA3_FWD_DEFINED__ */


#ifndef __IXPA4_FWD_DEFINED__
#define __IXPA4_FWD_DEFINED__
typedef interface IXPA4 IXPA4;

#endif 	/* __IXPA4_FWD_DEFINED__ */


#ifndef __INVRAM_FWD_DEFINED__
#define __INVRAM_FWD_DEFINED__
typedef interface INVRAM INVRAM;

#endif 	/* __INVRAM_FWD_DEFINED__ */


#ifndef __INVRAMWrapper_FWD_DEFINED__
#define __INVRAMWrapper_FWD_DEFINED__
typedef interface INVRAMWrapper INVRAMWrapper;

#endif 	/* __INVRAMWrapper_FWD_DEFINED__ */


#ifndef __INVRAMWrapper2_FWD_DEFINED__
#define __INVRAMWrapper2_FWD_DEFINED__
typedef interface INVRAMWrapper2 INVRAMWrapper2;

#endif 	/* __INVRAMWrapper2_FWD_DEFINED__ */


#ifndef __IControlPanel_FWD_DEFINED__
#define __IControlPanel_FWD_DEFINED__
typedef interface IControlPanel IControlPanel;

#endif 	/* __IControlPanel_FWD_DEFINED__ */


#ifndef __IControlPanel2_FWD_DEFINED__
#define __IControlPanel2_FWD_DEFINED__
typedef interface IControlPanel2 IControlPanel2;

#endif 	/* __IControlPanel2_FWD_DEFINED__ */


#ifndef __IControlPanelXML_FWD_DEFINED__
#define __IControlPanelXML_FWD_DEFINED__
typedef interface IControlPanelXML IControlPanelXML;

#endif 	/* __IControlPanelXML_FWD_DEFINED__ */


#ifndef __IRGBMatrix_FWD_DEFINED__
#define __IRGBMatrix_FWD_DEFINED__
typedef interface IRGBMatrix IRGBMatrix;

#endif 	/* __IRGBMatrix_FWD_DEFINED__ */


#ifndef __ILog_FWD_DEFINED__
#define __ILog_FWD_DEFINED__
typedef interface ILog ILog;

#endif 	/* __ILog_FWD_DEFINED__ */


#ifndef __ICalibrationSettings_FWD_DEFINED__
#define __ICalibrationSettings_FWD_DEFINED__
typedef interface ICalibrationSettings ICalibrationSettings;

#endif 	/* __ICalibrationSettings_FWD_DEFINED__ */


#ifndef __ILampTimer_FWD_DEFINED__
#define __ILampTimer_FWD_DEFINED__
typedef interface ILampTimer ILampTimer;

#endif 	/* __ILampTimer_FWD_DEFINED__ */


#ifndef __IHWProperty_FWD_DEFINED__
#define __IHWProperty_FWD_DEFINED__
typedef interface IHWProperty IHWProperty;

#endif 	/* __IHWProperty_FWD_DEFINED__ */


#ifndef __IStringTable_FWD_DEFINED__
#define __IStringTable_FWD_DEFINED__
typedef interface IStringTable IStringTable;

#endif 	/* __IStringTable_FWD_DEFINED__ */


#ifndef __IConvolution_FWD_DEFINED__
#define __IConvolution_FWD_DEFINED__
typedef interface IConvolution IConvolution;

#endif 	/* __IConvolution_FWD_DEFINED__ */


#ifndef __IDither_FWD_DEFINED__
#define __IDither_FWD_DEFINED__
typedef interface IDither IDither;

#endif 	/* __IDither_FWD_DEFINED__ */


#ifndef __ILampInstantWarmup_FWD_DEFINED__
#define __ILampInstantWarmup_FWD_DEFINED__
typedef interface ILampInstantWarmup ILampInstantWarmup;

#endif 	/* __ILampInstantWarmup_FWD_DEFINED__ */


#ifndef __ICompression_FWD_DEFINED__
#define __ICompression_FWD_DEFINED__
typedef interface ICompression ICompression;

#endif 	/* __ICompression_FWD_DEFINED__ */


#ifndef __IReserve_FWD_DEFINED__
#define __IReserve_FWD_DEFINED__
typedef interface IReserve IReserve;

#endif 	/* __IReserve_FWD_DEFINED__ */


#ifndef __IADFDiagnostic_FWD_DEFINED__
#define __IADFDiagnostic_FWD_DEFINED__
typedef interface IADFDiagnostic IADFDiagnostic;

#endif 	/* __IADFDiagnostic_FWD_DEFINED__ */


#ifndef __ISensor_FWD_DEFINED__
#define __ISensor_FWD_DEFINED__
typedef interface ISensor ISensor;

#endif 	/* __ISensor_FWD_DEFINED__ */


#ifndef __ISPF_FWD_DEFINED__
#define __ISPF_FWD_DEFINED__
typedef interface ISPF ISPF;

#endif 	/* __ISPF_FWD_DEFINED__ */


#ifndef __IMFG_FWD_DEFINED__
#define __IMFG_FWD_DEFINED__
typedef interface IMFG IMFG;

#endif 	/* __IMFG_FWD_DEFINED__ */


#ifndef __IResCCD_FWD_DEFINED__
#define __IResCCD_FWD_DEFINED__
typedef interface IResCCD IResCCD;

#endif 	/* __IResCCD_FWD_DEFINED__ */


#ifndef __IResolution_FWD_DEFINED__
#define __IResolution_FWD_DEFINED__
typedef interface IResolution IResolution;

#endif 	/* __IResolution_FWD_DEFINED__ */


#ifndef __IEventCallback_FWD_DEFINED__
#define __IEventCallback_FWD_DEFINED__
typedef interface IEventCallback IEventCallback;

#endif 	/* __IEventCallback_FWD_DEFINED__ */


#ifndef __IEventNotify_FWD_DEFINED__
#define __IEventNotify_FWD_DEFINED__
typedef interface IEventNotify IEventNotify;

#endif 	/* __IEventNotify_FWD_DEFINED__ */


#ifndef __IColorDropout_FWD_DEFINED__
#define __IColorDropout_FWD_DEFINED__
typedef interface IColorDropout IColorDropout;

#endif 	/* __IColorDropout_FWD_DEFINED__ */


#ifndef __IColorDropout2_FWD_DEFINED__
#define __IColorDropout2_FWD_DEFINED__
typedef interface IColorDropout2 IColorDropout2;

#endif 	/* __IColorDropout2_FWD_DEFINED__ */


#ifndef __iImprinter_FWD_DEFINED__
#define __iImprinter_FWD_DEFINED__
typedef interface iImprinter iImprinter;

#endif 	/* __iImprinter_FWD_DEFINED__ */


#ifndef __IExposure_FWD_DEFINED__
#define __IExposure_FWD_DEFINED__
typedef interface IExposure IExposure;

#endif 	/* __IExposure_FWD_DEFINED__ */


#ifndef __IFWUpdate_FWD_DEFINED__
#define __IFWUpdate_FWD_DEFINED__
typedef interface IFWUpdate IFWUpdate;

#endif 	/* __IFWUpdate_FWD_DEFINED__ */


#ifndef __IPowerManagement_FWD_DEFINED__
#define __IPowerManagement_FWD_DEFINED__
typedef interface IPowerManagement IPowerManagement;

#endif 	/* __IPowerManagement_FWD_DEFINED__ */


#ifndef __IHPScnMgr_FWD_DEFINED__
#define __IHPScnMgr_FWD_DEFINED__
typedef interface IHPScnMgr IHPScnMgr;

#endif 	/* __IHPScnMgr_FWD_DEFINED__ */


#ifndef __IHPScnMgr2_FWD_DEFINED__
#define __IHPScnMgr2_FWD_DEFINED__
typedef interface IHPScnMgr2 IHPScnMgr2;

#endif 	/* __IHPScnMgr2_FWD_DEFINED__ */


#ifndef __IHPScnMgr3_FWD_DEFINED__
#define __IHPScnMgr3_FWD_DEFINED__
typedef interface IHPScnMgr3 IHPScnMgr3;

#endif 	/* __IHPScnMgr3_FWD_DEFINED__ */


#ifndef __IHPScnMgr4_FWD_DEFINED__
#define __IHPScnMgr4_FWD_DEFINED__
typedef interface IHPScnMgr4 IHPScnMgr4;

#endif 	/* __IHPScnMgr4_FWD_DEFINED__ */


#ifndef __IHPScnMgr5_FWD_DEFINED__
#define __IHPScnMgr5_FWD_DEFINED__
typedef interface IHPScnMgr5 IHPScnMgr5;

#endif 	/* __IHPScnMgr5_FWD_DEFINED__ */


#ifndef __IHPScnMgr6_FWD_DEFINED__
#define __IHPScnMgr6_FWD_DEFINED__
typedef interface IHPScnMgr6 IHPScnMgr6;

#endif 	/* __IHPScnMgr6_FWD_DEFINED__ */


#ifndef __IHPScnMgr7_FWD_DEFINED__
#define __IHPScnMgr7_FWD_DEFINED__
typedef interface IHPScnMgr7 IHPScnMgr7;

#endif 	/* __IHPScnMgr7_FWD_DEFINED__ */


#ifndef __IHPRUMgr_FWD_DEFINED__
#define __IHPRUMgr_FWD_DEFINED__
typedef interface IHPRUMgr IHPRUMgr;

#endif 	/* __IHPRUMgr_FWD_DEFINED__ */


#ifndef __IPutLog_FWD_DEFINED__
#define __IPutLog_FWD_DEFINED__
typedef interface IPutLog IPutLog;

#endif 	/* __IPutLog_FWD_DEFINED__ */


#ifndef __IGetLog_FWD_DEFINED__
#define __IGetLog_FWD_DEFINED__
typedef interface IGetLog IGetLog;

#endif 	/* __IGetLog_FWD_DEFINED__ */


#ifndef __IStiHandler_FWD_DEFINED__
#define __IStiHandler_FWD_DEFINED__
typedef interface IStiHandler IStiHandler;

#endif 	/* __IStiHandler_FWD_DEFINED__ */


#ifndef __HPGTBB_FWD_DEFINED__
#define __HPGTBB_FWD_DEFINED__

#ifdef __cplusplus
typedef class HPGTBB HPGTBB;
#else
typedef struct HPGTBB HPGTBB;
#endif /* __cplusplus */

#endif 	/* __HPGTBB_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_hpgtblues_0000_0000 */
/* [local] */ 


#ifdef TULIP_NAMESPACE
namespace Tulip {
#endif

#ifndef _ISCANNER_TYPES_DEFINED
#define _ISCANNER_TYPES_DEFINED

enum TULIP_ERRORS
    {
        TERR_NONE	= 0,
        TERR_INVALID_SESSION	= 1,
        TERR_INVALID_PARAMETER	= 2,
        TERR_SCANNER_NOT_FOUND	= 3,
        TERR_SCANNER_NOT_LOCKED	= 4,
        TERR_SCANNER_LOCK_FAILED	= 5,
        TERR_SCANNER_LAMP_OFF	= 6,
        TERR_CALIBRATION_FAILED	= 7,
        TERR_OUT_OF_MEMORY	= 8,
        TERR_UNKNOWN	= 9,
        TERR_NOT_IMPLEMENTED	= 10,
        TERR_SCANNER_HOME_FAILED	= 11,
        TERR_SCANNER_COMMUNICATION_FAILED	= 12,
        TERR_INVALID_TOOLKIT	= 13,
        TERR_INVALID_PROPERTY	= 14,
        TERR_SCANNER_END_OF_PAGE	= 15,
        TERR_CANCEL_PRESSED	= 16,
        TERR_SCANNER_LAMP_FAILED	= 17,
        TERR_NET_SOCKET_BUSY	= 18,
        TERR_THERMAL_FAN_FAILED	= 19,
        TERR_BACKGROUND_COLOR_CHANGE_FAILED	= 20,
        TERR_ROTATION_BYTES_EXCEEDED	= 21
    } ;
#define TULIP_E_BASE			0
#define TULIP_E_SNF			10
#define TULIP_E_SCONF		30
#define TULIP_E_BDR			50
#define TULIP_E_LAMP		70
#define TULIP_E_OFFSET		0x200
static const int TULIP_E_DEVICE_ABORT						= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_BASE );
static const int TULIP_E_SNF_REDISCOVERY					= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_SNF );
static const int TULIP_E_SNF_REDISCOVERY_NO_DEVICE			= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_SNF + 1);
static const int TULIP_E_SNF_REDISCOVERY_NO_NETWORK_DATA	= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_SNF + 2);
static const int TULIP_E_SNF_REDISCOVERY_MUTEX				= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_SNF + 3);
static const int TULIP_E_SNF_REDISCOVERY_LIBRARY			= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_SNF + 4);
static const int TULIP_E_SNF_REDISCOVERY_CCI				= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_SNF + 5);
static const int TULIP_E_SCONF_REJECTED						= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_SCONF);
static const int TULIP_E_SCONF_REDISCOVERY_BADNAME			= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_SCONF + 1);
static const int TULIP_E_SCONF_SCL_ERROR					= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_SCONF + 2);
static const int TULIP_E_SCONF_BUSY							= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_SCONF + 3);
static const int TULIP_E_SCONF_FIREWALL						= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_OFFSET + TULIP_E_SCONF + 4);
static const int TULIP_E_BDR_GENERAL						= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_BDR);
static const int TULIP_E_BDR_JPEG							= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_BDR + 1);
static const int TULIP_E_BDR_REALIGNMENT					= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_BDR + 2);
static const int TULIP_E_BDR_IP								= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_BDR + 3);
static const int TULIP_E_LAMP_FLATBED						= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_LAMP);
static const int TULIP_E_LAMP_ADF							= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_LAMP + 1);
static const int TULIP_E_LAMP_TMA							= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  TULIP_E_LAMP + 2);

enum SCANNER_LOCKTYPE
    {
        T_PROCESS_LOCK	= 0,
        T_THREAD_LOCK	= 1
    } ;

enum SCANNER_MODE
    {
        T_COLOR	= 0x1,
        T_GREYSCALE	= 0x2,
        T_BW	= 0x4,
        T_COLOR_S3L	= 0x8,
        T_COLOR_S3L_RAW	= 0x10,
        T_ACD_COLOR	= 0x20,
        T_ACD_CGB	= 0x40
    } ;

enum SCANNER_OPTIONS
    {
        T_PREVIEW	= 0x1,
        T_PIXELCOORDINATES	= 0x2,
        T_QUALITY	= 0x4,
        T_BW_DITHER	= 0x8,
        T_IR	= 0x10,
        T_DEWARP	= 0x20,
        T_BW_INVERT	= 0x40,
        T_HW_ROTATE_90	= 0x80,
        T_HW_ROTATE_270	= 0x100,
        T_BW_THRESHOLD	= 0x200,
        T_HW_ROTATE_AUTO	= 0x400,
        T_HW_ROTATE_UPRIGHT	= 0x400,
        T_TONEMAP_PARAMETERS	= 0x800,
        T_NOISE_REMOVAL	= 0x1000,
        T_SHARPSMOOTH	= 0x2000,
        T_CONTENT_TYPE	= 0x4000,
        T_PAGECOORDINATES	= 0x8000,
        T_DESCREEN	= 0x10000,
        T_BLANK_PAGE_DETECT	= 0x20000,
        T_BW_DITHER_BAYER	= 0x40000,
        T_BW_DITHER_ERROR_DIFFUSION	= 0x80000,
        T_AUTO_DESKEW	= 0x100000,
        T_AUTO_CROP	= 0x200000,
        T_AUTO_ORIENTATION	= 0x400000,
        T_BW_THRESHOLD_AUTO	= 0x800000,
        T_EDGE_ERASE	= 0x1000000,
        T_AUTO_EXPOSURE	= 0x2000000,
        T_AUTO_EXCLUSION_AREAS	= 0x4000000,
        T_BLANK_PAGE_REMOVAL	= 0x8000000,
        T_BLANK_PAGE_SENSITIVITY	= 0x10000000,
        T_AUTO_CROP_TYPE	= 0x20000000,
        T_ACD_SENSITIVITY	= 0x40000000
    } ;

enum SCANNER_CHANNEL
    {
        T_RED	= 0x1,
        T_GREEN	= 0x2,
        T_BLUE	= 0x4,
        T_NTSC	= 0x8,
        T_GREY_CCD	= 0x10,
        T_GREY_CCD_EMULATION	= 0x20
    } ;

enum SCANNER_IO_TYPE
    {
        T_USB	= 1,
        T_PAR	= 2,
        T_SCSI	= 3,
        T_DOT4	= 4,
        T_IP	= 5,
        T_USB2	= 6,
        T_1394	= 7,
        T_USB_IP	= 8
    } ;

enum SCANNER_PAPER_PATH
    {
        T_PATH_FLATBED	= 0,
        T_PATH_SCROLL	= 1
    } ;

enum SCANNER_PIXEL_ORDER
    {
        T_RGB	= 1,
        T_BGR	= 2,
        T_RGBABC	= 3,
        T_BGRABC	= 4,
        T_RGB_CYBORG	= 5,
        T_BGR_CYBORG	= 6
    } ;

enum SCANNER_PIXEL_PACKING
    {
        T_PACKED	= 1,
        T_PLANAR	= 2
    } ;

enum SCANNER_METHOD
    {
        T_FLATBED	= 0,
        T_ADF	= 1,
        T_XPA_POSITIVE	= 2,
        T_XPA_NEGATIVE	= 3
    } ;

enum SCANNER_FEED_EDGE
    {
        T_FEED_TOP	= 0,
        T_FEED_LEFT	= 1,
        T_FEED_BOTTOM	= 2,
        T_FEED_RIGHT	= 3
    } ;

enum SCANNER_CONTENT_TYPE
    {
        T_CONTENT_ANY	= 0,
        T_CONTENT_PHOTO	= 1,
        T_CONTENT_DOCUMENT	= 2,
        T_CONTENT_3D_OBJECT	= 3
    } ;
struct TONEMAP_PARAMETERS
    {
    WORD wGamma;
    BYTE byBrightness;
    BYTE byContrast;
    WORD wHighlight;
    WORD wShadow;
    } ;

enum SCANNER_FILL_COLOR
    {
        T_BLACK	= 0x1,
        T_WHITE	= 0x2,
        T_PAGE_COLOR	= 0x4
    } ;

enum SCANNER_AUTOCROP_TYPE
    {
        T_PAGE_CROP	= 0x1,
        T_CONTENT_CROP	= 0x2
    } ;

enum SCANNER_ACD_2_SCANMODE_MAP
    {
        T_CBB	= 1,
        T_CHB	= 2,
        T_GBB	= 3,
        T_GHB	= 4,
        T_GGG	= 5,
        T_CGG	= 6,
        T_CGB	= 7,
        T_CCB	= 8,
        T_CCC	= 9,
        T_HBB	= 10,
        T_CCG	= 11,
        T_CHG	= 12,
        T_GGB	= 13,
        T_HHB	= 14,
        T_GHG	= 15,
        T_HGG	= 16,
        T_HHG	= 17,
        T_CB	= 1,
        T_CH	= 2,
        T_GB	= 3,
        T_GH	= 4,
        T_GG	= 5,
        T_CG	= 6,
        T_CC	= 9,
        T_HB	= 10,
        T_HG	= 16
    } ;

enum SCANNER_AUTOORIENT_LANGUAGE
    {
        T_AO_LATIN	= 0x1,
        T_AO_GREEK	= 0x2,
        T_AO_RUSSIAN	= 0x4,
        T_AO_JAPANESE	= 0x8,
        T_AO_SIMPLIFIED_CHINESE	= 0x10,
        T_AO_KOREAN	= 0x20,
        T_AO_TRADITIONAL_CHINESE	= 0x40
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_tonemap[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct TONEMAP_PARAMETERS) == 8);
struct SCANNER_CAPABILITIES
    {
    BYTE byModelNumber[ 16 ];
    WORD wLampWarmupTime;
    WORD wOpticalResolution;
    WORD wMaximumXResolution;
    WORD wMaximumYResolution;
    WORD wMinimumResolution;
    DWORD dwMaximumWidth;
    DWORD dwMinimumWidth;
    DWORD dwMaximumHeight;
    DWORD dwMinimumHeight;
    WORD wRawPixelOrder;
    WORD wPixelPacking;
    DWORD dwScanModesSupported;
    DWORD dwScanOptionsSupported;
    WORD wGreyChannelsSupported;
    WORD wPaperPath;
    WORD wGammaTableSize;
    BYTE byColorBitsPerChannel[ 8 ];
    BYTE byGreyBitsPerChannel[ 8 ];
    DWORD dwGammaEntriesPerChannel;
    DWORD dwGammaMaxEntryValue;
    WORD dwGammaBytesPerEntry;
    WORD wMaxGreyCCDResolution;
    DWORD dwMaxRotationBytes;
    BYTE byCanSharpen;
    BYTE byCanSmooth;
    small cSharpSmoothDefault;
    BYTE byNoiseRemovalDefault;
    BYTE byBlankPageSensitivityLevels;
    BYTE byACDsensitivityLevels;
    BYTE bySupportedFillColors;
    BYTE bySupportedAutoCropTypes;
    BYTE byACDsensitivityLevelsBW;
    WORD wFiller;
    DWORD dwAutoOrientSupportedLanguages;
    WORD wReserved[ 6 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_scancap[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct SCANNER_CAPABILITIES) == 124);
struct SCANNER_PARAMETERS
    {
    DWORD dwLeft;
    DWORD dwTop;
    DWORD dwWidth;
    DWORD dwLength;
    DWORD dwScanMode;
    DWORD dwScanOptions;
    BYTE byBitsPerChannel;
    BYTE byEdgeEraseTop;
    WORD wXResolution;
    WORD wYResolution;
    WORD wGreyChannel;
    WORD wScanMethod;
    DWORD dwBWThreshold;
    WORD wFeedEdge;
    struct TONEMAP_PARAMETERS tonemap;
    small cSharpSmoothLevel;
    BYTE byNoiseRemovalLevel;
    BYTE byContentType;
    BYTE byBlankPageSensitivity;
    BYTE byEdgeEraseBottom;
    BYTE byFillColor;
    DWORD dwPageWidth;
    DWORD dwPageHeight;
    BYTE byAutoCropType;
    BYTE byACDmapping;
    BYTE byACDsensitivity;
    BYTE byEdgeEraseLeft;
    BYTE byEdgeEraseRight;
    BYTE byAutoOrientQualityMode;
    BYTE byACDsensitivityBW;
    BYTE byAutoOrientLanguage;
    BYTE byAutoXclusionTop;
    BYTE byAutoXclusionLeft;
    BYTE byAutoXclusionRight;
    BYTE byAutoXclusionBottom;
    WORD wReserved[ 4 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_scanparam[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct SCANNER_PARAMETERS) == 84);
struct BUFFER_INFO
    {
    DWORD dwPixelWidth;
    DWORD dwPixelHeight;
    DWORD dwBytesPerLine;
    DWORD dwReserved[ 10 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_bufinfo[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct BUFFER_INFO) == 52);
#endif //_ISCANNER_TYPES_DEFINED


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0000_v0_0_s_ifspec;

#ifndef __IScanner_INTERFACE_DEFINED__
#define __IScanner_INTERFACE_DEFINED__

/* interface IScanner */
/* [version][unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IScanner;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("26955994-8038-444B-82FD-794A41367DD7")
    IScanner : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OpenScannerSession( 
            /* [string][in] */ LPOLESTR pszDevName,
            /* [in] */ DWORD dwDevIOType,
            /* [out] */ DWORD *phSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CloseScannerSession( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LockScanner( 
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wLockType,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnlockScanner( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetLampStatus( 
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbLampOn,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LampOn( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LampOff( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetScannerCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ struct SCANNER_CAPABILITIES *pScannerCapabilities,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBufferInfo( 
            /* [in] */ DWORD hSession,
            /* [in] */ struct SCANNER_PARAMETERS *pScanParam,
            /* [out] */ struct BUFFER_INFO *pBufferInfo,
            /* [out] */ DWORD *pdwdErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE InitializeScanner( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResetScanner( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetScannerParameters( 
            /* [in] */ DWORD hSession,
            /* [in] */ struct SCANNER_PARAMETERS *pScanParam,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetGammaTable( 
            /* [in] */ DWORD hSession,
            /* [in] */ ULONG uSz,
            /* [size_is][in] */ BYTE *pbyRed,
            /* [size_is][in] */ BYTE *pbyGreen,
            /* [size_is][in] */ BYTE *pbyBlue,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StartScan( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE StopScan( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReadScan( 
            /* [in] */ DWORD hSession,
            /* [size_is][out] */ BYTE *pbyBuffer,
            /* [in] */ DWORD dwBytes,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReadScanEx( 
            /* [in] */ DWORD hSession,
            /* [size_is][out] */ BYTE *pbyBuffer,
            /* [in] */ DWORD dwBytes,
            /* [out] */ DWORD *pdwBytesRead,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IScannerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IScanner * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IScanner * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IScanner * This);
        
        HRESULT ( STDMETHODCALLTYPE *OpenScannerSession )( 
            IScanner * This,
            /* [string][in] */ LPOLESTR pszDevName,
            /* [in] */ DWORD dwDevIOType,
            /* [out] */ DWORD *phSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *CloseScannerSession )( 
            IScanner * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *LockScanner )( 
            IScanner * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wLockType,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *UnlockScanner )( 
            IScanner * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetLampStatus )( 
            IScanner * This,
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbLampOn,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *LampOn )( 
            IScanner * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *LampOff )( 
            IScanner * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerCapabilities )( 
            IScanner * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct SCANNER_CAPABILITIES *pScannerCapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetBufferInfo )( 
            IScanner * This,
            /* [in] */ DWORD hSession,
            /* [in] */ struct SCANNER_PARAMETERS *pScanParam,
            /* [out] */ struct BUFFER_INFO *pBufferInfo,
            /* [out] */ DWORD *pdwdErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *InitializeScanner )( 
            IScanner * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ResetScanner )( 
            IScanner * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetScannerParameters )( 
            IScanner * This,
            /* [in] */ DWORD hSession,
            /* [in] */ struct SCANNER_PARAMETERS *pScanParam,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetGammaTable )( 
            IScanner * This,
            /* [in] */ DWORD hSession,
            /* [in] */ ULONG uSz,
            /* [size_is][in] */ BYTE *pbyRed,
            /* [size_is][in] */ BYTE *pbyGreen,
            /* [size_is][in] */ BYTE *pbyBlue,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *StartScan )( 
            IScanner * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *StopScan )( 
            IScanner * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ReadScan )( 
            IScanner * This,
            /* [in] */ DWORD hSession,
            /* [size_is][out] */ BYTE *pbyBuffer,
            /* [in] */ DWORD dwBytes,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ReadScanEx )( 
            IScanner * This,
            /* [in] */ DWORD hSession,
            /* [size_is][out] */ BYTE *pbyBuffer,
            /* [in] */ DWORD dwBytes,
            /* [out] */ DWORD *pdwBytesRead,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IScannerVtbl;

    interface IScanner
    {
        CONST_VTBL struct IScannerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IScanner_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IScanner_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IScanner_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IScanner_OpenScannerSession(This,pszDevName,dwDevIOType,phSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> OpenScannerSession(This,pszDevName,dwDevIOType,phSession,pdwErrorCode) ) 

#define IScanner_CloseScannerSession(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> CloseScannerSession(This,hSession,pdwErrorCode) ) 

#define IScanner_LockScanner(This,hSession,wLockType,dwTimeout,pdwErrorCode)	\
    ( (This)->lpVtbl -> LockScanner(This,hSession,wLockType,dwTimeout,pdwErrorCode) ) 

#define IScanner_UnlockScanner(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> UnlockScanner(This,hSession,pdwErrorCode) ) 

#define IScanner_GetLampStatus(This,hSession,pbLampOn,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetLampStatus(This,hSession,pbLampOn,pdwErrorCode) ) 

#define IScanner_LampOn(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> LampOn(This,hSession,pdwErrorCode) ) 

#define IScanner_LampOff(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> LampOff(This,hSession,pdwErrorCode) ) 

#define IScanner_GetScannerCapabilities(This,hSession,pScannerCapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetScannerCapabilities(This,hSession,pScannerCapabilities,pdwErrorCode) ) 

#define IScanner_GetBufferInfo(This,hSession,pScanParam,pBufferInfo,pdwdErrorCode)	\
    ( (This)->lpVtbl -> GetBufferInfo(This,hSession,pScanParam,pBufferInfo,pdwdErrorCode) ) 

#define IScanner_InitializeScanner(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> InitializeScanner(This,hSession,pdwErrorCode) ) 

#define IScanner_ResetScanner(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> ResetScanner(This,hSession,pdwErrorCode) ) 

#define IScanner_SetScannerParameters(This,hSession,pScanParam,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetScannerParameters(This,hSession,pScanParam,pdwErrorCode) ) 

#define IScanner_SetGammaTable(This,hSession,uSz,pbyRed,pbyGreen,pbyBlue,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetGammaTable(This,hSession,uSz,pbyRed,pbyGreen,pbyBlue,pdwErrorCode) ) 

#define IScanner_StartScan(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> StartScan(This,hSession,pdwErrorCode) ) 

#define IScanner_StopScan(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> StopScan(This,hSession,pdwErrorCode) ) 

#define IScanner_ReadScan(This,hSession,pbyBuffer,dwBytes,pdwErrorCode)	\
    ( (This)->lpVtbl -> ReadScan(This,hSession,pbyBuffer,dwBytes,pdwErrorCode) ) 

#define IScanner_ReadScanEx(This,hSession,pbyBuffer,dwBytes,pdwBytesRead,pdwErrorCode)	\
    ( (This)->lpVtbl -> ReadScanEx(This,hSession,pbyBuffer,dwBytes,pdwBytesRead,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IScanner_INTERFACE_DEFINED__ */


#ifndef __IScannerClassFactory_INTERFACE_DEFINED__
#define __IScannerClassFactory_INTERFACE_DEFINED__

/* interface IScannerClassFactory */
/* [helpstring][unique][uuid][object] */ 


EXTERN_C const IID IID_IScannerClassFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A147D0E8-0CCA-4CE1-8B88-B991B9A9ED53")
    IScannerClassFactory : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE CreateScannerObject( 
            /* [in] */ LPOLESTR pwszDevModel,
            /* [out] */ IScanner **ppScanner) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IScannerClassFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IScannerClassFactory * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IScannerClassFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IScannerClassFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *CreateScannerObject )( 
            IScannerClassFactory * This,
            /* [in] */ LPOLESTR pwszDevModel,
            /* [out] */ IScanner **ppScanner);
        
        END_INTERFACE
    } IScannerClassFactoryVtbl;

    interface IScannerClassFactory
    {
        CONST_VTBL struct IScannerClassFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IScannerClassFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IScannerClassFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IScannerClassFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IScannerClassFactory_CreateScannerObject(This,pwszDevModel,ppScanner)	\
    ( (This)->lpVtbl -> CreateScannerObject(This,pwszDevModel,ppScanner) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IScannerClassFactory_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0002 */
/* [local] */ 




extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0002_v0_0_s_ifspec;

#ifndef __IButton_INTERFACE_DEFINED__
#define __IButton_INTERFACE_DEFINED__

/* interface IButton */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IButton;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B59EE25B-8A4A-4798-8BB4-AE7AD33FBD4F")
    IButton : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetButtonCount( 
            /* [in] */ DWORD hSession,
            /* [out] */ WORD *pwButtonCount,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetButtonState( 
            /* [in] */ DWORD hSession,
            /* [in] */ BYTE byIndex,
            /* [out] */ BYTE *pbyState,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetButtonStateEx( 
            /* [in] */ ULONG hSession,
            /* [in] */ BYTE byCount,
            /* [size_is][out] */ BYTE *pState,
            /* [out] */ ULONG *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResetButton( 
            /* [in] */ DWORD hSession,
            /* [in] */ BYTE byIndex,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ResetButtonsEx( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IButtonVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IButton * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IButton * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IButton * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetButtonCount )( 
            IButton * This,
            /* [in] */ DWORD hSession,
            /* [out] */ WORD *pwButtonCount,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetButtonState )( 
            IButton * This,
            /* [in] */ DWORD hSession,
            /* [in] */ BYTE byIndex,
            /* [out] */ BYTE *pbyState,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetButtonStateEx )( 
            IButton * This,
            /* [in] */ ULONG hSession,
            /* [in] */ BYTE byCount,
            /* [size_is][out] */ BYTE *pState,
            /* [out] */ ULONG *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ResetButton )( 
            IButton * This,
            /* [in] */ DWORD hSession,
            /* [in] */ BYTE byIndex,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ResetButtonsEx )( 
            IButton * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IButtonVtbl;

    interface IButton
    {
        CONST_VTBL struct IButtonVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IButton_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IButton_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IButton_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IButton_GetButtonCount(This,hSession,pwButtonCount,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetButtonCount(This,hSession,pwButtonCount,pdwErrorCode) ) 

#define IButton_GetButtonState(This,hSession,byIndex,pbyState,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetButtonState(This,hSession,byIndex,pbyState,pdwErrorCode) ) 

#define IButton_GetButtonStateEx(This,hSession,byCount,pState,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetButtonStateEx(This,hSession,byCount,pState,pdwErrorCode) ) 

#define IButton_ResetButton(This,hSession,byIndex,pdwErrorCode)	\
    ( (This)->lpVtbl -> ResetButton(This,hSession,byIndex,pdwErrorCode) ) 

#define IButton_ResetButtonsEx(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> ResetButtonsEx(This,hSession,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IButton_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0003 */
/* [local] */ 






enum ADF_ERRORS
    {
        TERR_ADF_JAMMED	= 100,
        TERR_ADF_EMPTY	= 101,
        TERR_ADF_END_OF_PAGE	= 102,
        TERR_ADF_HATCH_OPEN	= 103,
        TERR_APF_JAMMED	= 104,
        TERR_APF_GENERAL_ERROR	= 105,
        TERR_ADF_DUPLEX_PAGE_TOO_SHORT	= 106,
        TERR_ADF_DUPLEX_PAGE_TOO_LONG	= 107,
        TERR_ADF_PICK_FAILED	= 108,
        TERR_ADF_DPD	= 109,
        TERR_ADF_MPD	= 109,
        TERR_ADF_MPD_SENSOR_FAILED	= 110,
        TERR_ADF_INPUT_TRAY_FAILED	= 111,
        TERR_ADF_INPUT_TRAY_OVERLOADED	= 112
    } ;

enum ADF_TYPE
    {
        T_ADF_FLATBED	= 0,
        T_ADF_SCROLL	= 1
    } ;
struct ADF_CAPABILITIES
    {
    BYTE byHasADF;
    BYTE byModelNumber[ 16 ];
    WORD wADFType;
    WORD wOpticalResolution;
    WORD wMaximumXResolution;
    WORD wMaximumYResolution;
    WORD wMinimumResolution;
    DWORD dwMaximumWidth;
    DWORD dwMinimumWidth;
    DWORD dwMaximumHeight;
    DWORD dwMinimumHeight;
    DWORD dwScanModesSupported;
    DWORD dwScanOptionsSupported;
    WORD wGreyChannelsSupported;
    WORD wGammaTableSize;
    BYTE byColorBitsPerChannel[ 8 ];
    BYTE byGreyBitsPerChannel[ 8 ];
    DWORD dwGammaEntriesPerChannel;
    DWORD dwGammaMaxEntryValue;
    WORD dwGammaBytesPerEntry;
    WORD wMaxGreyCCDResolution;
    BYTE byCanSharpen;
    BYTE byCanSmooth;
    small cSharpSmoothDefault;
    BYTE byNoiseRemovalDefault;
    BYTE byBlankPageSensitivityLevels;
    BYTE byACDSensitivityLevels;
    BYTE bySupportedFillColors;
    BYTE bySupportedAutoCropTypes;
    BYTE byACDsensitivityLevelsBW;
    WORD wReserved1;
    DWORD dwAutoOrientSupportedLanguages;
    WORD wReserved[ 8 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_adf[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct ADF_CAPABILITIES) == 116);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0003_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0003_v0_0_s_ifspec;

#ifndef __IADF_INTERFACE_DEFINED__
#define __IADF_INTERFACE_DEFINED__

/* interface IADF */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IADF;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F0FE1A19-9C93-4418-BBBD-BF87482F5FE9")
    IADF : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetADFCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ struct ADF_CAPABILITIES *pADFCapabilities,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ADFPaperFeed( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ADFIsReady( 
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbIsReady,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IADFVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IADF * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IADF * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IADF * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetADFCapabilities )( 
            IADF * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct ADF_CAPABILITIES *pADFCapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADFPaperFeed )( 
            IADF * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADFIsReady )( 
            IADF * This,
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbIsReady,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IADFVtbl;

    interface IADF
    {
        CONST_VTBL struct IADFVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IADF_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IADF_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IADF_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IADF_GetADFCapabilities(This,hSession,pADFCapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetADFCapabilities(This,hSession,pADFCapabilities,pdwErrorCode) ) 

#define IADF_ADFPaperFeed(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADFPaperFeed(This,hSession,pdwErrorCode) ) 

#define IADF_ADFIsReady(This,hSession,pbIsReady,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADFIsReady(This,hSession,pbIsReady,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IADF_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0004 */
/* [local] */ 


enum ADF_OPTIONS
    {
        T_ADF_DUPLEX	= 0x1,
        T_ADF_SINGLEPAGE_FLOW	= 0x2,
        T_ADF_MULTIPAGE_FLOW	= 0x4,
        T_ADF_FAST_DUPLEX_MODE	= 0x8,
        T_ADF_FLIP_DUPLEX_PAGE_DATA	= 0x10,
        T_ADF_FLIP_DUPLEX_PAGE_DATA_SHORTSIDE	= 0x20,
        T_ADF_DPD_LENGTH	= 0x40,
        T_ADF_DPD_THICKNESS	= 0x80,
        T_ADF_MPD	= 0x80,
        T_ADF_LONGPAGE_MODE	= 0x100,
        T_ADF_BLACK_BACKGROUND	= 0x200,
        T_ADF_DUPLEX_TABLET_LAYOUT	= 0x400,
        T_ADF_OVERSCAN	= 0x800,
        T_ADF_MPD_IGNORE_TOP	= 0x1000,
        T_ADF_MPD_IGNORE_MIDDLE	= 0x2000,
        T_ADF_MPD_IGNORE_BOTTOM	= 0x4000,
        T_ADF_ELP_MODE	= 0x8000,
        T_ADF_ELP_MODE_SLOW	= 0x10000,
        T_ADF_MPD_IGNORE_LENGTH	= 0x20000
    } ;
struct ADF2_CAPABILITIES
    {
    BYTE byHasADF;
    BYTE byModelNumber[ 16 ];
    WORD wADFType;
    BYTE byADFCanDetectEndOfPage;
    BYTE byADFFeedCanDetectLengthOfPage;
    DWORD dwADFOptionsSupported;
    WORD wOpticalResolution;
    WORD wMaximumXResolution;
    WORD wMaximumYResolution;
    WORD wMinimumResolution;
    DWORD dwMaximumWidth;
    DWORD dwMinimumWidth;
    DWORD dwMaximumHeight;
    DWORD dwMinimumHeight;
    DWORD dwDuplexMaximumWidth;
    DWORD dwDuplexMinimumWidth;
    DWORD dwDuplexMaximumHeight;
    DWORD dwDuplexMinimumHeight;
    DWORD dwScanModesSupported;
    DWORD dwScanOptionsSupported;
    WORD wGreyChannelsSupported;
    BYTE byColorBitsPerChannel[ 8 ];
    BYTE byGreyBitsPerChannel[ 8 ];
    DWORD dwGammaEntriesPerChannel;
    DWORD dwGammaMaxEntryValue;
    WORD dwGammaBytesPerEntry;
    WORD wMaxGreyCCDResolution;
    DWORD dwLongPageMaximumHeight;
    DWORD dwDuplexLongPageMaximumHeight;
    DWORD dwOverscanYDelta;
    BYTE byCanSharpen;
    BYTE byCanSmooth;
    small cSharpSmoothDefault;
    BYTE byNoiseRemovalDefault;
    BYTE byBlankPageSensitivityLevels;
    BYTE byACDSensitivityLevels;
    BYTE bySupportedFillColors;
    BYTE bySupportedAutoCropTypes;
    BYTE byACDsensitivityLevelsBW;
    BYTE byReserved1;
    WORD wMaxELPResolution;
    DWORD dwELPMaximumHeight;
    DWORD dwDuplexELPMaximumHeight;
    DWORD dwAutoOrientSupportedLanguages;
    WORD wReserved[ 22 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_adf2[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct ADF2_CAPABILITIES) == 188);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0004_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0004_v0_0_s_ifspec;

#ifndef __IADF2_INTERFACE_DEFINED__
#define __IADF2_INTERFACE_DEFINED__

/* interface IADF2 */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IADF2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D6F31B83-3EFC-4B12-A7BE-1DE76C606981")
    IADF2 : public IADF
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetADF2Capabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ struct ADF2_CAPABILITIES *pADF2Capabilities,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetADFOptions( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwADFOptionsSelected,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ADFEjectMedia( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ADF2PaperFeed( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwLength,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IADF2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IADF2 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IADF2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IADF2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetADFCapabilities )( 
            IADF2 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct ADF_CAPABILITIES *pADFCapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADFPaperFeed )( 
            IADF2 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADFIsReady )( 
            IADF2 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbIsReady,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetADF2Capabilities )( 
            IADF2 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct ADF2_CAPABILITIES *pADF2Capabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetADFOptions )( 
            IADF2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwADFOptionsSelected,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADFEjectMedia )( 
            IADF2 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADF2PaperFeed )( 
            IADF2 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwLength,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IADF2Vtbl;

    interface IADF2
    {
        CONST_VTBL struct IADF2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IADF2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IADF2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IADF2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IADF2_GetADFCapabilities(This,hSession,pADFCapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetADFCapabilities(This,hSession,pADFCapabilities,pdwErrorCode) ) 

#define IADF2_ADFPaperFeed(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADFPaperFeed(This,hSession,pdwErrorCode) ) 

#define IADF2_ADFIsReady(This,hSession,pbIsReady,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADFIsReady(This,hSession,pbIsReady,pdwErrorCode) ) 


#define IADF2_GetADF2Capabilities(This,hSession,pADF2Capabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetADF2Capabilities(This,hSession,pADF2Capabilities,pdwErrorCode) ) 

#define IADF2_SetADFOptions(This,hSession,dwADFOptionsSelected,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetADFOptions(This,hSession,dwADFOptionsSelected,pdwErrorCode) ) 

#define IADF2_ADFEjectMedia(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADFEjectMedia(This,hSession,pdwErrorCode) ) 

#define IADF2_ADF2PaperFeed(This,hSession,pdwLength,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADF2PaperFeed(This,hSession,pdwLength,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IADF2_INTERFACE_DEFINED__ */


#ifndef __IADFSetFeature_INTERFACE_DEFINED__
#define __IADFSetFeature_INTERFACE_DEFINED__

/* interface IADFSetFeature */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IADFSetFeature;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F2F5586B-2D9A-4bd1-9268-4203769C8FE4")
    IADFSetFeature : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetADFOptionValue( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwADFOption,
            /* [in] */ DWORD dwValue,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IADFSetFeatureVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IADFSetFeature * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IADFSetFeature * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IADFSetFeature * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetADFOptionValue )( 
            IADFSetFeature * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwADFOption,
            /* [in] */ DWORD dwValue,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IADFSetFeatureVtbl;

    interface IADFSetFeature
    {
        CONST_VTBL struct IADFSetFeatureVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IADFSetFeature_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IADFSetFeature_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IADFSetFeature_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IADFSetFeature_SetADFOptionValue(This,hSession,dwADFOption,dwValue,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetADFOptionValue(This,hSession,dwADFOption,dwValue,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IADFSetFeature_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0006 */
/* [local] */ 


enum HW_DUPLEX_TOGGLE_TYPE
    {
        T_STATE_CHANGEABLE	= 0x1,
        T_SW_CONTROL	= 0x2
    } ;
struct ADF3_CAPABILITIES
    {
    BYTE byHasADF;
    BYTE byModelNumber[ 16 ];
    WORD wADFType;
    BYTE byADFCanDetectEndOfPage;
    BYTE byADFFeedCanDetectLengthOfPage;
    BYTE byHasHWDuplexToggle;
    WORD wHWDuplexToggleType;
    BYTE byADFReverseOrder;
    DWORD dwADFOptionsSupported;
    WORD wOpticalResolution;
    WORD wMaximumXResolution;
    WORD wMaximumYResolution;
    WORD wMinimumResolution;
    DWORD dwMaximumWidth;
    DWORD dwMinimumWidth;
    DWORD dwMaximumHeight;
    DWORD dwMinimumHeight;
    DWORD dwDuplexMaximumWidth;
    DWORD dwDuplexMinimumWidth;
    DWORD dwDuplexMaximumHeight;
    DWORD dwDuplexMinimumHeight;
    DWORD dwScanModesSupported;
    DWORD dwScanOptionsSupported;
    WORD wGreyChannelsSupported;
    BYTE byColorBitsPerChannel[ 8 ];
    BYTE byGreyBitsPerChannel[ 8 ];
    DWORD dwGammaEntriesPerChannel;
    DWORD dwGammaMaxEntryValue;
    WORD dwGammaBytesPerEntry;
    WORD wMaxGreyCCDResolution;
    WORD wMaxFeederCapacity;
    WORD wRotation;
    DWORD dwLongPageMaximumHeight;
    DWORD dwDuplexLongPageMaximumHeight;
    DWORD dwOverscanYDelta;
    BYTE byCanSharpen;
    BYTE byCanSmooth;
    small cSharpSmoothDefault;
    BYTE byNoiseRemovalDefault;
    BYTE byBlankPageSensitivityLevels;
    BYTE byACDSensitivityLevels;
    BYTE bySupportedFillColors;
    BYTE bySupportedAutoCropTypes;
    BYTE byACDsensitivityLevelsBW;
    BYTE byReserved1;
    WORD wMaxELPResolution;
    DWORD dwELPMaximumHeight;
    DWORD dwDuplexELPMaximumHeight;
    DWORD dwAutoOrientSupportedLanguages;
    WORD wReserved[ 20 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_adf3[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct ADF3_CAPABILITIES) == 192);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0006_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0006_v0_0_s_ifspec;

#ifndef __IADF3_INTERFACE_DEFINED__
#define __IADF3_INTERFACE_DEFINED__

/* interface IADF3 */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IADF3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("AB85970A-0581-43B2-AF3A-45E9DF562BF0")
    IADF3 : public IADF2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetADF3Capabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ struct ADF3_CAPABILITIES *pADF3Capabilities,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHWDuplexToggleState( 
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbyState,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetHWDuplexToggleState( 
            /* [in] */ DWORD hSession,
            /* [in] */ BYTE byState,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ADFStartJob( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ADFEndJob( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IADF3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IADF3 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IADF3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IADF3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetADFCapabilities )( 
            IADF3 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct ADF_CAPABILITIES *pADFCapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADFPaperFeed )( 
            IADF3 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADFIsReady )( 
            IADF3 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbIsReady,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetADF2Capabilities )( 
            IADF3 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct ADF2_CAPABILITIES *pADF2Capabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetADFOptions )( 
            IADF3 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwADFOptionsSelected,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADFEjectMedia )( 
            IADF3 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADF2PaperFeed )( 
            IADF3 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwLength,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetADF3Capabilities )( 
            IADF3 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct ADF3_CAPABILITIES *pADF3Capabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetHWDuplexToggleState )( 
            IADF3 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbyState,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetHWDuplexToggleState )( 
            IADF3 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ BYTE byState,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADFStartJob )( 
            IADF3 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADFEndJob )( 
            IADF3 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IADF3Vtbl;

    interface IADF3
    {
        CONST_VTBL struct IADF3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IADF3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IADF3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IADF3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IADF3_GetADFCapabilities(This,hSession,pADFCapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetADFCapabilities(This,hSession,pADFCapabilities,pdwErrorCode) ) 

#define IADF3_ADFPaperFeed(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADFPaperFeed(This,hSession,pdwErrorCode) ) 

#define IADF3_ADFIsReady(This,hSession,pbIsReady,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADFIsReady(This,hSession,pbIsReady,pdwErrorCode) ) 


#define IADF3_GetADF2Capabilities(This,hSession,pADF2Capabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetADF2Capabilities(This,hSession,pADF2Capabilities,pdwErrorCode) ) 

#define IADF3_SetADFOptions(This,hSession,dwADFOptionsSelected,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetADFOptions(This,hSession,dwADFOptionsSelected,pdwErrorCode) ) 

#define IADF3_ADFEjectMedia(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADFEjectMedia(This,hSession,pdwErrorCode) ) 

#define IADF3_ADF2PaperFeed(This,hSession,pdwLength,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADF2PaperFeed(This,hSession,pdwLength,pdwErrorCode) ) 


#define IADF3_GetADF3Capabilities(This,hSession,pADF3Capabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetADF3Capabilities(This,hSession,pADF3Capabilities,pdwErrorCode) ) 

#define IADF3_GetHWDuplexToggleState(This,hSession,pbyState,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetHWDuplexToggleState(This,hSession,pbyState,pdwErrorCode) ) 

#define IADF3_SetHWDuplexToggleState(This,hSession,byState,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetHWDuplexToggleState(This,hSession,byState,pdwErrorCode) ) 

#define IADF3_ADFStartJob(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADFStartJob(This,hSession,pdwErrorCode) ) 

#define IADF3_ADFEndJob(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADFEndJob(This,hSession,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IADF3_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0007 */
/* [local] */ 


enum ADF_DUPLEX_HW_DEFAULT_ORIENT
    {
        T_ADF_DUPLEX_HW_NONE	= 0,
        T_ADF_DUPLEX_HW_HORZ	= 1,
        T_ADF_DUPLEX_HW_HORZ_VERT	= 2,
        T_ADF_DUPLEX_HW_FRONTSIDE	= 3
    } ;

enum ADF_INTRAY_OPTIONS
    {
        T_ADF_INTRAY_SELECT	= 1,
        T_ADF_INTRAY_DETECT	= 2
    } ;

enum ADF_INTRAY_ORIENTATION
    {
        T_ADF_INTRAY_CENTER	= 0,
        T_ADF_INTRAY_LEFT	= 1,
        T_ADF_INTRAY_RIGHT	= 2,
        T_ADF_INTRAY_CUSTOM	= 3
    } ;

enum ADF_EXTENDED_ERRORS
    {
        TERR_JAM_INPUT_TRAY	= 130,
        TERR_JAM_OUTPUT_TRAY	= 131,
        TERR_JAM_CLEANOUT	= 132,
        TERR_JAM_DOOR1	= 133,
        TERR_JAM_DOOR2	= 134,
        TERR_JAM_DOOR3	= 135,
        TERR_OPEN_DOOR1	= 136,
        TERR_OPEN_DOOR2	= 137,
        TERR_OPEN_DOOR3	= 138,
        TERR_OPEN_INPUT_TRAY	= 139,
        TERR_OPEN_ADF_PLATEN	= 140,
        TERR_JAM_PAPER_OVERLAP	= 141
    } ;
struct ADF4_CAPABILITIES
    {
    BYTE byHasADF;
    BYTE byModelNumber[ 16 ];
    WORD wADFType;
    WORD wLampWarmupTime;
    WORD wNumInputTrays;
    DWORD dwInputTrayOptions;
    DWORD dwMaxRotationSimplexBytes;
    DWORD dwMaxRotationDuplexBytes;
    BYTE byBlankPageSensitivityLevels;
    BYTE byACDSensitivityLevels;
    BYTE bySupportedFillColors;
    BYTE bySupportedAutoCropTypes;
    BYTE byACDsensitivityLevelsBW;
    BYTE byReserved1;
    DWORD dwAutoOrientSupportedLanguages;
    WORD wReserved[ 30 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_adf4[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct ADF4_CAPABILITIES) == 108);
struct ADF_INTRAY_CAPABILITIES
    {
    BYTE byADFCanDetectEndOfPage;
    BYTE byADFFeedCanDetectLengthOfPage;
    BYTE byHasHWDuplexToggle;
    WORD wHWDuplexToggleType;
    BYTE byADFReverseOrder;
    DWORD dwADFOptionsSupported;
    WORD wADFDuplexDefaultHWOrientation;
    WORD wHasDuplexSensor;
    WORD wOpticalResolution;
    WORD wMaximumXResolution;
    WORD wMaximumYResolution;
    WORD wMinimumResolution;
    DWORD dwMaximumWidth;
    DWORD dwMinimumWidth;
    DWORD dwMaximumHeight;
    DWORD dwMinimumHeight;
    DWORD dwDuplexMaximumWidth;
    DWORD dwDuplexMinimumWidth;
    DWORD dwDuplexMaximumHeight;
    DWORD dwDuplexMinimumHeight;
    DWORD dwInputMaximumWidth;
    DWORD dwInputMinimumWidth;
    DWORD dwInputMaximumHeight;
    DWORD dwInputMinimumHeight;
    DWORD dwScanModesSupported;
    DWORD dwScanOptionsSupported;
    WORD wGreyChannelsSupported;
    BYTE byColorBitsPerChannel[ 8 ];
    BYTE byGreyBitsPerChannel[ 8 ];
    DWORD dwGammaEntriesPerChannel;
    DWORD dwGammaMaxEntryValue;
    WORD dwGammaBytesPerEntry;
    WORD wMaxGreyCCDResolution;
    WORD wMaxFeederCapacity;
    WORD wRotation;
    WORD wIntrayOrientation;
    WORD wCustomOrientationOffset;
    DWORD dwLongPageMaximumHeight;
    DWORD dwDuplexLongPageMaximumHeight;
    DWORD dwOverscanYDelta;
    BYTE byCanSharpen;
    BYTE byCanSmooth;
    small cSharpSmoothDefault;
    BYTE byNoiseRemovalDefault;
    WORD wReserved1;
    WORD wMaxELPResolution;
    DWORD dwELPMaximumHeight;
    DWORD dwDuplexELPMaximumHeight;
    WORD wReserved[ 6 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_adf_intray[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct ADF_INTRAY_CAPABILITIES) == 160);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0007_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0007_v0_0_s_ifspec;

#ifndef __IADF4_INTERFACE_DEFINED__
#define __IADF4_INTERFACE_DEFINED__

/* interface IADF4 */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IADF4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C81F89FA-6C88-49A8-B701-E6B3B2BF864F")
    IADF4 : public IADF3
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetADF4Capabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ struct ADF4_CAPABILITIES *pADF4Capabilities,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetADFInputTrayCapabilities( 
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wInputTray,
            /* [out] */ struct ADF_INTRAY_CAPABILITIES *pIntrayCaps,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetADFInputTray( 
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wInputTray,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsInputTrayLoaded( 
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wInputTray,
            /* [out] */ BYTE *pbyLoaded,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentSide( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwSide,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetADFLampStatus( 
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbyLampOn,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ADFLampOn( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ADFLampOff( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetADFExtendedErrorInfo( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IADF4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IADF4 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IADF4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IADF4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetADFCapabilities )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct ADF_CAPABILITIES *pADFCapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADFPaperFeed )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADFIsReady )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbIsReady,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetADF2Capabilities )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct ADF2_CAPABILITIES *pADF2Capabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetADFOptions )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwADFOptionsSelected,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADFEjectMedia )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADF2PaperFeed )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwLength,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetADF3Capabilities )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct ADF3_CAPABILITIES *pADF3Capabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetHWDuplexToggleState )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbyState,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetHWDuplexToggleState )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ BYTE byState,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADFStartJob )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADFEndJob )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetADF4Capabilities )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct ADF4_CAPABILITIES *pADF4Capabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetADFInputTrayCapabilities )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wInputTray,
            /* [out] */ struct ADF_INTRAY_CAPABILITIES *pIntrayCaps,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetADFInputTray )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wInputTray,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *IsInputTrayLoaded )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wInputTray,
            /* [out] */ BYTE *pbyLoaded,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentSide )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwSide,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetADFLampStatus )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbyLampOn,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADFLampOn )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ADFLampOff )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetADFExtendedErrorInfo )( 
            IADF4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IADF4Vtbl;

    interface IADF4
    {
        CONST_VTBL struct IADF4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IADF4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IADF4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IADF4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IADF4_GetADFCapabilities(This,hSession,pADFCapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetADFCapabilities(This,hSession,pADFCapabilities,pdwErrorCode) ) 

#define IADF4_ADFPaperFeed(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADFPaperFeed(This,hSession,pdwErrorCode) ) 

#define IADF4_ADFIsReady(This,hSession,pbIsReady,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADFIsReady(This,hSession,pbIsReady,pdwErrorCode) ) 


#define IADF4_GetADF2Capabilities(This,hSession,pADF2Capabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetADF2Capabilities(This,hSession,pADF2Capabilities,pdwErrorCode) ) 

#define IADF4_SetADFOptions(This,hSession,dwADFOptionsSelected,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetADFOptions(This,hSession,dwADFOptionsSelected,pdwErrorCode) ) 

#define IADF4_ADFEjectMedia(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADFEjectMedia(This,hSession,pdwErrorCode) ) 

#define IADF4_ADF2PaperFeed(This,hSession,pdwLength,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADF2PaperFeed(This,hSession,pdwLength,pdwErrorCode) ) 


#define IADF4_GetADF3Capabilities(This,hSession,pADF3Capabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetADF3Capabilities(This,hSession,pADF3Capabilities,pdwErrorCode) ) 

#define IADF4_GetHWDuplexToggleState(This,hSession,pbyState,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetHWDuplexToggleState(This,hSession,pbyState,pdwErrorCode) ) 

#define IADF4_SetHWDuplexToggleState(This,hSession,byState,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetHWDuplexToggleState(This,hSession,byState,pdwErrorCode) ) 

#define IADF4_ADFStartJob(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADFStartJob(This,hSession,pdwErrorCode) ) 

#define IADF4_ADFEndJob(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADFEndJob(This,hSession,pdwErrorCode) ) 


#define IADF4_GetADF4Capabilities(This,hSession,pADF4Capabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetADF4Capabilities(This,hSession,pADF4Capabilities,pdwErrorCode) ) 

#define IADF4_GetADFInputTrayCapabilities(This,hSession,wInputTray,pIntrayCaps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetADFInputTrayCapabilities(This,hSession,wInputTray,pIntrayCaps,pdwErrorCode) ) 

#define IADF4_SetADFInputTray(This,hSession,wInputTray,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetADFInputTray(This,hSession,wInputTray,pdwErrorCode) ) 

#define IADF4_IsInputTrayLoaded(This,hSession,wInputTray,pbyLoaded,pdwErrorCode)	\
    ( (This)->lpVtbl -> IsInputTrayLoaded(This,hSession,wInputTray,pbyLoaded,pdwErrorCode) ) 

#define IADF4_GetCurrentSide(This,hSession,pdwSide,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetCurrentSide(This,hSession,pdwSide,pdwErrorCode) ) 

#define IADF4_GetADFLampStatus(This,hSession,pbyLampOn,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetADFLampStatus(This,hSession,pbyLampOn,pdwErrorCode) ) 

#define IADF4_ADFLampOn(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADFLampOn(This,hSession,pdwErrorCode) ) 

#define IADF4_ADFLampOff(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> ADFLampOff(This,hSession,pdwErrorCode) ) 

#define IADF4_GetADFExtendedErrorInfo(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetADFExtendedErrorInfo(This,hSession,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IADF4_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0008 */
/* [local] */ 






enum XPA_ERRORS
    {
        TERR_XPA_LAMP_FAILED	= 300,
        TERR_XPA_ALIGN_FAILED	= 301
    } ;
struct XPA_CAPABILITIES
    {
    BYTE byHasXPA;
    BYTE byModelNumber[ 16 ];
    WORD wLampWarmupTime;
    WORD wOpticalResolution;
    WORD wMaximumXResolution;
    WORD wMaximumYResolution;
    WORD wMinimumResolution;
    DWORD dwMaximumWidth;
    DWORD dwMinimumWidth;
    DWORD dwMaximumHeight;
    DWORD dwMinimumHeight;
    DWORD dwScanModesSupported;
    DWORD dwScanOptionsSupported;
    WORD wGreyChannelsSupported;
    WORD wGammaTableSize;
    BYTE byColorBitsPerChannel[ 8 ];
    BYTE byGreyBitsPerChannel[ 8 ];
    DWORD dwGammaEntriesPerChannel;
    DWORD dwGammaMaxEntryValue;
    WORD dwGammaBytesPerEntry;
    WORD wRotation;
    WORD wMaxGreyCCDResolution;
    BYTE byCanSharpen;
    BYTE byCanSmooth;
    small cSharpSmoothDefault;
    BYTE byNoiseRemovalDefault;
    WORD wReserved[ 13 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_xpa[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct XPA_CAPABILITIES) == 116);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0008_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0008_v0_0_s_ifspec;

#ifndef __IXPA_INTERFACE_DEFINED__
#define __IXPA_INTERFACE_DEFINED__

/* interface IXPA */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IXPA;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2BF83C23-96BE-4e6b-9CBD-F3F6A216377E")
    IXPA : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetXPACapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ struct XPA_CAPABILITIES *pXPACapabilities,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE XPAIsReady( 
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbIsReady,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE XPALampOn( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE XPALampOff( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IXPAVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IXPA * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IXPA * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IXPA * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetXPACapabilities )( 
            IXPA * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct XPA_CAPABILITIES *pXPACapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *XPAIsReady )( 
            IXPA * This,
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbIsReady,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *XPALampOn )( 
            IXPA * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *XPALampOff )( 
            IXPA * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IXPAVtbl;

    interface IXPA
    {
        CONST_VTBL struct IXPAVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IXPA_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IXPA_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IXPA_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IXPA_GetXPACapabilities(This,hSession,pXPACapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetXPACapabilities(This,hSession,pXPACapabilities,pdwErrorCode) ) 

#define IXPA_XPAIsReady(This,hSession,pbIsReady,pdwErrorCode)	\
    ( (This)->lpVtbl -> XPAIsReady(This,hSession,pbIsReady,pdwErrorCode) ) 

#define IXPA_XPALampOn(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> XPALampOn(This,hSession,pdwErrorCode) ) 

#define IXPA_XPALampOff(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> XPALampOff(This,hSession,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IXPA_INTERFACE_DEFINED__ */


#ifndef __IXPA2_INTERFACE_DEFINED__
#define __IXPA2_INTERFACE_DEFINED__

/* interface IXPA2 */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IXPA2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0AEF8EC0-29C2-4CF3-A89F-4589C76D8DED")
    IXPA2 : public IXPA
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetXPALampStatus( 
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbLampOn,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IXPA2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IXPA2 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IXPA2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IXPA2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetXPACapabilities )( 
            IXPA2 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct XPA_CAPABILITIES *pXPACapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *XPAIsReady )( 
            IXPA2 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbIsReady,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *XPALampOn )( 
            IXPA2 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *XPALampOff )( 
            IXPA2 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetXPALampStatus )( 
            IXPA2 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbLampOn,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IXPA2Vtbl;

    interface IXPA2
    {
        CONST_VTBL struct IXPA2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IXPA2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IXPA2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IXPA2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IXPA2_GetXPACapabilities(This,hSession,pXPACapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetXPACapabilities(This,hSession,pXPACapabilities,pdwErrorCode) ) 

#define IXPA2_XPAIsReady(This,hSession,pbIsReady,pdwErrorCode)	\
    ( (This)->lpVtbl -> XPAIsReady(This,hSession,pbIsReady,pdwErrorCode) ) 

#define IXPA2_XPALampOn(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> XPALampOn(This,hSession,pdwErrorCode) ) 

#define IXPA2_XPALampOff(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> XPALampOff(This,hSession,pdwErrorCode) ) 


#define IXPA2_GetXPALampStatus(This,hSession,pbLampOn,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetXPALampStatus(This,hSession,pbLampOn,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IXPA2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0010 */
/* [local] */ 


enum TMA_TEMPLATE_TYPE
    {
        TEMPLATE_NONE	= 0,
        TEMPLATE_MOUNTED	= 1,
        TEMPLATE_STRIP	= 2,
        TEMPLATE_LARGE_FORMAT	= 3,
        TEMPLATE_LID_MOUNTED	= 4,
        TEMPLATE_LID_STRIP	= 5
    } ;

enum TMA_DETECT_ERRORS
    {
        TERR_BACKING_NOT_REMOVED	= 320,
        TERR_TEMPLATE	= 321,
        TERR_UNPLUGGED	= 322,
        TERR_BUSY	= 323
    } ;
struct XPA3_CAPABILITIES
    {
    BYTE byHasXPA;
    BYTE byModelNumber[ 16 ];
    WORD wLampWarmupTime;
    BYTE byCanDetectTemplate;
    WORD wNumTemplates;
    DWORD dwMaxRotationBytes;
    WORD wReserved[ 18 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_xpa3[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct XPA3_CAPABILITIES) == 64);
struct TEMPLATE_CAPABILITIES
    {
    BYTE byDefaultMediaType;
    DWORD dwMaximumWidth;
    DWORD dwMinimumWidth;
    DWORD dwMaximumHeight;
    DWORD dwMinimumHeight;
    WORD wRotation;
    BYTE byMirror;
    WORD wTemplateType;
    WORD wReserved[ 19 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_template[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct TEMPLATE_CAPABILITIES) == 64);
struct MEDIA_CAPABILITIES
    {
    WORD wOpticalResolution;
    WORD wMaximumXResolution;
    WORD wMaximumYResolution;
    WORD wMinimumResolution;
    WORD wMaxGreyCCDResolution;
    DWORD dwScanModesSupported;
    DWORD dwScanOptionsSupported;
    WORD wGreyChannelsSupported;
    BYTE byColorBitsPerChannel[ 8 ];
    BYTE byGreyBitsPerChannel[ 8 ];
    DWORD dwGammaEntriesPerChannel;
    DWORD dwGammaMaxEntryValue;
    WORD dwGammaBytesPerEntry;
    BYTE byCanSharpen;
    BYTE byCanSmooth;
    small cSharpSmoothDefault;
    BYTE byNoiseRemovalDefault;
    WORD wReserved[ 18 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_media[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct MEDIA_CAPABILITIES) == 92);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0010_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0010_v0_0_s_ifspec;

#ifndef __IXPA3_INTERFACE_DEFINED__
#define __IXPA3_INTERFACE_DEFINED__

/* interface IXPA3 */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IXPA3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A3B83F41-503D-4FBF-AF11-6F866D18A3A7")
    IXPA3 : public IXPA2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetXPA3Capabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ struct XPA3_CAPABILITIES *pXPA3Caps,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetTemplateCapabilities( 
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wTemplate,
            /* [out] */ struct TEMPLATE_CAPABILITIES *pTemplateCaps,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMediaCapabilities( 
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wMedia,
            /* [out] */ struct MEDIA_CAPABILITIES *pMediaCaps,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE TemplateDetect( 
            /* [in] */ DWORD hSession,
            /* [out] */ WORD *pwTemplate,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IXPA3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IXPA3 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IXPA3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IXPA3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetXPACapabilities )( 
            IXPA3 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct XPA_CAPABILITIES *pXPACapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *XPAIsReady )( 
            IXPA3 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbIsReady,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *XPALampOn )( 
            IXPA3 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *XPALampOff )( 
            IXPA3 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetXPALampStatus )( 
            IXPA3 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbLampOn,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetXPA3Capabilities )( 
            IXPA3 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct XPA3_CAPABILITIES *pXPA3Caps,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetTemplateCapabilities )( 
            IXPA3 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wTemplate,
            /* [out] */ struct TEMPLATE_CAPABILITIES *pTemplateCaps,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetMediaCapabilities )( 
            IXPA3 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wMedia,
            /* [out] */ struct MEDIA_CAPABILITIES *pMediaCaps,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *TemplateDetect )( 
            IXPA3 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ WORD *pwTemplate,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IXPA3Vtbl;

    interface IXPA3
    {
        CONST_VTBL struct IXPA3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IXPA3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IXPA3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IXPA3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IXPA3_GetXPACapabilities(This,hSession,pXPACapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetXPACapabilities(This,hSession,pXPACapabilities,pdwErrorCode) ) 

#define IXPA3_XPAIsReady(This,hSession,pbIsReady,pdwErrorCode)	\
    ( (This)->lpVtbl -> XPAIsReady(This,hSession,pbIsReady,pdwErrorCode) ) 

#define IXPA3_XPALampOn(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> XPALampOn(This,hSession,pdwErrorCode) ) 

#define IXPA3_XPALampOff(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> XPALampOff(This,hSession,pdwErrorCode) ) 


#define IXPA3_GetXPALampStatus(This,hSession,pbLampOn,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetXPALampStatus(This,hSession,pbLampOn,pdwErrorCode) ) 


#define IXPA3_GetXPA3Capabilities(This,hSession,pXPA3Caps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetXPA3Capabilities(This,hSession,pXPA3Caps,pdwErrorCode) ) 

#define IXPA3_GetTemplateCapabilities(This,hSession,wTemplate,pTemplateCaps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetTemplateCapabilities(This,hSession,wTemplate,pTemplateCaps,pdwErrorCode) ) 

#define IXPA3_GetMediaCapabilities(This,hSession,wMedia,pMediaCaps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetMediaCapabilities(This,hSession,wMedia,pMediaCaps,pdwErrorCode) ) 

#define IXPA3_TemplateDetect(This,hSession,pwTemplate,pdwErrorCode)	\
    ( (This)->lpVtbl -> TemplateDetect(This,hSession,pwTemplate,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IXPA3_INTERFACE_DEFINED__ */


#ifndef __IXPA4_INTERFACE_DEFINED__
#define __IXPA4_INTERFACE_DEFINED__

/* interface IXPA4 */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IXPA4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1ECA8D6D-48E0-4418-8BBE-8A3144274ADC")
    IXPA4 : public IXPA3
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetTemplate( 
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wTemplate,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IXPA4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IXPA4 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IXPA4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IXPA4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetXPACapabilities )( 
            IXPA4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct XPA_CAPABILITIES *pXPACapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *XPAIsReady )( 
            IXPA4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbIsReady,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *XPALampOn )( 
            IXPA4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *XPALampOff )( 
            IXPA4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetXPALampStatus )( 
            IXPA4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbLampOn,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetXPA3Capabilities )( 
            IXPA4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct XPA3_CAPABILITIES *pXPA3Caps,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetTemplateCapabilities )( 
            IXPA4 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wTemplate,
            /* [out] */ struct TEMPLATE_CAPABILITIES *pTemplateCaps,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetMediaCapabilities )( 
            IXPA4 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wMedia,
            /* [out] */ struct MEDIA_CAPABILITIES *pMediaCaps,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *TemplateDetect )( 
            IXPA4 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ WORD *pwTemplate,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetTemplate )( 
            IXPA4 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wTemplate,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IXPA4Vtbl;

    interface IXPA4
    {
        CONST_VTBL struct IXPA4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IXPA4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IXPA4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IXPA4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IXPA4_GetXPACapabilities(This,hSession,pXPACapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetXPACapabilities(This,hSession,pXPACapabilities,pdwErrorCode) ) 

#define IXPA4_XPAIsReady(This,hSession,pbIsReady,pdwErrorCode)	\
    ( (This)->lpVtbl -> XPAIsReady(This,hSession,pbIsReady,pdwErrorCode) ) 

#define IXPA4_XPALampOn(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> XPALampOn(This,hSession,pdwErrorCode) ) 

#define IXPA4_XPALampOff(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> XPALampOff(This,hSession,pdwErrorCode) ) 


#define IXPA4_GetXPALampStatus(This,hSession,pbLampOn,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetXPALampStatus(This,hSession,pbLampOn,pdwErrorCode) ) 


#define IXPA4_GetXPA3Capabilities(This,hSession,pXPA3Caps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetXPA3Capabilities(This,hSession,pXPA3Caps,pdwErrorCode) ) 

#define IXPA4_GetTemplateCapabilities(This,hSession,wTemplate,pTemplateCaps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetTemplateCapabilities(This,hSession,wTemplate,pTemplateCaps,pdwErrorCode) ) 

#define IXPA4_GetMediaCapabilities(This,hSession,wMedia,pMediaCaps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetMediaCapabilities(This,hSession,wMedia,pMediaCaps,pdwErrorCode) ) 

#define IXPA4_TemplateDetect(This,hSession,pwTemplate,pdwErrorCode)	\
    ( (This)->lpVtbl -> TemplateDetect(This,hSession,pwTemplate,pdwErrorCode) ) 


#define IXPA4_SetTemplate(This,hSession,wTemplate,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetTemplate(This,hSession,wTemplate,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IXPA4_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0012 */
/* [local] */ 


struct NVRAM_CAPABILITIES
    {
    WORD wNVRAMSize;
    DWORD dwReserved[ 10 ];
    } ;


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0012_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0012_v0_0_s_ifspec;

#ifndef __INVRAM_INTERFACE_DEFINED__
#define __INVRAM_INTERFACE_DEFINED__

/* interface INVRAM */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_INVRAM;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("BF8EC696-4876-4aec-9275-531DD8ABDB08")
    INVRAM : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetNVRAMCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ struct NVRAM_CAPABILITIES *pNVRAMCapabilities,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE WriteNVRAM( 
            /* [in] */ DWORD hSession,
            /* [in] */ int nStartByte,
            /* [in] */ DWORD cbRequested,
            /* [size_is][in] */ BYTE *pbData,
            /* [out] */ DWORD *pcbActual,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ReadNVRAM( 
            /* [in] */ DWORD hSession,
            /* [in] */ int nStartByte,
            /* [in] */ DWORD cbRequested,
            /* [size_is][out] */ BYTE *pbData,
            /* [out] */ DWORD *pcbActual,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct INVRAMVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            INVRAM * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            INVRAM * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            INVRAM * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetNVRAMCapabilities )( 
            INVRAM * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct NVRAM_CAPABILITIES *pNVRAMCapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *WriteNVRAM )( 
            INVRAM * This,
            /* [in] */ DWORD hSession,
            /* [in] */ int nStartByte,
            /* [in] */ DWORD cbRequested,
            /* [size_is][in] */ BYTE *pbData,
            /* [out] */ DWORD *pcbActual,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ReadNVRAM )( 
            INVRAM * This,
            /* [in] */ DWORD hSession,
            /* [in] */ int nStartByte,
            /* [in] */ DWORD cbRequested,
            /* [size_is][out] */ BYTE *pbData,
            /* [out] */ DWORD *pcbActual,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } INVRAMVtbl;

    interface INVRAM
    {
        CONST_VTBL struct INVRAMVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define INVRAM_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define INVRAM_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define INVRAM_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define INVRAM_GetNVRAMCapabilities(This,hSession,pNVRAMCapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetNVRAMCapabilities(This,hSession,pNVRAMCapabilities,pdwErrorCode) ) 

#define INVRAM_WriteNVRAM(This,hSession,nStartByte,cbRequested,pbData,pcbActual,pdwErrorCode)	\
    ( (This)->lpVtbl -> WriteNVRAM(This,hSession,nStartByte,cbRequested,pbData,pcbActual,pdwErrorCode) ) 

#define INVRAM_ReadNVRAM(This,hSession,nStartByte,cbRequested,pbData,pcbActual,pdwErrorCode)	\
    ( (This)->lpVtbl -> ReadNVRAM(This,hSession,nStartByte,cbRequested,pbData,pcbActual,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __INVRAM_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0013 */
/* [local] */ 



enum NVRAM_VALUES
    {
        T_NVRAM_VALUE_SCAN_COUNT	= 0x1,
        T_NVRAM_VALUE_MAN_TEST_DATE	= 0x2,
        T_NVRAM_VALUE_MAN_TEST_SITE	= 0x4,
        T_NVRAM_VALUE_BORN_ON_DATE	= 0x8,
        T_NVRAM_VALUE_LAST_REPAIR_DATE	= 0x10,
        T_NVRAM_VALUE_NO_REPAIRS	= 0x20,
        T_NVRAM_VALUE_ERR1_SCAN_COUNT	= 0x40,
        T_NVRAM_VALUE_ERR2_SCAN_COUNT	= 0x80,
        T_NVRAM_VALUE_ERR3_SCAN_COUNT	= 0x100,
        T_NVRAM_VALUE_CARD_COUNT	= 0x200,
        T_NVRAM_VALUE_ERROR_1	= 0x400,
        T_NVRAM_VALUE_ERROR_2	= 0x800,
        T_NVRAM_VALUE_ERROR_3	= 0x1000,
        T_NVRAM_VALUE_JAM_COUNT	= 0x2000,
        T_NVRAM_VALUE_PICKFAILURE_COUNT	= 0x4000,
        T_NVRAM_VALUE_ADF_CLEAN_COUNT	= 0x8000,
        T_NVRAM_VALUE_ADF_ROLLER_REPLACE_COUNT	= 0x10000,
        T_NVRAM_VALUE_SCANTO_BUTTON_COUNT	= 0x20000,
        T_NVRAM_VALUE_SCANTOFILE_BUTTON_COUNT	= 0x40000,
        T_NVRAM_VALUE_SCANTOWEB_BUTTON_COUNT	= 0x80000,
        T_NVRAM_VALUE_COPY_BUTTON_COUNT	= 0x100000,
        T_NVRAM_VALUE_EMAIL_BUTTON_COUNT	= 0x200000,
        T_NVRAM_VALUE_OCR_BUTTON_COUNT	= 0x400000,
        T_NVRAM_VALUE_FAX_BUTTON_COUNT	= 0x800000,
        T_NVRAM_VALUE_ADF_PAGE_COUNT	= 0x1000000,
        T_NVRAM_VALUE_ADF_COUNT_AT_CLEAN	= 0x2000000,
        T_NVRAM_VALUE_ADF_COUNT_AT_ROLLER	= 0x4000000,
        T_NVRAM_VALUE_CLEAN_PAPER_PATH	= 0x8000000,
        T_NVRAM_VALUE_REPLACE_ROLLER	= 0x10000000,
        T_NVRAM_VALUE_ADF_ERROR_1	= 0x20000000,
        T_NVRAM_VALUE_ADF_ERROR_2	= 0x40000000,
        T_NVRAM_VALUE_ADF_ERROR_3	= 0x80000000
    } ;

enum NVRAM_VALUES2
    {
        T_NVRAM_VALUE_ADF_REPLACE_PAD	= 0x1,
        T_NVRAM_VALUE_ADF_COUNT_AT_PAD	= 0x2,
        T_NVRAM_VALUE_ADF_PAD_REPLACE_COUNT	= 0x4,
        T_NVRAM_VALUE_IMPRINTER_INK_LOW	= 0x8,
        T_NVRAM_VALUE_IMPRINTER_INK_REPLACE_COUNT	= 0x10,
        T_NVRAM_VALUE_IMPRINTER_INK_REPLACE_DATE	= 0x20,
        T_NVRAM_VALUE_MAINTENANCE_INTERVAL_ROLLER	= 0x40,
        T_NVRAM_VALUE_MAINTENANCE_INTERVAL_PAD	= 0x80,
        T_NVRAM_VALUE_MAINTENANCE_INTERVAL_CLEAN	= 0x100,
        T_NVRAM_VALUE_MAINTENANCE_INTERVAL_CLEANROLLER	= 0x200,
        T_NVRAM_VALUE_ADF_CLEAN_ROLLER	= 0x400,
        T_NVRAM_VALUE_ADF_CLEAN_ROLLER_COUNT	= 0x800,
        T_NVRAM_VALUE_ADF_SCANCOUNT_AT_CLEAN_ROLLER	= 0x1000,
        T_NVRAM_VALUE_ADF_ERR1_SCAN_COUNT	= 0x2000,
        T_NVRAM_VALUE_ADF_ERR2_SCAN_COUNT	= 0x4000,
        T_NVRAM_VALUE_ADF_ERR3_SCAN_COUNT	= 0x8000,
        T_NVRAM_VALUE_MULTIPICK_COUNT	= 0x10000
    } ;

enum NVRAM_STRINGS
    {
        T_NVRAM_STRING_SN	= 0x1,
        T_NVRAM_STRING_ADF_SN	= 0x2,
        T_NVRAM_STRING_WJA_ASSET	= 0x4,
        T_NVRAM_STRING_WJA_SYSLOCATION	= 0x8,
        T_NVRAM_STRING_WJA_UUID	= 0x10
    } ;
struct NVRAMWrapper_CAPABILITIES
    {
    WORD wNVRAMSize;
    DWORD dwNVRAMSupportedValues;
    DWORD dwNVRAMSupportedStrings;
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_nvram[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct NVRAMWrapper_CAPABILITIES) == 12);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0013_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0013_v0_0_s_ifspec;

#ifndef __INVRAMWrapper_INTERFACE_DEFINED__
#define __INVRAMWrapper_INTERFACE_DEFINED__

/* interface INVRAMWrapper */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_INVRAMWrapper;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("4D623AE3-4781-4ec0-B85B-0FD8223EA87E")
    INVRAMWrapper : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetNVRAMWrapperCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ struct NVRAMWrapper_CAPABILITIES *pCapabilities,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNVRAMValue( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [out] */ DWORD *pdwValue,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetNVRAMValue( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [in] */ DWORD dwValue,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNVRAMString( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [out] */ LPOLESTR *ppszString,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetNVRAMString( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [in] */ LPOLESTR pszString,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RAWWriteNVRAM( 
            /* [in] */ DWORD hSession,
            /* [in] */ int nStartByte,
            /* [in] */ DWORD cbRequested,
            /* [size_is][in] */ BYTE *pbData,
            /* [out] */ DWORD *pcbActual,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RAWReadNVRAM( 
            /* [in] */ DWORD hSession,
            /* [in] */ int nStartByte,
            /* [in] */ DWORD cbRequested,
            /* [size_is][out] */ BYTE *pbData,
            /* [out] */ DWORD *pcbActual,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct INVRAMWrapperVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            INVRAMWrapper * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            INVRAMWrapper * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            INVRAMWrapper * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetNVRAMWrapperCapabilities )( 
            INVRAMWrapper * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct NVRAMWrapper_CAPABILITIES *pCapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetNVRAMValue )( 
            INVRAMWrapper * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [out] */ DWORD *pdwValue,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetNVRAMValue )( 
            INVRAMWrapper * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [in] */ DWORD dwValue,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetNVRAMString )( 
            INVRAMWrapper * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [out] */ LPOLESTR *ppszString,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetNVRAMString )( 
            INVRAMWrapper * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [in] */ LPOLESTR pszString,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *RAWWriteNVRAM )( 
            INVRAMWrapper * This,
            /* [in] */ DWORD hSession,
            /* [in] */ int nStartByte,
            /* [in] */ DWORD cbRequested,
            /* [size_is][in] */ BYTE *pbData,
            /* [out] */ DWORD *pcbActual,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *RAWReadNVRAM )( 
            INVRAMWrapper * This,
            /* [in] */ DWORD hSession,
            /* [in] */ int nStartByte,
            /* [in] */ DWORD cbRequested,
            /* [size_is][out] */ BYTE *pbData,
            /* [out] */ DWORD *pcbActual,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } INVRAMWrapperVtbl;

    interface INVRAMWrapper
    {
        CONST_VTBL struct INVRAMWrapperVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define INVRAMWrapper_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define INVRAMWrapper_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define INVRAMWrapper_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define INVRAMWrapper_GetNVRAMWrapperCapabilities(This,hSession,pCapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetNVRAMWrapperCapabilities(This,hSession,pCapabilities,pdwErrorCode) ) 

#define INVRAMWrapper_GetNVRAMValue(This,hSession,dwVariable,pdwValue,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetNVRAMValue(This,hSession,dwVariable,pdwValue,pdwErrorCode) ) 

#define INVRAMWrapper_SetNVRAMValue(This,hSession,dwVariable,dwValue,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetNVRAMValue(This,hSession,dwVariable,dwValue,pdwErrorCode) ) 

#define INVRAMWrapper_GetNVRAMString(This,hSession,dwVariable,ppszString,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetNVRAMString(This,hSession,dwVariable,ppszString,pdwErrorCode) ) 

#define INVRAMWrapper_SetNVRAMString(This,hSession,dwVariable,pszString,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetNVRAMString(This,hSession,dwVariable,pszString,pdwErrorCode) ) 

#define INVRAMWrapper_RAWWriteNVRAM(This,hSession,nStartByte,cbRequested,pbData,pcbActual,pdwErrorCode)	\
    ( (This)->lpVtbl -> RAWWriteNVRAM(This,hSession,nStartByte,cbRequested,pbData,pcbActual,pdwErrorCode) ) 

#define INVRAMWrapper_RAWReadNVRAM(This,hSession,nStartByte,cbRequested,pbData,pcbActual,pdwErrorCode)	\
    ( (This)->lpVtbl -> RAWReadNVRAM(This,hSession,nStartByte,cbRequested,pbData,pcbActual,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __INVRAMWrapper_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0014 */
/* [local] */ 

struct NVRAMWrapper2_CAPABILITIES
    {
    DWORD dwNVRAMSize;
    DWORD dwNVRAMSupportedValues[ 40 ];
    DWORD dwNVRAMSupportedStrings;
    DWORD dwReserved[ 20 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_nvram2[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct NVRAMWrapper2_CAPABILITIES) == 248);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0014_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0014_v0_0_s_ifspec;

#ifndef __INVRAMWrapper2_INTERFACE_DEFINED__
#define __INVRAMWrapper2_INTERFACE_DEFINED__

/* interface INVRAMWrapper2 */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_INVRAMWrapper2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("00A3981B-B71D-4304-9A10-15F8FC3954B8")
    INVRAMWrapper2 : public INVRAMWrapper
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetNVRAMWrapper2Capabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ struct NVRAMWrapper2_CAPABILITIES *pCapabilities,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNVRAMValue2( 
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wTableIndex,
            /* [in] */ DWORD dwVariable,
            /* [out] */ DWORD *pdwValue,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetNVRAMValue2( 
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wTableIndex,
            /* [in] */ DWORD dwVariable,
            /* [in] */ DWORD dwValue,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNVRAMStringSize( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [out] */ DWORD *pdwSize,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct INVRAMWrapper2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            INVRAMWrapper2 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            INVRAMWrapper2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            INVRAMWrapper2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetNVRAMWrapperCapabilities )( 
            INVRAMWrapper2 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct NVRAMWrapper_CAPABILITIES *pCapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetNVRAMValue )( 
            INVRAMWrapper2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [out] */ DWORD *pdwValue,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetNVRAMValue )( 
            INVRAMWrapper2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [in] */ DWORD dwValue,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetNVRAMString )( 
            INVRAMWrapper2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [out] */ LPOLESTR *ppszString,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetNVRAMString )( 
            INVRAMWrapper2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [in] */ LPOLESTR pszString,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *RAWWriteNVRAM )( 
            INVRAMWrapper2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ int nStartByte,
            /* [in] */ DWORD cbRequested,
            /* [size_is][in] */ BYTE *pbData,
            /* [out] */ DWORD *pcbActual,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *RAWReadNVRAM )( 
            INVRAMWrapper2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ int nStartByte,
            /* [in] */ DWORD cbRequested,
            /* [size_is][out] */ BYTE *pbData,
            /* [out] */ DWORD *pcbActual,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetNVRAMWrapper2Capabilities )( 
            INVRAMWrapper2 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct NVRAMWrapper2_CAPABILITIES *pCapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetNVRAMValue2 )( 
            INVRAMWrapper2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wTableIndex,
            /* [in] */ DWORD dwVariable,
            /* [out] */ DWORD *pdwValue,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetNVRAMValue2 )( 
            INVRAMWrapper2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wTableIndex,
            /* [in] */ DWORD dwVariable,
            /* [in] */ DWORD dwValue,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetNVRAMStringSize )( 
            INVRAMWrapper2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [out] */ DWORD *pdwSize,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } INVRAMWrapper2Vtbl;

    interface INVRAMWrapper2
    {
        CONST_VTBL struct INVRAMWrapper2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define INVRAMWrapper2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define INVRAMWrapper2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define INVRAMWrapper2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define INVRAMWrapper2_GetNVRAMWrapperCapabilities(This,hSession,pCapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetNVRAMWrapperCapabilities(This,hSession,pCapabilities,pdwErrorCode) ) 

#define INVRAMWrapper2_GetNVRAMValue(This,hSession,dwVariable,pdwValue,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetNVRAMValue(This,hSession,dwVariable,pdwValue,pdwErrorCode) ) 

#define INVRAMWrapper2_SetNVRAMValue(This,hSession,dwVariable,dwValue,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetNVRAMValue(This,hSession,dwVariable,dwValue,pdwErrorCode) ) 

#define INVRAMWrapper2_GetNVRAMString(This,hSession,dwVariable,ppszString,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetNVRAMString(This,hSession,dwVariable,ppszString,pdwErrorCode) ) 

#define INVRAMWrapper2_SetNVRAMString(This,hSession,dwVariable,pszString,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetNVRAMString(This,hSession,dwVariable,pszString,pdwErrorCode) ) 

#define INVRAMWrapper2_RAWWriteNVRAM(This,hSession,nStartByte,cbRequested,pbData,pcbActual,pdwErrorCode)	\
    ( (This)->lpVtbl -> RAWWriteNVRAM(This,hSession,nStartByte,cbRequested,pbData,pcbActual,pdwErrorCode) ) 

#define INVRAMWrapper2_RAWReadNVRAM(This,hSession,nStartByte,cbRequested,pbData,pcbActual,pdwErrorCode)	\
    ( (This)->lpVtbl -> RAWReadNVRAM(This,hSession,nStartByte,cbRequested,pbData,pcbActual,pdwErrorCode) ) 


#define INVRAMWrapper2_GetNVRAMWrapper2Capabilities(This,hSession,pCapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetNVRAMWrapper2Capabilities(This,hSession,pCapabilities,pdwErrorCode) ) 

#define INVRAMWrapper2_GetNVRAMValue2(This,hSession,wTableIndex,dwVariable,pdwValue,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetNVRAMValue2(This,hSession,wTableIndex,dwVariable,pdwValue,pdwErrorCode) ) 

#define INVRAMWrapper2_SetNVRAMValue2(This,hSession,wTableIndex,dwVariable,dwValue,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetNVRAMValue2(This,hSession,wTableIndex,dwVariable,dwValue,pdwErrorCode) ) 

#define INVRAMWrapper2_GetNVRAMStringSize(This,hSession,dwVariable,pdwSize,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetNVRAMStringSize(This,hSession,dwVariable,pdwSize,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __INVRAMWrapper2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0015 */
/* [local] */ 




enum CONTROL_PANEL_ERRORS
    {
        TERR_NO_DESTINATIONS	= 600
    } ;

enum CONTROL_PANEL_COPY_TYPE
    {
        T_COPY_BLACK_AND_WHITE	= 0,
        T_COPY_COLOR	= 1
    } ;

enum CONTROL_PANEL_VALUES
    {
        T_CONTROL_PANEL_VALUE_COPY_TYPE	= 0x1,
        T_CONTROL_PANEL_VALUE_COPY_COUNT	= 0x2,
        T_CONTROL_PANEL_VALUE_SCANTO_INDEX	= 0x4,
        T_CONTROL_PANEL_VALUE_DESTINATION_COUNT	= 0x8,
        T_CONTROL_PANEL_VALUE_DESTINATION_INTERNAL_ID	= 0x10,
        T_CONTROL_PANEL_VALUE_COPY_COUNT_MAX	= 0x20,
        T_CONTROL_PANEL_VALUE_VIRTUAL_EVENTS	= 0x40,
        T_CONTROL_PANEL_VALUE_LCD_LANGUAGE	= 0x80
    } ;

enum CONTROL_PANEL_STRINGS
    {
        T_CONTROL_PANEL_STRING_LCD1	= 0x1
    } ;

enum CONTROL_PANEL_LCD_LANGUAGES
    {
        T_LCD_ENGLISH	= 0,
        T_LCD_FRENCH	= 0x1,
        T_LCD_ITALIAN	= 0x2,
        T_LCD_GERMAN	= 0x4,
        T_LCD_SPANISH	= 0x8,
        T_LCD_TRADITIONAL_CHINESE	= 0x10,
        T_LCD_SIMPLIFIED_CHINESE	= 0x20,
        T_LCD_KOREAN	= 0x40,
        T_LCD_PORTUGUESE	= 0x80
    } ;
struct CONTROL_PANEL_CAPABILITIES
    {
    BYTE byHasControlPanel;
    DWORD dwSupportedValues;
    DWORD dwSupportedStrings;
    DWORD dwCurrentCodePage;
    WORD wMaxStringCharacters[ 32 ];
    DWORD dwSupportedCodePages[ 16 ];
    DWORD dwDestinationListEntries;
    WORD dwMaxDestinationListEntryLength;
    DWORD dwSupportedLCDlanguages;
    DWORD dwReserved[ 9 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_cp[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct CONTROL_PANEL_CAPABILITIES) == 192);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0015_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0015_v0_0_s_ifspec;

#ifndef __IControlPanel_INTERFACE_DEFINED__
#define __IControlPanel_INTERFACE_DEFINED__

/* interface IControlPanel */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IControlPanel;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A7564228-0FBC-4bdf-ADD1-895C3BAACCBA")
    IControlPanel : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetControlPanelCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ struct CONTROL_PANEL_CAPABILITIES *pCapabilities,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetControlPanelValue( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [out] */ DWORD *pdwValue,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetControlPanelValue( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [in] */ DWORD dwValue,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCodePage( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwValue,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetControlPanelString( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [out] */ LPOLESTR *ppszString,
            /* [out] */ DWORD *pdwBytes,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetControlPanelString( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [in] */ LPOLESTR pszString,
            /* [in] */ DWORD dwBytes,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetControlPanelDestinationString( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwIndex,
            /* [in] */ LPOLESTR pszString,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IControlPanelVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IControlPanel * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IControlPanel * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IControlPanel * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetControlPanelCapabilities )( 
            IControlPanel * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct CONTROL_PANEL_CAPABILITIES *pCapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetControlPanelValue )( 
            IControlPanel * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [out] */ DWORD *pdwValue,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetControlPanelValue )( 
            IControlPanel * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [in] */ DWORD dwValue,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetCodePage )( 
            IControlPanel * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwValue,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetControlPanelString )( 
            IControlPanel * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [out] */ LPOLESTR *ppszString,
            /* [out] */ DWORD *pdwBytes,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetControlPanelString )( 
            IControlPanel * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [in] */ LPOLESTR pszString,
            /* [in] */ DWORD dwBytes,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetControlPanelDestinationString )( 
            IControlPanel * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwIndex,
            /* [in] */ LPOLESTR pszString,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IControlPanelVtbl;

    interface IControlPanel
    {
        CONST_VTBL struct IControlPanelVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IControlPanel_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IControlPanel_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IControlPanel_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IControlPanel_GetControlPanelCapabilities(This,hSession,pCapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetControlPanelCapabilities(This,hSession,pCapabilities,pdwErrorCode) ) 

#define IControlPanel_GetControlPanelValue(This,hSession,dwVariable,pdwValue,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetControlPanelValue(This,hSession,dwVariable,pdwValue,pdwErrorCode) ) 

#define IControlPanel_SetControlPanelValue(This,hSession,dwVariable,dwValue,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetControlPanelValue(This,hSession,dwVariable,dwValue,pdwErrorCode) ) 

#define IControlPanel_SetCodePage(This,hSession,dwValue,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetCodePage(This,hSession,dwValue,pdwErrorCode) ) 

#define IControlPanel_GetControlPanelString(This,hSession,dwVariable,ppszString,pdwBytes,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetControlPanelString(This,hSession,dwVariable,ppszString,pdwBytes,pdwErrorCode) ) 

#define IControlPanel_SetControlPanelString(This,hSession,dwVariable,pszString,dwBytes,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetControlPanelString(This,hSession,dwVariable,pszString,dwBytes,pdwErrorCode) ) 

#define IControlPanel_SetControlPanelDestinationString(This,hSession,dwIndex,pszString,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetControlPanelDestinationString(This,hSession,dwIndex,pszString,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IControlPanel_INTERFACE_DEFINED__ */


#ifndef __IControlPanel2_INTERFACE_DEFINED__
#define __IControlPanel2_INTERFACE_DEFINED__

/* interface IControlPanel2 */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IControlPanel2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("cf7f51cb-8b8b-4637-a9f3-93cb7c9a9f3b")
    IControlPanel2 : public IControlPanel
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetControlPanelDestinationInfoPair( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwIndex,
            /* [in] */ LPOLESTR pszDestString,
            /* [in] */ DWORD dwDestID,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetControlPanelDestinationIDList( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwDestCount,
            /* [size_is][size_is][out] */ DWORD **ppdwDestIDArray,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IControlPanel2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IControlPanel2 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IControlPanel2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IControlPanel2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetControlPanelCapabilities )( 
            IControlPanel2 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct CONTROL_PANEL_CAPABILITIES *pCapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetControlPanelValue )( 
            IControlPanel2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [out] */ DWORD *pdwValue,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetControlPanelValue )( 
            IControlPanel2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [in] */ DWORD dwValue,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetCodePage )( 
            IControlPanel2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwValue,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetControlPanelString )( 
            IControlPanel2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [out] */ LPOLESTR *ppszString,
            /* [out] */ DWORD *pdwBytes,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetControlPanelString )( 
            IControlPanel2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwVariable,
            /* [in] */ LPOLESTR pszString,
            /* [in] */ DWORD dwBytes,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetControlPanelDestinationString )( 
            IControlPanel2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwIndex,
            /* [in] */ LPOLESTR pszString,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetControlPanelDestinationInfoPair )( 
            IControlPanel2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwIndex,
            /* [in] */ LPOLESTR pszDestString,
            /* [in] */ DWORD dwDestID,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetControlPanelDestinationIDList )( 
            IControlPanel2 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwDestCount,
            /* [size_is][size_is][out] */ DWORD **ppdwDestIDArray,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IControlPanel2Vtbl;

    interface IControlPanel2
    {
        CONST_VTBL struct IControlPanel2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IControlPanel2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IControlPanel2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IControlPanel2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IControlPanel2_GetControlPanelCapabilities(This,hSession,pCapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetControlPanelCapabilities(This,hSession,pCapabilities,pdwErrorCode) ) 

#define IControlPanel2_GetControlPanelValue(This,hSession,dwVariable,pdwValue,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetControlPanelValue(This,hSession,dwVariable,pdwValue,pdwErrorCode) ) 

#define IControlPanel2_SetControlPanelValue(This,hSession,dwVariable,dwValue,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetControlPanelValue(This,hSession,dwVariable,dwValue,pdwErrorCode) ) 

#define IControlPanel2_SetCodePage(This,hSession,dwValue,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetCodePage(This,hSession,dwValue,pdwErrorCode) ) 

#define IControlPanel2_GetControlPanelString(This,hSession,dwVariable,ppszString,pdwBytes,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetControlPanelString(This,hSession,dwVariable,ppszString,pdwBytes,pdwErrorCode) ) 

#define IControlPanel2_SetControlPanelString(This,hSession,dwVariable,pszString,dwBytes,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetControlPanelString(This,hSession,dwVariable,pszString,dwBytes,pdwErrorCode) ) 

#define IControlPanel2_SetControlPanelDestinationString(This,hSession,dwIndex,pszString,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetControlPanelDestinationString(This,hSession,dwIndex,pszString,pdwErrorCode) ) 


#define IControlPanel2_SetControlPanelDestinationInfoPair(This,hSession,dwIndex,pszDestString,dwDestID,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetControlPanelDestinationInfoPair(This,hSession,dwIndex,pszDestString,dwDestID,pdwErrorCode) ) 

#define IControlPanel2_GetControlPanelDestinationIDList(This,hSession,pdwDestCount,ppdwDestIDArray,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetControlPanelDestinationIDList(This,hSession,pdwDestCount,ppdwDestIDArray,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IControlPanel2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0017 */
/* [local] */ 



enum TULIP_CHARSET
    {
        T_CHAR_SINGLE	= 1,
        T_CHAR_DOUBLE	= 2
    } ;

enum XML_MENU_TYPE
    {
        T_XML_ALL	= 1,
        T_XML_CURRENT_MENU	= 2
    } ;
struct XMLCONTROLPANEL_CAPABILITIES
    {
    WORD wCharSet;
    DWORD dwMaxDataSize;
    DWORD dwReserved[ 20 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_xml[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct XMLCONTROLPANEL_CAPABILITIES) == 88);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0017_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0017_v0_0_s_ifspec;

#ifndef __IControlPanelXML_INTERFACE_DEFINED__
#define __IControlPanelXML_INTERFACE_DEFINED__

/* interface IControlPanelXML */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IControlPanelXML;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A8273869-C850-4771-8683-7C4E4A865D21")
    IControlPanelXML : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetControlPanelXMLCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ struct XMLCONTROLPANEL_CAPABILITIES *pXMLCaps,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE PutXML( 
            /* [in] */ DWORD hSession,
            /* [in] */ BSTR bstrXMLdata,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetXML( 
            /* [in] */ DWORD hSession,
            /* [in] */ enum XML_MENU_TYPE menuType,
            /* [out] */ BSTR *pbstrXMLdata,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IControlPanelXMLVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IControlPanelXML * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IControlPanelXML * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IControlPanelXML * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetControlPanelXMLCapabilities )( 
            IControlPanelXML * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct XMLCONTROLPANEL_CAPABILITIES *pXMLCaps,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *PutXML )( 
            IControlPanelXML * This,
            /* [in] */ DWORD hSession,
            /* [in] */ BSTR bstrXMLdata,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetXML )( 
            IControlPanelXML * This,
            /* [in] */ DWORD hSession,
            /* [in] */ enum XML_MENU_TYPE menuType,
            /* [out] */ BSTR *pbstrXMLdata,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IControlPanelXMLVtbl;

    interface IControlPanelXML
    {
        CONST_VTBL struct IControlPanelXMLVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IControlPanelXML_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IControlPanelXML_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IControlPanelXML_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IControlPanelXML_GetControlPanelXMLCapabilities(This,hSession,pXMLCaps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetControlPanelXMLCapabilities(This,hSession,pXMLCaps,pdwErrorCode) ) 

#define IControlPanelXML_PutXML(This,hSession,bstrXMLdata,pdwErrorCode)	\
    ( (This)->lpVtbl -> PutXML(This,hSession,bstrXMLdata,pdwErrorCode) ) 

#define IControlPanelXML_GetXML(This,hSession,menuType,pbstrXMLdata,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetXML(This,hSession,menuType,pbstrXMLdata,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IControlPanelXML_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0018 */
/* [local] */ 


typedef /* [public][public][public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0018_0001
    {
    double dm11;
    double dm12;
    double dm13;
    double dm21;
    double dm22;
    double dm23;
    double dm31;
    double dm32;
    double dm33;
    } 	RGB_MATRIX;

typedef /* [public][public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0018_0002
    {
    WORD wScanModesSupported;
    BYTE byCanUploadMatrix;
    BYTE byCanDownloadMatrix;
    DWORD dwReserved[ 10 ];
    } 	RGB_MATRIX_CAPABILITIES;

// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_rgbmatrix[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(RGB_MATRIX_CAPABILITIES) == 44);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0018_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0018_v0_0_s_ifspec;

#ifndef __IRGBMatrix_INTERFACE_DEFINED__
#define __IRGBMatrix_INTERFACE_DEFINED__

/* interface IRGBMatrix */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IRGBMatrix;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("45A6DE15-FA6C-48B6-AAF6-7B98158AC0F4")
    IRGBMatrix : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetRGBMatrixCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ RGB_MATRIX_CAPABILITIES *pRGBMatrixCaps,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetRGBMatrix( 
            /* [in] */ DWORD hSession,
            /* [out] */ RGB_MATRIX *pMatrix,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetRGBMatrix( 
            /* [in] */ DWORD hSession,
            /* [in] */ RGB_MATRIX *pMatrix,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IRGBMatrixVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IRGBMatrix * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IRGBMatrix * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IRGBMatrix * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetRGBMatrixCapabilities )( 
            IRGBMatrix * This,
            /* [in] */ DWORD hSession,
            /* [out] */ RGB_MATRIX_CAPABILITIES *pRGBMatrixCaps,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetRGBMatrix )( 
            IRGBMatrix * This,
            /* [in] */ DWORD hSession,
            /* [out] */ RGB_MATRIX *pMatrix,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetRGBMatrix )( 
            IRGBMatrix * This,
            /* [in] */ DWORD hSession,
            /* [in] */ RGB_MATRIX *pMatrix,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IRGBMatrixVtbl;

    interface IRGBMatrix
    {
        CONST_VTBL struct IRGBMatrixVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IRGBMatrix_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IRGBMatrix_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IRGBMatrix_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IRGBMatrix_GetRGBMatrixCapabilities(This,hSession,pRGBMatrixCaps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetRGBMatrixCapabilities(This,hSession,pRGBMatrixCaps,pdwErrorCode) ) 

#define IRGBMatrix_GetRGBMatrix(This,hSession,pMatrix,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetRGBMatrix(This,hSession,pMatrix,pdwErrorCode) ) 

#define IRGBMatrix_SetRGBMatrix(This,hSession,pMatrix,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetRGBMatrix(This,hSession,pMatrix,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IRGBMatrix_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0019 */
/* [local] */ 



enum TULIP_LOG_ATTRIB
    {
        TLOG_POLLED	= 0x1,
        TLOG_ENTRYEXIT	= 0x2,
        TLOG_LOCK_UNLOCK	= 0x4,
        TLOG_INFO	= 0x8,
        TLOG_STRUCT	= 0x10,
        TLOG_DATA	= 0x20,
        TLOG_BASEINFO	= 0x40,
        TLOG_USER1	= 0x1000000,
        TLOG_USER2	= 0x2000000,
        TLOG_USER3	= 0x4000000,
        TLOG_USER4	= 0x8000000,
        TLOG_DBG	= 0x10000000,
        TLOG_WRN	= 0x20000000,
        TLOG_ERR	= 0x80000000,
        TLOG_ALL_LEVEL	= 0xf0000000,
        TLOG_ALL_TYPES	= 0xfffffff
    } ;

enum TULIP_LOG_DESTINATION
    {
        TLOG_DESTINATION_FILE	= 1,
        TLOG_DESTINATION_TRACE	= 2,
        TLOG_DESTINATION_APP	= 4
    } ;


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0019_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0019_v0_0_s_ifspec;

#ifndef __ILog_INTERFACE_DEFINED__
#define __ILog_INTERFACE_DEFINED__

/* interface ILog */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_ILog;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("EB0D7369-5CB0-437E-B61B-1B6A4D8538C0")
    ILog : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE LogString( 
            /* [in] */ DWORD dwFunctionAttribute,
            /* [string][in] */ LPOLESTR pszString) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE IsEnabled( 
            /* [in] */ DWORD dwFunctionAttribute,
            /* [out] */ BYTE *pbyEnabled) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ILogVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ILog * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ILog * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ILog * This);
        
        HRESULT ( STDMETHODCALLTYPE *LogString )( 
            ILog * This,
            /* [in] */ DWORD dwFunctionAttribute,
            /* [string][in] */ LPOLESTR pszString);
        
        HRESULT ( STDMETHODCALLTYPE *IsEnabled )( 
            ILog * This,
            /* [in] */ DWORD dwFunctionAttribute,
            /* [out] */ BYTE *pbyEnabled);
        
        END_INTERFACE
    } ILogVtbl;

    interface ILog
    {
        CONST_VTBL struct ILogVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ILog_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ILog_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ILog_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ILog_LogString(This,dwFunctionAttribute,pszString)	\
    ( (This)->lpVtbl -> LogString(This,dwFunctionAttribute,pszString) ) 

#define ILog_IsEnabled(This,dwFunctionAttribute,pbyEnabled)	\
    ( (This)->lpVtbl -> IsEnabled(This,dwFunctionAttribute,pbyEnabled) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ILog_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0020 */
/* [local] */ 



enum CALIBRATION_TYPE
    {
        CT_NONE	= 0,
        CT_DSNU	= 1,
        CT_PRNU	= 2,
        CT_ANALOGGAIN	= 4,
        CT_PWM	= 8,
        CT_FULL	= 0xffffffff
    } ;
struct CALIBRATION_AREA
    {
    DWORD dwLeft;
    DWORD dwTop;
    DWORD dwWidth;
    DWORD dwLength;
    DWORD dwReserved[ 10 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_calib[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct CALIBRATION_AREA) == 56);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0020_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0020_v0_0_s_ifspec;

#ifndef __ICalibrationSettings_INTERFACE_DEFINED__
#define __ICalibrationSettings_INTERFACE_DEFINED__

/* interface ICalibrationSettings */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_ICalibrationSettings;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("91C4C6D4-7773-4533-8400-C06DE080422B")
    ICalibrationSettings : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetCalibrationType( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwType,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCalibrationArea( 
            /* [in] */ DWORD hSession,
            /* [in] */ struct CALIBRATION_AREA *pArea,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentCalibrationGain( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdw600RedGain,
            /* [out] */ DWORD *pdw600GreenGain,
            /* [out] */ DWORD *pdw600BlueGain,
            /* [out] */ DWORD *pdw1200RedGain,
            /* [out] */ DWORD *pdw1200GreenGain,
            /* [out] */ DWORD *pdw1200BlueGain,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ICalibrationSettingsVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ICalibrationSettings * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ICalibrationSettings * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ICalibrationSettings * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetCalibrationType )( 
            ICalibrationSettings * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwType,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetCalibrationArea )( 
            ICalibrationSettings * This,
            /* [in] */ DWORD hSession,
            /* [in] */ struct CALIBRATION_AREA *pArea,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentCalibrationGain )( 
            ICalibrationSettings * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdw600RedGain,
            /* [out] */ DWORD *pdw600GreenGain,
            /* [out] */ DWORD *pdw600BlueGain,
            /* [out] */ DWORD *pdw1200RedGain,
            /* [out] */ DWORD *pdw1200GreenGain,
            /* [out] */ DWORD *pdw1200BlueGain,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } ICalibrationSettingsVtbl;

    interface ICalibrationSettings
    {
        CONST_VTBL struct ICalibrationSettingsVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ICalibrationSettings_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ICalibrationSettings_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ICalibrationSettings_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ICalibrationSettings_SetCalibrationType(This,hSession,dwType,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetCalibrationType(This,hSession,dwType,pdwErrorCode) ) 

#define ICalibrationSettings_SetCalibrationArea(This,hSession,pArea,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetCalibrationArea(This,hSession,pArea,pdwErrorCode) ) 

#define ICalibrationSettings_GetCurrentCalibrationGain(This,hSession,pdw600RedGain,pdw600GreenGain,pdw600BlueGain,pdw1200RedGain,pdw1200GreenGain,pdw1200BlueGain,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetCurrentCalibrationGain(This,hSession,pdw600RedGain,pdw600GreenGain,pdw600BlueGain,pdw1200RedGain,pdw1200GreenGain,pdw1200BlueGain,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ICalibrationSettings_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0021 */
/* [local] */ 




extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0021_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0021_v0_0_s_ifspec;

#ifndef __ILampTimer_INTERFACE_DEFINED__
#define __ILampTimer_INTERFACE_DEFINED__

/* interface ILampTimer */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_ILampTimer;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B70D71A5-C304-4c38-A7B8-1C81679813AC")
    ILampTimer : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetLampTimer( 
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wMinutes,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ILampTimerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ILampTimer * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ILampTimer * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ILampTimer * This);
        
        HRESULT ( STDMETHODCALLTYPE *SetLampTimer )( 
            ILampTimer * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wMinutes,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } ILampTimerVtbl;

    interface ILampTimer
    {
        CONST_VTBL struct ILampTimerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ILampTimer_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ILampTimer_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ILampTimer_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ILampTimer_SetLampTimer(This,hSession,wMinutes,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetLampTimer(This,hSession,wMinutes,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ILampTimer_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0022 */
/* [local] */ 


#define PROP_CCDTYPE L"CCD Type"
#define PROP_FWVERSION L"Firmware Version"
#define PROP_ACTIVELAMP L"CurrentLampOn"
#define PROP_SUPPORTXPA L"XPA"
#define PROP_SUPPORTADF L"ADF"
#define PROP_ADFFWVERSION L"ADF Firmware Version"
#define PROP_EXACTMODELSTRING L"Exact Model String"
#define PROP_LINESSCANNED L"Lines Last Scanned"
#define PROP_NICFWVERSION L"NIC FW Version"
#define PROP_IPADDR L"IP Address"
#define PROP_CONNECTTYPE L"Connection Type"
#define PROP_ADF_SCAN_SPEED L"PROP_ADF_SCAN_SPEED"
#define PROP_IM_DETECTED_COLOR_MODE L"PROPSTR_IM_DETECTED_COLOR_MODE"
#define PROP_IM_OUTPUT_COLOR_MODE L"PROPSTR_IM_OUTPUT_COLOR_MODE"
#define PROP_IM_OUTPUT_DATA_TYPE L"PROPSTR_IM_OUTPUT_DATA_TYPE"
#define PROP_IM_TRANSFER_BYTES L"PROPVAL_IM_TRANSFER_BYTES"
#define PROP_IM_PIXEL_WIDTH L"PROPVAL_IM_PIXEL_WIDTH"
#define PROP_IM_PIXEL_HEIGHT L"PROPVAL_IM_PIXEL_HEIGHT"
#define PROP_IM_BYTES_PER_LINE L"PROPVAL_IM_BYTES_PER_LINE"
#define PROP_IM_BLANK_PAGE_DETECTED L"PROPVAL_IM_BLANK_PAGE_DETECTED"
#define PROP_IM_DETECTED_BW_THRESHOLD L"PROPVAL_IM_DETECTED_BW_THRESHOLD"
#define PROP_IM_ORIENTATION_CORRECTION L"PROPVAL_IM_ORIENTATION_CORRECTION"
#define PROP_IM_SKEW_ANGLE L"PROPVAL_IM_SKEW_ANGLE"
#define PROP_DEFAULT_TM_GAMMA L"PROPVAL_DEFAULT_TM_GAMMA"
#define PROP_DEFAULT_TM_BRIGHTNESS L"PROPVAL_DEFAULT_TM_BRIGHTNESS"
#define PROP_DEFAULT_TM_CONTRAST L"PROPVAL_DEFAULT_TM_CONTRAST"
#define PROP_DEFAULT_TM_HIGHLIGHT L"PROPVAL_DEFAULT_TM_HIGHLIGHT"
#define PROP_DEFAULT_TM_SHADOW L"PROPVAL_DEFAULT_TM_SHADOW"
#define PROP_DEFAULT_BW_THRESHOLD L"PROPVAL_DEFAULT_BW_THRESHOLD"
#define PROP_AIO_FRONTPANEL_SHORTCUT  L"PROPSTR_AIO_FRONTPANEL_SHORTCUT"
#define PROP_AIO_FRONTPANEL_DUPLEX    L"PROPVAL_AIO_FRONTPANEL_DUPLEX"
#define PROP_AIO_FRONTPANEL_SCAN_CAPS    L"PROP_AIO_FRONTPANEL_SCAN_CAPS"
#define PROP_AIO_FRONTPANEL_USER_TIMEOUT    L"PROP_AIO_FRONTPANEL_USER_TIMEOUT"
#define PROP_AIO_FRONTPANEL_FLOW_STATE    L"PROP_AIO_FRONTPANEL_FLOW_STATE"
#define PROPSTR_CCDTYPE							PROP_CCDTYPE
#define PROPSTR_FWVERSION						PROP_FWVERSION
#define PROPSTR_ACTIVELAMP						PROP_ACTIVELAMP
#define PROPSTR_SUPPORTXPA						PROP_SUPPORTXPA
#define PROPSTR_SUPPORTADF						PROP_SUPPORTADF
#define PROPSTR_ADFFWVERSION						PROP_ADFFWVERSION
#define PROPSTR_EXACTMODELSTRING					PROP_EXACTMODELSTRING
#define PROPSTR_LINESSCANNED						PROP_LINESSCANNED
#define PROPSTR_NICFWVERSION						PROP_NICFWVERSION
#define PROPSTR_IPADDR							PROP_IPADDR
#define PROPSTR_CONNECTTYPE						PROP_CONNECTTYPE
#define PROPSTR_IM_DETECTED_COLOR_MODE			PROP_IM_DETECTED_COLOR_MODE
#define PROPSTR_IM_OUTPUT_COLOR_MODE				PROP_IM_OUTPUT_COLOR_MODE
#define PROPSTR_IM_OUTPUT_DATA_TYPE				PROP_IM_OUTPUT_DATA_TYPE
#define PROPVAL_IM_TRANSFER_BYTES				PROP_IM_TRANSFER_BYTES
#define PROPVAL_IM_PIXEL_WIDTH					PROP_IM_PIXEL_WIDTH
#define PROPVAL_IM_PIXEL_HEIGHT					PROP_IM_PIXEL_HEIGHT
#define PROPVAL_IM_BYTES_PER_LINE				PROP_IM_BYTES_PER_LINE
#define PROPVAL_IM_BLANK_PAGE_DETECTED			PROP_IM_BLANK_PAGE_DETECTED
#define PROPVAL_IM_DETECTED_BW_THRESHOLD			PROP_IM_DETECTED_BW_THRESHOLD
#define PROPVAL_IM_ORIENTATION_CORRECTION		PROP_IM_ORIENTATION_CORRECTION
#define PROPVAL_IM_SKEW_ANGLE					PROP_IM_SKEW_ANGLE
#define PROPVAL_DEFAULT_TM_GAMMA					PROP_DEFAULT_TM_GAMMA
#define PROPVAL_DEFAULT_TM_BRIGHTNESS			PROP_DEFAULT_TM_BRIGHTNESS
#define PROPVAL_DEFAULT_TM_CONTRAST				PROP_DEFAULT_TM_CONTRAST
#define PROPVAL_DEFAULT_TM_HIGHLIGHT				PROP_DEFAULT_TM_HIGHLIGHT
#define PROPVAL_DEFAULT_TM_SHADOW				PROP_DEFAULT_TM_SHADOW
#define PROPVAL_DEFAULT_BW_THRESHOLD				PROP_DEFAULT_BW_THRESHOLD
#define PROPSTR_AIO_FRONTPANEL_SHORTCUT			PROP_AIO_FRONTPANEL_SHORTCUT
#define PROPVAL_AIO_FRONTPANEL_DUPLEX			PROP_AIO_FRONTPANEL_DUPLEX


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0022_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0022_v0_0_s_ifspec;

#ifndef __IHWProperty_INTERFACE_DEFINED__
#define __IHWProperty_INTERFACE_DEFINED__

/* interface IHWProperty */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IHWProperty;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0A5CFEE1-21F4-4811-A52B-C8E5A398B15B")
    IHWProperty : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetHWPropString( 
            /* [in] */ DWORD hSession,
            /* [in] */ LPOLESTR pszTagString,
            /* [out] */ LPOLESTR *ppszString,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHWPropValue( 
            /* [in] */ DWORD hSession,
            /* [in] */ LPOLESTR pszTagString,
            /* [out] */ DWORD *pdwValue,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IHWPropertyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IHWProperty * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IHWProperty * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IHWProperty * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetHWPropString )( 
            IHWProperty * This,
            /* [in] */ DWORD hSession,
            /* [in] */ LPOLESTR pszTagString,
            /* [out] */ LPOLESTR *ppszString,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetHWPropValue )( 
            IHWProperty * This,
            /* [in] */ DWORD hSession,
            /* [in] */ LPOLESTR pszTagString,
            /* [out] */ DWORD *pdwValue,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IHWPropertyVtbl;

    interface IHWProperty
    {
        CONST_VTBL struct IHWPropertyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IHWProperty_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IHWProperty_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IHWProperty_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IHWProperty_GetHWPropString(This,hSession,pszTagString,ppszString,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetHWPropString(This,hSession,pszTagString,ppszString,pdwErrorCode) ) 

#define IHWProperty_GetHWPropValue(This,hSession,pszTagString,pdwValue,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetHWPropValue(This,hSession,pszTagString,pdwValue,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IHWProperty_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0023 */
/* [local] */ 



enum STRINGTABLE_ERRORS
    {
        TERR_STRINGTABLE_NO_RESDLL	= 200,
        TERR_STRINGTABLE_NO_STRINGS	= 201
    } ;
struct STRINGTABLE_CAPABILITIES
    {
    BYTE byHasStringTable;
    WORD wStringTableSize;
    WORD wMaxStrings;
    DWORD dwReserved[ 10 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_string[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct STRINGTABLE_CAPABILITIES) == 48);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0023_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0023_v0_0_s_ifspec;

#ifndef __IStringTable_INTERFACE_DEFINED__
#define __IStringTable_INTERFACE_DEFINED__

/* interface IStringTable */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IStringTable;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("966051B8-E98E-4BC2-9F52-822FD7F199C4")
    IStringTable : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetStringTableCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ struct STRINGTABLE_CAPABILITIES *pStringTableCapabilities,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LoadDefaultStringTable( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetStringTable( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD cbRequested,
            /* [size_is][in] */ BYTE *pbStringData,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IStringTableVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IStringTable * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IStringTable * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IStringTable * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetStringTableCapabilities )( 
            IStringTable * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct STRINGTABLE_CAPABILITIES *pStringTableCapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *LoadDefaultStringTable )( 
            IStringTable * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetStringTable )( 
            IStringTable * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD cbRequested,
            /* [size_is][in] */ BYTE *pbStringData,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IStringTableVtbl;

    interface IStringTable
    {
        CONST_VTBL struct IStringTableVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IStringTable_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IStringTable_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IStringTable_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IStringTable_GetStringTableCapabilities(This,hSession,pStringTableCapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetStringTableCapabilities(This,hSession,pStringTableCapabilities,pdwErrorCode) ) 

#define IStringTable_LoadDefaultStringTable(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> LoadDefaultStringTable(This,hSession,pdwErrorCode) ) 

#define IStringTable_SetStringTable(This,hSession,cbRequested,pbStringData,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetStringTable(This,hSession,cbRequested,pbStringData,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IStringTable_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0024 */
/* [local] */ 


typedef /* [public][public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0024_0001
    {
    WORD wRows;
    WORD wColumns;
    BYTE byHasSymmetricMatrices;
    BYTE byHasSeparateRGBMatrices;
    BYTE byHasPowerOf2ScalingOnly;
    WORD wMaxHWConvolutionResolution;
    WORD wReserved[ 19 ];
    } 	CONVOLUTION_CAPABILITIES;

// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_conv[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(CONVOLUTION_CAPABILITIES) == 48);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0024_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0024_v0_0_s_ifspec;

#ifndef __IConvolution_INTERFACE_DEFINED__
#define __IConvolution_INTERFACE_DEFINED__

/* interface IConvolution */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IConvolution;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F7213FDD-1BAD-4346-AB1D-F254AAE7754F")
    IConvolution : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetConvolutionCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ CONVOLUTION_CAPABILITIES *pConvolutionCapabilities,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetConvolutionMatrices( 
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wRows,
            /* [in] */ WORD wColumns,
            /* [size_is] */ BYTE *pbyRedMatrix,
            /* [in] */ BYTE wRedScale,
            /* [size_is] */ BYTE *pbyGreenMatrix,
            /* [in] */ BYTE wGreenScale,
            /* [size_is] */ BYTE *pbyBlueMatrix,
            /* [in] */ BYTE wBlueScale,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IConvolutionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IConvolution * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IConvolution * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IConvolution * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetConvolutionCapabilities )( 
            IConvolution * This,
            /* [in] */ DWORD hSession,
            /* [out] */ CONVOLUTION_CAPABILITIES *pConvolutionCapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetConvolutionMatrices )( 
            IConvolution * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wRows,
            /* [in] */ WORD wColumns,
            /* [size_is] */ BYTE *pbyRedMatrix,
            /* [in] */ BYTE wRedScale,
            /* [size_is] */ BYTE *pbyGreenMatrix,
            /* [in] */ BYTE wGreenScale,
            /* [size_is] */ BYTE *pbyBlueMatrix,
            /* [in] */ BYTE wBlueScale,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IConvolutionVtbl;

    interface IConvolution
    {
        CONST_VTBL struct IConvolutionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IConvolution_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IConvolution_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IConvolution_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IConvolution_GetConvolutionCapabilities(This,hSession,pConvolutionCapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetConvolutionCapabilities(This,hSession,pConvolutionCapabilities,pdwErrorCode) ) 

#define IConvolution_SetConvolutionMatrices(This,hSession,wRows,wColumns,pbyRedMatrix,wRedScale,pbyGreenMatrix,wGreenScale,pbyBlueMatrix,wBlueScale,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetConvolutionMatrices(This,hSession,wRows,wColumns,pbyRedMatrix,wRedScale,pbyGreenMatrix,wGreenScale,pbyBlueMatrix,wBlueScale,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IConvolution_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0025 */
/* [local] */ 



enum DITHER_MATRIX_TYPE
    {
        T_DITHER_8x8	= 0x1,
        T_DITHER_16x16	= 0x2
    } ;

enum DITHER_TYPE
    {
        T_DITHER_ERRORDIFFUSE	= 0x1,
        T_DITHER_PATTERN	= 0x2
    } ;
typedef /* [public][public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0025_0001
    {
    DWORD dwDither[ 8 ][ 8 ];
    } 	DITHER8x8_MATRIX;

typedef /* [public][public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0025_0002
    {
    DWORD dwDither[ 16 ][ 16 ];
    } 	DITHER16x16_MATRIX;

typedef /* [public][public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0025_0003
    {
    DWORD dwDitherType;
    DWORD dwMatricesSupported;
    DWORD dwMaxEntryValue;
    DWORD dwReserved[ 10 ];
    } 	DITHER_CAPABILITIES;

// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_dither[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(DITHER_CAPABILITIES) == 52);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0025_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0025_v0_0_s_ifspec;

#ifndef __IDither_INTERFACE_DEFINED__
#define __IDither_INTERFACE_DEFINED__

/* interface IDither */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IDither;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("46D00E3B-DE6D-40D5-B381-FEC9249661C8")
    IDither : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetDitherCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ DITHER_CAPABILITIES *pDitherCapabilities,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetDither8x8Matrix( 
            /* [in] */ DWORD hSession,
            /* [in] */ DITHER8x8_MATRIX *pMatrix,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetDither16x16Matrix( 
            /* [in] */ DWORD hSession,
            /* [in] */ DITHER16x16_MATRIX *pMatrix,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IDitherVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IDither * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IDither * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IDither * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetDitherCapabilities )( 
            IDither * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DITHER_CAPABILITIES *pDitherCapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetDither8x8Matrix )( 
            IDither * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DITHER8x8_MATRIX *pMatrix,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetDither16x16Matrix )( 
            IDither * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DITHER16x16_MATRIX *pMatrix,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IDitherVtbl;

    interface IDither
    {
        CONST_VTBL struct IDitherVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IDither_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IDither_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IDither_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IDither_GetDitherCapabilities(This,hSession,pDitherCapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetDitherCapabilities(This,hSession,pDitherCapabilities,pdwErrorCode) ) 

#define IDither_SetDither8x8Matrix(This,hSession,pMatrix,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetDither8x8Matrix(This,hSession,pMatrix,pdwErrorCode) ) 

#define IDither_SetDither16x16Matrix(This,hSession,pMatrix,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetDither16x16Matrix(This,hSession,pMatrix,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IDither_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0026 */
/* [local] */ 




extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0026_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0026_v0_0_s_ifspec;

#ifndef __ILampInstantWarmup_INTERFACE_DEFINED__
#define __ILampInstantWarmup_INTERFACE_DEFINED__

/* interface ILampInstantWarmup */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_ILampInstantWarmup;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3A877B0E-E5C5-42CA-BB7F-C97CBF5B0B49")
    ILampInstantWarmup : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetLampInstantWarmupState( 
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbyState,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetLampInstantWarmupState( 
            /* [in] */ DWORD hSession,
            /* [in] */ BYTE byState,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ILampInstantWarmupVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ILampInstantWarmup * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ILampInstantWarmup * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ILampInstantWarmup * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetLampInstantWarmupState )( 
            ILampInstantWarmup * This,
            /* [in] */ DWORD hSession,
            /* [out] */ BYTE *pbyState,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetLampInstantWarmupState )( 
            ILampInstantWarmup * This,
            /* [in] */ DWORD hSession,
            /* [in] */ BYTE byState,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } ILampInstantWarmupVtbl;

    interface ILampInstantWarmup
    {
        CONST_VTBL struct ILampInstantWarmupVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ILampInstantWarmup_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ILampInstantWarmup_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ILampInstantWarmup_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ILampInstantWarmup_GetLampInstantWarmupState(This,hSession,pbyState,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetLampInstantWarmupState(This,hSession,pbyState,pdwErrorCode) ) 

#define ILampInstantWarmup_SetLampInstantWarmupState(This,hSession,byState,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetLampInstantWarmupState(This,hSession,byState,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ILampInstantWarmup_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0027 */
/* [local] */ 



enum COMPRESSION_TYPES
    {
        T_COMPRESS_NONE	= 0x1,
        T_COMPRESS_YCC	= 0x2,
        T_COMPRESS_JPEG	= 0x4,
        T_COMPRESS_PNG	= 0x8
    } ;

enum COMPRESSION_LONGEVITY
    {
        T_COMPRESS_WIRE_ONLY	= 0x1,
        T_COMPRESS_PASS_THRU	= 0x2
    } ;

enum COMPRESSION_OPTIONS
    {
        T_COMPRESS_OPT_JPEG_LEVEL	= 0x1,
        T_COMPRESS_OPT_JPEG_PROGRESSIVE	= 0x2,
        T_COMPRESS_OPT_JPEG_CT	= 0x4,
        T_COMPRESS_OPT_JPEG_THUMBNAILS	= 0x8,
        T_COMPRESS_OPT_JPEG_422	= 0x10,
        T_COMPRESS_OPT_JPEG_420	= 0x20,
        T_COMPRESS_OPT_JPEG_444	= 0x40
    } ;
struct COMPRESSION_CAPABILITIES
    {
    DWORD dwTypesOfCompression;
    DWORD dwDefaultCompressionType;
    DWORD dwCompressionLongevity;
    DWORD dwJPEGLevelMinimum;
    DWORD dwJPEGLevelMaximum;
    DWORD dwCompressionOptions;
    WORD wMaxHWCompressionResolution;
    DWORD dwExcludeColorCompressionTypes;
    DWORD dwExcludeGreyscaleCompressionTypes;
    DWORD dwExcludeBWCompressionTypes;
    WORD wReserved[ 12 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_compress[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct COMPRESSION_CAPABILITIES) == 64);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0027_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0027_v0_0_s_ifspec;

#ifndef __ICompression_INTERFACE_DEFINED__
#define __ICompression_INTERFACE_DEFINED__

/* interface ICompression */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_ICompression;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E13D2DFA-D08C-4bf0-A964-21FC0EDE1017")
    ICompression : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCompressionCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ struct COMPRESSION_CAPABILITIES *pCompressionCapabilities,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCompressionType( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwType,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCompressionLongevity( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwLongevity,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCompressionOption( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwCompOption,
            /* [in] */ DWORD dwValue,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ICompressionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ICompression * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ICompression * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ICompression * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCompressionCapabilities )( 
            ICompression * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct COMPRESSION_CAPABILITIES *pCompressionCapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetCompressionType )( 
            ICompression * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwType,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetCompressionLongevity )( 
            ICompression * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwLongevity,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetCompressionOption )( 
            ICompression * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwCompOption,
            /* [in] */ DWORD dwValue,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } ICompressionVtbl;

    interface ICompression
    {
        CONST_VTBL struct ICompressionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ICompression_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ICompression_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ICompression_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ICompression_GetCompressionCapabilities(This,hSession,pCompressionCapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetCompressionCapabilities(This,hSession,pCompressionCapabilities,pdwErrorCode) ) 

#define ICompression_SetCompressionType(This,hSession,dwType,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetCompressionType(This,hSession,dwType,pdwErrorCode) ) 

#define ICompression_SetCompressionLongevity(This,hSession,dwLongevity,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetCompressionLongevity(This,hSession,dwLongevity,pdwErrorCode) ) 

#define ICompression_SetCompressionOption(This,hSession,dwCompOption,dwValue,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetCompressionOption(This,hSession,dwCompOption,dwValue,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ICompression_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0028 */
/* [local] */ 



enum RESERVE_ERRORS
    {
        TERR_RESERVE_BUSY	= 400,
        TERR_RESERVE_FAILED	= 401
    } ;
typedef /* [public][public][public] */ 
enum __MIDL___MIDL_itf_hpgtblues_0000_0028_0001
    {
        T_URS_DEFAULT	= 1,
        T_URS_WAIT	= 2,
        T_URS_START	= 3
    } 	URS_STATE;



extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0028_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0028_v0_0_s_ifspec;

#ifndef __IReserve_INTERFACE_DEFINED__
#define __IReserve_INTERFACE_DEFINED__

/* interface IReserve */
/* [version][unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IReserve;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("DB27523C-5AD3-4A7C-9B46-3A488E796FC0")
    IReserve : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE JobReserveHW( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE JobReleaseHW( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetUserReadyToScanState( 
            /* [in] */ DWORD hSession,
            /* [out] */ URS_STATE *pbyState,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetUserReadyToScanState( 
            /* [in] */ DWORD hSession,
            /* [in] */ URS_STATE byState,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IReserveVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IReserve * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IReserve * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IReserve * This);
        
        HRESULT ( STDMETHODCALLTYPE *JobReserveHW )( 
            IReserve * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwTimeout,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *JobReleaseHW )( 
            IReserve * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetUserReadyToScanState )( 
            IReserve * This,
            /* [in] */ DWORD hSession,
            /* [out] */ URS_STATE *pbyState,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetUserReadyToScanState )( 
            IReserve * This,
            /* [in] */ DWORD hSession,
            /* [in] */ URS_STATE byState,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IReserveVtbl;

    interface IReserve
    {
        CONST_VTBL struct IReserveVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IReserve_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IReserve_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IReserve_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IReserve_JobReserveHW(This,hSession,dwTimeout,pdwErrorCode)	\
    ( (This)->lpVtbl -> JobReserveHW(This,hSession,dwTimeout,pdwErrorCode) ) 

#define IReserve_JobReleaseHW(This,hSession,pdwErrorCode)	\
    ( (This)->lpVtbl -> JobReleaseHW(This,hSession,pdwErrorCode) ) 

#define IReserve_GetUserReadyToScanState(This,hSession,pbyState,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetUserReadyToScanState(This,hSession,pbyState,pdwErrorCode) ) 

#define IReserve_SetUserReadyToScanState(This,hSession,byState,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetUserReadyToScanState(This,hSession,byState,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IReserve_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0029 */
/* [local] */ 



enum ADF_DIAG_ERRORS
    {
        TERR_DIAG_NO_ADF	= 120,
        TERR_DIAG_MOVE_FAILED	= 121
    } ;

enum ADF_DIAG_TYPE
    {
        DIAG_CMD_CARRIAGE_2_ADF_SCAN_POS	= 11,
        DIAG_CMD_CARRIAGE_2_HOME_POS	= 12,
        DIAG_CMD_CARRIAGE_2_FB_SCAN_ZONE_END	= 13,
        DIAG_CMD_CARRIAGE_FORWARD	= 14,
        DIAG_CMD_ADF_PAPER_FEED_FW	= 21,
        DIAG_CMD_ADF_PAPER_FEED_REV	= 22,
        DIAG_CMD_ADF_BOGIE_UP	= 31,
        DIAG_CMD_ADF_BOGIE_DOWN	= 32,
        DIAG_CMD_XPA_ON	= 33,
        DIAG_CMD_XPA_OFF	= 34,
        DIAG_STATUS_INPUT_1	= 41,
        DIAG_STATUS_INPUT_2	= 42,
        DIAG_STATUS_INPUT_3	= 43,
        DIAG_STATUS_INPUT_4	= 44,
        DIAG_STATUS_INPUT_5	= 45,
        DIAG_STATUS_INPUT_6	= 46,
        DIAG_STATUS_INPUT_7	= 47,
        DIAG_STATUS_INPUT_8	= 48
    } ;


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0029_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0029_v0_0_s_ifspec;

#ifndef __IADFDiagnostic_INTERFACE_DEFINED__
#define __IADFDiagnostic_INTERFACE_DEFINED__

/* interface IADFDiagnostic */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IADFDiagnostic;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E383AAD9-AEBB-452D-9E70-62B747BD7D9D")
    IADFDiagnostic : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE DiagnosticCommand( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwDiagType,
            /* [in] */ DWORD dwParam1,
            /* [in] */ DWORD dwParam2,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DiagnosticStatus( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwDiagType,
            /* [out] */ DWORD *pdwResult,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IADFDiagnosticVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IADFDiagnostic * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IADFDiagnostic * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IADFDiagnostic * This);
        
        HRESULT ( STDMETHODCALLTYPE *DiagnosticCommand )( 
            IADFDiagnostic * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwDiagType,
            /* [in] */ DWORD dwParam1,
            /* [in] */ DWORD dwParam2,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *DiagnosticStatus )( 
            IADFDiagnostic * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwDiagType,
            /* [out] */ DWORD *pdwResult,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IADFDiagnosticVtbl;

    interface IADFDiagnostic
    {
        CONST_VTBL struct IADFDiagnosticVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IADFDiagnostic_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IADFDiagnostic_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IADFDiagnostic_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IADFDiagnostic_DiagnosticCommand(This,hSession,dwDiagType,dwParam1,dwParam2,pdwErrorCode)	\
    ( (This)->lpVtbl -> DiagnosticCommand(This,hSession,dwDiagType,dwParam1,dwParam2,pdwErrorCode) ) 

#define IADFDiagnostic_DiagnosticStatus(This,hSession,dwDiagType,pdwResult,pdwErrorCode)	\
    ( (This)->lpVtbl -> DiagnosticStatus(This,hSession,dwDiagType,pdwResult,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IADFDiagnostic_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0030 */
/* [local] */ 



enum SENSOR_LOCATION
    {
        T_SENSOR_FLATBED	= 0,
        T_SENSOR_ADF	= 1,
        T_SENSOR_XPA	= 2
    } ;

enum SENSOR_FEATURES
    {
        T_SENSOR_FEAT_LAMPTIMER	= 0x1,
        T_SENSOR_FEAT_LAMPINSTANTWARMUP	= 0x2,
        T_SENSOR_FEAT_RGBMATRIX	= 0x4
    } ;
typedef /* [public][public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0030_0001
    {
    WORD wSensorCount;
    BYTE bySensorLocation[ 16 ];
    WORD wSensorFeatures[ 16 ];
    WORD wReserved[ 20 ];
    } 	SENSOR_CAPABILITIES;

// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_sensor[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(SENSOR_CAPABILITIES) == 90);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0030_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0030_v0_0_s_ifspec;

#ifndef __ISensor_INTERFACE_DEFINED__
#define __ISensor_INTERFACE_DEFINED__

/* interface ISensor */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_ISensor;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("764C595E-CAEB-4001-9E1F-1130069A9EB9")
    ISensor : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSensorCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ SENSOR_CAPABILITIES *pSensorCapabilities,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSensor( 
            /* [in] */ DWORD hSession,
            /* [out] */ WORD *pwCurrentSensor,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetSensor( 
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wSensor,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ISensorVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISensor * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISensor * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISensor * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSensorCapabilities )( 
            ISensor * This,
            /* [in] */ DWORD hSession,
            /* [out] */ SENSOR_CAPABILITIES *pSensorCapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetSensor )( 
            ISensor * This,
            /* [in] */ DWORD hSession,
            /* [out] */ WORD *pwCurrentSensor,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetSensor )( 
            ISensor * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wSensor,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } ISensorVtbl;

    interface ISensor
    {
        CONST_VTBL struct ISensorVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISensor_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISensor_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISensor_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISensor_GetSensorCapabilities(This,hSession,pSensorCapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetSensorCapabilities(This,hSession,pSensorCapabilities,pdwErrorCode) ) 

#define ISensor_GetSensor(This,hSession,pwCurrentSensor,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetSensor(This,hSession,pwCurrentSensor,pdwErrorCode) ) 

#define ISensor_SetSensor(This,hSession,wSensor,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetSensor(This,hSession,wSensor,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISensor_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0031 */
/* [local] */ 



enum SPF_ERRORS
    {
        TERR_SPF_NOT_SUPPORTED	= 500
    } ;
struct SPF_CAPABILITIES
    {
    DWORD dwSPFsize;
    BYTE byEncrypted;
    DWORD dwReserved[ 10 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_spf[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct SPF_CAPABILITIES) == 48);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0031_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0031_v0_0_s_ifspec;

#ifndef __ISPF_INTERFACE_DEFINED__
#define __ISPF_INTERFACE_DEFINED__

/* interface ISPF */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_ISPF;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C07F621F-724E-4372-9265-A4E1FAC698C8")
    ISPF : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSPFCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ struct SPF_CAPABILITIES *pSPFcaps,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSPFData( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwSize,
            /* [size_is][out] */ BYTE *pbySPFdata,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ISPFVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISPF * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISPF * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISPF * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSPFCapabilities )( 
            ISPF * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct SPF_CAPABILITIES *pSPFcaps,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetSPFData )( 
            ISPF * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwSize,
            /* [size_is][out] */ BYTE *pbySPFdata,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } ISPFVtbl;

    interface ISPF
    {
        CONST_VTBL struct ISPFVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISPF_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISPF_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISPF_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISPF_GetSPFCapabilities(This,hSession,pSPFcaps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetSPFCapabilities(This,hSession,pSPFcaps,pdwErrorCode) ) 

#define ISPF_GetSPFData(This,hSession,dwSize,pbySPFdata,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetSPFData(This,hSession,dwSize,pbySPFdata,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISPF_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0032 */
/* [local] */ 



enum MFG_CONTROL
    {
        DEFAULT_ALL	= 0,
        LAB_ON	= 0x1,
        LHC_ON	= 0x2,
        CAL_DSNU_OFF	= 0x10,
        CAL_PRNU_OFF	= 0x20,
        CAL_ANALOGGAIN_OFF	= 0x40,
        CAL_PWM_OFF	= 0x80,
        CAL_FBLAMP_FOR_TMA_ON	= 0x100,
        IP_OFF	= 0x1000,
        DUPLEX_SKIP_1ST_SIDE	= 0x2000,
        SENSOR_POS_OFF	= 0x4000,
        PREGAMMA_OFF	= 0x8000,
        CONTINUOUS_LINE_ON	= 0x10000,
        SCALING_CORRECTION_OFF	= 0x20000,
        SHARPENING_OFF	= 0x40000,
        SCAN_EMULATE_COPY	= 0x80000
    } ;


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0032_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0032_v0_0_s_ifspec;

#ifndef __IMFG_INTERFACE_DEFINED__
#define __IMFG_INTERFACE_DEFINED__

/* interface IMFG */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IMFG;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("84D16247-2DE2-4EAA-A979-5A9E8C572014")
    IMFG : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE ReadContinuousScanLine( 
            /* [in] */ DWORD hSession,
            /* [size_is][out] */ BYTE *pbyBuffer,
            /* [in] */ DWORD dwBytes,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetMFGParameters( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwMfgOptions,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetMFGCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwMfgCaps,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IMFGVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IMFG * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IMFG * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IMFG * This);
        
        HRESULT ( STDMETHODCALLTYPE *ReadContinuousScanLine )( 
            IMFG * This,
            /* [in] */ DWORD hSession,
            /* [size_is][out] */ BYTE *pbyBuffer,
            /* [in] */ DWORD dwBytes,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetMFGParameters )( 
            IMFG * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwMfgOptions,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetMFGCapabilities )( 
            IMFG * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwMfgCaps,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IMFGVtbl;

    interface IMFG
    {
        CONST_VTBL struct IMFGVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IMFG_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IMFG_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IMFG_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IMFG_ReadContinuousScanLine(This,hSession,pbyBuffer,dwBytes,pdwErrorCode)	\
    ( (This)->lpVtbl -> ReadContinuousScanLine(This,hSession,pbyBuffer,dwBytes,pdwErrorCode) ) 

#define IMFG_SetMFGParameters(This,hSession,dwMfgOptions,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetMFGParameters(This,hSession,dwMfgOptions,pdwErrorCode) ) 

#define IMFG_GetMFGCapabilities(This,hSession,pdwMfgCaps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetMFGCapabilities(This,hSession,pdwMfgCaps,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IMFG_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0033 */
/* [local] */ 


typedef /* [public][public][public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0033_0001
    {
    WORD wXRes;
    WORD wYRes;
    BYTE byNumCCD;
    } 	DISCRETE_RESOLUTION;

// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_dr1[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(DISCRETE_RESOLUTION) == 6);
typedef /* [public][public][public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0033_0002
    {
    WORD wMinXRes;
    WORD wMaxXRes;
    WORD wMinYRes;
    WORD wMaxYRes;
    BYTE byNumCCD;
    BOOLEAN bXYSame;
    } 	CONTINUOUS_RESOLUTION;

// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_cr1[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(CONTINUOUS_RESOLUTION) == 10);
struct RESCCD_CAPABILITIES
    {
    WORD wDiscreteResCount;
    WORD wContinuousResCount;
    DISCRETE_RESOLUTION wDiscreteRes[ 30 ];
    CONTINUOUS_RESOLUTION wContinuousRes[ 30 ];
    DWORD dwXMagCorrectDiscrete;
    DWORD dwReserved[ 9 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_resccd[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct RESCCD_CAPABILITIES) == 524);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0033_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0033_v0_0_s_ifspec;

#ifndef __IResCCD_INTERFACE_DEFINED__
#define __IResCCD_INTERFACE_DEFINED__

/* interface IResCCD */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IResCCD;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("214523F8-82B0-45E1-BB0B-1448D56AD445")
    IResCCD : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetResCCDCapabilities( 
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wScanMethod,
            /* [out] */ struct RESCCD_CAPABILITIES *pResCCDcaps,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IResCCDVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IResCCD * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IResCCD * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IResCCD * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetResCCDCapabilities )( 
            IResCCD * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wScanMethod,
            /* [out] */ struct RESCCD_CAPABILITIES *pResCCDcaps,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IResCCDVtbl;

    interface IResCCD
    {
        CONST_VTBL struct IResCCDVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IResCCD_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IResCCD_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IResCCD_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IResCCD_GetResCCDCapabilities(This,hSession,wScanMethod,pResCCDcaps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetResCCDCapabilities(This,hSession,wScanMethod,pResCCDcaps,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IResCCD_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0034 */
/* [local] */ 


struct DISCRETE_RESOLUTION2
    {
    WORD wMinXRes;
    WORD wMaxXRes;
    WORD wMinYRes;
    WORD wMaxYRes;
    WORD wMultiplier;
    BYTE byNumCCD;
    BYTE byNativeRes;
    DWORD dwScanModesSupported;
    BYTE byBitsPerChannel[ 8 ];
    WORD wReserved[ 6 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_dr2[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct DISCRETE_RESOLUTION2) == 36);
struct CONTINUOUS_RESOLUTION2
    {
    WORD wMinXRes;
    WORD wMaxXRes;
    WORD wMinYRes;
    WORD wMaxYRes;
    BYTE byNumCCD;
    BYTE byXYSame;
    BYTE byNativeRes;
    DWORD dwScanModesSupported;
    BYTE byBitsPerChannel[ 8 ];
    WORD wReserved[ 6 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_cr2[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct CONTINUOUS_RESOLUTION2) == 36);
struct RES_CAPABILITIES
    {
    BOOL bUseLegacyValues;
    WORD wDiscreteResCount;
    WORD wContinuousResCount;
    WORD wScanMethod;
    DWORD dwXMagCorrectDiscrete;
    struct DISCRETE_RESOLUTION2 discreteResList[ 30 ];
    struct CONTINUOUS_RESOLUTION2 continuousResList[ 30 ];
    DWORD dwReserved[ 10 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_res[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct RES_CAPABILITIES) == 2216);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0034_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0034_v0_0_s_ifspec;

#ifndef __IResolution_INTERFACE_DEFINED__
#define __IResolution_INTERFACE_DEFINED__

/* interface IResolution */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IResolution;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("EADCA2A6-336C-48D1-B012-DFC1B52457AF")
    IResolution : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetResCapabilities( 
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wScanMethod,
            /* [out] */ struct RES_CAPABILITIES *pResCaps,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IResolutionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IResolution * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IResolution * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IResolution * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetResCapabilities )( 
            IResolution * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wScanMethod,
            /* [out] */ struct RES_CAPABILITIES *pResCaps,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IResolutionVtbl;

    interface IResolution
    {
        CONST_VTBL struct IResolutionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IResolution_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IResolution_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IResolution_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IResolution_GetResCapabilities(This,hSession,wScanMethod,pResCaps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetResCapabilities(This,hSession,wScanMethod,pResCaps,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IResolution_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0035 */
/* [local] */ 


typedef /* [public][public][public][public][public] */ 
enum __MIDL___MIDL_itf_hpgtblues_0000_0035_0001
    {
        T_EVENT_SW_CANCEL	= 0x1,
        T_EVENT_PERCENT_COMPLETE	= 0x2,
        T_EVENT_SCAN	= 0x4,
        T_EVENT_SCAN_S3L	= 0x8,
        T_EVENT_SCAN_IR	= 0x10,
        T_EVENT_CALIBRATION	= 0x20,
        T_EVENT_LAMP_WARMUP	= 0x40,
        T_EVENT_SW_PAUSE	= 0x80
    } 	TULIP_EVENT;

typedef /* [public][public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0035_0002
    {
    DWORD dwAppEventsSupported;
    DWORD dwDriverEventsSupported;
    DWORD dwReserved[ 10 ];
    } 	EVENT_CAPABILITIES;

// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_event[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(EVENT_CAPABILITIES) == 48);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0035_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0035_v0_0_s_ifspec;

#ifndef __IEventCallback_INTERFACE_DEFINED__
#define __IEventCallback_INTERFACE_DEFINED__

/* interface IEventCallback */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IEventCallback;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("32C839B1-F56A-4EE9-BA13-0A52043DA07E")
    IEventCallback : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EventCallback( 
            /* [in] */ DWORD hSession,
            /* [in] */ TULIP_EVENT event,
            /* [in] */ long lEventData,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IEventCallbackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEventCallback * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEventCallback * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEventCallback * This);
        
        HRESULT ( STDMETHODCALLTYPE *EventCallback )( 
            IEventCallback * This,
            /* [in] */ DWORD hSession,
            /* [in] */ TULIP_EVENT event,
            /* [in] */ long lEventData,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IEventCallbackVtbl;

    interface IEventCallback
    {
        CONST_VTBL struct IEventCallbackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEventCallback_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEventCallback_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEventCallback_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEventCallback_EventCallback(This,hSession,event,lEventData,pdwErrorCode)	\
    ( (This)->lpVtbl -> EventCallback(This,hSession,event,lEventData,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEventCallback_INTERFACE_DEFINED__ */


#ifndef __IEventNotify_INTERFACE_DEFINED__
#define __IEventNotify_INTERFACE_DEFINED__

/* interface IEventNotify */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IEventNotify;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5A71BAA9-220B-4AAF-9CBB-D3B9CD5AF1D7")
    IEventNotify : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetEventCapabilities( 
            /* [in] */ DWORD dwSession,
            /* [out] */ EVENT_CAPABILITIES *pEventCaps,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SendAppEvent( 
            /* [in] */ DWORD dwSession,
            /* [in] */ TULIP_EVENT event,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SubscribeDriverEvents( 
            /* [in] */ DWORD dwSession,
            /* [in] */ IEventCallback *pIEvent,
            /* [in] */ TULIP_EVENT events,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnsubscribeDriverEvents( 
            /* [in] */ DWORD dwSession,
            /* [in] */ TULIP_EVENT events,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IEventNotifyVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEventNotify * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEventNotify * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEventNotify * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetEventCapabilities )( 
            IEventNotify * This,
            /* [in] */ DWORD dwSession,
            /* [out] */ EVENT_CAPABILITIES *pEventCaps,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SendAppEvent )( 
            IEventNotify * This,
            /* [in] */ DWORD dwSession,
            /* [in] */ TULIP_EVENT event,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SubscribeDriverEvents )( 
            IEventNotify * This,
            /* [in] */ DWORD dwSession,
            /* [in] */ IEventCallback *pIEvent,
            /* [in] */ TULIP_EVENT events,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *UnsubscribeDriverEvents )( 
            IEventNotify * This,
            /* [in] */ DWORD dwSession,
            /* [in] */ TULIP_EVENT events,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IEventNotifyVtbl;

    interface IEventNotify
    {
        CONST_VTBL struct IEventNotifyVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEventNotify_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEventNotify_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEventNotify_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEventNotify_GetEventCapabilities(This,dwSession,pEventCaps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetEventCapabilities(This,dwSession,pEventCaps,pdwErrorCode) ) 

#define IEventNotify_SendAppEvent(This,dwSession,event,pdwErrorCode)	\
    ( (This)->lpVtbl -> SendAppEvent(This,dwSession,event,pdwErrorCode) ) 

#define IEventNotify_SubscribeDriverEvents(This,dwSession,pIEvent,events,pdwErrorCode)	\
    ( (This)->lpVtbl -> SubscribeDriverEvents(This,dwSession,pIEvent,events,pdwErrorCode) ) 

#define IEventNotify_UnsubscribeDriverEvents(This,dwSession,events,pdwErrorCode)	\
    ( (This)->lpVtbl -> UnsubscribeDriverEvents(This,dwSession,events,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEventNotify_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0037 */
/* [local] */ 


typedef /* [public] */ 
enum __MIDL___MIDL_itf_hpgtblues_0000_0037_0001
    {
        T_DROPOUT_CORRECTED	= 0,
        T_DROPOUT_RAW_MONO	= 1,
        T_DROPOUT_TRUECOLOR	= 2
    } 	DROPOUT_ALGORITHM;

typedef /* [public][public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0037_0002
    {
    BYTE byHasColorDropout;
    WORD wMaxDropoutColorsSupported;
    WORD wMaxDropoutLevel;
    WORD wRecommendedDropoutLevel;
    DWORD dwLUTentriesPerChannel;
    DWORD dwLUTmaxEntryValue;
    DWORD dwLUTbytesPerEntry;
    WORD wDropoutAlgorithm;
    WORD wReserved[ 8 ];
    } 	COLORDROPOUT_CAPABILITIES;

typedef /* [public][public][public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0037_0003
    {
    BYTE byRed;
    BYTE byGreen;
    BYTE byBlue;
    } 	TULIP_RGB;



extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0037_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0037_v0_0_s_ifspec;

#ifndef __IColorDropout_INTERFACE_DEFINED__
#define __IColorDropout_INTERFACE_DEFINED__

/* interface IColorDropout */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IColorDropout;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("73C93BB4-2F62-4DB2-AFA3-C5A74584BEB4")
    IColorDropout : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetColorDropoutCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ COLORDROPOUT_CAPABILITIES *pDropCaps,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ColorDropout( 
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wNumColors,
            /* [size_is][in] */ WORD wDropoutLevel[  ],
            /* [size_is][in] */ TULIP_RGB colors[  ],
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IColorDropoutVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IColorDropout * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IColorDropout * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IColorDropout * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetColorDropoutCapabilities )( 
            IColorDropout * This,
            /* [in] */ DWORD hSession,
            /* [out] */ COLORDROPOUT_CAPABILITIES *pDropCaps,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ColorDropout )( 
            IColorDropout * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wNumColors,
            /* [size_is][in] */ WORD wDropoutLevel[  ],
            /* [size_is][in] */ TULIP_RGB colors[  ],
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IColorDropoutVtbl;

    interface IColorDropout
    {
        CONST_VTBL struct IColorDropoutVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IColorDropout_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IColorDropout_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IColorDropout_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IColorDropout_GetColorDropoutCapabilities(This,hSession,pDropCaps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetColorDropoutCapabilities(This,hSession,pDropCaps,pdwErrorCode) ) 

#define IColorDropout_ColorDropout(This,hSession,wNumColors,wDropoutLevel,colors,pdwErrorCode)	\
    ( (This)->lpVtbl -> ColorDropout(This,hSession,wNumColors,wDropoutLevel,colors,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IColorDropout_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0038 */
/* [local] */ 

// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_drop[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(COLORDROPOUT_CAPABILITIES) == 40);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0038_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0038_v0_0_s_ifspec;

#ifndef __IColorDropout2_INTERFACE_DEFINED__
#define __IColorDropout2_INTERFACE_DEFINED__

/* interface IColorDropout2 */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IColorDropout2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A16D2B37-B1C4-49df-A15E-DA91E6B06D94")
    IColorDropout2 : public IColorDropout
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetDropoutLUT( 
            /* [in] */ DWORD hSession,
            /* [in] */ ULONG uSz,
            /* [size_is][in] */ BYTE *pbyRed,
            /* [size_is][in] */ BYTE *pbyGreen,
            /* [size_is][in] */ BYTE *pbyBlue,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ColorDropoutRaw( 
            /* [in] */ DWORD hSession,
            /* [in] */ ULONG uSz,
            /* [size_is][in] */ BYTE *pbyRed,
            /* [size_is][in] */ BYTE *pbyGreen,
            /* [size_is][in] */ BYTE *pbyBlue,
            /* [in] */ WORD wNumColors,
            /* [size_is][in] */ WORD wDropoutLevel[  ],
            /* [size_is][in] */ TULIP_RGB colors[  ],
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IColorDropout2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IColorDropout2 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IColorDropout2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IColorDropout2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetColorDropoutCapabilities )( 
            IColorDropout2 * This,
            /* [in] */ DWORD hSession,
            /* [out] */ COLORDROPOUT_CAPABILITIES *pDropCaps,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ColorDropout )( 
            IColorDropout2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ WORD wNumColors,
            /* [size_is][in] */ WORD wDropoutLevel[  ],
            /* [size_is][in] */ TULIP_RGB colors[  ],
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetDropoutLUT )( 
            IColorDropout2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ ULONG uSz,
            /* [size_is][in] */ BYTE *pbyRed,
            /* [size_is][in] */ BYTE *pbyGreen,
            /* [size_is][in] */ BYTE *pbyBlue,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ColorDropoutRaw )( 
            IColorDropout2 * This,
            /* [in] */ DWORD hSession,
            /* [in] */ ULONG uSz,
            /* [size_is][in] */ BYTE *pbyRed,
            /* [size_is][in] */ BYTE *pbyGreen,
            /* [size_is][in] */ BYTE *pbyBlue,
            /* [in] */ WORD wNumColors,
            /* [size_is][in] */ WORD wDropoutLevel[  ],
            /* [size_is][in] */ TULIP_RGB colors[  ],
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IColorDropout2Vtbl;

    interface IColorDropout2
    {
        CONST_VTBL struct IColorDropout2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IColorDropout2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IColorDropout2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IColorDropout2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IColorDropout2_GetColorDropoutCapabilities(This,hSession,pDropCaps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetColorDropoutCapabilities(This,hSession,pDropCaps,pdwErrorCode) ) 

#define IColorDropout2_ColorDropout(This,hSession,wNumColors,wDropoutLevel,colors,pdwErrorCode)	\
    ( (This)->lpVtbl -> ColorDropout(This,hSession,wNumColors,wDropoutLevel,colors,pdwErrorCode) ) 


#define IColorDropout2_SetDropoutLUT(This,hSession,uSz,pbyRed,pbyGreen,pbyBlue,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetDropoutLUT(This,hSession,uSz,pbyRed,pbyGreen,pbyBlue,pdwErrorCode) ) 

#define IColorDropout2_ColorDropoutRaw(This,hSession,uSz,pbyRed,pbyGreen,pbyBlue,wNumColors,wDropoutLevel,colors,pdwErrorCode)	\
    ( (This)->lpVtbl -> ColorDropoutRaw(This,hSession,uSz,pbyRed,pbyGreen,pbyBlue,wNumColors,wDropoutLevel,colors,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IColorDropout2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0039 */
/* [local] */ 


typedef /* [public] */ 
enum __MIDL___MIDL_itf_hpgtblues_0000_0039_0001
    {
        TERR_IMPRINTER_NO_PAPER	= 700,
        TERR_IMPRINTER_NO_CARTRIDGE	= 701
    } 	IMPRINTER_ERRORS;

typedef /* [public] */ 
enum __MIDL___MIDL_itf_hpgtblues_0000_0039_0002
    {
        IMPRINT_STRING_TOP_TO_BOTTOM	= 1,
        IMPRINT_STRING_BOTTOM_TO_TOP	= 2
    } 	IMPRINTER_STRING_ORIENT;

typedef /* [public][public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0039_0003
    {
    BYTE byHasCleaningMode;
    BYTE byHasInkIndicator;
    BYTE byCanDetectCartridge;
    BYTE byCanPrintVerticalChar;
    DWORD dwBitmapStringMaxSize;
    WORD wMaxPrintheadWidth;
    WORD wPrintResolution;
    WORD wMaxPageCountDigits;
    WORD wMinPageCountDigits;
    WORD wStringOrientOptions;
    WORD wDefaultStringOrient;
    WORD wDefaultPageCountDigits;
    WORD wDefaultPrintYOffset;
    WORD wReserved[ 20 ];
    } 	IMPRINTER_CAPABILITIES;

// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_imprint_cap[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(IMPRINTER_CAPABILITIES) == 64);
typedef /* [public][public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0039_0004
    {
    WORD wYOffset;
    WORD wPrintStringOrientation;
    DWORD dwPageCountStart;
    short pageCountDirectionStep;
    WORD wPageCountDigits;
    BOOL bPageCountBold;
    BOOL bPrintVerticalChar;
    WORD wReserved[ 20 ];
    } 	IMPRINTER_PARAMETERS;

// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_imprint_parms[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(IMPRINTER_PARAMETERS) == 60);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0039_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0039_v0_0_s_ifspec;

#ifndef __iImprinter_INTERFACE_DEFINED__
#define __iImprinter_INTERFACE_DEFINED__

/* interface iImprinter */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_iImprinter;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3198F15C-234B-43E0-86C2-94F0BFF496BC")
    iImprinter : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetImprinterCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ IMPRINTER_CAPABILITIES *pImprintCaps,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnableImprinter( 
            /* [in] */ DWORD hSession,
            /* [in] */ BOOL byEnable,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetImprinterParameters( 
            /* [in] */ DWORD hSession,
            /* [in] */ IMPRINTER_PARAMETERS *pImprintParams,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetImprinterString( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwSize,
            /* [size_is][in] */ BYTE *pbyString,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ImprinterClean( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pbyErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetImprinterInkLevel( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwInkLevel,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ImprinterCartridgeReset( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pbyErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct iImprinterVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            iImprinter * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            iImprinter * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            iImprinter * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetImprinterCapabilities )( 
            iImprinter * This,
            /* [in] */ DWORD hSession,
            /* [out] */ IMPRINTER_CAPABILITIES *pImprintCaps,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *EnableImprinter )( 
            iImprinter * This,
            /* [in] */ DWORD hSession,
            /* [in] */ BOOL byEnable,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetImprinterParameters )( 
            iImprinter * This,
            /* [in] */ DWORD hSession,
            /* [in] */ IMPRINTER_PARAMETERS *pImprintParams,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetImprinterString )( 
            iImprinter * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwSize,
            /* [size_is][in] */ BYTE *pbyString,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ImprinterClean )( 
            iImprinter * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pbyErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetImprinterInkLevel )( 
            iImprinter * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwInkLevel,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *ImprinterCartridgeReset )( 
            iImprinter * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pbyErrorCode);
        
        END_INTERFACE
    } iImprinterVtbl;

    interface iImprinter
    {
        CONST_VTBL struct iImprinterVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define iImprinter_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define iImprinter_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define iImprinter_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define iImprinter_GetImprinterCapabilities(This,hSession,pImprintCaps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetImprinterCapabilities(This,hSession,pImprintCaps,pdwErrorCode) ) 

#define iImprinter_EnableImprinter(This,hSession,byEnable,pdwErrorCode)	\
    ( (This)->lpVtbl -> EnableImprinter(This,hSession,byEnable,pdwErrorCode) ) 

#define iImprinter_SetImprinterParameters(This,hSession,pImprintParams,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetImprinterParameters(This,hSession,pImprintParams,pdwErrorCode) ) 

#define iImprinter_SetImprinterString(This,hSession,dwSize,pbyString,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetImprinterString(This,hSession,dwSize,pbyString,pdwErrorCode) ) 

#define iImprinter_ImprinterClean(This,hSession,pbyErrorCode)	\
    ( (This)->lpVtbl -> ImprinterClean(This,hSession,pbyErrorCode) ) 

#define iImprinter_GetImprinterInkLevel(This,hSession,pdwInkLevel,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetImprinterInkLevel(This,hSession,pdwInkLevel,pdwErrorCode) ) 

#define iImprinter_ImprinterCartridgeReset(This,hSession,pbyErrorCode)	\
    ( (This)->lpVtbl -> ImprinterCartridgeReset(This,hSession,pbyErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __iImprinter_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0040 */
/* [local] */ 


struct EXPOSURE_CAPABILITIES
    {
    BYTE byCanAdjustExposureFactor;
    DWORD dwExposureFactorScanMethods;
    BYTE byCanAdjustExposureRatios;
    DWORD dwExposureRatioScanMethods;
    DWORD dwMinExposureFactor;
    DWORD dwMaxExposureFactor;
    DWORD dwMinExposureRatio;
    DWORD dwMaxExposureRatio;
    DWORD dwReserved[ 20 ];
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_exposure[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(struct EXPOSURE_CAPABILITIES) == 112);


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0040_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0040_v0_0_s_ifspec;

#ifndef __IExposure_INTERFACE_DEFINED__
#define __IExposure_INTERFACE_DEFINED__

/* interface IExposure */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IExposure;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A412C823-9B58-4A61-A766-09FD29B863E4")
    IExposure : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetExposureCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ struct EXPOSURE_CAPABILITIES *pExpCaps,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetExposureFactor( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwExposureFactor,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetExposureRatios( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwRedRatio,
            /* [in] */ DWORD dwGreenRation,
            /* [in] */ DWORD dwBlueRatio,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IExposureVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IExposure * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IExposure * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IExposure * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetExposureCapabilities )( 
            IExposure * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct EXPOSURE_CAPABILITIES *pExpCaps,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetExposureFactor )( 
            IExposure * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwExposureFactor,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetExposureRatios )( 
            IExposure * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwRedRatio,
            /* [in] */ DWORD dwGreenRation,
            /* [in] */ DWORD dwBlueRatio,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IExposureVtbl;

    interface IExposure
    {
        CONST_VTBL struct IExposureVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IExposure_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IExposure_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IExposure_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IExposure_GetExposureCapabilities(This,hSession,pExpCaps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetExposureCapabilities(This,hSession,pExpCaps,pdwErrorCode) ) 

#define IExposure_SetExposureFactor(This,hSession,dwExposureFactor,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetExposureFactor(This,hSession,dwExposureFactor,pdwErrorCode) ) 

#define IExposure_SetExposureRatios(This,hSession,dwRedRatio,dwGreenRation,dwBlueRatio,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetExposureRatios(This,hSession,dwRedRatio,dwGreenRation,dwBlueRatio,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IExposure_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0041 */
/* [local] */ 



enum FW_UPDATE_ERRORS
    {
        TERR_FW_INVALID_DATA	= 900,
        TERR_FW_GENERAL_FAILURE	= 901,
        TERR_FW_ALREADY_NEWER	= 902,
        TERR_FW_INVALID_AREA	= 903,
        TERR_FW_DEVICE_NOT_READY	= 904
    } ;

enum FW_UPDATE_AREAS
    {
        T_FW_GENERAL	= 0x1,
        T_FW_BOOT	= 0x2,
        T_FW_PROGRAM	= 0x4,
        T_FW_CONTROL_PANEL	= 0x8,
        T_FW_ADF	= 0x10,
        T_FW_DEFAULT_TONEMAP	= 0x20,
        T_FW_FPGA	= 0x40
    } ;

enum FW_UPDATE_MODE
    {
        T_FW_UPDATE_ALWAYS	= 0,
        T_FW_UPDATE_IF_NEWER	= 1
    } ;
struct FW_UPDATE_CAPABILITIES
    {
    DWORD dwSupportedFWUpdateAreas;
    DWORD dwReserved[ 20 ];
    } ;


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0041_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0041_v0_0_s_ifspec;

#ifndef __IFWUpdate_INTERFACE_DEFINED__
#define __IFWUpdate_INTERFACE_DEFINED__

/* interface IFWUpdate */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IFWUpdate;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("69692383-909D-4116-B457-F8A490E34B36")
    IFWUpdate : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetFWUpdateCapabilities( 
            /* [in] */ DWORD hSession,
            /* [out] */ struct FW_UPDATE_CAPABILITIES *pCapabilities,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateFW( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwFWArea,
            /* [in] */ BYTE byUpdateMode,
            /* [in] */ DWORD dwFWSize,
            /* [size_is][in] */ BYTE *pbyFW,
            /* [out] */ BYTE *pbyDeviceResetNeeded,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetFWVersion( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwFWArea,
            /* [out] */ LPOLESTR *ppszFWVersion,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IFWUpdateVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IFWUpdate * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IFWUpdate * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IFWUpdate * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetFWUpdateCapabilities )( 
            IFWUpdate * This,
            /* [in] */ DWORD hSession,
            /* [out] */ struct FW_UPDATE_CAPABILITIES *pCapabilities,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateFW )( 
            IFWUpdate * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwFWArea,
            /* [in] */ BYTE byUpdateMode,
            /* [in] */ DWORD dwFWSize,
            /* [size_is][in] */ BYTE *pbyFW,
            /* [out] */ BYTE *pbyDeviceResetNeeded,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetFWVersion )( 
            IFWUpdate * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD dwFWArea,
            /* [out] */ LPOLESTR *ppszFWVersion,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IFWUpdateVtbl;

    interface IFWUpdate
    {
        CONST_VTBL struct IFWUpdateVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IFWUpdate_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IFWUpdate_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IFWUpdate_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IFWUpdate_GetFWUpdateCapabilities(This,hSession,pCapabilities,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetFWUpdateCapabilities(This,hSession,pCapabilities,pdwErrorCode) ) 

#define IFWUpdate_UpdateFW(This,hSession,dwFWArea,byUpdateMode,dwFWSize,pbyFW,pbyDeviceResetNeeded,pdwErrorCode)	\
    ( (This)->lpVtbl -> UpdateFW(This,hSession,dwFWArea,byUpdateMode,dwFWSize,pbyFW,pbyDeviceResetNeeded,pdwErrorCode) ) 

#define IFWUpdate_GetFWVersion(This,hSession,dwFWArea,ppszFWVersion,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetFWVersion(This,hSession,dwFWArea,ppszFWVersion,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IFWUpdate_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0042 */
/* [local] */ 


typedef /* [public] */ 
enum __MIDL___MIDL_itf_hpgtblues_0000_0042_0001
    {
        T_PM_SLEEP	= 0x1,
        T_PM_AUTO_OFF	= 0x2
    } 	PM_FEATURE;

typedef /* [public][public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0042_0002
    {
    BYTE byCanDisable;
    WORD wMinTimeMinutes;
    WORD wMaxTimeMinutes;
    DWORD dwReserved[ 10 ];
    } 	PM_CAPABILITIES;



extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0042_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0042_v0_0_s_ifspec;

#ifndef __IPowerManagement_INTERFACE_DEFINED__
#define __IPowerManagement_INTERFACE_DEFINED__

/* interface IPowerManagement */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IPowerManagement;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9CB47BBF-467E-4e2c-99CB-7CBBF380706D")
    IPowerManagement : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetSupportedPMFeatures( 
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwSupportedFeatures,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPMCapabilities( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD ePMFeature,
            /* [out] */ PM_CAPABILITIES *pPMCaps,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DisablePMFeature( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD ePMFeature,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPMFeatureTime( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD ePMFeature,
            /* [out] */ WORD *pwTime,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetPMFeatureTime( 
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD ePMFeature,
            /* [in] */ WORD wTime,
            /* [out] */ DWORD *pdwErrorCode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IPowerManagementVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IPowerManagement * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IPowerManagement * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IPowerManagement * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetSupportedPMFeatures )( 
            IPowerManagement * This,
            /* [in] */ DWORD hSession,
            /* [out] */ DWORD *pdwSupportedFeatures,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetPMCapabilities )( 
            IPowerManagement * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD ePMFeature,
            /* [out] */ PM_CAPABILITIES *pPMCaps,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *DisablePMFeature )( 
            IPowerManagement * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD ePMFeature,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetPMFeatureTime )( 
            IPowerManagement * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD ePMFeature,
            /* [out] */ WORD *pwTime,
            /* [out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *SetPMFeatureTime )( 
            IPowerManagement * This,
            /* [in] */ DWORD hSession,
            /* [in] */ DWORD ePMFeature,
            /* [in] */ WORD wTime,
            /* [out] */ DWORD *pdwErrorCode);
        
        END_INTERFACE
    } IPowerManagementVtbl;

    interface IPowerManagement
    {
        CONST_VTBL struct IPowerManagementVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IPowerManagement_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IPowerManagement_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IPowerManagement_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IPowerManagement_GetSupportedPMFeatures(This,hSession,pdwSupportedFeatures,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetSupportedPMFeatures(This,hSession,pdwSupportedFeatures,pdwErrorCode) ) 

#define IPowerManagement_GetPMCapabilities(This,hSession,ePMFeature,pPMCaps,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetPMCapabilities(This,hSession,ePMFeature,pPMCaps,pdwErrorCode) ) 

#define IPowerManagement_DisablePMFeature(This,hSession,ePMFeature,pdwErrorCode)	\
    ( (This)->lpVtbl -> DisablePMFeature(This,hSession,ePMFeature,pdwErrorCode) ) 

#define IPowerManagement_GetPMFeatureTime(This,hSession,ePMFeature,pwTime,pdwErrorCode)	\
    ( (This)->lpVtbl -> GetPMFeatureTime(This,hSession,ePMFeature,pwTime,pdwErrorCode) ) 

#define IPowerManagement_SetPMFeatureTime(This,hSession,ePMFeature,wTime,pdwErrorCode)	\
    ( (This)->lpVtbl -> SetPMFeatureTime(This,hSession,ePMFeature,wTime,pdwErrorCode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IPowerManagement_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0043 */
/* [local] */ 


#ifndef _IHPSCNMGR_TYPES_DEFINED
#define _IHPSCNMGR_TYPES_DEFINED
static const int HPGSCNSV_E_NO_DEVICES				= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  1 );
static const int HPGSCNSV_E_DEV_NOT_PRESENT			= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  2 );
static const int HPGSCNSV_E_DEV_CCI_TULIP			= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  3 );
static const int HPGSCNSV_E_DEV_TULIP_GETMODEL		= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  4 );
static const int HPGSCNSV_E_REGOPEN_IMAGECLASS		= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  5 );
static const int HPGSCNSV_E_REGOPEN_DEVINST			= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  6 );
static const int HPGSCNSV_E_REGOPEN_DEVDATA			= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  7 );
static const int HPGSCNSV_E_REGDATA_INVALID			= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  8 );
static const int HPGSCNSV_E_CATMGR					= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  9 );
static const int HPGSCNSV_E_OS_INCORRECT			= MAKE_HRESULT( SEVERITY_ERROR, FACILITY_ITF,  10 );
static const int HPGSCNSV_S_REGDATA_INVALID			= MAKE_HRESULT( 0, FACILITY_ITF, 1);
typedef /* [public][public][public][public][public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0043_0001
    {
    WCHAR wszFriendlyName[ 256 ];
    WCHAR wszDevName[ 256 ];
    WORD wIOType;
    CLSID clsidTulip;
    WCHAR wszModelNumber[ 16 ];
    } 	SCAN_DEV_INFO;

typedef struct __MIDL___MIDL_itf_hpgtblues_0000_0043_0001 *PSCAN_DEV_INFO;

// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_sdi[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(SCAN_DEV_INFO) == 1076);
typedef /* [public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0043_0002
    {
    WCHAR wszFriendlyName[ 256 ];
    WCHAR wszDevName[ 256 ];
    WORD wIOType;
    CLSID clsidTulip;
    WCHAR wszModelNumber[ 16 ];
    WCHAR wszDevIndex[ 5 ];
    } 	SCAN_DEV_INFO_EX;

typedef struct __MIDL___MIDL_itf_hpgtblues_0000_0043_0002 *PSCAN_DEV_INFO_EX;

// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_sdiex[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(SCAN_DEV_INFO_EX) == 1088);
typedef /* [public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0043_0003
    {
    WCHAR wszFriendlyName[ 256 ];
    WCHAR wszDevName[ 256 ];
    WORD wIOType;
    CLSID clsidTulip;
    WCHAR wszModelNumber[ 16 ];
    WCHAR wszDevIndex[ 5 ];
    WCHAR wszCueContextID[ 256 ];
    } 	SCAN_DEV_INFO_EX2;

typedef struct __MIDL___MIDL_itf_hpgtblues_0000_0043_0003 *PSCAN_DEV_INFO_EX2;

// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_sdiex2[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(SCAN_DEV_INFO_EX2) == 1600);

enum DLG_TYPE
    {
        DLG_DEFAULT	= 0,
        DLG_ALWAYS	= ( DLG_DEFAULT + 1 ) 
    } ;
typedef /* [public][public] */ struct __MIDL___MIDL_itf_hpgtblues_0000_0043_0004
    {
    WCHAR wszFriendlyName[ 256 ];
    WCHAR wszDevName[ 256 ];
    WORD wIOType;
    CLSID clsidTulip;
    WCHAR wszModelNumber[ 16 ];
    WCHAR wszDevIndex[ 5 ];
    CLSID clsidRoundUp;
    } 	HPRU_DEV_INFO;


enum SCANNER_DEV_NAME_TYPE
    {
        T_CREATE_FILE_NAME	= 1,
        T_NETWORK_DEVICE_ID	= 2,
        T_DEVICE_INDEX	= 3
    } ;
// this checks the static size of the structure to ensure it hasn't changed, and maintains COM immutability.
// if the compiler gives an array of size 0 error, then most likely the structure was improperly modified.
#undef compile_time_struct_check
#define compile_time_struct_check(cond) typedef char _check_adf[(cond) ? 1 : 0]
compile_time_struct_check(sizeof(HPRU_DEV_INFO) == 1104);
#endif  //_IHPSCNMGR_TYPES_DEFINED


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0043_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0043_v0_0_s_ifspec;

#ifndef __IHPScnMgr_INTERFACE_DEFINED__
#define __IHPScnMgr_INTERFACE_DEFINED__

/* interface IHPScnMgr */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IHPScnMgr;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7EA49EAF-5188-440B-B671-9F05CA436341")
    IHPScnMgr : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCurrentScanner( 
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCurrentScanner( 
            /* [in] */ SCAN_DEV_INFO *pScanDevInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DlgGetScanner( 
            /* [in] */ WORD dlgType,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetScannerByFriendlyName( 
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IHPScnMgrVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IHPScnMgr * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IHPScnMgr * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IHPScnMgr * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScanner )( 
            IHPScnMgr * This,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetCurrentScanner )( 
            IHPScnMgr * This,
            /* [in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScanner )( 
            IHPScnMgr * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByFriendlyName )( 
            IHPScnMgr * This,
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        END_INTERFACE
    } IHPScnMgrVtbl;

    interface IHPScnMgr
    {
        CONST_VTBL struct IHPScnMgrVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IHPScnMgr_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IHPScnMgr_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IHPScnMgr_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IHPScnMgr_GetCurrentScanner(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetCurrentScanner(This,pScanDevInfo) ) 

#define IHPScnMgr_SetCurrentScanner(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> SetCurrentScanner(This,pScanDevInfo) ) 

#define IHPScnMgr_DlgGetScanner(This,dlgType,pScanDevInfo)	\
    ( (This)->lpVtbl -> DlgGetScanner(This,dlgType,pScanDevInfo) ) 

#define IHPScnMgr_GetScannerByFriendlyName(This,pszFriendlyName,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByFriendlyName(This,pszFriendlyName,pScanDevInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IHPScnMgr_INTERFACE_DEFINED__ */


#ifndef __IHPScnMgr2_INTERFACE_DEFINED__
#define __IHPScnMgr2_INTERFACE_DEFINED__

/* interface IHPScnMgr2 */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IHPScnMgr2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9C6F8AE6-3F25-4EE4-8715-0BD04D3BFB35")
    IHPScnMgr2 : public IHPScnMgr
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetCurrentScanner2( 
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetCurrentScanner2( 
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DlgGetScanner2( 
            /* [in] */ WORD dlgType,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetScannerByFriendlyName2( 
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetScannerByIndex( 
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetCurrentScannerInterface( 
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetScannerInterfaceByIndex( 
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DlgGetScannerInterface( 
            /* [in] */ WORD dlgType,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetScannerInterfaceByFriendlyName( 
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IHPScnMgr2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IHPScnMgr2 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IHPScnMgr2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IHPScnMgr2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScanner )( 
            IHPScnMgr2 * This,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetCurrentScanner )( 
            IHPScnMgr2 * This,
            /* [in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScanner )( 
            IHPScnMgr2 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByFriendlyName )( 
            IHPScnMgr2 * This,
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScanner2 )( 
            IHPScnMgr2 * This,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetCurrentScanner2 )( 
            IHPScnMgr2 * This,
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScanner2 )( 
            IHPScnMgr2 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByFriendlyName2 )( 
            IHPScnMgr2 * This,
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByIndex )( 
            IHPScnMgr2 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScannerInterface )( 
            IHPScnMgr2 * This,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerInterfaceByIndex )( 
            IHPScnMgr2 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScannerInterface )( 
            IHPScnMgr2 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerInterfaceByFriendlyName )( 
            IHPScnMgr2 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        END_INTERFACE
    } IHPScnMgr2Vtbl;

    interface IHPScnMgr2
    {
        CONST_VTBL struct IHPScnMgr2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IHPScnMgr2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IHPScnMgr2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IHPScnMgr2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IHPScnMgr2_GetCurrentScanner(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetCurrentScanner(This,pScanDevInfo) ) 

#define IHPScnMgr2_SetCurrentScanner(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> SetCurrentScanner(This,pScanDevInfo) ) 

#define IHPScnMgr2_DlgGetScanner(This,dlgType,pScanDevInfo)	\
    ( (This)->lpVtbl -> DlgGetScanner(This,dlgType,pScanDevInfo) ) 

#define IHPScnMgr2_GetScannerByFriendlyName(This,pszFriendlyName,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByFriendlyName(This,pszFriendlyName,pScanDevInfo) ) 


#define IHPScnMgr2_GetCurrentScanner2(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetCurrentScanner2(This,pScanDevInfo) ) 

#define IHPScnMgr2_SetCurrentScanner2(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> SetCurrentScanner2(This,pScanDevInfo) ) 

#define IHPScnMgr2_DlgGetScanner2(This,dlgType,pScanDevInfo)	\
    ( (This)->lpVtbl -> DlgGetScanner2(This,dlgType,pScanDevInfo) ) 

#define IHPScnMgr2_GetScannerByFriendlyName2(This,pszFriendlyName,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByFriendlyName2(This,pszFriendlyName,pScanDevInfo) ) 

#define IHPScnMgr2_GetScannerByIndex(This,pszIndex,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByIndex(This,pszIndex,pScanDevInfo) ) 

#define IHPScnMgr2_GetCurrentScannerInterface(This,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetCurrentScannerInterface(This,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr2_GetScannerInterfaceByIndex(This,pszIndex,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetScannerInterfaceByIndex(This,pszIndex,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr2_DlgGetScannerInterface(This,dlgType,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> DlgGetScannerInterface(This,dlgType,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr2_GetScannerInterfaceByFriendlyName(This,pszIndex,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetScannerInterfaceByFriendlyName(This,pszIndex,pScanDevInfoEx,ppScanner) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IHPScnMgr2_INTERFACE_DEFINED__ */


#ifndef __IHPScnMgr3_INTERFACE_DEFINED__
#define __IHPScnMgr3_INTERFACE_DEFINED__

/* interface IHPScnMgr3 */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IHPScnMgr3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("082E6A1A-819E-49F1-A1E7-E64CEE9AA181")
    IHPScnMgr3 : public IHPScnMgr2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnumerateScanners( 
            /* [out] */ WORD *pwCount,
            /* [size_is][size_is][out] */ PSCAN_DEV_INFO_EX *ppScanDevInfo) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IHPScnMgr3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IHPScnMgr3 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IHPScnMgr3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IHPScnMgr3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScanner )( 
            IHPScnMgr3 * This,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetCurrentScanner )( 
            IHPScnMgr3 * This,
            /* [in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScanner )( 
            IHPScnMgr3 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByFriendlyName )( 
            IHPScnMgr3 * This,
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScanner2 )( 
            IHPScnMgr3 * This,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetCurrentScanner2 )( 
            IHPScnMgr3 * This,
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScanner2 )( 
            IHPScnMgr3 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByFriendlyName2 )( 
            IHPScnMgr3 * This,
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByIndex )( 
            IHPScnMgr3 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScannerInterface )( 
            IHPScnMgr3 * This,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerInterfaceByIndex )( 
            IHPScnMgr3 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScannerInterface )( 
            IHPScnMgr3 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerInterfaceByFriendlyName )( 
            IHPScnMgr3 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *EnumerateScanners )( 
            IHPScnMgr3 * This,
            /* [out] */ WORD *pwCount,
            /* [size_is][size_is][out] */ PSCAN_DEV_INFO_EX *ppScanDevInfo);
        
        END_INTERFACE
    } IHPScnMgr3Vtbl;

    interface IHPScnMgr3
    {
        CONST_VTBL struct IHPScnMgr3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IHPScnMgr3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IHPScnMgr3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IHPScnMgr3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IHPScnMgr3_GetCurrentScanner(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetCurrentScanner(This,pScanDevInfo) ) 

#define IHPScnMgr3_SetCurrentScanner(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> SetCurrentScanner(This,pScanDevInfo) ) 

#define IHPScnMgr3_DlgGetScanner(This,dlgType,pScanDevInfo)	\
    ( (This)->lpVtbl -> DlgGetScanner(This,dlgType,pScanDevInfo) ) 

#define IHPScnMgr3_GetScannerByFriendlyName(This,pszFriendlyName,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByFriendlyName(This,pszFriendlyName,pScanDevInfo) ) 


#define IHPScnMgr3_GetCurrentScanner2(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetCurrentScanner2(This,pScanDevInfo) ) 

#define IHPScnMgr3_SetCurrentScanner2(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> SetCurrentScanner2(This,pScanDevInfo) ) 

#define IHPScnMgr3_DlgGetScanner2(This,dlgType,pScanDevInfo)	\
    ( (This)->lpVtbl -> DlgGetScanner2(This,dlgType,pScanDevInfo) ) 

#define IHPScnMgr3_GetScannerByFriendlyName2(This,pszFriendlyName,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByFriendlyName2(This,pszFriendlyName,pScanDevInfo) ) 

#define IHPScnMgr3_GetScannerByIndex(This,pszIndex,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByIndex(This,pszIndex,pScanDevInfo) ) 

#define IHPScnMgr3_GetCurrentScannerInterface(This,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetCurrentScannerInterface(This,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr3_GetScannerInterfaceByIndex(This,pszIndex,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetScannerInterfaceByIndex(This,pszIndex,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr3_DlgGetScannerInterface(This,dlgType,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> DlgGetScannerInterface(This,dlgType,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr3_GetScannerInterfaceByFriendlyName(This,pszIndex,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetScannerInterfaceByFriendlyName(This,pszIndex,pScanDevInfoEx,ppScanner) ) 


#define IHPScnMgr3_EnumerateScanners(This,pwCount,ppScanDevInfo)	\
    ( (This)->lpVtbl -> EnumerateScanners(This,pwCount,ppScanDevInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IHPScnMgr3_INTERFACE_DEFINED__ */


#ifndef __IHPScnMgr4_INTERFACE_DEFINED__
#define __IHPScnMgr4_INTERFACE_DEFINED__

/* interface IHPScnMgr4 */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IHPScnMgr4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("51EBFEBD-8DC1-4C27-BD27-8959A5FA6888")
    IHPScnMgr4 : public IHPScnMgr3
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetContextIDByFriendlyName( 
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [string][out] */ LPOLESTR *ppszContextID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetContextIDByIndex( 
            /* [in] */ LPOLESTR pszIndex,
            /* [string][out] */ LPOLESTR *ppszContextID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetContextIDBySymbolicLink( 
            /* [in] */ LPOLESTR pszDevName,
            /* [string][out] */ LPOLESTR *ppszContextID) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IHPScnMgr4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IHPScnMgr4 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IHPScnMgr4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IHPScnMgr4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScanner )( 
            IHPScnMgr4 * This,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetCurrentScanner )( 
            IHPScnMgr4 * This,
            /* [in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScanner )( 
            IHPScnMgr4 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByFriendlyName )( 
            IHPScnMgr4 * This,
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScanner2 )( 
            IHPScnMgr4 * This,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetCurrentScanner2 )( 
            IHPScnMgr4 * This,
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScanner2 )( 
            IHPScnMgr4 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByFriendlyName2 )( 
            IHPScnMgr4 * This,
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByIndex )( 
            IHPScnMgr4 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScannerInterface )( 
            IHPScnMgr4 * This,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerInterfaceByIndex )( 
            IHPScnMgr4 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScannerInterface )( 
            IHPScnMgr4 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerInterfaceByFriendlyName )( 
            IHPScnMgr4 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *EnumerateScanners )( 
            IHPScnMgr4 * This,
            /* [out] */ WORD *pwCount,
            /* [size_is][size_is][out] */ PSCAN_DEV_INFO_EX *ppScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextIDByFriendlyName )( 
            IHPScnMgr4 * This,
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [string][out] */ LPOLESTR *ppszContextID);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextIDByIndex )( 
            IHPScnMgr4 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [string][out] */ LPOLESTR *ppszContextID);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextIDBySymbolicLink )( 
            IHPScnMgr4 * This,
            /* [in] */ LPOLESTR pszDevName,
            /* [string][out] */ LPOLESTR *ppszContextID);
        
        END_INTERFACE
    } IHPScnMgr4Vtbl;

    interface IHPScnMgr4
    {
        CONST_VTBL struct IHPScnMgr4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IHPScnMgr4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IHPScnMgr4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IHPScnMgr4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IHPScnMgr4_GetCurrentScanner(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetCurrentScanner(This,pScanDevInfo) ) 

#define IHPScnMgr4_SetCurrentScanner(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> SetCurrentScanner(This,pScanDevInfo) ) 

#define IHPScnMgr4_DlgGetScanner(This,dlgType,pScanDevInfo)	\
    ( (This)->lpVtbl -> DlgGetScanner(This,dlgType,pScanDevInfo) ) 

#define IHPScnMgr4_GetScannerByFriendlyName(This,pszFriendlyName,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByFriendlyName(This,pszFriendlyName,pScanDevInfo) ) 


#define IHPScnMgr4_GetCurrentScanner2(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetCurrentScanner2(This,pScanDevInfo) ) 

#define IHPScnMgr4_SetCurrentScanner2(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> SetCurrentScanner2(This,pScanDevInfo) ) 

#define IHPScnMgr4_DlgGetScanner2(This,dlgType,pScanDevInfo)	\
    ( (This)->lpVtbl -> DlgGetScanner2(This,dlgType,pScanDevInfo) ) 

#define IHPScnMgr4_GetScannerByFriendlyName2(This,pszFriendlyName,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByFriendlyName2(This,pszFriendlyName,pScanDevInfo) ) 

#define IHPScnMgr4_GetScannerByIndex(This,pszIndex,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByIndex(This,pszIndex,pScanDevInfo) ) 

#define IHPScnMgr4_GetCurrentScannerInterface(This,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetCurrentScannerInterface(This,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr4_GetScannerInterfaceByIndex(This,pszIndex,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetScannerInterfaceByIndex(This,pszIndex,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr4_DlgGetScannerInterface(This,dlgType,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> DlgGetScannerInterface(This,dlgType,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr4_GetScannerInterfaceByFriendlyName(This,pszIndex,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetScannerInterfaceByFriendlyName(This,pszIndex,pScanDevInfoEx,ppScanner) ) 


#define IHPScnMgr4_EnumerateScanners(This,pwCount,ppScanDevInfo)	\
    ( (This)->lpVtbl -> EnumerateScanners(This,pwCount,ppScanDevInfo) ) 


#define IHPScnMgr4_GetContextIDByFriendlyName(This,pszFriendlyName,ppszContextID)	\
    ( (This)->lpVtbl -> GetContextIDByFriendlyName(This,pszFriendlyName,ppszContextID) ) 

#define IHPScnMgr4_GetContextIDByIndex(This,pszIndex,ppszContextID)	\
    ( (This)->lpVtbl -> GetContextIDByIndex(This,pszIndex,ppszContextID) ) 

#define IHPScnMgr4_GetContextIDBySymbolicLink(This,pszDevName,ppszContextID)	\
    ( (This)->lpVtbl -> GetContextIDBySymbolicLink(This,pszDevName,ppszContextID) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IHPScnMgr4_INTERFACE_DEFINED__ */


#ifndef __IHPScnMgr5_INTERFACE_DEFINED__
#define __IHPScnMgr5_INTERFACE_DEFINED__

/* interface IHPScnMgr5 */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IHPScnMgr5;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("00063F10-98C8-4C5A-9E3D-1A1AD2FA076A")
    IHPScnMgr5 : public IHPScnMgr4
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetExactModelString( 
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo,
            /* [string][out] */ LPOLESTR *ppszModelString) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IHPScnMgr5Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IHPScnMgr5 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IHPScnMgr5 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IHPScnMgr5 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScanner )( 
            IHPScnMgr5 * This,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetCurrentScanner )( 
            IHPScnMgr5 * This,
            /* [in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScanner )( 
            IHPScnMgr5 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByFriendlyName )( 
            IHPScnMgr5 * This,
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScanner2 )( 
            IHPScnMgr5 * This,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetCurrentScanner2 )( 
            IHPScnMgr5 * This,
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScanner2 )( 
            IHPScnMgr5 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByFriendlyName2 )( 
            IHPScnMgr5 * This,
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByIndex )( 
            IHPScnMgr5 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScannerInterface )( 
            IHPScnMgr5 * This,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerInterfaceByIndex )( 
            IHPScnMgr5 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScannerInterface )( 
            IHPScnMgr5 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerInterfaceByFriendlyName )( 
            IHPScnMgr5 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *EnumerateScanners )( 
            IHPScnMgr5 * This,
            /* [out] */ WORD *pwCount,
            /* [size_is][size_is][out] */ PSCAN_DEV_INFO_EX *ppScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextIDByFriendlyName )( 
            IHPScnMgr5 * This,
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [string][out] */ LPOLESTR *ppszContextID);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextIDByIndex )( 
            IHPScnMgr5 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [string][out] */ LPOLESTR *ppszContextID);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextIDBySymbolicLink )( 
            IHPScnMgr5 * This,
            /* [in] */ LPOLESTR pszDevName,
            /* [string][out] */ LPOLESTR *ppszContextID);
        
        HRESULT ( STDMETHODCALLTYPE *GetExactModelString )( 
            IHPScnMgr5 * This,
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo,
            /* [string][out] */ LPOLESTR *ppszModelString);
        
        END_INTERFACE
    } IHPScnMgr5Vtbl;

    interface IHPScnMgr5
    {
        CONST_VTBL struct IHPScnMgr5Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IHPScnMgr5_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IHPScnMgr5_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IHPScnMgr5_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IHPScnMgr5_GetCurrentScanner(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetCurrentScanner(This,pScanDevInfo) ) 

#define IHPScnMgr5_SetCurrentScanner(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> SetCurrentScanner(This,pScanDevInfo) ) 

#define IHPScnMgr5_DlgGetScanner(This,dlgType,pScanDevInfo)	\
    ( (This)->lpVtbl -> DlgGetScanner(This,dlgType,pScanDevInfo) ) 

#define IHPScnMgr5_GetScannerByFriendlyName(This,pszFriendlyName,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByFriendlyName(This,pszFriendlyName,pScanDevInfo) ) 


#define IHPScnMgr5_GetCurrentScanner2(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetCurrentScanner2(This,pScanDevInfo) ) 

#define IHPScnMgr5_SetCurrentScanner2(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> SetCurrentScanner2(This,pScanDevInfo) ) 

#define IHPScnMgr5_DlgGetScanner2(This,dlgType,pScanDevInfo)	\
    ( (This)->lpVtbl -> DlgGetScanner2(This,dlgType,pScanDevInfo) ) 

#define IHPScnMgr5_GetScannerByFriendlyName2(This,pszFriendlyName,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByFriendlyName2(This,pszFriendlyName,pScanDevInfo) ) 

#define IHPScnMgr5_GetScannerByIndex(This,pszIndex,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByIndex(This,pszIndex,pScanDevInfo) ) 

#define IHPScnMgr5_GetCurrentScannerInterface(This,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetCurrentScannerInterface(This,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr5_GetScannerInterfaceByIndex(This,pszIndex,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetScannerInterfaceByIndex(This,pszIndex,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr5_DlgGetScannerInterface(This,dlgType,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> DlgGetScannerInterface(This,dlgType,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr5_GetScannerInterfaceByFriendlyName(This,pszIndex,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetScannerInterfaceByFriendlyName(This,pszIndex,pScanDevInfoEx,ppScanner) ) 


#define IHPScnMgr5_EnumerateScanners(This,pwCount,ppScanDevInfo)	\
    ( (This)->lpVtbl -> EnumerateScanners(This,pwCount,ppScanDevInfo) ) 


#define IHPScnMgr5_GetContextIDByFriendlyName(This,pszFriendlyName,ppszContextID)	\
    ( (This)->lpVtbl -> GetContextIDByFriendlyName(This,pszFriendlyName,ppszContextID) ) 

#define IHPScnMgr5_GetContextIDByIndex(This,pszIndex,ppszContextID)	\
    ( (This)->lpVtbl -> GetContextIDByIndex(This,pszIndex,ppszContextID) ) 

#define IHPScnMgr5_GetContextIDBySymbolicLink(This,pszDevName,ppszContextID)	\
    ( (This)->lpVtbl -> GetContextIDBySymbolicLink(This,pszDevName,ppszContextID) ) 


#define IHPScnMgr5_GetExactModelString(This,pScanDevInfo,ppszModelString)	\
    ( (This)->lpVtbl -> GetExactModelString(This,pScanDevInfo,ppszModelString) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IHPScnMgr5_INTERFACE_DEFINED__ */


#ifndef __IHPScnMgr6_INTERFACE_DEFINED__
#define __IHPScnMgr6_INTERFACE_DEFINED__

/* interface IHPScnMgr6 */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IHPScnMgr6;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("61E3ABA1-B565-4B6D-9C1A-575489F9A6AD")
    IHPScnMgr6 : public IHPScnMgr5
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetEdnaCLSID( 
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo,
            /* [out] */ CLSID *pCLSIDEdna) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetScannerBySN( 
            /* [in] */ LPOLESTR pszSN,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo,
            /* [out] */ BYTE *pbyPresent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetScannerInterfaceBySN( 
            /* [in] */ LPOLESTR pszSN,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDeviceDataString( 
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo,
            /* [string][in] */ BYTE *pbyRegValue,
            /* [string][out] */ LPOLESTR *ppszRegString) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetDeviceDataNum( 
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo,
            /* [string][in] */ BYTE *pbyRegValue,
            /* [out] */ DWORD *pdwRegNum) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IHPScnMgr6Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IHPScnMgr6 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IHPScnMgr6 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IHPScnMgr6 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScanner )( 
            IHPScnMgr6 * This,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetCurrentScanner )( 
            IHPScnMgr6 * This,
            /* [in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScanner )( 
            IHPScnMgr6 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByFriendlyName )( 
            IHPScnMgr6 * This,
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScanner2 )( 
            IHPScnMgr6 * This,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetCurrentScanner2 )( 
            IHPScnMgr6 * This,
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScanner2 )( 
            IHPScnMgr6 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByFriendlyName2 )( 
            IHPScnMgr6 * This,
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByIndex )( 
            IHPScnMgr6 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScannerInterface )( 
            IHPScnMgr6 * This,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerInterfaceByIndex )( 
            IHPScnMgr6 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScannerInterface )( 
            IHPScnMgr6 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerInterfaceByFriendlyName )( 
            IHPScnMgr6 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *EnumerateScanners )( 
            IHPScnMgr6 * This,
            /* [out] */ WORD *pwCount,
            /* [size_is][size_is][out] */ PSCAN_DEV_INFO_EX *ppScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextIDByFriendlyName )( 
            IHPScnMgr6 * This,
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [string][out] */ LPOLESTR *ppszContextID);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextIDByIndex )( 
            IHPScnMgr6 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [string][out] */ LPOLESTR *ppszContextID);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextIDBySymbolicLink )( 
            IHPScnMgr6 * This,
            /* [in] */ LPOLESTR pszDevName,
            /* [string][out] */ LPOLESTR *ppszContextID);
        
        HRESULT ( STDMETHODCALLTYPE *GetExactModelString )( 
            IHPScnMgr6 * This,
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo,
            /* [string][out] */ LPOLESTR *ppszModelString);
        
        HRESULT ( STDMETHODCALLTYPE *GetEdnaCLSID )( 
            IHPScnMgr6 * This,
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo,
            /* [out] */ CLSID *pCLSIDEdna);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerBySN )( 
            IHPScnMgr6 * This,
            /* [in] */ LPOLESTR pszSN,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo,
            /* [out] */ BYTE *pbyPresent);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerInterfaceBySN )( 
            IHPScnMgr6 * This,
            /* [in] */ LPOLESTR pszSN,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *GetDeviceDataString )( 
            IHPScnMgr6 * This,
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo,
            /* [string][in] */ BYTE *pbyRegValue,
            /* [string][out] */ LPOLESTR *ppszRegString);
        
        HRESULT ( STDMETHODCALLTYPE *GetDeviceDataNum )( 
            IHPScnMgr6 * This,
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo,
            /* [string][in] */ BYTE *pbyRegValue,
            /* [out] */ DWORD *pdwRegNum);
        
        END_INTERFACE
    } IHPScnMgr6Vtbl;

    interface IHPScnMgr6
    {
        CONST_VTBL struct IHPScnMgr6Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IHPScnMgr6_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IHPScnMgr6_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IHPScnMgr6_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IHPScnMgr6_GetCurrentScanner(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetCurrentScanner(This,pScanDevInfo) ) 

#define IHPScnMgr6_SetCurrentScanner(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> SetCurrentScanner(This,pScanDevInfo) ) 

#define IHPScnMgr6_DlgGetScanner(This,dlgType,pScanDevInfo)	\
    ( (This)->lpVtbl -> DlgGetScanner(This,dlgType,pScanDevInfo) ) 

#define IHPScnMgr6_GetScannerByFriendlyName(This,pszFriendlyName,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByFriendlyName(This,pszFriendlyName,pScanDevInfo) ) 


#define IHPScnMgr6_GetCurrentScanner2(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetCurrentScanner2(This,pScanDevInfo) ) 

#define IHPScnMgr6_SetCurrentScanner2(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> SetCurrentScanner2(This,pScanDevInfo) ) 

#define IHPScnMgr6_DlgGetScanner2(This,dlgType,pScanDevInfo)	\
    ( (This)->lpVtbl -> DlgGetScanner2(This,dlgType,pScanDevInfo) ) 

#define IHPScnMgr6_GetScannerByFriendlyName2(This,pszFriendlyName,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByFriendlyName2(This,pszFriendlyName,pScanDevInfo) ) 

#define IHPScnMgr6_GetScannerByIndex(This,pszIndex,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByIndex(This,pszIndex,pScanDevInfo) ) 

#define IHPScnMgr6_GetCurrentScannerInterface(This,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetCurrentScannerInterface(This,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr6_GetScannerInterfaceByIndex(This,pszIndex,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetScannerInterfaceByIndex(This,pszIndex,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr6_DlgGetScannerInterface(This,dlgType,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> DlgGetScannerInterface(This,dlgType,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr6_GetScannerInterfaceByFriendlyName(This,pszIndex,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetScannerInterfaceByFriendlyName(This,pszIndex,pScanDevInfoEx,ppScanner) ) 


#define IHPScnMgr6_EnumerateScanners(This,pwCount,ppScanDevInfo)	\
    ( (This)->lpVtbl -> EnumerateScanners(This,pwCount,ppScanDevInfo) ) 


#define IHPScnMgr6_GetContextIDByFriendlyName(This,pszFriendlyName,ppszContextID)	\
    ( (This)->lpVtbl -> GetContextIDByFriendlyName(This,pszFriendlyName,ppszContextID) ) 

#define IHPScnMgr6_GetContextIDByIndex(This,pszIndex,ppszContextID)	\
    ( (This)->lpVtbl -> GetContextIDByIndex(This,pszIndex,ppszContextID) ) 

#define IHPScnMgr6_GetContextIDBySymbolicLink(This,pszDevName,ppszContextID)	\
    ( (This)->lpVtbl -> GetContextIDBySymbolicLink(This,pszDevName,ppszContextID) ) 


#define IHPScnMgr6_GetExactModelString(This,pScanDevInfo,ppszModelString)	\
    ( (This)->lpVtbl -> GetExactModelString(This,pScanDevInfo,ppszModelString) ) 


#define IHPScnMgr6_GetEdnaCLSID(This,pScanDevInfo,pCLSIDEdna)	\
    ( (This)->lpVtbl -> GetEdnaCLSID(This,pScanDevInfo,pCLSIDEdna) ) 

#define IHPScnMgr6_GetScannerBySN(This,pszSN,pScanDevInfo,pbyPresent)	\
    ( (This)->lpVtbl -> GetScannerBySN(This,pszSN,pScanDevInfo,pbyPresent) ) 

#define IHPScnMgr6_GetScannerInterfaceBySN(This,pszSN,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetScannerInterfaceBySN(This,pszSN,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr6_GetDeviceDataString(This,pScanDevInfo,pbyRegValue,ppszRegString)	\
    ( (This)->lpVtbl -> GetDeviceDataString(This,pScanDevInfo,pbyRegValue,ppszRegString) ) 

#define IHPScnMgr6_GetDeviceDataNum(This,pScanDevInfo,pbyRegValue,pdwRegNum)	\
    ( (This)->lpVtbl -> GetDeviceDataNum(This,pScanDevInfo,pbyRegValue,pdwRegNum) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IHPScnMgr6_INTERFACE_DEFINED__ */


#ifndef __IHPScnMgr7_INTERFACE_DEFINED__
#define __IHPScnMgr7_INTERFACE_DEFINED__

/* interface IHPScnMgr7 */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IHPScnMgr7;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("0D1BF678-2352-4d42-9820-E2BC6D54C3F7")
    IHPScnMgr7 : public IHPScnMgr6
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnumerateScanners2( 
            /* [in] */ BOOL bPresent,
            /* [out] */ WORD *pwCount,
            /* [size_is][size_is][out] */ PSCAN_DEV_INFO_EX *ppScanDevInfo) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IHPScnMgr7Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IHPScnMgr7 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IHPScnMgr7 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IHPScnMgr7 * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScanner )( 
            IHPScnMgr7 * This,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetCurrentScanner )( 
            IHPScnMgr7 * This,
            /* [in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScanner )( 
            IHPScnMgr7 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByFriendlyName )( 
            IHPScnMgr7 * This,
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [out][in] */ SCAN_DEV_INFO *pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScanner2 )( 
            IHPScnMgr7 * This,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetCurrentScanner2 )( 
            IHPScnMgr7 * This,
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScanner2 )( 
            IHPScnMgr7 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByFriendlyName2 )( 
            IHPScnMgr7 * This,
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerByIndex )( 
            IHPScnMgr7 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetCurrentScannerInterface )( 
            IHPScnMgr7 * This,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerInterfaceByIndex )( 
            IHPScnMgr7 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *DlgGetScannerInterface )( 
            IHPScnMgr7 * This,
            /* [in] */ WORD dlgType,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerInterfaceByFriendlyName )( 
            IHPScnMgr7 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *EnumerateScanners )( 
            IHPScnMgr7 * This,
            /* [out] */ WORD *pwCount,
            /* [size_is][size_is][out] */ PSCAN_DEV_INFO_EX *ppScanDevInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextIDByFriendlyName )( 
            IHPScnMgr7 * This,
            /* [in] */ LPOLESTR pszFriendlyName,
            /* [string][out] */ LPOLESTR *ppszContextID);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextIDByIndex )( 
            IHPScnMgr7 * This,
            /* [in] */ LPOLESTR pszIndex,
            /* [string][out] */ LPOLESTR *ppszContextID);
        
        HRESULT ( STDMETHODCALLTYPE *GetContextIDBySymbolicLink )( 
            IHPScnMgr7 * This,
            /* [in] */ LPOLESTR pszDevName,
            /* [string][out] */ LPOLESTR *ppszContextID);
        
        HRESULT ( STDMETHODCALLTYPE *GetExactModelString )( 
            IHPScnMgr7 * This,
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo,
            /* [string][out] */ LPOLESTR *ppszModelString);
        
        HRESULT ( STDMETHODCALLTYPE *GetEdnaCLSID )( 
            IHPScnMgr7 * This,
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo,
            /* [out] */ CLSID *pCLSIDEdna);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerBySN )( 
            IHPScnMgr7 * This,
            /* [in] */ LPOLESTR pszSN,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfo,
            /* [out] */ BYTE *pbyPresent);
        
        HRESULT ( STDMETHODCALLTYPE *GetScannerInterfaceBySN )( 
            IHPScnMgr7 * This,
            /* [in] */ LPOLESTR pszSN,
            /* [out][in] */ PSCAN_DEV_INFO_EX pScanDevInfoEx,
            /* [out] */ IScanner **ppScanner);
        
        HRESULT ( STDMETHODCALLTYPE *GetDeviceDataString )( 
            IHPScnMgr7 * This,
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo,
            /* [string][in] */ BYTE *pbyRegValue,
            /* [string][out] */ LPOLESTR *ppszRegString);
        
        HRESULT ( STDMETHODCALLTYPE *GetDeviceDataNum )( 
            IHPScnMgr7 * This,
            /* [in] */ PSCAN_DEV_INFO_EX pScanDevInfo,
            /* [string][in] */ BYTE *pbyRegValue,
            /* [out] */ DWORD *pdwRegNum);
        
        HRESULT ( STDMETHODCALLTYPE *EnumerateScanners2 )( 
            IHPScnMgr7 * This,
            /* [in] */ BOOL bPresent,
            /* [out] */ WORD *pwCount,
            /* [size_is][size_is][out] */ PSCAN_DEV_INFO_EX *ppScanDevInfo);
        
        END_INTERFACE
    } IHPScnMgr7Vtbl;

    interface IHPScnMgr7
    {
        CONST_VTBL struct IHPScnMgr7Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IHPScnMgr7_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IHPScnMgr7_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IHPScnMgr7_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IHPScnMgr7_GetCurrentScanner(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetCurrentScanner(This,pScanDevInfo) ) 

#define IHPScnMgr7_SetCurrentScanner(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> SetCurrentScanner(This,pScanDevInfo) ) 

#define IHPScnMgr7_DlgGetScanner(This,dlgType,pScanDevInfo)	\
    ( (This)->lpVtbl -> DlgGetScanner(This,dlgType,pScanDevInfo) ) 

#define IHPScnMgr7_GetScannerByFriendlyName(This,pszFriendlyName,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByFriendlyName(This,pszFriendlyName,pScanDevInfo) ) 


#define IHPScnMgr7_GetCurrentScanner2(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetCurrentScanner2(This,pScanDevInfo) ) 

#define IHPScnMgr7_SetCurrentScanner2(This,pScanDevInfo)	\
    ( (This)->lpVtbl -> SetCurrentScanner2(This,pScanDevInfo) ) 

#define IHPScnMgr7_DlgGetScanner2(This,dlgType,pScanDevInfo)	\
    ( (This)->lpVtbl -> DlgGetScanner2(This,dlgType,pScanDevInfo) ) 

#define IHPScnMgr7_GetScannerByFriendlyName2(This,pszFriendlyName,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByFriendlyName2(This,pszFriendlyName,pScanDevInfo) ) 

#define IHPScnMgr7_GetScannerByIndex(This,pszIndex,pScanDevInfo)	\
    ( (This)->lpVtbl -> GetScannerByIndex(This,pszIndex,pScanDevInfo) ) 

#define IHPScnMgr7_GetCurrentScannerInterface(This,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetCurrentScannerInterface(This,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr7_GetScannerInterfaceByIndex(This,pszIndex,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetScannerInterfaceByIndex(This,pszIndex,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr7_DlgGetScannerInterface(This,dlgType,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> DlgGetScannerInterface(This,dlgType,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr7_GetScannerInterfaceByFriendlyName(This,pszIndex,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetScannerInterfaceByFriendlyName(This,pszIndex,pScanDevInfoEx,ppScanner) ) 


#define IHPScnMgr7_EnumerateScanners(This,pwCount,ppScanDevInfo)	\
    ( (This)->lpVtbl -> EnumerateScanners(This,pwCount,ppScanDevInfo) ) 


#define IHPScnMgr7_GetContextIDByFriendlyName(This,pszFriendlyName,ppszContextID)	\
    ( (This)->lpVtbl -> GetContextIDByFriendlyName(This,pszFriendlyName,ppszContextID) ) 

#define IHPScnMgr7_GetContextIDByIndex(This,pszIndex,ppszContextID)	\
    ( (This)->lpVtbl -> GetContextIDByIndex(This,pszIndex,ppszContextID) ) 

#define IHPScnMgr7_GetContextIDBySymbolicLink(This,pszDevName,ppszContextID)	\
    ( (This)->lpVtbl -> GetContextIDBySymbolicLink(This,pszDevName,ppszContextID) ) 


#define IHPScnMgr7_GetExactModelString(This,pScanDevInfo,ppszModelString)	\
    ( (This)->lpVtbl -> GetExactModelString(This,pScanDevInfo,ppszModelString) ) 


#define IHPScnMgr7_GetEdnaCLSID(This,pScanDevInfo,pCLSIDEdna)	\
    ( (This)->lpVtbl -> GetEdnaCLSID(This,pScanDevInfo,pCLSIDEdna) ) 

#define IHPScnMgr7_GetScannerBySN(This,pszSN,pScanDevInfo,pbyPresent)	\
    ( (This)->lpVtbl -> GetScannerBySN(This,pszSN,pScanDevInfo,pbyPresent) ) 

#define IHPScnMgr7_GetScannerInterfaceBySN(This,pszSN,pScanDevInfoEx,ppScanner)	\
    ( (This)->lpVtbl -> GetScannerInterfaceBySN(This,pszSN,pScanDevInfoEx,ppScanner) ) 

#define IHPScnMgr7_GetDeviceDataString(This,pScanDevInfo,pbyRegValue,ppszRegString)	\
    ( (This)->lpVtbl -> GetDeviceDataString(This,pScanDevInfo,pbyRegValue,ppszRegString) ) 

#define IHPScnMgr7_GetDeviceDataNum(This,pScanDevInfo,pbyRegValue,pdwRegNum)	\
    ( (This)->lpVtbl -> GetDeviceDataNum(This,pScanDevInfo,pbyRegValue,pdwRegNum) ) 


#define IHPScnMgr7_EnumerateScanners2(This,bPresent,pwCount,ppScanDevInfo)	\
    ( (This)->lpVtbl -> EnumerateScanners2(This,bPresent,pwCount,ppScanDevInfo) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IHPScnMgr7_INTERFACE_DEFINED__ */


#ifndef __IHPRUMgr_INTERFACE_DEFINED__
#define __IHPRUMgr_INTERFACE_DEFINED__

/* interface IHPRUMgr */
/* [unique][helpstring][nonextensible][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_IHPRUMgr;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("FBDBD3C3-2F19-409e-A67A-06683C344595")
    IHPRUMgr : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE EnumRUDevices( 
            /* [in] */ BOOL bPresent,
            /* [out] */ WORD *pwCount,
            /* [size_is][size_is][out] */ HPRU_DEV_INFO **ppRUDI) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IHPRUMgrVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IHPRUMgr * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IHPRUMgr * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IHPRUMgr * This);
        
        HRESULT ( STDMETHODCALLTYPE *EnumRUDevices )( 
            IHPRUMgr * This,
            /* [in] */ BOOL bPresent,
            /* [out] */ WORD *pwCount,
            /* [size_is][size_is][out] */ HPRU_DEV_INFO **ppRUDI);
        
        END_INTERFACE
    } IHPRUMgrVtbl;

    interface IHPRUMgr
    {
        CONST_VTBL struct IHPRUMgrVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IHPRUMgr_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IHPRUMgr_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IHPRUMgr_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IHPRUMgr_EnumRUDevices(This,bPresent,pwCount,ppRUDI)	\
    ( (This)->lpVtbl -> EnumRUDevices(This,bPresent,pwCount,ppRUDI) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IHPRUMgr_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0051 */
/* [local] */ 




extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0051_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0051_v0_0_s_ifspec;

#ifndef __IPutLog_INTERFACE_DEFINED__
#define __IPutLog_INTERFACE_DEFINED__

/* interface IPutLog */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IPutLog;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E32B0B26-0EDA-4E0A-9EB0-4BF4CC60842F")
    IPutLog : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE RegisterCaller( 
            /* [string][in] */ LPOLESTR pszCaller,
            /* [out] */ DWORD *pdwCallerID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE LogString( 
            /* [in] */ DWORD dwCallerID,
            /* [size_is][in] */ BYTE *pbyString,
            /* [in] */ DWORD dwSize) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IPutLogVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IPutLog * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IPutLog * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IPutLog * This);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterCaller )( 
            IPutLog * This,
            /* [string][in] */ LPOLESTR pszCaller,
            /* [out] */ DWORD *pdwCallerID);
        
        HRESULT ( STDMETHODCALLTYPE *LogString )( 
            IPutLog * This,
            /* [in] */ DWORD dwCallerID,
            /* [size_is][in] */ BYTE *pbyString,
            /* [in] */ DWORD dwSize);
        
        END_INTERFACE
    } IPutLogVtbl;

    interface IPutLog
    {
        CONST_VTBL struct IPutLogVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IPutLog_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IPutLog_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IPutLog_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IPutLog_RegisterCaller(This,pszCaller,pdwCallerID)	\
    ( (This)->lpVtbl -> RegisterCaller(This,pszCaller,pdwCallerID) ) 

#define IPutLog_LogString(This,dwCallerID,pbyString,dwSize)	\
    ( (This)->lpVtbl -> LogString(This,dwCallerID,pbyString,dwSize) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IPutLog_INTERFACE_DEFINED__ */


#ifndef __IGetLog_INTERFACE_DEFINED__
#define __IGetLog_INTERFACE_DEFINED__

/* interface IGetLog */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_IGetLog;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E5063F39-C1DB-4bbb-89CE-11610FABE72D")
    IGetLog : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE HasData( 
            /* [out] */ BYTE *pbyHasData) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBuffer( 
            /* [out] */ BYTE *ppBuffer,
            /* [out] */ DWORD *pdwBytes) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IGetLogVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IGetLog * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IGetLog * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IGetLog * This);
        
        HRESULT ( STDMETHODCALLTYPE *HasData )( 
            IGetLog * This,
            /* [out] */ BYTE *pbyHasData);
        
        HRESULT ( STDMETHODCALLTYPE *GetBuffer )( 
            IGetLog * This,
            /* [out] */ BYTE *ppBuffer,
            /* [out] */ DWORD *pdwBytes);
        
        END_INTERFACE
    } IGetLogVtbl;

    interface IGetLog
    {
        CONST_VTBL struct IGetLogVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IGetLog_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IGetLog_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IGetLog_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IGetLog_HasData(This,pbyHasData)	\
    ( (This)->lpVtbl -> HasData(This,pbyHasData) ) 

#define IGetLog_GetBuffer(This,ppBuffer,pdwBytes)	\
    ( (This)->lpVtbl -> GetBuffer(This,ppBuffer,pdwBytes) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IGetLog_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0053 */
/* [local] */ 


struct STI_EVENT_INFO
    {
    GUID guid;
    DWORD index;
    WCHAR szName[ 256 ];
    } ;


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0053_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0053_v0_0_s_ifspec;

#ifndef __IStiHandler_INTERFACE_DEFINED__
#define __IStiHandler_INTERFACE_DEFINED__

/* interface IStiHandler */
/* [unique][helpstring][local][uuid][object] */ 


EXTERN_C const IID IID_IStiHandler;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7BB06C8F-677E-4960-ADF2-618849817E70")
    IStiHandler : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE StiInitialize( 
            /* [in] */ LPOLESTR pszDevName,
            /* [in] */ DWORD dwDevIOType,
            /* [in] */ DWORD dwStiCapabilities,
            /* [in] */ HKEY hParametersKey,
            /* [retval][out] */ DWORD *pdwErrorCode) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetNumStiEvents( 
            /* [out] */ DWORD *pdwNumEvents) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetStiEventInfo( 
            /* [in] */ DWORD dwEventIndex,
            /* [out] */ struct STI_EVENT_INFO *pEventInfo) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetStiEventHandle( 
            /* [in] */ HANDLE hEvent) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetStiEventGuid( 
            /* [out] */ GUID *pEventGUID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE EnableStiEventMonitoring( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DisableStiEventMonitoring( 
            /* [in] */ BOOL IsDeviceDisconnected) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IStiHandlerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IStiHandler * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IStiHandler * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IStiHandler * This);
        
        HRESULT ( STDMETHODCALLTYPE *StiInitialize )( 
            IStiHandler * This,
            /* [in] */ LPOLESTR pszDevName,
            /* [in] */ DWORD dwDevIOType,
            /* [in] */ DWORD dwStiCapabilities,
            /* [in] */ HKEY hParametersKey,
            /* [retval][out] */ DWORD *pdwErrorCode);
        
        HRESULT ( STDMETHODCALLTYPE *GetNumStiEvents )( 
            IStiHandler * This,
            /* [out] */ DWORD *pdwNumEvents);
        
        HRESULT ( STDMETHODCALLTYPE *GetStiEventInfo )( 
            IStiHandler * This,
            /* [in] */ DWORD dwEventIndex,
            /* [out] */ struct STI_EVENT_INFO *pEventInfo);
        
        HRESULT ( STDMETHODCALLTYPE *SetStiEventHandle )( 
            IStiHandler * This,
            /* [in] */ HANDLE hEvent);
        
        HRESULT ( STDMETHODCALLTYPE *GetStiEventGuid )( 
            IStiHandler * This,
            /* [out] */ GUID *pEventGUID);
        
        HRESULT ( STDMETHODCALLTYPE *EnableStiEventMonitoring )( 
            IStiHandler * This);
        
        HRESULT ( STDMETHODCALLTYPE *DisableStiEventMonitoring )( 
            IStiHandler * This,
            /* [in] */ BOOL IsDeviceDisconnected);
        
        END_INTERFACE
    } IStiHandlerVtbl;

    interface IStiHandler
    {
        CONST_VTBL struct IStiHandlerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IStiHandler_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IStiHandler_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IStiHandler_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IStiHandler_StiInitialize(This,pszDevName,dwDevIOType,dwStiCapabilities,hParametersKey,pdwErrorCode)	\
    ( (This)->lpVtbl -> StiInitialize(This,pszDevName,dwDevIOType,dwStiCapabilities,hParametersKey,pdwErrorCode) ) 

#define IStiHandler_GetNumStiEvents(This,pdwNumEvents)	\
    ( (This)->lpVtbl -> GetNumStiEvents(This,pdwNumEvents) ) 

#define IStiHandler_GetStiEventInfo(This,dwEventIndex,pEventInfo)	\
    ( (This)->lpVtbl -> GetStiEventInfo(This,dwEventIndex,pEventInfo) ) 

#define IStiHandler_SetStiEventHandle(This,hEvent)	\
    ( (This)->lpVtbl -> SetStiEventHandle(This,hEvent) ) 

#define IStiHandler_GetStiEventGuid(This,pEventGUID)	\
    ( (This)->lpVtbl -> GetStiEventGuid(This,pEventGUID) ) 

#define IStiHandler_EnableStiEventMonitoring(This)	\
    ( (This)->lpVtbl -> EnableStiEventMonitoring(This) ) 

#define IStiHandler_DisableStiEventMonitoring(This,IsDeviceDisconnected)	\
    ( (This)->lpVtbl -> DisableStiEventMonitoring(This,IsDeviceDisconnected) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IStiHandler_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_hpgtblues_0000_0054 */
/* [local] */ 

#ifdef TULIP_NAMESPACE
} // namespace Tulip
#endif


extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0054_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_hpgtblues_0000_0054_v0_0_s_ifspec;


#ifndef __HPGTBBLib_LIBRARY_DEFINED__
#define __HPGTBBLib_LIBRARY_DEFINED__

/* library HPGTBBLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_HPGTBBLib;

EXTERN_C const CLSID CLSID_HPGTBB;

#ifdef __cplusplus

class DECLSPEC_UUID("0F141990-2F8F-44d6-8C7C-D8755CEB302E")
HPGTBB;
#endif
#endif /* __HPGTBBLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


